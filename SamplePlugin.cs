// SamplePlugin.cs - 完整的AMX Mod X C#插件示例
// Complete AMX Mod X C# Plugin Example

using System;
using System.Collections.Generic;
using AmxModX;
using AmxModX.Interop;

namespace SamplePlugin
{
    /// <summary>
    /// 示例插件主类 / Sample plugin main class
    /// </summary>
    public class SamplePlugin
    {
        // 插件信息 / Plugin information
        private const string PLUGIN_NAME = "Sample C# Plugin";
        private const string PLUGIN_VERSION = "1.0.0";
        private const string PLUGIN_AUTHOR = "AMX Mod X Team";

        // 菜单和配置ID / Menu and config IDs
        private static int _mainMenuId = -1;
        private static int _gameConfigId = -1;
        private static int _dataPackId = -1;

        // 回调钩子ID / Callback hook IDs
        private static readonly List<int> _cvarHooks = new List<int>();
        private static readonly List<int> _messageHooks = new List<int>();

        /// <summary>
        /// 插件初始化 / Plugin initialization
        /// </summary>
        public static void OnPluginInit()
        {
            Console.WriteLine($"[{PLUGIN_NAME}] Version {PLUGIN_VERSION} by {PLUGIN_AUTHOR} loaded!");

            // 初始化各个系统 / Initialize systems
            InitializeCvars();
            InitializeMenus();
            InitializeGameConfig();
            InitializeNatives();
            InitializeMessages();
            InitializeDataPacks();

            Console.WriteLine($"[{PLUGIN_NAME}] All systems initialized successfully!");
        }

        /// <summary>
        /// 插件卸载 / Plugin unload
        /// </summary>
        public static void OnPluginEnd()
        {
            Console.WriteLine($"[{PLUGIN_NAME}] Cleaning up...");

            // 清理资源 / Cleanup resources
            CleanupCvars();
            CleanupMenus();
            CleanupGameConfig();
            CleanupMessages();
            CleanupDataPacks();

            Console.WriteLine($"[{PLUGIN_NAME}] Cleanup completed!");
        }

        #region CVar System Example

        /// <summary>
        /// 初始化CVar系统 / Initialize CVar system
        /// </summary>
        private static void InitializeCvars()
        {
            Console.WriteLine("[CVar] Initializing CVar system...");

            // 创建插件专用CVar / Create plugin-specific CVars
            int pluginEnabledId = CvarManager.CreateCvar("sample_plugin_enabled", "1", 0, "Enable/disable sample plugin");
            int debugModeId = CvarManager.CreateCvar("sample_debug_mode", "0", 0, "Enable debug mode for sample plugin");
            int maxPlayersId = CvarManager.CreateCvar("sample_max_players", "32", 0, "Maximum players for sample plugin features");

            if (pluginEnabledId != -1)
                Console.WriteLine("[CVar] Created sample_plugin_enabled CVar");

            if (debugModeId != -1)
                Console.WriteLine("[CVar] Created sample_debug_mode CVar");

            if (maxPlayersId != -1)
                Console.WriteLine("[CVar] Created sample_max_players CVar");

            // 钩子重要CVar的变化 / Hook important CVar changes
            int hook1 = CvarManager.HookCvarChange("sample_plugin_enabled", OnPluginEnabledChanged);
            int hook2 = CvarManager.HookCvarChange("sample_debug_mode", OnDebugModeChanged);
            int hook3 = CvarManager.HookCvarChange("mp_timelimit", OnTimeLimitChanged);

            if (hook1 != -1) _cvarHooks.Add(hook1);
            if (hook2 != -1) _cvarHooks.Add(hook2);
            if (hook3 != -1) _cvarHooks.Add(hook3);

            Console.WriteLine($"[CVar] Registered {_cvarHooks.Count} CVar hooks");
        }

        /// <summary>
        /// 插件启用状态变化回调 / Plugin enabled state change callback
        /// </summary>
        private static void OnPluginEnabledChanged(string cvarName, string oldValue, string newValue)
        {
            bool enabled = newValue == "1";
            Console.WriteLine($"[CVar] Plugin {(enabled ? "enabled" : "disabled")}");

            if (!enabled)
            {
                // 禁用插件功能 / Disable plugin features
                Console.WriteLine("[CVar] Disabling plugin features...");
            }
            else
            {
                // 启用插件功能 / Enable plugin features
                Console.WriteLine("[CVar] Enabling plugin features...");
            }
        }

        /// <summary>
        /// 调试模式变化回调 / Debug mode change callback
        /// </summary>
        private static void OnDebugModeChanged(string cvarName, string oldValue, string newValue)
        {
            bool debugMode = newValue == "1";
            Console.WriteLine($"[CVar] Debug mode {(debugMode ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// 时间限制变化回调 / Time limit change callback
        /// </summary>
        private static void OnTimeLimitChanged(string cvarName, string oldValue, string newValue)
        {
            if (int.TryParse(newValue, out int timeLimit))
            {
                Console.WriteLine($"[CVar] Time limit changed to {timeLimit} minutes");
                
                if (timeLimit > 60)
                {
                    Console.WriteLine("[CVar] Warning: Time limit is very high!");
                }
            }
        }

        /// <summary>
        /// 清理CVar系统 / Cleanup CVar system
        /// </summary>
        private static void CleanupCvars()
        {
            foreach (int hookId in _cvarHooks)
            {
                CvarManager.UnhookCvarChange(hookId);
            }
            _cvarHooks.Clear();
            Console.WriteLine("[CVar] Cleaned up CVar hooks");
        }

        #endregion

        #region Menu System Example

        /// <summary>
        /// 初始化菜单系统 / Initialize menu system
        /// </summary>
        private static void InitializeMenus()
        {
            Console.WriteLine("[Menu] Initializing menu system...");

            // 创建主菜单 / Create main menu
            _mainMenuId = MenuManager.CreateMenu("Sample Plugin Menu", OnMenuSelect, OnMenuCancel);
            
            if (_mainMenuId != -1)
            {
                // 添加菜单项 / Add menu items
                MenuManager.AddMenuItem(_mainMenuId, "Player Info", "player_info", 0);
                MenuManager.AddMenuItem(_mainMenuId, "Server Stats", "server_stats", 0);
                MenuManager.AddMenuBlank(_mainMenuId);
                MenuManager.AddMenuText(_mainMenuId, "--- Settings ---");
                MenuManager.AddMenuItem(_mainMenuId, "Toggle Debug", "toggle_debug", 0);
                MenuManager.AddMenuItem(_mainMenuId, "Reload Config", "reload_config", 0);
                MenuManager.AddMenuBlank(_mainMenuId);
                MenuManager.AddMenuItem(_mainMenuId, "Exit", "exit", 0);

                var menuInfo = MenuManager.GetMenuInfo(_mainMenuId);
                if (menuInfo.HasValue)
                {
                    Console.WriteLine($"[Menu] Created menu '{menuInfo.Value.Title}' with {menuInfo.Value.ItemCount} items");
                }
            }
        }

        /// <summary>
        /// 菜单选择回调 / Menu selection callback
        /// </summary>
        private static void OnMenuSelect(int clientId, int menuId, int item)
        {
            Console.WriteLine($"[Menu] Client {clientId} selected item {item}");

            switch (item)
            {
                case 0: // Player Info
                    ShowPlayerInfo(clientId);
                    break;
                case 1: // Server Stats
                    ShowServerStats(clientId);
                    break;
                case 4: // Toggle Debug
                    ToggleDebugMode();
                    break;
                case 5: // Reload Config
                    ReloadConfiguration();
                    break;
                case 7: // Exit
                    Console.WriteLine($"[Menu] Client {clientId} exited menu");
                    break;
            }
        }

        /// <summary>
        /// 菜单取消回调 / Menu cancel callback
        /// </summary>
        private static void OnMenuCancel(int clientId, int menuId, int reason)
        {
            Console.WriteLine($"[Menu] Client {clientId} cancelled menu, reason: {reason}");
        }

        /// <summary>
        /// 显示玩家信息 / Show player information
        /// </summary>
        private static void ShowPlayerInfo(int clientId)
        {
            Console.WriteLine($"[Menu] Showing player info for client {clientId}");
            // 这里可以添加显示玩家信息的逻辑
            // Here you can add logic to display player information
        }

        /// <summary>
        /// 显示服务器统计 / Show server statistics
        /// </summary>
        private static void ShowServerStats(int clientId)
        {
            Console.WriteLine($"[Menu] Showing server stats for client {clientId}");
            // 这里可以添加显示服务器统计的逻辑
            // Here you can add logic to display server statistics
        }

        /// <summary>
        /// 切换调试模式 / Toggle debug mode
        /// </summary>
        private static void ToggleDebugMode()
        {
            int currentValue = CvarManager.GetCvarInt("sample_debug_mode");
            int newValue = currentValue == 0 ? 1 : 0;
            CvarManager.SetCvarInt("sample_debug_mode", newValue);
            Console.WriteLine($"[Menu] Debug mode toggled to {(newValue == 1 ? "ON" : "OFF")}");
        }

        /// <summary>
        /// 重新加载配置 / Reload configuration
        /// </summary>
        private static void ReloadConfiguration()
        {
            Console.WriteLine("[Menu] Reloading configuration...");
            // 这里可以添加重新加载配置的逻辑
            // Here you can add logic to reload configuration
        }

        /// <summary>
        /// 清理菜单系统 / Cleanup menu system
        /// </summary>
        private static void CleanupMenus()
        {
            if (_mainMenuId != -1)
            {
                MenuManager.DestroyMenu(_mainMenuId);
                _mainMenuId = -1;
                Console.WriteLine("[Menu] Cleaned up menu system");
            }
        }

        #endregion

        #region Game Config Example

        /// <summary>
        /// 初始化游戏配置 / Initialize game configuration
        /// </summary>
        private static void InitializeGameConfig()
        {
            Console.WriteLine("[GameConfig] Initializing game configuration...");

            // 加载游戏配置文件 / Load game configuration file
            _gameConfigId = GameConfigManager.LoadGameConfig("sample.games.txt");
            
            if (_gameConfigId != -1)
            {
                Console.WriteLine("[GameConfig] Game configuration loaded successfully");

                // 获取一些配置值 / Get some configuration values
                var healthOffset = GameConfigManager.GetGameConfigOffset(_gameConfigId, "m_iHealth");
                if (healthOffset.HasValue)
                {
                    Console.WriteLine($"[GameConfig] Health offset: {healthOffset.Value}");
                }

                string engineName = GameConfigManager.GetGameConfigKeyValue(_gameConfigId, "engine");
                if (!string.IsNullOrEmpty(engineName))
                {
                    Console.WriteLine($"[GameConfig] Engine: {engineName}");
                }
            }
            else
            {
                Console.WriteLine("[GameConfig] Failed to load game configuration");
            }
        }

        /// <summary>
        /// 清理游戏配置 / Cleanup game configuration
        /// </summary>
        private static void CleanupGameConfig()
        {
            if (_gameConfigId != -1)
            {
                GameConfigManager.CloseGameConfig(_gameConfigId);
                _gameConfigId = -1;
                Console.WriteLine("[GameConfig] Cleaned up game configuration");
            }
        }

        #endregion

        #region Native Functions Example

        /// <summary>
        /// 初始化Native函数 / Initialize native functions
        /// </summary>
        private static void InitializeNatives()
        {
            Console.WriteLine("[Native] Initializing native functions...");

            // 注册自定义Native函数 / Register custom native functions
            bool success1 = NativeManager.RegisterNative("sample_get_plugin_info", GetPluginInfo);
            bool success2 = NativeManager.RegisterNative("sample_is_debug_enabled", IsDebugEnabled);
            bool success3 = NativeManager.RegisterNative("sample_calculate_distance", CalculateDistance);

            int registeredCount = 0;
            if (success1) registeredCount++;
            if (success2) registeredCount++;
            if (success3) registeredCount++;

            Console.WriteLine($"[Native] Registered {registeredCount} native functions");
        }

        /// <summary>
        /// 获取插件信息Native函数 / Get plugin info native function
        /// </summary>
        private static int GetPluginInfo(int paramCount)
        {
            Console.WriteLine($"[Native] GetPluginInfo called with {paramCount} parameters");

            if (paramCount >= 3)
            {
                // 设置插件名称 / Set plugin name
                NativeManager.SetNativeString(0, PLUGIN_NAME);
                // 设置版本 / Set version
                NativeManager.SetNativeString(1, PLUGIN_VERSION);
                // 设置作者 / Set author
                NativeManager.SetNativeString(2, PLUGIN_AUTHOR);
                return 1; // 成功 / Success
            }

            return 0; // 失败 / Failure
        }

        /// <summary>
        /// 检查调试模式是否启用Native函数 / Check if debug mode is enabled native function
        /// </summary>
        private static int IsDebugEnabled(int paramCount)
        {
            Console.WriteLine("[Native] IsDebugEnabled called");
            return CvarManager.GetCvarInt("sample_debug_mode");
        }

        /// <summary>
        /// 计算距离Native函数 / Calculate distance native function
        /// </summary>
        private static int CalculateDistance(int paramCount)
        {
            Console.WriteLine($"[Native] CalculateDistance called with {paramCount} parameters");

            if (paramCount >= 2)
            {
                // 获取两个3D点的坐标数组 / Get coordinate arrays for two 3D points
                int[] point1 = NativeManager.GetNativeArray(0, 3);
                int[] point2 = NativeManager.GetNativeArray(1, 3);

                if (point1.Length >= 3 && point2.Length >= 3)
                {
                    // 计算3D距离 / Calculate 3D distance
                    double dx = point2[0] - point1[0];
                    double dy = point2[1] - point1[1];
                    double dz = point2[2] - point1[2];
                    double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    return (int)Math.Round(distance);
                }
            }

            return 0;
        }

        #endregion

        #region Message System Example

        /// <summary>
        /// 初始化消息系统 / Initialize message system
        /// </summary>
        private static void InitializeMessages()
        {
            Console.WriteLine("[Message] Initializing message system...");

            // 注册消息钩子 / Register message hooks
            int hook1 = MessageManager.RegisterMessage(123, OnCustomMessage); // 假设消息ID 123
            int hook2 = MessageManager.RegisterMessage(124, OnAnotherMessage); // 假设消息ID 124

            if (hook1 != -1) _messageHooks.Add(hook1);
            if (hook2 != -1) _messageHooks.Add(hook2);

            Console.WriteLine($"[Message] Registered {_messageHooks.Count} message hooks");
        }

        /// <summary>
        /// 自定义消息回调 / Custom message callback
        /// </summary>
        private static void OnCustomMessage(int msgType, int msgDest, int entityId)
        {
            Console.WriteLine($"[Message] Custom message received - Type: {msgType}, Dest: {msgDest}, Entity: {entityId}");
        }

        /// <summary>
        /// 另一个消息回调 / Another message callback
        /// </summary>
        private static void OnAnotherMessage(int msgType, int msgDest, int entityId)
        {
            Console.WriteLine($"[Message] Another message received - Type: {msgType}, Dest: {msgDest}, Entity: {entityId}");
        }

        /// <summary>
        /// 发送自定义消息给玩家 / Send custom message to player
        /// </summary>
        public static void SendCustomMessageToPlayer(int clientId, string message)
        {
            Console.WriteLine($"[Message] Sending custom message to client {clientId}: {message}");

            // 开始消息 / Begin message
            if (MessageManager.BeginMessage(123, 1, clientId)) // 消息类型123，目标类型1
            {
                // 写入消息数据 / Write message data
                MessageManager.WriteByte(1); // 消息版本
                MessageManager.WriteString(message); // 消息内容
                MessageManager.WriteLong(Environment.TickCount); // 时间戳

                // 结束消息 / End message
                MessageManager.EndMessage();
                Console.WriteLine("[Message] Custom message sent successfully");
            }
            else
            {
                Console.WriteLine("[Message] Failed to send custom message");
            }
        }

        /// <summary>
        /// 清理消息系统 / Cleanup message system
        /// </summary>
        private static void CleanupMessages()
        {
            foreach (int hookId in _messageHooks)
            {
                MessageManager.UnregisterMessage(hookId);
            }
            _messageHooks.Clear();
            Console.WriteLine("[Message] Cleaned up message hooks");
        }

        #endregion

        #region DataPack Example

        /// <summary>
        /// 初始化数据包系统 / Initialize data pack system
        /// </summary>
        private static void InitializeDataPacks()
        {
            Console.WriteLine("[DataPack] Initializing data pack system...");

            // 创建数据包 / Create data pack
            _dataPackId = DataPackManager.CreateDataPack();

            if (_dataPackId != -1)
            {
                Console.WriteLine($"[DataPack] Created data pack with ID: {_dataPackId}");

                // 写入一些示例数据 / Write some sample data
                DataPackManager.WritePackCell(_dataPackId, 42);
                DataPackManager.WritePackFloat(_dataPackId, 3.14159f);
                DataPackManager.WritePackString(_dataPackId, "Hello DataPack!");
                DataPackManager.WritePackCell(_dataPackId, Environment.TickCount);

                Console.WriteLine("[DataPack] Sample data written to data pack");

                // 演示读取数据 / Demonstrate reading data
                DemonstrateDataPackReading();
            }
            else
            {
                Console.WriteLine("[DataPack] Failed to create data pack");
            }
        }

        /// <summary>
        /// 演示数据包读取 / Demonstrate data pack reading
        /// </summary>
        private static void DemonstrateDataPackReading()
        {
            if (_dataPackId == -1) return;

            Console.WriteLine("[DataPack] Demonstrating data pack reading...");

            // 重置位置到开始 / Reset position to beginning
            DataPackManager.ResetPack(_dataPackId);

            // 读取数据 / Read data
            int intValue = DataPackManager.ReadPackCell(_dataPackId);
            float floatValue = DataPackManager.ReadPackFloat(_dataPackId);
            string stringValue = DataPackManager.ReadPackString(_dataPackId);
            int timestampValue = DataPackManager.ReadPackCell(_dataPackId);

            Console.WriteLine($"[DataPack] Read integer: {intValue}");
            Console.WriteLine($"[DataPack] Read float: {floatValue}");
            Console.WriteLine($"[DataPack] Read string: {stringValue}");
            Console.WriteLine($"[DataPack] Read timestamp: {timestampValue}");

            // 检查是否读取完毕 / Check if reading is complete
            bool isEnded = DataPackManager.IsPackEnded(_dataPackId);
            Console.WriteLine($"[DataPack] Pack reading ended: {isEnded}");
        }

        /// <summary>
        /// 保存玩家数据到数据包 / Save player data to data pack
        /// </summary>
        public static void SavePlayerData(int clientId, int score, float health, string name)
        {
            if (_dataPackId == -1) return;

            Console.WriteLine($"[DataPack] Saving player data for client {clientId}");

            // 获取当前位置 / Get current position
            int currentPos = DataPackManager.GetPackPosition(_dataPackId);

            // 写入玩家数据 / Write player data
            DataPackManager.WritePackCell(_dataPackId, clientId);
            DataPackManager.WritePackCell(_dataPackId, score);
            DataPackManager.WritePackFloat(_dataPackId, health);
            DataPackManager.WritePackString(_dataPackId, name);
            DataPackManager.WritePackCell(_dataPackId, Environment.TickCount); // 保存时间

            Console.WriteLine($"[DataPack] Player data saved at position {currentPos}");
        }

        /// <summary>
        /// 从数据包加载玩家数据 / Load player data from data pack
        /// </summary>
        public static bool LoadPlayerData(out int clientId, out int score, out float health, out string name)
        {
            clientId = 0;
            score = 0;
            health = 0.0f;
            name = "";

            if (_dataPackId == -1) return false;

            try
            {
                // 读取玩家数据 / Read player data
                clientId = DataPackManager.ReadPackCell(_dataPackId);
                score = DataPackManager.ReadPackCell(_dataPackId);
                health = DataPackManager.ReadPackFloat(_dataPackId);
                name = DataPackManager.ReadPackString(_dataPackId);
                int timestamp = DataPackManager.ReadPackCell(_dataPackId);

                Console.WriteLine($"[DataPack] Loaded player data: ID={clientId}, Score={score}, Health={health}, Name={name}, Time={timestamp}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DataPack] Error loading player data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清理数据包系统 / Cleanup data pack system
        /// </summary>
        private static void CleanupDataPacks()
        {
            if (_dataPackId != -1)
            {
                DataPackManager.DestroyDataPack(_dataPackId);
                _dataPackId = -1;
                Console.WriteLine("[DataPack] Cleaned up data pack system");
            }
        }

        #endregion

        #region Public API Methods

        /// <summary>
        /// 显示主菜单给玩家 / Show main menu to player
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        public static void ShowMainMenu(int clientId)
        {
            if (_mainMenuId != -1)
            {
                bool success = MenuManager.DisplayMenu(_mainMenuId, clientId, 0);
                Console.WriteLine($"[API] Main menu display result for client {clientId}: {success}");
            }
            else
            {
                Console.WriteLine($"[API] Main menu not available for client {clientId}");
            }
        }

        /// <summary>
        /// 获取插件状态信息 / Get plugin status information
        /// </summary>
        /// <returns>状态信息字符串 / Status information string</returns>
        public static string GetPluginStatus()
        {
            bool enabled = CvarManager.GetCvarInt("sample_plugin_enabled") == 1;
            bool debugMode = CvarManager.GetCvarInt("sample_debug_mode") == 1;
            int maxPlayers = CvarManager.GetCvarInt("sample_max_players");

            return $"Plugin: {(enabled ? "Enabled" : "Disabled")}, " +
                   $"Debug: {(debugMode ? "On" : "Off")}, " +
                   $"Max Players: {maxPlayers}, " +
                   $"Menu ID: {_mainMenuId}, " +
                   $"Config ID: {_gameConfigId}, " +
                   $"DataPack ID: {_dataPackId}";
        }

        /// <summary>
        /// 执行插件命令 / Execute plugin command
        /// </summary>
        /// <param name="command">命令名称 / Command name</param>
        /// <param name="args">命令参数 / Command arguments</param>
        /// <returns>执行结果 / Execution result</returns>
        public static bool ExecuteCommand(string command, params string[] args)
        {
            Console.WriteLine($"[API] Executing command: {command} with {args.Length} arguments");

            switch (command.ToLower())
            {
                case "reload":
                    ReloadConfiguration();
                    return true;

                case "debug":
                    if (args.Length > 0 && bool.TryParse(args[0], out bool debugEnabled))
                    {
                        CvarManager.SetCvarInt("sample_debug_mode", debugEnabled ? 1 : 0);
                        return true;
                    }
                    break;

                case "status":
                    Console.WriteLine($"[API] Status: {GetPluginStatus()}");
                    return true;

                case "menu":
                    if (args.Length > 0 && int.TryParse(args[0], out int clientId))
                    {
                        ShowMainMenu(clientId);
                        return true;
                    }
                    break;

                case "message":
                    if (args.Length >= 2 && int.TryParse(args[0], out int targetClient))
                    {
                        SendCustomMessageToPlayer(targetClient, args[1]);
                        return true;
                    }
                    break;

                default:
                    Console.WriteLine($"[API] Unknown command: {command}");
                    break;
            }

            return false;
        }

        #endregion
    }
}
