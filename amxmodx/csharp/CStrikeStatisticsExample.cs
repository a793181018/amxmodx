// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike Statistics Analysis C# Usage Examples
//

using System;
using AmxModX.CStrike;

namespace AmxModX.Examples
{
    /// <summary>
    /// Counter-Strike统计分析模块使用示例类
    /// Counter-Strike statistics analysis module usage example class
    /// </summary>
    public class CStrikeStatisticsExample
    {
        /// <summary>
        /// 运行所有统计分析示例
        /// Run all statistics analysis examples
        /// </summary>
        public static void RunAllExamples()
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("Counter-Strike Statistics Analysis Examples");
            Console.WriteLine("Counter-Strike 统计分析示例");
            Console.WriteLine("=================================================================");

            // 初始化桥接层
            // Initialize bridge layer
            if (!CStrikeInterop.Initialize())
            {
                Console.WriteLine("错误：无法初始化Counter-Strike桥接层 / Error: Failed to initialize Counter-Strike bridge layer");
                return;
            }

            try
            {
                // 全局统计示例
                // Global statistics examples
                GlobalStatisticsExamples();

                // 玩家统计示例
                // Player statistics examples
                PlayerStatisticsExamples();

                // 武器统计示例
                // Weapon statistics examples
                WeaponStatisticsExamples();

                // 自定义武器示例
                // Custom weapon examples
                CustomWeaponExamples();

                // 高级统计分析示例
                // Advanced statistics analysis examples
                AdvancedAnalysisExamples();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"示例执行过程中发生错误 / Error during example execution: {ex.Message}");
            }
            finally
            {
                // 清理资源
                // Cleanup resources
                CStrikeInterop.Shutdown();
            }
        }

        /// <summary>
        /// 全局统计示例
        /// Global statistics examples
        /// </summary>
        private static void GlobalStatisticsExamples()
        {
            Console.WriteLine("\n=== 全局统计示例 / Global Statistics Examples ===");

            // 获取统计条目总数
            // Get total statistics count
            int totalStats = CStrikeInterop.StatisticsManager.GetStatsCount();
            Console.WriteLine($"服务器统计条目总数: {totalStats} / Total server statistics entries: {totalStats}");

            if (totalStats == 0)
            {
                Console.WriteLine("没有统计数据可显示 / No statistics data to display");
                return;
            }

            // 显示排行榜前10名
            // Display top 10 leaderboard
            Console.WriteLine("\n--- 服务器排行榜 / Server Leaderboard ---");
            for (int i = 0; i < Math.Min(10, totalStats); i++)
            {
                var (stats, bodyHits, success) = CStrikeInterop.StatisticsManager.GetGlobalStats(i, out string name, out string authid);
                
                if (success)
                {
                    Console.WriteLine($"#{i + 1:D2} {name,-20} | K:{stats.Kills,4} D:{stats.Deaths,4} | K/D:{stats.KillDeathRatio,6:F2} | 命中率:{stats.HitRatio,5:F1}% | 爆头率:{stats.HeadshotRatio,5:F1}%");
                    Console.WriteLine($"#{i + 1:D2} {name,-20} | K:{stats.Kills,4} D:{stats.Deaths,4} | K/D:{stats.KillDeathRatio,6:F2} | Hit:{stats.HitRatio,5:F1}% | HS:{stats.HeadshotRatio,5:F1}%");
                }
            }

            // 显示扩展统计信息
            // Display extended statistics
            Console.WriteLine("\n--- 扩展排行榜（包含目标统计） / Extended Leaderboard (with Objectives) ---");
            for (int i = 0; i < Math.Min(5, totalStats); i++)
            {
                var (stats, bodyHits, objectives, success) = CStrikeInterop.StatisticsManager.GetExtendedGlobalStats(i, out string name, out string authid);
                
                if (success)
                {
                    Console.WriteLine($"{name}: 炸弹安装:{objectives.BombsPlanted} 拆弹:{objectives.BombsDefused} 成功率:{objectives.DefuseSuccessRatio:F1}%");
                    Console.WriteLine($"{name}: Planted:{objectives.BombsPlanted} Defused:{objectives.BombsDefused} Success:{objectives.DefuseSuccessRatio:F1}%");
                }
            }
        }

        /// <summary>
        /// 玩家统计示例
        /// Player statistics examples
        /// </summary>
        private static void PlayerStatisticsExamples()
        {
            Console.WriteLine("\n=== 玩家统计示例 / Player Statistics Examples ===");

            int playerId = 1; // 示例玩家ID / Example player ID

            // 获取玩家总体统计
            // Get player overall statistics
            var (stats, bodyHits, success) = CStrikeInterop.StatisticsManager.GetPlayerStats(playerId);
            
            if (success)
            {
                Console.WriteLine($"\n玩家 {playerId} 总体统计 / Player {playerId} Overall Statistics:");
                Console.WriteLine($"  击杀/死亡: {stats.Kills}/{stats.Deaths} (K/D: {stats.KillDeathRatio:F2})");
                Console.WriteLine($"  Kills/Deaths: {stats.Kills}/{stats.Deaths} (K/D: {stats.KillDeathRatio:F2})");
                Console.WriteLine($"  爆头: {stats.Headshots} ({stats.HeadshotRatio:F1}%)");
                Console.WriteLine($"  Headshots: {stats.Headshots} ({stats.HeadshotRatio:F1}%)");
                Console.WriteLine($"  射击/命中: {stats.Shots}/{stats.Hits} (命中率: {stats.HitRatio:F1}%)");
                Console.WriteLine($"  Shots/Hits: {stats.Shots}/{stats.Hits} (Accuracy: {stats.HitRatio:F1}%)");
                Console.WriteLine($"  总伤害: {stats.Damage}");
                Console.WriteLine($"  Total Damage: {stats.Damage}");

                // 身体部位命中统计
                // Body hit statistics
                Console.WriteLine("\n  身体部位命中统计 / Body Hit Statistics:");
                Console.WriteLine($"    头部/Head: {bodyHits.HeadHits,4} | 胸部/Chest: {bodyHits.ChestHits,4} | 腹部/Stomach: {bodyHits.StomachHits,4}");
                Console.WriteLine($"    左臂/L.Arm: {bodyHits.LeftArmHits,3} | 右臂/R.Arm: {bodyHits.RightArmHits,3} | 左腿/L.Leg: {bodyHits.LeftLegHits,3} | 右腿/R.Leg: {bodyHits.RightLegHits,3}");
            }

            // 获取玩家扩展统计
            // Get player extended statistics
            var (extStats, extBodyHits, objectives, extSuccess) = CStrikeInterop.StatisticsManager.GetPlayerExtendedStats(playerId);
            
            if (extSuccess)
            {
                Console.WriteLine($"\n玩家 {playerId} 目标统计 / Player {playerId} Objective Statistics:");
                Console.WriteLine($"  炸弹安装: {objectives.BombsPlanted} | 炸弹爆炸: {objectives.BombExplosions}");
                Console.WriteLine($"  Bombs Planted: {objectives.BombsPlanted} | Bomb Explosions: {objectives.BombExplosions}");
                Console.WriteLine($"  拆弹尝试: {objectives.TotalDefusions} | 成功拆弹: {objectives.BombsDefused}");
                Console.WriteLine($"  Defuse Attempts: {objectives.TotalDefusions} | Successful Defuses: {objectives.BombsDefused}");
                Console.WriteLine($"  拆弹成功率: {objectives.DefuseSuccessRatio:F1}%");
                Console.WriteLine($"  Defuse Success Rate: {objectives.DefuseSuccessRatio:F1}%");
            }

            // 获取玩家回合统计
            // Get player round statistics
            var (roundStats, roundBodyHits, roundSuccess) = CStrikeInterop.StatisticsManager.GetPlayerRoundStats(playerId);
            
            if (roundSuccess)
            {
                Console.WriteLine($"\n玩家 {playerId} 本回合统计 / Player {playerId} Round Statistics:");
                Console.WriteLine($"  回合击杀: {roundStats.Kills} | 回合死亡: {roundStats.Deaths}");
                Console.WriteLine($"  Round Kills: {roundStats.Kills} | Round Deaths: {roundStats.Deaths}");
                Console.WriteLine($"  回合伤害: {roundStats.Damage} | 回合射击: {roundStats.Shots}");
                Console.WriteLine($"  Round Damage: {roundStats.Damage} | Round Shots: {roundStats.Shots}");
            }

            // 获取攻击者统计
            // Get attacker statistics
            var (attackerStats, attackerBodyHits, attackerSuccess) = CStrikeInterop.StatisticsManager.GetPlayerAttackerStats(playerId, 0, out string attackerWeapon);
            
            if (attackerSuccess)
            {
                Console.WriteLine($"\n玩家 {playerId} 被攻击统计 / Player {playerId} Attacked Statistics:");
                Console.WriteLine($"  被击杀次数: {attackerStats.Deaths} | 受到伤害: {attackerStats.Damage}");
                Console.WriteLine($"  Times Killed: {attackerStats.Deaths} | Damage Received: {attackerStats.Damage}");
                Console.WriteLine($"  最后被击武器: {attackerWeapon}");
                Console.WriteLine($"  Last Weapon Killed By: {attackerWeapon}");
            }
        }

        /// <summary>
        /// 武器统计示例
        /// Weapon statistics examples
        /// </summary>
        private static void WeaponStatisticsExamples()
        {
            Console.WriteLine("\n=== 武器统计示例 / Weapon Statistics Examples ===");

            int playerId = 1; // 示例玩家ID / Example player ID
            
            // 常见武器ID（这些值可能需要根据实际游戏调整）
            // Common weapon IDs (these values may need adjustment based on actual game)
            int[] commonWeapons = { 1, 2, 3, 4, 5 }; // AK47, M4A1, AWP, Deagle, Glock等
            string[] weaponNames = { "AK47", "M4A1", "AWP", "Deagle", "Glock" };

            Console.WriteLine($"\n玩家 {playerId} 武器使用统计 / Player {playerId} Weapon Usage Statistics:");
            Console.WriteLine("武器名称    击杀  射击  命中  命中率  伤害");
            Console.WriteLine("Weapon      Kills Shots Hits  Accuracy Damage");
            Console.WriteLine("------------------------------------------------");

            for (int i = 0; i < commonWeapons.Length; i++)
            {
                int weaponId = commonWeapons[i];
                string weaponName = weaponNames[i];
                
                var (weaponStats, weaponBodyHits, weaponSuccess) = CStrikeInterop.StatisticsManager.GetPlayerWeaponStats(playerId, weaponId);
                
                if (weaponSuccess && weaponStats.Shots > 0)
                {
                    Console.WriteLine($"{weaponName,-10} {weaponStats.Kills,5} {weaponStats.Shots,5} {weaponStats.Hits,4} {weaponStats.HitRatio,7:F1}% {weaponStats.Damage,6}");
                }
            }

            // 显示最佳武器
            // Display best weapon
            int bestWeaponId = 0;
            float bestKD = 0;
            string bestWeaponName = "";
            
            for (int i = 0; i < commonWeapons.Length; i++)
            {
                var (weaponStats, _, weaponSuccess) = CStrikeInterop.StatisticsManager.GetPlayerWeaponStats(playerId, commonWeapons[i]);
                
                if (weaponSuccess && weaponStats.KillDeathRatio > bestKD)
                {
                    bestKD = weaponStats.KillDeathRatio;
                    bestWeaponId = commonWeapons[i];
                    bestWeaponName = weaponNames[i];
                }
            }

            if (bestWeaponId > 0)
            {
                Console.WriteLine($"\n最佳武器: {bestWeaponName} (K/D: {bestKD:F2})");
                Console.WriteLine($"Best Weapon: {bestWeaponName} (K/D: {bestKD:F2})");
            }

            // 重置武器统计示例
            // Reset weapon statistics example
            Console.WriteLine($"\n重置玩家 {playerId} 武器统计...");
            Console.WriteLine($"Resetting player {playerId} weapon statistics...");
            bool resetSuccess = CStrikeInterop.StatisticsManager.ResetPlayerWeaponStats(playerId);
            Console.WriteLine($"重置结果: {(resetSuccess ? "成功" : "失败")} / Reset result: {(resetSuccess ? "Success" : "Failed")}");
        }

        /// <summary>
        /// 自定义武器示例
        /// Custom weapon examples
        /// </summary>
        private static void CustomWeaponExamples()
        {
            Console.WriteLine("\n=== 自定义武器示例 / Custom Weapon Examples ===");

            // 添加自定义武器
            // Add custom weapons
            Console.WriteLine("添加自定义武器... / Adding custom weapons...");
            
            int customRifle = CStrikeInterop.CustomWeaponManager.AddCustomWeapon("超级步枪", false, "weapon_super_rifle");
            int customKnife = CStrikeInterop.CustomWeaponManager.AddCustomWeapon("激光刀", true, "weapon_laser_knife");
            int customSniper = CStrikeInterop.CustomWeaponManager.AddCustomWeapon("量子狙击枪", false, "weapon_quantum_sniper");

            Console.WriteLine($"超级步枪 ID: {customRifle} / Super Rifle ID: {customRifle}");
            Console.WriteLine($"激光刀 ID: {customKnife} / Laser Knife ID: {customKnife}");
            Console.WriteLine($"量子狙击枪 ID: {customSniper} / Quantum Sniper ID: {customSniper}");

            // 模拟自定义武器使用
            // Simulate custom weapon usage
            if (customRifle > 0)
            {
                Console.WriteLine("\n模拟超级步枪使用... / Simulating Super Rifle usage...");
                
                // 触发射击事件
                // Trigger shot events
                for (int i = 0; i < 5; i++)
                {
                    bool shotResult = CStrikeInterop.CustomWeaponManager.TriggerCustomWeaponShot(customRifle, 1);
                    Console.WriteLine($"射击 {i + 1}: {(shotResult ? "成功" : "失败")} / Shot {i + 1}: {(shotResult ? "Success" : "Failed")}");
                }

                // 触发伤害事件
                // Trigger damage events
                bool damageResult = CStrikeInterop.CustomWeaponManager.TriggerCustomWeaponDamage(
                    customRifle, 1, 2, 75, CStrikeInterop.BodyHit.Chest);
                Console.WriteLine($"伤害事件: {(damageResult ? "成功" : "失败")} / Damage event: {(damageResult ? "Success" : "Failed")}");

                bool headshotResult = CStrikeInterop.CustomWeaponManager.TriggerCustomWeaponDamage(
                    customRifle, 1, 3, 100, CStrikeInterop.BodyHit.Head);
                Console.WriteLine($"爆头事件: {(headshotResult ? "成功" : "失败")} / Headshot event: {(headshotResult ? "Success" : "Failed")}");
            }

            // 获取武器信息
            // Get weapon information
            Console.WriteLine("\n武器信息查询... / Weapon information query...");
            int maxWeapons = CStrikeInterop.CustomWeaponManager.GetMaxWeapons();
            Console.WriteLine($"最大武器数量: {maxWeapons} / Maximum weapons: {maxWeapons}");

            for (int i = 1; i <= Math.Min(10, maxWeapons); i++)
            {
                string weaponName = CStrikeInterop.CustomWeaponManager.GetWeaponName(i);
                string logName = CStrikeInterop.CustomWeaponManager.GetWeaponLogName(i);
                bool isMelee = CStrikeInterop.CustomWeaponManager.IsWeaponMelee(i);

                if (!string.IsNullOrEmpty(weaponName))
                {
                    Console.WriteLine($"武器 {i}: {weaponName} ({logName}) - {(isMelee ? "近战" : "远程")}");
                    Console.WriteLine($"Weapon {i}: {weaponName} ({logName}) - {(isMelee ? "Melee" : "Ranged")}");
                }
            }
        }

        /// <summary>
        /// 高级统计分析示例
        /// Advanced statistics analysis examples
        /// </summary>
        private static void AdvancedAnalysisExamples()
        {
            Console.WriteLine("\n=== 高级统计分析示例 / Advanced Statistics Analysis Examples ===");

            // 分析服务器整体表现
            // Analyze server overall performance
            Console.WriteLine("\n--- 服务器整体分析 / Server Overall Analysis ---");
            
            int totalPlayers = CStrikeInterop.StatisticsManager.GetStatsCount();
            if (totalPlayers == 0)
            {
                Console.WriteLine("没有足够的数据进行分析 / Not enough data for analysis");
                return;
            }

            int totalKills = 0, totalDeaths = 0, totalShots = 0, totalHits = 0, totalHeadshots = 0;
            int activePlayers = 0;

            for (int i = 0; i < totalPlayers; i++)
            {
                var (stats, _, success) = CStrikeInterop.StatisticsManager.GetGlobalStats(i, out _, out _);
                if (success && stats.Kills > 0)
                {
                    totalKills += stats.Kills;
                    totalDeaths += stats.Deaths;
                    totalShots += stats.Shots;
                    totalHits += stats.Hits;
                    totalHeadshots += stats.Headshots;
                    activePlayers++;
                }
            }

            if (activePlayers > 0)
            {
                float avgKD = totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
                float avgAccuracy = totalShots > 0 ? (float)totalHits / totalShots * 100 : 0;
                float avgHeadshotRate = totalKills > 0 ? (float)totalHeadshots / totalKills * 100 : 0;

                Console.WriteLine($"活跃玩家数: {activePlayers} / Active players: {activePlayers}");
                Console.WriteLine($"总击杀数: {totalKills} / Total kills: {totalKills}");
                Console.WriteLine($"总死亡数: {totalDeaths} / Total deaths: {totalDeaths}");
                Console.WriteLine($"平均K/D: {avgKD:F2} / Average K/D: {avgKD:F2}");
                Console.WriteLine($"平均命中率: {avgAccuracy:F1}% / Average accuracy: {avgAccuracy:F1}%");
                Console.WriteLine($"平均爆头率: {avgHeadshotRate:F1}% / Average headshot rate: {avgHeadshotRate:F1}%");
            }

            // 地图目标分析
            // Map objectives analysis
            Console.WriteLine("\n--- 地图目标分析 / Map Objectives Analysis ---");
            int mapObjectives = CStrikeInterop.CustomWeaponManager.GetMapObjectives();
            Console.WriteLine($"地图目标标志: {mapObjectives} / Map objectives flags: {mapObjectives}");
            
            // 解析目标标志
            // Parse objective flags
            if ((mapObjectives & 1) != 0) Console.WriteLine("- 支持炸弹安装/拆除 / Supports bomb plant/defuse");
            if ((mapObjectives & 2) != 0) Console.WriteLine("- 支持人质救援 / Supports hostage rescue");
            if ((mapObjectives & 4) != 0) Console.WriteLine("- 支持VIP护送 / Supports VIP escort");

            Console.WriteLine("\n统计分析完成 / Statistics analysis completed");
        }
    }
}
