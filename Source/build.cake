#load "nuget:?package=cake.mug.tools"
#load "nuget:?package=cake.mug"

var target = Argument("target", "Default");
BuildParameters.Configuration = Argument("configuration", "Release");

Task("Default")
    .IsDependentOn("Analyze")
    .IsDependentOn("CreatePackages")
    .Does(() =>
{
});

RunTarget(target);
