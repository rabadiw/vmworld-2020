function Invoke-StuuckClean {
    Write-Information "Clean"
    
    ($ProjDir, $CSProj, $Configuration) = (Get-StuuckProps)[('ProjDir', 'CSProj', 'Configuration')]

    Push-Location $ProjDir
    SafeExec { & $MSBuildExe ${CSProj} /v:q -t:Clean -p:Configuration=${Configuration} -p:Platform="Any CPU" }
    Pop-Location
}

function Invoke-StuuckRestore {
    Write-Information "Restore"

    ($ProjDir, $CSProj, $Configuration) = (Get-StuuckProps)[('ProjDir', 'CSProj', 'Configuration')]

    Push-Location $ProjDir
    SafeExec { & $MSBuildExe ${CSProj} /v:q -t:restore -p:Configuration=$Configuration -p:Platform="Any CPU" }
    pop-Location
}

function Invoke-StuuckPublish {

    $props = Get-StuuckProps

    Push-Location $props.ProjDir
    
    Write-Information "Working folder: $PWD"
    Write-Information ("Publishing dir: {0}" -f $props.OutputDir)

    SafeExec { & $MSBuildExe "$($props.CSProj)" `
            -t:"Build" `
            -p:TargetFrameworkVersion=v$($props.Framework) `
            -p:Configuration=$($props.Configuration) `
            -p:Platform="Any CPU" `
            -p:OutputPath=$($props.OutputDir) `
            -p:OutDir=$($props.OutputDir) `
            -p:PublishProfileRootFolder="$($props.PublishProfileDir)" `
            -p:PublishProfile=$($props.PublishProfile) `
            -p:Disable_CopyWebApplication=True `
            -p:DeployOnBuild=True }

    pop-Location
}

function Invoke-StuuckPush {
    Write-Information "Push"

    ($ProjDir, $ManifestFile, $ManifestVarsFile) = (Get-StuuckProps)[( 'ProjDir', 'ManifestFile', 'ManifestVarsFile')]

    Push-Location $ProjDir
    SafeExec { & "cf" push -f $ManifestFile -p "$ProjDir\bin\app.publish\" --vars-file $ManifestVarsFile }
    Pop-Location
}

function Get-MSBuild() {
    $vspath = SafeExec { & "${env:ProgramFiles(x86)}\\Microsoft Visual Studio\\Installer\\vswhere.exe"  -latest -prerelease -products * -requires Microsoft.Component.MSBuild -property installationPath }
    if (!$vspath) {
        throw 'Failed to find VS path'
    }

    $tool = join-path $vspath 'MSBuild\Current\Bin\MSBuild.exe'
    if (-not (test-path $tool)) {
        $tool = join-path $vspath 'MSBuild\15.0\Bin\MSBuild.exe'
        if (-not (test-path $tool)) {
            throw 'Failed to find MSBuild'
        }
    }

    return $tool

    # Keeping in here in hopes of being able to use after >= 19.0 updates
    #    return SafeExec { & "${env:ProgramFiles(x86)}\\Microsoft Visual Studio\\Installer\\vswhere.exe" `
    #        -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1 }
}

function Get-Version() {
    return '1.0.0'
}

function SafeExec {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$cmd,

        [string]$errorMessage = ($msgs.error_bad_command -f $cmd),

        [string]$workingDirectory = $null
    )

    do {
        try {
            if ($workingDirectory) {
                Push-Location -Path $workingDirectory
            }

            $global:lastexitcode = 0
            & $cmd
            if ($global:lastexitcode -ne 0) {
                throw "Exec: $errorMessage"
            }
            break
        }
        finally {
            if ($workingDirectory) {
                Pop-Location
            }
        }
    }
    while ($true)
}

function Get-StuuckProps {
    $CSProj = "$PSScriptRoot\Stuuck\Stuuck.csproj"
    $ProjDir = (Split-Path $CSProj -Parent -Resolve)
    $Configuration = "Release"

    return @{
        Framework         = '4.8'
        Configuration     = $Configuration
        NugetPackages     = "${PSScriptRoot}\packages"
        NugetExe          = "${PSScriptRoot}\.nuget\nuget.exe"        
        Version           = get-version
        TAS               = "https://api.run.pcfone.io"
        ManifestFile      = "${PSScriptRoot}\manifest.yaml"
        ManifestVarsFile  = "${PSScriptRoot}\manifest.vars.yaml"

        CSProj            = ${CSProj}
        ProjDir           = ${ProjDir}
        OutputDir         = "${ProjDir}\bin\${Configuration}"
        # test_dir           = "${self.base_dir}\test"
        PublishProfileDir = "${ProjDir}\Properties\PublishProfiles"
        PublishProfile    = "FolderProfile"
    }
}

$MSBuildExe = get-msbuild