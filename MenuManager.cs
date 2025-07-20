using System;
using System.Collections.Generic;
using System.Text;

namespace AmxxMenuBridge
{
    /// <summary>
    /// 菜单项信息类
    /// Menu item information class
    /// </summary>
    public class MenuItemInfo
    {
        /// <summary>项目名称 / Item name</summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>项目命令 / Item command</summary>
        public string Command { get; set; } = string.Empty;
        
        /// <summary>访问权限 / Access level</summary>
        public int Access { get; set; }
        
        /// <summary>回调函数 / Callback function</summary>
        public MenuItemDelegate? Callback { get; set; }
    }

    /// <summary>
    /// 菜单管理器类
    /// Menu manager class
    /// </summary>
    public class MenuManager
    {
        private static readonly Dictionary<int, MenuManager> _menus = new();
        private static readonly Dictionary<int, MenuHandlerDelegate> _handlers = new();
        
        /// <summary>菜单ID / Menu ID</summary>
        public int MenuId { get; private set; }
        
        /// <summary>菜单标题 / Menu title</summary>
        public string Title { get; private set; }
        
        /// <summary>是否使用多语言 / Use multilingual</summary>
        public bool UseMultilingual { get; private set; }

        private MenuManager(int menuId, string title, bool useMultilingual)
        {
            MenuId = menuId;
            Title = title;
            UseMultilingual = useMultilingual;
        }

        #region 静态工厂方法 / Static Factory Methods

        /// <summary>
        /// 创建菜单
        /// Create menu
        /// </summary>
        /// <param name="title">菜单标题 / Menu title</param>
        /// <param name="handler">处理器名称 / Handler name</param>
        /// <param name="useMultilingual">是否使用多语言 / Use multilingual</param>
        /// <returns>菜单管理器实例 / Menu manager instance</returns>
        public static MenuManager? CreateMenu(string title, string handler, bool useMultilingual = false)
        {
            int menuId = MenuBridgeImports.CreateMenu(title, handler, useMultilingual);
            if (menuId == -1)
                return null;

            var menu = new MenuManager(menuId, title, useMultilingual);
            _menus[menuId] = menu;
            return menu;
        }

        /// <summary>
        /// 根据ID获取菜单
        /// Get menu by ID
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>菜单管理器实例 / Menu manager instance</returns>
        public static MenuManager? GetMenu(int menuId)
        {
            return _menus.TryGetValue(menuId, out var menu) ? menu : null;
        }

        /// <summary>
        /// 根据名称查找菜单
        /// Find menu by name
        /// </summary>
        /// <param name="menuName">菜单名称 / Menu name</param>
        /// <returns>菜单管理器实例 / Menu manager instance</returns>
        public static MenuManager? FindMenu(string menuName)
        {
            int menuId = MenuBridgeImports.FindMenuById(menuName);
            return menuId != -1 ? GetMenu(menuId) : null;
        }

        #endregion

        #region 菜单项管理 / Menu Item Management

        /// <summary>
        /// 添加菜单项
        /// Add menu item
        /// </summary>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        public bool AddItem(string name, string command = "", int access = 0)
        {
            return MenuBridgeImports.AddMenuItem(MenuId, name, command, access);
        }

        /// <summary>
        /// 添加菜单项（带回调）
        /// Add menu item with callback
        /// </summary>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        public bool AddItem(string name, MenuItemDelegate callback, string command = "", int access = 0)
        {
            if (!AddItem(name, command, access))
                return false;

            int itemIndex = GetItemCount() - 1;
            return SetItemCallback(itemIndex, callback);
        }

        /// <summary>
        /// 添加空白行
        /// Add blank line
        /// </summary>
        /// <param name="useSlot">是否占用槽位 / Use slot</param>
        /// <returns>是否成功 / Success</returns>
        public bool AddBlank(bool useSlot = true)
        {
            return MenuBridgeImports.AddMenuBlank(MenuId, useSlot);
        }

        /// <summary>
        /// 添加文本行
        /// Add text line
        /// </summary>
        /// <param name="text">文本内容 / Text content</param>
        /// <param name="useSlot">是否占用槽位 / Use slot</param>
        /// <returns>是否成功 / Success</returns>
        public bool AddText(string text, bool useSlot = true)
        {
            return MenuBridgeImports.AddMenuText(MenuId, text, useSlot);
        }

        /// <summary>
        /// 插入菜单项
        /// Insert menu item
        /// </summary>
        /// <param name="position">插入位置 / Insert position</param>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        public bool InsertItem(int position, string name, string command = "", int access = 0)
        {
            return MenuBridgeImports.InsertMenuItem(MenuId, position, name, command, access);
        }

        #endregion

        #region 菜单项属性设置 / Menu Item Property Setting

        /// <summary>
        /// 设置菜单项名称
        /// Set menu item name
        /// </summary>
        /// <param name="itemIndex">项目索引 / Item index</param>
        /// <param name="name">新名称 / New name</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetItemName(int itemIndex, string name)
        {
            return MenuBridgeImports.SetMenuItemName(MenuId, itemIndex, name);
        }

        /// <summary>
        /// 设置菜单项命令
        /// Set menu item command
        /// </summary>
        /// <param name="itemIndex">项目索引 / Item index</param>
        /// <param name="command">新命令 / New command</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetItemCommand(int itemIndex, string command)
        {
            return MenuBridgeImports.SetMenuItemCommand(MenuId, itemIndex, command);
        }

        /// <summary>
        /// 设置菜单项访问权限
        /// Set menu item access level
        /// </summary>
        /// <param name="itemIndex">项目索引 / Item index</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetItemAccess(int itemIndex, int access)
        {
            return MenuBridgeImports.SetMenuItemAccess(MenuId, itemIndex, access);
        }

        /// <summary>
        /// 设置菜单项回调
        /// Set menu item callback
        /// </summary>
        /// <param name="itemIndex">项目索引 / Item index</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetItemCallback(int itemIndex, MenuItemDelegate callback)
        {
            return MenuBridgeImports.SetMenuItemCallback(MenuId, itemIndex, callback);
        }

        #endregion

        #region 菜单信息获取 / Menu Information Retrieval

        /// <summary>
        /// 获取菜单项信息
        /// Get menu item information
        /// </summary>
        /// <param name="itemIndex">项目索引 / Item index</param>
        /// <returns>菜单项信息 / Menu item information</returns>
        public MenuItemInfo? GetItemInfo(int itemIndex)
        {
            var name = new StringBuilder(256);
            var command = new StringBuilder(256);
            
            if (!MenuBridgeImports.GetMenuItemInfo(MenuId, itemIndex, 0, name, name.Capacity, command, command.Capacity))
                return null;

            return new MenuItemInfo
            {
                Name = name.ToString(),
                Command = command.ToString()
            };
        }

        /// <summary>
        /// 获取菜单项数量
        /// Get menu item count
        /// </summary>
        /// <returns>项目数量 / Item count</returns>
        public int GetItemCount()
        {
            return MenuBridgeImports.GetMenuItemCount(MenuId);
        }

        /// <summary>
        /// 获取菜单页数
        /// Get menu page count
        /// </summary>
        /// <returns>页数 / Page count</returns>
        public int GetPageCount()
        {
            return MenuBridgeImports.GetMenuPageCount(MenuId);
        }

        #endregion

        #region 菜单显示和控制 / Menu Display and Control

        /// <summary>
        /// 显示菜单给玩家
        /// Display menu to player
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="page">页码 / Page number</param>
        /// <returns>是否成功 / Success</returns>
        public bool Display(int playerId, int page = 0)
        {
            return MenuBridgeImports.DisplayMenu(MenuId, playerId, page);
        }

        /// <summary>
        /// 取消玩家菜单
        /// Cancel player menu
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>是否成功 / Success</returns>
        public static bool Cancel(int playerId)
        {
            return MenuBridgeImports.CancelMenu(playerId);
        }

        /// <summary>
        /// 检查玩家是否有活动菜单
        /// Check if player has active menu
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>是否有活动菜单 / Has active menu</returns>
        public static bool IsActive(int playerId)
        {
            return MenuBridgeImports.IsMenuActive(playerId);
        }

        #endregion

        #region 菜单属性管理 / Menu Property Management

        /// <summary>
        /// 设置菜单属性
        /// Set menu property
        /// </summary>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetProperty(MenuProperty property, string value)
        {
            return MenuBridgeImports.SetMenuProperty(MenuId, property, value);
        }

        /// <summary>
        /// 获取菜单属性
        /// Get menu property
        /// </summary>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值 / Property value</returns>
        public string GetProperty(MenuProperty property)
        {
            var buffer = new StringBuilder(256);
            return MenuBridgeImports.GetMenuProperty(MenuId, property, buffer, buffer.Capacity)
                ? buffer.ToString()
                : string.Empty;
        }

        /// <summary>
        /// 设置菜单标题
        /// Set menu title
        /// </summary>
        /// <param name="title">新标题 / New title</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetTitle(string title)
        {
            if (SetProperty(MenuProperty.Title, title))
            {
                Title = title;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置每页项目数
        /// Set items per page
        /// </summary>
        /// <param name="itemsPerPage">每页项目数 / Items per page</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetItemsPerPage(int itemsPerPage)
        {
            return SetProperty(MenuProperty.PerPage, itemsPerPage.ToString());
        }

        /// <summary>
        /// 设置是否显示页码
        /// Set show page numbers
        /// </summary>
        /// <param name="showPage">是否显示页码 / Show page numbers</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetShowPageNumbers(bool showPage)
        {
            return SetProperty(MenuProperty.ShowPage, showPage ? "1" : "0");
        }

        /// <summary>
        /// 设置永不退出
        /// Set never exit
        /// </summary>
        /// <param name="neverExit">永不退出 / Never exit</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetNeverExit(bool neverExit)
        {
            return SetProperty(MenuProperty.NeverExit, neverExit ? "1" : "0");
        }

        /// <summary>
        /// 设置强制退出
        /// Set force exit
        /// </summary>
        /// <param name="forceExit">强制退出 / Force exit</param>
        /// <returns>是否成功 / Success</returns>
        public bool SetForceExit(bool forceExit)
        {
            return SetProperty(MenuProperty.ForceExit, forceExit ? "1" : "0");
        }

        #endregion

        #region 回调管理 / Callback Management

        /// <summary>
        /// 注册菜单处理器
        /// Register menu handler
        /// </summary>
        /// <param name="handler">处理器委托 / Handler delegate</param>
        /// <returns>是否成功 / Success</returns>
        public bool RegisterHandler(MenuHandlerDelegate handler)
        {
            if (MenuBridgeImports.RegisterMenuHandler(MenuId, handler))
            {
                _handlers[MenuId] = handler;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 注销菜单处理器
        /// Unregister menu handler
        /// </summary>
        /// <returns>是否成功 / Success</returns>
        public bool UnregisterHandler()
        {
            if (MenuBridgeImports.UnregisterMenuHandler(MenuId))
            {
                _handlers.Remove(MenuId);
                return true;
            }
            return false;
        }

        #endregion

        #region 静态工具方法 / Static Utility Methods

        /// <summary>
        /// 获取玩家菜单信息
        /// Get player menu information
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="oldMenu">旧菜单ID / Old menu ID</param>
        /// <param name="newMenu">新菜单ID / New menu ID</param>
        /// <param name="page">当前页码 / Current page</param>
        /// <returns>是否成功 / Success</returns>
        public static bool GetPlayerMenuInfo(int playerId, out int oldMenu, out int newMenu, out int page)
        {
            return MenuBridgeImports.GetPlayerMenuInfo(playerId, out oldMenu, out newMenu, out page);
        }

        #endregion

        #region 资源清理 / Resource Cleanup

        /// <summary>
        /// 销毁菜单
        /// Destroy menu
        /// </summary>
        /// <returns>是否成功 / Success</returns>
        public bool Destroy()
        {
            if (MenuBridgeImports.DestroyMenu(MenuId))
            {
                _menus.Remove(MenuId);
                _handlers.Remove(MenuId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 析构函数
        /// Destructor
        /// </summary>
        ~MenuManager()
        {
            Destroy();
        }

        #endregion
    }
}
