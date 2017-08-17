$ErrorActionPreference = 'Stop'

& $env:ChocolateyInstall\lib\Jeeves.Host\tools\Jeeves.Host.exe install
& $env:ChocolateyInstall\lib\Jeeves.Host\tools\Jeeves.Host.exe start
