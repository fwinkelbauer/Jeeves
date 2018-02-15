$ErrorActionPreference = 'Stop'

$installFolder = (Split-Path -Parent $MyInvocation.MyCommand.Definition)

& $installFolder\Jeeves.exe install

if ($lastExitCode -ne 0)
{
  throw "Could not install Jeeves."
}
