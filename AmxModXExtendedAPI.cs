// AmxModXExtendedAPI.cs - High-level C# API for AMX Mod X Extended Systems
// Provides managed wrapper classes for CVar, Menu, GameConfig, Native, Message and DataPack systems

using System;
using System.Collections.Generic;
using System.Text;
using AmxModX.Interop;

namespace AmxModX
{
    /// <summary>
    /// CVar系统管理器 / CVar system manager
    /// </summary>
    public static class CvarManager
    {
        private static readonly Dictionary<int, CvarChangeCallback> _cvarHooks = new Dictionary<int, CvarChangeCallback>();

        /// <summary>
        /// 创建CVar / Create CVar
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="value">默认值 / Default value</param>
        /// <param name="flags">标志位 / Flags</param>
        /// <param name="description">描述 / Description</param>
        /// <returns>CVar ID，失败返回-1 / CVar ID, returns -1 on failure</returns>
        public static int CreateCvar(string name, string value, int flags = 0, string description = "")
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("CVar name cannot be null or empty.", nameof(name));

            return ExtendedNativeMethods.CreateCvar(name, value ?? "", flags, description ?? "");
        }

        /// <summary>
        /// 检查CVar是否存在 / Check if CVar exists
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <returns>是否存在 / Whether exists</returns>
        public static bool CvarExists(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.CvarExists(name);
        }

        /// <summary>
        /// 获取CVar字符串值 / Get CVar string value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <returns>CVar值 / CVar value</returns>
        public static string GetCvarString(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            var buffer = new StringBuilder(256);
            if (ExtendedNativeMethods.GetCvarString(name, buffer, buffer.Capacity))
                return buffer.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 设置CVar字符串值 / Set CVar string value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="value">新值 / New value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetCvarString(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.SetCvarString(name, value ?? "");
        }

        /// <summary>
        /// 获取CVar整数值 / Get CVar integer value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <returns>CVar值 / CVar value</returns>
        public static int GetCvarInt(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0;

            return ExtendedNativeMethods.GetCvarInt(name);
        }

        /// <summary>
        /// 设置CVar整数值 / Set CVar integer value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="value">新值 / New value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetCvarInt(string name, int value)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.SetCvarInt(name, value);
        }

        /// <summary>
        /// 获取CVar浮点值 / Get CVar float value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <returns>CVar值 / CVar value</returns>
        public static float GetCvarFloat(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0.0f;

            return ExtendedNativeMethods.GetCvarFloat(name);
        }

        /// <summary>
        /// 设置CVar浮点值 / Set CVar float value
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="value">新值 / New value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetCvarFloat(string name, float value)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.SetCvarFloat(name, value);
        }

        /// <summary>
        /// 获取CVar标志 / Get CVar flags
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <returns>标志位 / Flags</returns>
        public static int GetCvarFlags(string name)
        {
            if (string.IsNullOrEmpty(name))
                return 0;

            return ExtendedNativeMethods.GetCvarFlags(name);
        }

        /// <summary>
        /// 设置CVar标志 / Set CVar flags
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="flags">标志位 / Flags</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetCvarFlags(string name, int flags)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.SetCvarFlags(name, flags);
        }

        /// <summary>
        /// 钩子CVar变化 / Hook CVar change
        /// </summary>
        /// <param name="name">CVar名称 / CVar name</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>钩子ID，失败返回-1 / Hook ID, returns -1 on failure</returns>
        public static int HookCvarChange(string name, CvarChangeCallback callback)
        {
            if (string.IsNullOrEmpty(name) || callback == null)
                return -1;

            int hookId = ExtendedNativeMethods.HookCvarChange(name, callback);
            if (hookId != -1)
            {
                _cvarHooks[hookId] = callback;
            }

            return hookId;
        }

        /// <summary>
        /// 取消钩子CVar变化 / Unhook CVar change
        /// </summary>
        /// <param name="hookId">钩子ID / Hook ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool UnhookCvarChange(int hookId)
        {
            if (hookId < 0)
                return false;

            bool result = ExtendedNativeMethods.UnhookCvarChange(hookId);
            if (result)
            {
                _cvarHooks.Remove(hookId);
            }

            return result;
        }
    }

    /// <summary>
    /// 菜单系统管理器 / Menu system manager
    /// </summary>
    public static class MenuManager
    {
        private static readonly Dictionary<int, MenuSelectCallback> _selectCallbacks = new Dictionary<int, MenuSelectCallback>();
        private static readonly Dictionary<int, MenuCancelCallback> _cancelCallbacks = new Dictionary<int, MenuCancelCallback>();

        /// <summary>
        /// 创建菜单 / Create menu
        /// </summary>
        /// <param name="title">菜单标题 / Menu title</param>
        /// <param name="selectCallback">选择回调 / Select callback</param>
        /// <param name="cancelCallback">取消回调 / Cancel callback</param>
        /// <returns>菜单ID，失败返回-1 / Menu ID, returns -1 on failure</returns>
        public static int CreateMenu(string title, MenuSelectCallback selectCallback, MenuCancelCallback cancelCallback = null)
        {
            if (string.IsNullOrEmpty(title) || selectCallback == null)
                return -1;

            int menuId = ExtendedNativeMethods.CreateMenu(title, selectCallback, cancelCallback);
            if (menuId != -1)
            {
                _selectCallbacks[menuId] = selectCallback;
                if (cancelCallback != null)
                    _cancelCallbacks[menuId] = cancelCallback;
            }

            return menuId;
        }

        /// <summary>
        /// 添加菜单项 / Add menu item
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="name">项目名称 / Item name</param>
        /// <param name="command">命令 / Command</param>
        /// <param name="access">访问权限 / Access permission</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AddMenuItem(int menuId, string name, string command = "", int access = 0)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return ExtendedNativeMethods.AddMenuItem(menuId, name, command ?? "", access);
        }

        /// <summary>
        /// 添加空行 / Add blank line
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="slot">位置，-1为自动 / Slot, -1 for auto</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AddMenuBlank(int menuId, int slot = -1)
        {
            return ExtendedNativeMethods.AddMenuBlank(menuId, slot);
        }

        /// <summary>
        /// 添加文本 / Add text
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="text">文本内容 / Text content</param>
        /// <param name="slot">位置，-1为自动 / Slot, -1 for auto</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AddMenuText(int menuId, string text, int slot = -1)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return ExtendedNativeMethods.AddMenuText(menuId, text, slot);
        }

        /// <summary>
        /// 显示菜单 / Display menu
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <param name="page">页面，0为第一页 / Page, 0 for first page</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool DisplayMenu(int menuId, int clientId, int page = 0)
        {
            return ExtendedNativeMethods.DisplayMenu(menuId, clientId, page);
        }

        /// <summary>
        /// 销毁菜单 / Destroy menu
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool DestroyMenu(int menuId)
        {
            bool result = ExtendedNativeMethods.DestroyMenu(menuId);
            if (result)
            {
                _selectCallbacks.Remove(menuId);
                _cancelCallbacks.Remove(menuId);
            }

            return result;
        }

        /// <summary>
        /// 获取菜单信息 / Get menu information
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>菜单信息，失败返回null / Menu info, returns null on failure</returns>
        public static MenuInfo? GetMenuInfo(int menuId)
        {
            if (ExtendedNativeMethods.GetMenuInfo(menuId, out MenuInfo info))
                return info;

            return null;
        }

        /// <summary>
        /// 获取菜单页数 / Get menu pages
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>页数 / Page count</returns>
        public static int GetMenuPages(int menuId)
        {
            return ExtendedNativeMethods.GetMenuPages(menuId);
        }

        /// <summary>
        /// 获取菜单项数 / Get menu items
        /// </summary>
        /// <param name="menuId">菜单ID / Menu ID</param>
        /// <returns>项目数 / Item count</returns>
        public static int GetMenuItems(int menuId)
        {
            return ExtendedNativeMethods.GetMenuItems(menuId);
        }
    }

    /// <summary>
    /// 游戏配置管理器 / Game config manager
    /// </summary>
    public static class GameConfigManager
    {
        /// <summary>
        /// 加载游戏配置 / Load game config
        /// </summary>
        /// <param name="fileName">配置文件名 / Config file name</param>
        /// <returns>配置ID，失败返回-1 / Config ID, returns -1 on failure</returns>
        public static int LoadGameConfig(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return -1;

            return ExtendedNativeMethods.LoadGameConfig(fileName);
        }

        /// <summary>
        /// 获取配置偏移量 / Get config offset
        /// </summary>
        /// <param name="configId">配置ID / Config ID</param>
        /// <param name="key">键名 / Key name</param>
        /// <returns>偏移量，失败返回null / Offset, returns null on failure</returns>
        public static int? GetGameConfigOffset(int configId, string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            if (ExtendedNativeMethods.GetGameConfigOffset(configId, key, out int offset))
                return offset;

            return null;
        }

        /// <summary>
        /// 获取配置地址 / Get config address
        /// </summary>
        /// <param name="configId">配置ID / Config ID</param>
        /// <param name="key">键名 / Key name</param>
        /// <returns>地址，失败返回IntPtr.Zero / Address, returns IntPtr.Zero on failure</returns>
        public static IntPtr GetGameConfigAddress(int configId, string key)
        {
            if (string.IsNullOrEmpty(key))
                return IntPtr.Zero;

            if (ExtendedNativeMethods.GetGameConfigAddress(configId, key, out IntPtr address))
                return address;

            return IntPtr.Zero;
        }

        /// <summary>
        /// 获取配置键值 / Get config key value
        /// </summary>
        /// <param name="configId">配置ID / Config ID</param>
        /// <param name="key">键名 / Key name</param>
        /// <returns>键值，失败返回空字符串 / Key value, returns empty string on failure</returns>
        public static string GetGameConfigKeyValue(int configId, string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            var buffer = new StringBuilder(256);
            if (ExtendedNativeMethods.GetGameConfigKeyValue(configId, key, buffer, buffer.Capacity))
                return buffer.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 关闭游戏配置 / Close game config
        /// </summary>
        /// <param name="configId">配置ID / Config ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CloseGameConfig(int configId)
        {
            return ExtendedNativeMethods.CloseGameConfig(configId);
        }
    }

    /// <summary>
    /// Native函数管理器 / Native function manager
    /// </summary>
    public static class NativeManager
    {
        private static readonly Dictionary<string, NativeCallback> _nativeCallbacks = new Dictionary<string, NativeCallback>();

        /// <summary>
        /// 注册Native函数 / Register native function
        /// </summary>
        /// <param name="name">函数名 / Function name</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool RegisterNative(string name, NativeCallback callback)
        {
            if (string.IsNullOrEmpty(name) || callback == null)
                return false;

            bool result = ExtendedNativeMethods.RegisterNative(name, callback);
            if (result)
            {
                _nativeCallbacks[name] = callback;
            }

            return result;
        }

        /// <summary>
        /// 取消注册Native函数 / Unregister native function
        /// </summary>
        /// <param name="name">函数名 / Function name</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool UnregisterNative(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            bool result = ExtendedNativeMethods.UnregisterNative(name);
            if (result)
            {
                _nativeCallbacks.Remove(name);
            }

            return result;
        }

        /// <summary>
        /// 获取Native参数 / Get native parameter
        /// </summary>
        /// <param name="index">参数索引 / Parameter index</param>
        /// <returns>参数值 / Parameter value</returns>
        public static int GetNativeParam(int index)
        {
            return ExtendedNativeMethods.GetNativeParam(index);
        }

        /// <summary>
        /// 获取Native字符串参数 / Get native string parameter
        /// </summary>
        /// <param name="index">参数索引 / Parameter index</param>
        /// <returns>字符串值 / String value</returns>
        public static string GetNativeString(int index)
        {
            var buffer = new StringBuilder(256);
            if (ExtendedNativeMethods.GetNativeString(index, buffer, buffer.Capacity))
                return buffer.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 设置Native字符串参数 / Set native string parameter
        /// </summary>
        /// <param name="index">参数索引 / Parameter index</param>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetNativeString(int index, string value)
        {
            return ExtendedNativeMethods.SetNativeString(index, value ?? "");
        }

        /// <summary>
        /// 获取Native数组参数 / Get native array parameter
        /// </summary>
        /// <param name="index">参数索引 / Parameter index</param>
        /// <param name="size">数组大小 / Array size</param>
        /// <returns>数组值 / Array value</returns>
        public static int[] GetNativeArray(int index, int size)
        {
            if (size <= 0)
                return new int[0];

            int[] buffer = new int[size];
            if (ExtendedNativeMethods.GetNativeArray(index, buffer, size))
                return buffer;

            return new int[0];
        }

        /// <summary>
        /// 设置Native数组参数 / Set native array parameter
        /// </summary>
        /// <param name="index">参数索引 / Parameter index</param>
        /// <param name="buffer">数组值 / Array value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetNativeArray(int index, int[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return false;

            return ExtendedNativeMethods.SetNativeArray(index, buffer, buffer.Length);
        }
    }

    /// <summary>
    /// 消息系统管理器 / Message system manager
    /// </summary>
    public static class MessageManager
    {
        private static readonly Dictionary<int, MessageCallback> _messageCallbacks = new Dictionary<int, MessageCallback>();

        /// <summary>
        /// 开始消息 / Begin message
        /// </summary>
        /// <param name="msgType">消息类型 / Message type</param>
        /// <param name="msgDest">消息目标 / Message destination</param>
        /// <param name="entityId">实体ID / Entity ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool BeginMessage(int msgType, int msgDest, int entityId = 0)
        {
            return ExtendedNativeMethods.BeginMessage(msgType, msgDest, entityId);
        }

        /// <summary>
        /// 结束消息 / End message
        /// </summary>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool EndMessage()
        {
            return ExtendedNativeMethods.EndMessage();
        }

        /// <summary>
        /// 写入字节 / Write byte
        /// </summary>
        /// <param name="value">字节值 / Byte value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteByte(int value)
        {
            return ExtendedNativeMethods.WriteByte(value);
        }

        /// <summary>
        /// 写入字符 / Write char
        /// </summary>
        /// <param name="value">字符值 / Char value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteChar(int value)
        {
            return ExtendedNativeMethods.WriteChar(value);
        }

        /// <summary>
        /// 写入短整数 / Write short
        /// </summary>
        /// <param name="value">短整数值 / Short value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteShort(int value)
        {
            return ExtendedNativeMethods.WriteShort(value);
        }

        /// <summary>
        /// 写入长整数 / Write long
        /// </summary>
        /// <param name="value">长整数值 / Long value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteLong(int value)
        {
            return ExtendedNativeMethods.WriteLong(value);
        }

        /// <summary>
        /// 写入角度 / Write angle
        /// </summary>
        /// <param name="value">角度值 / Angle value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteAngle(float value)
        {
            return ExtendedNativeMethods.WriteAngle(value);
        }

        /// <summary>
        /// 写入坐标 / Write coordinate
        /// </summary>
        /// <param name="value">坐标值 / Coordinate value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteCoord(float value)
        {
            return ExtendedNativeMethods.WriteCoord(value);
        }

        /// <summary>
        /// 写入字符串 / Write string
        /// </summary>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteString(string value)
        {
            return ExtendedNativeMethods.WriteString(value ?? "");
        }

        /// <summary>
        /// 写入实体 / Write entity
        /// </summary>
        /// <param name="entityId">实体ID / Entity ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WriteEntity(int entityId)
        {
            return ExtendedNativeMethods.WriteEntity(entityId);
        }

        /// <summary>
        /// 注册消息钩子 / Register message hook
        /// </summary>
        /// <param name="msgId">消息ID / Message ID</param>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>钩子ID，失败返回-1 / Hook ID, returns -1 on failure</returns>
        public static int RegisterMessage(int msgId, MessageCallback callback)
        {
            if (callback == null)
                return -1;

            int hookId = ExtendedNativeMethods.RegisterMessage(msgId, callback);
            if (hookId != -1)
            {
                _messageCallbacks[hookId] = callback;
            }

            return hookId;
        }

        /// <summary>
        /// 取消注册消息钩子 / Unregister message hook
        /// </summary>
        /// <param name="hookId">钩子ID / Hook ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool UnregisterMessage(int hookId)
        {
            bool result = ExtendedNativeMethods.UnregisterMessage(hookId);
            if (result)
            {
                _messageCallbacks.Remove(hookId);
            }

            return result;
        }
    }

    /// <summary>
    /// 数据包管理器 / Data pack manager
    /// </summary>
    public static class DataPackManager
    {
        /// <summary>
        /// 创建数据包 / Create data pack
        /// </summary>
        /// <returns>数据包ID，失败返回-1 / Data pack ID, returns -1 on failure</returns>
        public static int CreateDataPack()
        {
            return ExtendedNativeMethods.CreateDataPack();
        }

        /// <summary>
        /// 写入整数到数据包 / Write cell to data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <param name="value">整数值 / Integer value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WritePackCell(int packId, int value)
        {
            return ExtendedNativeMethods.WritePackCell(packId, value);
        }

        /// <summary>
        /// 写入浮点数到数据包 / Write float to data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <param name="value">浮点数值 / Float value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WritePackFloat(int packId, float value)
        {
            return ExtendedNativeMethods.WritePackFloat(packId, value);
        }

        /// <summary>
        /// 写入字符串到数据包 / Write string to data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool WritePackString(int packId, string value)
        {
            return ExtendedNativeMethods.WritePackString(packId, value ?? "");
        }

        /// <summary>
        /// 从数据包读取整数 / Read cell from data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>整数值 / Integer value</returns>
        public static int ReadPackCell(int packId)
        {
            return ExtendedNativeMethods.ReadPackCell(packId);
        }

        /// <summary>
        /// 从数据包读取浮点数 / Read float from data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>浮点数值 / Float value</returns>
        public static float ReadPackFloat(int packId)
        {
            return ExtendedNativeMethods.ReadPackFloat(packId);
        }

        /// <summary>
        /// 从数据包读取字符串 / Read string from data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>字符串值 / String value</returns>
        public static string ReadPackString(int packId)
        {
            var buffer = new StringBuilder(256);
            if (ExtendedNativeMethods.ReadPackString(packId, buffer, buffer.Capacity))
                return buffer.ToString();

            return string.Empty;
        }

        /// <summary>
        /// 重置数据包 / Reset data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool ResetPack(int packId)
        {
            return ExtendedNativeMethods.ResetPack(packId);
        }

        /// <summary>
        /// 获取数据包位置 / Get pack position
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>位置 / Position</returns>
        public static int GetPackPosition(int packId)
        {
            return ExtendedNativeMethods.GetPackPosition(packId);
        }

        /// <summary>
        /// 设置数据包位置 / Set pack position
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <param name="position">位置 / Position</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool SetPackPosition(int packId, int position)
        {
            return ExtendedNativeMethods.SetPackPosition(packId, position);
        }

        /// <summary>
        /// 检查数据包是否结束 / Check if pack is ended
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>是否结束 / Whether ended</returns>
        public static bool IsPackEnded(int packId)
        {
            return ExtendedNativeMethods.IsPackEnded(packId);
        }

        /// <summary>
        /// 销毁数据包 / Destroy data pack
        /// </summary>
        /// <param name="packId">数据包ID / Data pack ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool DestroyDataPack(int packId)
        {
            return ExtendedNativeMethods.DestroyDataPack(packId);
        }
    }

    /// <summary>
    /// 核心AMX功能管理器 / Core AMX functionality manager
    /// </summary>
    public static class CoreAmxManager
    {
        private static readonly Dictionary<int, LogCallback> _logCallbacks = new Dictionary<int, LogCallback>();

        #region Plugin Management / 插件管理

        /// <summary>
        /// 获取插件数量 / Get plugins number
        /// </summary>
        /// <returns>插件数量 / Number of plugins</returns>
        public static int GetPluginsNum()
        {
            return ExtendedNativeMethods.GetPluginsNum();
        }

        /// <summary>
        /// 获取插件信息 / Get plugin information
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>插件信息，失败返回null / Plugin info, returns null on failure</returns>
        public static PluginInfo? GetPluginInfo(int pluginId)
        {
            if (ExtendedNativeMethods.GetPluginInfo(pluginId, out PluginInfo info))
                return info;

            return null;
        }

        /// <summary>
        /// 查找插件 / Find plugin
        /// </summary>
        /// <param name="fileName">文件名 / File name</param>
        /// <returns>插件ID，失败返回-1 / Plugin ID, returns -1 on failure</returns>
        public static int FindPlugin(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return -1;

            return ExtendedNativeMethods.FindPlugin(fileName);
        }

        /// <summary>
        /// 检查插件是否有效 / Check if plugin is valid
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>是否有效 / Whether valid</returns>
        public static bool IsPluginValid(int pluginId)
        {
            return ExtendedNativeMethods.IsPluginValid(pluginId);
        }

        /// <summary>
        /// 检查插件是否运行中 / Check if plugin is running
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>是否运行中 / Whether running</returns>
        public static bool IsPluginRunning(int pluginId)
        {
            return ExtendedNativeMethods.IsPluginRunning(pluginId);
        }

        /// <summary>
        /// 暂停插件 / Pause plugin
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool PausePlugin(int pluginId)
        {
            return ExtendedNativeMethods.PausePlugin(pluginId);
        }

        /// <summary>
        /// 恢复插件 / Unpause plugin
        /// </summary>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool UnpausePlugin(int pluginId)
        {
            return ExtendedNativeMethods.UnpausePlugin(pluginId);
        }

        #endregion

        #region Function Call System / 函数调用系统

        /// <summary>
        /// 开始函数调用 / Begin function call
        /// </summary>
        /// <param name="funcName">函数名 / Function name</param>
        /// <param name="pluginName">插件名，为空则自动查找 / Plugin name, empty for auto-find</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncBegin(string funcName, string pluginName = "")
        {
            if (string.IsNullOrEmpty(funcName))
                return false;

            return ExtendedNativeMethods.CallFuncBegin(funcName, pluginName ?? "");
        }

        /// <summary>
        /// 通过ID开始函数调用 / Begin function call by ID
        /// </summary>
        /// <param name="funcId">函数ID / Function ID</param>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncBeginById(int funcId, int pluginId)
        {
            return ExtendedNativeMethods.CallFuncBeginById(funcId, pluginId);
        }

        /// <summary>
        /// 压入整数参数 / Push integer parameter
        /// </summary>
        /// <param name="value">整数值 / Integer value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncPushInt(int value)
        {
            return ExtendedNativeMethods.CallFuncPushInt(value);
        }

        /// <summary>
        /// 压入浮点参数 / Push float parameter
        /// </summary>
        /// <param name="value">浮点值 / Float value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncPushFloat(float value)
        {
            return ExtendedNativeMethods.CallFuncPushFloat(value);
        }

        /// <summary>
        /// 压入字符串参数 / Push string parameter
        /// </summary>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncPushString(string value)
        {
            return ExtendedNativeMethods.CallFuncPushString(value ?? "");
        }

        /// <summary>
        /// 压入数组参数 / Push array parameter
        /// </summary>
        /// <param name="array">数组值 / Array value</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool CallFuncPushArray(int[] array)
        {
            if (array == null || array.Length == 0)
                return false;

            return ExtendedNativeMethods.CallFuncPushArray(array, array.Length);
        }

        /// <summary>
        /// 结束函数调用 / End function call
        /// </summary>
        /// <returns>函数返回值 / Function return value</returns>
        public static int CallFuncEnd()
        {
            return ExtendedNativeMethods.CallFuncEnd();
        }

        /// <summary>
        /// 获取函数ID / Get function ID
        /// </summary>
        /// <param name="funcName">函数名 / Function name</param>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <returns>函数ID，失败返回-1 / Function ID, returns -1 on failure</returns>
        public static int GetFuncId(string funcName, int pluginId)
        {
            if (string.IsNullOrEmpty(funcName))
                return -1;

            return ExtendedNativeMethods.GetFuncId(funcName, pluginId);
        }

        #endregion

        #region Forward System / Forward系统

        /// <summary>
        /// Forward执行类型 / Forward execution type
        /// </summary>
        public enum ForwardExecType
        {
            /// <summary>忽略返回值 / Ignore return value</summary>
            Ignore = 0,
            /// <summary>停止执行 / Stop execution</summary>
            Stop = 1,
            /// <summary>停止执行2 / Stop execution 2</summary>
            Stop2 = 2,
            /// <summary>继续执行 / Continue execution</summary>
            Continue = 3
        }

        /// <summary>
        /// Forward参数类型 / Forward parameter type
        /// </summary>
        public enum ForwardParamType
        {
            /// <summary>整数 / Integer</summary>
            Cell = 1,
            /// <summary>浮点数 / Float</summary>
            Float = 2,
            /// <summary>字符串 / String</summary>
            String = 3,
            /// <summary>数组 / Array</summary>
            Array = 4,
            /// <summary>完成标记 / Done marker</summary>
            Done = 5
        }

        /// <summary>
        /// 创建Forward / Create forward
        /// </summary>
        /// <param name="funcName">函数名 / Function name</param>
        /// <param name="execType">执行类型 / Execution type</param>
        /// <param name="paramTypes">参数类型数组 / Parameter types array</param>
        /// <returns>Forward ID，失败返回-1 / Forward ID, returns -1 on failure</returns>
        public static int CreateForward(string funcName, ForwardExecType execType, params ForwardParamType[] paramTypes)
        {
            if (string.IsNullOrEmpty(funcName))
                return -1;

            int[] types = paramTypes?.Select(t => (int)t).ToArray() ?? new int[0];
            return ExtendedNativeMethods.CreateForward(funcName, (int)execType, types, types.Length);
        }

        /// <summary>
        /// 创建单插件Forward / Create single plugin forward
        /// </summary>
        /// <param name="funcName">函数名 / Function name</param>
        /// <param name="pluginId">插件ID / Plugin ID</param>
        /// <param name="paramTypes">参数类型数组 / Parameter types array</param>
        /// <returns>Forward ID，失败返回-1 / Forward ID, returns -1 on failure</returns>
        public static int CreateSPForward(string funcName, int pluginId, params ForwardParamType[] paramTypes)
        {
            if (string.IsNullOrEmpty(funcName))
                return -1;

            int[] types = paramTypes?.Select(t => (int)t).ToArray() ?? new int[0];
            return ExtendedNativeMethods.CreateSPForward(funcName, pluginId, types, types.Length);
        }

        /// <summary>
        /// 销毁Forward / Destroy forward
        /// </summary>
        /// <param name="forwardId">Forward ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool DestroyForward(int forwardId)
        {
            return ExtendedNativeMethods.DestroyForward(forwardId);
        }

        /// <summary>
        /// 执行Forward / Execute forward
        /// </summary>
        /// <param name="forwardId">Forward ID</param>
        /// <param name="parameters">参数数组 / Parameters array</param>
        /// <returns>执行结果 / Execution result</returns>
        public static int ExecuteForward(int forwardId, params int[] parameters)
        {
            int[] params = parameters ?? new int[0];
            return ExtendedNativeMethods.ExecuteForward(forwardId, params, params.Length);
        }

        /// <summary>
        /// 获取Forward信息 / Get forward information
        /// </summary>
        /// <param name="forwardId">Forward ID</param>
        /// <returns>Forward信息，失败返回null / Forward info, returns null on failure</returns>
        public static ForwardInfo? GetForwardInfo(int forwardId)
        {
            if (ExtendedNativeMethods.GetForwardInfo(forwardId, out ForwardInfo info))
                return info;

            return null;
        }

        #endregion

        #region Server Management / 服务器管理

        /// <summary>
        /// 服务器打印消息 / Server print message
        /// </summary>
        /// <param name="message">消息内容 / Message content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool ServerPrint(string message)
        {
            return ExtendedNativeMethods.ServerPrint(message ?? "");
        }

        /// <summary>
        /// 执行服务器命令 / Execute server command
        /// </summary>
        /// <param name="command">命令内容 / Command content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool ServerCmd(string command)
        {
            return ExtendedNativeMethods.ServerCmd(command ?? "");
        }

        /// <summary>
        /// 执行服务器命令队列 / Execute server command queue
        /// </summary>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool ServerExec()
        {
            return ExtendedNativeMethods.ServerExec();
        }

        /// <summary>
        /// 检查是否为专用服务器 / Check if dedicated server
        /// </summary>
        /// <returns>是否为专用服务器 / Whether dedicated server</returns>
        public static bool IsDedicatedServer()
        {
            return ExtendedNativeMethods.IsDedicatedServer();
        }

        /// <summary>
        /// 检查是否为Linux服务器 / Check if Linux server
        /// </summary>
        /// <returns>是否为Linux服务器 / Whether Linux server</returns>
        public static bool IsLinuxServer()
        {
            return ExtendedNativeMethods.IsLinuxServer();
        }

        /// <summary>
        /// 检查地图是否有效 / Check if map is valid
        /// </summary>
        /// <param name="mapName">地图名称 / Map name</param>
        /// <returns>是否有效 / Whether valid</returns>
        public static bool IsMapValid(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
                return false;

            return ExtendedNativeMethods.IsMapValid(mapName);
        }

        #endregion

        #region Client Management / 客户端管理

        /// <summary>
        /// 获取玩家数量 / Get players number
        /// </summary>
        /// <param name="includeConnecting">是否包含连接中的玩家 / Whether to include connecting players</param>
        /// <returns>玩家数量 / Number of players</returns>
        public static int GetPlayersNum(bool includeConnecting = false)
        {
            return ExtendedNativeMethods.GetPlayersNum(includeConnecting);
        }

        /// <summary>
        /// 检查用户是否为机器人 / Check if user is bot
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <returns>是否为机器人 / Whether is bot</returns>
        public static bool IsUserBot(int clientId)
        {
            return ExtendedNativeMethods.IsUserBot(clientId);
        }

        /// <summary>
        /// 检查用户是否已连接 / Check if user is connected
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <returns>是否已连接 / Whether connected</returns>
        public static bool IsUserConnected(int clientId)
        {
            return ExtendedNativeMethods.IsUserConnected(clientId);
        }

        /// <summary>
        /// 检查用户是否存活 / Check if user is alive
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <returns>是否存活 / Whether alive</returns>
        public static bool IsUserAlive(int clientId)
        {
            return ExtendedNativeMethods.IsUserAlive(clientId);
        }

        /// <summary>
        /// 获取用户时间 / Get user time
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <param name="playtime">是否获取游戏时间 / Whether to get playtime</param>
        /// <returns>时间（秒） / Time in seconds</returns>
        public static int GetUserTime(int clientId, bool playtime = false)
        {
            return ExtendedNativeMethods.GetUserTime(clientId, playtime);
        }

        /// <summary>
        /// 向客户端发送命令 / Send command to client
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <param name="command">命令内容 / Command content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool ClientCmd(int clientId, string command)
        {
            return ExtendedNativeMethods.ClientCmd(clientId, command ?? "");
        }

        /// <summary>
        /// 向客户端发送虚假命令 / Send fake command to client
        /// </summary>
        /// <param name="clientId">客户端ID / Client ID</param>
        /// <param name="command">命令内容 / Command content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool FakeClientCmd(int clientId, string command)
        {
            return ExtendedNativeMethods.FakeClientCmd(clientId, command ?? "");
        }

        #endregion

        #region Admin Management / 管理员管理

        /// <summary>
        /// 管理员属性类型 / Admin property type
        /// </summary>
        public enum AdminProperty
        {
            /// <summary>认证数据 / Auth data</summary>
            Auth = 0,
            /// <summary>密码 / Password</summary>
            Password = 1,
            /// <summary>访问权限 / Access flags</summary>
            Access = 2,
            /// <summary>管理员标志 / Admin flags</summary>
            Flags = 3
        }

        /// <summary>
        /// 添加管理员 / Add admin
        /// </summary>
        /// <param name="authData">认证数据（SteamID、IP或名称） / Auth data (SteamID, IP or name)</param>
        /// <param name="password">密码 / Password</param>
        /// <param name="access">访问权限 / Access flags</param>
        /// <param name="flags">管理员标志 / Admin flags</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AdminsPush(string authData, string password, int access, int flags)
        {
            if (string.IsNullOrEmpty(authData))
                return false;

            return ExtendedNativeMethods.AdminsPush(authData, password ?? "", access, flags);
        }

        /// <summary>
        /// 清空管理员列表 / Clear admins list
        /// </summary>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AdminsFlush()
        {
            return ExtendedNativeMethods.AdminsFlush();
        }

        /// <summary>
        /// 获取管理员数量 / Get admins number
        /// </summary>
        /// <returns>管理员数量 / Number of admins</returns>
        public static int AdminsNum()
        {
            return ExtendedNativeMethods.AdminsNum();
        }

        /// <summary>
        /// 查找管理员信息 / Lookup admin information
        /// </summary>
        /// <param name="index">索引 / Index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值，失败返回null / Property value, returns null on failure</returns>
        public static object AdminsLookup(int index, AdminProperty property)
        {
            var buffer = new StringBuilder(256);
            int outValue;

            if (ExtendedNativeMethods.AdminsLookup(index, (int)property, buffer, buffer.Capacity, out outValue))
            {
                switch (property)
                {
                    case AdminProperty.Auth:
                    case AdminProperty.Password:
                        return buffer.ToString();
                    case AdminProperty.Access:
                    case AdminProperty.Flags:
                        return outValue;
                }
            }

            return null;
        }

        #endregion

        #region Logging Management / 日志管理

        /// <summary>
        /// 日志级别 / Log level
        /// </summary>
        public enum LogLevel
        {
            /// <summary>调试 / Debug</summary>
            Debug = 0,
            /// <summary>信息 / Info</summary>
            Info = 1,
            /// <summary>警告 / Warning</summary>
            Warning = 2,
            /// <summary>错误 / Error</summary>
            Error = 3,
            /// <summary>致命错误 / Fatal</summary>
            Fatal = 4
        }

        /// <summary>
        /// 记录到文件 / Log to file
        /// </summary>
        /// <param name="fileName">文件名 / File name</param>
        /// <param name="message">消息内容 / Message content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool LogToFile(string fileName, string message)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(message))
                return false;

            return ExtendedNativeMethods.LogToFile(fileName, message);
        }

        /// <summary>
        /// AMX日志 / AMX log
        /// </summary>
        /// <param name="message">消息内容 / Message content</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool LogAmx(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            return ExtendedNativeMethods.LogAmx(message);
        }

        /// <summary>
        /// 记录错误 / Log error
        /// </summary>
        /// <param name="errorCode">错误代码 / Error code</param>
        /// <param name="message">错误消息 / Error message</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool LogError(int errorCode, string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            return ExtendedNativeMethods.LogError(errorCode, message);
        }

        /// <summary>
        /// 注册日志回调 / Register log callback
        /// </summary>
        /// <param name="callback">回调函数 / Callback function</param>
        /// <returns>回调ID，失败返回-1 / Callback ID, returns -1 on failure</returns>
        public static int RegisterLogCallback(Action<LogLevel, string> callback)
        {
            if (callback == null)
                return -1;

            var nativeCallback = new LogCallback((level, message) => callback((LogLevel)level, message));
            int callbackId = ExtendedNativeMethods.RegisterLogCallback(nativeCallback);

            if (callbackId >= 0)
            {
                _logCallbacks[callbackId] = nativeCallback;
            }

            return callbackId;
        }

        /// <summary>
        /// 取消注册日志回调 / Unregister log callback
        /// </summary>
        /// <param name="callbackId">回调ID / Callback ID</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool UnregisterLogCallback(int callbackId)
        {
            if (_logCallbacks.ContainsKey(callbackId))
            {
                _logCallbacks.Remove(callbackId);
            }

            return ExtendedNativeMethods.UnregisterLogCallback(callbackId);
        }

        #endregion

        #region Library Management / 库管理

        /// <summary>
        /// 注册库 / Register library
        /// </summary>
        /// <param name="libraryName">库名称 / Library name</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool RegisterLibrary(string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
                return false;

            return ExtendedNativeMethods.RegisterLibrary(libraryName);
        }

        /// <summary>
        /// 检查库是否存在 / Check if library exists
        /// </summary>
        /// <param name="libraryName">库名称 / Library name</param>
        /// <returns>是否存在 / Whether exists</returns>
        public static bool LibraryExists(string libraryName)
        {
            if (string.IsNullOrEmpty(libraryName))
                return false;

            return ExtendedNativeMethods.LibraryExists(libraryName);
        }

        #endregion

        #region Utility Functions / 工具函数

        /// <summary>
        /// 中止执行 / Abort execution
        /// </summary>
        /// <param name="errorCode">错误代码 / Error code</param>
        /// <param name="message">错误消息 / Error message</param>
        /// <returns>是否成功 / Whether successful</returns>
        public static bool AbortExecution(int errorCode, string message = "")
        {
            return ExtendedNativeMethods.AbortExecution(errorCode, message ?? "");
        }

        /// <summary>
        /// 获取堆空间 / Get heap space
        /// </summary>
        /// <returns>堆空间大小 / Heap space size</returns>
        public static int GetHeapSpace()
        {
            return ExtendedNativeMethods.GetHeapSpace();
        }

        /// <summary>
        /// 获取参数数量 / Get number of arguments
        /// </summary>
        /// <returns>参数数量 / Number of arguments</returns>
        public static int GetNumArgs()
        {
            return ExtendedNativeMethods.GetNumArgs();
        }

        /// <summary>
        /// 交换字符串中的字符 / Swap characters in string
        /// </summary>
        /// <param name="text">文本 / Text</param>
        /// <param name="char1">字符1 / Character 1</param>
        /// <param name="char2">字符2 / Character 2</param>
        /// <returns>交换后的字符串 / String after swapping</returns>
        public static string SwapChars(string text, char char1, char char2)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var buffer = new StringBuilder(text);
            ExtendedNativeMethods.SwapChars(buffer, char1, char2);
            return buffer.ToString();
        }

        /// <summary>
        /// 生成随机整数 / Generate random integer
        /// </summary>
        /// <param name="max">最大值（不包含） / Maximum value (exclusive)</param>
        /// <returns>随机整数 / Random integer</returns>
        public static int RandomInt(int max)
        {
            return ExtendedNativeMethods.RandomInt(max);
        }

        /// <summary>
        /// 获取两个整数的最小值 / Get minimum of two integers
        /// </summary>
        /// <param name="a">整数A / Integer A</param>
        /// <param name="b">整数B / Integer B</param>
        /// <returns>最小值 / Minimum value</returns>
        public static int MinInt(int a, int b)
        {
            return ExtendedNativeMethods.MinInt(a, b);
        }

        /// <summary>
        /// 获取两个整数的最大值 / Get maximum of two integers
        /// </summary>
        /// <param name="a">整数A / Integer A</param>
        /// <param name="b">整数B / Integer B</param>
        /// <returns>最大值 / Maximum value</returns>
        public static int MaxInt(int a, int b)
        {
            return ExtendedNativeMethods.MaxInt(a, b);
        }

        /// <summary>
        /// 限制整数在指定范围内 / Clamp integer within specified range
        /// </summary>
        /// <param name="value">值 / Value</param>
        /// <param name="min">最小值 / Minimum value</param>
        /// <param name="max">最大值 / Maximum value</param>
        /// <returns>限制后的值 / Clamped value</returns>
        public static int ClampInt(int value, int min, int max)
        {
            return ExtendedNativeMethods.ClampInt(value, min, max);
        }

        #endregion
    }
}
