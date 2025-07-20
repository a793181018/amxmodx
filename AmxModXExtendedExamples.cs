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
}
