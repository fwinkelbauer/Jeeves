#load "nuget:?package=cake.mug.tools"
#load "nuget:?package=cake.mug"

var target = Argument("target", "Default");
BuildParameters.Configuration = Argument("configuration", "Release");

PackageParameters.ChocolateySpecs.Add("../NuSpec/Chocolatey/Jeeves.Host.nuspec");

PackageParameters.NuGetSpecs.Add("../NuSpec/NuGet/Jeeves.Core.nuspec");

Task("Default")
    .IsDependentOn("Analyze")
    .IsDependentOn("CreatePackages")
    .Does(() =>
{
});

RunTarget(target);
