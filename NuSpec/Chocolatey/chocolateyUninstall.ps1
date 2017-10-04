$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe stop
& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe uninstall

if ($lastExitCode -ne 0)
{
  throw "Could not uninstall Jeeves. See the log file for more information"
}
