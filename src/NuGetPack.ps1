param(
	[Parameter(Mandatory = $true, ValueFromPipeline = $true)]
	[string]
	$apiKey
)

del *.nupkg
.nuget\nuget pack NanoTube\NanoTube.csproj -Prop Configuration=Release -Exclude '**\*.CodeAnalysisLog.xml'
$package = Get-ChildItem *.nupkg | Select -First 1 -ExpandProperty Name
.nuget\nuget push $package $apiKey
del *.nupkg