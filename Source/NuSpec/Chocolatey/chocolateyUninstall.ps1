$ErrorActionPreference = 'Stop'

$installFolder = (Split-Path -Parent $MyInvocation.MyCommand.Definition)

& $installFolder\Jeeves.exe stop
& $installFolder\Jeeves.exe uninstall

if ($lastExitCode -ne 0)
{
  throw "Could not uninstall Jeeves."
}
