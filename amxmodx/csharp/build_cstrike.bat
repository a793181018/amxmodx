@echo off
REM vim: set ts=4 sw=4 tw=99 noet:
REM
REM AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
REM Copyright (C) The AMX Mod X Development Team.
REM
REM This software is licensed under the GNU General Public License, version 3 or higher.
REM Additional exceptions apply. For full license details, see LICENSE.txt or visit:
REM     https://alliedmods.net/amxmodx-license

REM
REM Counter-Strike C# Bridge Build Script for Windows
REM

setlocal enabledelayedexpansion

echo ===================================================================
echo AMX Mod X Counter-Strike C# Bridge Build Script
echo ===================================================================

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK is not installed or not in PATH
    echo Please install .NET SDK 6.0 or later from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

REM Display .NET version
echo Detected .NET version:
dotnet --version

REM Clean previous builds
echo.
echo Cleaning previous builds...
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

REM Build the Counter-Strike C# library
echo.
echo Building Counter-Strike C# library...
dotnet build AmxModX.CStrike.csproj --configuration Release --verbosity minimal

if errorlevel 1 (
    echo X Failed to build Counter-Strike C# library
    pause
    exit /b 1
) else (
    echo √ Counter-Strike C# library built successfully
)

REM Build the test application
echo.
echo Building test application...
dotnet build CStrikeTestApp.csproj --configuration Release --verbosity minimal

if errorlevel 1 (
    echo X Failed to build test application
    pause
    exit /b 1
) else (
    echo √ Test application built successfully
)

REM Create output directory
echo.
echo Creating output directory...
if not exist "..\..\..\build\csharp\cstrike" mkdir "..\..\..\build\csharp\cstrike"

REM Copy built files
echo Copying built files...
copy "bin\Release\net6.0\AmxModX.CStrike.dll" "..\..\..\build\csharp\cstrike\" >nul
copy "bin\Release\net6.0\AmxModX.CStrike.xml" "..\..\..\build\csharp\cstrike\" >nul
copy "bin\Release\net6.0\CStrikeTestApp.exe" "..\..\..\build\csharp\cstrike\" >nul

REM Copy source files for reference
copy "*.cs" "..\..\..\build\csharp\cstrike\" >nul
copy "*.csproj" "..\..\..\build\csharp\cstrike\" >nul
if exist "README.md" copy "README.md" "..\..\..\build\csharp\cstrike\" >nul

echo.
echo ===================================================================
echo Build completed successfully!
echo ===================================================================
echo Output files:
echo   - AmxModX.CStrike.dll (C# library)
echo   - AmxModX.CStrike.xml (documentation)
echo   - CStrikeTestApp.exe (test application)
echo   - Source files for reference
echo.
echo Files are located in: ..\..\..\build\csharp\cstrike\
echo.
echo To run the test application:
echo   cd ..\..\..\build\csharp\cstrike\
echo   CStrikeTestApp.exe
echo.
echo Note: The C++ bridge layer (cstrike_bridge.cpp) needs to be compiled
echo       with the AMX Mod X build system to create the native library.
echo ===================================================================

pause
