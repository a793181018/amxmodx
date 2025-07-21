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
- **核心AMX功能** / Core AMX Features - 插件管理、函数调用、Forward系统等

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

## 🎯 核心AMX功能 / Core AMX Features

### 插件管理 / Plugin Management

```csharp
// 获取插件数量
int pluginCount = CoreAmxManager.GetPluginsNum();

// 获取插件信息
var pluginInfo = CoreAmxManager.GetPluginInfo(0);
if (pluginInfo.HasValue)
{
    Console.WriteLine($"插件名称: {pluginInfo.Value.Name}");
    Console.WriteLine($"版本: {pluginInfo.Value.Version}");
    Console.WriteLine($"作者: {pluginInfo.Value.Author}");
}

// 查找插件
int pluginId = CoreAmxManager.FindPlugin("admin.amxx");

// 插件状态控制
bool isValid = CoreAmxManager.IsPluginValid(pluginId);
bool isRunning = CoreAmxManager.IsPluginRunning(pluginId);
CoreAmxManager.PausePlugin(pluginId);
CoreAmxManager.UnpausePlugin(pluginId);
```

### 函数调用系统 / Function Call System

```csharp
// 调用插件函数
if (CoreAmxManager.CallFuncBegin("my_function", "plugin.amxx"))
{
    CoreAmxManager.CallFuncPushInt(123);
    CoreAmxManager.CallFuncPushFloat(45.67f);
    CoreAmxManager.CallFuncPushString("Hello");
    CoreAmxManager.CallFuncPushArray(new int[] { 1, 2, 3 });

    int result = CoreAmxManager.CallFuncEnd();
    Console.WriteLine($"函数返回值: {result}");
}

// 通过ID调用函数
int funcId = CoreAmxManager.GetFuncId("function_name", pluginId);
if (CoreAmxManager.CallFuncBeginById(funcId, pluginId))
{
    CoreAmxManager.CallFuncPushString("参数");
    int result = CoreAmxManager.CallFuncEnd();
}
```

### Forward系统 / Forward System

```csharp
// 创建全局Forward
int forwardId = CoreAmxManager.CreateForward(
    "player_connect",
    CoreAmxManager.ForwardExecType.Ignore,
    CoreAmxManager.ForwardParamType.Cell,    // 玩家ID
    CoreAmxManager.ForwardParamType.String,  // 玩家名称
    CoreAmxManager.ForwardParamType.String   // IP地址
);

// 创建单插件Forward
int spForwardId = CoreAmxManager.CreateSPForward(
    "my_callback",
    pluginId,
    CoreAmxManager.ForwardParamType.Cell,
    CoreAmxManager.ForwardParamType.Float
);

// 执行Forward
int result = CoreAmxManager.ExecuteForward(forwardId, 1, 0, 0);

// 获取Forward信息
var forwardInfo = CoreAmxManager.GetForwardInfo(forwardId);
if (forwardInfo.HasValue)
{
    Console.WriteLine($"Forward名称: {forwardInfo.Value.Name}");
    Console.WriteLine($"参数数量: {forwardInfo.Value.ParamCount}");
}

// 销毁Forward
CoreAmxManager.DestroyForward(forwardId);
```

### 服务器管理 / Server Management

```csharp
// 服务器信息
bool isDedicated = CoreAmxManager.IsDedicatedServer();
bool isLinux = CoreAmxManager.IsLinuxServer();

// 服务器输出和命令
CoreAmxManager.ServerPrint("服务器消息");
CoreAmxManager.ServerCmd("echo \"Hello World\"");
CoreAmxManager.ServerExec(); // 立即执行命令队列

// 地图验证
bool isMapValid = CoreAmxManager.IsMapValid("de_dust2");
```

### 客户端管理 / Client Management

```csharp
// 玩家信息
int playerCount = CoreAmxManager.GetPlayersNum();
int connectingCount = CoreAmxManager.GetPlayersNum(true);

// 玩家状态检查
bool isBot = CoreAmxManager.IsUserBot(clientId);
bool isConnected = CoreAmxManager.IsUserConnected(clientId);
bool isAlive = CoreAmxManager.IsUserAlive(clientId);
int playTime = CoreAmxManager.GetUserTime(clientId, true);

// 客户端命令
CoreAmxManager.ClientCmd(clientId, "say \"Hello\"");
CoreAmxManager.FakeClientCmd(clientId, "kill");
```

### 管理员管理 / Admin Management

```csharp
// 清空管理员列表
CoreAmxManager.AdminsFlush();

// 添加管理员
CoreAmxManager.AdminsPush("STEAM_0:1:12345", "password", 1023, 0);
CoreAmxManager.AdminsPush("192.168.1.100", "", 511, 1);

// 获取管理员信息
int adminCount = CoreAmxManager.AdminsNum();
for (int i = 0; i < adminCount; i++)
{
    var auth = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Auth) as string;
    var access = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Access);
    Console.WriteLine($"管理员: {auth}, 权限: {access}");
}
```

### 日志管理 / Logging Management

```csharp
// 基本日志记录
CoreAmxManager.LogAmx("AMX日志消息");
CoreAmxManager.LogToFile("custom.log", "自定义日志");
CoreAmxManager.LogError(404, "错误消息");

// 注册日志回调
int callbackId = CoreAmxManager.RegisterLogCallback((level, message) =>
{
    Console.WriteLine($"[{level}] {message}");
});

// 取消注册回调
CoreAmxManager.UnregisterLogCallback(callbackId);
```

### 库管理 / Library Management

```csharp
// 注册库
CoreAmxManager.RegisterLibrary("my_library");

// 检查库是否存在
bool exists = CoreAmxManager.LibraryExists("my_library");
```

### 工具函数 / Utility Functions

```csharp
// 数学函数
int min = CoreAmxManager.MinInt(10, 20);
int max = CoreAmxManager.MaxInt(10, 20);
int clamped = CoreAmxManager.ClampInt(25, 10, 20);
int random = CoreAmxManager.RandomInt(100);

// 字符串工具
string swapped = CoreAmxManager.SwapChars("Hello", 'l', 'L');

// 系统信息
int heapSpace = CoreAmxManager.GetHeapSpace();
int numArgs = CoreAmxManager.GetNumArgs();

// 执行控制
CoreAmxManager.AbortExecution(500, "严重错误");
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
