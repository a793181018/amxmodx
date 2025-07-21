// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Engine Module C# Interop Layer
//

using System;
using System.Runtime.InteropServices;

namespace AmxModX.Engine
{
    /// <summary>
    /// 三维向量结构 / 3D Vector structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        /// <summary>X坐标 / X coordinate</summary>
        public float X;
        /// <summary>Y坐标 / Y coordinate</summary>
        public float Y;
        /// <summary>Z坐标 / Z coordinate</summary>
        public float Z;

        /// <summary>
        /// 构造函数 / Constructor
        /// </summary>
        /// <param name="x">X坐标 / X coordinate</param>
        /// <param name="y">Y坐标 / Y coordinate</param>
        /// <param name="z">Z坐标 / Z coordinate</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// 零向量 / Zero vector
        /// </summary>
        public static Vector3 Zero => new Vector3(0, 0, 0);

        /// <summary>
        /// 单位向量 / Unit vector
        /// </summary>
        public static Vector3 One => new Vector3(1, 1, 1);
    }

    /// <summary>
    /// 追踪结果结构 / Trace result structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TraceResult
    {
        /// <summary>是否完全在固体中 / Whether all solid</summary>
        public int AllSolid;
        /// <summary>是否起始在固体中 / Whether start solid</summary>
        public int StartSolid;
        /// <summary>是否在开放空间 / Whether in open</summary>
        public int InOpen;
        /// <summary>是否在水中 / Whether in water</summary>
        public int InWater;
        /// <summary>碰撞分数 / Collision fraction</summary>
        public float Fraction;
        /// <summary>结束位置 / End position</summary>
        public Vector3 EndPos;
        /// <summary>平面距离 / Plane distance</summary>
        public float PlaneDist;
        /// <summary>平面法线 / Plane normal</summary>
        public Vector3 PlaneNormal;
        /// <summary>碰撞实体 / Hit entity</summary>
        public int HitEntity;
        /// <summary>碰撞组 / Hit group</summary>
        public int HitGroup;
    }

    /// <summary>
    /// 用户命令结构 / User command structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UserCmd
    {
        /// <summary>插值毫秒 / Lerp milliseconds</summary>
        public short LerpMsec;
        /// <summary>毫秒 / Milliseconds</summary>
        public byte Msec;
        /// <summary>视角 / View angles</summary>
        public Vector3 ViewAngles;
        /// <summary>前进移动 / Forward move</summary>
        public float ForwardMove;
        /// <summary>侧向移动 / Side move</summary>
        public float SideMove;
        /// <summary>向上移动 / Up move</summary>
        public float UpMove;
        /// <summary>光照级别 / Light level</summary>
        public short LightLevel;
        /// <summary>按钮状态 / Button state</summary>
        public ushort Buttons;
        /// <summary>脉冲 / Impulse</summary>
        public byte Impulse;
        /// <summary>武器选择 / Weapon select</summary>
        public byte WeaponSelect;
        /// <summary>撞击索引 / Impact index</summary>
        public int ImpactIndex;
        /// <summary>撞击位置 / Impact position</summary>
        public Vector3 ImpactPosition;
    }

    /// <summary>
    /// 实体属性类型枚举 / Entity property type enumeration
    /// </summary>
    public enum EntityProperty
    {
        // Float properties
        Health = 0,
        MaxHealth = 1,
        Speed = 2,
        Gravity = 3,
        TakeDamage = 4,
        Damage = 5,

        // Integer properties  
        MoveType = 0,
        Solid = 1,
        Skin = 2,
        Body = 3,
        Effects = 4,
        Sequence = 5,
        ModelIndex = 6,
        WaterLevel = 7,
        WaterType = 8,
        SpawnFlags = 9,
        Flags = 10,
        Team = 11,
        Weapons = 12,
        RenderMode = 13,
        RenderFx = 14,
        Button = 15,
        Impulse = 16,
        DeadFlag = 17,

        // Vector properties
        Origin = 0,
        OldOrigin = 1,
        Velocity = 2,
        BaseVelocity = 3,
        MoveDir = 4,
        Angles = 5,
        AngularVelocity = 6,
        PunchAngle = 7,
        ViewAngle = 8,
        EndPos = 9,
        StartPos = 10,
        AbsMin = 11,
        AbsMax = 12,
        Mins = 13,
        Maxs = 14,
        Size = 15,
        RenderColor = 16,
        ViewOffset = 17
    }

    /// <summary>
    /// 视图类型枚举 / View type enumeration
    /// </summary>
    public enum ViewType
    {
        /// <summary>无摄像机 / No camera</summary>
        None = 0,
        /// <summary>第三人称 / Third person</summary>
        ThirdPerson = 1,
        /// <summary>左上角 / Up left</summary>
        UpLeft = 2,
        /// <summary>俯视 / Top down</summary>
        TopDown = 3
    }

    /// <summary>
    /// 委托类型定义 / Delegate type definitions
    /// </summary>
    public static class EngineDelegates
    {
        /// <summary>
        /// 实体生成回调委托 / Entity spawn callback delegate
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        public delegate void EntitySpawnCallback(int entityId);

        /// <summary>
        /// 实体移除回调委托 / Entity remove callback delegate
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        public delegate void EntityRemoveCallback(int entityId);

        /// <summary>
        /// 实体触碰回调委托 / Entity touch callback delegate
        /// </summary>
        /// <param name="toucher">触碰者实体索引 / Toucher entity index</param>
        /// <param name="touched">被触碰者实体索引 / Touched entity index</param>
        public delegate void EntityTouchCallback(int toucher, int touched);

        /// <summary>
        /// 实体思考回调委托 / Entity think callback delegate
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        public delegate void EntityThinkCallback(int entityId);

        /// <summary>
        /// 玩家脉冲回调委托 / Player impulse callback delegate
        /// </summary>
        /// <param name="playerId">玩家索引 / Player index</param>
        /// <param name="impulse">脉冲值 / Impulse value</param>
        public delegate void PlayerImpulseCallback(int playerId, int impulse);
    }

    /// <summary>
    /// Engine模块原生接口导入 / Engine module native interface imports
    /// </summary>
    internal static class EngineNativeImports
    {
        private const string DllName = "amxmodx";

        // ========== Entity Lifecycle Management ==========

        /// <summary>
        /// 创建实体 / Create entity
        /// </summary>
        /// <param name="className">实体类名 / Entity class name</param>
        /// <returns>实体索引，失败返回0 / Entity index, returns 0 on failure</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int CreateEntity([MarshalAs(UnmanagedType.LPStr)] string className);

        /// <summary>
        /// 移除实体 / Remove entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool RemoveEntity(int entityId);

        /// <summary>
        /// 获取实体总数 / Get entity count
        /// </summary>
        /// <returns>实体总数 / Entity count</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetEntityCount();

        /// <summary>
        /// 检查实体是否有效 / Check if entity is valid
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <returns>是否有效 / Whether valid</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool IsValidEntity(int entityId);

        // ========== Entity Property Read/Write ==========

        /// <summary>
        /// 获取实体浮点属性 / Get entity float property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值 / Property value</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern float GetEntityFloat(int entityId, int property);

        /// <summary>
        /// 设置实体浮点属性 / Set entity float property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityFloat(int entityId, int property, float value);

        /// <summary>
        /// 获取实体整数属性 / Get entity integer property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值 / Property value</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetEntityInt(int entityId, int property);

        /// <summary>
        /// 设置实体整数属性 / Set entity integer property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityInt(int entityId, int property, int value);

        /// <summary>
        /// 获取实体向量属性 / Get entity vector property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="vector">输出向量 / Output vector</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool GetEntityVector(int entityId, int property, out Vector3 vector);

        /// <summary>
        /// 设置实体向量属性 / Set entity vector property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="vector">向量值 / Vector value</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityVector(int entityId, int property, ref Vector3 vector);

        /// <summary>
        /// 获取实体字符串属性 / Get entity string property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="buffer">输出缓冲区 / Output buffer</param>
        /// <param name="maxLength">缓冲区最大长度 / Buffer max length</param>
        /// <returns>实际长度 / Actual length</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int GetEntityString(int entityId, int property,
            [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder buffer, int maxLength);

        /// <summary>
        /// 设置实体字符串属性 / Set entity string property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityString(int entityId, int property,
            [MarshalAs(UnmanagedType.LPStr)] string value);

        /// <summary>
        /// 获取实体关联的实体 / Get entity's associated entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>关联实体索引 / Associated entity index</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetEntityEdict(int entityId, int property);

        /// <summary>
        /// 设置实体关联的实体 / Set entity's associated entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="targetEntityId">目标实体索引 / Target entity index</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityEdict(int entityId, int property, int targetEntityId);

        /// <summary>
        /// 设置实体原点 / Set entity origin
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="origin">原点坐标 / Origin coordinates</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityOrigin(int entityId, ref Vector3 origin);

        /// <summary>
        /// 设置实体模型 / Set entity model
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="modelName">模型名称 / Model name</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntityModel(int entityId,
            [MarshalAs(UnmanagedType.LPStr)] string modelName);

        /// <summary>
        /// 设置实体大小 / Set entity size
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="mins">最小边界 / Minimum bounds</param>
        /// <param name="maxs">最大边界 / Maximum bounds</param>
        /// <returns>是否成功 / Whether successful</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool SetEntitySize(int entityId, ref Vector3 mins, ref Vector3 maxs);

        // ========== Entity Search and Finding ==========

        /// <summary>
        /// 获取实体范围内的距离 / Get distance to entity in range
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="targetEntityId">目标实体索引 / Target entity index</param>
        /// <returns>距离值 / Distance value</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern float GetEntityRange(int entityId, int targetEntityId);

        /// <summary>
        /// 在球形范围内查找实体 / Find entity in sphere
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="origin">中心点 / Center point</param>
        /// <param name="radius">半径 / Radius</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int FindEntityInSphere(int startEntityId, ref Vector3 origin, float radius);

        /// <summary>
        /// 根据类名查找实体 / Find entity by class name
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="className">类名 / Class name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindEntityByClass(int startEntityId,
            [MarshalAs(UnmanagedType.LPStr)] string className);

        /// <summary>
        /// 在球形范围内根据类名查找实体 / Find entity by class in sphere
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="origin">中心点 / Center point</param>
        /// <param name="radius">半径 / Radius</param>
        /// <param name="className">类名 / Class name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindEntityByClassInSphere(int startEntityId, ref Vector3 origin,
            float radius, [MarshalAs(UnmanagedType.LPStr)] string className);

        /// <summary>
        /// 根据模型名查找实体 / Find entity by model name
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="modelName">模型名 / Model name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindEntityByModel(int startEntityId,
            [MarshalAs(UnmanagedType.LPStr)] string modelName);

        /// <summary>
        /// 根据目标名查找实体 / Find entity by target name
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="targetName">目标名 / Target name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindEntityByTarget(int startEntityId,
            [MarshalAs(UnmanagedType.LPStr)] string targetName);

        /// <summary>
        /// 根据目标名称查找实体 / Find entity by target name
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="targetName">目标名称 / Target name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindEntityByTargetName(int startEntityId,
            [MarshalAs(UnmanagedType.LPStr)] string targetName);

        /// <summary>
        /// 根据拥有者查找实体 / Find entity by owner
        /// </summary>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <param name="ownerEntityId">拥有者实体索引 / Owner entity index</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        internal static extern int FindEntityByOwner(int startEntityId, int ownerEntityId);

        /// <summary>
        /// 查找手榴弹实体 / Find grenade entity
        /// </summary>
        /// <param name="playerId">玩家索引 / Player index</param>
        /// <param name="modelName">模型名缓冲区 / Model name buffer</param>
        /// <param name="maxLength">缓冲区最大长度 / Buffer max length</param>
        /// <param name="startEntityId">起始实体索引 / Start entity index</param>
        /// <returns>找到的手榴弹实体索引，未找到返回0 / Found grenade entity index, returns 0 if not found</returns>
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        internal static extern int FindGrenadeEntity(int playerId,
            [MarshalAs(UnmanagedType.LPStr)] System.Text.StringBuilder modelName,
            int maxLength, int startEntityId);
    }
}
