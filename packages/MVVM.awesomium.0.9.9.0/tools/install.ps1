param($installPath, $toolsPath, $package, $project)
$file1 = $project.ProjectItems.Item("Javascript").ProjectItems.Item("knockout.js")
$file2 = $project.ProjectItems.Item("Javascript").ProjectItems.Item("Ko_Extension.min.js")
$file3 = $project.ProjectItems.Item("Javascript").ProjectItems.Item("Ko_Extension.min.js.map")
$file4 = $project.ProjectItems.Item("Javascript").ProjectItems.Item("Ko_Extension.js")

# set 'Copy To Output Directory' to 'Copy if newer'
$copyToOutput1 = $file1.Properties.Item("CopyToOutputDirectory")
$copyToOutput1.Value = 1

$copyToOutput2 = $file2.Properties.Item("CopyToOutputDirectory")
$copyToOutput2.Value = 1

$copyToOutput3 = $file3.Properties.Item("CopyToOutputDirectory")
$copyToOutput3.Value = 1
$file3.Properties.Item("BuildAction").Value = 2

$copyToOutput4 = $file4.Properties.Item("CopyToOutputDirectory")
$copyToOutput4.Value = 1