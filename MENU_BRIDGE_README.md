# AMXX Menu Bridge

一个将AMX Mod X菜单系统接口桥接到C#的完整解决方案。

A complete solution for bridging AMX Mod X menu system interfaces to C#.

## 功能特性 / Features

- ✅ **完整的菜单系统封装** / Complete menu system encapsulation
- ✅ **跨平台支持** / Cross-platform support (Windows/Linux/macOS)
- ✅ **类型安全的C#接口** / Type-safe C# interfaces
- ✅ **委托回调支持** / Delegate callback support
- ✅ **自动字符串长度处理** / Automatic string length handling
- ✅ **大驼峰命名规范** / PascalCase naming convention
- ✅ **完整的XML文档** / Complete XML documentation
- ✅ **与AMXX层互通** / Interoperability with AMXX layer

## 项目结构 / Project Structure

```
amxx-menu-bridge/
├── MenuBridge.h              # C++桥接层头文件
├── MenuBridge.cpp            # C++桥接层实现
├── MenuBridgeImports.cs      # C# DLL导入定义
├── MenuManager.cs            # C#高级封装类
├── MenuExample.cs            # C#使用示例
├── AmxxMenuBridge.csproj     # C#项目文件
├── CMakeLists.txt            # CMake构建配置
├── build.sh                  # Linux/macOS构建脚本
├── build.bat                 # Windows构建脚本
└── README.md                 # 项目文档
```

## 快速开始 / Quick Start

### 1. 环境要求 / Requirements

**Windows:**
- Visual Studio 2019+ 或 Build Tools
- CMake 3.16+
- .NET 6.0+ SDK

**Linux/macOS:**
- GCC 7+ 或 Clang 6+
- CMake 3.16+
- .NET 6.0+ SDK

### 2. 构建项目 / Build Project

**Windows:**
```cmd
build.bat build
```

**Linux/macOS:**
```bash
chmod +x build.sh
./build.sh build
```

### 3. 基本使用 / Basic Usage

```csharp
using AmxxMenuBridge;

// 创建菜单
var menu = MenuManager.CreateMenu("主菜单", "MainMenuHandler", true);

// 添加菜单项
menu.AddItem("选项1", "option1");
menu.AddItem("选项2", "option2");

// 添加带回调的菜单项
menu.AddItem("武器菜单", (menuId, playerId, item) => {
    Console.WriteLine($"玩家 {playerId} 选择了武器菜单");
    return 1;
});

// 显示菜单
menu.Display(playerId);
```

## API文档 / API Documentation

### 核心类 / Core Classes

#### MenuManager
主要的菜单管理类，提供完整的菜单操作功能。

Main menu management class providing complete menu operation functionality.

**主要方法 / Main Methods:**
- `CreateMenu(title, handler, useMultilingual)` - 创建菜单
- `AddItem(name, command, access)` - 添加菜单项
- `AddItem(name, callback, command, access)` - 添加带回调的菜单项
- `Display(playerId, page)` - 显示菜单
- `SetProperty(property, value)` - 设置菜单属性

#### MenuBridgeImports
底层DLL导入类，包含所有原生函数声明。

Low-level DLL import class containing all native function declarations.

### 委托类型 / Delegate Types

```csharp
// 菜单处理器委托
public delegate int MenuHandlerDelegate(int menuId, int playerId, int item);

// 菜单项回调委托
public delegate int MenuItemDelegate(int menuId, int playerId, int item);
```

### 枚举类型 / Enum Types

```csharp
// 菜单属性
public enum MenuProperty
{
    BackName, NextName, ExitName, Title, ItemColor,
    NeverExit, ForceExit, PerPage, ShowPage
}

// 菜单项绘制类型
public enum ItemDrawType
{
    Default, Disabled, RawLine, NoText, Spacer
}
```

## 高级示例 / Advanced Examples

### 动态菜单创建 / Dynamic Menu Creation

```csharp
public class WeaponMenu
{
    private MenuManager menu;
    
    public WeaponMenu()
    {
        menu = MenuManager.CreateMenu("武器选择", "WeaponHandler");
        menu.SetItemsPerPage(7);
        menu.SetShowPageNumbers(true);
        
        // 动态添加武器
        var weapons = new[] { "AK-47", "M4A1", "AWP", "Deagle" };
        foreach (var weapon in weapons)
        {
            menu.AddItem(weapon, OnWeaponSelected, $"weapon_{weapon.ToLower()}");
        }
    }
    
    private int OnWeaponSelected(int menuId, int playerId, int item)
    {
        var itemInfo = menu.GetItemInfo(item);
        GivePlayerWeapon(playerId, itemInfo.Command);
        return 1;
    }
}
```

### 分页菜单处理 / Paginated Menu Handling

```csharp
public class PlayerListMenu
{
    public static void ShowPlayerList(int adminId)
    {
        var menu = MenuManager.CreateMenu("玩家列表", "PlayerListHandler");
        
        // 获取所有在线玩家
        for (int i = 1; i <= 32; i++)
        {
            if (IsPlayerConnected(i))
            {
                string playerName = GetPlayerName(i);
                menu.AddItem(playerName, (menuId, playerId, item) => {
                    // 处理玩家选择
                    HandlePlayerSelection(adminId, i);
                    return 1;
                });
            }
        }
        
        menu.Display(adminId);
    }
}
```

## 与AMXX互通 / AMXX Interoperability

该桥接层完全兼容现有的AMXX菜单系统，可以：

This bridge layer is fully compatible with existing AMXX menu system and can:

- 与AMXX插件创建的菜单共存
- 接收AMXX菜单事件
- 调用AMXX菜单函数
- 共享菜单状态

## 性能优化 / Performance Optimization

- 使用对象池减少GC压力
- 缓存频繁访问的菜单对象
- 异步处理大量菜单项
- 智能字符串缓冲区管理

## 故障排除 / Troubleshooting

### 常见问题 / Common Issues

1. **DLL加载失败**
   - 确保MenuBridge.dll在正确路径
   - 检查依赖库是否完整
   - 验证平台架构匹配(x86/x64)

2. **回调函数不执行**
   - 确保委托对象保持引用
   - 检查函数签名是否正确
   - 验证菜单ID有效性

3. **字符编码问题**
   - 使用UTF-8编码
   - 检查字符串长度限制
   - 验证特殊字符处理

## 贡献指南 / Contributing

1. Fork项目
2. 创建功能分支
3. 提交更改
4. 创建Pull Request

## 许可证 / License

本项目采用MIT许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 联系方式 / Contact

- 项目主页: [GitHub Repository]
- 问题反馈: [Issues]
- 文档: [Wiki]

## 更新日志 / Changelog

### v1.0.0
- 初始版本发布
- 完整的菜单系统桥接
- 跨平台支持
- 完整的C#封装

---

**注意**: 本项目需要AMX Mod X环境才能正常运行。

**Note**: This project requires AMX Mod X environment to function properly.
