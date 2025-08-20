<#
.SYNOPSIS
    Link Abstractions to Core PowerShell script
    Used to create virtual folders to link extension abstractions to core projects

.DESCRIPTION
    This script creates a junction point (virtual folder) linking the AudioSystem abstractions
    to the MF.Services.Abstractions project and updates the project file to include the files.

.PARAMETER CoreProjectPath
    Path to the core project directory (default: "..\Core\MF.Services.Abstractions")

.PARAMETER ExtensionPath
    Path to the extension abstractions directory (default: "..\Extensions\AudioSystem\Services\Abstractions")

.PARAMETER TargetFolderName
    Name of the virtual folder in the core project (default: "AudioSystem")

.PARAMETER DryRun
    Preview changes without making them

.EXAMPLE
    .\LinkAbstractionsToCore.ps1
    Links AudioSystem abstractions to MF.Services.Abstractions core project

.EXAMPLE
    .\LinkAbstractionsToCore.ps1 -DryRun
    Preview the linking changes without making them
#>

param(
    [string]$CoreProjectPath = "..\Core\MF.Services.Abstractions",
    [string]$ExtensionPath = "..\Extensions\AudioSystem\Services.Abstractions",
    [string]$TargetFolderName = "AudioSystem",
    [switch]$DryRun = $false
)

# Function to create junction point (virtual folder)
function Create-JunctionPoint {
    param([string]$SourcePath, [string]$TargetPath)
    
    # Check if source path exists
    if (Test-Path $SourcePath) {
        Write-Host "Source path already exists, skipping: $SourcePath" -ForegroundColor Yellow
        return
    }
    
    # Check if target path exists
    if (-not (Test-Path $TargetPath)) {
        Write-Host "Target path does not exist: $TargetPath" -ForegroundColor Red
        return
    }
    
    # Create junction point
    if ($DryRun) {
        Write-Host "[DRY RUN] Will create junction point:`n  Source: $SourcePath`n  Target: $TargetPath" -ForegroundColor Yellow
    } else {
        # Create parent directory if it doesn't exist
        $parentDir = Split-Path $SourcePath -Parent
        if (-not (Test-Path $parentDir)) {
            Write-Host "Creating parent directory: $parentDir" -ForegroundColor Cyan
            New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
        }
        
        # Create junction point using cmd mklink
        Write-Host "Creating junction point:`n  Source: $SourcePath`n  Target: $TargetPath" -ForegroundColor Green
        cmd /c mklink /j "$SourcePath" "$TargetPath" | Out-Null
    }
}

# Function to add link to csproj file
function Add-ProjectLink {
    param([string]$ProjectFile, [string]$LinkPath)
    
    # Check if project file exists
    if (-not (Test-Path $ProjectFile)) {
        Write-Host "Project file does not exist: $ProjectFile" -ForegroundColor Red
        return
    }
    
    # Read project file content
    $content = Get-Content $ProjectFile -Raw
    
    # Check if link already exists
    if ($content -match [regex]::Escape("AudioSystem\\")) {
        Write-Host "Link already exists in project file, skipping" -ForegroundColor Yellow
        return
    }
    
    # Create link element
    $linkElement = "  <!-- Extension References -->`n  <ItemGroup>`n    <Compile Include=`"AudioSystem\**\*.cs`" />`n    <None Include=`"AudioSystem\**\*.*`">`n      <Link>AudioSystem\%(RecursiveDir)%(Filename)%(Extension)</Link>`n      <CopyToOutputDirectory>Never</CopyToOutputDirectory>`n    </None>`n  </ItemGroup>"
    
    # Find position to insert link (before closing Project tag)
    if ($content -match "</Project>") {
        # Use a more precise method to insert the link element
        # Find the last ItemGroup and insert after it
        $lines = $content -split "`r?`n"
        $insertIndex = -1
        
        # Find the last ItemGroup closing tag
        for ($i = $lines.Count - 1; $i -ge 0; $i--) {
            if ($lines[$i] -match "</ItemGroup>") {
                $insertIndex = $i + 1
                break
            }
        }
        
        # Also find the closing Project tag to ensure we don't remove it
        $projectEndIndex = -1
        for ($i = $lines.Count - 1; $i -ge 0; $i--) {
            if ($lines[$i] -match "</Project>") {
                $projectEndIndex = $i
                break
            }
        }
        
        if ($insertIndex -ge 0 -and $projectEndIndex -ge 0) {
            # Insert the link element at the found position, but before the closing Project tag
            # Ensure we don't insert after the Project tag
            if ($insertIndex -gt $projectEndIndex) {
                $insertIndex = $projectEndIndex
            }
            
            # Split the link element into lines
            $linkElementLines = $linkElement -split "`r?`n"
            
            # Insert the link element at the found position
            $beforeInsert = $lines[0..($insertIndex-1)]
            $afterInsert = $lines[$insertIndex..($lines.Count-1)]
            $newLines = $beforeInsert + $linkElementLines + $afterInsert
            $newContent = $newLines -join "`r`n"
            
            if ($DryRun) {
                Write-Host "[DRY RUN] Will add link to project file: $ProjectFile" -ForegroundColor Yellow
                Write-Host "Link element:`n$linkElement" -ForegroundColor Yellow
            } else {
                Set-Content -Path $ProjectFile -Value $newContent -Encoding UTF8
                Write-Host "Added link to project file: $ProjectFile" -ForegroundColor Green
            }
        } else {
            Write-Host "Could not find appropriate position to insert link in: $ProjectFile" -ForegroundColor Red
        }
    } else {
        Write-Host "Could not find closing Project tag in: $ProjectFile" -ForegroundColor Red
    }
}

# Main execution logic
Write-Host "Starting link abstractions to core process..." -ForegroundColor Green

if ($DryRun) {
    Write-Host "=== DRY RUN MODE - No changes will be made ===" -ForegroundColor Yellow
} else {
    # Create backup of project file before making changes
    Write-Host "Creating backup of project file..." -ForegroundColor Cyan
    $backupScript = Join-Path $scriptDir "BackupProjectFile.ps1"
    if (Test-Path $backupScript) {
        & $backupScript
    } else {
        Write-Host "Backup script not found: $backupScript" -ForegroundColor Yellow
        Write-Host "Continuing without backup..." -ForegroundColor Yellow
    }
}

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Script directory: $scriptDir" -ForegroundColor Cyan

# Resolve paths to absolute paths
try {
    # Resolve core project path relative to script directory
    $fullCoreProjectPath = Join-Path $scriptDir $CoreProjectPath
    $resolvedCorePath = Resolve-Path $fullCoreProjectPath -ErrorAction Stop
    
    # Resolve extension path relative to script directory
    $fullExtensionPath = Join-Path $scriptDir $ExtensionPath
    $resolvedExtensionPath = Resolve-Path $fullExtensionPath -ErrorAction Stop
    
    Write-Host "Resolved paths:`n  Core: $resolvedCorePath`n  Extension: $resolvedExtensionPath" -ForegroundColor Cyan
} catch {
    Write-Host "Error resolving paths: $_" -ForegroundColor Red
    exit 1
}

# Create junction point
$sourcePath = Join-Path $resolvedCorePath $TargetFolderName
Write-Host "Source path: $sourcePath" -ForegroundColor Cyan
Create-JunctionPoint -SourcePath $sourcePath -TargetPath $resolvedExtensionPath

# Add link to project file
$projectFile = Join-Path $resolvedCorePath "MF.Services.Abstractions.csproj"
Write-Host "Project file: $projectFile" -ForegroundColor Cyan
Add-ProjectLink -ProjectFile $projectFile

if ($DryRun) {
    Write-Host "=== DRY RUN COMPLETE ===" -ForegroundColor Yellow
    Write-Host "To actually execute linking, run: .\LinkAbstractionsToCore.ps1" -ForegroundColor Yellow
} else {
    Write-Host "=== LINKING COMPLETE ===" -ForegroundColor Green
    Write-Host "Abstractions linked to core project successfully" -ForegroundColor Green
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Build the solution to verify the linking" -ForegroundColor Cyan
Write-Host "2. Test the audio service abstractions functionality" -ForegroundColor Cyan