// AmxModXExtendedInterop.cs - Extended C# Interop Layer for AMX Mod X
// Provides managed interface for CVar, Menu, GameConfig, Native, Message and DataPack systems

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AmxModX.Interop
{
    /// <summary>
    /// CVar变化回调委托 / CVar change callback delegate
    /// </summary>
    /// <param name="cvarName">CVar名称 / CVar name</param>
    /// <param name="oldValue">旧值 / Old value</param>
    /// <param name="newValue">新值 / New value</param>
    public delegate void CvarChangeCallback(string cvarName, string oldValue, string newValue);

    /// <summary>
    /// 菜单选择回调委托 / Menu selection callback delegate
    /// </summary>
    /// <param name="clientId">客户端ID / Client ID</param>
    /// <param name="menuId">菜单ID / Menu ID</param>
    /// <param name="item">选择的项目 / Selected item</param>
    public delegate void MenuSelectCallback(int clientId, int menuId, int item);

    /// <summary>
    /// 菜单取消回调委托 / Menu cancel callback delegate
    /// </summary>
    /// <param name="clientId">客户端ID / Client ID</param>
    /// <param name="menuId">菜单ID / Menu ID</param>
    /// <param name="reason">取消原因 / Cancel reason</param>
    public delegate void MenuCancelCallback(int clientId, int menuId, int reason);

    /// <summary>
    /// Native函数回调委托 / Native function callback delegate
    /// </summary>
    /// <param name="paramCount">参数数量 / Parameter count</param>
    /// <returns>返回值 / Return value</returns>
    public delegate int NativeCallback(int paramCount);

    /// <summary>
    /// 消息回调委托 / Message callback delegate
    /// </summary>
    /// <param name="msgType">消息类型 / Message type</param>
    /// <param name="msgDest">消息目标 / Message destination</param>
    /// <param name="entityId">实体ID / Entity ID</param>
    public delegate void MessageCallback(int msgType, int msgDest, int entityId);

    /// <summary>
    /// 菜单信息结构 / Menu information structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MenuInfo
    {
        /// <summary>菜单标题 / Menu title</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Title;

        /// <summary>菜单ID / Menu ID</summary>
        public int MenuId;

        /// <summary>项目数量 / Item count</summary>
        public int ItemCount;

        /// <summary>页面数量 / Page count</summary>
        public int PageCount;

        /// <summary>是否激活 / Is active</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsActive;

        /// <summary>永不退出 / Never exit</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool NeverExit;

        /// <summary>强制退出 / Force exit</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool ForceExit;
    }

    /// <summary>
    /// 游戏配置信息结构 / Game config information structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct GameConfigInfo
    {
        /// <summary>文件名 / File name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string FileName;

        /// <summary>配置ID / Config ID</summary>
        public int ConfigId;

        /// <summary>是否有效 / Is valid</summary>
        [MarshalAs(UnmanagedType.I1)]
        public bool IsValid;
    }

    /// <summary>
    /// 扩展原生函数导入 / Extended native function imports
    /// </summary>
    internal static class ExtendedNativeMethods
    {
        private const string DllName = "AmxModXBridge";

        #region CVar System Functions

        /// <summary>
        /// 创建CVar / Create CVar
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int CreateCvar(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string value,
            int flags,
            [MarshalAs(UnmanagedType.LPStr)] string description);

        /// <summary>
        /// 检查CVar是否存在 / Check if CVar exists
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CvarExists([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 获取CVar字符串值 / Get CVar string value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetCvarString(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            int bufferSize);

        /// <summary>
        /// 设置CVar字符串值 / Set CVar string value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetCvarString(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 获取CVar整数值 / Get CVar integer value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int GetCvarInt([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 设置CVar整数值 / Set CVar integer value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetCvarInt(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            int value);

        /// <summary>
        /// 获取CVar浮点值 / Get CVar float value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern float GetCvarFloat([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 设置CVar浮点值 / Set CVar float value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetCvarFloat(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            float value);

        /// <summary>
        /// 获取CVar标志 / Get CVar flags
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int GetCvarFlags([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 设置CVar标志 / Set CVar flags
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetCvarFlags(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            int flags);

        /// <summary>
        /// 钩子CVar变化 / Hook CVar change
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int HookCvarChange(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            CvarChangeCallback callback);

        /// <summary>
        /// 取消钩子CVar变化 / Unhook CVar change
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnhookCvarChange(int hookId);

        #endregion

        #region Menu System Functions

        /// <summary>
        /// 创建菜单 / Create menu
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int CreateMenu(
            [MarshalAs(UnmanagedType.LPStr)] string title,
            MenuSelectCallback selectCallback,
            MenuCancelCallback cancelCallback);

        /// <summary>
        /// 添加菜单项 / Add menu item
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AddMenuItem(
            int menuId,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string command,
            int access);

        /// <summary>
        /// 添加空行 / Add blank line
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AddMenuBlank(int menuId, int slot);

        /// <summary>
        /// 添加文本 / Add text
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AddMenuText(
            int menuId,
            [MarshalAs(UnmanagedType.LPStr)] string text,
            int slot);

        /// <summary>
        /// 显示菜单 / Display menu
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool DisplayMenu(int menuId, int clientId, int page);

        /// <summary>
        /// 销毁菜单 / Destroy menu
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool DestroyMenu(int menuId);

        /// <summary>
        /// 获取菜单信息 / Get menu information
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetMenuInfo(int menuId, out MenuInfo outInfo);

        /// <summary>
        /// 获取菜单页数 / Get menu pages
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetMenuPages(int menuId);

        /// <summary>
        /// 获取菜单项数 / Get menu items
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetMenuItems(int menuId);

        #endregion

        #region Game Config Functions

        /// <summary>
        /// 加载游戏配置 / Load game config
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int LoadGameConfig([MarshalAs(UnmanagedType.LPStr)] string fileName);

        /// <summary>
        /// 获取配置偏移量 / Get config offset
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetGameConfigOffset(
            int configId,
            [MarshalAs(UnmanagedType.LPStr)] string key,
            out int offset);

        /// <summary>
        /// 获取配置地址 / Get config address
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetGameConfigAddress(
            int configId,
            [MarshalAs(UnmanagedType.LPStr)] string key,
            out IntPtr address);

        /// <summary>
        /// 获取配置键值 / Get config key value
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetGameConfigKeyValue(
            int configId,
            [MarshalAs(UnmanagedType.LPStr)] string key,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            int bufferSize);

        /// <summary>
        /// 关闭游戏配置 / Close game config
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CloseGameConfig(int configId);

        #endregion

        #region Native Management Functions

        /// <summary>
        /// 注册Native函数 / Register native function
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RegisterNative(
            [MarshalAs(UnmanagedType.LPStr)] string name,
            NativeCallback callback);

        /// <summary>
        /// 取消注册Native函数 / Unregister native function
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnregisterNative([MarshalAs(UnmanagedType.LPStr)] string name);

        /// <summary>
        /// 获取Native参数 / Get native parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetNativeParam(int index);

        /// <summary>
        /// 获取Native字符串参数 / Get native string parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetNativeString(
            int index,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            int bufferSize);

        /// <summary>
        /// 设置Native字符串参数 / Set native string parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetNativeString(
            int index,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 获取Native数组参数 / Get native array parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetNativeArray(int index, int[] buffer, int size);

        /// <summary>
        /// 设置Native数组参数 / Set native array parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetNativeArray(int index, int[] buffer, int size);

        #endregion

        #region Message System Functions

        /// <summary>
        /// 开始消息 / Begin message
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool BeginMessage(int msgType, int msgDest, int entityId);

        /// <summary>
        /// 结束消息 / End message
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool EndMessage();

        /// <summary>
        /// 写入字节 / Write byte
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteByte(int value);

        /// <summary>
        /// 写入字符 / Write char
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteChar(int value);

        /// <summary>
        /// 写入短整数 / Write short
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteShort(int value);

        /// <summary>
        /// 写入长整数 / Write long
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteLong(int value);

        /// <summary>
        /// 写入角度 / Write angle
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteAngle(float value);

        /// <summary>
        /// 写入坐标 / Write coordinate
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteCoord(float value);

        /// <summary>
        /// 写入字符串 / Write string
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteString([MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 写入实体 / Write entity
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WriteEntity(int entityId);

        /// <summary>
        /// 注册消息钩子 / Register message hook
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int RegisterMessage(int msgId, MessageCallback callback);

        /// <summary>
        /// 取消注册消息钩子 / Unregister message hook
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnregisterMessage(int hookId);

        #endregion

        #region DataPack Functions

        /// <summary>
        /// 创建数据包 / Create data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CreateDataPack();

        /// <summary>
        /// 写入整数到数据包 / Write cell to data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WritePackCell(int packId, int value);

        /// <summary>
        /// 写入浮点数到数据包 / Write float to data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WritePackFloat(int packId, float value);

        /// <summary>
        /// 写入字符串到数据包 / Write string to data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool WritePackString(
            int packId,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 从数据包读取整数 / Read cell from data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ReadPackCell(int packId);

        /// <summary>
        /// 从数据包读取浮点数 / Read float from data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern float ReadPackFloat(int packId);

        /// <summary>
        /// 从数据包读取字符串 / Read string from data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ReadPackString(
            int packId,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            int bufferSize);

        /// <summary>
        /// 重置数据包 / Reset data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ResetPack(int packId);

        /// <summary>
        /// 获取数据包位置 / Get pack position
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetPackPosition(int packId);

        /// <summary>
        /// 设置数据包位置 / Set pack position
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetPackPosition(int packId, int position);

        /// <summary>
        /// 检查数据包是否结束 / Check if pack is ended
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsPackEnded(int packId);

        /// <summary>
        /// 销毁数据包 / Destroy data pack
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool DestroyDataPack(int packId);

        #endregion

        #region Core AMX Functions

        /// <summary>
        /// 日志回调委托 / Log callback delegate
        /// </summary>
        /// <param name="logLevel">日志级别 / Log level</param>
        /// <param name="message">日志消息 / Log message</param>
        public delegate void LogCallback(int logLevel, string message);

        /// <summary>
        /// 函数调用回调委托 / Function call callback delegate
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <param name="funcId">函数ID / Function ID</param>
        /// <param name="paramCount">参数数量 / Parameter count</param>
        /// <returns>返回值 / Return value</returns>
        public delegate int CallFuncCallback(int pluginId, int funcId, int paramCount);

        /// <summary>
        /// 插件信息结构 / Plugin information structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PluginInfo
        {
            /// <summary>文件名 / File name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string FileName;

            /// <summary>插件名称 / Plugin name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Name;

            /// <summary>版本 / Version</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Version;

            /// <summary>作者 / Author</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Author;

            /// <summary>状态 / Status</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Status;

            /// <summary>URL</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Url;

            /// <summary>描述 / Description</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
            public string Description;

            /// <summary>插件ID / Plugin ID</summary>
            public int PluginId;

            /// <summary>状态代码 / Status code</summary>
            public int StatusCode;

            /// <summary>是否有效 / Is valid</summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsValid;

            /// <summary>是否运行中 / Is running</summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsRunning;

            /// <summary>是否暂停 / Is paused</summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsPaused;
        }

        /// <summary>
        /// Forward信息结构 / Forward information structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ForwardInfo
        {
            /// <summary>Forward名称 / Forward name</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Name;

            /// <summary>Forward ID</summary>
            public int ForwardId;

            /// <summary>参数数量 / Parameter count</summary>
            public int ParamCount;

            /// <summary>执行类型 / Execution type</summary>
            public int ExecType;

            /// <summary>是否有效 / Is valid</summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool IsValid;
        }

        // Plugin management functions

        /// <summary>
        /// 获取插件数量 / Get plugins number
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetPluginsNum();

        /// <summary>
        /// 获取插件信息 / Get plugin information
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetPluginInfo(int pluginId, out PluginInfo outInfo);

        /// <summary>
        /// 查找插件 / Find plugin
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindPlugin([MarshalAs(UnmanagedType.LPStr)] string fileName);

        /// <summary>
        /// 检查插件是否有效 / Check if plugin is valid
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsPluginValid(int pluginId);

        /// <summary>
        /// 检查插件是否运行中 / Check if plugin is running
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsPluginRunning(int pluginId);

        /// <summary>
        /// 暂停插件 / Pause plugin
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool PausePlugin(int pluginId);

        /// <summary>
        /// 恢复插件 / Unpause plugin
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnpausePlugin(int pluginId);

        // Function call system

        /// <summary>
        /// 开始函数调用 / Begin function call
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncBegin(
            [MarshalAs(UnmanagedType.LPStr)] string funcName,
            [MarshalAs(UnmanagedType.LPStr)] string pluginName);

        /// <summary>
        /// 通过ID开始函数调用 / Begin function call by ID
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncBeginById(int funcId, int pluginId);

        /// <summary>
        /// 压入整数参数 / Push integer parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncPushInt(int value);

        /// <summary>
        /// 压入浮点参数 / Push float parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncPushFloat(float value);

        /// <summary>
        /// 压入字符串参数 / Push string parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncPushString([MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 压入数组参数 / Push array parameter
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool CallFuncPushArray(int[] array, int size);

        /// <summary>
        /// 结束函数调用 / End function call
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CallFuncEnd();

        /// <summary>
        /// 获取函数ID / Get function ID
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int GetFuncId(
            [MarshalAs(UnmanagedType.LPStr)] string funcName,
            int pluginId);

        // Forward system functions

        /// <summary>
        /// 创建Forward / Create forward
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int CreateForward(
            [MarshalAs(UnmanagedType.LPStr)] string funcName,
            int execType,
            int[] paramTypes,
            int paramCount);

        /// <summary>
        /// 创建单插件Forward / Create single plugin forward
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int CreateSPForward(
            [MarshalAs(UnmanagedType.LPStr)] string funcName,
            int pluginId,
            int[] paramTypes,
            int paramCount);

        /// <summary>
        /// 销毁Forward / Destroy forward
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool DestroyForward(int forwardId);

        /// <summary>
        /// 执行Forward / Execute forward
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ExecuteForward(int forwardId, int[] parameters, int paramCount);

        /// <summary>
        /// 获取Forward信息 / Get forward information
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetForwardInfo(int forwardId, out ForwardInfo outInfo);

        // Server functions

        /// <summary>
        /// 服务器打印 / Server print
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ServerPrint([MarshalAs(UnmanagedType.LPStr)] string message);

        /// <summary>
        /// 服务器命令 / Server command
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ServerCmd([MarshalAs(UnmanagedType.LPStr)] string command);

        /// <summary>
        /// 执行服务器命令 / Execute server command
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ServerExec();

        /// <summary>
        /// 是否为专用服务器 / Is dedicated server
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsDedicatedServer();

        /// <summary>
        /// 是否为Linux服务器 / Is Linux server
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsLinuxServer();

        /// <summary>
        /// 检查地图是否有效 / Check if map is valid
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsMapValid([MarshalAs(UnmanagedType.LPStr)] string mapName);

        // Client functions

        /// <summary>
        /// 获取玩家数量 / Get players number
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetPlayersNum([MarshalAs(UnmanagedType.I1)] bool includeConnecting);

        /// <summary>
        /// 检查用户是否为机器人 / Check if user is bot
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsUserBot(int clientId);

        /// <summary>
        /// 检查用户是否已连接 / Check if user is connected
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsUserConnected(int clientId);

        /// <summary>
        /// 检查用户是否存活 / Check if user is alive
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsUserAlive(int clientId);

        /// <summary>
        /// 获取用户时间 / Get user time
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetUserTime(int clientId, [MarshalAs(UnmanagedType.I1)] bool playtime);

        /// <summary>
        /// 客户端命令 / Client command
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool ClientCmd(int clientId, [MarshalAs(UnmanagedType.LPStr)] string command);

        /// <summary>
        /// 虚假客户端命令 / Fake client command
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool FakeClientCmd(int clientId, [MarshalAs(UnmanagedType.LPStr)] string command);

        // Admin management functions

        /// <summary>
        /// 添加管理员 / Add admin
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AdminsPush(
            [MarshalAs(UnmanagedType.LPStr)] string authData,
            [MarshalAs(UnmanagedType.LPStr)] string password,
            int access,
            int flags);

        /// <summary>
        /// 清空管理员列表 / Flush admins list
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AdminsFlush();

        /// <summary>
        /// 获取管理员数量 / Get admins number
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int AdminsNum();

        /// <summary>
        /// 查找管理员信息 / Lookup admin information
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AdminsLookup(
            int index,
            int property,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer,
            int bufferSize,
            out int outValue);

        // Logging functions

        /// <summary>
        /// 记录到文件 / Log to file
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LogToFile(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.LPStr)] string message);

        /// <summary>
        /// AMX日志 / AMX log
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LogAmx([MarshalAs(UnmanagedType.LPStr)] string message);

        /// <summary>
        /// 记录错误 / Log error
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LogError(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string message);

        /// <summary>
        /// 注册日志回调 / Register log callback
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int RegisterLogCallback(LogCallback callback);

        /// <summary>
        /// 取消注册日志回调 / Unregister log callback
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool UnregisterLogCallback(int callbackId);

        // Library management functions

        /// <summary>
        /// 注册库 / Register library
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RegisterLibrary([MarshalAs(UnmanagedType.LPStr)] string libraryName);

        /// <summary>
        /// 检查库是否存在 / Check if library exists
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool LibraryExists([MarshalAs(UnmanagedType.LPStr)] string libraryName);

        // Utility functions

        /// <summary>
        /// 中止执行 / Abort execution
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool AbortExecution(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string message);

        /// <summary>
        /// 获取堆空间 / Get heap space
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetHeapSpace();

        /// <summary>
        /// 获取参数数量 / Get number of arguments
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetNumArgs();

        /// <summary>
        /// 交换字符 / Swap characters
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SwapChars(
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder stringValue,
            int char1,
            int char2);

        /// <summary>
        /// 随机整数 / Random integer
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int RandomInt(int max);

        /// <summary>
        /// 最小整数 / Minimum integer
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int MinInt(int a, int b);

        /// <summary>
        /// 最大整数 / Maximum integer
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int MaxInt(int a, int b);

        /// <summary>
        /// 限制整数范围 / Clamp integer
        /// </summary>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int ClampInt(int value, int min, int max);

        #endregion
    }
}
