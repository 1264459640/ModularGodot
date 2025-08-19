# Remove Core system implementations PowerShell script
# Used to clean up system modules that have been converted to extensions

param(
    [string]$CorePath = "..\Core",
    [switch]$DryRun = $false
)

# Define systems to remove
$systemsToRemove = @(
    "TO.Services\Core\AudioSystem",
    "TO.Services\Core\UISystem",
    "TO.Services\Core\SceneSystem",
    "TO.Services\Core\SequenceSystem",
    "TO.Services\Core\SerializationSystem",
    "TO.Services\Core\GameAbilitySystem",
    "TO.Services\Core\ReadTableSystem",
    "TO.Repositories\Core\AudioSystem",
    "TO.Repositories\Core\UISystem",
    "TO.Repositories\Core\SceneSystem",
    "TO.Repositories\Core\SerializationSystem",
    "TO.Repositories\Core\GameAbilitySystem",
    "TO.Repositories\Core\ReadTableSystem",
    "TO.Repositories\Core\ResourceSystem",
    "TO.Repositories\Core\EventBus",
    "TO.Repositories\Core\LogSystem"
)

$abstractionsToRemove = @(
    "TO.Services.Abstractions\Core\AudioSystem",
    "TO.Services.Abstractions\Core\UISystem",
    "TO.Services.Abstractions\Core\SceneSystem",
    "TO.Services.Abstractions\Core\SequenceSystem",
    "TO.Services.Abstractions\Core\SerializationSystem",
    "TO.Services.Abstractions\Core\GameAbilitySystem",
    "TO.Services.Abstractions\Core\ReadTableSystem",
    "TO.Repositories.Abstractions\Core\AudioSystem",
    "TO.Repositories.Abstractions\Core\UISystem",
    "TO.Repositories.Abstractions\Core\SceneSystem",
    "TO.Repositories.Abstractions\Core\SerializationSystem",
    "TO.Repositories.Abstractions\Core\GameAbilitySystem",
    "TO.Repositories.Abstractions\Core\ReadTableSystem",
    "TO.Repositories.Abstractions\Core\ResourceSystem",
    "TO.Repositories.Abstractions\Core\EventBus",
    "TO.Repositories.Abstractions\Core\LogSystem"
)

$commandHandlersToRemove = @(
    "TO.CommandHandlers\Core\Save",
    "TO.CommandHandlers\Core\GameProgress"
)

# Function to remove system directory
function Remove-SystemDirectory {
    param([string]$RelativePath)
    
    $fullPath = Join-Path $CorePath $RelativePath
    
    if (Test-Path $fullPath) {
        if ($DryRun) {
            Write-Host "[DRY RUN] Will remove directory: $fullPath" -ForegroundColor Yellow
        } else {
            Write-Host "Removing directory: $fullPath" -ForegroundColor Red
            Remove-Item -Path $fullPath -Recurse -Force
        }
    } else {
        Write-Host "Directory not found, skipping: $fullPath" -ForegroundColor Gray
    }
}

# Function to backup Core directory
function Backup-CoreDirectory {
    $backupPath = "$CorePath.backup.$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    
    if ($DryRun) {
        Write-Host "[DRY RUN] Will create backup: $backupPath" -ForegroundColor Yellow
    } else {
        Write-Host "Creating Core directory backup: $backupPath" -ForegroundColor Green
        Copy-Item -Path $CorePath -Destination $backupPath -Recurse -Force
    }
}

# Main execution logic
Write-Host "Starting Core system modules cleanup..." -ForegroundColor Green

if ($DryRun) {
    Write-Host "=== DRY RUN MODE - No files will be deleted ===" -ForegroundColor Yellow
} else {
    Write-Host "=== ACTUAL EXECUTION MODE - Files will be deleted ===" -ForegroundColor Red
    Backup-CoreDirectory
}

Write-Host "`n1. Removing system modules from Services..." -ForegroundColor Cyan
foreach ($system in $systemsToRemove) {
    Remove-SystemDirectory -RelativePath $system
}

Write-Host "`n2. Removing interfaces from Abstractions..." -ForegroundColor Cyan
foreach ($abstraction in $abstractionsToRemove) {
    Remove-SystemDirectory -RelativePath $abstraction
}

Write-Host "`n3. Removing handlers from CommandHandlers..." -ForegroundColor Cyan
foreach ($handler in $commandHandlersToRemove) {
    Remove-SystemDirectory -RelativePath $handler
}

if ($DryRun) {
    Write-Host "`n=== DRY RUN COMPLETE ===" -ForegroundColor Yellow
    Write-Host "To actually execute deletion, run: .\RemoveCoreSystems.ps1" -ForegroundColor Yellow
} else {
    Write-Host "`n=== CLEANUP COMPLETE ===" -ForegroundColor Green
    Write-Host "Core system modules removed, backup created" -ForegroundColor Green
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. Check and fix project references" -ForegroundColor Cyan
Write-Host "2. Update service registration in Contexts" -ForegroundColor Cyan
Write-Host "3. Register all extensions in main project" -ForegroundColor Cyan
Write-Host "4. Test extension system integrity" -ForegroundColor Cyan