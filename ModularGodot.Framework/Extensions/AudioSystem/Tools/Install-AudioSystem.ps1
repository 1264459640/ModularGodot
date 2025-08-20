# Install-AudioSystem.ps1
# Script to install/link the AudioSystem extension to the core projects

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Script directory: $scriptDir" -ForegroundColor Cyan

# Define the mapping between extension directories and core projects
$extensionMapping = @{
    "CommandHandlers" = "MF.CommandHandlers"
    "Commands" = "MF.Commands"
    "Repositories" = "MF.Repositories"
    "Repositories.Abstractions" = "MF.Repositories.Abstractions"
    "Services" = "MF.Services"
    "Services.Abstractions" = "MF.Services.Abstractions"
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
$coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\MF.Commons\Extensions"
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
    Write-Host "Source path already exists, skipping: $sourcePath" -ForegroundColor Yellow
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