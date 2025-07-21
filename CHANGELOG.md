# 更新日志 / Changelog

本文档记录了 AMX Mod X Extended C# API 的所有重要更改。

This document records all notable changes to AMX Mod X Extended C# API.

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
并且本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2024-01-21

### 新增 / Added

#### 🎯 核心AMX功能系统 / Core AMX Features System

**插件管理系统** / Plugin Management System
- ✅ `CoreAmxManager.GetPluginsNum()` - 获取插件数量 / Get plugins number
- ✅ `CoreAmxManager.GetPluginInfo()` - 获取插件详细信息 / Get plugin information
- ✅ `CoreAmxManager.FindPlugin()` - 查找插件 / Find plugin
- ✅ `CoreAmxManager.IsPluginValid()` - 检查插件是否有效 / Check if plugin is valid
- ✅ `CoreAmxManager.IsPluginRunning()` - 检查插件是否运行 / Check if plugin is running
- ✅ `CoreAmxManager.PausePlugin()` - 暂停插件 / Pause plugin
- ✅ `CoreAmxManager.UnpausePlugin()` - 恢复插件 / Unpause plugin

**函数调用系统** / Function Call System
- ✅ `CoreAmxManager.CallFuncBegin()` - 开始函数调用 / Begin function call
- ✅ `CoreAmxManager.CallFuncBeginById()` - 通过ID开始函数调用 / Begin function call by ID
- ✅ `CoreAmxManager.CallFuncPushInt()` - 压入整数参数 / Push integer parameter
- ✅ `CoreAmxManager.CallFuncPushFloat()` - 压入浮点参数 / Push float parameter
- ✅ `CoreAmxManager.CallFuncPushString()` - 压入字符串参数 / Push string parameter
- ✅ `CoreAmxManager.CallFuncPushArray()` - 压入数组参数 / Push array parameter
- ✅ `CoreAmxManager.CallFuncEnd()` - 结束函数调用 / End function call
- ✅ `CoreAmxManager.GetFuncId()` - 获取函数ID / Get function ID

**Forward系统** / Forward System
- ✅ `CoreAmxManager.CreateForward()` - 创建全局Forward / Create global forward
- ✅ `CoreAmxManager.CreateSPForward()` - 创建单插件Forward / Create single plugin forward
- ✅ `CoreAmxManager.ExecuteForward()` - 执行Forward / Execute forward
- ✅ `CoreAmxManager.GetForwardInfo()` - 获取Forward信息 / Get forward information
- ✅ `CoreAmxManager.DestroyForward()` - 销毁Forward / Destroy forward

**服务器管理** / Server Management
- ✅ `CoreAmxManager.ServerPrint()` - 服务器打印 / Server print
- ✅ `CoreAmxManager.ServerCmd()` - 服务器命令 / Server command
- ✅ `CoreAmxManager.ServerExec()` - 执行服务器命令 / Execute server command
- ✅ `CoreAmxManager.IsDedicatedServer()` - 检查是否专用服务器 / Check if dedicated server
- ✅ `CoreAmxManager.IsLinuxServer()` - 检查是否Linux服务器 / Check if Linux server
- ✅ `CoreAmxManager.IsMapValid()` - 检查地图有效性 / Check map validity

**客户端管理** / Client Management
- ✅ `CoreAmxManager.GetPlayersNum()` - 获取玩家数量 / Get players number
- ✅ `CoreAmxManager.IsUserBot()` - 检查是否机器人 / Check if user is bot
- ✅ `CoreAmxManager.IsUserConnected()` - 检查用户连接状态 / Check if user is connected
- ✅ `CoreAmxManager.IsUserAlive()` - 检查用户存活状态 / Check if user is alive
- ✅ `CoreAmxManager.GetUserTime()` - 获取用户时间 / Get user time
- ✅ `CoreAmxManager.ClientCmd()` - 客户端命令 / Client command
- ✅ `CoreAmxManager.FakeClientCmd()` - 虚假客户端命令 / Fake client command

**管理员管理** / Admin Management
- ✅ `CoreAmxManager.AdminsPush()` - 添加管理员 / Add admin
- ✅ `CoreAmxManager.AdminsFlush()` - 清空管理员列表 / Flush admins list
- ✅ `CoreAmxManager.AdminsNum()` - 获取管理员数量 / Get admins number
- ✅ `CoreAmxManager.AdminsLookup()` - 查找管理员信息 / Lookup admin information

**日志管理** / Logging Management
- ✅ `CoreAmxManager.LogAmx()` - AMX日志 / AMX log
- ✅ `CoreAmxManager.LogToFile()` - 记录到文件 / Log to file
- ✅ `CoreAmxManager.LogError()` - 记录错误 / Log error
- ✅ `CoreAmxManager.RegisterLogCallback()` - 注册日志回调 / Register log callback
- ✅ `CoreAmxManager.UnregisterLogCallback()` - 取消日志回调 / Unregister log callback

**库管理** / Library Management
- ✅ `CoreAmxManager.RegisterLibrary()` - 注册库 / Register library
- ✅ `CoreAmxManager.LibraryExists()` - 检查库存在 / Check if library exists

**工具函数** / Utility Functions
- ✅ `CoreAmxManager.MinInt()` / `MaxInt()` / `ClampInt()` - 数学函数 / Math functions
- ✅ `CoreAmxManager.RandomInt()` - 随机整数 / Random integer
- ✅ `CoreAmxManager.SwapChars()` - 字符交换 / Swap characters
- ✅ `CoreAmxManager.GetHeapSpace()` - 获取堆空间 / Get heap space
- ✅ `CoreAmxManager.GetNumArgs()` - 获取参数数量 / Get number of arguments
- ✅ `CoreAmxManager.AbortExecution()` - 中止执行 / Abort execution

### 改进 / Improved

**C++桥接层扩展** / Extended C++ Bridge Layer
- 新增70+个核心AMX功能接口 / Added 70+ core AMX functionality interfaces
- 完善的错误处理和参数验证 / Comprehensive error handling and parameter validation
- 跨平台兼容性支持 / Cross-platform compatibility support
- 线程安全的回调处理 / Thread-safe callback handling

**C#管理类扩展** / Extended C# Manager Classes
- 新增 `CoreAmxManager` 静态类 / Added CoreAmxManager static class
- 完整的XML文档注释（中英文）/ Complete XML documentation (Chinese/English)
- 类型安全的枚举定义 / Type-safe enum definitions
- 改进的异常处理 / Improved exception handling

**示例和文档** / Examples and Documentation
- 扩展的使用示例和演示代码 / Extended usage examples and demo code
- 更新的README文档 / Updated README documentation
- 完整的API参考 / Complete API reference
- 新增核心AMX功能演示 / Added core AMX features demonstration

### 技术细节 / Technical Details

**新增结构体** / New Structures
- `PluginInfo` - 插件信息结构 / Plugin information structure
- `ForwardInfo` - Forward信息结构 / Forward information structure
- `LogCallback` / `CallFuncCallback` - 回调委托 / Callback delegates

**内部优化** / Internal Optimizations
- 改进的内存管理 / Improved memory management
- 优化的字符串处理 / Optimized string handling
- 增强的错误报告 / Enhanced error reporting

## [1.0.0] - 2024-01-20

### 新增 / Added

#### CVar系统 / CVar System
- ✅ `CvarManager.CreateCvar()` - 创建新的CVar / Create new CVar
- ✅ `CvarManager.CvarExists()` - 检查CVar是否存在 / Check if CVar exists
- ✅ `CvarManager.GetCvarString()` - 获取CVar字符串值 / Get CVar string value
- ✅ `CvarManager.SetCvarString()` - 设置CVar字符串值 / Set CVar string value
- ✅ `CvarManager.GetCvarInt()` - 获取CVar整数值 / Get CVar integer value
- ✅ `CvarManager.SetCvarInt()` - 设置CVar整数值 / Set CVar integer value
- ✅ `CvarManager.GetCvarFloat()` - 获取CVar浮点值 / Get CVar float value
- ✅ `CvarManager.SetCvarFloat()` - 设置CVar浮点值 / Set CVar float value
- ✅ `CvarManager.GetCvarFlags()` - 获取CVar标志 / Get CVar flags
- ✅ `CvarManager.SetCvarFlags()` - 设置CVar标志 / Set CVar flags
- ✅ `CvarManager.HookCvarChange()` - 钩子CVar变化 / Hook CVar change
- ✅ `CvarManager.UnhookCvarChange()` - 取消钩子CVar变化 / Unhook CVar change

#### 菜单系统 / Menu System
- ✅ `MenuManager.CreateMenu()` - 创建菜单 / Create menu
- ✅ `MenuManager.AddMenuItem()` - 添加菜单项 / Add menu item
- ✅ `MenuManager.AddMenuBlank()` - 添加空行 / Add blank line
- ✅ `MenuManager.AddMenuText()` - 添加文本 / Add text
- ✅ `MenuManager.DisplayMenu()` - 显示菜单 / Display menu
- ✅ `MenuManager.DestroyMenu()` - 销毁菜单 / Destroy menu
- ✅ `MenuManager.GetMenuInfo()` - 获取菜单信息 / Get menu information
- ✅ `MenuManager.GetMenuPages()` - 获取菜单页数 / Get menu pages
- ✅ `MenuManager.GetMenuItems()` - 获取菜单项数 / Get menu items

#### 游戏配置系统 / Game Config System
- ✅ `GameConfigManager.LoadGameConfig()` - 加载游戏配置 / Load game config
- ✅ `GameConfigManager.GetGameConfigOffset()` - 获取配置偏移量 / Get config offset
- ✅ `GameConfigManager.GetGameConfigAddress()` - 获取配置地址 / Get config address
- ✅ `GameConfigManager.GetGameConfigKeyValue()` - 获取配置键值 / Get config key value
- ✅ `GameConfigManager.CloseGameConfig()` - 关闭游戏配置 / Close game config

#### Native函数管理 / Native Function Management
- ✅ `NativeManager.RegisterNative()` - 注册Native函数 / Register native function
- ✅ `NativeManager.UnregisterNative()` - 取消注册Native函数 / Unregister native function
- ✅ `NativeManager.GetNativeParam()` - 获取Native参数 / Get native parameter
- ✅ `NativeManager.GetNativeString()` - 获取Native字符串参数 / Get native string parameter
- ✅ `NativeManager.SetNativeString()` - 设置Native字符串参数 / Set native string parameter
- ✅ `NativeManager.GetNativeArray()` - 获取Native数组参数 / Get native array parameter
- ✅ `NativeManager.SetNativeArray()` - 设置Native数组参数 / Set native array parameter

#### 消息系统 / Message System
- ✅ `MessageManager.BeginMessage()` - 开始消息 / Begin message
- ✅ `MessageManager.EndMessage()` - 结束消息 / End message
- ✅ `MessageManager.WriteByte()` - 写入字节 / Write byte
- ✅ `MessageManager.WriteChar()` - 写入字符 / Write char
- ✅ `MessageManager.WriteShort()` - 写入短整数 / Write short
- ✅ `MessageManager.WriteLong()` - 写入长整数 / Write long
- ✅ `MessageManager.WriteAngle()` - 写入角度 / Write angle
- ✅ `MessageManager.WriteCoord()` - 写入坐标 / Write coordinate
- ✅ `MessageManager.WriteString()` - 写入字符串 / Write string
- ✅ `MessageManager.WriteEntity()` - 写入实体 / Write entity
- ✅ `MessageManager.RegisterMessage()` - 注册消息钩子 / Register message hook
- ✅ `MessageManager.UnregisterMessage()` - 取消注册消息钩子 / Unregister message hook

#### 数据包系统 / DataPack System
- ✅ `DataPackManager.CreateDataPack()` - 创建数据包 / Create data pack
- ✅ `DataPackManager.WritePackCell()` - 写入整数到数据包 / Write cell to data pack
- ✅ `DataPackManager.WritePackFloat()` - 写入浮点数到数据包 / Write float to data pack
- ✅ `DataPackManager.WritePackString()` - 写入字符串到数据包 / Write string to data pack
- ✅ `DataPackManager.ReadPackCell()` - 从数据包读取整数 / Read cell from data pack
- ✅ `DataPackManager.ReadPackFloat()` - 从数据包读取浮点数 / Read float from data pack
- ✅ `DataPackManager.ReadPackString()` - 从数据包读取字符串 / Read string from data pack
- ✅ `DataPackManager.ResetPack()` - 重置数据包 / Reset data pack
- ✅ `DataPackManager.GetPackPosition()` - 获取数据包位置 / Get pack position
- ✅ `DataPackManager.SetPackPosition()` - 设置数据包位置 / Set pack position
- ✅ `DataPackManager.IsPackEnded()` - 检查数据包是否结束 / Check if pack is ended
- ✅ `DataPackManager.DestroyDataPack()` - 销毁数据包 / Destroy data pack

#### 核心功能 / Core Features
- ✅ 完整的C++桥接层实现 / Complete C++ bridge layer implementation
- ✅ 跨平台兼容性支持 / Cross-platform compatibility support
- ✅ 线程安全的内存管理 / Thread-safe memory management
- ✅ 自动字符串长度处理 / Automatic string length handling
- ✅ 委托回调系统 / Delegate callback system
- ✅ 完整的XML文档注释 / Complete XML documentation comments
- ✅ 大驼峰命名规范 / PascalCase naming convention

#### 示例和文档 / Examples and Documentation
- ✅ `SamplePlugin.cs` - 完整的示例插件 / Complete sample plugin
- ✅ `AmxModXExtendedExamples.cs` - 各系统使用示例 / Usage examples for all systems
- ✅ `README_EXTENDED.md` - 详细的使用文档 / Detailed usage documentation
- ✅ 中英文双语注释 / Bilingual comments (Chinese/English)

### 技术实现 / Technical Implementation

#### C++桥接层 / C++ Bridge Layer
- ✅ `csharp_bridge.h` - 桥接层头文件定义 / Bridge layer header definitions
- ✅ `csharp_bridge.cpp` - 桥接层完整实现 / Complete bridge layer implementation
- ✅ 内存安全管理 / Memory safety management
- ✅ 异常处理机制 / Exception handling mechanism
- ✅ 资源自动清理 / Automatic resource cleanup

#### C#互操作层 / C# Interop Layer
- ✅ `AmxModXExtendedInterop.cs` - P/Invoke声明 / P/Invoke declarations
- ✅ 结构体定义和封送 / Structure definitions and marshaling
- ✅ 委托类型定义 / Delegate type definitions
- ✅ 平台调用约定 / Platform calling conventions

#### 高级管理类 / High-level Manager Classes
- ✅ `AmxModXExtendedAPI.cs` - 管理类实现 / Manager class implementations
- ✅ 错误处理和验证 / Error handling and validation
- ✅ 资源生命周期管理 / Resource lifecycle management
- ✅ 类型安全的API设计 / Type-safe API design

### 已知问题 / Known Issues
- ⚠️ 某些Native函数实现需要与AMX上下文集成 / Some native function implementations need AMX context integration
- ⚠️ 消息系统的参数读取功能需要完善 / Message system parameter reading needs improvement
- ⚠️ 游戏配置的边界检查需要加强 / Game config bounds checking needs enhancement

### 计划功能 / Planned Features
- 🔄 字符串处理系统适配 / String processing system adaptation
- 🔄 文件操作系统适配 / File operation system adaptation
- 🔄 数据结构系统适配 / Data structure system adaptation
- 🔄 浮点数运算系统适配 / Float operation system adaptation
- 🔄 向量计算系统适配 / Vector calculation system adaptation

---

## 版本说明 / Version Notes

### 版本号格式 / Version Format
- **主版本号** / Major: 不兼容的API更改 / Incompatible API changes
- **次版本号** / Minor: 向后兼容的功能添加 / Backward compatible feature additions  
- **修订号** / Patch: 向后兼容的问题修复 / Backward compatible bug fixes

### 状态标识 / Status Indicators
- ✅ 已完成 / Completed
- 🔄 进行中 / In Progress
- ⚠️ 已知问题 / Known Issue
- ❌ 未实现 / Not Implemented
- 📋 计划中 / Planned

### 优先级 / Priority
- 🔥 极高 / Critical
- ⚡ 高 / High
- 📋 中 / Medium
- 📝 低 / Low

---

## 贡献者 / Contributors

感谢所有为本项目做出贡献的开发者！

Thanks to all developers who contributed to this project!

- AMX Mod X Team - 核心架构设计 / Core architecture design
- Community Contributors - 测试和反馈 / Testing and feedback

---

## 反馈和建议 / Feedback and Suggestions

如果您有任何问题、建议或发现了bug，请通过以下方式联系我们：

If you have any questions, suggestions, or found bugs, please contact us through:

1. GitHub Issues - 问题报告和功能请求 / Issue reports and feature requests
2. AMX Mod X Forums - 社区讨论 / Community discussions
3. Discord - 实时交流 / Real-time communication
