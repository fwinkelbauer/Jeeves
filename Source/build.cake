#load "artifact.cake"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("Clean")
    .Does(() =>
{
    CleanArtifacts();
    CleanDirectories($"Jeeves*/bin/{configuration}");
    CleanDirectory("TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Jeeves.sln");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild("Jeeves.sln", new MSBuildSettings { Configuration = configuration, WarningsAsError = true });
    StoreBuildArtifacts("Jeeves", $"Jeeves/bin/{configuration}/**/*");
    StoreBuildArtifacts("Jeeves.Core", $"Jeeves.Core/bin/{configuration}/**/*");
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    MSTest($"*.UnitTests/bin/{configuration}/*.UnitTests.dll");
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    StoreChocolateyArtifacts("NuSpec/Chocolatey/Jeeves.nuspec");
    StoreNuGetArtifacts("NuSpec/NuGet/Jeeves.Core.nuspec");
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
{
    PublishNuGetArtifact("Jeeves.Core", "https://www.nuget.org/api/v2/package");
});

Task("Default").IsDependentOn("Pack").Does(() => { });

RunTarget(target);
