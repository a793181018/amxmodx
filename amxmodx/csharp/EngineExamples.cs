// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Engine Module C# Usage Examples
//

using System;
using AmxModX.Engine;
using static AmxModX.Engine.EngineAPI;
using static AmxModX.Engine.EngineDelegates;

namespace AmxModX.Examples
{
    /// <summary>
    /// Engine模块使用示例 / Engine module usage examples
    /// </summary>
    public static class EngineExamples
    {
        /// <summary>
        /// 实体生命周期管理示例 / Entity lifecycle management example
        /// </summary>
        public static void EntityLifecycleExample()
        {
            Console.WriteLine("=== 实体生命周期管理示例 / Entity Lifecycle Management Example ===");

            try
            {
                // 创建一个信息目标实体 / Create an info_target entity
                int entityId = CreateEntity("info_target");
                if (entityId > 0)
                {
                    Console.WriteLine($"成功创建实体，ID: {entityId} / Successfully created entity, ID: {entityId}");

                    // 检查实体是否有效 / Check if entity is valid
                    bool isValid = IsValidEntity(entityId);
                    Console.WriteLine($"实体是否有效: {isValid} / Entity is valid: {isValid}");

                    // 获取当前实体总数 / Get current entity count
                    int entityCount = GetEntityCount();
                    Console.WriteLine($"当前实体总数: {entityCount} / Current entity count: {entityCount}");

                    // 移除实体 / Remove entity
                    bool removed = RemoveEntity(entityId);
                    Console.WriteLine($"实体移除结果: {removed} / Entity removal result: {removed}");
                }
                else
                {
                    Console.WriteLine("创建实体失败 / Failed to create entity");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"实体生命周期管理出错: {ex.Message} / Entity lifecycle management error: {ex.Message}");
            }
        }

        /// <summary>
        /// 实体属性操作示例 / Entity property manipulation example
        /// </summary>
        public static void EntityPropertyExample()
        {
            Console.WriteLine("=== 实体属性操作示例 / Entity Property Manipulation Example ===");

            try
            {
                // 创建一个实体用于测试 / Create an entity for testing
                int entityId = CreateEntity("info_target");
                if (entityId <= 0)
                {
                    Console.WriteLine("无法创建测试实体 / Cannot create test entity");
                    return;
                }

                // 设置实体浮点属性 / Set entity float properties
                SetEntityFloat(entityId, EntityProperty.Health, 100.0f);
                SetEntityFloat(entityId, EntityProperty.MaxHealth, 100.0f);
                SetEntityFloat(entityId, EntityProperty.Speed, 250.0f);

                // 读取实体浮点属性 / Read entity float properties
                float health = GetEntityFloat(entityId, EntityProperty.Health);
                float maxHealth = GetEntityFloat(entityId, EntityProperty.MaxHealth);
                float speed = GetEntityFloat(entityId, EntityProperty.Speed);

                Console.WriteLine($"实体生命值: {health} / Entity health: {health}");
                Console.WriteLine($"实体最大生命值: {maxHealth} / Entity max health: {maxHealth}");
                Console.WriteLine($"实体速度: {speed} / Entity speed: {speed}");

                // 设置实体整数属性 / Set entity integer properties
                SetEntityInt(entityId, EntityProperty.MoveType, 5); // MOVETYPE_FLY
                SetEntityInt(entityId, EntityProperty.Solid, 2);    // SOLID_BBOX
                SetEntityInt(entityId, EntityProperty.Effects, 32); // EF_NODRAW

                // 读取实体整数属性 / Read entity integer properties
                int moveType = GetEntityInt(entityId, EntityProperty.MoveType);
                int solid = GetEntityInt(entityId, EntityProperty.Solid);
                int effects = GetEntityInt(entityId, EntityProperty.Effects);

                Console.WriteLine($"实体移动类型: {moveType} / Entity move type: {moveType}");
                Console.WriteLine($"实体碰撞类型: {solid} / Entity solid type: {solid}");
                Console.WriteLine($"实体效果: {effects} / Entity effects: {effects}");

                // 设置实体向量属性 / Set entity vector properties
                var origin = new Vector3(100.0f, 200.0f, 300.0f);
                var angles = new Vector3(0.0f, 90.0f, 0.0f);
                var velocity = new Vector3(50.0f, 0.0f, 0.0f);

                SetEntityVector(entityId, EntityProperty.Origin, origin);
                SetEntityVector(entityId, EntityProperty.Angles, angles);
                SetEntityVector(entityId, EntityProperty.Velocity, velocity);

                // 读取实体向量属性 / Read entity vector properties
                Vector3 currentOrigin = GetEntityVector(entityId, EntityProperty.Origin);
                Vector3 currentAngles = GetEntityVector(entityId, EntityProperty.Angles);
                Vector3 currentVelocity = GetEntityVector(entityId, EntityProperty.Velocity);

                Console.WriteLine($"实体位置: ({currentOrigin.X}, {currentOrigin.Y}, {currentOrigin.Z}) / Entity origin: ({currentOrigin.X}, {currentOrigin.Y}, {currentOrigin.Z})");
                Console.WriteLine($"实体角度: ({currentAngles.X}, {currentAngles.Y}, {currentAngles.Z}) / Entity angles: ({currentAngles.X}, {currentAngles.Y}, {currentAngles.Z})");
                Console.WriteLine($"实体速度: ({currentVelocity.X}, {currentVelocity.Y}, {currentVelocity.Z}) / Entity velocity: ({currentVelocity.X}, {currentVelocity.Y}, {currentVelocity.Z})");

                // 设置实体模型和大小 / Set entity model and size
                SetEntityModel(entityId, "models/player.mdl");
                var mins = new Vector3(-16.0f, -16.0f, -36.0f);
                var maxs = new Vector3(16.0f, 16.0f, 36.0f);
                SetEntitySize(entityId, mins, maxs);

                Console.WriteLine("实体模型和大小设置完成 / Entity model and size set");

                // 清理测试实体 / Clean up test entity
                RemoveEntity(entityId);
                Console.WriteLine("测试实体已清理 / Test entity cleaned up");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"实体属性操作出错: {ex.Message} / Entity property manipulation error: {ex.Message}");
            }
        }

        /// <summary>
        /// 实体查找示例 / Entity finding example
        /// </summary>
        public static void EntityFindingExample()
        {
            Console.WriteLine("=== 实体查找示例 / Entity Finding Example ===");

            try
            {
                // 创建几个测试实体 / Create several test entities
                int entity1 = CreateEntity("info_target");
                int entity2 = CreateEntity("info_target");
                int entity3 = CreateEntity("func_wall");

                if (entity1 > 0 && entity2 > 0 && entity3 > 0)
                {
                    // 设置实体位置 / Set entity positions
                    SetEntityOrigin(entity1, new Vector3(0.0f, 0.0f, 0.0f));
                    SetEntityOrigin(entity2, new Vector3(50.0f, 0.0f, 0.0f));
                    SetEntityOrigin(entity3, new Vector3(100.0f, 0.0f, 0.0f));

                    Console.WriteLine($"创建了测试实体: {entity1}, {entity2}, {entity3} / Created test entities: {entity1}, {entity2}, {entity3}");

                    // 根据类名查找实体 / Find entities by class name
                    int foundEntity = 0;
                    int searchStart = 0;
                    Console.WriteLine("查找所有 info_target 实体: / Finding all info_target entities:");

                    while ((foundEntity = FindEntityByClass(searchStart, "info_target")) > 0)
                    {
                        Vector3 pos = GetEntityVector(foundEntity, EntityProperty.Origin);
                        Console.WriteLine($"  找到实体 {foundEntity}，位置: ({pos.X}, {pos.Y}, {pos.Z}) / Found entity {foundEntity}, position: ({pos.X}, {pos.Y}, {pos.Z})");
                        searchStart = foundEntity;
                    }

                    // 在球形范围内查找实体 / Find entities in sphere
                    var searchOrigin = new Vector3(25.0f, 0.0f, 0.0f);
                    float searchRadius = 30.0f;
                    searchStart = 0;

                    Console.WriteLine($"在位置 ({searchOrigin.X}, {searchOrigin.Y}, {searchOrigin.Z}) 半径 {searchRadius} 范围内查找实体: / Finding entities in sphere at ({searchOrigin.X}, {searchOrigin.Y}, {searchOrigin.Z}) with radius {searchRadius}:");

                    while ((foundEntity = FindEntityInSphere(searchStart, searchOrigin, searchRadius)) > 0)
                    {
                        Vector3 pos = GetEntityVector(foundEntity, EntityProperty.Origin);
                        float distance = GetEntityRange(foundEntity, entity1); // 计算与第一个实体的距离 / Calculate distance to first entity
                        Console.WriteLine($"  找到实体 {foundEntity}，位置: ({pos.X}, {pos.Y}, {pos.Z})，距离: {distance} / Found entity {foundEntity}, position: ({pos.X}, {pos.Y}, {pos.Z}), distance: {distance}");
                        searchStart = foundEntity;
                    }

                    // 清理测试实体 / Clean up test entities
                    RemoveEntity(entity1);
                    RemoveEntity(entity2);
                    RemoveEntity(entity3);
                    Console.WriteLine("测试实体已清理 / Test entities cleaned up");
                }
                else
                {
                    Console.WriteLine("无法创建足够的测试实体 / Cannot create enough test entities");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"实体查找出错: {ex.Message} / Entity finding error: {ex.Message}");
            }
        }

        /// <summary>
        /// 事件回调注册示例 / Event callback registration example
        /// </summary>
        public static void EventCallbackExample()
        {
            Console.WriteLine("=== 事件回调注册示例 / Event Callback Registration Example ===");

            try
            {
                // 注册实体触碰回调 / Register entity touch callback
                EntityTouchCallback touchCallback = (toucher, touched) =>
                {
                    Console.WriteLine($"实体触碰事件: 触碰者 {toucher} 触碰了 {touched} / Entity touch event: toucher {toucher} touched {touched}");
                    
                    // 获取触碰实体的信息 / Get information about touching entities
                    if (IsValidEntity(toucher) && IsValidEntity(touched))
                    {
                        Vector3 toucherPos = GetEntityVector(toucher, EntityProperty.Origin);
                        Vector3 touchedPos = GetEntityVector(touched, EntityProperty.Origin);
                        float distance = GetEntityRange(toucher, touched);
                        
                        Console.WriteLine($"  触碰者位置: ({toucherPos.X}, {toucherPos.Y}, {toucherPos.Z}) / Toucher position: ({toucherPos.X}, {toucherPos.Y}, {toucherPos.Z})");
                        Console.WriteLine($"  被触碰者位置: ({touchedPos.X}, {touchedPos.Y}, {touchedPos.Z}) / Touched position: ({touchedPos.X}, {touchedPos.Y}, {touchedPos.Z})");
                        Console.WriteLine($"  距离: {distance} / Distance: {distance}");
                    }
                };

                // 注册实体思考回调 / Register entity think callback
                EntityThinkCallback thinkCallback = (entityId) =>
                {
                    Console.WriteLine($"实体思考事件: 实体 {entityId} 正在思考 / Entity think event: entity {entityId} is thinking");
                    
                    // 获取思考实体的信息 / Get information about thinking entity
                    if (IsValidEntity(entityId))
                    {
                        Vector3 pos = GetEntityVector(entityId, EntityProperty.Origin);
                        float health = GetEntityFloat(entityId, EntityProperty.Health);
                        
                        Console.WriteLine($"  实体位置: ({pos.X}, {pos.Y}, {pos.Z}) / Entity position: ({pos.X}, {pos.Y}, {pos.Z})");
                        Console.WriteLine($"  实体生命值: {health} / Entity health: {health}");
                    }
                };

                // 注册玩家脉冲回调 / Register player impulse callback
                PlayerImpulseCallback impulseCallback = (playerId, impulse) =>
                {
                    Console.WriteLine($"玩家脉冲事件: 玩家 {playerId} 触发了脉冲 {impulse} / Player impulse event: player {playerId} triggered impulse {impulse}");
                    
                    // 根据脉冲类型执行不同操作 / Perform different actions based on impulse type
                    switch (impulse)
                    {
                        case 100: // 假设这是一个特殊脉冲 / Assume this is a special impulse
                            Console.WriteLine("  执行特殊操作 / Executing special action");
                            break;
                        case 201: // 假设这是手电筒 / Assume this is flashlight
                            Console.WriteLine("  玩家切换手电筒 / Player toggled flashlight");
                            break;
                        default:
                            Console.WriteLine($"  未知脉冲类型: {impulse} / Unknown impulse type: {impulse}");
                            break;
                    }
                };

                Console.WriteLine("事件回调已注册 / Event callbacks registered");
                Console.WriteLine("注意: 实际的回调注册需要通过相应的Engine API函数完成 / Note: Actual callback registration needs to be done through corresponding Engine API functions");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"事件回调注册出错: {ex.Message} / Event callback registration error: {ex.Message}");
            }
        }

        /// <summary>
        /// 运行所有示例 / Run all examples
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("开始运行Engine模块示例 / Starting Engine module examples");
            Console.WriteLine("========================================");

            EntityLifecycleExample();
            Console.WriteLine();

            EntityPropertyExample();
            Console.WriteLine();

            EntityFindingExample();
            Console.WriteLine();

            EventCallbackExample();
            Console.WriteLine();

            Console.WriteLine("所有示例运行完成 / All examples completed");
        }
    }
}
