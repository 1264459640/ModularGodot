# Uninstall-AudioSystem.ps1
# Script to uninstall/remove the AudioSystem extension links from the core projects

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host "Script directory: $scriptDir" -ForegroundColor Cyan

# Define the mapping between core projects and the AudioSystem junction points to remove
$coreProjects = @(
    "2_App\MF.CommandHandlers",
    "2_App\MF.Commands",
    "1_2_Backend\MF.Repositories",
    "1_2_Backend\MF.Repositories.Abstractions",
    "2_App\MF.Services",
    "2_App\MF.Services.Abstractions"
)

# Process each core project
foreach ($project in $coreProjects) {
    Write-Host "\nProcessing $project" -ForegroundColor Cyan
    
    # Define paths
    $coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\$project\Extensions"
    $junctionPointPath = Join-Path $coreProjectPath "AudioSystem"
    $projectFilePath = Join-Path $coreProjectPath "..\$project.csproj"
    
    # Resolve core project path
    try {
        $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
    } catch {
        Write-Host "Error resolving path for ${project}: $($_)" -ForegroundColor Red
        continue
    }
    
    Write-Host "Resolved path: $resolvedCorePath" -ForegroundColor Cyan
    
    # Check if junction point exists and remove it
    if (Test-Path $junctionPointPath) {
        # Check if it's a junction point
        $item = Get-Item $junctionPointPath -Force
        if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
            Write-Host "Removing junction point: $junctionPointPath" -ForegroundColor Yellow
            # Remove the junction point
            cmd /c rmdir "$junctionPointPath" | Out-Null
            
            if (Test-Path $junctionPointPath) {
                Write-Host "Failed to remove junction point: $junctionPointPath" -ForegroundColor Red
            } else {
                Write-Host "Successfully removed junction point: $junctionPointPath" -ForegroundColor Green
            }
        } else {
            Write-Host "Path exists but is not a junction point: $junctionPointPath" -ForegroundColor Yellow
        }
    } else {
        Write-Host "Junction point does not exist: $junctionPointPath" -ForegroundColor Yellow
    }
    
    # Check if project file exists
    if (-not (Test-Path $projectFilePath)) {
        Write-Host "Project file does not exist: $projectFilePath" -ForegroundColor Red
        continue
    }
    
    # Read project file content
    $content = Get-Content $projectFilePath -Raw
    
    # Check if AudioSystem link exists in the project file
    if ($content -match [regex]::Escape("AudioSystem\\")) {
        Write-Host "Removing AudioSystem link from project file: $projectFilePath" -ForegroundColor Yellow
        
        # Remove the link element
        $lines = $content -split "`r?`n"
        $newLines = @()
        $inExtensionSection = $false
        
        foreach ($line in $lines) {
            # Check if we're entering the extension references section
            if ($line -match "<!-- Extension References -->") {
                $inExtensionSection = $true
                continue
            }
            
            # Check if we're leaving the extension references section
            if ($inExtensionSection -and $line -match "</ItemGroup>") {
                $inExtensionSection = $false
                continue
            }
            
            # Skip lines within the extension references section
            if ($inExtensionSection) {
                continue
            }
            
            # Add line to new content
            $newLines += $line
        }
        
        # Join the lines back together
        $newContent = $newLines -join "`r`n"
        
        # Write the updated content back to the file
        Set-Content -Path $projectFilePath -Value $newContent -Encoding UTF8
        Write-Host "Successfully removed AudioSystem link from project file" -ForegroundColor Green
    } else {
        Write-Host "AudioSystem link not found in project file" -ForegroundColor Yellow
    }
}

Write-Host "\nProcessing MF.Commons" -ForegroundColor Cyan

# Define paths for MF.Commons
$coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\0_Base\MF.Commons\Extensions"
$junctionPointPath = Join-Path $coreProjectPath "AudioSystem"

# Resolve core project path
try {
    $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving path for MF.Commons: $($_)" -ForegroundColor Red
}

Write-Host "Resolved path: $resolvedCorePath" -ForegroundColor Cyan

# Check if junction point exists and remove it
if (Test-Path $junctionPointPath) {
    # Check if it's a junction point
    $item = Get-Item $junctionPointPath -Force
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        Write-Host "Removing junction point: $junctionPointPath" -ForegroundColor Yellow
        # Remove the junction point
        cmd /c rmdir "$junctionPointPath" | Out-Null
        
        if (Test-Path $junctionPointPath) {
            Write-Host "Failed to remove junction point: $junctionPointPath" -ForegroundColor Red
        } else {
            Write-Host "Successfully removed junction point: $junctionPointPath" -ForegroundColor Green
        }
    } else {
        Write-Host "Path exists but is not a junction point: $junctionPointPath" -ForegroundColor Yellow
    }
} else {
    Write-Host "Junction point does not exist: $junctionPointPath" -ForegroundColor Yellow
}

Write-Host "\nProcessing AutoLoad" -ForegroundColor Cyan

# Define paths for AutoLoad
$autoLoadPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\AutoLoad"
$junctionPointPath = Join-Path $autoLoadPath "AudioManager.cs"

# Resolve AutoLoad path
try {
    $resolvedAutoLoadPath = Resolve-Path $autoLoadPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving path for AutoLoad: $($_)" -ForegroundColor Red
}

Write-Host "Resolved path: $resolvedAutoLoadPath" -ForegroundColor Cyan

# Check if junction point exists and remove it
if (Test-Path $junctionPointPath) {
    # Check if it's a junction point or hard link
    $item = Get-Item $junctionPointPath -Force
    $isReparsePoint = $item.Attributes -band [System.IO.FileAttributes]::ReparsePoint
    $isHardLink = (Get-ItemProperty -Path $junctionPointPath).LinkType -eq "HardLink"
    
    if ($isReparsePoint -or $isHardLink) {
        Write-Host "Removing link: $junctionPointPath" -ForegroundColor Yellow
        # Remove the junction point or hard link
        if ($isReparsePoint) {
            cmd /c rmdir "$junctionPointPath" | Out-Null
        } else {
            # For hard link, we need to use different approach
            Remove-Item "$junctionPointPath" -Force
        }
        
        if (Test-Path $junctionPointPath) {
            Write-Host "Failed to remove link: $junctionPointPath" -ForegroundColor Red
        } else {
            Write-Host "Successfully removed link: $junctionPointPath" -ForegroundColor Green
        }
    } else {
        Write-Host "Path exists but is not a link: $junctionPointPath" -ForegroundColor Yellow
        # If it's a regular file, remove it
        Remove-Item "$junctionPointPath" -Force
        Write-Host "Removed regular file: $junctionPointPath" -ForegroundColor Yellow
    }
} else {
    Write-Host "Link does not exist: $junctionPointPath" -ForegroundColor Yellow
}

Write-Host "\nProcessing MF.Nodes.Abstractions" -ForegroundColor Cyan

# Define paths for MF.Nodes.Abstractions
$coreProjectPath = Join-Path $scriptDir "..\..\..\..\ModularGodot.Framework\Core\1_1_Fronted\MF.Nodes.Abstractions\Extensions"
$junctionPointPath = Join-Path $coreProjectPath "AudioSystem"

# Resolve core project path
try {
    $resolvedCorePath = Resolve-Path $coreProjectPath -ErrorAction Stop
} catch {
    Write-Host "Error resolving path for MF.Nodes.Abstractions: $($_)" -ForegroundColor Red
}

Write-Host "Resolved path: $resolvedCorePath" -ForegroundColor Cyan

# Check if junction point exists and remove it
if (Test-Path $junctionPointPath) {
    # Check if it's a junction point
    $item = Get-Item $junctionPointPath -Force
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        Write-Host "Removing junction point: $junctionPointPath" -ForegroundColor Yellow
        # Remove the junction point
        cmd /c rmdir "$junctionPointPath" | Out-Null
        
        if (Test-Path $junctionPointPath) {
            Write-Host "Failed to remove junction point: $junctionPointPath" -ForegroundColor Red
        } else {
            Write-Host "Successfully removed junction point: $junctionPointPath" -ForegroundColor Green
        }
    } else {
        Write-Host "Path exists but is not a junction point: $junctionPointPath" -ForegroundColor Yellow
    }
} else {
    Write-Host "Junction point does not exist: $junctionPointPath" -ForegroundColor Yellow
}

Write-Host "\n=== AUDIO SYSTEM UNINSTALLATION COMPLETE ===" -ForegroundColor Green
Write-Host "AudioSystem extension links have been successfully removed from core projects." -ForegroundColor Green