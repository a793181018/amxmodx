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
}
