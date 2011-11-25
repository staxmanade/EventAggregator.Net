
function Get-Last-NuGet-Version($nuGetPackageId) {
    $feeedUrl = "http://packages.nuget.org/v1/FeedService.svc/Packages()?`$filter=Id%20eq%20'$nuGetPackageId'"
    $webClient = new-object System.Net.WebClient
	$proxy = New-Object System.Net.WebProxy($global:ProxyUrl, $global:ProxyPort)
    $proxy.Credentials = (Get-Credential).GetNetworkCredential()
    $webclient.Proxy = $proxy
    $queryResults = [xml]($webClient.DownloadString($feeedUrl))
    $queryResults.feed.entry | %{ $_.properties.version } | sort-object | select -last 1
}

function Increment-Version($version){
    $parts = $version.split('.')
    for($i = $parts.length-1; $i -ge 0; $i--){
        $x = ([int]$parts[$i]) + 1
        if($i -ne 0) {
            # Don't roll the previous minor or ref past 10
            if($x -eq 10) {
                $parts[$i] = "0"
                continue
            }
        }
        $parts[$i] = $x.ToString()
        break;
    }
    [System.String]::Join(".", $parts)
}

$version = Get-Last-NuGet-Version 'EventAggregator.Net'

if(!$version){
    $version = "0.0"
}

$newVersion = Increment-Version $version

$nuget = ls .\packages\NuGet.CommandLine*\tools\NuGet.exe

$buildRoot = ".\NuGetBuild"
$eventAggregatorDestination = "$buildRoot\content\Events"
rm $buildRoot -force -recurse -ErrorAction SilentlyContinue
mkdir $eventAggregatorDestination | out-null
cp .\EventAggregator\EventAggregator.cs (Join-Path $eventAggregatorDestination "EventAggregator.cs.pp")
$eaFile = Join-Path $eventAggregatorDestination EventAggregator.cs.pp
(Get-Content $eaFile) | 
Foreach-Object { $_ -replace 'namespace EventAggregatorNet', 'namespace $rootnamespace$.Events' } | 
Set-Content $eaFile

$nuspecFile = "EventAggregator.Net.$newVersion.nuspec"
cp .\EventAggregator.Net.nuspec "$buildRoot\$nuspecFile"
pushd $buildRoot

    $nuspec = [xml](cat $nuspecFile)
    $nuspec.package.metadata.version = $newVersion
    $nuspec.Save((get-item $nuspecFile))
    
    & $nuget pack $nuspecFile

popd
