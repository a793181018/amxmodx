using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AmxxMenuBridge
{
    /// <summary>
    /// 菜单属性枚举
    /// Menu property enumeration
    /// </summary>
    public enum MenuProperty
    {
        /// <summary>返回按钮名称 / Back button name</summary>
        BackName = 0,
        /// <summary>下一页按钮名称 / Next page button name</summary>
        NextName,
        /// <summary>退出按钮名称 / Exit button name</summary>
        ExitName,
        /// <summary>菜单标题 / Menu title</summary>
        Title,
        /// <summary>菜单项颜色 / Menu item color</summary>
        ItemColor,
        /// <summary>永不退出 / Never exit</summary>
        NeverExit,
        /// <summary>强制退出 / Force exit</summary>
        ForceExit,
        /// <summary>每页项目数 / Items per page</summary>
        PerPage,
        /// <summary>显示页码 / Show page number</summary>
        ShowPage
    }

    /// <summary>
    /// 菜单项绘制类型
    /// Menu item draw type
    /// </summary>
    public enum ItemDrawType
    {
        /// <summary>默认 / Default</summary>
        Default = 0,
        /// <summary>禁用 / Disabled</summary>
        Disabled,
        /// <summary>原始行 / Raw line</summary>
        RawLine,
        /// <summary>无文本 / No text</summary>
        NoText,
        /// <summary>分隔符 / Spacer</summary>
        Spacer
    }

    /// <summary>
    /// 菜单回调类型
    /// Menu callback type
    /// </summary>
    public enum MenuCallbackType
    {
        /// <summary>显示回调 / Display callback</summary>
        Display = 0,
        /// <summary>选择回调 / Select callback</summary>
        Select
    }

    /// <summary>
    /// 菜单处理器委托
    /// Menu handler delegate
    /// </summary>
    /// <param name="menuId">菜单ID / Menu ID</param>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="item">选择的项目 / Selected item</param>
    /// <returns>处理结果 / Handle result</returns>
    public delegate int MenuHandlerDelegate(int menuId, int playerId, int item);

    /// <summary>
    /// 菜单项回调委托
    /// Menu item callback delegate
    /// </summary>
    /// <param name="menuId">菜单ID / Menu ID</param>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="item">选择的项目 / Selected item</param>
    /// <returns>回调结果 / Callback result</returns>
    public delegate int MenuItemDelegate(int menuId, int playerId, int item);

    /// <summary>
    /// AMXX菜单桥接DLL导入类
    /// AMXX Menu Bridge DLL Import Class
    /// </summary>
    public static class MenuBridgeImports
    {
        private const string DllName = "MenuBridge";

        #region 菜单管理接口 / Menu Management Interface

        /// <summary>
        /// 创建菜单
        /// Create a menu
        /// </summary>
        /// <param name="title">菜单标题 / Menu title</param>
        /// <param name="handler">处理器名称 / Handler name</param>
        /// <param name="useMultilingual">是否使用多语言 / Use multilingual</param>
        /// <returns>菜单ID，失败返回-1 / Menu ID, returns -1 on failure</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "CreateMenuEx")]
        public static extern int CreateMenu([MarshalAs(UnmanagedType.LPStr)] string title,
                                          [MarshalAs(UnmanagedType.LPStr)] string handler,
                                          [MarshalAs(UnmanagedType.Bool)] bool useMultilingual);

        /// <summary>
        /// 销毁菜单
        /// Destroy a menu
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "DestroyMenuEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyMenu(int menuId);

        /// <summary>
        /// 根据名称查找菜单ID
        /// Find menu ID by name
        /// </summary>
        /// <param name="menuName">菜单名称 / Menu name</param>
        /// <returns>菜单ID，未找到返回-1 / Menu ID, returns -1 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int FindMenuById([MarshalAs(UnmanagedType.LPStr)] string menuName);

        #endregion

        #region 菜单项管理接口 / Menu Item Management Interface

        /// <summary>
        /// 添加菜单项
        /// Add menu item
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "AddMenuItemEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddMenuItem(int menuId,
                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                            [MarshalAs(UnmanagedType.LPStr)] string command,
                                            int access);

        /// <summary>
        /// 添加空白行
        /// Add blank line
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="slot">是否占用槽位 / Use slot</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddMenuBlank(int menuId, [MarshalAs(UnmanagedType.Bool)] bool slot);

        /// <summary>
        /// 添加文本行
        /// Add text line
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="name">文本内容 / Text content</param>
        /// <param name="slot">是否占用槽位 / Use slot</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddMenuText(int menuId,
                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                            [MarshalAs(UnmanagedType.Bool)] bool slot);

        /// <summary>
        /// 插入菜单项
        /// Insert menu item
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="position">插入位置 / Insert position</param>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "InsertMenuItemEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InsertMenuItem(int menuId, int position,
                                               [MarshalAs(UnmanagedType.LPStr)] string name,
                                               [MarshalAs(UnmanagedType.LPStr)] string command,
                                               int access);

        #endregion

        #region 菜单项属性设置 / Menu Item Property Setting

        /// <summary>
        /// 设置菜单项名称
        /// Set menu item name
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="item">项目索引 / Item index</param>
        /// <param name="name">新名称 / New name</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemName(int menuId, int item,
                                                 [MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 设置菜单项命令
        /// Set menu item command
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="item">项目索引 / Item index</param>
        /// <param name="command">新命令 / New command</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemCommand(int menuId, int item,
                                                   [MarshalAs(UnmanagedType.LPStr)] string command);

        /// <summary>
        /// 设置菜单项访问权限
        /// Set menu item access level
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="item">项目索引 / Item index</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemAccess(int menuId, int item, int access);

        /// <summary>
        /// 设置菜单项回调
        /// Set menu item callback
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="item">项目索引 / Item index</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuItemCallback(int menuId, int item, MenuItemDelegate callback);

        #endregion

        #region 菜单项信息获取 / Menu Item Information Retrieval

        /// <summary>
        /// 获取菜单项信息
        /// Get menu item information
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="item">项目索引 / Item index</param>
        /// <param name="access">访问权限 / Access level</param>
        /// <param name="name">名称缓冲区 / Name buffer</param>
        /// <param name="nameLen">名称缓冲区长度 / Name buffer length</param>
        /// <param name="command">命令缓冲区 / Command buffer</param>
        /// <param name="commandLen">命令缓冲区长度 / Command buffer length</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, EntryPoint = "GetMenuItemInfoEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMenuItemInfo(int menuId, int item, int access,
                                                [MarshalAs(UnmanagedType.LPStr)] StringBuilder name, int nameLen,
                                                [MarshalAs(UnmanagedType.LPStr)] StringBuilder command, int commandLen);

        /// <summary>
        /// 获取菜单项数量
        /// Get menu item count
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>项目数量，失败返回-1 / Item count, returns -1 on failure</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "GetMenuItemCountEx")]
        public static extern int GetMenuItemCount(int menuId);

        /// <summary>
        /// 获取菜单页数
        /// Get menu page count
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>页数，失败返回-1 / Page count, returns -1 on failure</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetMenuPageCount(int menuId);

        #endregion

        #region 菜单显示和控制 / Menu Display and Control

        /// <summary>
        /// 显示菜单
        /// Display menu
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="page">页码 / Page number</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "DisplayMenuEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DisplayMenu(int menuId, int playerId, int page);

        /// <summary>
        /// 取消玩家菜单
        /// Cancel player menu
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "CancelMenuEx")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelMenu(int playerId);

        /// <summary>
        /// 检查玩家是否有活动菜单
        /// Check if player has active menu
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <returns>是否有活动菜单 / Has active menu</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsMenuActive(int playerId);

        #endregion

        #region 菜单属性设置 / Menu Property Setting

        /// <summary>
        /// 设置菜单属性
        /// Set menu property
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMenuProperty(int menuId, MenuProperty property,
                                                [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 获取菜单属性
        /// Get menu property
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="buffer">缓冲区 / Buffer</param>
        /// <param name="bufferSize">缓冲区大小 / Buffer size</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMenuProperty(int menuId, MenuProperty property,
                                                [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer, int bufferSize);

        #endregion

        #region 玩家菜单信息 / Player Menu Information

        /// <summary>
        /// 获取玩家菜单信息
        /// Get player menu information
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="oldMenu">旧菜单ID / Old menu ID</param>
        /// <param name="newMenu">新菜单ID / New menu ID</param>
        /// <param name="page">当前页码 / Current page</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPlayerMenuInfo(int playerId, out int oldMenu, out int newMenu, out int page);

        #endregion

        #region 回调注册 / Callback Registration

        /// <summary>
        /// 注册菜单处理器
        /// Register menu handler
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterMenuHandler(int menuId, MenuHandlerDelegate callback);

        /// <summary>
        /// 注销菜单处理器
        /// Unregister menu handler
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>是否成功 / Success</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterMenuHandler(int menuId);

        /// <summary>
        /// 创建菜单回调
        /// Create menu callback
        /// </summary>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>回调ID，失败返回-1 / Callback ID, returns -1 on failure</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int CreateMenuCallback(MenuItemDelegate callback);

        #endregion
    }
}
