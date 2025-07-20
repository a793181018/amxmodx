// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

// EntityMessageCvarExample.cs - Entity Management, Message System, and CVars Examples
// 实体管理、消息系统和CVars系统示例

using System;
using System.Collections.Generic;
using AmxModX.Interop;

namespace AmxModX.Examples
{
    /// <summary>
    /// 实体管理、消息系统和CVars系统综合示例 / Entity Management, Message System, and CVars System Examples
    /// 演示实体操作、消息发送和CVar管理功能 / Demonstrates entity operations, message sending, and CVar management
    /// </summary>
    public static class EntityMessageCvarExample
    {
        private static readonly Dictionary<string, int> _registeredCommands = new Dictionary<string, int>();
        private static readonly Dictionary<string, int> _registeredCvars = new Dictionary<string, int>();
        private static readonly Dictionary<int, int> _registeredMessages = new Dictionary<int, int>();

        /// <summary>
        /// 初始化示例系统 / Initialize example system
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("Initializing Entity, Message, and CVar Example...");

            try
            {
                // 初始化AMX Mod X命令系统 / Initialize AMX Mod X command system
                AmxModXCommands.Initialize();

                // 注册实体管理命令 / Register entity management commands
                RegisterEntityCommands();

                // 注册消息系统命令 / Register message system commands
                RegisterMessageCommands();

                // 注册CVar管理命令 / Register CVar management commands
                RegisterCvarCommands();

                // 创建示例CVars / Create example CVars
                CreateExampleCvars();

                Console.WriteLine("Entity, Message, and CVar Example initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize example: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 注册实体管理命令 / Register entity management commands
        /// </summary>
        private static void RegisterEntityCommands()
        {
            // 创建实体命令 / Create entity command
            int createEntityCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_create_entity",
                callback: OnCreateEntityCommand,
                flags: CommandFlags.Admin,
                info: "Create entity - Usage: amx_create_entity <classname>"
            );
            _registeredCommands["amx_create_entity"] = createEntityCmd;

            // 查找实体命令 / Find entity command
            int findEntityCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_find_entity",
                callback: OnFindEntityCommand,
                flags: CommandFlags.Admin,
                info: "Find entity - Usage: amx_find_entity <classname>"
            );
            _registeredCommands["amx_find_entity"] = findEntityCmd;

            // 实体信息命令 / Entity info command
            int entityInfoCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_entity_info",
                callback: OnEntityInfoCommand,
                flags: CommandFlags.Admin,
                info: "Get entity info - Usage: amx_entity_info <entityid>"
            );
            _registeredCommands["amx_entity_info"] = entityInfoCmd;

            // 设置实体属性命令 / Set entity property command
            int setEntityCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_set_entity",
                callback: OnSetEntityCommand,
                flags: CommandFlags.Admin,
                info: "Set entity property - Usage: amx_set_entity <entityid> <property> <value>"
            );
            _registeredCommands["amx_set_entity"] = setEntityCmd;

            Console.WriteLine($"Registered {_registeredCommands.Count} entity management commands");
        }

        /// <summary>
        /// 注册消息系统命令 / Register message system commands
        /// </summary>
        private static void RegisterMessageCommands()
        {
            // 发送HUD消息命令 / Send HUD message command
            int hudMsgCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_hud_message",
                callback: OnHudMessageCommand,
                flags: CommandFlags.Admin,
                info: "Send HUD message - Usage: amx_hud_message <player> <message>"
            );
            _registeredCommands["amx_hud_message"] = hudMsgCmd;

            // 消息阻塞命令 / Message block command
            int blockMsgCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_block_message",
                callback: OnBlockMessageCommand,
                flags: CommandFlags.Admin,
                info: "Block message - Usage: amx_block_message <msgname> <blocktype>"
            );
            _registeredCommands["amx_block_message"] = blockMsgCmd;

            Console.WriteLine("Registered message system commands");
        }

        /// <summary>
        /// 注册CVar管理命令 / Register CVar management commands
        /// </summary>
        private static void RegisterCvarCommands()
        {
            // CVar信息命令 / CVar info command
            int cvarInfoCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_cvar_info",
                callback: OnCvarInfoCommand,
                flags: CommandFlags.Admin,
                info: "Get CVar info - Usage: amx_cvar_info <cvarname>"
            );
            _registeredCommands["amx_cvar_info"] = cvarInfoCmd;

            // 设置CVar命令 / Set CVar command
            int setCvarCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_set_cvar",
                callback: OnSetCvarCommand,
                flags: CommandFlags.Admin,
                info: "Set CVar value - Usage: amx_set_cvar <cvarname> <value>"
            );
            _registeredCommands["amx_set_cvar"] = setCvarCmd;

            // 创建CVar命令 / Create CVar command
            int createCvarCmd = AmxModXCommands.RegisterConsoleCommand(
                command: "amx_create_cvar",
                callback: OnCreateCvarCommand,
                flags: CommandFlags.Admin,
                info: "Create CVar - Usage: amx_create_cvar <name> <value> <description>"
            );
            _registeredCommands["amx_create_cvar"] = createCvarCmd;

            Console.WriteLine("Registered CVar management commands");
        }

        /// <summary>
        /// 创建示例CVars / Create example CVars
        /// </summary>
        private static void CreateExampleCvars()
        {
            try
            {
                // 创建示例CVar / Create example CVar
                bool success = AmxModXCommands.CreateCvar(
                    name: "amx_example_enabled",
                    value: "1",
                    flags: CvarFlags.Archive | CvarFlags.Notify,
                    description: "Enable AMX Mod X C# example features",
                    hasMin: true,
                    minValue: 0.0f,
                    hasMax: true,
                    maxValue: 1.0f
                );

                if (success)
                {
                    Console.WriteLine("Created example CVar: amx_example_enabled");

                    // 钩子CVar变化 / Hook CVar change
                    int hookId = AmxModXCommands.HookCvarChange("amx_example_enabled", OnExampleCvarChanged);
                    if (hookId > 0)
                    {
                        _registeredCvars["amx_example_enabled"] = hookId;
                        Console.WriteLine("Hooked CVar change for amx_example_enabled");
                    }
                }

                // 创建调试级别CVar / Create debug level CVar
                success = AmxModXCommands.CreateCvar(
                    name: "amx_example_debug",
                    value: "0",
                    flags: CvarFlags.Archive,
                    description: "Debug level for AMX Mod X C# examples (0-3)",
                    hasMin: true,
                    minValue: 0.0f,
                    hasMax: true,
                    maxValue: 3.0f
                );

                if (success)
                {
                    Console.WriteLine("Created debug CVar: amx_example_debug");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating example CVars: {ex.Message}");
            }
        }

        // ========== 命令回调函数 / Command Callback Functions ==========

        /// <summary>
        /// 创建实体命令回调 / Create entity command callback
        /// </summary>
        private static void OnCreateEntityCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[CREATE ENTITY] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 2)
                {
                    Console.WriteLine("[CREATE ENTITY] Usage: amx_create_entity <classname>");
                    return;
                }

                string className = AmxModXCommands.GetCommandArg(1);
                int entityId = AmxModXCommands.CreateEntity(className);

                if (entityId > 0)
                {
                    Console.WriteLine($"[CREATE ENTITY] Created entity '{className}' with ID {entityId}");

                    // 设置一些基本属性 / Set some basic properties
                    AmxModXCommands.SetEntityString(entityId, EntityProperty.TargetName, $"csharp_entity_{entityId}");
                    AmxModXCommands.SetEntityFloat(entityId, EntityProperty.Health, 100.0f);

                    Console.WriteLine($"[CREATE ENTITY] Set properties for entity {entityId}");
                }
                else
                {
                    Console.WriteLine($"[CREATE ENTITY] Failed to create entity '{className}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CREATE ENTITY] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 查找实体命令回调 / Find entity command callback
        /// </summary>
        private static void OnFindEntityCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[FIND ENTITY] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 2)
                {
                    Console.WriteLine("[FIND ENTITY] Usage: amx_find_entity <classname>");
                    return;
                }

                string className = AmxModXCommands.GetCommandArg(1);
                int entityId = 0;
                int count = 0;

                Console.WriteLine($"[FIND ENTITY] Searching for entities with class '{className}':");

                // 查找所有匹配的实体 / Find all matching entities
                while ((entityId = AmxModXCommands.FindEntityByClassName(entityId, className)) > 0)
                {
                    count++;
                    Console.WriteLine($"[FIND ENTITY]   Entity {entityId}: {className}");

                    // 获取实体的一些属性 / Get some entity properties
                    string targetName = AmxModXCommands.GetEntityString(entityId, EntityProperty.TargetName);
                    float health = AmxModXCommands.GetEntityFloat(entityId, EntityProperty.Health);
                    
                    if (!string.IsNullOrEmpty(targetName))
                        Console.WriteLine($"[FIND ENTITY]     Target Name: {targetName}");
                    
                    if (health > 0)
                        Console.WriteLine($"[FIND ENTITY]     Health: {health}");
                }

                Console.WriteLine($"[FIND ENTITY] Found {count} entities with class '{className}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FIND ENTITY] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 实体信息命令回调 / Entity info command callback
        /// </summary>
        private static void OnEntityInfoCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[ENTITY INFO] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 2)
                {
                    Console.WriteLine("[ENTITY INFO] Usage: amx_entity_info <entityid>");
                    return;
                }

                int entityId = AmxModXCommands.GetCommandArgInt(1);
                if (entityId <= 0)
                {
                    Console.WriteLine("[ENTITY INFO] Invalid entity ID");
                    return;
                }

                Console.WriteLine($"[ENTITY INFO] Information for entity {entityId}:");

                // 获取实体属性 / Get entity properties
                string className = AmxModXCommands.GetEntityString(entityId, EntityProperty.ClassName);
                string targetName = AmxModXCommands.GetEntityString(entityId, EntityProperty.TargetName);
                string model = AmxModXCommands.GetEntityString(entityId, EntityProperty.Model);
                
                float health = AmxModXCommands.GetEntityFloat(entityId, EntityProperty.Health);
                int flags = AmxModXCommands.GetEntityInt(entityId, EntityProperty.Flags);
                int solid = AmxModXCommands.GetEntityInt(entityId, EntityProperty.Solid);

                float[] origin = new float[3];
                bool hasOrigin = AmxModXCommands.GetEntityVector(entityId, EntityProperty.Origin, origin);

                Console.WriteLine($"[ENTITY INFO]   Class Name: {className}");
                Console.WriteLine($"[ENTITY INFO]   Target Name: {targetName}");
                Console.WriteLine($"[ENTITY INFO]   Model: {model}");
                Console.WriteLine($"[ENTITY INFO]   Health: {health}");
                Console.WriteLine($"[ENTITY INFO]   Flags: {flags}");
                Console.WriteLine($"[ENTITY INFO]   Solid: {solid}");

                if (hasOrigin)
                {
                    Console.WriteLine($"[ENTITY INFO]   Origin: ({origin[0]:F2}, {origin[1]:F2}, {origin[2]:F2})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ENTITY INFO] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置实体属性命令回调 / Set entity property command callback
        /// </summary>
        private static void OnSetEntityCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[SET ENTITY] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 4)
                {
                    Console.WriteLine("[SET ENTITY] Usage: amx_set_entity <entityid> <property> <value>");
                    return;
                }

                int entityId = AmxModXCommands.GetCommandArgInt(1);
                string property = AmxModXCommands.GetCommandArg(2);
                string value = AmxModXCommands.GetCommandArg(3);

                bool success = false;

                // 根据属性类型设置值 / Set value based on property type
                if (property == EntityProperty.Health || property == EntityProperty.Armor)
                {
                    float floatValue = AmxModXCommands.GetCommandArgFloat(3);
                    success = AmxModXCommands.SetEntityFloat(entityId, property, floatValue);
                }
                else if (property == EntityProperty.Flags || property == EntityProperty.Team)
                {
                    int intValue = AmxModXCommands.GetCommandArgInt(3);
                    success = AmxModXCommands.SetEntityInt(entityId, property, intValue);
                }
                else
                {
                    success = AmxModXCommands.SetEntityString(entityId, property, value);
                }

                if (success)
                {
                    Console.WriteLine($"[SET ENTITY] Set entity {entityId} property '{property}' to '{value}'");
                }
                else
                {
                    Console.WriteLine($"[SET ENTITY] Failed to set entity {entityId} property '{property}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SET ENTITY] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// HUD消息命令回调 / HUD message command callback
        /// </summary>
        private static void OnHudMessageCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[HUD MESSAGE] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 3)
                {
                    Console.WriteLine("[HUD MESSAGE] Usage: amx_hud_message <player> <message>");
                    return;
                }

                string playerTarget = AmxModXCommands.GetCommandArg(1);
                string message = AmxModXCommands.GetCommandArgs().Substring(playerTarget.Length).Trim();

                // 查找目标玩家 / Find target player
                var targetPlayers = AmxModXCommands.FindPlayersByName(playerTarget);
                if (targetPlayers.Count == 0)
                {
                    Console.WriteLine($"[HUD MESSAGE] No players found matching '{playerTarget}'");
                    return;
                }

                foreach (int playerId in targetPlayers)
                {
                    // 发送HUD消息 / Send HUD message
                    SendHudMessage(playerId, message);

                    string playerName = AmxModXCommands.GetPlayerName(playerId);
                    Console.WriteLine($"[HUD MESSAGE] Sent HUD message to {playerName}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HUD MESSAGE] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 消息阻塞命令回调 / Message block command callback
        /// </summary>
        private static void OnBlockMessageCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[BLOCK MESSAGE] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 3)
                {
                    Console.WriteLine("[BLOCK MESSAGE] Usage: amx_block_message <msgname> <blocktype>");
                    Console.WriteLine("[BLOCK MESSAGE] Block types: 0=not blocked, 1=blocked, 2=once");
                    return;
                }

                string msgName = AmxModXCommands.GetCommandArg(1);
                int blockType = AmxModXCommands.GetCommandArgInt(2);

                int msgId = AmxModXCommands.GetUserMessageId(msgName);
                if (msgId == 0)
                {
                    Console.WriteLine($"[BLOCK MESSAGE] Unknown message: {msgName}");
                    return;
                }

                bool success = AmxModXCommands.SetMessageBlock(msgId, blockType);
                if (success)
                {
                    string blockTypeStr = blockType switch
                    {
                        MessageBlock.NotBlocked => "not blocked",
                        MessageBlock.Blocked => "blocked",
                        MessageBlock.Once => "blocked once",
                        _ => "unknown"
                    };
                    Console.WriteLine($"[BLOCK MESSAGE] Set message '{msgName}' to {blockTypeStr}");
                }
                else
                {
                    Console.WriteLine($"[BLOCK MESSAGE] Failed to set block for message '{msgName}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BLOCK MESSAGE] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// CVar信息命令回调 / CVar info command callback
        /// </summary>
        private static void OnCvarInfoCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[CVAR INFO] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 2)
                {
                    Console.WriteLine("[CVAR INFO] Usage: amx_cvar_info <cvarname>");
                    return;
                }

                string cvarName = AmxModXCommands.GetCommandArg(1);

                if (!AmxModXCommands.CvarExists(cvarName))
                {
                    Console.WriteLine($"[CVAR INFO] CVar '{cvarName}' does not exist");
                    return;
                }

                // 获取CVar信息 / Get CVar info
                var cvarInfo = AmxModXCommands.GetCvarInfo(cvarName);
                if (cvarInfo.HasValue)
                {
                    var info = cvarInfo.Value;
                    Console.WriteLine($"[CVAR INFO] Information for CVar '{cvarName}':");
                    Console.WriteLine($"[CVAR INFO]   Value: {info.Value}");
                    Console.WriteLine($"[CVAR INFO]   Default: {info.DefaultValue}");
                    Console.WriteLine($"[CVAR INFO]   Description: {info.Description}");
                    Console.WriteLine($"[CVAR INFO]   Flags: {info.Flags}");
                    Console.WriteLine($"[CVAR INFO]   Float Value: {info.FloatValue}");

                    if (info.HasMin)
                        Console.WriteLine($"[CVAR INFO]   Min Value: {info.MinValue}");
                    if (info.HasMax)
                        Console.WriteLine($"[CVAR INFO]   Max Value: {info.MaxValue}");
                }
                else
                {
                    // 手动获取基本信息 / Manually get basic info
                    string value = AmxModXCommands.GetCvarString(cvarName);
                    int intValue = AmxModXCommands.GetCvarInt(cvarName);
                    float floatValue = AmxModXCommands.GetCvarFloat(cvarName);
                    int flags = AmxModXCommands.GetCvarFlags(cvarName);

                    Console.WriteLine($"[CVAR INFO] Basic information for CVar '{cvarName}':");
                    Console.WriteLine($"[CVAR INFO]   String Value: {value}");
                    Console.WriteLine($"[CVAR INFO]   Int Value: {intValue}");
                    Console.WriteLine($"[CVAR INFO]   Float Value: {floatValue}");
                    Console.WriteLine($"[CVAR INFO]   Flags: {flags}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CVAR INFO] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 设置CVar命令回调 / Set CVar command callback
        /// </summary>
        private static void OnSetCvarCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[SET CVAR] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 3)
                {
                    Console.WriteLine("[SET CVAR] Usage: amx_set_cvar <cvarname> <value>");
                    return;
                }

                string cvarName = AmxModXCommands.GetCommandArg(1);
                string value = AmxModXCommands.GetCommandArg(2);

                if (!AmxModXCommands.CvarExists(cvarName))
                {
                    Console.WriteLine($"[SET CVAR] CVar '{cvarName}' does not exist");
                    return;
                }

                string oldValue = AmxModXCommands.GetCvarString(cvarName);
                bool success = AmxModXCommands.SetCvarString(cvarName, value);

                if (success)
                {
                    Console.WriteLine($"[SET CVAR] Changed CVar '{cvarName}' from '{oldValue}' to '{value}'");
                }
                else
                {
                    Console.WriteLine($"[SET CVAR] Failed to set CVar '{cvarName}' to '{value}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SET CVAR] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建CVar命令回调 / Create CVar command callback
        /// </summary>
        private static void OnCreateCvarCommand(int clientId, int commandId, int flags)
        {
            Console.WriteLine($"[CREATE CVAR] Executed by client {clientId}");

            try
            {
                int argc = AmxModXCommands.GetCommandArgCount();
                if (argc < 4)
                {
                    Console.WriteLine("[CREATE CVAR] Usage: amx_create_cvar <name> <value> <description>");
                    return;
                }

                string name = AmxModXCommands.GetCommandArg(1);
                string value = AmxModXCommands.GetCommandArg(2);
                string description = AmxModXCommands.GetCommandArgs().Split(' ', 4)[3]; // Get remaining args as description

                if (AmxModXCommands.CvarExists(name))
                {
                    Console.WriteLine($"[CREATE CVAR] CVar '{name}' already exists");
                    return;
                }

                bool success = AmxModXCommands.CreateCvar(
                    name: name,
                    value: value,
                    flags: CvarFlags.Archive,
                    description: description,
                    hasMin: false,
                    minValue: 0.0f,
                    hasMax: false,
                    maxValue: 0.0f
                );

                if (success)
                {
                    Console.WriteLine($"[CREATE CVAR] Created CVar '{name}' with value '{value}'");
                    Console.WriteLine($"[CREATE CVAR] Description: {description}");
                }
                else
                {
                    Console.WriteLine($"[CREATE CVAR] Failed to create CVar '{name}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CREATE CVAR] Error: {ex.Message}");
            }
        }

        // ========== CVar回调函数 / CVar Callback Functions ==========

        /// <summary>
        /// 示例CVar变化回调 / Example CVar change callback
        /// </summary>
        private static void OnExampleCvarChanged(string cvarName, string oldValue, string newValue)
        {
            Console.WriteLine($"[CVAR CHANGE] CVar '{cvarName}' changed from '{oldValue}' to '{newValue}'");

            if (cvarName == "amx_example_enabled")
            {
                bool enabled = newValue == "1";
                Console.WriteLine($"[CVAR CHANGE] Example features are now {(enabled ? "enabled" : "disabled")}");
            }
        }

        // ========== 辅助方法 / Helper Methods ==========

        /// <summary>
        /// 发送HUD消息 / Send HUD message
        /// </summary>
        private static void SendHudMessage(int clientId, string message)
        {
            try
            {
                // 获取HUD消息ID / Get HUD message ID
                int hudMsgId = AmxModXCommands.GetUserMessageId("HudMsg");
                if (hudMsgId == 0)
                {
                    Console.WriteLine("[HUD MESSAGE] HudMsg not available, using TextMsg");
                    hudMsgId = AmxModXCommands.GetUserMessageId("TextMsg");
                }

                if (hudMsgId > 0)
                {
                    // 开始消息 / Begin message
                    bool success = AmxModXCommands.MessageBegin(MessageDest.OneClient, hudMsgId, null, clientId);
                    if (success)
                    {
                        // 写入消息内容 / Write message content
                        AmxModXCommands.WriteByte(4); // HUD channel
                        AmxModXCommands.WriteString(message);

                        // 结束消息 / End message
                        AmxModXCommands.MessageEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HUD MESSAGE] Error sending message: {ex.Message}");
            }
        }

        /// <summary>
        /// 运行完整演示 / Run complete demonstration
        /// </summary>
        public static void RunFullDemo()
        {
            Console.WriteLine("\n========== Full Entity, Message, and CVar Demo ==========");

            try
            {
                // 实体管理演示 / Entity management demo
                Console.WriteLine("\n--- Entity Management Demo ---");
                DemoEntityManagement();

                // 消息系统演示 / Message system demo
                Console.WriteLine("\n--- Message System Demo ---");
                DemoMessageSystem();

                // CVar系统演示 / CVar system demo
                Console.WriteLine("\n--- CVar System Demo ---");
                DemoCvarSystem();

                Console.WriteLine("\nFull demonstration completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Demo execution error: {ex.Message}");
            }
        }

        /// <summary>
        /// 实体管理演示 / Entity management demonstration
        /// </summary>
        private static void DemoEntityManagement()
        {
            try
            {
                // 获取实体数量 / Get entity count
                int entityCount = AmxModXCommands.GetEntityCount();
                Console.WriteLine($"[ENTITY DEMO] Total entities: {entityCount}");

                // 查找玩家实体 / Find player entities
                int playerId = AmxModXCommands.FindEntityByClassName(0, "player");
                if (playerId > 0)
                {
                    Console.WriteLine($"[ENTITY DEMO] Found player entity: {playerId}");

                    // 获取玩家属性 / Get player properties
                    float health = AmxModXCommands.GetEntityFloat(playerId, EntityProperty.Health);
                    string targetName = AmxModXCommands.GetEntityString(playerId, EntityProperty.TargetName);

                    Console.WriteLine($"[ENTITY DEMO] Player health: {health}");
                    Console.WriteLine($"[ENTITY DEMO] Player target name: {targetName}");
                }

                // 创建测试实体 / Create test entity
                int testEntity = AmxModXCommands.CreateEntity("info_target");
                if (testEntity > 0)
                {
                    Console.WriteLine($"[ENTITY DEMO] Created test entity: {testEntity}");

                    // 设置属性 / Set properties
                    AmxModXCommands.SetEntityString(testEntity, EntityProperty.TargetName, "csharp_test_entity");
                    AmxModXCommands.SetEntityFloat(testEntity, EntityProperty.Health, 50.0f);

                    // 设置位置 / Set position
                    float[] origin = { 0.0f, 0.0f, 0.0f };
                    AmxModXCommands.SetEntityOrigin(testEntity, origin);

                    Console.WriteLine($"[ENTITY DEMO] Configured test entity {testEntity}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ENTITY DEMO] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 消息系统演示 / Message system demonstration
        /// </summary>
        private static void DemoMessageSystem()
        {
            try
            {
                // 获取消息ID / Get message IDs
                int textMsgId = AmxModXCommands.GetUserMessageId("TextMsg");
                int hudMsgId = AmxModXCommands.GetUserMessageId("HudMsg");

                Console.WriteLine($"[MESSAGE DEMO] TextMsg ID: {textMsgId}");
                Console.WriteLine($"[MESSAGE DEMO] HudMsg ID: {hudMsgId}");

                // 演示消息阻塞 / Demonstrate message blocking
                if (textMsgId > 0)
                {
                    int currentBlock = AmxModXCommands.GetMessageBlock(textMsgId);
                    Console.WriteLine($"[MESSAGE DEMO] TextMsg current block status: {currentBlock}");

                    // 临时阻塞消息 / Temporarily block message
                    AmxModXCommands.SetMessageBlock(textMsgId, MessageBlock.Once);
                    Console.WriteLine($"[MESSAGE DEMO] Set TextMsg to block once");

                    // 恢复消息 / Restore message
                    AmxModXCommands.SetMessageBlock(textMsgId, MessageBlock.NotBlocked);
                    Console.WriteLine($"[MESSAGE DEMO] Restored TextMsg to not blocked");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MESSAGE DEMO] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// CVar系统演示 / CVar system demonstration
        /// </summary>
        private static void DemoCvarSystem()
        {
            try
            {
                // 检查现有CVar / Check existing CVars
                string[] testCvars = { "hostname", "mp_timelimit", "amx_example_enabled" };

                foreach (string cvarName in testCvars)
                {
                    if (AmxModXCommands.CvarExists(cvarName))
                    {
                        string value = AmxModXCommands.GetCvarString(cvarName);
                        int flags = AmxModXCommands.GetCvarFlags(cvarName);

                        Console.WriteLine($"[CVAR DEMO] {cvarName}: '{value}' (flags: {flags})");
                    }
                    else
                    {
                        Console.WriteLine($"[CVAR DEMO] {cvarName}: does not exist");
                    }
                }

                // 创建临时CVar / Create temporary CVar
                string tempCvarName = "amx_temp_demo";
                if (!AmxModXCommands.CvarExists(tempCvarName))
                {
                    bool success = AmxModXCommands.CreateCvar(
                        name: tempCvarName,
                        value: "demo_value",
                        flags: CvarFlags.None,
                        description: "Temporary demo CVar",
                        hasMin: false,
                        minValue: 0.0f,
                        hasMax: false,
                        maxValue: 0.0f
                    );

                    if (success)
                    {
                        Console.WriteLine($"[CVAR DEMO] Created temporary CVar: {tempCvarName}");

                        // 修改值 / Modify value
                        AmxModXCommands.SetCvarString(tempCvarName, "modified_value");
                        string newValue = AmxModXCommands.GetCvarString(tempCvarName);
                        Console.WriteLine($"[CVAR DEMO] Modified {tempCvarName} to: {newValue}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CVAR DEMO] Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 清理资源 / Cleanup resources
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                Console.WriteLine("Cleaning up Entity, Message, and CVar Example...");

                // 注销所有命令 / Unregister all commands
                foreach (var kvp in _registeredCommands)
                {
                    bool success = AmxModXCommands.UnregisterCommand(kvp.Value);
                    Console.WriteLine($"Unregistered command '{kvp.Key}': {(success ? "Success" : "Failed")}");
                }

                // 注销所有CVar钩子 / Unregister all CVar hooks
                foreach (var kvp in _registeredCvars)
                {
                    bool success = AmxModXCommands.UnhookCvarChange(kvp.Value);
                    Console.WriteLine($"Unhooked CVar '{kvp.Key}': {(success ? "Success" : "Failed")}");
                }

                _registeredCommands.Clear();
                _registeredCvars.Clear();
                _registeredMessages.Clear();

                // 清理AMX Mod X命令系统 / Cleanup AMX Mod X command system
                AmxModXCommands.Cleanup();

                Console.WriteLine("Entity, Message, and CVar Example cleaned up successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cleanup error: {ex.Message}");
            }
        }
    }
}
