namespace NanoTube.Net
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>	A helper class for turning a list of strings into a Udp packet.  </summary>
	/// <remarks>	1/19/2012. </remarks>
	public static class PacketBuilder
	{
		private static byte[] _terminator = Encoding.UTF8.GetBytes("\n");

		/// <summary>
		/// Takes a list of metric strings, separating them with newlines into a byte packet that is a maximum of 512 bytes in size.
		/// </summary>
		/// <param name="metrics">	The metrics to act on. </param>
		/// <returns>	A streamed list of byte arrays, where each array is a maximum of 512 bytes. </returns>
		public static IEnumerable<byte[]> ToMaximumBytePackets(this IEnumerable<string> metrics)
		{
			return ToMaximumBytePackets(metrics, 512);
		}

		/// <summary>
		/// Takes a list of metric strings, separating them with newlines into a byte packet of the maximum specified size.
		/// </summary>
		/// <param name="metrics">   	The metrics to act on. </param>
		/// <param name="packetSize">	Maximum size of each packet (512 bytes recommended for Udp). </param>
		/// <returns>	A streamed list of byte arrays, where each array is a maximum of 512 bytes. </returns>
		public static IEnumerable<byte[]> ToMaximumBytePackets(this IEnumerable<string> metrics, int packetSize)
		{
			List<byte> packet = new List<byte>(packetSize);

			foreach (string metric in metrics)
			{
				var bytes = Encoding.UTF8.GetBytes(metric);
				if (packet.Count + _terminator.Length + bytes.Length <= packetSize)
				{
					packet.AddRange(bytes);
					packet.AddRange(_terminator);
				}
				else if (bytes.Length >= packetSize)
				{
					yield return bytes;
				}
				else
				{
					yield return packet.ToArray();
					packet.Clear();
					packet.AddRange(bytes);
					packet.AddRange(_terminator);
				}
			}

			if (packet.Count > 0)
			{
				yield return packet.ToArray();
			}
		}
	}
}