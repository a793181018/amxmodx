// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Engine Module High-Level C# API
//

using System;
using System.Text;
using static AmxModX.Engine.EngineNativeImports;
using static AmxModX.Engine.EngineDelegates;

namespace AmxModX.Engine
{
    /// <summary>
    /// Engine模块高级API / Engine module high-level API
    /// </summary>
    public static class EngineAPI
    {
        // ========== Entity Lifecycle Management ==========

        /// <summary>
        /// 创建实体 / Create entity
        /// </summary>
        /// <param name="className">实体类名 / Entity class name</param>
        /// <returns>实体索引，失败返回0 / Entity index, returns 0 on failure</returns>
        /// <exception cref="ArgumentNullException">当className为null时抛出 / Thrown when className is null</exception>
        public static int CreateEntity(string className)
        {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException(nameof(className));

            return EngineNativeImports.CreateEntity(className);
        }

        /// <summary>
        /// 移除实体 / Remove entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool RemoveEntity(int entityId)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.RemoveEntity(entityId);
        }

        /// <summary>
        /// 获取实体总数 / Get entity count
        /// </summary>
        /// <returns>实体总数 / Entity count</returns>
        public static int GetEntityCount()
        {
            return EngineNativeImports.GetEntityCount();
        }

        /// <summary>
        /// 检查实体是否有效 / Check if entity is valid
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <returns>是否有效 / Whether valid</returns>
        public static bool IsValidEntity(int entityId)
        {
            return entityId > 0 && EngineNativeImports.IsValidEntity(entityId);
        }

        // ========== Entity Property Read/Write ==========

        /// <summary>
        /// 获取实体浮点属性 / Get entity float property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值 / Property value</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static float GetEntityFloat(int entityId, EntityProperty property)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.GetEntityFloat(entityId, (int)property);
        }

        /// <summary>
        /// 设置实体浮点属性 / Set entity float property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntityFloat(int entityId, EntityProperty property, float value)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntityFloat(entityId, (int)property, value);
        }

        /// <summary>
        /// 获取实体整数属性 / Get entity integer property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>属性值 / Property value</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static int GetEntityInt(int entityId, EntityProperty property)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.GetEntityInt(entityId, (int)property);
        }

        /// <summary>
        /// 设置实体整数属性 / Set entity integer property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">属性值 / Property value</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntityInt(int entityId, EntityProperty property, int value)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntityInt(entityId, (int)property, value);
        }

        /// <summary>
        /// 获取实体向量属性 / Get entity vector property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>向量值 / Vector value</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static Vector3 GetEntityVector(int entityId, EntityProperty property)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            EngineNativeImports.GetEntityVector(entityId, (int)property, out Vector3 vector);
            return vector;
        }

        /// <summary>
        /// 设置实体向量属性 / Set entity vector property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="vector">向量值 / Vector value</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntityVector(int entityId, EntityProperty property, Vector3 vector)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntityVector(entityId, (int)property, ref vector);
        }

        /// <summary>
        /// 获取实体字符串属性 / Get entity string property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="maxLength">最大长度，默认256 / Max length, default 256</param>
        /// <returns>字符串值 / String value</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static string GetEntityString(int entityId, EntityProperty property, int maxLength = 256)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            var buffer = new StringBuilder(maxLength);
            int actualLength = EngineNativeImports.GetEntityString(entityId, (int)property, buffer, maxLength);
            return actualLength > 0 ? buffer.ToString() : string.Empty;
        }

        /// <summary>
        /// 设置实体字符串属性 / Set entity string property
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="value">字符串值 / String value</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        /// <exception cref="ArgumentNullException">当value为null时抛出 / Thrown when value is null</exception>
        public static bool SetEntityString(int entityId, EntityProperty property, string value)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return EngineNativeImports.SetEntityString(entityId, (int)property, value);
        }

        /// <summary>
        /// 获取实体关联的实体 / Get entity's associated entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <returns>关联实体索引 / Associated entity index</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static int GetEntityEdict(int entityId, EntityProperty property)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.GetEntityEdict(entityId, (int)property);
        }

        /// <summary>
        /// 设置实体关联的实体 / Set entity's associated entity
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="property">属性类型 / Property type</param>
        /// <param name="targetEntityId">目标实体索引 / Target entity index</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntityEdict(int entityId, EntityProperty property, int targetEntityId)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntityEdict(entityId, (int)property, targetEntityId);
        }

        /// <summary>
        /// 设置实体原点 / Set entity origin
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="origin">原点坐标 / Origin coordinates</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntityOrigin(int entityId, Vector3 origin)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntityOrigin(entityId, ref origin);
        }

        /// <summary>
        /// 设置实体模型 / Set entity model
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="modelName">模型名称 / Model name</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        /// <exception cref="ArgumentNullException">当modelName为null时抛出 / Thrown when modelName is null</exception>
        public static bool SetEntityModel(int entityId, string modelName)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");
            if (string.IsNullOrEmpty(modelName))
                throw new ArgumentNullException(nameof(modelName));

            return EngineNativeImports.SetEntityModel(entityId, modelName);
        }

        /// <summary>
        /// 设置实体大小 / Set entity size
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="mins">最小边界 / Minimum bounds</param>
        /// <param name="maxs">最大边界 / Maximum bounds</param>
        /// <returns>是否成功 / Whether successful</returns>
        /// <exception cref="ArgumentOutOfRangeException">当entityId无效时抛出 / Thrown when entityId is invalid</exception>
        public static bool SetEntitySize(int entityId, Vector3 mins, Vector3 maxs)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");

            return EngineNativeImports.SetEntitySize(entityId, ref mins, ref maxs);
        }

        // ========== Entity Search and Finding ==========

        /// <summary>
        /// 获取实体间距离 / Get distance between entities
        /// </summary>
        /// <param name="entityId">实体索引 / Entity index</param>
        /// <param name="targetEntityId">目标实体索引 / Target entity index</param>
        /// <returns>距离值 / Distance value</returns>
        /// <exception cref="ArgumentOutOfRangeException">当实体ID无效时抛出 / Thrown when entity ID is invalid</exception>
        public static float GetEntityRange(int entityId, int targetEntityId)
        {
            if (entityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(entityId), "Entity ID must be greater than 0");
            if (targetEntityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(targetEntityId), "Target entity ID must be greater than 0");

            return EngineNativeImports.GetEntityRange(entityId, targetEntityId);
        }

        /// <summary>
        /// 在球形范围内查找实体 / Find entity in sphere
        /// </summary>
        /// <param name="startEntityId">起始实体索引，0表示从头开始 / Start entity index, 0 means start from beginning</param>
        /// <param name="origin">中心点 / Center point</param>
        /// <param name="radius">半径 / Radius</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        /// <exception cref="ArgumentOutOfRangeException">当radius为负数时抛出 / Thrown when radius is negative</exception>
        public static int FindEntityInSphere(int startEntityId, Vector3 origin, float radius)
        {
            if (radius < 0)
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be non-negative");

            return EngineNativeImports.FindEntityInSphere(startEntityId, ref origin, radius);
        }

        /// <summary>
        /// 根据类名查找实体 / Find entity by class name
        /// </summary>
        /// <param name="startEntityId">起始实体索引，0表示从头开始 / Start entity index, 0 means start from beginning</param>
        /// <param name="className">类名 / Class name</param>
        /// <returns>找到的实体索引，未找到返回0 / Found entity index, returns 0 if not found</returns>
        /// <exception cref="ArgumentNullException">当className为null时抛出 / Thrown when className is null</exception>
        public static int FindEntityByClass(int startEntityId, string className)
        {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException(nameof(className));

            return EngineNativeImports.FindEntityByClass(startEntityId, className);
        }
    }
}
