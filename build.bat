@echo off
setlocal enabledelayedexpansion

REM AMXX菜单桥接库构建脚本 (Windows)
REM AMXX Menu Bridge Library Build Script (Windows)

set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "NC=[0m"

REM 打印带颜色的消息
:print_message
echo %~1%~2%NC%
goto :eof

REM 检查依赖
:check_dependencies
call :print_message "%BLUE%" "检查构建依赖..."

REM 检查CMake
cmake --version >nul 2>&1
if errorlevel 1 (
    call :print_message "%RED%" "错误: 未找到CMake，请先安装CMake"
    exit /b 1
)

REM 检查Visual Studio或Build Tools
where cl >nul 2>&1
if errorlevel 1 (
    call :print_message "%RED%" "错误: 未找到MSVC编译器，请先安装Visual Studio或Build Tools"
    exit /b 1
)

REM 检查.NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    call :print_message "%RED%" "错误: 未找到.NET SDK，请先安装.NET 6.0或更高版本"
    exit /b 1
)

call :print_message "%GREEN%" "依赖检查完成"
goto :eof

REM 构建C++库
:build_cpp_library
call :print_message "%BLUE%" "构建C++菜单桥接库..."

REM 创建构建目录
if not exist build mkdir build
cd build

REM 配置CMake (使用Visual Studio生成器)
cmake .. -G "Visual Studio 16 2019" -A x64 -DCMAKE_BUILD_TYPE=Release
if errorlevel 1 (
    call :print_message "%RED%" "CMake配置失败"
    cd ..
    exit /b 1
)

REM 编译
cmake --build . --config Release
if errorlevel 1 (
    call :print_message "%RED%" "编译失败"
    cd ..
    exit /b 1
)

REM 返回根目录
cd ..

REM 复制库文件到bin目录
if not exist bin mkdir bin
copy build\Release\MenuBridge.dll bin\ >nul 2>&1
if errorlevel 1 (
    copy build\bin\Release\MenuBridge.dll bin\ >nul 2>&1
)

call :print_message "%GREEN%" "Windows库构建完成: bin\MenuBridge.dll"
goto :eof

REM 构建C#库
:build_csharp_library
call :print_message "%BLUE%" "构建C#菜单管理库..."

REM 构建C#项目
dotnet build AmxxMenuBridge.csproj -c Release
if errorlevel 1 (
    call :print_message "%RED%" "C#库构建失败"
    exit /b 1
)

call :print_message "%GREEN%" "C#库构建完成"
goto :eof

REM 运行测试
:run_tests
call :print_message "%BLUE%" "运行测试..."

REM 编译并运行示例
dotnet run --project AmxxMenuBridge.csproj -c Release
if errorlevel 1 (
    call :print_message "%RED%" "测试失败"
    exit /b 1
)

call :print_message "%GREEN%" "测试完成"
goto :eof

REM 清理构建文件
:clean
call :print_message "%YELLOW%" "清理构建文件..."

if exist build rmdir /s /q build
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj

call :print_message "%GREEN%" "清理完成"
goto :eof

REM 创建发布包
:create_package
call :print_message "%BLUE%" "创建发布包..."

REM 获取当前日期
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "datestamp=%YYYY%%MM%%DD%"

REM 创建包目录
set "PACKAGE_DIR=amxx-menu-bridge-%datestamp%"
if exist %PACKAGE_DIR% rmdir /s /q %PACKAGE_DIR%
mkdir %PACKAGE_DIR%

REM 复制文件
xcopy bin %PACKAGE_DIR%\bin\ /e /i /q >nul 2>&1
copy MenuBridge.h %PACKAGE_DIR%\ >nul 2>&1
copy MenuBridgeImports.cs %PACKAGE_DIR%\ >nul 2>&1
copy MenuManager.cs %PACKAGE_DIR%\ >nul 2>&1
copy MenuExample.cs %PACKAGE_DIR%\ >nul 2>&1
copy AmxxMenuBridge.csproj %PACKAGE_DIR%\ >nul 2>&1
if exist README.md (
    copy README.md %PACKAGE_DIR%\ >nul 2>&1
) else (
    echo # AMXX Menu Bridge > %PACKAGE_DIR%\README.md
)

REM 创建压缩包 (需要7zip或其他压缩工具)
where 7z >nul 2>&1
if not errorlevel 1 (
    7z a -tzip %PACKAGE_DIR%.zip %PACKAGE_DIR%\* >nul 2>&1
    call :print_message "%GREEN%" "发布包创建完成: %PACKAGE_DIR%.zip"
) else (
    call :print_message "%YELLOW%" "未找到7zip，请手动压缩 %PACKAGE_DIR% 目录"
)

goto :eof

REM 显示帮助信息
:show_help
echo AMXX菜单桥接库构建脚本 (Windows)
echo.
echo 用法: %~nx0 [选项]
echo.
echo 选项:
echo   build     构建C++和C#库
echo   cpp       仅构建C++库
echo   csharp    仅构建C#库
echo   test      运行测试
echo   clean     清理构建文件
echo   package   创建发布包
echo   help      显示此帮助信息
echo.
echo 示例:
echo   %~nx0 build    # 构建所有库
echo   %~nx0 clean    # 清理构建文件
echo   %~nx0 package  # 创建发布包
goto :eof

REM 主函数
:main
set "action=%~1"
if "%action%"=="" set "action=build"

if "%action%"=="build" (
    call :check_dependencies
    if errorlevel 1 exit /b 1
    call :build_cpp_library
    if errorlevel 1 exit /b 1
    call :build_csharp_library
    if errorlevel 1 exit /b 1
    call :print_message "%GREEN%" "构建完成！"
) else if "%action%"=="cpp" (
    call :check_dependencies
    if errorlevel 1 exit /b 1
    call :build_cpp_library
) else if "%action%"=="csharp" (
    call :check_dependencies
    if errorlevel 1 exit /b 1
    call :build_csharp_library
) else if "%action%"=="test" (
    call :check_dependencies
    if errorlevel 1 exit /b 1
    call :run_tests
) else if "%action%"=="clean" (
    call :clean
) else if "%action%"=="package" (
    call :create_package
) else if "%action%"=="help" (
    call :show_help
) else if "%action%"=="-h" (
    call :show_help
) else if "%action%"=="--help" (
    call :show_help
) else (
    call :print_message "%RED%" "未知选项: %action%"
    call :show_help
    exit /b 1
)

goto :eof

REM 调用主函数
call :main %*
