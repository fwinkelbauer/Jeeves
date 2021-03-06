#load "artifact.cake"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

ArtifactsDirectory = "../Artifacts";

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
    VSTest($"*.UnitTests/bin/{configuration}/*.UnitTests.dll", new VSTestSettings { Logger = "trx", TestAdapterPath = "." });
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    StoreChocolateyArtifact("NuSpec/Chocolatey/Jeeves.nuspec");
    StoreNuGetArtifact("NuSpec/NuGet/Jeeves.Core.nuspec");
});

Task("Publish")
    .IsDependentOn("Pack")
    .Does(() =>
{
    PublishNuGetArtifact("Jeeves.Core", "https://www.nuget.org/api/v2/package");
});

Task("Default").IsDependentOn("Pack").Does(() => { });

RunTarget(target);
