# Install-AudioSystem.ps1
# Script to install/link the AudioSystem extension to the core projects

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Script directory: $scriptDir" -ForegroundColor Cyan

# Define the mapping between extension directories and core projects
$extensionMapping = @{
    "CommandHandlers" = "2_App\MF.CommandHandlers"
    "Commands" = "2_App\MF.Commands"
    "Repositories" = "1_2_Backend\MF.Repositories"
    "Repositories.Abstractions" = "1_2_Backend\MF.Repositories.Abstractions"
    "Services" = "2_App\MF.Services"
    "Services.Abstractions" = "2_App\MF.Services.Abstractions"
}

# Process each extension mapping
foreach ($extension in $extensionMapping.GetEnumerator()) {
    $extensionDir = $extension.Key
    $coreProject = $extension.Value
    
    Write-Host "\nProcessing $extensionDir -> $coreProject" -ForegroundColor Cyan
    
    # Define paths
    $extensionPath = Join-Path $scriptDir "..\$extensionDir"
    $coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\$coreProject\Extensions"
    $targetFolderName = "AudioSystem"
    
    # Check if extension path exists
    if (-not (Test-Path $extensionPath)) {
        Write-Host "Extension path does not exist: $extensionPath" -ForegroundColor Red
        continue
    }
    
    # Resolve extension path
    try {
        $resolvedExtensionPath = Resolve-Path $extensionPath -ErrorAction Stop
    } catch {
        Write-Host "Error resolving extension path for $($extension.Key): $_" -ForegroundColor Red
        continue
    }
    
    # Create core project Extensions directory if it doesn't exist
    if (-not (Test-Path $coreProjectPath)) {
        Write-Host "Creating Extensions directory: $coreProjectPath" -ForegroundColor Cyan
        New-Item -ItemType Directory -Path $coreProjectPath -Force | Out-Null
    }
    
    # Resolve core project path
    try {
        $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
    } catch {
        Write-Host "Error resolving core project path for $($extension.Key): $_" -ForegroundColor Red
        continue
    }
    
    Write-Host "Resolved paths:`n  Extension: $resolvedExtensionPath`n  Core: $resolvedCorePath" -ForegroundColor Cyan
    
    # Check if extension path exists
    if (-not (Test-Path $resolvedExtensionPath)) {
        Write-Host "Extension path does not exist: $resolvedExtensionPath" -ForegroundColor Red
        continue
    }
    
    # Create junction point
    $sourcePath = Join-Path $resolvedCorePath $targetFolderName
    
    # Check if source path already exists
    if (Test-Path $sourcePath) {
        Write-Host "Source path already exists, skipping: $sourcePath" -ForegroundColor Yellow
        continue
    }
    
    # Create Extensions directory if it doesn't exist
    if (-not (Test-Path $resolvedCorePath)) {
        Write-Host "Creating Extensions directory: $resolvedCorePath" -ForegroundColor Cyan
        New-Item -ItemType Directory -Path $resolvedCorePath -Force | Out-Null
    }
    
    # Create parent directory if it doesn't exist
    $parentDir = Split-Path $sourcePath -Parent
    if (-not (Test-Path $parentDir)) {
        Write-Host "Creating parent directory: $parentDir" -ForegroundColor Cyan
        New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
    }
    
    # Create junction point using cmd mklink
    Write-Host "Creating junction point:`n  Source: $sourcePath`n  Target: $resolvedExtensionPath" -ForegroundColor Green
    cmd /c mklink /j "$sourcePath" "$resolvedExtensionPath" | Out-Null
    
    # Removed project file modification logic as per user request
}

Write-Host "
Processing MF.Commons" -ForegroundColor Cyan

# Define paths for MF.Commons
$extensionPath = Join-Path $scriptDir "..\Commons"
$coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\0_Base\MF.Commons\Extensions"
$targetFolderName = "AudioSystem"

# Check if extension path exists
if (-not (Test-Path $extensionPath)) {
    Write-Host "Extension path does not exist: $extensionPath" -ForegroundColor Red
}

# Resolve extension path
try {
    $resolvedExtensionPath = Resolve-Path $extensionPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving extension path for MF.Commons: $_" -ForegroundColor Red
}

# Create core project Extensions directory if it doesn't exist
if (-not (Test-Path $coreProjectPath)) {
    Write-Host "Creating Extensions directory: $coreProjectPath" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $coreProjectPath -Force | Out-Null
}

# Resolve core project path
try {
    $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving core project path for MF.Commons: $_" -ForegroundColor Red
}

Write-Host "Resolved paths:`n  Extension: $resolvedExtensionPath`n  Core: $resolvedCorePath" -ForegroundColor Cyan

# Check if extension path exists
if (-not (Test-Path $resolvedExtensionPath)) {
    Write-Host "Extension path does not exist: $resolvedExtensionPath" -ForegroundColor Red
}

# Create junction point
$sourcePath = Join-Path $resolvedCorePath $targetFolderName

# Check if source path already exists
if (Test-Path $sourcePath) {
    # Check if it's a valid junction point
    $item = Get-Item $sourcePath -Force
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        Write-Host "Valid junction point already exists, removing and recreating: $sourcePath" -ForegroundColor Yellow
        cmd /c rmdir "$sourcePath" | Out-Null
    } else {
        Write-Host "File already exists but is not a junction point, removing and recreating: $sourcePath" -ForegroundColor Yellow
        Remove-Item $sourcePath -Force
    }
}

# Create Extensions directory if it doesn't exist
if (-not (Test-Path $resolvedCorePath)) {
    Write-Host "Creating Extensions directory: $resolvedCorePath" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $resolvedCorePath -Force | Out-Null
}

# Create parent directory if it doesn't exist
$parentDir = Split-Path $sourcePath -Parent
if (-not (Test-Path $parentDir)) {
    Write-Host "Creating parent directory: $parentDir" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
}

# Create junction point using cmd mklink
Write-Host "Creating junction point:`n  Source: $sourcePath`n  Target: $resolvedExtensionPath" -ForegroundColor Green
cmd /c mklink /j "$sourcePath" "$resolvedExtensionPath" | Out-Null

# Create junction point for AudioManager.cs to AutoLoad directory
Write-Host "
Processing AutoLoad" -ForegroundColor Cyan

# Define paths for AutoLoad
$extensionPath = Join-Path $scriptDir "..\AutoLoad\AudioManager.cs"
$autoLoadPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\AutoLoad"
$targetFileName = "AudioManager.cs"

# Check if extension path exists
if (-not (Test-Path $extensionPath)) {
    Write-Host "Extension path does not exist: $extensionPath" -ForegroundColor Red
}

# Resolve extension path
try {
    $resolvedExtensionPath = Resolve-Path $extensionPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving extension path for AutoLoad: $_" -ForegroundColor Red
}

# Create AutoLoad directory if it doesn't exist
if (-not (Test-Path $autoLoadPath)) {
    Write-Host "Creating AutoLoad directory: $autoLoadPath" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $autoLoadPath -Force | Out-Null
}

# Resolve AutoLoad path
try {
    $resolvedAutoLoadPath = Resolve-Path $autoLoadPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving AutoLoad path: $_" -ForegroundColor Red
}

Write-Host "Resolved paths:`n  Extension: $resolvedExtensionPath`n  AutoLoad: $resolvedAutoLoadPath" -ForegroundColor Cyan

# Check if extension path exists
if (-not (Test-Path $resolvedExtensionPath)) {
    Write-Host "Extension path does not exist: $resolvedExtensionPath" -ForegroundColor Red
}

# Create junction point
$sourcePath = Join-Path $resolvedAutoLoadPath $targetFileName

# Check if source path already exists
if (Test-Path $sourcePath) {
    # Check if it's a valid junction point
    $item = Get-Item $sourcePath -Force
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        Write-Host "Valid junction point already exists, removing and recreating: $sourcePath" -ForegroundColor Yellow
        cmd /c rmdir "$sourcePath" | Out-Null
    } else {
        Write-Host "File already exists but is not a junction point, removing and recreating: $sourcePath" -ForegroundColor Yellow
        Remove-Item $sourcePath -Force
    }
}

# Create parent directory if it doesn't exist
$parentDir = Split-Path $sourcePath -Parent
if (-not (Test-Path $parentDir)) {
    Write-Host "Creating parent directory: $parentDir" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
}

# Create junction point using cmd mklink
Write-Host "Creating junction point:`n  Source: $sourcePath`n  Target: $resolvedExtensionPath" -ForegroundColor Green
cmd /c mklink /j "$sourcePath" "$resolvedExtensionPath" | Out-Null

# Create junction point for IAudioManager.cs to MF.Nodes.Abstractions Extensions directory
Write-Host "
Processing MF.Nodes.Abstractions" -ForegroundColor Cyan

# Define paths for MF.Nodes.Abstractions
$extensionPath = Join-Path $scriptDir "..\Nodes.Abstractions"
$coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\1_1_Fronted\MF.Nodes.Abstractions\Extensions"
$targetFolderName = "AudioSystem"

# Check if extension path exists
if (-not (Test-Path $extensionPath)) {
    Write-Host "Extension path does not exist: $extensionPath" -ForegroundColor Red
}

# Resolve extension path
try {
    $resolvedExtensionPath = Resolve-Path $extensionPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving extension path for MF.Nodes.Abstractions: $_" -ForegroundColor Red
}

# Create core project Extensions directory if it doesn't exist
if (-not (Test-Path $coreProjectPath)) {
    Write-Host "Creating Extensions directory: $coreProjectPath" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $coreProjectPath -Force | Out-Null
}

# Resolve core project path
try {
    $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving core project path for MF.Nodes.Abstractions: $_" -ForegroundColor Red
}

Write-Host "Resolved paths:`n  Extension: $resolvedExtensionPath`n  Core: $resolvedCorePath" -ForegroundColor Cyan

# Check if extension path exists
if (-not (Test-Path $resolvedExtensionPath)) {
    Write-Host "Extension path does not exist: $resolvedExtensionPath" -ForegroundColor Red
}

# Create junction point
$sourcePath = Join-Path $resolvedCorePath $targetFolderName

# Check if source path already exists
if (Test-Path $sourcePath) {
    # Check if it's a valid junction point
    $item = Get-Item $sourcePath -Force
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        Write-Host "Valid junction point already exists, removing and recreating: $sourcePath" -ForegroundColor Yellow
        cmd /c rmdir "$sourcePath" | Out-Null
    } else {
        Write-Host "File already exists but is not a junction point, removing and recreating: $sourcePath" -ForegroundColor Yellow
        Remove-Item $sourcePath -Force
    }
}

# Create Extensions directory if it doesn't exist
if (-not (Test-Path $resolvedCorePath)) {
    Write-Host "Creating Extensions directory: $resolvedCorePath" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $resolvedCorePath -Force | Out-Null
}

# Create parent directory if it doesn't exist
$parentDir = Split-Path $sourcePath -Parent
if (-not (Test-Path $parentDir)) {
    Write-Host "Creating parent directory: $parentDir" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
}

# Create junction point using cmd mklink
Write-Host "Creating junction point:`n  Source: $sourcePath`n  Target: $resolvedExtensionPath" -ForegroundColor Green
cmd /c mklink /j "$sourcePath" "$resolvedExtensionPath" | Out-Null

Write-Host "
=== AUDIO SYSTEM INSTALLATION COMPLETE ===" -ForegroundColor Green
Write-Host "AudioSystem extension has been successfully installed/linked to core projects." -ForegroundColor Green

Write-Host "
Next steps:" -ForegroundColor Cyan
Write-Host "1. Build the solution to verify the linking" -ForegroundColor Cyan
Write-Host "2. Test the audio service functionality" -ForegroundColor Cyan