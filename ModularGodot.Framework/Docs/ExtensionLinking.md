# Extension Linking

This document explains how to link extension modules to core projects using virtual folders.

## Overview

The ModularGodot framework supports linking extension modules to core projects without duplicating code. This is achieved through the use of virtual folders (junction points) and MSBuild project references.

## How It Works

1. **Virtual Folders**: A junction point is created in the core project that points to the extension module's directory.
2. **Project References**: The core project file is updated to include the extension module's source files.

## Prerequisites

- PowerShell 5.1 or later
- Windows OS (for junction point support)

## Linking Process

### Using the PowerShell Scripts

#### Linking Services

The `LinkExtensionToCore.ps1` script automates the linking process for services:

```powershell
# Link AudioSystem extension to MF.Services core project
.\Tools\LinkExtensionToCore.ps1
```

The script accepts the following parameters:

- `-CoreProjectPath`: Path to the core project (default: "..\Core\MF.Services")
- `-ExtensionPath`: Path to the extension module (default: "..\Extensions\AudioSystem\Services")
- `-TargetFolderName`: Name of the virtual folder in the core project (default: "AudioSystem")
- `-DryRun`: Preview changes without making them

#### Linking Abstractions

The `LinkAbstractionsToCore.ps1` script automates the linking process for abstractions:

```powershell
# Link AudioSystem abstractions to MF.Services.Abstractions core project
.\Tools\LinkAbstractionsToCore.ps1
```

The script accepts the following parameters:

- `-CoreProjectPath`: Path to the core project (default: "..\Core\MF.Services.Abstractions")
- `-ExtensionPath`: Path to the extension abstractions directory (default: "..\Extensions\AudioSystem\Services\Abstractions")
- `-TargetFolderName`: Name of the virtual folder in the core project (default: "AudioSystem")
- `-DryRun`: Preview changes without making them

### Backup and Restore

Before making changes to the project file, the script automatically creates a backup. You can also manually create backups using the `BackupProjectFile.ps1` script:

```powershell
.\Tools\BackupProjectFile.ps1
```

If the project file becomes corrupted, you can restore it from the latest backup using the `RestoreProjectFile.ps1` script:

```powershell
.\Tools\RestoreProjectFile.ps1
```

You can also restore from a specific backup:

```powershell
.\Tools\RestoreProjectFile.ps1 -BackupFile "MF.Services.csproj.backup.20230101-120000"
```

### Manual Process

1. Create a junction point:
   ```cmd
   mklink /j "Core\MF.Services\AudioSystem" "Extensions\AudioSystem\Services"
   ```

2. Update the project file to include the extension files:
   ```xml
   <ItemGroup>
     <Compile Include="AudioSystem\**\*.cs" />
     <None Include="AudioSystem\**\*.*">
       <Link>AudioSystem\%(RecursiveDir)%(Filename)%(Extension)</Link>
       <CopyToOutputDirectory>Never</CopyToOutputDirectory>
     </None>
   </ItemGroup>
   ```

## Building

After linking, build the solution to ensure everything is working correctly:

```bash
dotnet build
```

## Unlinking

To unlink an extension module:

1. Delete the junction point folder
2. Remove the project file references

## Troubleshooting

### Junction Point Issues

- Ensure you have the necessary permissions to create junction points
- Verify that the extension path exists
- Check that the core project path is correct

### Build Issues

- Ensure all dependencies are correctly referenced
- Check for naming conflicts between core and extension modules
- Verify that the extension module compiles independently