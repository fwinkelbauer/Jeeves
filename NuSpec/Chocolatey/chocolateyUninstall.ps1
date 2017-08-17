$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves.Host\tools\Jeeves.Host.exe stop
& $env:ChocolateyInstall\lib\Jeeves.Host\tools\Jeeves.Host.exe uninstall
