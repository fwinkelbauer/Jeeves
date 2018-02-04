$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe install

if ($lastExitCode -ne 0)
{
  throw "Could not install Jeeves."
}
