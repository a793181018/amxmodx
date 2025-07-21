// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike C# Usage Examples
//

using System;
using AmxModX.CStrike;

namespace AmxModX.Examples
{
    /// <summary>
    /// Counter-Strike模块使用示例类
    /// Counter-Strike module usage example class
    /// </summary>
    public class CStrikeExample
    {
        // 存储回调ID用于后续注销
        // Store callback IDs for later unregistration
        private static int _buyAttemptCallbackId;
        private static int _buyCallbackId;
        private static int _internalCommandCallbackId;

        /// <summary>
        /// 初始化示例
        /// Initialize example
        /// </summary>
        public static void Initialize()
        {
            Console.WriteLine("正在初始化Counter-Strike桥接层... / Initializing Counter-Strike bridge layer...");
            
            if (CStrikeInterop.Initialize())
            {
                Console.WriteLine("Counter-Strike桥接层初始化成功! / Counter-Strike bridge layer initialized successfully!");
                
                // 注册Forward回调
                // Register Forward callbacks
                RegisterCallbacks();
                
                // 运行示例
                // Run examples
                RunExamples();
            }
            else
            {
                Console.WriteLine("Counter-Strike桥接层初始化失败! / Counter-Strike bridge layer initialization failed!");
            }
        }

        /// <summary>
        /// 注册Forward回调示例
        /// Register Forward callbacks example
        /// </summary>
        private static void RegisterCallbacks()
        {
            Console.WriteLine("\n=== Forward回调注册示例 / Forward Callback Registration Example ===");

            // 注册购买尝试回调
            // Register buy attempt callback
            _buyAttemptCallbackId = CStrikeInterop.ForwardManager.RegisterBuyAttempt(OnBuyAttempt);
            Console.WriteLine($"购买尝试回调已注册，ID: {_buyAttemptCallbackId} / Buy attempt callback registered, ID: {_buyAttemptCallbackId}");

            // 注册购买完成回调
            // Register buy completion callback
            _buyCallbackId = CStrikeInterop.ForwardManager.RegisterBuy(OnBuy);
            Console.WriteLine($"购买完成回调已注册，ID: {_buyCallbackId} / Buy completion callback registered, ID: {_buyCallbackId}");

            // 注册内部命令回调
            // Register internal command callback
            _internalCommandCallbackId = CStrikeInterop.ForwardManager.RegisterInternalCommand(OnInternalCommand);
            Console.WriteLine($"内部命令回调已注册，ID: {_internalCommandCallbackId} / Internal command callback registered, ID: {_internalCommandCallbackId}");
        }

        /// <summary>
        /// 运行所有示例
        /// Run all examples
        /// </summary>
        private static void RunExamples()
        {
            // 玩家管理示例
            // Player management examples
            PlayerManagementExamples();

            // 武器系统示例
            // Weapon system examples
            WeaponSystemExamples();

            // 游戏实体示例
            // Game entity examples
            GameEntityExamples();

            // 地图环境示例
            // Map environment examples
            MapEnvironmentExamples();

            // 特殊功能示例
            // Special features examples
            SpecialFeaturesExamples();

            // 统计分析示例
            // Statistics analysis examples
            StatisticsAnalysisExamples();
        }

        /// <summary>
        /// 玩家管理类示例
        /// Player management category examples
        /// </summary>
        private static void PlayerManagementExamples()
        {
            Console.WriteLine("\n=== 玩家管理类示例 / Player Management Category Examples ===");

            int playerId = 1; // 假设玩家ID为1 / Assume player ID is 1

            // 金钱系统示例 / Money system example
            Console.WriteLine("\n--- 金钱系统 / Money System ---");
            int currentMoney = CStrikeInterop.PlayerManager.GetMoney(playerId);
            Console.WriteLine($"玩家 {playerId} 当前金钱: ${currentMoney} / Player {playerId} current money: ${currentMoney}");

            bool moneySet = CStrikeInterop.PlayerManager.SetMoney(playerId, 16000, true);
            Console.WriteLine($"设置玩家金钱为 $16000: {(moneySet ? "成功" : "失败")} / Set player money to $16000: {(moneySet ? "Success" : "Failed")}");

            // 队伍系统示例 / Team system example
            Console.WriteLine("\n--- 队伍系统 / Team System ---");
            var currentTeam = CStrikeInterop.PlayerManager.GetTeam(playerId);
            Console.WriteLine($"玩家 {playerId} 当前队伍: {currentTeam} / Player {playerId} current team: {currentTeam}");

            bool teamSet = CStrikeInterop.PlayerManager.SetTeam(playerId, CStrikeInterop.CsTeam.CounterTerrorist, true);
            Console.WriteLine($"设置玩家队伍为反恐精英: {(teamSet ? "成功" : "失败")} / Set player team to Counter-Terrorist: {(teamSet ? "Success" : "Failed")}");

            // VIP状态示例 / VIP status example
            Console.WriteLine("\n--- VIP状态 / VIP Status ---");
            bool isVip = CStrikeInterop.PlayerManager.IsVip(playerId);
            Console.WriteLine($"玩家 {playerId} VIP状态: {isVip} / Player {playerId} VIP status: {isVip}");

            bool vipSet = CStrikeInterop.PlayerManager.SetVip(playerId, true);
            Console.WriteLine($"设置玩家为VIP: {(vipSet ? "成功" : "失败")} / Set player as VIP: {(vipSet ? "Success" : "Failed")}");

            // 装备系统示例 / Equipment system example
            Console.WriteLine("\n--- 装备系统 / Equipment System ---");
            bool hasC4 = CStrikeInterop.PlayerManager.HasC4(playerId);
            Console.WriteLine($"玩家 {playerId} 是否有C4: {hasC4} / Player {playerId} has C4: {hasC4}");

            bool c4Set = CStrikeInterop.PlayerManager.SetC4(playerId, true, true);
            Console.WriteLine($"给玩家C4: {(c4Set ? "成功" : "失败")} / Give player C4: {(c4Set ? "Success" : "Failed")}");

            bool hasDefuseKit = CStrikeInterop.PlayerManager.HasDefuseKit(playerId);
            Console.WriteLine($"玩家 {playerId} 是否有拆弹器: {hasDefuseKit} / Player {playerId} has defuse kit: {hasDefuseKit}");

            bool defuseKitSet = CStrikeInterop.PlayerManager.SetDefuseKit(playerId, true, true);
            Console.WriteLine($"给玩家拆弹器: {(defuseKitSet ? "成功" : "失败")} / Give player defuse kit: {(defuseKitSet ? "Success" : "Failed")}");

            // 重生示例 / Respawn example
            Console.WriteLine("\n--- 重生系统 / Respawn System ---");
            bool spawned = CStrikeInterop.PlayerManager.Spawn(playerId);
            Console.WriteLine($"重生玩家: {(spawned ? "成功" : "失败")} / Respawn player: {(spawned ? "Success" : "Failed")}");
        }

        /// <summary>
        /// 武器系统类示例
        /// Weapon system category examples
        /// </summary>
        private static void WeaponSystemExamples()
        {
            Console.WriteLine("\n=== 武器系统类示例 / Weapon System Category Examples ===");

            int playerId = 1; // 假设玩家ID为1 / Assume player ID is 1
            int weaponEntity = 100; // 假设武器实体ID为100 / Assume weapon entity ID is 100

            // 武器属性示例 / Weapon properties example
            Console.WriteLine("\n--- 武器属性 / Weapon Properties ---");
            bool isSilenced = CStrikeInterop.WeaponManager.IsSilenced(weaponEntity);
            Console.WriteLine($"武器 {weaponEntity} 是否装有消音器: {isSilenced} / Weapon {weaponEntity} has silencer: {isSilenced}");

            bool silencerSet = CStrikeInterop.WeaponManager.SetSilenced(weaponEntity, true, true);
            Console.WriteLine($"为武器装上消音器: {(silencerSet ? "成功" : "失败")} / Attach silencer to weapon: {(silencerSet ? "Success" : "Failed")}");

            bool isBurstMode = CStrikeInterop.WeaponManager.IsBurstMode(weaponEntity);
            Console.WriteLine($"武器 {weaponEntity} 是否为连发模式: {isBurstMode} / Weapon {weaponEntity} is burst mode: {isBurstMode}");

            // 弹药管理示例 / Ammo management example
            Console.WriteLine("\n--- 弹药管理 / Ammo Management ---");
            int clipAmmo = CStrikeInterop.WeaponManager.GetAmmo(weaponEntity);
            Console.WriteLine($"武器 {weaponEntity} 弹夹弹药: {clipAmmo} / Weapon {weaponEntity} clip ammo: {clipAmmo}");

            bool ammoSet = CStrikeInterop.WeaponManager.SetAmmo(weaponEntity, 30);
            Console.WriteLine($"设置武器弹夹弹药为30: {(ammoSet ? "成功" : "失败")} / Set weapon clip ammo to 30: {(ammoSet ? "Success" : "Failed")}");

            int backpackAmmo = CStrikeInterop.WeaponManager.GetBackpackAmmo(playerId, 1);
            Console.WriteLine($"玩家 {playerId} 背包弹药: {backpackAmmo} / Player {playerId} backpack ammo: {backpackAmmo}");

            // 当前武器信息示例 / Current weapon info example
            Console.WriteLine("\n--- 当前武器信息 / Current Weapon Info ---");
            int currentWeapon = CStrikeInterop.WeaponManager.GetCurrentWeapon(playerId, out int clip, out int ammo);
            Console.WriteLine($"玩家 {playerId} 当前武器: {currentWeapon}, 弹夹: {clip}, 弹药: {ammo} / Player {playerId} current weapon: {currentWeapon}, clip: {clip}, ammo: {ammo}");

            bool hasPrimary = CStrikeInterop.WeaponManager.HasPrimaryWeapon(playerId);
            Console.WriteLine($"玩家 {playerId} 是否有主武器: {hasPrimary} / Player {playerId} has primary weapon: {hasPrimary}");
        }

        /// <summary>
        /// 游戏实体类示例
        /// Game entity category examples
        /// </summary>
        private static void GameEntityExamples()
        {
            Console.WriteLine("\n=== 游戏实体类示例 / Game Entity Category Examples ===");

            // 实体创建示例 / Entity creation example
            Console.WriteLine("\n--- 实体创建 / Entity Creation ---");
            int newEntity = CStrikeInterop.EntityManager.CreateEntity("weapon_ak47");
            Console.WriteLine($"创建AK47实体: {(newEntity > 0 ? $"成功，ID: {newEntity}" : "失败")} / Create AK47 entity: {(newEntity > 0 ? $"Success, ID: {newEntity}" : "Failed")}");

            // 实体查找示例 / Entity finding example
            Console.WriteLine("\n--- 实体查找 / Entity Finding ---");
            int foundEntity = CStrikeInterop.EntityManager.FindEntityByClass(0, "weapon_ak47");
            Console.WriteLine($"查找AK47实体: {(foundEntity > 0 ? $"找到，ID: {foundEntity}" : "未找到")} / Find AK47 entity: {(foundEntity > 0 ? $"Found, ID: {foundEntity}" : "Not found")}");

            int ownedEntity = CStrikeInterop.EntityManager.FindEntityByOwner(0, 1);
            Console.WriteLine($"查找玩家1拥有的实体: {(ownedEntity > 0 ? $"找到，ID: {ownedEntity}" : "未找到")} / Find entity owned by player 1: {(ownedEntity > 0 ? $"Found, ID: {ownedEntity}" : "Not found")}");

            // 实体类名设置示例 / Entity classname setting example
            if (newEntity > 0)
            {
                bool classnameSet = CStrikeInterop.EntityManager.SetEntityClass(newEntity, "weapon_m4a1");
                Console.WriteLine($"将实体类名改为M4A1: {(classnameSet ? "成功" : "失败")} / Change entity classname to M4A1: {(classnameSet ? "Success" : "Failed")}");
            }
        }

        /// <summary>
        /// 地图环境类示例
        /// Map environment category examples
        /// </summary>
        private static void MapEnvironmentExamples()
        {
            Console.WriteLine("\n=== 地图环境类示例 / Map Environment Category Examples ===");

            int playerId = 1; // 假设玩家ID为1 / Assume player ID is 1

            // 注意：这些功能需要在实际游戏环境中测试
            // Note: These functions need to be tested in actual game environment
            Console.WriteLine("注意：地图环境功能需要在实际游戏中测试 / Note: Map environment functions need to be tested in actual game");
            Console.WriteLine($"玩家 {playerId} 的地图环境信息将在游戏中显示 / Player {playerId} map environment info will be shown in game");
        }

        /// <summary>
        /// 特殊功能类示例
        /// Special features category examples
        /// </summary>
        private static void SpecialFeaturesExamples()
        {
            Console.WriteLine("\n=== 特殊功能类示例 / Special Features Category Examples ===");

            // 注意：这些功能需要在实际游戏环境中测试
            // Note: These functions need to be tested in actual game environment
            Console.WriteLine("注意：特殊功能需要在实际游戏中测试 / Note: Special features need to be tested in actual game");
            Console.WriteLine("无刀模式、物品信息等功能将在游戏中生效 / No knives mode, item info and other features will take effect in game");
        }

        /// <summary>
        /// 统计分析类示例
        /// Statistics analysis category examples
        /// </summary>
        private static void StatisticsAnalysisExamples()
        {
            Console.WriteLine("\n=== 统计分析类示例 / Statistics Analysis Category Examples ===");

            int playerId = 1; // 假设玩家ID为1 / Assume player ID is 1

            // 全局统计示例 / Global statistics example
            Console.WriteLine("\n--- 全局统计 / Global Statistics ---");
            int statsCount = CStrikeInterop.StatisticsManager.GetStatsCount();
            Console.WriteLine($"统计条目总数: {statsCount} / Total statistics entries: {statsCount}");

            // 显示前5名玩家统计
            // Display top 5 player statistics
            for (int i = 0; i < Math.Min(5, statsCount); i++)
            {
                var (stats, bodyHits, success) = CStrikeInterop.StatisticsManager.GetGlobalStats(i, out string name, out string authid);
                if (success)
                {
                    Console.WriteLine($"排名 {i + 1}: {name} - 击杀:{stats.Kills} 死亡:{stats.Deaths} K/D:{stats.KillDeathRatio:F2}");
                    Console.WriteLine($"Rank {i + 1}: {name} - Kills:{stats.Kills} Deaths:{stats.Deaths} K/D:{stats.KillDeathRatio:F2}");
                }
            }

            // 玩家统计示例 / Player statistics example
            Console.WriteLine("\n--- 玩家统计 / Player Statistics ---");
            var (playerStats, playerBodyHits, playerSuccess) = CStrikeInterop.StatisticsManager.GetPlayerStats(playerId);
            if (playerSuccess)
            {
                Console.WriteLine($"玩家 {playerId} 总体统计 / Player {playerId} overall statistics:");
                Console.WriteLine($"  击杀: {playerStats.Kills} / Kills: {playerStats.Kills}");
                Console.WriteLine($"  死亡: {playerStats.Deaths} / Deaths: {playerStats.Deaths}");
                Console.WriteLine($"  爆头: {playerStats.Headshots} / Headshots: {playerStats.Headshots}");
                Console.WriteLine($"  射击: {playerStats.Shots} / Shots: {playerStats.Shots}");
                Console.WriteLine($"  命中: {playerStats.Hits} / Hits: {playerStats.Hits}");
                Console.WriteLine($"  伤害: {playerStats.Damage} / Damage: {playerStats.Damage}");
                Console.WriteLine($"  命中率: {playerStats.HitRatio:F1}% / Hit ratio: {playerStats.HitRatio:F1}%");
                Console.WriteLine($"  K/D比率: {playerStats.KillDeathRatio:F2} / K/D ratio: {playerStats.KillDeathRatio:F2}");
                Console.WriteLine($"  爆头率: {playerStats.HeadshotRatio:F1}% / Headshot ratio: {playerStats.HeadshotRatio:F1}%");

                Console.WriteLine("\n  身体部位命中统计 / Body hit statistics:");
                Console.WriteLine($"    头部: {playerBodyHits.HeadHits} / Head: {playerBodyHits.HeadHits}");
                Console.WriteLine($"    胸部: {playerBodyHits.ChestHits} / Chest: {playerBodyHits.ChestHits}");
                Console.WriteLine($"    腹部: {playerBodyHits.StomachHits} / Stomach: {playerBodyHits.StomachHits}");
            }

            // 扩展统计示例 / Extended statistics example
            Console.WriteLine("\n--- 扩展统计 / Extended Statistics ---");
            var (extStats, extBodyHits, objectives, extSuccess) = CStrikeInterop.StatisticsManager.GetPlayerExtendedStats(playerId);
            if (extSuccess)
            {
                Console.WriteLine($"玩家 {playerId} 目标统计 / Player {playerId} objective statistics:");
                Console.WriteLine($"  安装炸弹: {objectives.BombsPlanted} / Bombs planted: {objectives.BombsPlanted}");
                Console.WriteLine($"  成功拆弹: {objectives.BombsDefused} / Bombs defused: {objectives.BombsDefused}");
                Console.WriteLine($"  总拆弹尝试: {objectives.TotalDefusions} / Total defusion attempts: {objectives.TotalDefusions}");
                Console.WriteLine($"  炸弹爆炸: {objectives.BombExplosions} / Bomb explosions: {objectives.BombExplosions}");
                Console.WriteLine($"  拆弹成功率: {objectives.DefuseSuccessRatio:F1}% / Defuse success ratio: {objectives.DefuseSuccessRatio:F1}%");
            }

            // 回合统计示例 / Round statistics example
            Console.WriteLine("\n--- 回合统计 / Round Statistics ---");
            var (roundStats, roundBodyHits, roundSuccess) = CStrikeInterop.StatisticsManager.GetPlayerRoundStats(playerId);
            if (roundSuccess)
            {
                Console.WriteLine($"玩家 {playerId} 本回合统计 / Player {playerId} round statistics:");
                Console.WriteLine($"  回合击杀: {roundStats.Kills} / Round kills: {roundStats.Kills}");
                Console.WriteLine($"  回合死亡: {roundStats.Deaths} / Round deaths: {roundStats.Deaths}");
                Console.WriteLine($"  回合伤害: {roundStats.Damage} / Round damage: {roundStats.Damage}");
            }

            // 武器统计示例 / Weapon statistics example
            Console.WriteLine("\n--- 武器统计 / Weapon Statistics ---");
            int ak47WeaponId = 1; // 假设AK47的武器ID为1 / Assume AK47 weapon ID is 1
            var (weaponStats, weaponBodyHits, weaponSuccess) = CStrikeInterop.StatisticsManager.GetPlayerWeaponStats(playerId, ak47WeaponId);
            if (weaponSuccess)
            {
                Console.WriteLine($"玩家 {playerId} AK47统计 / Player {playerId} AK47 statistics:");
                Console.WriteLine($"  AK47击杀: {weaponStats.Kills} / AK47 kills: {weaponStats.Kills}");
                Console.WriteLine($"  AK47射击: {weaponStats.Shots} / AK47 shots: {weaponStats.Shots}");
                Console.WriteLine($"  AK47命中: {weaponStats.Hits} / AK47 hits: {weaponStats.Hits}");
                Console.WriteLine($"  AK47命中率: {weaponStats.HitRatio:F1}% / AK47 hit ratio: {weaponStats.HitRatio:F1}%");
            }

            // 攻击者统计示例 / Attacker statistics example
            Console.WriteLine("\n--- 攻击者统计 / Attacker Statistics ---");
            var (attackerStats, attackerBodyHits, attackerSuccess) = CStrikeInterop.StatisticsManager.GetPlayerAttackerStats(playerId, 0, out string attackerWeapon);
            if (attackerSuccess)
            {
                Console.WriteLine($"玩家 {playerId} 被攻击统计 / Player {playerId} attacked statistics:");
                Console.WriteLine($"  被击杀次数: {attackerStats.Deaths} / Times killed: {attackerStats.Deaths}");
                Console.WriteLine($"  受到伤害: {attackerStats.Damage} / Damage received: {attackerStats.Damage}");
                Console.WriteLine($"  最后被击武器: {attackerWeapon} / Last weapon killed by: {attackerWeapon}");
            }

            // 自定义武器示例 / Custom weapon example
            Console.WriteLine("\n--- 自定义武器 / Custom Weapon ---");
            int customWeaponId = CStrikeInterop.CustomWeaponManager.AddCustomWeapon("Custom Rifle", false, "weapon_custom_rifle");
            if (customWeaponId > 0)
            {
                Console.WriteLine($"成功添加自定义武器，ID: {customWeaponId} / Successfully added custom weapon, ID: {customWeaponId}");

                // 触发自定义武器事件
                // Trigger custom weapon events
                bool shotTriggered = CStrikeInterop.CustomWeaponManager.TriggerCustomWeaponShot(customWeaponId, playerId);
                Console.WriteLine($"触发自定义武器射击: {(shotTriggered ? "成功" : "失败")} / Trigger custom weapon shot: {(shotTriggered ? "Success" : "Failed")}");

                bool damageTriggered = CStrikeInterop.CustomWeaponManager.TriggerCustomWeaponDamage(customWeaponId, playerId, 2, 50, CStrikeInterop.BodyHit.Head);
                Console.WriteLine($"触发自定义武器伤害: {(damageTriggered ? "成功" : "失败")} / Trigger custom weapon damage: {(damageTriggered ? "Success" : "Failed")}");
            }

            // 武器信息示例 / Weapon information example
            Console.WriteLine("\n--- 武器信息 / Weapon Information ---");
            int maxWeapons = CStrikeInterop.CustomWeaponManager.GetMaxWeapons();
            Console.WriteLine($"最大武器数量: {maxWeapons} / Maximum weapons: {maxWeapons}");

            for (int i = 1; i <= Math.Min(5, maxWeapons); i++)
            {
                string weaponName = CStrikeInterop.CustomWeaponManager.GetWeaponName(i);
                string weaponLogName = CStrikeInterop.CustomWeaponManager.GetWeaponLogName(i);
                bool isMelee = CStrikeInterop.CustomWeaponManager.IsWeaponMelee(i);

                if (!string.IsNullOrEmpty(weaponName))
                {
                    Console.WriteLine($"武器 {i}: {weaponName} ({weaponLogName}) - {(isMelee ? "近战" : "远程")}");
                    Console.WriteLine($"Weapon {i}: {weaponName} ({weaponLogName}) - {(isMelee ? "Melee" : "Ranged")}");
                }
            }

            // 重置统计示例 / Reset statistics example
            Console.WriteLine("\n--- 重置统计 / Reset Statistics ---");
            bool resetSuccess = CStrikeInterop.StatisticsManager.ResetPlayerWeaponStats(playerId);
            Console.WriteLine($"重置玩家 {playerId} 武器统计: {(resetSuccess ? "成功" : "失败")} / Reset player {playerId} weapon stats: {(resetSuccess ? "Success" : "Failed")}");
        }

        #region Forward Callback Implementations / Forward回调实现

        /// <summary>
        /// 购买尝试回调实现
        /// Buy attempt callback implementation
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="item">物品ID / Item ID</param>
        /// <returns>0=允许购买，非0=阻止购买 / 0=allow, non-zero=block</returns>
        private static int OnBuyAttempt(int playerId, int item)
        {
            Console.WriteLine($"[BuyAttempt] 玩家 {playerId} 尝试购买物品 {item} / Player {playerId} attempts to buy item {item}");

            // 示例：阻止购买AWP (假设AWP的物品ID是9)
            // Example: Block AWP purchase (assume AWP item ID is 9)
            if (item == 9)
            {
                Console.WriteLine($"[BuyAttempt] 阻止玩家 {playerId} 购买AWP / Block player {playerId} from buying AWP");
                return 1; // 阻止购买 / Block purchase
            }

            return 0; // 允许购买 / Allow purchase
        }

        /// <summary>
        /// 购买完成回调实现
        /// Buy completion callback implementation
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="item">物品ID / Item ID</param>
        /// <returns>0=正常处理，非0=自定义处理 / 0=normal, non-zero=custom</returns>
        private static int OnBuy(int playerId, int item)
        {
            Console.WriteLine($"[Buy] 玩家 {playerId} 成功购买物品 {item} / Player {playerId} successfully bought item {item}");

            // 示例：购买后给予额外金钱奖励
            // Example: Give extra money reward after purchase
            int currentMoney = CStrikeInterop.PlayerManager.GetMoney(playerId);
            CStrikeInterop.PlayerManager.SetMoney(playerId, currentMoney + 100, true);
            Console.WriteLine($"[Buy] 给予玩家 {playerId} 购买奖励 $100 / Give player {playerId} purchase reward $100");

            return 0; // 正常处理 / Normal processing
        }

        /// <summary>
        /// 内部命令回调实现
        /// Internal command callback implementation
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="command">命令字符串 / Command string</param>
        /// <returns>0=继续执行，非0=阻止执行 / 0=continue, non-zero=block</returns>
        private static int OnInternalCommand(int playerId, string command)
        {
            Console.WriteLine($"[InternalCommand] 玩家 {playerId} 执行内部命令: {command} / Player {playerId} executes internal command: {command}");

            // 示例：记录所有内部命令
            // Example: Log all internal commands
            // 这里可以添加命令过滤逻辑
            // Command filtering logic can be added here

            return 0; // 继续执行 / Continue execution
        }

        #endregion

        /// <summary>
        /// 清理资源
        /// Cleanup resources
        /// </summary>
        public static void Cleanup()
        {
            Console.WriteLine("\n=== 清理资源 / Cleanup Resources ===");

            // 注销回调
            // Unregister callbacks
            if (_buyAttemptCallbackId > 0)
            {
                CStrikeInterop.ForwardManager.UnregisterBuyAttempt(_buyAttemptCallbackId);
                Console.WriteLine($"购买尝试回调已注销 / Buy attempt callback unregistered");
            }

            if (_buyCallbackId > 0)
            {
                CStrikeInterop.ForwardManager.UnregisterBuy(_buyCallbackId);
                Console.WriteLine($"购买完成回调已注销 / Buy completion callback unregistered");
            }

            if (_internalCommandCallbackId > 0)
            {
                CStrikeInterop.ForwardManager.UnregisterInternalCommand(_internalCommandCallbackId);
                Console.WriteLine($"内部命令回调已注销 / Internal command callback unregistered");
            }

            // 关闭桥接层
            // Shutdown bridge layer
            CStrikeInterop.Shutdown();
            Console.WriteLine("Counter-Strike桥接层已关闭 / Counter-Strike bridge layer shutdown");
        }
    }
}
