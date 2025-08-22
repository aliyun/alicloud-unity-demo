@echo off
setlocal enabledelayedexpansion

echo ==============================================
echo Building HttpDNS Unity Bridge for Windows
echo ==============================================

REM Check if VCPKG_ROOT is set
if defined VCPKG_ROOT (
    echo Using VCPKG from: %VCPKG_ROOT%
    set "CMAKE_TOOLCHAIN_FILE=%VCPKG_ROOT%\scripts\buildsystems\vcpkg.cmake"
    set "VCPKG_TARGET_TRIPLET=x64-windows"
) else (
    echo VCPKG_ROOT not set. Checking for offline dependencies...
    if defined CMAKE_PREFIX_PATH (
        echo Using offline dependencies from: %CMAKE_PREFIX_PATH%
    ) else (
        echo ERROR: Neither VCPKG_ROOT nor CMAKE_PREFIX_PATH is set!
        echo Please set one of the following:
        echo   - VCPKG_ROOT=C:\vcpkg
        echo   - CMAKE_PREFIX_PATH=C:\path\to\offline\deps
        pause
        exit /b 1
    )
)

REM Set build directories
set "SOURCE_DIR=%~dp0"
set "BUILD_DIR=%SOURCE_DIR%build_windows"

REM Clean previous build
if exist "%BUILD_DIR%" (
    echo Cleaning previous build...
    rmdir /s /q "%BUILD_DIR%"
)

REM Create build directory
mkdir "%BUILD_DIR%"
cd /d "%BUILD_DIR%"

REM Configure CMake
echo Configuring with CMake...
if defined VCPKG_ROOT (
    cmake "%SOURCE_DIR%" -G "Visual Studio 17 2022" -A x64 -DCMAKE_BUILD_TYPE=Release -DENABLE_HTTPDNS_C_SDK=ON -DCMAKE_TOOLCHAIN_FILE="%CMAKE_TOOLCHAIN_FILE%" -DVCPKG_TARGET_TRIPLET=%VCPKG_TARGET_TRIPLET%
) else (
    cmake "%SOURCE_DIR%" -G "Visual Studio 17 2022" -A x64 -DCMAKE_BUILD_TYPE=Release -DENABLE_HTTPDNS_C_SDK=ON -DCMAKE_PREFIX_PATH="%CMAKE_PREFIX_PATH%"
)

if %ERRORLEVEL% neq 0 (
    echo CMake configuration failed!
    pause
    exit /b %ERRORLEVEL%
)

REM Build the project
echo Building...
cmake --build . --config Release --parallel

if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b %ERRORLEVEL%
)

echo Build completed successfully!

REM Show output location
set "OUTPUT_DIR=%SOURCE_DIR%..\x86_64"
if exist "%OUTPUT_DIR%\HttpDnsUnityBridge.dll" (
    echo Output library: %OUTPUT_DIR%\HttpDnsUnityBridge.dll
    dir "%OUTPUT_DIR%\HttpDnsUnityBridge.dll"
) else (
    echo Warning: Output library not found at expected location
    for /r "%BUILD_DIR%" %%f in (*.dll) do echo Found DLL: %%f
)

echo ==============================================
echo Build completed
echo ==============================================
pause