# AMX Mod X Extended C# API

## 概述 / Overview

AMX Mod X Extended C# API 是对 AMX Mod X 的扩展，为 C# 开发者提供了完整的接口适配层，支持以下系统：

AMX Mod X Extended C# API is an extension to AMX Mod X that provides a complete interface adaptation layer for C# developers, supporting the following systems:

- **CVar系统** / CVar System - 控制台变量管理
- **菜单系统** / Menu System - 游戏内菜单创建和管理
- **游戏配置** / Game Config - 游戏配置文件解析
- **Native管理** / Native Management - 自定义Native函数注册
- **消息系统** / Message System - 游戏消息发送和接收
- **数据包** / DataPack - 序列化数据存储

## 特性 / Features

### ✅ 已实现的功能 / Implemented Features

1. **完整的C++桥接层** / Complete C++ Bridge Layer
   - 跨平台兼容性 / Cross-platform compatibility
   - 线程安全 / Thread safety
   - 内存管理 / Memory management

2. **高级C#管理类** / High-level C# Manager Classes
   - 大驼峰命名规范 / PascalCase naming convention
   - 完整的XML文档注释 / Complete XML documentation
   - 异常处理 / Exception handling

3. **委托回调系统** / Delegate Callback System
   - CVar变化监听 / CVar change monitoring
   - 菜单选择回调 / Menu selection callbacks
   - 消息钩子 / Message hooks

4. **自动字符串长度处理** / Automatic String Length Handling
   - 无需手动指定缓冲区大小 / No manual buffer size specification
   - 自动内存分配 / Automatic memory allocation

## 系统架构 / System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    C# Application Layer                     │
│  ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐ │
│  │   CvarManager   │ │   MenuManager   │ │ MessageManager  │ │
│  └─────────────────┘ └─────────────────┘ └─────────────────┘ │
│  ┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐ │
│  │GameConfigManager│ │  NativeManager  │ │DataPackManager  │ │
│  └─────────────────┘ └─────────────────┘ └─────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│                   C# Interop Layer                         │
│              ExtendedNativeMethods                          │
├─────────────────────────────────────────────────────────────┤
│                   C++ Bridge Layer                         │
│                 csharp_bridge.cpp                          │
├─────────────────────────────────────────────────────────────┤
│                   AMX Mod X Core                           │
│              Native Functions & Systems                     │
└─────────────────────────────────────────────────────────────┘
```

## 快速开始 / Quick Start

### 1. CVar系统使用 / CVar System Usage

```csharp
// 创建CVar / Create CVar
int cvarId = CvarManager.CreateCvar("my_plugin_enabled", "1", 0, "Enable/disable my plugin");

// 获取CVar值 / Get CVar value
string value = CvarManager.GetCvarString("my_plugin_enabled");
int intValue = CvarManager.GetCvarInt("my_plugin_enabled");
float floatValue = CvarManager.GetCvarFloat("my_plugin_enabled");

// 设置CVar值 / Set CVar value
CvarManager.SetCvarString("my_plugin_enabled", "0");
CvarManager.SetCvarInt("my_plugin_enabled", 0);

// 监听CVar变化 / Monitor CVar changes
int hookId = CvarManager.HookCvarChange("my_plugin_enabled", OnCvarChanged);

private static void OnCvarChanged(string cvarName, string oldValue, string newValue)
{
    Console.WriteLine($"CVar {cvarName} changed from '{oldValue}' to '{newValue}'");
}
```

### 2. 菜单系统使用 / Menu System Usage

```csharp
// 创建菜单 / Create menu
int menuId = MenuManager.CreateMenu("My Menu", OnMenuSelect, OnMenuCancel);

// 添加菜单项 / Add menu items
MenuManager.AddMenuItem(menuId, "Option 1", "cmd1", 0);
MenuManager.AddMenuItem(menuId, "Option 2", "cmd2", 0);
MenuManager.AddMenuBlank(menuId); // 添加空行 / Add blank line
MenuManager.AddMenuText(menuId, "--- Settings ---"); // 添加文本 / Add text

// 显示菜单 / Display menu
MenuManager.DisplayMenu(menuId, clientId, 0);

private static void OnMenuSelect(int clientId, int menuId, int item)
{
    Console.WriteLine($"Client {clientId} selected item {item}");
}
```

### 3. 游戏配置使用 / Game Config Usage

```csharp
// 加载配置 / Load config
int configId = GameConfigManager.LoadGameConfig("mymod.games.txt");

// 获取偏移量 / Get offset
var offset = GameConfigManager.GetGameConfigOffset(configId, "m_iHealth");
if (offset.HasValue)
{
    Console.WriteLine($"Health offset: {offset.Value}");
}

// 获取键值 / Get key value
string engine = GameConfigManager.GetGameConfigKeyValue(configId, "engine");
```

### 4. Native函数注册 / Native Function Registration

```csharp
// 注册Native函数 / Register native function
NativeManager.RegisterNative("my_custom_native", MyNativeFunction);

private static int MyNativeFunction(int paramCount)
{
    // 获取参数 / Get parameters
    int param1 = NativeManager.GetNativeParam(0);
    string param2 = NativeManager.GetNativeString(1);
    
    // 处理逻辑 / Process logic
    Console.WriteLine($"Native called with: {param1}, {param2}");
    
    return 1; // 返回值 / Return value
}
```

### 5. 消息系统使用 / Message System Usage

```csharp
// 发送消息 / Send message
if (MessageManager.BeginMessage(msgType, msgDest, clientId))
{
    MessageManager.WriteByte(100);
    MessageManager.WriteString("Hello World");
    MessageManager.EndMessage();
}

// 注册消息钩子 / Register message hook
int hookId = MessageManager.RegisterMessage(msgId, OnMessageReceived);

private static void OnMessageReceived(int msgType, int msgDest, int entityId)
{
    Console.WriteLine($"Message received: Type={msgType}, Dest={msgDest}, Entity={entityId}");
}
```

### 6. 数据包使用 / DataPack Usage

```csharp
// 创建数据包 / Create data pack
int packId = DataPackManager.CreateDataPack();

// 写入数据 / Write data
DataPackManager.WritePackCell(packId, 42);
DataPackManager.WritePackFloat(packId, 3.14f);
DataPackManager.WritePackString(packId, "Hello DataPack");

// 重置位置 / Reset position
DataPackManager.ResetPack(packId);

// 读取数据 / Read data
int intValue = DataPackManager.ReadPackCell(packId);
float floatValue = DataPackManager.ReadPackFloat(packId);
string stringValue = DataPackManager.ReadPackString(packId);
```

## 编译和安装 / Build and Installation

### 前置要求 / Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 或 Visual Studio Code
- AMX Mod X 1.9.0 或更高版本

### 编译步骤 / Build Steps

1. 克隆仓库 / Clone repository
```bash
git clone https://github.com/your-repo/amxmodx-extended-csharp.git
cd amxmodx-extended-csharp
```

2. 编译C#库 / Build C# library
```bash
dotnet build AmxModXExtended.csproj --configuration Release
```

3. 编译C++桥接层 / Build C++ bridge layer
```bash
# 在AMX Mod X源码目录中编译
make
```

4. 安装文件 / Install files
```bash
# 复制DLL到AMX Mod X目录
cp bin/Release/AmxModXExtended.dll /path/to/amxmodx/
cp amxmodx_mm.dll /path/to/amxmodx/
```

## 示例项目 / Example Project

查看 `SamplePlugin.cs` 文件获取完整的使用示例，包括：

See `SamplePlugin.cs` file for complete usage examples, including:

- 插件初始化和清理 / Plugin initialization and cleanup
- 所有系统的综合使用 / Comprehensive usage of all systems
- 错误处理和最佳实践 / Error handling and best practices
- 公共API设计 / Public API design

## 文件结构 / File Structure

```
├── amxmodx/
│   ├── csharp_bridge.h          # C++桥接层头文件
│   ├── csharp_bridge.cpp        # C++桥接层实现
│   └── ...
├── AmxModXExtendedInterop.cs    # P/Invoke声明
├── AmxModXExtendedAPI.cs        # 高级管理类
├── AmxModXExtendedExamples.cs   # 使用示例
├── SamplePlugin.cs              # 完整示例插件
├── AmxModXExtended.csproj       # 项目文件
└── README_EXTENDED.md           # 本文档
```

## 贡献 / Contributing

欢迎贡献代码！请遵循以下准则：

Contributions are welcome! Please follow these guidelines:

1. 遵循现有的代码风格 / Follow existing code style
2. 添加适当的注释和文档 / Add appropriate comments and documentation
3. 包含单元测试 / Include unit tests
4. 更新README和CHANGELOG / Update README and CHANGELOG

## 许可证 / License

本项目采用 MIT 许可证。详见 LICENSE 文件。

This project is licensed under the MIT License. See LICENSE file for details.

## 支持 / Support

如有问题或建议，请：

For questions or suggestions, please:

1. 创建 GitHub Issue / Create a GitHub Issue
2. 访问 AMX Mod X 论坛 / Visit AMX Mod X forums
3. 加入 Discord 社区 / Join Discord community
