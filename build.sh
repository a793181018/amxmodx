#!/bin/bash

# AMXX菜单桥接库构建脚本
# AMXX Menu Bridge Library Build Script

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 打印带颜色的消息
print_message() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# 检查依赖
check_dependencies() {
    print_message $BLUE "检查构建依赖..."
    
    # 检查CMake
    if ! command -v cmake &> /dev/null; then
        print_message $RED "错误: 未找到CMake，请先安装CMake"
        exit 1
    fi
    
    # 检查编译器
    if ! command -v g++ &> /dev/null && ! command -v clang++ &> /dev/null; then
        print_message $RED "错误: 未找到C++编译器，请先安装g++或clang++"
        exit 1
    fi
    
    # 检查.NET SDK
    if ! command -v dotnet &> /dev/null; then
        print_message $RED "错误: 未找到.NET SDK，请先安装.NET 6.0或更高版本"
        exit 1
    fi
    
    print_message $GREEN "依赖检查完成"
}

# 构建C++库
build_cpp_library() {
    print_message $BLUE "构建C++菜单桥接库..."
    
    # 创建构建目录
    mkdir -p build
    cd build
    
    # 配置CMake
    cmake .. -DCMAKE_BUILD_TYPE=Release
    
    # 编译
    make -j$(nproc)
    
    # 返回根目录
    cd ..
    
    # 复制库文件到bin目录
    mkdir -p bin
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        cp build/libMenuBridge.so bin/
        print_message $GREEN "Linux库构建完成: bin/libMenuBridge.so"
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        cp build/libMenuBridge.dylib bin/
        print_message $GREEN "macOS库构建完成: bin/libMenuBridge.dylib"
    fi
}

# 构建C#库
build_csharp_library() {
    print_message $BLUE "构建C#菜单管理库..."
    
    # 构建C#项目
    dotnet build AmxxMenuBridge.csproj -c Release
    
    print_message $GREEN "C#库构建完成"
}

# 运行测试
run_tests() {
    print_message $BLUE "运行测试..."
    
    # 编译并运行示例
    dotnet run --project AmxxMenuBridge.csproj -c Release
    
    print_message $GREEN "测试完成"
}

# 清理构建文件
clean() {
    print_message $YELLOW "清理构建文件..."
    
    rm -rf build
    rm -rf bin
    rm -rf obj
    
    print_message $GREEN "清理完成"
}

# 安装库文件
install_library() {
    print_message $BLUE "安装库文件..."
    
    # 创建安装目录
    INSTALL_DIR="/usr/local/lib/amxx-menu-bridge"
    sudo mkdir -p $INSTALL_DIR
    sudo mkdir -p /usr/local/include/amxx-menu-bridge
    
    # 复制库文件
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        sudo cp bin/libMenuBridge.so $INSTALL_DIR/
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        sudo cp bin/libMenuBridge.dylib $INSTALL_DIR/
    fi
    
    # 复制头文件
    sudo cp MenuBridge.h /usr/local/include/amxx-menu-bridge/
    
    # 复制C#库
    sudo cp bin/Release/net6.0/AmxxMenuBridge.dll $INSTALL_DIR/
    
    print_message $GREEN "库文件安装完成到: $INSTALL_DIR"
}

# 创建发布包
create_package() {
    print_message $BLUE "创建发布包..."
    
    # 创建包目录
    PACKAGE_DIR="amxx-menu-bridge-$(date +%Y%m%d)"
    mkdir -p $PACKAGE_DIR
    
    # 复制文件
    cp -r bin $PACKAGE_DIR/
    cp MenuBridge.h $PACKAGE_DIR/
    cp MenuBridgeImports.cs $PACKAGE_DIR/
    cp MenuManager.cs $PACKAGE_DIR/
    cp MenuExample.cs $PACKAGE_DIR/
    cp AmxxMenuBridge.csproj $PACKAGE_DIR/
    cp README.md $PACKAGE_DIR/ 2>/dev/null || echo "# AMXX Menu Bridge" > $PACKAGE_DIR/README.md
    
    # 创建压缩包
    tar -czf $PACKAGE_DIR.tar.gz $PACKAGE_DIR
    
    # 清理临时目录
    rm -rf $PACKAGE_DIR
    
    print_message $GREEN "发布包创建完成: $PACKAGE_DIR.tar.gz"
}

# 显示帮助信息
show_help() {
    echo "AMXX菜单桥接库构建脚本"
    echo ""
    echo "用法: $0 [选项]"
    echo ""
    echo "选项:"
    echo "  build     构建C++和C#库"
    echo "  cpp       仅构建C++库"
    echo "  csharp    仅构建C#库"
    echo "  test      运行测试"
    echo "  clean     清理构建文件"
    echo "  install   安装库文件到系统"
    echo "  package   创建发布包"
    echo "  help      显示此帮助信息"
    echo ""
    echo "示例:"
    echo "  $0 build    # 构建所有库"
    echo "  $0 clean    # 清理构建文件"
    echo "  $0 install  # 安装到系统"
}

# 主函数
main() {
    case "${1:-build}" in
        "build")
            check_dependencies
            build_cpp_library
            build_csharp_library
            print_message $GREEN "构建完成！"
            ;;
        "cpp")
            check_dependencies
            build_cpp_library
            ;;
        "csharp")
            check_dependencies
            build_csharp_library
            ;;
        "test")
            check_dependencies
            run_tests
            ;;
        "clean")
            clean
            ;;
        "install")
            install_library
            ;;
        "package")
            create_package
            ;;
        "help"|"-h"|"--help")
            show_help
            ;;
        *)
            print_message $RED "未知选项: $1"
            show_help
            exit 1
            ;;
    esac
}

# 运行主函数
main "$@"
