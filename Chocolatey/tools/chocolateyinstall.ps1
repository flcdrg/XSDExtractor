$ErrorActionPreference = 'Stop';
$toolsDir     = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"
$fileLocation = 'D:\Dev\git\XSDExtractor\XSDExtractor\bin\Release\XSDExtractor.exe'

#Based on NO DETECTION IN PRO
$packageArgs = @{
  packageName   = $env:ChocolateyPackageName
  softwareName  = 'xsdextractor2*'
  file          = $fileLocation
  fileType      = 'exe'
  silentArgs    = "-"
  #OTHERS
  # Uncomment matching EXE type (sorted by most to least common)
  #$silentArgs = '/S'           # NSIS
  #silentArgs   = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /SP-' # Inno Setup
  #silentArgs   = '/s'           # InstallShield
  #silentArgs   = '/s /v"/qn"' # InstallShield with MSI
  #silentArgs   = '/s'           # Wise InstallMaster
  #silentArgs   = '-s'           # Squirrel
  #silentArgs   = '-q'           # Install4j
  #silentArgs   = '-s'           # Ghost
  # Note that some installers, in addition to the silentArgs above, may also need assistance of AHK to achieve silence.
  #silentArgs   = ''             # none; make silent with input macro script like AutoHotKey (AHK)
                                 #       https://chocolatey.org/packages/autohotkey.portable
  
  validExitCodes= @(0) #please insert other valid exit codes here
  url           = ""  #download URL, HTTPS preferrred
  checksum      = '23D91B404E04EB40EEA24C992C1DA8A993B45857F344F8F2CFB16DE97123C1E8'
  checksumType  = 'sha256'
  url64bit      = ""   # 64bit URL here (HTTPS preferred) or remove - if installer contains both architectures (very rare), use $url
  checksum64    = ''
  checksumType64= 'sha256'
  destination   = $toolsDir
}

Install-ChocolateyInstallPackage @packageArgs 

## See https://chocolatey.org/docs/helpers-reference

