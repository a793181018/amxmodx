// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike C# Bridge Layer
//

#ifndef CSTRIKE_BRIDGE_H
#define CSTRIKE_BRIDGE_H

#include <amxxmodule.h>

// Cross-platform export macros
#ifdef _WIN32
    #define CSTRIKE_EXPORT __declspec(dllexport)
    #define CSTRIKE_CALL __stdcall
#else
    #define CSTRIKE_EXPORT __attribute__((visibility("default")))
    #define CSTRIKE_CALL
#endif

// Forward declarations
extern "C" {

// ============================================================================
// Player Management Category / 玩家管理类
// ============================================================================

// Money System / 金钱系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserMoney(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserMoney(int playerId, int money, int flash);

// Deaths System / 死亡系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDeaths(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserDeaths(int playerId, int deaths, int scoreboard);

// Team and VIP / 队伍和VIP
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserTeam(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserTeam(int playerId, int team, int model);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserVip(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserVip(int playerId, int vip);

// Equipment / 装备
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserPlant(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserPlant(int playerId, int plant, int draw);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDefuse(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserDefuse(int playerId, int defuse, int draw);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserNvg(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserNvg(int playerId, int nvg);

// Armor and Shield / 护甲和盾牌
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserArmor(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserArmor(int playerId, int armor, int armorType);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserShield(int playerId);

// Player State / 玩家状态
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDriving(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStationary(int playerId);
CSTRIKE_EXPORT float CSTRIKE_CALL GetUserLastActivity(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserLastActivity(int playerId, float time);

// Player Model / 玩家模型
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserModel(int playerId, char* model, int maxLength);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserModel(int playerId, const char* model);
CSTRIKE_EXPORT int CSTRIKE_CALL ResetUserModel(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserSubmodel(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserSubmodel(int playerId, int submodel);

// Zoom and View / 缩放和视角
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserZoom(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserZoom(int playerId, int zoom, int weapon);

// Statistics / 统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserTked(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserTked(int playerId, int tk, int subtract);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserHostageKills(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserHostageKills(int playerId, int hk);

// Special Functions / 特殊功能
CSTRIKE_EXPORT int CSTRIKE_CALL UserSpawn(int playerId);

// ============================================================================
// Weapon System Category / 武器系统类
// ============================================================================

// Weapon Properties / 武器属性
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponSilenced(int weaponEntity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponSilenced(int weaponEntity, int silenced, int drawAnimation);
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponBurstMode(int weaponEntity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponBurstMode(int weaponEntity, int burstMode, int drawAnimation);

// Weapon Ammo / 武器弹药
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponAmmo(int weaponEntity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponAmmo(int weaponEntity, int ammo);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserBackpackAmmo(int playerId, int weapon);
CSTRIKE_EXPORT int CSTRIKE_CALL SetUserBackpackAmmo(int playerId, int weapon, int amount);

// Weapon Information / 武器信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponId(int weaponEntity);
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponInfo(int weapon, int infoType);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponEntity(int playerId, int weapon);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeapon(int playerId, int* clip, int* ammo);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserHasPrimary(int playerId);

// ============================================================================
// Game Entity Category / 游戏实体类
// ============================================================================

// Entity Management / 实体管理
CSTRIKE_EXPORT int CSTRIKE_CALL CreateEntity(const char* classname);
CSTRIKE_EXPORT int CSTRIKE_CALL FindEntityByClass(int startIndex, const char* classname);
CSTRIKE_EXPORT int CSTRIKE_CALL FindEntityByOwner(int startIndex, int owner);
CSTRIKE_EXPORT int CSTRIKE_CALL SetEntityClass(int entity, const char* classname);

// Hostage System / 人质系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetHostageId(int index);
CSTRIKE_EXPORT int CSTRIKE_CALL GetHostageFollow(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageFollow(int entity, int followId);
CSTRIKE_EXPORT float CSTRIKE_CALL GetHostageLastUse(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageLastUse(int entity, float time);
CSTRIKE_EXPORT float CSTRIKE_CALL GetHostageNextUse(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageNextUse(int entity, float time);

// C4 Bomb System / C4炸弹系统
CSTRIKE_EXPORT float CSTRIKE_CALL GetC4ExplodeTime(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetC4ExplodeTime(int entity, float time);
CSTRIKE_EXPORT int CSTRIKE_CALL GetC4Defusing(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetC4Defusing(int entity, int defusing);

// Armoury System / 武器库系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetArmouryType(int entity);
CSTRIKE_EXPORT int CSTRIKE_CALL SetArmouryType(int entity, int type);

// Weapon Box / 武器盒
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponBoxItem(int entity, int slot);

// ============================================================================
// Map Environment Category / 地图环境类
// ============================================================================

// Zone Detection / 区域检测
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserBuyZone(int playerId);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserMapZones(int playerId);

// ============================================================================
// Special Features Category / 特殊功能类
// ============================================================================

// No Knives Mode / 无刀模式
CSTRIKE_EXPORT int CSTRIKE_CALL GetNoKnives();
CSTRIKE_EXPORT int CSTRIKE_CALL SetNoKnives(int noKnives);

// Item Information / 物品信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetItemId(const char* name);
CSTRIKE_EXPORT int CSTRIKE_CALL GetItemAlias(int id, char* alias, int maxLength);
CSTRIKE_EXPORT int CSTRIKE_CALL GetTranslatedItemAlias(int id, char* alias, int maxLength);

// ============================================================================
// Forward Callbacks / Forward回调
// ============================================================================

// Callback function types / 回调函数类型
typedef int (*InternalCommandCallback)(int playerId, const char* command);
typedef int (*BuyAttemptCallback)(int playerId, int item);
typedef int (*BuyCallback)(int playerId, int item);

// Forward registration / Forward注册
CSTRIKE_EXPORT int CSTRIKE_CALL RegisterInternalCommandCallback(InternalCommandCallback callback);
CSTRIKE_EXPORT int CSTRIKE_CALL RegisterBuyAttemptCallback(BuyAttemptCallback callback);
CSTRIKE_EXPORT int CSTRIKE_CALL RegisterBuyCallback(BuyCallback callback);

// Forward unregistration / Forward注销
CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterInternalCommandCallback(int callbackId);
CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterBuyAttemptCallback(int callbackId);
CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterBuyCallback(int callbackId);

// ============================================================================
// Statistics Analysis Category / 统计分析类
// ============================================================================

// Global Statistics / 全局统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetStats(int index, int* stats, int* bodyhits, char* name, int nameLength, char* authid, int authidLength);
CSTRIKE_EXPORT int CSTRIKE_CALL GetStats2(int index, int* stats, int* bodyhits, char* name, int nameLength, char* authid, int authidLength, int* objectives);
CSTRIKE_EXPORT int CSTRIKE_CALL GetStatsNum();

// Player Statistics / 玩家统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStats(int playerId, int* stats, int* bodyhits);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStats2(int playerId, int* stats, int* bodyhits, int* objectives);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserRoundStats(int playerId, int* stats, int* bodyhits);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserAttackerStats(int playerId, int attackerId, int* stats, int* bodyhits, char* weaponName, int weaponNameLength);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserVictimStats(int playerId, int victimId, int* stats, int* bodyhits, char* weaponName, int weaponNameLength);

// Weapon Statistics / 武器统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponStats(int playerId, int weaponId, int* stats, int* bodyhits);
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponRoundStats(int playerId, int weaponId, int* stats, int* bodyhits);
CSTRIKE_EXPORT int CSTRIKE_CALL ResetUserWeaponStats(int playerId);

// Custom Weapon Support / 自定义武器支持
CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponAdd(const char* weaponName, int melee, const char* logName);
CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponDamage(int weapon, int attacker, int victim, int damage, int hitplace);
CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponShot(int weapon, int playerId);

// Weapon Information / 武器信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponName(int weaponId, char* name, int maxLength);
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponLogName(int weaponId, char* logName, int maxLength);
CSTRIKE_EXPORT int CSTRIKE_CALL IsWeaponMelee(int weaponId);
CSTRIKE_EXPORT int CSTRIKE_CALL GetMaxWeapons();
CSTRIKE_EXPORT int CSTRIKE_CALL GetStatsSize();

// Map Objectives / 地图目标
CSTRIKE_EXPORT int CSTRIKE_CALL GetMapObjectives();

// ============================================================================
// Bridge Initialization / 桥接层初始化
// ============================================================================

CSTRIKE_EXPORT int CSTRIKE_CALL InitializeCStrikeBridge();
CSTRIKE_EXPORT void CSTRIKE_CALL ShutdownCStrikeBridge();

} // extern "C"

// ============================================================================
// Internal Bridge Management / 内部桥接管理
// ============================================================================

namespace CStrikeBridge {
    // Callback management / 回调管理
    struct CallbackInfo {
        int id;
        void* callback;
        int type; // 0=InternalCommand, 1=BuyAttempt, 2=Buy
    };

    // Thread safety / 线程安全
    void InitializeLocks();
    void CleanupLocks();
    void Lock();
    void Unlock();

    // Callback registry / 回调注册表
    int RegisterCallback(void* callback, int type);
    void UnregisterCallback(int callbackId);
    void* GetCallback(int callbackId);

    // Forward execution / Forward执行
    int ExecuteInternalCommandForward(int playerId, const char* command);
    int ExecuteBuyAttemptForward(int playerId, int item);
    int ExecuteBuyForward(int playerId, int item);

    // Utility functions / 工具函数
    bool IsValidPlayer(int playerId);
    bool IsValidEntity(int entity);
    const char* GetSafeString(const char* str);
    void SafeStringCopy(char* dest, const char* src, int maxLength);
}

#endif // CSTRIKE_BRIDGE_H
