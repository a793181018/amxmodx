// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

// csharp_bridge.h - C# Bridge Header for AMX Mod X Command Registration Interface
// Cross-platform bridge for C# interop

#ifndef CSHARP_BRIDGE_H
#define CSHARP_BRIDGE_H

#include "amxmodx.h"

// Cross-platform export macros
#ifdef _WIN32
    #define CSHARP_EXPORT extern "C" __declspec(dllexport)
    #define CSHARP_CALL __stdcall
#else
    #define CSHARP_EXPORT extern "C" __attribute__((visibility("default")))
    #define CSHARP_CALL
#endif

// Command types enumeration
enum CSharpCommandType
{
    CSHARP_COMMAND_TYPE_CONSOLE = 0,    // Console command (register_concmd)
    CSHARP_COMMAND_TYPE_CLIENT = 1,     // Client command (register_clcmd)
    CSHARP_COMMAND_TYPE_SERVER = 2      // Server command (register_srvcmd)
};

// Command callback delegate signature
typedef void (CSHARP_CALL *CSharpCommandCallback)(int clientId, int commandId, int flags);

// Menu callback delegate signature
typedef void (CSHARP_CALL *CSharpMenuCallback)(int clientId, int menuId, int key);

// Event callback delegate signature
typedef void (CSHARP_CALL *CSharpEventCallback)(int eventId, int clientId, int numParams);

// Forward callback delegate signature
typedef int (CSHARP_CALL *CSharpForwardCallback)(int forwardId, int numParams);

// Command information structure for C# interop
struct CSharpCommandInfo
{
    char command[128];
    char info[256];
    int flags;
    int commandId;
    bool infoMultiLang;
    bool listable;
};

// Event information structure for C# interop
struct CSharpEventInfo
{
    char eventName[64];
    int eventId;
    int flags;
    int numParams;
    bool isActive;
};

// Forward information structure for C# interop
struct CSharpForwardInfo
{
    char forwardName[64];
    int forwardId;
    int numParams;
    int execType;
    bool isValid;
};

// Event parameter structure for C# interop
struct CSharpEventParam
{
    int type;           // 0=int, 1=float, 2=string
    int intValue;
    float floatValue;
    char stringValue[256];
};

// Player information structure for C# interop
struct CSharpPlayerInfo
{
    char name[64];
    char ip[32];
    char authId[64];
    char team[32];
    int index;
    int teamId;
    int userId;
    int flags;
    float connectTime;
    float playTime;
    bool isInGame;
    bool isBot;
    bool isAlive;
    bool isAuthorized;
    bool isConnecting;
    bool isHLTV;
    bool hasVGUI;
};

// Player statistics structure for C# interop
struct CSharpPlayerStats
{
    int deaths;
    int kills;
    float frags;
    int currentWeapon;
    int menu;
    int keys;
    float health;
    float armor;
    int aiming;
    float menuExpire;
    int weapons[32];  // Weapon ammo array
    int clips[32];    // Weapon clip array
};

// Bridge initialization and cleanup
CSHARP_EXPORT void CSHARP_CALL InitializeCSharpBridge();
CSHARP_EXPORT void CSHARP_CALL CleanupCSharpBridge();

// Command registration functions
CSHARP_EXPORT int CSHARP_CALL RegisterConsoleCommand(
    const char* command, 
    CSharpCommandCallback callback, 
    int flags, 
    const char* info, 
    bool infoMultiLang
);

CSHARP_EXPORT int CSHARP_CALL RegisterClientCommand(
    const char* command, 
    CSharpCommandCallback callback, 
    int flags, 
    const char* info, 
    bool infoMultiLang
);

CSHARP_EXPORT int CSHARP_CALL RegisterServerCommand(
    const char* command, 
    CSharpCommandCallback callback, 
    int flags, 
    const char* info, 
    bool infoMultiLang
);

// Menu command functions
CSHARP_EXPORT int CSHARP_CALL RegisterMenuCommand(
    int menuId, 
    int keyMask, 
    CSharpMenuCallback callback
);

CSHARP_EXPORT int CSHARP_CALL RegisterMenuId(
    const char* menuName, 
    bool global
);

// Command query functions
CSHARP_EXPORT bool CSHARP_CALL GetCommandInfo(
    int commandId, 
    CSharpCommandType commandType, 
    CSharpCommandInfo* outInfo
);

CSHARP_EXPORT int CSHARP_CALL GetCommandCount(
    CSharpCommandType commandType, 
    int accessFlags
);

// Command management functions
CSHARP_EXPORT bool CSHARP_CALL UnregisterCommand(int commandId);

// Command execution functions
CSHARP_EXPORT void CSHARP_CALL ExecuteServerCommand(const char* command);
CSHARP_EXPORT void CSHARP_CALL ExecuteClientCommand(int clientId, const char* command);
CSHARP_EXPORT void CSHARP_CALL ExecuteConsoleCommand(int clientId, const char* command);

// Command argument reading functions
CSHARP_EXPORT int CSHARP_CALL GetCommandArgCount();
CSHARP_EXPORT bool CSHARP_CALL GetCommandArg(int index, char* buffer, int bufferSize);
CSHARP_EXPORT bool CSHARP_CALL GetCommandArgs(char* buffer, int bufferSize);
CSHARP_EXPORT int CSHARP_CALL GetCommandArgInt(int index);
CSHARP_EXPORT float CSHARP_CALL GetCommandArgFloat(int index);

// Command query functions
CSHARP_EXPORT bool CSHARP_CALL FindCommand(const char* commandName, CSharpCommandType commandType, CSharpCommandInfo* outInfo);
CSHARP_EXPORT int CSHARP_CALL GetCommandsCount(CSharpCommandType commandType, int accessFlags);
CSHARP_EXPORT bool CSHARP_CALL GetCommandByIndex(int index, CSharpCommandType commandType, int accessFlags, CSharpCommandInfo* outInfo);

// Event system functions
CSHARP_EXPORT int CSHARP_CALL RegisterEvent(const char* eventName, CSharpEventCallback callback, int flags, const char* conditions);
CSHARP_EXPORT bool CSHARP_CALL UnregisterEvent(int eventHandle);
CSHARP_EXPORT int CSHARP_CALL GetEventId(const char* eventName);
CSHARP_EXPORT bool CSHARP_CALL GetEventInfo(int eventId, CSharpEventInfo* outInfo);

// Event parameter reading functions
CSHARP_EXPORT int CSHARP_CALL GetEventArgCount();
CSHARP_EXPORT bool CSHARP_CALL GetEventArg(int index, CSharpEventParam* outParam);
CSHARP_EXPORT int CSHARP_CALL GetEventArgInt(int index);
CSHARP_EXPORT float CSHARP_CALL GetEventArgFloat(int index);
CSHARP_EXPORT bool CSHARP_CALL GetEventArgString(int index, char* buffer, int bufferSize);

// Forward system functions
CSHARP_EXPORT int CSHARP_CALL CreateForward(const char* forwardName, int execType, const int* paramTypes, int numParams);
CSHARP_EXPORT int CSHARP_CALL CreateSingleForward(const char* functionName, CSharpForwardCallback callback, const int* paramTypes, int numParams);
CSHARP_EXPORT bool CSHARP_CALL ExecuteForward(int forwardId, const CSharpEventParam* params, int numParams, int* outResult);
CSHARP_EXPORT bool CSHARP_CALL UnregisterForward(int forwardId);
CSHARP_EXPORT bool CSHARP_CALL GetForwardInfo(int forwardId, CSharpForwardInfo* outInfo);

// Player information functions
CSHARP_EXPORT bool CSHARP_CALL IsPlayerValid(int clientId);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerInfo(int clientId, CSharpPlayerInfo* outInfo);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerStats(int clientId, CSharpPlayerStats* outStats);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerName(int clientId, char* buffer, int bufferSize);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerIP(int clientId, char* buffer, int bufferSize);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerAuthId(int clientId, char* buffer, int bufferSize);
CSHARP_EXPORT bool CSHARP_CALL GetPlayerTeam(int clientId, char* buffer, int bufferSize);

// Player state functions
CSHARP_EXPORT bool CSHARP_CALL IsPlayerInGame(int clientId);
CSHARP_EXPORT bool CSHARP_CALL IsPlayerBot(int clientId);
CSHARP_EXPORT bool CSHARP_CALL IsPlayerAlive(int clientId);
CSHARP_EXPORT bool CSHARP_CALL IsPlayerAuthorized(int clientId);
CSHARP_EXPORT bool CSHARP_CALL IsPlayerConnecting(int clientId);
CSHARP_EXPORT bool CSHARP_CALL IsPlayerHLTV(int clientId);

// Player property getters
CSHARP_EXPORT int CSHARP_CALL GetPlayerUserId(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerTeamId(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerFlags(int clientId);
CSHARP_EXPORT float CSHARP_CALL GetPlayerConnectTime(int clientId);
CSHARP_EXPORT float CSHARP_CALL GetPlayerPlayTime(int clientId);
CSHARP_EXPORT float CSHARP_CALL GetPlayerHealth(int clientId);
CSHARP_EXPORT float CSHARP_CALL GetPlayerArmor(int clientId);
CSHARP_EXPORT float CSHARP_CALL GetPlayerFrags(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerDeaths(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerCurrentWeapon(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerMenu(int clientId);
CSHARP_EXPORT int CSHARP_CALL GetPlayerKeys(int clientId);

// Player property setters
CSHARP_EXPORT bool CSHARP_CALL SetPlayerHealth(int clientId, float health);
CSHARP_EXPORT bool CSHARP_CALL SetPlayerArmor(int clientId, float armor);
CSHARP_EXPORT bool CSHARP_CALL SetPlayerFrags(int clientId, float frags);
CSHARP_EXPORT bool CSHARP_CALL SetPlayerTeamInfo(int clientId, int teamId, const char* teamName);
CSHARP_EXPORT bool CSHARP_CALL SetPlayerFlags(int clientId, int flags);

// Player utility functions
CSHARP_EXPORT int CSHARP_CALL GetMaxClients();
CSHARP_EXPORT int CSHARP_CALL GetConnectedPlayersCount();
CSHARP_EXPORT bool CSHARP_CALL GetConnectedPlayers(int* playerIds, int maxPlayers, int* outCount);
CSHARP_EXPORT bool CSHARP_CALL KickPlayer(int clientId, const char* reason);
CSHARP_EXPORT bool CSHARP_CALL SlayPlayer(int clientId);

// ========== 实体管理接口 / Entity Management Interfaces ==========

// Entity creation and removal
CSHARP_EXPORT int CSHARP_CALL CreateEntity(const char* className);
CSHARP_EXPORT bool CSHARP_CALL RemoveEntity(int entityId);
CSHARP_EXPORT int CSHARP_CALL GetEntityCount();

// Entity finding functions
CSHARP_EXPORT int CSHARP_CALL FindEntityByClassName(int startEntity, const char* className);
CSHARP_EXPORT int CSHARP_CALL FindEntityByTargetName(int startEntity, const char* targetName);
CSHARP_EXPORT int CSHARP_CALL FindEntityInSphere(int startEntity, const float* origin, float radius);

// Entity property getters
CSHARP_EXPORT int CSHARP_CALL GetEntityInt(int entityId, const char* property);
CSHARP_EXPORT float CSHARP_CALL GetEntityFloat(int entityId, const char* property);
CSHARP_EXPORT bool CSHARP_CALL GetEntityString(int entityId, const char* property, char* buffer, int bufferSize);
CSHARP_EXPORT bool CSHARP_CALL GetEntityVector(int entityId, const char* property, float* vector);

// Entity property setters
CSHARP_EXPORT bool CSHARP_CALL SetEntityInt(int entityId, const char* property, int value);
CSHARP_EXPORT bool CSHARP_CALL SetEntityFloat(int entityId, const char* property, float value);
CSHARP_EXPORT bool CSHARP_CALL SetEntityString(int entityId, const char* property, const char* value);
CSHARP_EXPORT bool CSHARP_CALL SetEntityVector(int entityId, const char* property, const float* vector);

// Entity utility functions
CSHARP_EXPORT bool CSHARP_CALL SetEntityOrigin(int entityId, const float* origin);
CSHARP_EXPORT bool CSHARP_CALL SetEntitySize(int entityId, const float* mins, const float* maxs);
CSHARP_EXPORT bool CSHARP_CALL SetEntityModel(int entityId, const char* model);

// ========== 消息系统接口 / Message System Interfaces ==========

// Message callback delegate signature
typedef void (CSHARP_CALL *CSharpMessageCallback)(int msgType, int msgDest, int entityId);

// Message registration and management
CSHARP_EXPORT int CSHARP_CALL RegisterMessage(int msgId, CSharpMessageCallback callback);
CSHARP_EXPORT bool CSHARP_CALL UnregisterMessage(int msgId, int msgHandle);
CSHARP_EXPORT bool CSHARP_CALL SetMessageBlock(int msgId, int blockType);
CSHARP_EXPORT int CSHARP_CALL GetMessageBlock(int msgId);

// Message sending functions
CSHARP_EXPORT bool CSHARP_CALL MessageBegin(int msgDest, int msgType, const float* origin, int entityId);
CSHARP_EXPORT void CSHARP_CALL MessageEnd();

// Message writing functions
CSHARP_EXPORT void CSHARP_CALL WriteByte(int value);
CSHARP_EXPORT void CSHARP_CALL WriteChar(int value);
CSHARP_EXPORT void CSHARP_CALL WriteShort(int value);
CSHARP_EXPORT void CSHARP_CALL WriteLong(int value);
CSHARP_EXPORT void CSHARP_CALL WriteAngle(float value);
CSHARP_EXPORT void CSHARP_CALL WriteCoord(float value);
CSHARP_EXPORT void CSHARP_CALL WriteString(const char* value);
CSHARP_EXPORT void CSHARP_CALL WriteEntity(int value);

// Engine message functions
CSHARP_EXPORT bool CSHARP_CALL EngineMessageBegin(int msgDest, int msgType, const float* origin, int entityId);
CSHARP_EXPORT void CSHARP_CALL EngineMessageEnd();
CSHARP_EXPORT void CSHARP_CALL EngineWriteByte(int value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteChar(int value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteShort(int value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteLong(int value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteAngle(float value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteCoord(float value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteString(const char* value);
CSHARP_EXPORT void CSHARP_CALL EngineWriteEntity(int value);

// Message utility functions
CSHARP_EXPORT int CSHARP_CALL GetUserMessageId(const char* msgName);
CSHARP_EXPORT bool CSHARP_CALL GetUserMessageName(int msgId, char* buffer, int bufferSize);

// ========== CVars系统接口 / CVars System Interfaces ==========

// CVar callback delegate signature
typedef void (CSHARP_CALL *CSharpCvarCallback)(const char* cvarName, const char* oldValue, const char* newValue);

// CVar information structure
struct CSharpCvarInfo
{
    char name[64];
    char value[256];
    char defaultValue[256];
    char description[256];
    int flags;
    float floatValue;
    bool hasMin;
    float minValue;
    bool hasMax;
    float maxValue;
};

// CVar creation and registration
CSHARP_EXPORT bool CSHARP_CALL CreateCvar(const char* name, const char* value, int flags, const char* description, bool hasMin, float minValue, bool hasMax, float maxValue);
CSHARP_EXPORT bool CSHARP_CALL RegisterCvar(const char* name, const char* value, int flags, float floatValue);
CSHARP_EXPORT bool CSHARP_CALL CvarExists(const char* name);

// CVar value getters
CSHARP_EXPORT bool CSHARP_CALL GetCvarString(const char* name, char* buffer, int bufferSize);
CSHARP_EXPORT int CSHARP_CALL GetCvarInt(const char* name);
CSHARP_EXPORT float CSHARP_CALL GetCvarFloat(const char* name);
CSHARP_EXPORT int CSHARP_CALL GetCvarFlags(const char* name);

// CVar value setters
CSHARP_EXPORT bool CSHARP_CALL SetCvarString(const char* name, const char* value);
CSHARP_EXPORT bool CSHARP_CALL SetCvarInt(const char* name, int value);
CSHARP_EXPORT bool CSHARP_CALL SetCvarFloat(const char* name, float value);
CSHARP_EXPORT bool CSHARP_CALL SetCvarFlags(const char* name, int flags);

// CVar information and management
CSHARP_EXPORT bool CSHARP_CALL GetCvarInfo(const char* name, CSharpCvarInfo* outInfo);
CSHARP_EXPORT int CSHARP_CALL HookCvarChange(const char* name, CSharpCvarCallback callback);
CSHARP_EXPORT bool CSHARP_CALL UnhookCvarChange(int hookId);

// CVar bounds management
CSHARP_EXPORT bool CSHARP_CALL SetCvarBounds(const char* name, bool hasMin, float minValue, bool hasMax, float maxValue);
CSHARP_EXPORT bool CSHARP_CALL GetCvarBounds(const char* name, bool* hasMin, float* minValue, bool* hasMax, float* maxValue);

// Internal bridge management
namespace CSharpBridge
{
    // Internal callback storage
    struct CallbackInfo
    {
        CSharpCommandCallback commandCallback;
        CSharpMenuCallback menuCallback;
        ke::AString commandName;
        int commandType;
        int flags;
        ke::AString info;
        bool infoMultiLang;
        int amxForwardId;  // AMX forward ID for integration
    };

    // Event callback storage
    struct EventCallbackInfo
    {
        CSharpEventCallback eventCallback;
        ke::AString eventName;
        int eventId;
        int flags;
        ke::AString conditions;
        int eventHandle;
        int amxForwardId;
    };

    // Forward callback storage
    struct ForwardCallbackInfo
    {
        CSharpForwardCallback forwardCallback;
        ke::AString forwardName;
        int forwardId;
        int execType;
        ke::Vector<int> paramTypes;
        int amxForwardId;
    };

    // Message callback storage
    struct MessageCallbackInfo
    {
        CSharpMessageCallback messageCallback;
        int msgId;
        int msgHandle;
        int amxForwardId;
    };

    // CVar callback storage
    struct CvarCallbackInfo
    {
        CSharpCvarCallback cvarCallback;
        ke::AString cvarName;
        int hookId;
        int amxForwardId;
    };

    // Bridge state management
    void Initialize();
    void Cleanup();
    
    // Command registration helpers
    int RegisterCommand(const char* command, CSharpCommandCallback callback, 
                       int flags, const char* info, bool infoMultiLang, 
                       CSharpCommandType type);
    
    // Internal callback handlers
    void HandleCommandCallback(int clientId, int commandId, int flags);
    void HandleMenuCallback(int clientId, int menuId, int key);
    
    // Storage management
    extern ke::Vector<CallbackInfo*> g_commandCallbacks;
    extern ke::Vector<ke::AString> g_menuIds;
    extern ke::Vector<EventCallbackInfo*> g_eventCallbacks;
    extern ke::Vector<ForwardCallbackInfo*> g_forwardCallbacks;
    extern ke::Vector<MessageCallbackInfo*> g_messageCallbacks;
    extern ke::Vector<CvarCallbackInfo*> g_cvarCallbacks;
    extern int g_nextCommandId;
    extern int g_nextMenuId;
    extern int g_nextEventHandle;
    extern int g_nextForwardId;
    extern int g_nextMessageHandle;
    extern int g_nextCvarHookId;
    extern bool g_initialized;

    // Event system helpers
    int RegisterEventInternal(const char* eventName, CSharpEventCallback callback,
                             int flags, const char* conditions);
    void HandleEventCallback(int eventId, int clientId, int numParams);

    // Forward system helpers
    int CreateForwardInternal(const char* forwardName, int execType,
                             const int* paramTypes, int numParams);
    int HandleForwardCallback(int forwardId, int numParams);

    // Message system helpers
    int RegisterMessageInternal(int msgId, CSharpMessageCallback callback);
    void HandleMessageCallback(int msgType, int msgDest, int entityId);

    // CVar system helpers
    int HookCvarChangeInternal(const char* name, CSharpCvarCallback callback);
    void HandleCvarCallback(const char* cvarName, const char* oldValue, const char* newValue);
}

#endif // CSHARP_BRIDGE_H
