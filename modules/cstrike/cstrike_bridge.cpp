// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike C# Bridge Layer Implementation
//

#include "cstrike_bridge.h"
#include "CstrikeDatas.h"
#include "CstrikePlayer.h"
#include "CstrikeUtils.h"
#include "CstrikeHacks.h"
#include <vector>
#include <mutex>
#include <algorithm>

// ============================================================================
// Global Variables and Thread Safety / 全局变量和线程安全
// ============================================================================

namespace CStrikeBridge {
    static std::vector<CallbackInfo> g_callbacks;
    static int g_nextCallbackId = 1;
    static std::mutex g_callbackMutex;
    static bool g_initialized = false;

    void InitializeLocks() {
        // Mutex is automatically initialized
        g_initialized = true;
    }

    void CleanupLocks() {
        std::lock_guard<std::mutex> lock(g_callbackMutex);
        g_callbacks.clear();
        g_initialized = false;
    }

    void Lock() {
        g_callbackMutex.lock();
    }

    void Unlock() {
        g_callbackMutex.unlock();
    }

    bool IsValidPlayer(int playerId) {
        return playerId >= 1 && playerId <= gpGlobals->maxClients;
    }

    bool IsValidEntity(int entity) {
        return entity > 0 && entity <= gpGlobals->maxEntities;
    }

    const char* GetSafeString(const char* str) {
        return str ? str : "";
    }

    void SafeStringCopy(char* dest, const char* src, int maxLength) {
        if (!dest || maxLength <= 0) return;
        if (!src) {
            dest[0] = '\0';
            return;
        }
        strncpy(dest, src, maxLength - 1);
        dest[maxLength - 1] = '\0';
    }

    int RegisterCallback(void* callback, int type) {
        if (!callback) return 0;
        
        std::lock_guard<std::mutex> lock(g_callbackMutex);
        CallbackInfo info;
        info.id = g_nextCallbackId++;
        info.callback = callback;
        info.type = type;
        g_callbacks.push_back(info);
        return info.id;
    }

    void UnregisterCallback(int callbackId) {
        std::lock_guard<std::mutex> lock(g_callbackMutex);
        g_callbacks.erase(
            std::remove_if(g_callbacks.begin(), g_callbacks.end(),
                [callbackId](const CallbackInfo& info) { return info.id == callbackId; }),
            g_callbacks.end()
        );
    }

    void* GetCallback(int callbackId) {
        std::lock_guard<std::mutex> lock(g_callbackMutex);
        for (const auto& info : g_callbacks) {
            if (info.id == callbackId) {
                return info.callback;
            }
        }
        return nullptr;
    }
}

// ============================================================================
// Bridge Initialization / 桥接层初始化
// ============================================================================

extern "C" {

CSTRIKE_EXPORT int CSTRIKE_CALL InitializeCStrikeBridge() {
    if (CStrikeBridge::g_initialized) {
        return 1; // Already initialized
    }
    
    CStrikeBridge::InitializeLocks();
    return 1;
}

CSTRIKE_EXPORT void CSTRIKE_CALL ShutdownCStrikeBridge() {
    CStrikeBridge::CleanupLocks();
}

// ============================================================================
// Player Management Category Implementation / 玩家管理类实现
// ============================================================================

// Money System / 金钱系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserMoney(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iAccount);
    return get_pdata<int>(pPlayer, m_iAccount);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserMoney(int playerId, int money, int flash) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iAccount);
    set_pdata<int>(pPlayer, m_iAccount, money);
    
    if (flash) {
        MESSAGE_BEGIN(MSG_ONE, MessageIdMoney, nullptr, pPlayer);
            WRITE_LONG(money);
            WRITE_BYTE(flash);
        MESSAGE_END();
    }
    
    return 1;
}

// Deaths System / 死亡系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDeaths(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iDeaths);
    return get_pdata<int>(pPlayer, m_iDeaths);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserDeaths(int playerId, int deaths, int scoreboard) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iDeaths);
    GET_OFFSET("CBasePlayer", m_iTeam);
    
    set_pdata<int>(pPlayer, m_iDeaths, deaths);
    
    if (scoreboard) {
        MESSAGE_BEGIN(MSG_ALL, MessageIdScoreInfo);
            WRITE_BYTE(playerId);
            WRITE_SHORT(static_cast<int>(pPlayer->v.frags));
            WRITE_SHORT(deaths);
            WRITE_SHORT(0);
            WRITE_SHORT(get_pdata<int>(pPlayer, m_iTeam));
        MESSAGE_END();
    }
    
    return 1;
}

// Team and VIP / 队伍和VIP
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserTeam(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iTeam);
    return get_pdata<int>(pPlayer, m_iTeam);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserTeam(int playerId, int team, int model) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    if (team < 0 || team > 3) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_iTeam);
    set_pdata<int>(pPlayer, m_iTeam, team);
    
    if (model) {
        // Set appropriate model based on team
        const char* teamModel = nullptr;
        switch (team) {
            case 1: teamModel = "terror"; break;
            case 2: teamModel = "cstriker"; break;
            default: break;
        }
        
        if (teamModel) {
            SET_CLIENT_KEY_VALUE(playerId, GET_INFO_BUFFER(pPlayer), "model", teamModel);
        }
    }
    
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserVip(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bIsVIP);
    return get_pdata<bool>(pPlayer, m_bIsVIP) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserVip(int playerId, int vip) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bIsVIP);
    set_pdata<bool>(pPlayer, m_bIsVIP, vip != 0);
    
    return 1;
}

// Equipment / 装备
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserPlant(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasC4);
    return get_pdata<bool>(pPlayer, m_bHasC4) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserPlant(int playerId, int plant, int draw) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasC4);
    set_pdata<bool>(pPlayer, m_bHasC4, plant != 0);
    
    if (draw) {
        MESSAGE_BEGIN(MSG_ONE, MessageIdStatusIcon, nullptr, pPlayer);
            WRITE_BYTE(plant ? 1 : 0);
            WRITE_STRING("c4");
            WRITE_BYTE(0);
            WRITE_BYTE(160);
            WRITE_BYTE(0);
        MESSAGE_END();
    }
    
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDefuse(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasDefuser);
    return get_pdata<bool>(pPlayer, m_bHasDefuser) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserDefuse(int playerId, int defuse, int draw) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasDefuser);
    set_pdata<bool>(pPlayer, m_bHasDefuser, defuse != 0);
    pPlayer->v.body = defuse ? 1 : 0;
    
    if (draw) {
        MESSAGE_BEGIN(MSG_ONE, MessageIdStatusIcon, nullptr, pPlayer);
            WRITE_BYTE(defuse ? 1 : 0);
            WRITE_STRING("defuser");
            WRITE_BYTE(0);
            WRITE_BYTE(160);
            WRITE_BYTE(0);
        MESSAGE_END();
    }
    
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserNvg(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasNightVision);
    return get_pdata<bool>(pPlayer, m_bHasNightVision) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserNvg(int playerId, int nvg) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;
    
    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;
    
    GET_OFFSET("CBasePlayer", m_bHasNightVision);
    set_pdata<bool>(pPlayer, m_bHasNightVision, nvg != 0);
    
    return 1;
}

// Armor and Shield / 护甲和盾牌
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserArmor(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    return static_cast<int>(pPlayer->v.armorvalue);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserArmor(int playerId, int armor, int armorType) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    pPlayer->v.armorvalue = static_cast<float>(armor);
    pPlayer->v.armortype = armorType;

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserShield(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bOwnsShield);
    return get_pdata<bool>(pPlayer, m_bOwnsShield) ? 1 : 0;
}

// Player State / 玩家状态
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserDriving(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bIsInBuyZone);
    return get_pdata<bool>(pPlayer, m_bIsInBuyZone) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStationary(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    return (pPlayer->v.velocity.x == 0.0 && pPlayer->v.velocity.y == 0.0) ? 1 : 0;
}

CSTRIKE_EXPORT float CSTRIKE_CALL GetUserLastActivity(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0.0f;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0.0f;

    GET_OFFSET("CBasePlayer", m_fLastMovement);
    return get_pdata<float>(pPlayer, m_fLastMovement);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserLastActivity(int playerId, float time) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_fLastMovement);
    set_pdata<float>(pPlayer, m_fLastMovement, time);

    return 1;
}

// Player Model / 玩家模型
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserModel(int playerId, char* model, int maxLength) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !model || maxLength <= 0) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    const char* playerModel = INFOKEY_VALUE(GET_INFO_BUFFER(pPlayer), "model");
    CStrikeBridge::SafeStringCopy(model, playerModel, maxLength);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserModel(int playerId, const char* model) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !model) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    SET_CLIENT_KEY_VALUE(playerId, GET_INFO_BUFFER(pPlayer), "model", model);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL ResetUserModel(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_iTeam);
    int team = get_pdata<int>(pPlayer, m_iTeam);

    const char* defaultModel = nullptr;
    switch (team) {
        case 1: defaultModel = "terror"; break;
        case 2: defaultModel = "cstriker"; break;
        default: return 0;
    }

    SET_CLIENT_KEY_VALUE(playerId, GET_INFO_BUFFER(pPlayer), "model", defaultModel);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserSubmodel(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    return pPlayer->v.body;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserSubmodel(int playerId, int submodel) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    pPlayer->v.body = submodel;

    return 1;
}

// Zoom and View / 缩放和视角
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserZoom(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_iFOV);
    return get_pdata<int>(pPlayer, m_iFOV);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserZoom(int playerId, int zoom, int weapon) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_iFOV);
    set_pdata<int>(pPlayer, m_iFOV, zoom);

    return 1;
}

// Statistics / 统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserTked(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bJustKilledTeammate);
    return get_pdata<bool>(pPlayer, m_bJustKilledTeammate) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserTked(int playerId, int tk, int subtract) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bJustKilledTeammate);
    GET_OFFSET("CBasePlayer", m_iTeam);
    GET_OFFSET("CBasePlayer", m_iDeaths);

    set_pdata<bool>(pPlayer, m_bJustKilledTeammate, tk != 0);

    if (subtract > 0) {
        pPlayer->v.frags -= subtract;

        MESSAGE_BEGIN(MSG_ALL, MessageIdScoreInfo);
            WRITE_BYTE(playerId);
            WRITE_SHORT(static_cast<int>(pPlayer->v.frags));
            WRITE_SHORT(get_pdata<int>(pPlayer, m_iDeaths));
            WRITE_SHORT(0);
            WRITE_SHORT(get_pdata<int>(pPlayer, m_iTeam));
        MESSAGE_END();
    }

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserHostageKills(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_iHostagesKilled);
    return get_pdata<int>(pPlayer, m_iHostagesKilled);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserHostageKills(int playerId, int hk) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_iHostagesKilled);
    set_pdata<int>(pPlayer, m_iHostagesKilled, hk);

    return 1;
}

// Special Functions / 特殊功能
CSTRIKE_EXPORT int CSTRIKE_CALL UserSpawn(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    MDLL_Spawn(pPlayer);

    return 1;
}

// ============================================================================
// Weapon System Category Implementation / 武器系统类实现
// ============================================================================

// Weapon Properties / 武器属性
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponSilenced(int weaponEntity) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerWeapon", m_iWeaponState);
    int weaponState = get_pdata<int>(pWeapon, m_iWeaponState);

    return (weaponState & WPNSTATE_USP_SILENCED) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponSilenced(int weaponEntity, int silenced, int drawAnimation) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerItem", m_iId);
    GET_OFFSET("CBasePlayerWeapon", m_iWeaponState);

    int weaponId = get_pdata<int>(pWeapon, m_iId);
    int weaponState = get_pdata<int>(pWeapon, m_iWeaponState);

    if (silenced) {
        weaponState |= WPNSTATE_USP_SILENCED;
    } else {
        weaponState &= ~WPNSTATE_USP_SILENCED;
    }

    set_pdata<int>(pWeapon, m_iWeaponState, weaponState);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponBurstMode(int weaponEntity) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerWeapon", m_iWeaponState);
    int weaponState = get_pdata<int>(pWeapon, m_iWeaponState);

    return (weaponState & WPNSTATE_GLOCK18_BURST_MODE) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponBurstMode(int weaponEntity, int burstMode, int drawAnimation) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerWeapon", m_iWeaponState);
    int weaponState = get_pdata<int>(pWeapon, m_iWeaponState);

    if (burstMode) {
        weaponState |= WPNSTATE_GLOCK18_BURST_MODE;
    } else {
        weaponState &= ~WPNSTATE_GLOCK18_BURST_MODE;
    }

    set_pdata<int>(pWeapon, m_iWeaponState, weaponState);

    return 1;
}

// Weapon Ammo / 武器弹药
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponAmmo(int weaponEntity) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerWeapon", m_iClip);
    return get_pdata<int>(pWeapon, m_iClip);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetWeaponAmmo(int weaponEntity, int ammo) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerWeapon", m_iClip);
    set_pdata<int>(pWeapon, m_iClip, ammo);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserBackpackAmmo(int playerId, int weapon) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_rgAmmo);

    if (weapon < 1 || weapon >= MAX_AMMO_SLOTS) return 0;

    return get_pdata<int>(pPlayer, m_rgAmmo + weapon);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetUserBackpackAmmo(int playerId, int weapon, int amount) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_rgAmmo);

    if (weapon < 1 || weapon >= MAX_AMMO_SLOTS) return 0;

    set_pdata<int>(pPlayer, m_rgAmmo + weapon, amount);

    return 1;
}

// Weapon Information / 武器信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponId(int weaponEntity) {
    if (!CStrikeBridge::IsValidEntity(weaponEntity)) return 0;

    edict_t *pWeapon = TypeConversion.id_to_edict(weaponEntity);
    if (!pWeapon) return 0;

    GET_OFFSET("CBasePlayerItem", m_iId);
    return get_pdata<int>(pWeapon, m_iId);
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponInfo(int weapon, int infoType) {
    if (!GetWeaponInfo) return 0;

    WeaponInfoStruct* info = GetWeaponInfo(weapon);
    if (!info) return 0;

    switch (infoType) {
        case 0: return info->id;
        case 1: return info->cost;
        case 2: return info->clipCost;
        case 3: return info->buyClipSize;
        case 4: return info->gunClipSize;
        case 5: return info->maxRounds;
        case 6: return info->ammoType;
        case 7: return info->ammoName ? 1 : 0;
        default: return 0;
    }
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponEntity(int playerId, int weapon) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_rgpPlayerItems);

    for (int i = 0; i < MAX_ITEM_TYPES; i++) {
        edict_t *pItem = get_pdata<edict_t*>(pPlayer, m_rgpPlayerItems + i);

        while (pItem) {
            GET_OFFSET("CBasePlayerItem", m_iId);
            GET_OFFSET("CBasePlayerItem", m_pNext);

            if (get_pdata<int>(pItem, m_iId) == weapon) {
                return TypeConversion.edict_to_id(pItem);
            }

            pItem = get_pdata<edict_t*>(pItem, m_pNext);
        }
    }

    return 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeapon(int playerId, int* clip, int* ammo) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_pActiveItem);
    edict_t *pActiveItem = get_pdata<edict_t*>(pPlayer, m_pActiveItem);

    if (!pActiveItem) return 0;

    GET_OFFSET("CBasePlayerItem", m_iId);
    GET_OFFSET("CBasePlayerWeapon", m_iClip);

    int weaponId = get_pdata<int>(pActiveItem, m_iId);

    if (clip) {
        *clip = get_pdata<int>(pActiveItem, m_iClip);
    }

    if (ammo) {
        GET_OFFSET("CBasePlayer", m_rgAmmo);
        *ammo = get_pdata<int>(pPlayer, m_rgAmmo + weaponId);
    }

    return weaponId;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserHasPrimary(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bHasPrimary);
    return get_pdata<bool>(pPlayer, m_bHasPrimary) ? 1 : 0;
}

// ============================================================================
// Game Entity Category Implementation / 游戏实体类实现
// ============================================================================

// Entity Management / 实体管理
CSTRIKE_EXPORT int CSTRIKE_CALL CreateEntity(const char* classname) {
    if (!CS_CreateNamedEntity || !classname) return 0;

    int iszClass = ALLOC_STRING(classname);
    edict_t *pEnt = CS_CreateNamedEntity(iszClass);

    if (!FNullEnt(pEnt)) {
        return TypeConversion.edict_to_id(pEnt);
    }

    return 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL FindEntityByClass(int startIndex, const char* classname) {
    if (!classname) return 0;

    edict_t *pEntity = nullptr;

    if (startIndex > 0) {
        pEntity = TypeConversion.id_to_edict(startIndex);
    }

    pEntity = FIND_ENTITY_BY_STRING(pEntity, "classname", classname);

    if (!FNullEnt(pEntity)) {
        return TypeConversion.edict_to_id(pEntity);
    }

    return 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL FindEntityByOwner(int startIndex, int owner) {
    edict_t *pEntity = nullptr;
    edict_t *pOwner = nullptr;

    if (startIndex > 0) {
        pEntity = TypeConversion.id_to_edict(startIndex);
    }

    if (owner > 0) {
        pOwner = TypeConversion.id_to_edict(owner);
    }

    pEntity = FIND_ENTITY_BY_STRING(pEntity, "owner", STRING(pOwner->v.netname));

    if (!FNullEnt(pEntity)) {
        return TypeConversion.edict_to_id(pEntity);
    }

    return 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetEntityClass(int entity, const char* classname) {
    if (!CStrikeBridge::IsValidEntity(entity) || !classname) return 0;

    edict_t *pEntity = TypeConversion.id_to_edict(entity);
    if (!pEntity) return 0;

    pEntity->v.classname = ALLOC_STRING(classname);

    return 1;
}

// Hostage System / 人质系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetHostageId(int index) {
    // Implementation depends on hostage entity structure
    return 0; // Placeholder
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetHostageFollow(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0;

    GET_OFFSET("CHostage", m_hTargetEnt);
    edict_t *pTarget = get_pdata<edict_t*>(pHostage, m_hTargetEnt);

    return pTarget ? TypeConversion.edict_to_id(pTarget) : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageFollow(int entity, int followId) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0;

    edict_t *pTarget = nullptr;
    if (followId > 0) {
        pTarget = TypeConversion.id_to_edict(followId);
    }

    GET_OFFSET("CHostage", m_hTargetEnt);
    set_pdata<edict_t*>(pHostage, m_hTargetEnt, pTarget);

    return 1;
}

CSTRIKE_EXPORT float CSTRIKE_CALL GetHostageLastUse(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0.0f;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0.0f;

    GET_OFFSET("CHostage", m_flLastUse);
    return get_pdata<float>(pHostage, m_flLastUse);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageLastUse(int entity, float time) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0;

    GET_OFFSET("CHostage", m_flLastUse);
    set_pdata<float>(pHostage, m_flLastUse, time);

    return 1;
}

CSTRIKE_EXPORT float CSTRIKE_CALL GetHostageNextUse(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0.0f;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0.0f;

    GET_OFFSET("CHostage", m_flNextUse);
    return get_pdata<float>(pHostage, m_flNextUse);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetHostageNextUse(int entity, float time) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pHostage = TypeConversion.id_to_edict(entity);
    if (!pHostage) return 0;

    GET_OFFSET("CHostage", m_flNextUse);
    set_pdata<float>(pHostage, m_flNextUse, time);

    return 1;
}

// C4 Bomb System / C4炸弹系统
CSTRIKE_EXPORT float CSTRIKE_CALL GetC4ExplodeTime(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0.0f;

    edict_t *pC4 = TypeConversion.id_to_edict(entity);
    if (!pC4) return 0.0f;

    GET_OFFSET("CGrenade", m_flC4Blow);
    return get_pdata<float>(pC4, m_flC4Blow);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetC4ExplodeTime(int entity, float time) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pC4 = TypeConversion.id_to_edict(entity);
    if (!pC4) return 0;

    GET_OFFSET("CGrenade", m_flC4Blow);
    set_pdata<float>(pC4, m_flC4Blow, time);

    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetC4Defusing(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pC4 = TypeConversion.id_to_edict(entity);
    if (!pC4) return 0;

    GET_OFFSET("CGrenade", m_bStartDefuse);
    return get_pdata<bool>(pC4, m_bStartDefuse) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetC4Defusing(int entity, int defusing) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pC4 = TypeConversion.id_to_edict(entity);
    if (!pC4) return 0;

    GET_OFFSET("CGrenade", m_bStartDefuse);
    set_pdata<bool>(pC4, m_bStartDefuse, defusing != 0);

    return 1;
}

// Armoury System / 武器库系统
CSTRIKE_EXPORT int CSTRIKE_CALL GetArmouryType(int entity) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pArmoury = TypeConversion.id_to_edict(entity);
    if (!pArmoury) return 0;

    GET_OFFSET("CArmoury", m_iItem);
    return get_pdata<int>(pArmoury, m_iItem);
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetArmouryType(int entity, int type) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pArmoury = TypeConversion.id_to_edict(entity);
    if (!pArmoury) return 0;

    GET_OFFSET("CArmoury", m_iItem);
    set_pdata<int>(pArmoury, m_iItem, type);

    return 1;
}

// Weapon Box / 武器盒
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponBoxItem(int entity, int slot) {
    if (!CStrikeBridge::IsValidEntity(entity)) return 0;

    edict_t *pWeaponBox = TypeConversion.id_to_edict(entity);
    if (!pWeaponBox) return 0;

    GET_OFFSET("CWeaponBox", m_rgpPlayerItems);

    if (slot < 0 || slot >= MAX_ITEM_TYPES) return 0;

    edict_t *pItem = get_pdata<edict_t*>(pWeaponBox, m_rgpPlayerItems + slot);

    return pItem ? TypeConversion.edict_to_id(pItem) : 0;
}

// ============================================================================
// Map Environment Category Implementation / 地图环境类实现
// ============================================================================

// Zone Detection / 区域检测
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserBuyZone(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    GET_OFFSET("CBasePlayer", m_bIsInBuyZone);
    return get_pdata<bool>(pPlayer, m_bIsInBuyZone) ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserMapZones(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    edict_t *pPlayer = MF_GetPlayerEdict(playerId);
    if (!pPlayer) return 0;

    int zones = 0;

    GET_OFFSET("CBasePlayer", m_bIsInBuyZone);
    GET_OFFSET("CBasePlayer", m_bIsInBombZone);
    GET_OFFSET("CBasePlayer", m_bIsInRescueZone);

    if (get_pdata<bool>(pPlayer, m_bIsInBuyZone)) zones |= (1 << 0);
    if (get_pdata<bool>(pPlayer, m_bIsInBombZone)) zones |= (1 << 1);
    if (get_pdata<bool>(pPlayer, m_bIsInRescueZone)) zones |= (1 << 2);

    return zones;
}

// ============================================================================
// Special Features Category Implementation / 特殊功能类实现
// ============================================================================

// No Knives Mode / 无刀模式
CSTRIKE_EXPORT int CSTRIKE_CALL GetNoKnives() {
    extern bool NoKnivesMode;
    return NoKnivesMode ? 1 : 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL SetNoKnives(int noKnives) {
    extern bool NoKnivesMode;
    extern void ToggleHook_GiveDefaultItems(bool enable);

    NoKnivesMode = noKnives != 0;
    ToggleHook_GiveDefaultItems(NoKnivesMode);

    return 1;
}

// Item Information / 物品信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetItemId(const char* name) {
    if (!name) return 0;

    // Search through item list
    for (int i = 0; i < CSI_MAX_COUNT; i++) {
        if (ItemsManager.GetItemInfo(i) &&
            strcmp(ItemsManager.GetItemInfo(i)->name, name) == 0) {
            return i;
        }
    }

    return 0;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetItemAlias(int id, char* alias, int maxLength) {
    if (!alias || maxLength <= 0) return 0;

    auto itemInfo = ItemsManager.GetItemInfo(id);
    if (!itemInfo || !itemInfo->alias) {
        alias[0] = '\0';
        return 0;
    }

    CStrikeBridge::SafeStringCopy(alias, itemInfo->alias, maxLength);
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetTranslatedItemAlias(int id, char* alias, int maxLength) {
    if (!alias || maxLength <= 0) return 0;

    auto itemInfo = ItemsManager.GetItemInfo(id);
    if (!itemInfo) {
        alias[0] = '\0';
        return 0;
    }

    const char* translatedAlias = itemInfo->alias;
    // Add translation logic here if needed

    CStrikeBridge::SafeStringCopy(alias, translatedAlias, maxLength);
    return 1;
}

// ============================================================================
// Forward Callbacks Implementation / Forward回调实现
// ============================================================================

namespace CStrikeBridge {
    int ExecuteInternalCommandForward(int playerId, const char* command) {
        std::lock_guard<std::mutex> lock(g_callbackMutex);

        for (const auto& info : g_callbacks) {
            if (info.type == 0) { // InternalCommand
                auto callback = reinterpret_cast<InternalCommandCallback>(info.callback);
                if (callback) {
                    int result = callback(playerId, command);
                    if (result != 0) return result; // Stop execution if handled
                }
            }
        }

        return 0; // Continue
    }

    int ExecuteBuyAttemptForward(int playerId, int item) {
        std::lock_guard<std::mutex> lock(g_callbackMutex);

        for (const auto& info : g_callbacks) {
            if (info.type == 1) { // BuyAttempt
                auto callback = reinterpret_cast<BuyAttemptCallback>(info.callback);
                if (callback) {
                    int result = callback(playerId, item);
                    if (result != 0) return result; // Stop execution if handled
                }
            }
        }

        return 0; // Continue
    }

    int ExecuteBuyForward(int playerId, int item) {
        std::lock_guard<std::mutex> lock(g_callbackMutex);

        for (const auto& info : g_callbacks) {
            if (info.type == 2) { // Buy
                auto callback = reinterpret_cast<BuyCallback>(info.callback);
                if (callback) {
                    int result = callback(playerId, item);
                    if (result != 0) return result; // Stop execution if handled
                }
            }
        }

        return 0; // Continue
    }
}

// Forward registration / Forward注册
CSTRIKE_EXPORT int CSTRIKE_CALL RegisterInternalCommandCallback(InternalCommandCallback callback) {
    return CStrikeBridge::RegisterCallback(reinterpret_cast<void*>(callback), 0);
}

CSTRIKE_EXPORT int CSTRIKE_CALL RegisterBuyAttemptCallback(BuyAttemptCallback callback) {
    return CStrikeBridge::RegisterCallback(reinterpret_cast<void*>(callback), 1);
}

CSTRIKE_EXPORT int CSTRIKE_CALL RegisterBuyCallback(BuyCallback callback) {
    return CStrikeBridge::RegisterCallback(reinterpret_cast<void*>(callback), 2);
}

// Forward unregistration / Forward注销
CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterInternalCommandCallback(int callbackId) {
    CStrikeBridge::UnregisterCallback(callbackId);
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterBuyAttemptCallback(int callbackId) {
    CStrikeBridge::UnregisterCallback(callbackId);
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL UnregisterBuyCallback(int callbackId) {
    CStrikeBridge::UnregisterCallback(callbackId);
    return 1;
}

// ============================================================================
// Statistics Analysis Category Implementation / 统计分析类实现
// ============================================================================

// External references to CSX module functions
extern AMX_NATIVE_INFO stats_Natives[];

// Helper function to call CSX native functions
static cell CallCSXNative(const char* nativeName, AMX* amx, cell* params) {
    // Find the native function in stats_Natives array
    for (int i = 0; stats_Natives[i].name; i++) {
        if (strcmp(stats_Natives[i].name, nativeName) == 0) {
            return stats_Natives[i].func(amx, params);
        }
    }
    return 0;
}

// Global Statistics / 全局统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetStats(int index, int* stats, int* bodyhits, char* name, int nameLength, char* authid, int authidLength) {
    if (!stats || !bodyhits) return 0;

    // Create a temporary AMX instance for the call
    AMX amx = {0};
    cell params[8];
    cell statsArray[8];  // STATSX_MAX_STATS
    cell bodyhitsArray[8]; // MAX_BODYHITS

    params[0] = 7 * sizeof(cell); // param count * cell size
    params[1] = index;
    params[2] = (cell)statsArray;
    params[3] = (cell)bodyhitsArray;
    params[4] = 0; // name buffer (will be handled separately)
    params[5] = nameLength;
    params[6] = 0; // authid buffer (will be handled separately)
    params[7] = authidLength;

    int result = CallCSXNative("get_stats", &amx, params);

    if (result > 0) {
        // Copy stats array
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
        }

        // Copy bodyhits array
        for (int i = 0; i < 8; i++) {
            bodyhits[i] = (int)bodyhitsArray[i];
        }

        // Handle name and authid strings would require more complex AMX integration
        // For now, we'll leave them empty
        if (name && nameLength > 0) name[0] = '\0';
        if (authid && authidLength > 0) authid[0] = '\0';
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetStats2(int index, int* stats, int* bodyhits, char* name, int nameLength, char* authid, int authidLength, int* objectives) {
    if (!stats || !bodyhits || !objectives) return 0;

    // First get basic stats
    int result = GetStats(index, stats, bodyhits, name, nameLength, authid, authidLength);

    if (result > 0) {
        // Get extended objectives stats
        AMX amx = {0};
        cell params[5];
        cell objectivesArray[4]; // STATSX_MAX_OBJECTIVE

        params[0] = 4 * sizeof(cell);
        params[1] = index;
        params[2] = (cell)objectivesArray;
        params[3] = 0; // authid buffer
        params[4] = authidLength;

        int objResult = CallCSXNative("get_stats2", &amx, params);

        if (objResult > 0) {
            for (int i = 0; i < 4; i++) {
                objectives[i] = (int)objectivesArray[i];
            }
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetStatsNum() {
    AMX amx = {0};
    cell params[1];
    params[0] = 0; // no parameters

    return (int)CallCSXNative("get_statsnum", &amx, params);
}

// Player Statistics / 玩家统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStats(int playerId, int* stats, int* bodyhits) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[4];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 3 * sizeof(cell);
    params[1] = playerId;
    params[2] = (cell)statsArray;
    params[3] = (cell)bodyhitsArray;

    int result = CallCSXNative("get_user_stats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserStats2(int playerId, int* stats, int* bodyhits, int* objectives) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits || !objectives) return 0;

    // First get basic stats
    int result = GetUserStats(playerId, stats, bodyhits);

    if (result > 0) {
        // Get extended objectives stats
        AMX amx = {0};
        cell params[4];
        cell objectivesArray[4];

        params[0] = 3 * sizeof(cell);
        params[1] = playerId;
        params[2] = (cell)objectivesArray;
        params[3] = (cell)objectivesArray; // placeholder

        int objResult = CallCSXNative("get_user_stats2", &amx, params);

        if (objResult > 0) {
            for (int i = 0; i < 4; i++) {
                objectives[i] = (int)objectivesArray[i];
            }
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserRoundStats(int playerId, int* stats, int* bodyhits) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[4];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 3 * sizeof(cell);
    params[1] = playerId;
    params[2] = (cell)statsArray;
    params[3] = (cell)bodyhitsArray;

    int result = CallCSXNative("get_user_rstats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserAttackerStats(int playerId, int attackerId, int* stats, int* bodyhits, char* weaponName, int weaponNameLength) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[7];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 6 * sizeof(cell);
    params[1] = playerId;
    params[2] = attackerId;
    params[3] = (cell)statsArray;
    params[4] = (cell)bodyhitsArray;
    params[5] = 0; // weapon name buffer
    params[6] = weaponNameLength;

    int result = CallCSXNative("get_user_astats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }

        // Clear weapon name for now
        if (weaponName && weaponNameLength > 0) {
            weaponName[0] = '\0';
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserVictimStats(int playerId, int victimId, int* stats, int* bodyhits, char* weaponName, int weaponNameLength) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[7];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 6 * sizeof(cell);
    params[1] = playerId;
    params[2] = victimId;
    params[3] = (cell)statsArray;
    params[4] = (cell)bodyhitsArray;
    params[5] = 0; // weapon name buffer
    params[6] = weaponNameLength;

    int result = CallCSXNative("get_user_vstats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }

        // Clear weapon name for now
        if (weaponName && weaponNameLength > 0) {
            weaponName[0] = '\0';
        }
    }

    return result;
}

// Weapon Statistics / 武器统计
CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponStats(int playerId, int weaponId, int* stats, int* bodyhits) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[5];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 4 * sizeof(cell);
    params[1] = playerId;
    params[2] = weaponId;
    params[3] = (cell)statsArray;
    params[4] = (cell)bodyhitsArray;

    int result = CallCSXNative("get_user_wstats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetUserWeaponRoundStats(int playerId, int weaponId, int* stats, int* bodyhits) {
    if (!CStrikeBridge::IsValidPlayer(playerId) || !stats || !bodyhits) return 0;

    AMX amx = {0};
    cell params[5];
    cell statsArray[8];
    cell bodyhitsArray[8];

    params[0] = 4 * sizeof(cell);
    params[1] = playerId;
    params[2] = weaponId;
    params[3] = (cell)statsArray;
    params[4] = (cell)bodyhitsArray;

    int result = CallCSXNative("get_user_wrstats", &amx, params);

    if (result > 0) {
        for (int i = 0; i < 8; i++) {
            stats[i] = (int)statsArray[i];
            bodyhits[i] = (int)bodyhitsArray[i];
        }
    }

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL ResetUserWeaponStats(int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    AMX amx = {0};
    cell params[2];

    params[0] = 1 * sizeof(cell);
    params[1] = playerId;

    CallCSXNative("reset_user_wstats", &amx, params);
    return 1;
}

// Custom Weapon Support / 自定义武器支持
CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponAdd(const char* weaponName, int melee, const char* logName) {
    if (!weaponName) return 0;

    AMX amx = {0};
    cell params[4];

    params[0] = 3 * sizeof(cell);
    params[1] = 0; // weapon name string (would need AMX string handling)
    params[2] = melee;
    params[3] = 0; // log name string (would need AMX string handling)

    // For now, return a placeholder value
    // Full implementation would require proper AMX string handling
    return CallCSXNative("custom_weapon_add", &amx, params);
}

CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponDamage(int weapon, int attacker, int victim, int damage, int hitplace) {
    if (!CStrikeBridge::IsValidPlayer(attacker) || !CStrikeBridge::IsValidPlayer(victim)) return 0;

    AMX amx = {0};
    cell params[6];

    params[0] = 5 * sizeof(cell);
    params[1] = weapon;
    params[2] = attacker;
    params[3] = victim;
    params[4] = damage;
    params[5] = hitplace;

    CallCSXNative("custom_weapon_dmg", &amx, params);
    return 1;
}

CSTRIKE_EXPORT int CSTRIKE_CALL CustomWeaponShot(int weapon, int playerId) {
    if (!CStrikeBridge::IsValidPlayer(playerId)) return 0;

    AMX amx = {0};
    cell params[3];

    params[0] = 2 * sizeof(cell);
    params[1] = weapon;
    params[2] = playerId;

    CallCSXNative("custom_weapon_shot", &amx, params);
    return 1;
}

// Weapon Information / 武器信息
CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponName(int weaponId, char* name, int maxLength) {
    if (!name || maxLength <= 0) return 0;

    AMX amx = {0};
    cell params[4];

    params[0] = 3 * sizeof(cell);
    params[1] = weaponId;
    params[2] = 0; // name buffer
    params[3] = maxLength;

    int result = CallCSXNative("xmod_get_wpnname", &amx, params);

    // For now, clear the name buffer
    name[0] = '\0';

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetWeaponLogName(int weaponId, char* logName, int maxLength) {
    if (!logName || maxLength <= 0) return 0;

    AMX amx = {0};
    cell params[4];

    params[0] = 3 * sizeof(cell);
    params[1] = weaponId;
    params[2] = 0; // log name buffer
    params[3] = maxLength;

    int result = CallCSXNative("xmod_get_wpnlogname", &amx, params);

    // For now, clear the log name buffer
    logName[0] = '\0';

    return result;
}

CSTRIKE_EXPORT int CSTRIKE_CALL IsWeaponMelee(int weaponId) {
    AMX amx = {0};
    cell params[2];

    params[0] = 1 * sizeof(cell);
    params[1] = weaponId;

    return (int)CallCSXNative("xmod_is_melee_wpn", &amx, params);
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetMaxWeapons() {
    AMX amx = {0};
    cell params[1];
    params[0] = 0;

    return (int)CallCSXNative("xmod_get_maxweapons", &amx, params);
}

CSTRIKE_EXPORT int CSTRIKE_CALL GetStatsSize() {
    // Return the size of STATSX_MAX_STATS
    return 8;
}

// Map Objectives / 地图目标
CSTRIKE_EXPORT int CSTRIKE_CALL GetMapObjectives() {
    AMX amx = {0};
    cell params[1];
    params[0] = 0;

    return (int)CallCSXNative("get_map_objectives", &amx, params);
}

} // extern "C"
