// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike C# Interop Layer
//

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AmxModX.CStrike
{
    /// <summary>
    /// Counter-Strike模块的C#互操作层，提供对CS游戏功能的完整访问
    /// C# interop layer for Counter-Strike module, providing complete access to CS game functionality
    /// </summary>
    public static class CStrikeInterop
    {
        #region DLL Import Management / DLL导入管理

        /// <summary>
        /// 原生方法导入类，统一管理所有P/Invoke声明
        /// Native methods import class, centrally managing all P/Invoke declarations
        /// </summary>
        private static class NativeMethods
        {
            private const string DllName = "cstrike_amxx";

            #region Bridge Initialization / 桥接层初始化

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int InitializeCStrikeBridge();

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern void ShutdownCStrikeBridge();

            #endregion

            #region Player Management / 玩家管理

            // Money System / 金钱系统
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserMoney(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserMoney(int playerId, int money, int flash);

            // Deaths System / 死亡系统
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserDeaths(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserDeaths(int playerId, int deaths, int scoreboard);

            // Team and VIP / 队伍和VIP
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserTeam(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserTeam(int playerId, int team, int model);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserVip(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserVip(int playerId, int vip);

            // Equipment / 装备
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserPlant(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserPlant(int playerId, int plant, int draw);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserDefuse(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserDefuse(int playerId, int defuse, int draw);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserNvg(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserNvg(int playerId, int nvg);

            // Armor and Shield / 护甲和盾牌
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserArmor(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserArmor(int playerId, int armor, int armorType);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserShield(int playerId);

            // Player State / 玩家状态
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserDriving(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserStationary(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern float GetUserLastActivity(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserLastActivity(int playerId, float time);

            // Player Model / 玩家模型
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserModel(int playerId, StringBuilder model, int maxLength);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
            public static extern int SetUserModel(int playerId, string model);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int ResetUserModel(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserSubmodel(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserSubmodel(int playerId, int submodel);

            // Zoom and View / 缩放和视角
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserZoom(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserZoom(int playerId, int zoom, int weapon);

            // Statistics / 统计
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserTked(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserTked(int playerId, int tk, int subtract);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserHostageKills(int playerId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserHostageKills(int playerId, int hk);

            // Special Functions / 特殊功能
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int UserSpawn(int playerId);

            #endregion

            #region Weapon System / 武器系统

            // Weapon Properties / 武器属性
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponSilenced(int weaponEntity);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWeaponSilenced(int weaponEntity, int silenced, int drawAnimation);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponBurstMode(int weaponEntity);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWeaponBurstMode(int weaponEntity, int burstMode, int drawAnimation);

            // Weapon Ammo / 武器弹药
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponAmmo(int weaponEntity);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetWeaponAmmo(int weaponEntity, int ammo);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserBackpackAmmo(int playerId, int weapon);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int SetUserBackpackAmmo(int playerId, int weapon, int amount);

            // Weapon Information / 武器信息
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponId(int weaponEntity);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponInfo(int weapon, int infoType);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserWeaponEntity(int playerId, int weapon);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserWeapon(int playerId, out int clip, out int ammo);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserHasPrimary(int playerId);

            #endregion

            #region Game Entity / 游戏实体

            // Entity Management / 实体管理
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
            public static extern int CreateEntity(string classname);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
            public static extern int FindEntityByClass(int startIndex, string classname);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int FindEntityByOwner(int startIndex, int owner);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
            public static extern int SetEntityClass(int entity, string classname);

            #endregion

            #region Forward Callbacks / Forward回调

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int RegisterInternalCommandCallback(InternalCommandDelegate callback);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int RegisterBuyAttemptCallback(BuyAttemptDelegate callback);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int RegisterBuyCallback(BuyDelegate callback);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int UnregisterInternalCommandCallback(int callbackId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int UnregisterBuyAttemptCallback(int callbackId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int UnregisterBuyCallback(int callbackId);

            #endregion

            #region Statistics Analysis / 统计分析

            // Global Statistics / 全局统计
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetStats(int index, [Out] int[] stats, [Out] int[] bodyhits, StringBuilder name, int nameLength, StringBuilder authid, int authidLength);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetStats2(int index, [Out] int[] stats, [Out] int[] bodyhits, StringBuilder name, int nameLength, StringBuilder authid, int authidLength, [Out] int[] objectives);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetStatsNum();

            // Player Statistics / 玩家统计
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserStats(int playerId, [Out] int[] stats, [Out] int[] bodyhits);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserStats2(int playerId, [Out] int[] stats, [Out] int[] bodyhits, [Out] int[] objectives);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserRoundStats(int playerId, [Out] int[] stats, [Out] int[] bodyhits);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserAttackerStats(int playerId, int attackerId, [Out] int[] stats, [Out] int[] bodyhits, StringBuilder weaponName, int weaponNameLength);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserVictimStats(int playerId, int victimId, [Out] int[] stats, [Out] int[] bodyhits, StringBuilder weaponName, int weaponNameLength);

            // Weapon Statistics / 武器统计
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserWeaponStats(int playerId, int weaponId, [Out] int[] stats, [Out] int[] bodyhits);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetUserWeaponRoundStats(int playerId, int weaponId, [Out] int[] stats, [Out] int[] bodyhits);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int ResetUserWeaponStats(int playerId);

            // Custom Weapon Support / 自定义武器支持
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
            public static extern int CustomWeaponAdd(string weaponName, int melee, string logName);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int CustomWeaponDamage(int weapon, int attacker, int victim, int damage, int hitplace);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int CustomWeaponShot(int weapon, int playerId);

            // Weapon Information / 武器信息
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponName(int weaponId, StringBuilder name, int maxLength);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetWeaponLogName(int weaponId, StringBuilder logName, int maxLength);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int IsWeaponMelee(int weaponId);

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetMaxWeapons();

            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetStatsSize();

            // Map Objectives / 地图目标
            [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
            public static extern int GetMapObjectives();

            #endregion
        }

        #endregion

        #region Delegate Definitions / 委托定义

        /// <summary>
        /// 内部命令回调委托
        /// Internal command callback delegate
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="command">命令字符串 / Command string</param>
        /// <returns>0=继续执行，非0=阻止执行 / 0=continue, non-zero=block</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate int InternalCommandDelegate(int playerId, string command);

        /// <summary>
        /// 购买尝试回调委托
        /// Buy attempt callback delegate
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="item">物品ID / Item ID</param>
        /// <returns>0=允许购买，非0=阻止购买 / 0=allow, non-zero=block</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int BuyAttemptDelegate(int playerId, int item);

        /// <summary>
        /// 购买完成回调委托
        /// Buy completion callback delegate
        /// </summary>
        /// <param name="playerId">玩家ID / Player ID</param>
        /// <param name="item">物品ID / Item ID</param>
        /// <returns>0=正常处理，非0=自定义处理 / 0=normal, non-zero=custom</returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int BuyDelegate(int playerId, int item);

        #endregion

        #region Enumerations / 枚举定义

        /// <summary>
        /// CS队伍枚举
        /// CS team enumeration
        /// </summary>
        public enum CsTeam
        {
            /// <summary>未分配队伍 / Unassigned</summary>
            Unassigned = 0,
            /// <summary>恐怖分子 / Terrorist</summary>
            Terrorist = 1,
            /// <summary>反恐精英 / Counter-Terrorist</summary>
            CounterTerrorist = 2,
            /// <summary>观察者 / Spectator</summary>
            Spectator = 3
        }

        /// <summary>
        /// CS护甲类型枚举
        /// CS armor type enumeration
        /// </summary>
        public enum CsArmorType
        {
            /// <summary>无护甲 / No armor</summary>
            None = 0,
            /// <summary>防弹衣 / Kevlar vest</summary>
            Kevlar = 1,
            /// <summary>防弹衣+头盔 / Kevlar + helmet</summary>
            KevlarHelmet = 2
        }

        /// <summary>
        /// 武器信息类型枚举
        /// Weapon info type enumeration
        /// </summary>
        public enum WeaponInfoType
        {
            /// <summary>武器ID / Weapon ID</summary>
            Id = 0,
            /// <summary>价格 / Cost</summary>
            Cost = 1,
            /// <summary>弹夹价格 / Clip cost</summary>
            ClipCost = 2,
            /// <summary>购买弹夹大小 / Buy clip size</summary>
            BuyClipSize = 3,
            /// <summary>武器弹夹大小 / Gun clip size</summary>
            GunClipSize = 4,
            /// <summary>最大弹药数 / Max rounds</summary>
            MaxRounds = 5,
            /// <summary>弹药类型 / Ammo type</summary>
            AmmoType = 6,
            /// <summary>弹药名称 / Ammo name</summary>
            AmmoName = 7
        }

        /// <summary>
        /// 统计数据索引枚举
        /// Statistics data index enumeration
        /// </summary>
        public enum StatsIndex
        {
            /// <summary>击杀数 / Kills</summary>
            Kills = 0,
            /// <summary>死亡数 / Deaths</summary>
            Deaths = 1,
            /// <summary>爆头数 / Headshots</summary>
            Headshots = 2,
            /// <summary>团队击杀数 / Team kills</summary>
            TeamKills = 3,
            /// <summary>射击数 / Shots</summary>
            Shots = 4,
            /// <summary>命中数 / Hits</summary>
            Hits = 5,
            /// <summary>伤害值 / Damage</summary>
            Damage = 6,
            /// <summary>排名 / Rank</summary>
            Rank = 7
        }

        /// <summary>
        /// 身体部位命中枚举
        /// Body hit enumeration
        /// </summary>
        public enum BodyHit
        {
            /// <summary>通用 / Generic</summary>
            Generic = 0,
            /// <summary>头部 / Head</summary>
            Head = 1,
            /// <summary>胸部 / Chest</summary>
            Chest = 2,
            /// <summary>腹部 / Stomach</summary>
            Stomach = 3,
            /// <summary>左臂 / Left arm</summary>
            LeftArm = 4,
            /// <summary>右臂 / Right arm</summary>
            RightArm = 5,
            /// <summary>左腿 / Left leg</summary>
            LeftLeg = 6,
            /// <summary>右腿 / Right leg</summary>
            RightLeg = 7
        }

        /// <summary>
        /// 目标统计索引枚举
        /// Objective statistics index enumeration
        /// </summary>
        public enum ObjectiveIndex
        {
            /// <summary>总拆弹次数 / Total defusions</summary>
            TotalDefusions = 0,
            /// <summary>成功拆弹次数 / Bombs defused</summary>
            BombsDefused = 1,
            /// <summary>安装炸弹次数 / Bombs planted</summary>
            BombsPlanted = 2,
            /// <summary>炸弹爆炸次数 / Bomb explosions</summary>
            BombExplosions = 3
        }

        /// <summary>
        /// 统计数据结构
        /// Statistics data structure
        /// </summary>
        public struct PlayerStats
        {
            /// <summary>击杀数 / Kills</summary>
            public int Kills;
            /// <summary>死亡数 / Deaths</summary>
            public int Deaths;
            /// <summary>爆头数 / Headshots</summary>
            public int Headshots;
            /// <summary>团队击杀数 / Team kills</summary>
            public int TeamKills;
            /// <summary>射击数 / Shots</summary>
            public int Shots;
            /// <summary>命中数 / Hits</summary>
            public int Hits;
            /// <summary>伤害值 / Damage</summary>
            public int Damage;
            /// <summary>排名 / Rank</summary>
            public int Rank;

            /// <summary>
            /// 构造函数
            /// Constructor
            /// </summary>
            /// <param name="stats">统计数据数组 / Statistics data array</param>
            public PlayerStats(int[] stats)
            {
                Kills = stats?[0] ?? 0;
                Deaths = stats?[1] ?? 0;
                Headshots = stats?[2] ?? 0;
                TeamKills = stats?[3] ?? 0;
                Shots = stats?[4] ?? 0;
                Hits = stats?[5] ?? 0;
                Damage = stats?[6] ?? 0;
                Rank = stats?[7] ?? 0;
            }

            /// <summary>
            /// 计算命中率
            /// Calculate hit ratio
            /// </summary>
            public float HitRatio => Shots > 0 ? (float)Hits / Shots * 100.0f : 0.0f;

            /// <summary>
            /// 计算K/D比率
            /// Calculate K/D ratio
            /// </summary>
            public float KillDeathRatio => Deaths > 0 ? (float)Kills / Deaths : Kills;

            /// <summary>
            /// 计算爆头率
            /// Calculate headshot ratio
            /// </summary>
            public float HeadshotRatio => Kills > 0 ? (float)Headshots / Kills * 100.0f : 0.0f;
        }

        /// <summary>
        /// 身体部位命中统计结构
        /// Body hit statistics structure
        /// </summary>
        public struct BodyHitStats
        {
            /// <summary>各部位命中次数数组 / Hit counts for each body part</summary>
            public int[] Hits;

            /// <summary>
            /// 构造函数
            /// Constructor
            /// </summary>
            /// <param name="hits">命中数据数组 / Hit data array</param>
            public BodyHitStats(int[] hits)
            {
                Hits = new int[8];
                if (hits != null && hits.Length >= 8)
                {
                    Array.Copy(hits, Hits, 8);
                }
            }

            /// <summary>获取头部命中次数 / Get head hit count</summary>
            public int HeadHits => Hits[(int)BodyHit.Head];
            /// <summary>获取胸部命中次数 / Get chest hit count</summary>
            public int ChestHits => Hits[(int)BodyHit.Chest];
            /// <summary>获取腹部命中次数 / Get stomach hit count</summary>
            public int StomachHits => Hits[(int)BodyHit.Stomach];
            /// <summary>获取左臂命中次数 / Get left arm hit count</summary>
            public int LeftArmHits => Hits[(int)BodyHit.LeftArm];
            /// <summary>获取右臂命中次数 / Get right arm hit count</summary>
            public int RightArmHits => Hits[(int)BodyHit.RightArm];
            /// <summary>获取左腿命中次数 / Get left leg hit count</summary>
            public int LeftLegHits => Hits[(int)BodyHit.LeftLeg];
            /// <summary>获取右腿命中次数 / Get right leg hit count</summary>
            public int RightLegHits => Hits[(int)BodyHit.RightLeg];
        }

        /// <summary>
        /// 目标统计结构
        /// Objective statistics structure
        /// </summary>
        public struct ObjectiveStats
        {
            /// <summary>总拆弹次数 / Total defusions</summary>
            public int TotalDefusions;
            /// <summary>成功拆弹次数 / Bombs defused</summary>
            public int BombsDefused;
            /// <summary>安装炸弹次数 / Bombs planted</summary>
            public int BombsPlanted;
            /// <summary>炸弹爆炸次数 / Bomb explosions</summary>
            public int BombExplosions;

            /// <summary>
            /// 构造函数
            /// Constructor
            /// </summary>
            /// <param name="objectives">目标数据数组 / Objective data array</param>
            public ObjectiveStats(int[] objectives)
            {
                TotalDefusions = objectives?[0] ?? 0;
                BombsDefused = objectives?[1] ?? 0;
                BombsPlanted = objectives?[2] ?? 0;
                BombExplosions = objectives?[3] ?? 0;
            }

            /// <summary>
            /// 计算拆弹成功率
            /// Calculate defuse success ratio
            /// </summary>
            public float DefuseSuccessRatio => TotalDefusions > 0 ? (float)BombsDefused / TotalDefusions * 100.0f : 0.0f;
        }

        #endregion

        #region High-Level Interface / 高级接口

        /// <summary>
        /// 初始化Counter-Strike桥接层
        /// Initialize Counter-Strike bridge layer
        /// </summary>
        /// <returns>成功返回true，失败返回false / True on success, false on failure</returns>
        public static bool Initialize()
        {
            return NativeMethods.InitializeCStrikeBridge() != 0;
        }

        /// <summary>
        /// 关闭Counter-Strike桥接层
        /// Shutdown Counter-Strike bridge layer
        /// </summary>
        public static void Shutdown()
        {
            NativeMethods.ShutdownCStrikeBridge();
        }

        #endregion

        #region Player Management Interface / 玩家管理接口

        /// <summary>
        /// 玩家管理类，提供所有玩家相关的功能
        /// Player management class providing all player-related functionality
        /// </summary>
        public static class PlayerManager
        {
            /// <summary>
            /// 获取玩家金钱数量
            /// Get player money amount
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>金钱数量 / Money amount</returns>
            public static int GetMoney(int playerId)
            {
                return NativeMethods.GetUserMoney(playerId);
            }

            /// <summary>
            /// 设置玩家金钱数量
            /// Set player money amount
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="money">金钱数量 / Money amount</param>
            /// <param name="flash">是否显示金钱变化动画 / Whether to show money change animation</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetMoney(int playerId, int money, bool flash = true)
            {
                return NativeMethods.SetUserMoney(playerId, money, flash ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取玩家死亡次数
            /// Get player death count
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>死亡次数 / Death count</returns>
            public static int GetDeaths(int playerId)
            {
                return NativeMethods.GetUserDeaths(playerId);
            }

            /// <summary>
            /// 设置玩家死亡次数
            /// Set player death count
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="deaths">死亡次数 / Death count</param>
            /// <param name="updateScoreboard">是否更新记分板 / Whether to update scoreboard</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetDeaths(int playerId, int deaths, bool updateScoreboard = true)
            {
                return NativeMethods.SetUserDeaths(playerId, deaths, updateScoreboard ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取玩家队伍
            /// Get player team
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>队伍枚举 / Team enumeration</returns>
            public static CsTeam GetTeam(int playerId)
            {
                return (CsTeam)NativeMethods.GetUserTeam(playerId);
            }

            /// <summary>
            /// 设置玩家队伍
            /// Set player team
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="team">队伍枚举 / Team enumeration</param>
            /// <param name="updateModel">是否更新模型 / Whether to update model</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetTeam(int playerId, CsTeam team, bool updateModel = false)
            {
                return NativeMethods.SetUserTeam(playerId, (int)team, updateModel ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取玩家VIP状态
            /// Get player VIP status
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>是否为VIP / Whether is VIP</returns>
            public static bool IsVip(int playerId)
            {
                return NativeMethods.GetUserVip(playerId) != 0;
            }

            /// <summary>
            /// 设置玩家VIP状态
            /// Set player VIP status
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="isVip">是否为VIP / Whether is VIP</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetVip(int playerId, bool isVip)
            {
                return NativeMethods.SetUserVip(playerId, isVip ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取玩家是否有C4炸弹
            /// Get whether player has C4 bomb
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>是否有C4 / Whether has C4</returns>
            public static bool HasC4(int playerId)
            {
                return NativeMethods.GetUserPlant(playerId) != 0;
            }

            /// <summary>
            /// 设置玩家C4炸弹状态
            /// Set player C4 bomb status
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="hasC4">是否有C4 / Whether has C4</param>
            /// <param name="showIcon">是否显示图标 / Whether to show icon</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetC4(int playerId, bool hasC4, bool showIcon = true)
            {
                return NativeMethods.SetUserPlant(playerId, hasC4 ? 1 : 0, showIcon ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取玩家是否有拆弹器
            /// Get whether player has defuse kit
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>是否有拆弹器 / Whether has defuse kit</returns>
            public static bool HasDefuseKit(int playerId)
            {
                return NativeMethods.GetUserDefuse(playerId) != 0;
            }

            /// <summary>
            /// 设置玩家拆弹器状态
            /// Set player defuse kit status
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="hasDefuseKit">是否有拆弹器 / Whether has defuse kit</param>
            /// <param name="showIcon">是否显示图标 / Whether to show icon</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetDefuseKit(int playerId, bool hasDefuseKit, bool showIcon = true)
            {
                return NativeMethods.SetUserDefuse(playerId, hasDefuseKit ? 1 : 0, showIcon ? 1 : 0) != 0;
            }

            /// <summary>
            /// 重生玩家
            /// Respawn player
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool Spawn(int playerId)
            {
                return NativeMethods.UserSpawn(playerId) != 0;
            }
        }

        #endregion

        #region Weapon System Interface / 武器系统接口

        /// <summary>
        /// 武器管理类，提供所有武器相关的功能
        /// Weapon management class providing all weapon-related functionality
        /// </summary>
        public static class WeaponManager
        {
            /// <summary>
            /// 获取武器是否装有消音器
            /// Get whether weapon has silencer
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <returns>是否装有消音器 / Whether has silencer</returns>
            public static bool IsSilenced(int weaponEntity)
            {
                return NativeMethods.GetWeaponSilenced(weaponEntity) != 0;
            }

            /// <summary>
            /// 设置武器消音器状态
            /// Set weapon silencer status
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <param name="silenced">是否装有消音器 / Whether has silencer</param>
            /// <param name="playAnimation">是否播放动画 / Whether to play animation</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetSilenced(int weaponEntity, bool silenced, bool playAnimation = true)
            {
                return NativeMethods.SetWeaponSilenced(weaponEntity, silenced ? 1 : 0, playAnimation ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取武器是否为连发模式
            /// Get whether weapon is in burst mode
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <returns>是否为连发模式 / Whether is burst mode</returns>
            public static bool IsBurstMode(int weaponEntity)
            {
                return NativeMethods.GetWeaponBurstMode(weaponEntity) != 0;
            }

            /// <summary>
            /// 设置武器连发模式
            /// Set weapon burst mode
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <param name="burstMode">是否为连发模式 / Whether is burst mode</param>
            /// <param name="playAnimation">是否播放动画 / Whether to play animation</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetBurstMode(int weaponEntity, bool burstMode, bool playAnimation = true)
            {
                return NativeMethods.SetWeaponBurstMode(weaponEntity, burstMode ? 1 : 0, playAnimation ? 1 : 0) != 0;
            }

            /// <summary>
            /// 获取武器弹夹弹药数
            /// Get weapon clip ammo count
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <returns>弹夹弹药数 / Clip ammo count</returns>
            public static int GetAmmo(int weaponEntity)
            {
                return NativeMethods.GetWeaponAmmo(weaponEntity);
            }

            /// <summary>
            /// 设置武器弹夹弹药数
            /// Set weapon clip ammo count
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <param name="ammo">弹夹弹药数 / Clip ammo count</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetAmmo(int weaponEntity, int ammo)
            {
                return NativeMethods.SetWeaponAmmo(weaponEntity, ammo) != 0;
            }

            /// <summary>
            /// 获取玩家背包中指定武器的弹药数
            /// Get player backpack ammo count for specified weapon
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="weapon">武器ID / Weapon ID</param>
            /// <returns>背包弹药数 / Backpack ammo count</returns>
            public static int GetBackpackAmmo(int playerId, int weapon)
            {
                return NativeMethods.GetUserBackpackAmmo(playerId, weapon);
            }

            /// <summary>
            /// 设置玩家背包中指定武器的弹药数
            /// Set player backpack ammo count for specified weapon
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="weapon">武器ID / Weapon ID</param>
            /// <param name="amount">弹药数量 / Ammo amount</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetBackpackAmmo(int playerId, int weapon, int amount)
            {
                return NativeMethods.SetUserBackpackAmmo(playerId, weapon, amount) != 0;
            }

            /// <summary>
            /// 获取武器ID
            /// Get weapon ID
            /// </summary>
            /// <param name="weaponEntity">武器实体ID / Weapon entity ID</param>
            /// <returns>武器ID / Weapon ID</returns>
            public static int GetWeaponId(int weaponEntity)
            {
                return NativeMethods.GetWeaponId(weaponEntity);
            }

            /// <summary>
            /// 获取武器信息
            /// Get weapon information
            /// </summary>
            /// <param name="weapon">武器ID / Weapon ID</param>
            /// <param name="infoType">信息类型 / Information type</param>
            /// <returns>武器信息值 / Weapon information value</returns>
            public static int GetWeaponInfo(int weapon, WeaponInfoType infoType)
            {
                return NativeMethods.GetWeaponInfo(weapon, (int)infoType);
            }

            /// <summary>
            /// 获取玩家当前武器信息
            /// Get player current weapon information
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="clip">弹夹弹药数 / Clip ammo count</param>
            /// <param name="ammo">背包弹药数 / Backpack ammo count</param>
            /// <returns>武器ID / Weapon ID</returns>
            public static int GetCurrentWeapon(int playerId, out int clip, out int ammo)
            {
                return NativeMethods.GetUserWeapon(playerId, out clip, out ammo);
            }

            /// <summary>
            /// 获取玩家是否有主武器
            /// Get whether player has primary weapon
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>是否有主武器 / Whether has primary weapon</returns>
            public static bool HasPrimaryWeapon(int playerId)
            {
                return NativeMethods.GetUserHasPrimary(playerId) != 0;
            }
        }

        #endregion

        #region Game Entity Interface / 游戏实体接口

        /// <summary>
        /// 实体管理类，提供所有实体相关的功能
        /// Entity management class providing all entity-related functionality
        /// </summary>
        public static class EntityManager
        {
            /// <summary>
            /// 创建游戏实体
            /// Create game entity
            /// </summary>
            /// <param name="classname">实体类名 / Entity classname</param>
            /// <returns>实体ID，失败返回0 / Entity ID, 0 on failure</returns>
            public static int CreateEntity(string classname)
            {
                return NativeMethods.CreateEntity(classname);
            }

            /// <summary>
            /// 按类名查找实体
            /// Find entity by classname
            /// </summary>
            /// <param name="startIndex">开始搜索的实体索引 / Start search entity index</param>
            /// <param name="classname">实体类名 / Entity classname</param>
            /// <returns>找到的实体ID，未找到返回0 / Found entity ID, 0 if not found</returns>
            public static int FindEntityByClass(int startIndex, string classname)
            {
                return NativeMethods.FindEntityByClass(startIndex, classname);
            }

            /// <summary>
            /// 按拥有者查找实体
            /// Find entity by owner
            /// </summary>
            /// <param name="startIndex">开始搜索的实体索引 / Start search entity index</param>
            /// <param name="owner">拥有者实体ID / Owner entity ID</param>
            /// <returns>找到的实体ID，未找到返回0 / Found entity ID, 0 if not found</returns>
            public static int FindEntityByOwner(int startIndex, int owner)
            {
                return NativeMethods.FindEntityByOwner(startIndex, owner);
            }

            /// <summary>
            /// 设置实体类名
            /// Set entity classname
            /// </summary>
            /// <param name="entity">实体ID / Entity ID</param>
            /// <param name="classname">新的类名 / New classname</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool SetEntityClass(int entity, string classname)
            {
                return NativeMethods.SetEntityClass(entity, classname) != 0;
            }
        }

        #endregion

        #region Forward Callback Interface / Forward回调接口

        /// <summary>
        /// Forward回调管理类，提供事件回调注册功能
        /// Forward callback management class providing event callback registration functionality
        /// </summary>
        public static class ForwardManager
        {
            /// <summary>
            /// 注册内部命令回调
            /// Register internal command callback
            /// </summary>
            /// <param name="callback">回调委托 / Callback delegate</param>
            /// <returns>回调ID / Callback ID</returns>
            public static int RegisterInternalCommand(InternalCommandDelegate callback)
            {
                return NativeMethods.RegisterInternalCommandCallback(callback);
            }

            /// <summary>
            /// 注册购买尝试回调
            /// Register buy attempt callback
            /// </summary>
            /// <param name="callback">回调委托 / Callback delegate</param>
            /// <returns>回调ID / Callback ID</returns>
            public static int RegisterBuyAttempt(BuyAttemptDelegate callback)
            {
                return NativeMethods.RegisterBuyAttemptCallback(callback);
            }

            /// <summary>
            /// 注册购买完成回调
            /// Register buy completion callback
            /// </summary>
            /// <param name="callback">回调委托 / Callback delegate</param>
            /// <returns>回调ID / Callback ID</returns>
            public static int RegisterBuy(BuyDelegate callback)
            {
                return NativeMethods.RegisterBuyCallback(callback);
            }

            /// <summary>
            /// 注销内部命令回调
            /// Unregister internal command callback
            /// </summary>
            /// <param name="callbackId">回调ID / Callback ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool UnregisterInternalCommand(int callbackId)
            {
                return NativeMethods.UnregisterInternalCommandCallback(callbackId) != 0;
            }

            /// <summary>
            /// 注销购买尝试回调
            /// Unregister buy attempt callback
            /// </summary>
            /// <param name="callbackId">回调ID / Callback ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool UnregisterBuyAttempt(int callbackId)
            {
                return NativeMethods.UnregisterBuyAttemptCallback(callbackId) != 0;
            }

            /// <summary>
            /// 注销购买完成回调
            /// Unregister buy completion callback
            /// </summary>
            /// <param name="callbackId">回调ID / Callback ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool UnregisterBuy(int callbackId)
            {
                return NativeMethods.UnregisterBuyCallback(callbackId) != 0;
            }
        }

        #endregion

        #region Statistics Analysis Interface / 统计分析接口

        /// <summary>
        /// 统计管理类，提供所有统计分析相关的功能
        /// Statistics management class providing all statistics analysis functionality
        /// </summary>
        public static class StatisticsManager
        {
            /// <summary>
            /// 获取全局排名统计
            /// Get global ranking statistics
            /// </summary>
            /// <param name="index">排名索引 / Ranking index</param>
            /// <param name="name">玩家名称 / Player name</param>
            /// <param name="authid">玩家认证ID / Player auth ID</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetGlobalStats(int index, out string name, out string authid)
            {
                var stats = new int[8];
                var bodyhits = new int[8];
                var nameBuffer = new StringBuilder(64);
                var authidBuffer = new StringBuilder(64);

                int result = NativeMethods.GetStats(index, stats, bodyhits, nameBuffer, nameBuffer.Capacity, authidBuffer, authidBuffer.Capacity);

                name = nameBuffer.ToString();
                authid = authidBuffer.ToString();

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取扩展全局排名统计（包含目标统计）
            /// Get extended global ranking statistics (including objective stats)
            /// </summary>
            /// <param name="index">排名索引 / Ranking index</param>
            /// <param name="name">玩家名称 / Player name</param>
            /// <param name="authid">玩家认证ID / Player auth ID</param>
            /// <returns>统计数据、身体命中数据和目标统计 / Statistics, body hit data and objective stats</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, ObjectiveStats objectives, bool success) GetExtendedGlobalStats(int index, out string name, out string authid)
            {
                var stats = new int[8];
                var bodyhits = new int[8];
                var objectives = new int[4];
                var nameBuffer = new StringBuilder(64);
                var authidBuffer = new StringBuilder(64);

                int result = NativeMethods.GetStats2(index, stats, bodyhits, nameBuffer, nameBuffer.Capacity, authidBuffer, authidBuffer.Capacity, objectives);

                name = nameBuffer.ToString();
                authid = authidBuffer.ToString();

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), new ObjectiveStats(objectives), result > 0);
            }

            /// <summary>
            /// 获取统计条目总数
            /// Get total number of statistics entries
            /// </summary>
            /// <returns>统计条目数量 / Number of statistics entries</returns>
            public static int GetStatsCount()
            {
                return NativeMethods.GetStatsNum();
            }

            /// <summary>
            /// 获取玩家总体统计
            /// Get player overall statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerStats(int playerId)
            {
                var stats = new int[8];
                var bodyhits = new int[8];

                int result = NativeMethods.GetUserStats(playerId, stats, bodyhits);

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取玩家扩展统计（包含目标统计）
            /// Get player extended statistics (including objective stats)
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>统计数据、身体命中数据和目标统计 / Statistics, body hit data and objective stats</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, ObjectiveStats objectives, bool success) GetPlayerExtendedStats(int playerId)
            {
                var stats = new int[8];
                var bodyhits = new int[8];
                var objectives = new int[4];

                int result = NativeMethods.GetUserStats2(playerId, stats, bodyhits, objectives);

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), new ObjectiveStats(objectives), result > 0);
            }

            /// <summary>
            /// 获取玩家回合统计
            /// Get player round statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerRoundStats(int playerId)
            {
                var stats = new int[8];
                var bodyhits = new int[8];

                int result = NativeMethods.GetUserRoundStats(playerId, stats, bodyhits);

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取玩家攻击者统计
            /// Get player attacker statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="attackerId">攻击者ID，0表示所有攻击者 / Attacker ID, 0 for all attackers</param>
            /// <param name="weaponName">最后使用的武器名称 / Last used weapon name</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerAttackerStats(int playerId, int attackerId, out string weaponName)
            {
                var stats = new int[8];
                var bodyhits = new int[8];
                var weaponBuffer = new StringBuilder(64);

                int result = NativeMethods.GetUserAttackerStats(playerId, attackerId, stats, bodyhits, weaponBuffer, weaponBuffer.Capacity);

                weaponName = weaponBuffer.ToString();

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取玩家受害者统计
            /// Get player victim statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="victimId">受害者ID，0表示所有受害者 / Victim ID, 0 for all victims</param>
            /// <param name="weaponName">最后使用的武器名称 / Last used weapon name</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerVictimStats(int playerId, int victimId, out string weaponName)
            {
                var stats = new int[8];
                var bodyhits = new int[8];
                var weaponBuffer = new StringBuilder(64);

                int result = NativeMethods.GetUserVictimStats(playerId, victimId, stats, bodyhits, weaponBuffer, weaponBuffer.Capacity);

                weaponName = weaponBuffer.ToString();

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取玩家武器统计
            /// Get player weapon statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="weaponId">武器ID / Weapon ID</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerWeaponStats(int playerId, int weaponId)
            {
                var stats = new int[8];
                var bodyhits = new int[8];

                int result = NativeMethods.GetUserWeaponStats(playerId, weaponId, stats, bodyhits);

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 获取玩家武器回合统计
            /// Get player weapon round statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <param name="weaponId">武器ID / Weapon ID</param>
            /// <returns>统计数据和身体命中数据 / Statistics and body hit data</returns>
            public static (PlayerStats stats, BodyHitStats bodyHits, bool success) GetPlayerWeaponRoundStats(int playerId, int weaponId)
            {
                var stats = new int[8];
                var bodyhits = new int[8];

                int result = NativeMethods.GetUserWeaponRoundStats(playerId, weaponId, stats, bodyhits);

                return (new PlayerStats(stats), new BodyHitStats(bodyhits), result > 0);
            }

            /// <summary>
            /// 重置玩家武器统计
            /// Reset player weapon statistics
            /// </summary>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool ResetPlayerWeaponStats(int playerId)
            {
                return NativeMethods.ResetUserWeaponStats(playerId) != 0;
            }
        }

        #endregion

        #region Custom Weapon Interface / 自定义武器接口

        /// <summary>
        /// 自定义武器管理类，提供自定义武器支持功能
        /// Custom weapon management class providing custom weapon support functionality
        /// </summary>
        public static class CustomWeaponManager
        {
            /// <summary>
            /// 添加自定义武器
            /// Add custom weapon
            /// </summary>
            /// <param name="weaponName">武器显示名称 / Weapon display name</param>
            /// <param name="isMelee">是否为近战武器 / Whether is melee weapon</param>
            /// <param name="logName">武器日志名称 / Weapon log name</param>
            /// <returns>自定义武器ID，失败返回0 / Custom weapon ID, 0 on failure</returns>
            public static int AddCustomWeapon(string weaponName, bool isMelee = false, string logName = "")
            {
                return NativeMethods.CustomWeaponAdd(weaponName, isMelee ? 1 : 0, logName);
            }

            /// <summary>
            /// 触发自定义武器伤害事件
            /// Trigger custom weapon damage event
            /// </summary>
            /// <param name="weaponId">自定义武器ID / Custom weapon ID</param>
            /// <param name="attackerId">攻击者ID / Attacker ID</param>
            /// <param name="victimId">受害者ID / Victim ID</param>
            /// <param name="damage">伤害值 / Damage value</param>
            /// <param name="hitPlace">命中部位 / Hit place</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool TriggerCustomWeaponDamage(int weaponId, int attackerId, int victimId, int damage, BodyHit hitPlace = BodyHit.Generic)
            {
                return NativeMethods.CustomWeaponDamage(weaponId, attackerId, victimId, damage, (int)hitPlace) != 0;
            }

            /// <summary>
            /// 触发自定义武器射击事件
            /// Trigger custom weapon shot event
            /// </summary>
            /// <param name="weaponId">自定义武器ID / Custom weapon ID</param>
            /// <param name="playerId">玩家ID / Player ID</param>
            /// <returns>成功返回true / True on success</returns>
            public static bool TriggerCustomWeaponShot(int weaponId, int playerId)
            {
                return NativeMethods.CustomWeaponShot(weaponId, playerId) != 0;
            }

            /// <summary>
            /// 获取武器名称
            /// Get weapon name
            /// </summary>
            /// <param name="weaponId">武器ID / Weapon ID</param>
            /// <returns>武器名称 / Weapon name</returns>
            public static string GetWeaponName(int weaponId)
            {
                var buffer = new StringBuilder(64);
                if (NativeMethods.GetWeaponName(weaponId, buffer, buffer.Capacity) != 0)
                {
                    return buffer.ToString();
                }
                return string.Empty;
            }

            /// <summary>
            /// 获取武器日志名称
            /// Get weapon log name
            /// </summary>
            /// <param name="weaponId">武器ID / Weapon ID</param>
            /// <returns>武器日志名称 / Weapon log name</returns>
            public static string GetWeaponLogName(int weaponId)
            {
                var buffer = new StringBuilder(64);
                if (NativeMethods.GetWeaponLogName(weaponId, buffer, buffer.Capacity) != 0)
                {
                    return buffer.ToString();
                }
                return string.Empty;
            }

            /// <summary>
            /// 检查武器是否为近战武器
            /// Check if weapon is melee weapon
            /// </summary>
            /// <param name="weaponId">武器ID / Weapon ID</param>
            /// <returns>是否为近战武器 / Whether is melee weapon</returns>
            public static bool IsWeaponMelee(int weaponId)
            {
                return NativeMethods.IsWeaponMelee(weaponId) != 0;
            }

            /// <summary>
            /// 获取最大武器数量
            /// Get maximum weapon count
            /// </summary>
            /// <returns>最大武器数量 / Maximum weapon count</returns>
            public static int GetMaxWeapons()
            {
                return NativeMethods.GetMaxWeapons();
            }

            /// <summary>
            /// 获取统计数据大小
            /// Get statistics data size
            /// </summary>
            /// <returns>统计数据大小 / Statistics data size</returns>
            public static int GetStatsSize()
            {
                return NativeMethods.GetStatsSize();
            }

            /// <summary>
            /// 获取地图目标标志
            /// Get map objectives flags
            /// </summary>
            /// <returns>地图目标标志 / Map objectives flags</returns>
            public static int GetMapObjectives()
            {
                return NativeMethods.GetMapObjectives();
            }
        }

        #endregion
    }
}