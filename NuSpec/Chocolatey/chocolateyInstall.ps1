$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe install
& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe start

if ($lastExitCode -ne 0)
{
  throw "Could not start the Jeeves service. See the log file for more information"
}
