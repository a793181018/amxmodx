using System;
using AmxxMenuBridge;

namespace AmxxMenuExample
{
    /// <summary>
    /// AMXX菜单系统使用示例
    /// AMXX Menu System Usage Example
    /// </summary>
    public class MenuExample
    {
        private static MenuManager? _mainMenu;
        private static MenuManager? _weaponMenu;
        private static MenuManager? _adminMenu;

        /// <summary>
        /// 初始化菜单系统
        /// Initialize menu system
        /// </summary>
        public static void InitializeMenus()
        {
            // 创建主菜单
            // Create main menu
            _mainMenu = MenuManager.CreateMenu("主菜单 / Main Menu", "MainMenuHandler", true);
            if (_mainMenu != null)
            {
                // 设置菜单属性
                // Set menu properties
                _mainMenu.SetTitle("=== 服务器主菜单 ===");
                _mainMenu.SetItemsPerPage(7);
                _mainMenu.SetShowPageNumbers(true);
                _mainMenu.SetNeverExit(false);

                // 添加菜单项
                // Add menu items
                _mainMenu.AddItem("武器菜单", "weapon_menu", 0);
                _mainMenu.AddItem("管理员菜单", "admin_menu", ADMIN_LEVEL_A);
                _mainMenu.AddItem("玩家信息", "player_info", 0);
                _mainMenu.AddBlank(true);
                _mainMenu.AddText("--- 其他选项 ---", false);
                _mainMenu.AddItem("服务器信息", "server_info", 0);
                _mainMenu.AddItem("退出", "exit", 0);

                // 注册菜单处理器
                // Register menu handler
                _mainMenu.RegisterHandler(HandleMainMenu);
            }

            // 创建武器菜单
            // Create weapon menu
            _weaponMenu = MenuManager.CreateMenu("武器菜单 / Weapon Menu", "WeaponMenuHandler", true);
            if (_weaponMenu != null)
            {
                _weaponMenu.SetTitle("=== 武器选择菜单 ===");
                
                // 添加武器选项（带回调）
                // Add weapon options with callbacks
                _weaponMenu.AddItem("AK-47", OnWeaponSelected, "weapon_ak47");
                _weaponMenu.AddItem("M4A1", OnWeaponSelected, "weapon_m4a1");
                _weaponMenu.AddItem("AWP", OnWeaponSelected, "weapon_awp");
                _weaponMenu.AddItem("Deagle", OnWeaponSelected, "weapon_deagle");
                _weaponMenu.AddBlank(true);
                _weaponMenu.AddItem("返回主菜单", "back_to_main", 0);

                _weaponMenu.RegisterHandler(HandleWeaponMenu);
            }

            // 创建管理员菜单
            // Create admin menu
            _adminMenu = MenuManager.CreateMenu("管理员菜单 / Admin Menu", "AdminMenuHandler", true);
            if (_adminMenu != null)
            {
                _adminMenu.SetTitle("=== 管理员控制面板 ===");
                _adminMenu.SetForceExit(false);
                
                _adminMenu.AddItem("踢出玩家", OnKickPlayer, "kick_player", ADMIN_KICK);
                _adminMenu.AddItem("封禁玩家", OnBanPlayer, "ban_player", ADMIN_BAN);
                _adminMenu.AddItem("更换地图", OnChangeMap, "change_map", ADMIN_MAP);
                _adminMenu.AddItem("重启服务器", OnRestartServer, "restart_server", ADMIN_RCON);
                _adminMenu.AddBlank(true);
                _adminMenu.AddItem("返回主菜单", "back_to_main", 0);

                _adminMenu.RegisterHandler(HandleAdminMenu);
            }
        }

        #region 菜单处理器 / Menu Handlers

        /// <summary>
        /// 主菜单处理器
        /// Main menu handler
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="item">选择的项目 / Selected item</param>
        /// <returns>处理结果 / Handle result</returns>
        private static int HandleMainMenu(int menuId, int playerId, int item)
        {
            Console.WriteLine($"玩家 {playerId} 在主菜单选择了项目 {item}");
            
            switch (item)
            {
                case 0: // 武器菜单
                    _weaponMenu?.Display(playerId);
                    break;
                case 1: // 管理员菜单
                    if (HasAdminAccess(playerId, ADMIN_LEVEL_A))
                    {
                        _adminMenu?.Display(playerId);
                    }
                    else
                    {
                        SendMessage(playerId, "您没有访问管理员菜单的权限！");
                    }
                    break;
                case 2: // 玩家信息
                    ShowPlayerInfo(playerId);
                    break;
                case 5: // 服务器信息
                    ShowServerInfo(playerId);
                    break;
                case 6: // 退出
                    MenuManager.Cancel(playerId);
                    break;
            }
            
            return 1; // 继续显示菜单
        }

        /// <summary>
        /// 武器菜单处理器
        /// Weapon menu handler
        /// </summary>
        private static int HandleWeaponMenu(int menuId, int playerId, int item)
        {
            Console.WriteLine($"玩家 {playerId} 在武器菜单选择了项目 {item}");
            
            if (item == 5) // 返回主菜单
            {
                _mainMenu?.Display(playerId);
            }
            
            return 1;
        }

        /// <summary>
        /// 管理员菜单处理器
        /// Admin menu handler
        /// </summary>
        private static int HandleAdminMenu(int menuId, int playerId, int item)
        {
            Console.WriteLine($"管理员 {playerId} 在管理员菜单选择了项目 {item}");
            
            if (item == 5) // 返回主菜单
            {
                _mainMenu?.Display(playerId);
            }
            
            return 1;
        }

        #endregion

        #region 菜单项回调 / Menu Item Callbacks

        /// <summary>
        /// 武器选择回调
        /// Weapon selection callback
        /// </summary>
        private static int OnWeaponSelected(int menuId, int playerId, int item)
        {
            var itemInfo = _weaponMenu?.GetItemInfo(item);
            if (itemInfo != null)
            {
                Console.WriteLine($"玩家 {playerId} 选择了武器: {itemInfo.Name}");
                GiveWeapon(playerId, itemInfo.Command);
                SendMessage(playerId, $"您获得了 {itemInfo.Name}！");
            }
            
            return 1;
        }

        /// <summary>
        /// 踢出玩家回调
        /// Kick player callback
        /// </summary>
        private static int OnKickPlayer(int menuId, int playerId, int item)
        {
            Console.WriteLine($"管理员 {playerId} 选择踢出玩家功能");
            ShowPlayerListMenu(playerId, PlayerAction.Kick);
            return 1;
        }

        /// <summary>
        /// 封禁玩家回调
        /// Ban player callback
        /// </summary>
        private static int OnBanPlayer(int menuId, int playerId, int item)
        {
            Console.WriteLine($"管理员 {playerId} 选择封禁玩家功能");
            ShowPlayerListMenu(playerId, PlayerAction.Ban);
            return 1;
        }

        /// <summary>
        /// 更换地图回调
        /// Change map callback
        /// </summary>
        private static int OnChangeMap(int menuId, int playerId, int item)
        {
            Console.WriteLine($"管理员 {playerId} 选择更换地图功能");
            ShowMapListMenu(playerId);
            return 1;
        }

        /// <summary>
        /// 重启服务器回调
        /// Restart server callback
        /// </summary>
        private static int OnRestartServer(int menuId, int playerId, int item)
        {
            Console.WriteLine($"管理员 {playerId} 选择重启服务器");
            SendMessage(playerId, "服务器将在30秒后重启！");
            // 这里可以调用服务器重启逻辑
            // Server restart logic can be called here
            return 1;
        }

        #endregion

        #region 辅助方法 / Helper Methods

        /// <summary>
        /// 显示菜单给玩家
        /// Display menu to player
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        public static void ShowMainMenu(int playerId)
        {
            if (_mainMenu != null)
            {
                _mainMenu.Display(playerId);
            }
            else
            {
                Console.WriteLine("主菜单未初始化！");
            }
        }

        /// <summary>
        /// 检查玩家是否有管理员权限
        /// Check if player has admin access
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="level">权限级别 / Access level</param>
        /// <returns>是否有权限 / Has access</returns>
        private static bool HasAdminAccess(int playerId, int level)
        {
            // 这里应该调用AMXX的权限检查函数
            // Should call AMXX permission check function here
            return true; // 示例中总是返回true
        }

        /// <summary>
        /// 发送消息给玩家
        /// Send message to player
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="message">消息内容 / Message content</param>
        private static void SendMessage(int playerId, string message)
        {
            Console.WriteLine($"发送消息给玩家 {playerId}: {message}");
            // 这里应该调用AMXX的消息发送函数
            // Should call AMXX message sending function here
        }

        /// <summary>
        /// 给玩家武器
        /// Give weapon to player
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="weapon">武器名称 / Weapon name</param>
        private static void GiveWeapon(int playerId, string weapon)
        {
            Console.WriteLine($"给玩家 {playerId} 武器: {weapon}");
            // 这里应该调用AMXX的给武器函数
            // Should call AMXX give weapon function here
        }

        /// <summary>
        /// 显示玩家信息
        /// Show player information
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        private static void ShowPlayerInfo(int playerId)
        {
            Console.WriteLine($"显示玩家 {playerId} 的信息");
            // 实现玩家信息显示逻辑
            // Implement player info display logic
        }

        /// <summary>
        /// 显示服务器信息
        /// Show server information
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        private static void ShowServerInfo(int playerId)
        {
            Console.WriteLine($"显示服务器信息给玩家 {playerId}");
            // 实现服务器信息显示逻辑
            // Implement server info display logic
        }

        /// <summary>
        /// 显示玩家列表菜单
        /// Show player list menu
        /// </summary>
        /// <param name="adminId">管理员ID / Admin ID</param>
        /// <param name="action">操作类型 / Action type</param>
        private static void ShowPlayerListMenu(int adminId, PlayerAction action)
        {
            // 创建动态玩家列表菜单
            // Create dynamic player list menu
            var playerMenu = MenuManager.CreateMenu($"玩家列表 - {action}", "PlayerListHandler");
            if (playerMenu != null)
            {
                // 添加在线玩家到菜单
                // Add online players to menu
                for (int i = 1; i <= 32; i++)
                {
                    if (IsPlayerConnected(i))
                    {
                        string playerName = GetPlayerName(i);
                        playerMenu.AddItem(playerName, (menuId, playerId, item) =>
                        {
                            ExecutePlayerAction(adminId, i, action);
                            return 1;
                        });
                    }
                }
                
                playerMenu.AddBlank(true);
                playerMenu.AddItem("返回", "back", 0);
                playerMenu.Display(adminId);
            }
        }

        /// <summary>
        /// 显示地图列表菜单
        /// Show map list menu
        /// </summary>
        /// <param name="adminId">管理员ID / Admin ID</param>
        private static void ShowMapListMenu(int adminId)
        {
            // 实现地图列表菜单
            // Implement map list menu
            Console.WriteLine($"显示地图列表给管理员 {adminId}");
        }

        /// <summary>
        /// 执行玩家操作
        /// Execute player action
        /// </summary>
        /// <param name="adminId">管理员ID / Admin ID</param>
        /// <param name="targetId">目标玩家ID / Target player ID</param>
        /// <param name="action">操作类型 / Action type</param>
        private static void ExecutePlayerAction(int adminId, int targetId, PlayerAction action)
        {
            switch (action)
            {
                case PlayerAction.Kick:
                    Console.WriteLine($"管理员 {adminId} 踢出了玩家 {targetId}");
                    break;
                case PlayerAction.Ban:
                    Console.WriteLine($"管理员 {adminId} 封禁了玩家 {targetId}");
                    break;
            }
        }

        /// <summary>
        /// 检查玩家是否连接
        /// Check if player is connected
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>是否连接 / Is connected</returns>
        private static bool IsPlayerConnected(int playerId)
        {
            // 这里应该调用AMXX的玩家连接检查函数
            // Should call AMXX player connection check function here
            return playerId <= 10; // 示例：假设前10个玩家在线
        }

        /// <summary>
        /// 获取玩家名称
        /// Get player name
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>玩家名称 / Player name</returns>
        private static string GetPlayerName(int playerId)
        {
            // 这里应该调用AMXX的获取玩家名称函数
            // Should call AMXX get player name function here
            return $"Player{playerId}";
        }

        #endregion

        #region 常量定义 / Constants Definition

        private const int ADMIN_LEVEL_A = 1;
        private const int ADMIN_KICK = 2;
        private const int ADMIN_BAN = 4;
        private const int ADMIN_MAP = 8;
        private const int ADMIN_RCON = 16;

        /// <summary>
        /// 玩家操作类型
        /// Player action type
        /// </summary>
        private enum PlayerAction
        {
            Kick,
            Ban
        }

        #endregion
    }

    /// <summary>
    /// 程序入口点
    /// Program entry point
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("初始化AMXX菜单系统...");
            MenuExample.InitializeMenus();
            
            Console.WriteLine("菜单系统初始化完成！");
            Console.WriteLine("使用示例：MenuExample.ShowMainMenu(playerId);");
            
            // 模拟显示菜单给玩家1
            // Simulate showing menu to player 1
            MenuExample.ShowMainMenu(1);
            
            Console.ReadKey();
        }
    }
}
