// AmxModXExtendedExamples.cs - Usage examples for AMX Mod X Extended C# API
// 展示如何使用扩展的C# API进行各种系统操作

using System;
using AmxModX;
using AmxModX.Interop;

namespace AmxModX.Examples
{
    /// <summary>
    /// CVar系统使用示例 / CVar system usage examples
    /// </summary>
    public static class CvarExamples
    {
        /// <summary>
        /// CVar基本操作示例 / Basic CVar operations example
        /// </summary>
        public static void BasicCvarOperations()
        {
            // 创建一个新的CVar / Create a new CVar
            int cvarId = CvarManager.CreateCvar("my_plugin_enabled", "1", 0, "Enable/disable my plugin");
            if (cvarId != -1)
            {
                Console.WriteLine("CVar created successfully with ID: " + cvarId);
            }

            // 检查CVar是否存在 / Check if CVar exists
            if (CvarManager.CvarExists("my_plugin_enabled"))
            {
                Console.WriteLine("CVar exists!");

                // 获取CVar值 / Get CVar value
                string stringValue = CvarManager.GetCvarString("my_plugin_enabled");
                int intValue = CvarManager.GetCvarInt("my_plugin_enabled");
                float floatValue = CvarManager.GetCvarFloat("my_plugin_enabled");

                Console.WriteLine($"String value: {stringValue}");
                Console.WriteLine($"Int value: {intValue}");
                Console.WriteLine($"Float value: {floatValue}");

                // 设置CVar值 / Set CVar value
                CvarManager.SetCvarString("my_plugin_enabled", "0");
                CvarManager.SetCvarInt("my_plugin_enabled", 0);
                CvarManager.SetCvarFloat("my_plugin_enabled", 0.0f);
            }
        }

        /// <summary>
        /// CVar变化监听示例 / CVar change monitoring example
        /// </summary>
        public static void CvarChangeMonitoring()
        {
            // 钩子CVar变化 / Hook CVar change
            int hookId = CvarManager.HookCvarChange("mp_timelimit", OnTimeLimitChanged);
            if (hookId != -1)
            {
                Console.WriteLine("CVar change hook registered with ID: " + hookId);
            }

            // 在适当的时候取消钩子 / Unhook when appropriate
            // CvarManager.UnhookCvarChange(hookId);
        }

        /// <summary>
        /// 时间限制CVar变化回调 / Time limit CVar change callback
        /// </summary>
        private static void OnTimeLimitChanged(string cvarName, string oldValue, string newValue)
        {
            Console.WriteLine($"CVar {cvarName} changed from '{oldValue}' to '{newValue}'");
            
            // 可以在这里添加自定义逻辑 / Add custom logic here
            if (int.TryParse(newValue, out int timeLimit))
            {
                if (timeLimit > 60)
                {
                    Console.WriteLine("Warning: Time limit is very high!");
                }
            }
        }
    }

    /// <summary>
    /// 菜单系统使用示例 / Menu system usage examples
    /// </summary>
    public static class MenuExamples
    {
        private static int _mainMenuId = -1;

        /// <summary>
        /// 创建主菜单示例 / Create main menu example
        /// </summary>
        public static void CreateMainMenu()
        {
            // 创建菜单 / Create menu
            _mainMenuId = MenuManager.CreateMenu("Main Menu", OnMenuSelect, OnMenuCancel);
            if (_mainMenuId != -1)
            {
                Console.WriteLine("Menu created with ID: " + _mainMenuId);

                // 添加菜单项 / Add menu items
                MenuManager.AddMenuItem(_mainMenuId, "Option 1", "option1", 0);
                MenuManager.AddMenuItem(_mainMenuId, "Option 2", "option2", 0);
                MenuManager.AddMenuBlank(_mainMenuId); // 添加空行 / Add blank line
                MenuManager.AddMenuText(_mainMenuId, "--- Settings ---"); // 添加文本 / Add text
                MenuManager.AddMenuItem(_mainMenuId, "Settings", "settings", 0);
                MenuManager.AddMenuItem(_mainMenuId, "Exit", "exit", 0);

                // 获取菜单信息 / Get menu info
                var menuInfo = MenuManager.GetMenuInfo(_mainMenuId);
                if (menuInfo.HasValue)
                {
                    Console.WriteLine($"Menu title: {menuInfo.Value.Title}");
                    Console.WriteLine($"Item count: {menuInfo.Value.ItemCount}");
                    Console.WriteLine($"Page count: {menuInfo.Value.PageCount}");
                }
            }
        }

        /// <summary>
        /// 显示菜单给玩家 / Display menu to player
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        public static void ShowMenuToPlayer(int clientId)
        {
            if (_mainMenuId != -1)
            {
                bool success = MenuManager.DisplayMenu(_mainMenuId, clientId, 0);
                Console.WriteLine($"Menu display result: {success}");
            }
        }

        /// <summary>
        /// 菜单选择回调 / Menu selection callback
        /// </summary>
        private static void OnMenuSelect(int clientId, int menuId, int item)
        {
            Console.WriteLine($"Client {clientId} selected item {item} from menu {menuId}");

            // 根据选择的项目执行不同操作 / Execute different actions based on selected item
            switch (item)
            {
                case 0: // Option 1
                    Console.WriteLine("Option 1 selected");
                    break;
                case 1: // Option 2
                    Console.WriteLine("Option 2 selected");
                    break;
                case 3: // Settings (跳过空行和文本)
                    Console.WriteLine("Settings selected");
                    break;
                case 4: // Exit
                    Console.WriteLine("Exit selected");
                    break;
            }
        }

        /// <summary>
        /// 菜单取消回调 / Menu cancel callback
        /// </summary>
        private static void OnMenuCancel(int clientId, int menuId, int reason)
        {
            Console.WriteLine($"Client {clientId} cancelled menu {menuId}, reason: {reason}");
        }

        /// <summary>
        /// 清理菜单资源 / Cleanup menu resources
        /// </summary>
        public static void CleanupMenu()
        {
            if (_mainMenuId != -1)
            {
                MenuManager.DestroyMenu(_mainMenuId);
                _mainMenuId = -1;
                Console.WriteLine("Menu destroyed");
            }
        }
    }

    /// <summary>
    /// 游戏配置使用示例 / Game config usage examples
    /// </summary>
    public static class GameConfigExamples
    {
        /// <summary>
        /// 加载和使用游戏配置 / Load and use game config
        /// </summary>
        public static void LoadAndUseGameConfig()
        {
            // 加载游戏配置文件 / Load game config file
            int configId = GameConfigManager.LoadGameConfig("mymod.games.txt");
            if (configId != -1)
            {
                Console.WriteLine("Game config loaded with ID: " + configId);

                // 获取偏移量 / Get offset
                var offset = GameConfigManager.GetGameConfigOffset(configId, "m_iHealth");
                if (offset.HasValue)
                {
                    Console.WriteLine($"Health offset: {offset.Value}");
                }

                // 获取地址 / Get address
                IntPtr address = GameConfigManager.GetGameConfigAddress(configId, "CreateInterface");
                if (address != IntPtr.Zero)
                {
                    Console.WriteLine($"CreateInterface address: 0x{address.ToInt64():X}");
                }

                // 获取键值 / Get key value
                string keyValue = GameConfigManager.GetGameConfigKeyValue(configId, "engine");
                if (!string.IsNullOrEmpty(keyValue))
                {
                    Console.WriteLine($"Engine: {keyValue}");
                }

                // 使用完毕后关闭配置 / Close config when done
                GameConfigManager.CloseGameConfig(configId);
                Console.WriteLine("Game config closed");
            }
        }
    }

    /// <summary>
    /// Native函数使用示例 / Native function usage examples
    /// </summary>
    public static class NativeExamples
    {
        /// <summary>
        /// 注册自定义Native函数 / Register custom native function
        /// </summary>
        public static void RegisterCustomNatives()
        {
            // 注册Native函数 / Register native function
            bool success = NativeManager.RegisterNative("my_custom_native", MyCustomNative);
            if (success)
            {
                Console.WriteLine("Custom native registered successfully");
            }
        }

        /// <summary>
        /// 自定义Native函数实现 / Custom native function implementation
        /// </summary>
        private static int MyCustomNative(int paramCount)
        {
            Console.WriteLine($"MyCustomNative called with {paramCount} parameters");

            // 获取参数 / Get parameters
            if (paramCount > 0)
            {
                int param1 = NativeManager.GetNativeParam(0);
                Console.WriteLine($"Parameter 1: {param1}");
            }

            if (paramCount > 1)
            {
                string param2 = NativeManager.GetNativeString(1);
                Console.WriteLine($"Parameter 2: {param2}");
            }

            if (paramCount > 2)
            {
                int[] param3 = NativeManager.GetNativeArray(2, 10);
                Console.WriteLine($"Parameter 3 (array): [{string.Join(", ", param3)}]");
            }

            // 返回值 / Return value
            return 1;
        }
    }

    /// <summary>
    /// 消息系统使用示例 / Message system usage examples
    /// </summary>
    public static class MessageExamples
    {
        /// <summary>
        /// 发送自定义消息 / Send custom message
        /// </summary>
        public static void SendCustomMessage(int clientId)
        {
            // 开始消息 / Begin message
            if (MessageManager.BeginMessage(123, 1, clientId)) // 假设消息类型123，目标类型1
            {
                // 写入消息数据 / Write message data
                MessageManager.WriteByte(100);
                MessageManager.WriteShort(1000);
                MessageManager.WriteLong(100000);
                MessageManager.WriteFloat(3.14f);
                MessageManager.WriteString("Hello World");
                MessageManager.WriteEntity(clientId);

                // 结束消息 / End message
                MessageManager.EndMessage();
                Console.WriteLine("Custom message sent successfully");
            }
        }

        /// <summary>
        /// 注册消息钩子 / Register message hook
        /// </summary>
        public static void RegisterMessageHook()
        {
            // 注册消息钩子 / Register message hook
            int hookId = MessageManager.RegisterMessage(123, OnMessageReceived);
            if (hookId != -1)
            {
                Console.WriteLine("Message hook registered with ID: " + hookId);
            }
        }

        /// <summary>
        /// 消息接收回调 / Message received callback
        /// </summary>
        private static void OnMessageReceived(int msgType, int msgDest, int entityId)
        {
            Console.WriteLine($"Message received - Type: {msgType}, Dest: {msgDest}, Entity: {entityId}");
        }
    }

    /// <summary>
    /// 数据包使用示例 / Data pack usage examples
    /// </summary>
    public static class DataPackExamples
    {
        /// <summary>
        /// 数据包基本操作 / Basic data pack operations
        /// </summary>
        public static void BasicDataPackOperations()
        {
            // 创建数据包 / Create data pack
            int packId = DataPackManager.CreateDataPack();
            if (packId != -1)
            {
                Console.WriteLine("Data pack created with ID: " + packId);

                // 写入数据 / Write data
                DataPackManager.WritePackCell(packId, 42);
                DataPackManager.WritePackFloat(packId, 3.14f);
                DataPackManager.WritePackString(packId, "Hello DataPack");

                // 重置位置以便读取 / Reset position for reading
                DataPackManager.ResetPack(packId);

                // 读取数据 / Read data
                int cellValue = DataPackManager.ReadPackCell(packId);
                float floatValue = DataPackManager.ReadPackFloat(packId);
                string stringValue = DataPackManager.ReadPackString(packId);

                Console.WriteLine($"Read cell: {cellValue}");
                Console.WriteLine($"Read float: {floatValue}");
                Console.WriteLine($"Read string: {stringValue}");

                // 检查是否读取完毕 / Check if reading is complete
                bool isEnded = DataPackManager.IsPackEnded(packId);
                Console.WriteLine($"Pack ended: {isEnded}");

                // 销毁数据包 / Destroy data pack
                DataPackManager.DestroyDataPack(packId);
                Console.WriteLine("Data pack destroyed");
            }
        }

        /// <summary>
        /// 数据包位置操作 / Data pack position operations
        /// </summary>
        public static void DataPackPositionOperations()
        {
            int packId = DataPackManager.CreateDataPack();
            if (packId != -1)
            {
                // 写入一些数据 / Write some data
                DataPackManager.WritePackCell(packId, 1);
                DataPackManager.WritePackCell(packId, 2);
                DataPackManager.WritePackCell(packId, 3);

                // 获取当前位置 / Get current position
                int position = DataPackManager.GetPackPosition(packId);
                Console.WriteLine($"Current position: {position}");

                // 设置位置到开始 / Set position to beginning
                DataPackManager.SetPackPosition(packId, 0);

                // 读取第二个值 / Read second value
                DataPackManager.ReadPackCell(packId); // 跳过第一个 / Skip first
                int secondValue = DataPackManager.ReadPackCell(packId);
                Console.WriteLine($"Second value: {secondValue}");

                DataPackManager.DestroyDataPack(packId);
            }
        }
    }

    /// <summary>
    /// 核心AMX功能使用示例 / Core AMX functionality usage examples
    /// </summary>
    public static class CoreAmxExamples
    {
        /// <summary>
        /// 插件管理示例 / Plugin management example
        /// </summary>
        public static void PluginManagementExample()
        {
            Console.WriteLine("=== 插件管理示例 / Plugin Management Example ===");

            // 获取插件数量 / Get plugins number
            int pluginCount = CoreAmxManager.GetPluginsNum();
            Console.WriteLine($"当前加载的插件数量 / Current loaded plugins: {pluginCount}");

            // 遍历所有插件 / Iterate through all plugins
            for (int i = 0; i < pluginCount; i++)
            {
                var pluginInfo = CoreAmxManager.GetPluginInfo(i);
                if (pluginInfo.HasValue)
                {
                    var info = pluginInfo.Value;
                    Console.WriteLine($"插件 {i} / Plugin {i}:");
                    Console.WriteLine($"  名称 / Name: {info.Name}");
                    Console.WriteLine($"  文件 / File: {info.FileName}");
                    Console.WriteLine($"  版本 / Version: {info.Version}");
                    Console.WriteLine($"  作者 / Author: {info.Author}");
                    Console.WriteLine($"  状态 / Status: {info.Status}");
                    Console.WriteLine($"  是否运行 / Is Running: {info.IsRunning}");
                    Console.WriteLine($"  是否暂停 / Is Paused: {info.IsPaused}");
                }
            }

            // 查找特定插件 / Find specific plugin
            int pluginId = CoreAmxManager.FindPlugin("admin.amxx");
            if (pluginId >= 0)
            {
                Console.WriteLine($"找到admin.amxx插件，ID: {pluginId}");

                // 检查插件状态 / Check plugin status
                bool isValid = CoreAmxManager.IsPluginValid(pluginId);
                bool isRunning = CoreAmxManager.IsPluginRunning(pluginId);
                Console.WriteLine($"插件有效: {isValid}, 运行中: {isRunning}");
            }
        }

        /// <summary>
        /// 函数调用示例 / Function call example
        /// </summary>
        public static void FunctionCallExample()
        {
            Console.WriteLine("=== 函数调用示例 / Function Call Example ===");

            // 调用插件函数示例 / Call plugin function example
            if (CoreAmxManager.CallFuncBegin("my_custom_function", "myplugin.amxx"))
            {
                // 压入参数 / Push parameters
                CoreAmxManager.CallFuncPushInt(123);
                CoreAmxManager.CallFuncPushFloat(45.67f);
                CoreAmxManager.CallFuncPushString("Hello World");
                CoreAmxManager.CallFuncPushArray(new int[] { 1, 2, 3, 4, 5 });

                // 执行函数并获取返回值 / Execute function and get return value
                int result = CoreAmxManager.CallFuncEnd();
                Console.WriteLine($"函数返回值 / Function return value: {result}");
            }
            else
            {
                Console.WriteLine("函数调用失败 / Function call failed");
            }

            // 通过ID调用函数 / Call function by ID
            int funcId = CoreAmxManager.GetFuncId("another_function", 0);
            if (funcId >= 0)
            {
                if (CoreAmxManager.CallFuncBeginById(funcId, 0))
                {
                    CoreAmxManager.CallFuncPushString("Parameter");
                    int result = CoreAmxManager.CallFuncEnd();
                    Console.WriteLine($"通过ID调用函数返回值 / Function call by ID return value: {result}");
                }
            }
        }

        /// <summary>
        /// Forward系统示例 / Forward system example
        /// </summary>
        public static void ForwardSystemExample()
        {
            Console.WriteLine("=== Forward系统示例 / Forward System Example ===");

            // 创建全局Forward / Create global forward
            int forwardId = CoreAmxManager.CreateForward(
                "player_connect",
                CoreAmxManager.ForwardExecType.Ignore,
                CoreAmxManager.ForwardParamType.Cell,    // 玩家ID / Player ID
                CoreAmxManager.ForwardParamType.String,  // 玩家名称 / Player name
                CoreAmxManager.ForwardParamType.String   // IP地址 / IP address
            );

            if (forwardId >= 0)
            {
                Console.WriteLine($"创建Forward成功，ID: {forwardId}");

                // 获取Forward信息 / Get forward information
                var forwardInfo = CoreAmxManager.GetForwardInfo(forwardId);
                if (forwardInfo.HasValue)
                {
                    var info = forwardInfo.Value;
                    Console.WriteLine($"Forward名称: {info.Name}");
                    Console.WriteLine($"参数数量: {info.ParamCount}");
                    Console.WriteLine($"执行类型: {info.ExecType}");
                }

                // 执行Forward / Execute forward
                int result = CoreAmxManager.ExecuteForward(forwardId, 1, 0, 0); // 参数需要转换为int
                Console.WriteLine($"Forward执行结果: {result}");

                // 销毁Forward / Destroy forward
                CoreAmxManager.DestroyForward(forwardId);
                Console.WriteLine("Forward已销毁");
            }

            // 创建单插件Forward / Create single plugin forward
            int spForwardId = CoreAmxManager.CreateSPForward(
                "my_callback",
                0, // 插件ID
                CoreAmxManager.ForwardParamType.Cell,
                CoreAmxManager.ForwardParamType.Float
            );

            if (spForwardId >= 0)
            {
                Console.WriteLine($"创建单插件Forward成功，ID: {spForwardId}");
                CoreAmxManager.DestroyForward(spForwardId);
            }
        }

        /// <summary>
        /// 服务器管理示例 / Server management example
        /// </summary>
        public static void ServerManagementExample()
        {
            Console.WriteLine("=== 服务器管理示例 / Server Management Example ===");

            // 服务器信息 / Server information
            bool isDedicated = CoreAmxManager.IsDedicatedServer();
            bool isLinux = CoreAmxManager.IsLinuxServer();
            Console.WriteLine($"专用服务器 / Dedicated Server: {isDedicated}");
            Console.WriteLine($"Linux服务器 / Linux Server: {isLinux}");

            // 服务器打印 / Server print
            CoreAmxManager.ServerPrint("这是一条服务器消息 / This is a server message");

            // 检查地图有效性 / Check map validity
            bool isMapValid = CoreAmxManager.IsMapValid("de_dust2");
            Console.WriteLine($"de_dust2地图有效性 / de_dust2 map validity: {isMapValid}");

            // 执行服务器命令 / Execute server command
            CoreAmxManager.ServerCmd("echo \"Hello from C#\"");
            CoreAmxManager.ServerExec(); // 立即执行命令队列 / Execute command queue immediately
        }

        /// <summary>
        /// 客户端管理示例 / Client management example
        /// </summary>
        public static void ClientManagementExample()
        {
            Console.WriteLine("=== 客户端管理示例 / Client Management Example ===");

            // 获取玩家信息 / Get player information
            int playerCount = CoreAmxManager.GetPlayersNum();
            int connectingCount = CoreAmxManager.GetPlayersNum(true);
            Console.WriteLine($"在线玩家数 / Online players: {playerCount}");
            Console.WriteLine($"包含连接中玩家数 / Including connecting players: {connectingCount}");

            // 检查特定玩家 / Check specific player
            int clientId = 1; // 假设玩家ID为1 / Assume player ID is 1
            if (CoreAmxManager.IsUserConnected(clientId))
            {
                bool isBot = CoreAmxManager.IsUserBot(clientId);
                bool isAlive = CoreAmxManager.IsUserAlive(clientId);
                int playTime = CoreAmxManager.GetUserTime(clientId, true);

                Console.WriteLine($"玩家 {clientId} / Player {clientId}:");
                Console.WriteLine($"  是机器人 / Is Bot: {isBot}");
                Console.WriteLine($"  存活状态 / Alive Status: {isAlive}");
                Console.WriteLine($"  游戏时间 / Play Time: {playTime} 秒 / seconds");

                // 向玩家发送命令 / Send command to player
                CoreAmxManager.ClientCmd(clientId, "say \"Hello from C#\"");

                // 模拟玩家命令 / Simulate player command
                CoreAmxManager.FakeClientCmd(clientId, "kill");
            }
        }

        /// <summary>
        /// 管理员管理示例 / Admin management example
        /// </summary>
        public static void AdminManagementExample()
        {
            Console.WriteLine("=== 管理员管理示例 / Admin Management Example ===");

            // 清空现有管理员 / Clear existing admins
            CoreAmxManager.AdminsFlush();
            Console.WriteLine("已清空管理员列表 / Admin list cleared");

            // 添加管理员 / Add admins
            CoreAmxManager.AdminsPush("STEAM_0:1:12345", "password123", 1023, 0); // SteamID管理员
            CoreAmxManager.AdminsPush("192.168.1.100", "", 511, 1); // IP管理员
            CoreAmxManager.AdminsPush("AdminName", "admin_pass", 255, 2); // 名称管理员

            int adminCount = CoreAmxManager.AdminsNum();
            Console.WriteLine($"管理员数量 / Admin count: {adminCount}");

            // 遍历管理员信息 / Iterate through admin information
            for (int i = 0; i < adminCount; i++)
            {
                var auth = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Auth) as string;
                var password = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Password) as string;
                var access = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Access);
                var flags = CoreAmxManager.AdminsLookup(i, CoreAmxManager.AdminProperty.Flags);

                Console.WriteLine($"管理员 {i} / Admin {i}:");
                Console.WriteLine($"  认证 / Auth: {auth}");
                Console.WriteLine($"  密码 / Password: {(string.IsNullOrEmpty(password) ? "无 / None" : "***")}");
                Console.WriteLine($"  权限 / Access: {access}");
                Console.WriteLine($"  标志 / Flags: {flags}");
            }
        }

        /// <summary>
        /// 日志管理示例 / Logging management example
        /// </summary>
        public static void LoggingManagementExample()
        {
            Console.WriteLine("=== 日志管理示例 / Logging Management Example ===");

            // 基本日志记录 / Basic logging
            CoreAmxManager.LogAmx("这是一条AMX日志消息 / This is an AMX log message");
            CoreAmxManager.LogToFile("custom.log", "自定义日志文件消息 / Custom log file message");
            CoreAmxManager.LogError(404, "找不到指定的资源 / Resource not found");

            // 注册日志回调 / Register log callback
            int callbackId = CoreAmxManager.RegisterLogCallback((level, message) =>
            {
                string levelStr = level switch
                {
                    CoreAmxManager.LogLevel.Debug => "调试 / DEBUG",
                    CoreAmxManager.LogLevel.Info => "信息 / INFO",
                    CoreAmxManager.LogLevel.Warning => "警告 / WARNING",
                    CoreAmxManager.LogLevel.Error => "错误 / ERROR",
                    CoreAmxManager.LogLevel.Fatal => "致命 / FATAL",
                    _ => "未知 / UNKNOWN"
                };

                Console.WriteLine($"[{levelStr}] {message}");
            });

            if (callbackId >= 0)
            {
                Console.WriteLine($"日志回调注册成功，ID: {callbackId}");

                // 模拟一些日志事件 / Simulate some log events
                CoreAmxManager.LogAmx("测试日志回调 / Test log callback");
                CoreAmxManager.LogError(500, "服务器内部错误 / Internal server error");

                // 取消注册回调 / Unregister callback
                CoreAmxManager.UnregisterLogCallback(callbackId);
                Console.WriteLine("日志回调已取消注册 / Log callback unregistered");
            }
        }

        /// <summary>
        /// 库管理示例 / Library management example
        /// </summary>
        public static void LibraryManagementExample()
        {
            Console.WriteLine("=== 库管理示例 / Library Management Example ===");

            // 注册库 / Register libraries
            CoreAmxManager.RegisterLibrary("my_custom_library");
            CoreAmxManager.RegisterLibrary("database_helper");
            CoreAmxManager.RegisterLibrary("utility_functions");

            // 检查库是否存在 / Check if libraries exist
            string[] libraries = { "my_custom_library", "database_helper", "non_existent_lib" };

            foreach (string lib in libraries)
            {
                bool exists = CoreAmxManager.LibraryExists(lib);
                Console.WriteLine($"库 '{lib}' 存在 / Library '{lib}' exists: {exists}");
            }
        }

        /// <summary>
        /// 工具函数示例 / Utility functions example
        /// </summary>
        public static void UtilityFunctionsExample()
        {
            Console.WriteLine("=== 工具函数示例 / Utility Functions Example ===");

            // 数学工具函数 / Math utility functions
            int a = 10, b = 20;
            Console.WriteLine($"Min({a}, {b}) = {CoreAmxManager.MinInt(a, b)}");
            Console.WriteLine($"Max({a}, {b}) = {CoreAmxManager.MaxInt(a, b)}");
            Console.WriteLine($"Clamp(25, {a}, {b}) = {CoreAmxManager.ClampInt(25, a, b)}");
            Console.WriteLine($"Clamp(5, {a}, {b}) = {CoreAmxManager.ClampInt(5, a, b)}");

            // 随机数生成 / Random number generation
            Console.WriteLine("随机数示例 / Random number examples:");
            for (int i = 0; i < 5; i++)
            {
                int random = CoreAmxManager.RandomInt(100);
                Console.WriteLine($"  随机数 {i + 1} / Random {i + 1}: {random}");
            }

            // 字符串工具 / String utilities
            string text = "Hello World!";
            string swapped = CoreAmxManager.SwapChars(text, 'l', 'L');
            Console.WriteLine($"原文本 / Original: {text}");
            Console.WriteLine($"交换后 / Swapped: {swapped}");

            // 系统信息 / System information
            int heapSpace = CoreAmxManager.GetHeapSpace();
            int numArgs = CoreAmxManager.GetNumArgs();
            Console.WriteLine($"堆空间 / Heap space: {heapSpace} bytes");
            Console.WriteLine($"参数数量 / Number of args: {numArgs}");
        }

        /// <summary>
        /// 综合示例 / Comprehensive example
        /// </summary>
        public static void ComprehensiveExample()
        {
            Console.WriteLine("=== 综合示例 / Comprehensive Example ===");

            try
            {
                // 1. 初始化日志系统 / Initialize logging system
                int logCallbackId = CoreAmxManager.RegisterLogCallback((level, message) =>
                {
                    Console.WriteLine($"[LOG-{level}] {DateTime.Now:HH:mm:ss} - {message}");
                });

                // 2. 检查服务器状态 / Check server status
                CoreAmxManager.LogAmx("开始服务器状态检查 / Starting server status check");

                bool isDedicated = CoreAmxManager.IsDedicatedServer();
                bool isLinux = CoreAmxManager.IsLinuxServer();
                int playerCount = CoreAmxManager.GetPlayersNum();

                CoreAmxManager.LogAmx($"服务器类型: {(isDedicated ? "专用" : "监听")} / Server type: {(isDedicated ? "Dedicated" : "Listen")}");
                CoreAmxManager.LogAmx($"操作系统: {(isLinux ? "Linux" : "Windows")} / OS: {(isLinux ? "Linux" : "Windows")}");
                CoreAmxManager.LogAmx($"在线玩家: {playerCount} / Online players: {playerCount}");

                // 3. 管理插件 / Manage plugins
                int pluginCount = CoreAmxManager.GetPluginsNum();
                CoreAmxManager.LogAmx($"加载的插件数量: {pluginCount} / Loaded plugins: {pluginCount}");

                // 4. 设置管理员 / Setup admins
                CoreAmxManager.AdminsFlush();
                CoreAmxManager.AdminsPush("STEAM_0:1:12345", "", 1023, 0);
                CoreAmxManager.LogAmx("管理员配置完成 / Admin configuration completed");

                // 5. 注册库 / Register libraries
                CoreAmxManager.RegisterLibrary("csharp_integration");
                CoreAmxManager.LogAmx("C#集成库已注册 / C# integration library registered");

                // 6. 执行一些服务器命令 / Execute some server commands
                CoreAmxManager.ServerCmd("echo \"C# Integration Active\"");
                CoreAmxManager.ServerExec();

                CoreAmxManager.LogAmx("综合示例执行完成 / Comprehensive example completed");

                // 清理 / Cleanup
                CoreAmxManager.UnregisterLogCallback(logCallbackId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"综合示例执行出错 / Error in comprehensive example: {ex.Message}");
            }
        }

        /// <summary>
        /// 运行所有示例 / Run all examples
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("开始运行核心AMX功能示例 / Starting Core AMX functionality examples");
            Console.WriteLine(new string('=', 60));

            try
            {
                PluginManagementExample();
                Console.WriteLine();

                FunctionCallExample();
                Console.WriteLine();

                ForwardSystemExample();
                Console.WriteLine();

                ServerManagementExample();
                Console.WriteLine();

                ClientManagementExample();
                Console.WriteLine();

                AdminManagementExample();
                Console.WriteLine();

                LoggingManagementExample();
                Console.WriteLine();

                LibraryManagementExample();
                Console.WriteLine();

                UtilityFunctionsExample();
                Console.WriteLine();

                ComprehensiveExample();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"示例执行出错 / Error running examples: {ex.Message}");
                Console.WriteLine($"堆栈跟踪 / Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine(new string('=', 60));
            Console.WriteLine("核心AMX功能示例执行完成 / Core AMX functionality examples completed");
        }
    }
}
