$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe stop
& $env:ChocolateyInstall\lib\Jeeves\tools\Jeeves.exe uninstall
