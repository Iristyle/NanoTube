![Logo](https://github.com/EastPoint/NanoTube/raw/master/logo-128.png)
# NanoTube 
A .NET based client library for publishing metrics to Graphite through a StatsD or StatSite listener, designed with performance in mind.

After [PerfTap](https://github.com/EastPoint/PerfTap) was put together, it became apparent that it would be useful to factor out the Udp messaging bits to it's own library.  On top of that, a new simple client API was cooked up that accepts simple POCOs that map to the metrics that StatSite and StatsD can accept.

Designed for compatibility with a StatsD style listener, such as:

* [StatsD](https://github.com/etsy/statsd) - The Etsy original, built with node.js
* [statsite](https://github.com/kiip/statsite) - Built with Python

## Installation

The preferred method of using the library is to install via [NuGet](http://nuget.org).

<div><p><code style="background-color: #202020; border: 4px solid #c0c0c0; border-radius: 5px; -moz-border-radius: 5px; -webkit-border-radius: 5px; box-shadow: 2px 2px 3px #6e6e6e; color: #e2e2e2; display:block; font: 1.5em 'andale mono', 'lucida console', monospace; line-height: 1.5em; overflow: auto; padding: 15px;">PM&gt; Install-Package NanoTube</code></p></div>

### Requirements

* .NET Framework 4+

## Usage

### Counter Types

* `KeyValue` - This generally speaking matches a moment in time reading of a value, although the Timestamp is optional.  StatSite has support for passing along such values, but StatsD does not.  These metrics are pushed to StatsD exactly like a Timing.
* `Timing` - Represents an elapsed amount of time, attached to a key, with no particular unit.  
* `Sample` - This is a value reading where the read frequency may be specified as a <1 value.  For instance, 1/10th is represented as 0.1.  For StatsD, this is treated as a Counter Adjustment value.  For StatSite, this is treated as a Timing.
* `Counter` - Represents adjustments made to a named server-side counter.  The adjustment value may be positive or negative.

Counter factories may be accessed through the `Metric` class.


### MetricClass - Static Examples

The `MetricClient` class has two operating modes.  There are a number of static methods for calling one-off Send, Stream or Time operations.  These methods keep a `UdpMessenger` pool for each hostname, to keep object creations low and performance high.  The static methods will generally be the simplest to use, and the highest performance means of using `NanoTube`.


#### Send
Send will materialize the entire list of Metrics passed in.  This is useful when you know you want to send all metrics together, for instance when all metrics have been read at the same time and have no timestamps attached.  Metrics will be concatenated together into the smallest number of 512 byte UDP packets as possible.

**Never use this method with an infinite Metrics enumerable.**

```csharp
MetricClient.Send("foo.bar.com", 8125, MetricFormat.StatSite, "prefix", new [] { Metric.Counter("name", 50) })
```

#### Stream
Stream will batch up the list of Metrics into 10 packet chunks by default.  This method should be used when acting as a consumer to an infinite stream of metrics data created by a producer.  The enumeration will never be materialized.

```csharp
MetricClient.Stream("foo.bar.com", 8125, MetricFormat.StatSite, "prefix", GetSomeInfiniteIEnumerableMetric())
```

#### Time
Provides a means to high-resolution time an arbitrary `Action` and push the timing result to the server.

```csharp
MetricClient.Time("foo.bar.com", 8125, MetricFormat.StatSite, "prefix", () => { Thread.Sleep(1); });
```

### MetricClass - Instance Examples

The `MetricClient` can also be constructed.  In this mode, the `UdpMessenger` pool is not used, and a new one is created that will be disposed of with the `MetricClient`.  Note that all `UdpMessenger` instances share a static `SocketAsyncEventArgs` pool.

The instance of MetricClient supports `Send` and `Stream` methods which behave identically to the static methods mentioned above.

```csharp
using (var client = new MetricClient("foo.bar.com", 8125, MetricFormat.StatSite, "prefix"))
{
	client.Send(Metric.KeyValue("name", 50, DateTime.Now));
	client.Send(new [] { Metric.Counter("name", 50) });
}
```

```csharp
using (var client = new MetricClient("foo.bar.com", 8125, MetricFormat.StatSite, "prefix"))
{
	client.Stream(GetSomeInfiniteIEnumerableMetric());
}
```

### Formatting values

* Values are always sent as `[key.]metricname` + the appropriate formatting for StatsD or StatSite based on the type of counter.  
* Key is always optional.  
* If the key or name of a metric contains a `.` then it will create additional hierarchy in Graphite as `.` is the delimiter character.
* Metric names and keys must not contain any explicitly disallowed characters.  These characters are those prohibited in Unix filesystems - `! ;:/()\\#%$^*`

### Internals

* `UdpMessenger` - this piece provides the backbone for the asynchronous socket communication and is there if you want to drop down a level.  This class also allows customization of the 10 packets per send when streaming, that `MetricClient` defaults to.
* `KeyHelper` - contains some extension methods on strings to sanitize key values.

## Implementation Details

To keep down memory utilization, and improve performance:

* Metrics are grouped together into max 512 byte packets to prevent packet fragmentation
* Asynchronous sockets are used with the [SendPacketsAsync](http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.sendpacketsasync.aspx) API in fire'n'forget fashion.  This API uses a simple object pool to cut down significantly on temporal object constructions, including IAsyncResult instances.
* MetricClient, when used with static methods, uses a 3 instance pool of `UdpMessenger` instances for each hostname / port combination.
* If no `UdpMessenger` is available in the pool when using the static methods, the current set of metrics is dropped.  Similarly, if no `SendPacketAsyncEventArgs` is available, the current bytes that have made it to the Udp send procedure are dropped.

## Future Improvements

* A raw Graphite client that doesn't go through StatsD/StatSite first
* Verification of Mono support
* Better configuration file error checking
* IP Support in addition to hostnames

## Contributing

Fork the code, and submit a pull request!  

Any useful changes are welcomed.  If you have an idea you'd like to see implemented that strays far from the simple spirit of the application, ping us first so that we're on the same page.