function Invoke-StuuckClean {
    Write-Information "Clean"
    
    ($CSProj) = (Get-StuuckProps)[('CSProj')]

    Push-Location $ProjDir
    SafeExec { & dotnet clean ${CSProj} }
    Pop-Location
}

function Invoke-StuuckRestore {
    Write-Information "Restore"

    ($CSProj) = (Get-StuuckProps)[('CSProj')]

    Push-Location $ProjDir
    SafeExec { & dotnet restore ${CSProj} }
    pop-Location
}

function Invoke-StuuckTest {
    Write-Information "Test"

    ($TestCSProj) = (Get-StuuckProps)[('TestCSProj')]

    Push-Location $ProjDir
    SafeExec { & dotnet test ${TestCSProj} }
    pop-Location
}

function Invoke-StuuckPublish {

    ($CSProj, $Configuration) = (Get-StuuckProps)[('CSProj', 'Configuration')]

    Push-Location $props.ProjDir
    
    Write-Information "Working folder: $PWD"

    SafeExec { & dotnet publish ${CSProj} --configuration $Configuration --runtime linux-x64 }

    pop-Location
}

function Invoke-StuuckPush {
    Write-Information "Push"

    ($ProjDir, $ManifestFile, $ManifestVarsFile) = (Get-StuuckProps)[( 'ProjDir', 'ManifestFile', 'ManifestVarsFile')]

    Push-Location $ProjDir
    SafeExec { & "cf" push -f $ManifestFile -p "$ProjDir\bin\Release\netcoreapp3.1\linux-x64\publish\" --vars-file $ManifestVarsFile }
    Pop-Location
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
    $CSProj = "$PSScriptRoot\Stuuck.Api\Stuuck.Api.csproj"
    $TestCSProj = "$PSScriptRoot\Stuuck.Api.Tests\Stuuck.Api.Tests.csproj"
    $ProjDir = (Split-Path $CSProj -Parent -Resolve)
    $Configuration = "Release"

    return @{
        Framework        = '4.8'
        Configuration    = $Configuration
        Version          = get-version
        TAS              = "https://api.run.pcfone.io"
        ManifestFile     = "${PSScriptRoot}\manifest.yaml"
        ManifestVarsFile = "${PSScriptRoot}\manifest.vars.yaml"

        CSProj           = ${CSProj}
        ProjDir          = ${ProjDir}
        TestCSProj       = ${TestCSProj}
    }
}

$MSBuildExe = get-msbuild