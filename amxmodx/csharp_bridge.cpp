// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

// csharp_bridge.cpp - C# Bridge Implementation for AMX Mod X Command Registration Interface

#include "csharp_bridge.h"
#include "CCmd.h"
#include "CMenu.h"
#include <cstring>

// Cross-platform threading support
#ifdef _WIN32
    #include <windows.h>
    static CRITICAL_SECTION g_criticalSection;
    #define LOCK_INIT() InitializeCriticalSection(&g_criticalSection)
    #define LOCK_DESTROY() DeleteCriticalSection(&g_criticalSection)
    #define LOCK_ENTER() EnterCriticalSection(&g_criticalSection)
    #define LOCK_LEAVE() LeaveCriticalSection(&g_criticalSection)
#else
    #include <pthread.h>
    static pthread_mutex_t g_mutex = PTHREAD_MUTEX_INITIALIZER;
    #define LOCK_INIT() pthread_mutex_init(&g_mutex, NULL)
    #define LOCK_DESTROY() pthread_mutex_destroy(&g_mutex)
    #define LOCK_ENTER() pthread_mutex_lock(&g_mutex)
    #define LOCK_LEAVE() pthread_mutex_unlock(&g_mutex)
#endif

namespace CSharpBridge
{
    // Global storage
    ke::Vector<CallbackInfo*> g_commandCallbacks;
    ke::Vector<ke::AString> g_menuIds;
    ke::Vector<EventCallbackInfo*> g_eventCallbacks;
    ke::Vector<ForwardCallbackInfo*> g_forwardCallbacks;
    ke::Vector<MessageCallbackInfo*> g_messageCallbacks;
    ke::Vector<CvarCallbackInfo*> g_cvarCallbacks;
    ke::Vector<MenuCallbackInfo*> g_menuCallbacks;
    ke::Vector<GameConfigInfo*> g_gameConfigs;
    ke::Vector<NativeCallbackInfo*> g_nativeCallbacks;
    ke::Vector<DataPackInfo*> g_dataPacks;
    ke::Vector<LogCallbackInfo*> g_logCallbacks;
    ke::Vector<ForwardCallbackInfo*> g_forwardCallbacks;
    CallFuncInfo g_callFuncInfo = {nullptr, -1, 0, false};
    int g_nextCommandId = 1;
    int g_nextMenuId = 1;
    int g_nextEventHandle = 1;
    int g_nextForwardId = 1;
    int g_nextMessageHandle = 1;
    int g_nextCvarHookId = 1;
    int g_nextGameConfigId = 1;
    int g_nextNativeId = 1;
    int g_nextDataPackId = 1;
    int g_nextLogCallbackId = 1;
    bool g_initialized = false;

    // Current event context for parameter reading
    static int g_currentEventId = -1;
    static int g_currentEventParams = 0;
    static CSharpEventParam g_eventParamBuffer[32]; // Max 32 parameters

    // Thread-safe helper class
    class AutoLock
    {
    public:
        AutoLock() { LOCK_ENTER(); }
        ~AutoLock() { LOCK_LEAVE(); }
    };

    // AMX forward callback for console commands
    static cell AMX_NATIVE_CALL CSharpConsoleCommandHandler(AMX *amx, cell *params)
    {
        int clientId = params[1];
        int flags = params[2];
        int commandId = params[3];
        
        HandleCommandCallback(clientId, commandId, flags);
        return 1;
    }

    // AMX forward callback for client commands
    static cell AMX_NATIVE_CALL CSharpClientCommandHandler(AMX *amx, cell *params)
    {
        int clientId = params[1];
        int flags = params[2];
        int commandId = params[3];
        
        HandleCommandCallback(clientId, commandId, flags);
        return 1;
    }

    // AMX forward callback for server commands
    static cell AMX_NATIVE_CALL CSharpServerCommandHandler(AMX *amx, cell *params)
    {
        int clientId = params[1];
        int flags = params[2];
        int commandId = params[3];
        
        HandleCommandCallback(clientId, commandId, flags);
        return 1;
    }

    // AMX forward callback for menu commands
    static cell AMX_NATIVE_CALL CSharpMenuCommandHandler(AMX *amx, cell *params)
    {
        int clientId = params[1];
        int menuId = params[2];
        int key = params[3];

        HandleMenuCallback(clientId, menuId, key);
        return 1;
    }

    // AMX forward callback for events
    static cell AMX_NATIVE_CALL CSharpEventHandler(AMX *amx, cell *params)
    {
        int eventId = params[1];
        int clientId = params[2];
        int numParams = params[3];

        // Store event parameters for reading
        g_currentEventId = eventId;
        g_currentEventParams = numParams;

        // Copy event parameters from AMX
        for (int i = 0; i < numParams && i < 32; i++)
        {
            cell* paramPtr = get_amxaddr(amx, params[4 + i]);
            if (paramPtr)
            {
                // Determine parameter type and store
                if (*paramPtr >= -2147483648 && *paramPtr <= 2147483647)
                {
                    g_eventParamBuffer[i].type = 0; // int
                    g_eventParamBuffer[i].intValue = *paramPtr;
                }
                else
                {
                    g_eventParamBuffer[i].type = 1; // float
                    g_eventParamBuffer[i].floatValue = amx_ctof(*paramPtr);
                }
            }
        }

        HandleEventCallback(eventId, clientId, numParams);
        return 1;
    }

    // AMX forward callback for custom forwards
    static cell AMX_NATIVE_CALL CSharpForwardHandler(AMX *amx, cell *params)
    {
        int forwardId = params[1];
        int numParams = params[2];

        return HandleForwardCallback(forwardId, numParams);
    }

    void Initialize()
    {
        if (g_initialized)
            return;

        LOCK_INIT();
        g_commandCallbacks.clear();
        g_menuIds.clear();
        g_nextCommandId = 1;
        g_nextMenuId = 1;
        g_initialized = true;
    }

    void Cleanup()
    {
        if (!g_initialized)
            return;

        AutoLock lock;
        
        // Clean up command callbacks
        for (size_t i = 0; i < g_commandCallbacks.length(); i++)
        {
            delete g_commandCallbacks[i];
        }
        g_commandCallbacks.clear();
        g_menuIds.clear();

        // Clean up event callbacks
        for (size_t i = 0; i < g_eventCallbacks.length(); i++)
        {
            delete g_eventCallbacks[i];
        }
        g_eventCallbacks.clear();

        // Clean up forward callbacks
        for (size_t i = 0; i < g_forwardCallbacks.length(); i++)
        {
            delete g_forwardCallbacks[i];
        }
        g_forwardCallbacks.clear();

        // Clean up message callbacks
        for (size_t i = 0; i < g_messageCallbacks.length(); i++)
        {
            delete g_messageCallbacks[i];
        }
        g_messageCallbacks.clear();

        // Clean up CVar callbacks
        for (size_t i = 0; i < g_cvarCallbacks.length(); i++)
        {
            delete g_cvarCallbacks[i];
        }
        g_cvarCallbacks.clear();

        // Clean up menu callbacks
        for (size_t i = 0; i < g_menuCallbacks.length(); i++)
        {
            delete g_menuCallbacks[i];
        }
        g_menuCallbacks.clear();

        // Clean up game configs
        for (size_t i = 0; i < g_gameConfigs.length(); i++)
        {
            delete g_gameConfigs[i];
        }
        g_gameConfigs.clear();

        // Clean up native callbacks
        for (size_t i = 0; i < g_nativeCallbacks.length(); i++)
        {
            delete g_nativeCallbacks[i];
        }
        g_nativeCallbacks.clear();

        // Clean up data packs
        for (size_t i = 0; i < g_dataPacks.length(); i++)
        {
            delete g_dataPacks[i];
        }
        g_dataPacks.clear();

        // Clean up log callbacks
        for (size_t i = 0; i < g_logCallbacks.length(); i++)
        {
            delete g_logCallbacks[i];
        }
        g_logCallbacks.clear();

        // Clean up forward callbacks
        for (size_t i = 0; i < g_forwardCallbacks.length(); i++)
        {
            delete g_forwardCallbacks[i];
        }
        g_forwardCallbacks.clear();

        // Clean up call func info
        CleanupCallFunc();

        g_initialized = false;
        LOCK_DESTROY();
    }

    int RegisterCommand(const char* command, CSharpCommandCallback callback, 
                       int flags, const char* info, bool infoMultiLang, 
                       CSharpCommandType type)
    {
        if (!g_initialized || !command || !callback)
            return -1;

        AutoLock lock;

        int commandId = g_nextCommandId++;
        
        CallbackInfo* callbackInfo = new CallbackInfo();
        callbackInfo->commandCallback = callback;
        callbackInfo->menuCallback = nullptr;
        callbackInfo->commandName = command;
        callbackInfo->commandType = type;
        callbackInfo->flags = flags;
        callbackInfo->info = info ? info : "";
        callbackInfo->infoMultiLang = infoMultiLang;
        callbackInfo->amxForwardId = -1;

        // Create AMX forward based on command type
        const char* handlerName = nullptr;
        switch (type)
        {
            case CSHARP_COMMAND_TYPE_CONSOLE:
                handlerName = "CSharpConsoleCommandHandler";
                break;
            case CSHARP_COMMAND_TYPE_CLIENT:
                handlerName = "CSharpClientCommandHandler";
                break;
            case CSHARP_COMMAND_TYPE_SERVER:
                handlerName = "CSharpServerCommandHandler";
                break;
        }

        if (handlerName)
        {
            // Register the command with AMX Mod X command system
            // This integrates with the existing command infrastructure
            CmdMngr::Command* cmd = nullptr;
            
            // Find a plugin to register the command with (use first available)
            CPluginMngr::CPlugin* plugin = nullptr;
            for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
            {
                if ((*iter).isValid())
                {
                    plugin = &(*iter);
                    break;
                }
            }

            if (plugin)
            {
                // Create a forward for this command
                int forwardId = registerSPForwardByName(plugin->getAMX(), handlerName, 
                                                       FP_CELL, FP_CELL, FP_CELL, FP_DONE);
                if (forwardId != -1)
                {
                    callbackInfo->amxForwardId = forwardId;
                    
                    // Register with command manager
                    bool listable = (flags >= 0);
                    cmd = g_commands.registerCommand(plugin, forwardId, command, 
                                                   callbackInfo->info.chars(), 
                                                   flags < 0 ? 0 : flags, 
                                                   listable, infoMultiLang);
                    
                    if (cmd)
                    {
                        switch (type)
                        {
                            case CSHARP_COMMAND_TYPE_CONSOLE:
                                cmd->setCmdType(CMD_ConsoleCommand);
                                REG_SVR_COMMAND((char*)cmd->getCommand(), plugin_srvcmd);
                                break;
                            case CSHARP_COMMAND_TYPE_CLIENT:
                                cmd->setCmdType(CMD_ClientCommand);
                                break;
                            case CSHARP_COMMAND_TYPE_SERVER:
                                cmd->setCmdType(CMD_ServerCommand);
                                REG_SVR_COMMAND((char*)cmd->getCommand(), plugin_srvcmd);
                                break;
                        }
                    }
                }
            }
        }

        g_commandCallbacks.append(callbackInfo);
        return commandId;
    }

    void HandleCommandCallback(int clientId, int commandId, int flags)
    {
        AutoLock lock;
        
        // Find the callback by command ID
        for (size_t i = 0; i < g_commandCallbacks.length(); i++)
        {
            CallbackInfo* info = g_commandCallbacks[i];
            if (info && info->commandCallback && (i + 1) == commandId)
            {
                info->commandCallback(clientId, commandId, flags);
                break;
            }
        }
    }

    void HandleMenuCallback(int clientId, int menuId, int key)
    {
        AutoLock lock;

        // Find the callback by menu ID
        for (size_t i = 0; i < g_commandCallbacks.length(); i++)
        {
            CallbackInfo* info = g_commandCallbacks[i];
            if (info && info->menuCallback && info->commandType == -1) // Menu type
            {
                info->menuCallback(clientId, menuId, key);
                break;
            }
        }
    }

    void HandleEventCallback(int eventId, int clientId, int numParams)
    {
        AutoLock lock;

        // Find the callback by event ID
        for (size_t i = 0; i < g_eventCallbacks.length(); i++)
        {
            EventCallbackInfo* info = g_eventCallbacks[i];
            if (info && info->eventCallback && info->eventId == eventId)
            {
                info->eventCallback(eventId, clientId, numParams);
                break;
            }
        }
    }

    int HandleForwardCallback(int forwardId, int numParams)
    {
        AutoLock lock;

        // Find the callback by forward ID
        for (size_t i = 0; i < g_forwardCallbacks.length(); i++)
        {
            ForwardCallbackInfo* info = g_forwardCallbacks[i];
            if (info && info->forwardCallback && info->forwardId == forwardId)
            {
                return info->forwardCallback(forwardId, numParams);
            }
        }

        return 0; // Default return value
    }

    int RegisterEventInternal(const char* eventName, CSharpEventCallback callback,
                             int flags, const char* conditions)
    {
        if (!g_initialized || !eventName || !callback)
            return -1;

        AutoLock lock;

        int eventHandle = g_nextEventHandle++;

        EventCallbackInfo* callbackInfo = new EventCallbackInfo();
        callbackInfo->eventCallback = callback;
        callbackInfo->eventName = eventName;
        callbackInfo->eventId = -1; // Will be set when event is found
        callbackInfo->flags = flags;
        callbackInfo->conditions = conditions ? conditions : "";
        callbackInfo->eventHandle = eventHandle;
        callbackInfo->amxForwardId = -1;

        // Register with AMX event system
        // This would integrate with the existing event infrastructure
        CPluginMngr::CPlugin* plugin = nullptr;
        for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
        {
            if ((*iter).isValid())
            {
                plugin = &(*iter);
                break;
            }
        }

        if (plugin)
        {
            // Create a forward for this event
            int forwardId = registerSPForwardByName(plugin->getAMX(), "CSharpEventHandler",
                                                   FP_CELL, FP_CELL, FP_CELL, FP_DONE);
            if (forwardId != -1)
            {
                callbackInfo->amxForwardId = forwardId;

                // Register with event manager (simplified - actual implementation would vary)
                // This would typically involve registering with the game event system
                callbackInfo->eventId = g_nextEventHandle; // Simplified ID assignment
            }
        }

        g_eventCallbacks.append(callbackInfo);
        return eventHandle;
    }
}

// Exported C functions for C# interop
CSHARP_EXPORT void CSHARP_CALL InitializeCSharpBridge()
{
    CSharpBridge::Initialize();
}

CSHARP_EXPORT void CSHARP_CALL CleanupCSharpBridge()
{
    CSharpBridge::Cleanup();
}

CSHARP_EXPORT int CSHARP_CALL RegisterConsoleCommand(
    const char* command, 
    CSharpCommandCallback callback, 
    int flags, 
    const char* info, 
    bool infoMultiLang)
{
    return CSharpBridge::RegisterCommand(command, callback, flags, info, 
                                        infoMultiLang, CSHARP_COMMAND_TYPE_CONSOLE);
}

CSHARP_EXPORT int CSHARP_CALL RegisterClientCommand(
    const char* command, 
    CSharpCommandCallback callback, 
    int flags, 
    const char* info, 
    bool infoMultiLang)
{
    return CSharpBridge::RegisterCommand(command, callback, flags, info, 
                                        infoMultiLang, CSHARP_COMMAND_TYPE_CLIENT);
}

CSHARP_EXPORT int CSHARP_CALL RegisterServerCommand(
    const char* command,
    CSharpCommandCallback callback,
    int flags,
    const char* info,
    bool infoMultiLang)
{
    return CSharpBridge::RegisterCommand(command, callback, flags, info,
                                        infoMultiLang, CSHARP_COMMAND_TYPE_SERVER);
}

CSHARP_EXPORT int CSHARP_CALL RegisterMenuCommand(
    int menuId,
    int keyMask,
    CSharpMenuCallback callback)
{
    if (!CSharpBridge::g_initialized || !callback)
        return -1;

    CSharpBridge::AutoLock lock;

    int commandId = CSharpBridge::g_nextCommandId++;

    CSharpBridge::CallbackInfo* callbackInfo = new CSharpBridge::CallbackInfo();
    callbackInfo->commandCallback = nullptr;
    callbackInfo->menuCallback = callback;
    callbackInfo->commandName = "";
    callbackInfo->commandType = -1; // Menu type
    callbackInfo->flags = keyMask;
    callbackInfo->info = "";
    callbackInfo->infoMultiLang = false;
    callbackInfo->amxForwardId = -1;

    // Register with menu system
    CPluginMngr::CPlugin* plugin = nullptr;
    for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
    {
        if ((*iter).isValid())
        {
            plugin = &(*iter);
            break;
        }
    }

    if (plugin)
    {
        int forwardId = registerSPForwardByName(plugin->getAMX(), "CSharpMenuCommandHandler",
                                               FP_CELL, FP_CELL, FP_CELL, FP_DONE);
        if (forwardId != -1)
        {
            callbackInfo->amxForwardId = forwardId;
            g_menucmds.registerMenuCmd(plugin, menuId, keyMask, forwardId);
        }
    }

    CSharpBridge::g_commandCallbacks.append(callbackInfo);
    return commandId;
}

CSHARP_EXPORT int CSHARP_CALL RegisterMenuId(
    const char* menuName,
    bool global)
{
    if (!CSharpBridge::g_initialized || !menuName)
        return -1;

    CSharpBridge::AutoLock lock;

    int menuId = CSharpBridge::g_nextMenuId++;
    CSharpBridge::g_menuIds.append(ke::AString(menuName));

    // Register with menu ID system
    AMX* amx = global ? nullptr : (g_plugins.begin() ? (*g_plugins.begin()).getAMX() : nullptr);
    int registeredId = g_menucmds.registerMenuId(menuName, amx);

    return registeredId != 0 ? menuId : -1;
}

CSHARP_EXPORT bool CSHARP_CALL GetCommandInfo(
    int commandId,
    CSharpCommandType commandType,
    CSharpCommandInfo* outInfo)
{
    if (!CSharpBridge::g_initialized || !outInfo || commandId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    size_t index = commandId - 1;
    if (index >= CSharpBridge::g_commandCallbacks.length())
        return false;

    CSharpBridge::CallbackInfo* info = CSharpBridge::g_commandCallbacks[index];
    if (!info || info->commandType != commandType)
        return false;

    // Copy information to output structure
    strncpy(outInfo->command, info->commandName.chars(), sizeof(outInfo->command) - 1);
    outInfo->command[sizeof(outInfo->command) - 1] = '\0';

    strncpy(outInfo->info, info->info.chars(), sizeof(outInfo->info) - 1);
    outInfo->info[sizeof(outInfo->info) - 1] = '\0';

    outInfo->flags = info->flags;
    outInfo->commandId = commandId;
    outInfo->infoMultiLang = info->infoMultiLang;
    outInfo->listable = (info->flags >= 0);

    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetCommandCount(
    CSharpCommandType commandType,
    int accessFlags)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    CSharpBridge::AutoLock lock;

    int count = 0;
    for (size_t i = 0; i < CSharpBridge::g_commandCallbacks.length(); i++)
    {
        CSharpBridge::CallbackInfo* info = CSharpBridge::g_commandCallbacks[i];
        if (info && info->commandType == commandType)
        {
            if (accessFlags == -1 || (info->flags & accessFlags))
                count++;
        }
    }

    return count;
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterCommand(int commandId)
{
    if (!CSharpBridge::g_initialized || commandId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    size_t index = commandId - 1;
    if (index >= CSharpBridge::g_commandCallbacks.length())
        return false;

    CSharpBridge::CallbackInfo* info = CSharpBridge::g_commandCallbacks[index];
    if (!info)
        return false;

    // Unregister from AMX forward system
    if (info->amxForwardId != -1)
    {
        unregisterSPForward(info->amxForwardId);
    }

    // Clean up
    delete info;
    CSharpBridge::g_commandCallbacks[index] = nullptr;

    return true;
}

// Command execution functions implementation
CSHARP_EXPORT void CSHARP_CALL ExecuteServerCommand(const char* command)
{
    if (!command || !CSharpBridge::g_initialized)
        return;

    // Create a copy of the command and add newline
    size_t len = strlen(command);
    char* cmd = new char[len + 2];
    strcpy(cmd, command);
    cmd[len] = '\n';
    cmd[len + 1] = '\0';

    // Execute server command
    SERVER_COMMAND(cmd);

    delete[] cmd;
}

CSHARP_EXPORT void CSHARP_CALL ExecuteClientCommand(int clientId, const char* command)
{
    if (!command || !CSharpBridge::g_initialized)
        return;

    if (clientId < 0 || clientId > gpGlobals->maxClients)
        return;

    // Create a copy of the command and add newline
    size_t len = strlen(command);
    char* cmd = new char[len + 2];
    strcpy(cmd, command);
    cmd[len] = '\n';
    cmd[len + 1] = '\0';

    if (clientId == 0)
    {
        // Send to all clients
        for (int i = 1; i <= gpGlobals->maxClients; ++i)
        {
            CPlayer* pPlayer = GET_PLAYER_POINTER_I(i);
            if (!pPlayer->IsBot() && pPlayer->initialized)
                CLIENT_COMMAND(pPlayer->pEdict, "%s", cmd);
        }
    }
    else
    {
        // Send to specific client
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
        if (!pPlayer->IsBot() && pPlayer->initialized)
            CLIENT_COMMAND(pPlayer->pEdict, "%s", cmd);
    }

    delete[] cmd;
}

CSHARP_EXPORT void CSHARP_CALL ExecuteConsoleCommand(int clientId, const char* command)
{
    if (!command || !CSharpBridge::g_initialized)
        return;

    // Create a copy of the command and add newline
    size_t len = strlen(command);
    char* cmd = new char[len + 2];
    strcpy(cmd, command);
    cmd[len] = '\n';
    cmd[len + 1] = '\0';

    if (clientId < 1 || clientId > gpGlobals->maxClients)
    {
        // Execute as server command
        SERVER_COMMAND(cmd);
    }
    else
    {
        // Execute as client console command
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
        if (!pPlayer->IsBot() && pPlayer->initialized)
            CLIENT_COMMAND(pPlayer->pEdict, "%s", cmd);
    }

    delete[] cmd;
}

// Command argument reading functions implementation
CSHARP_EXPORT int CSHARP_CALL GetCommandArgCount()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return CMD_ARGC();
}

CSHARP_EXPORT bool CSHARP_CALL GetCommandArg(int index, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    if (index < 0)
        return false;

    const char* arg = CMD_ARGV(index);
    if (!arg)
    {
        buffer[0] = '\0';
        return false;
    }

    // Copy argument to buffer with bounds checking
    size_t argLen = strlen(arg);
    size_t copyLen = (argLen < (size_t)(bufferSize - 1)) ? argLen : (bufferSize - 1);

    strncpy(buffer, arg, copyLen);
    buffer[copyLen] = '\0';

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL GetCommandArgs(char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    const char* args = CMD_ARGS();
    if (!args)
    {
        buffer[0] = '\0';
        return false;
    }

    // Copy arguments to buffer with bounds checking
    size_t argsLen = strlen(args);
    size_t copyLen = (argsLen < (size_t)(bufferSize - 1)) ? argsLen : (bufferSize - 1);

    strncpy(buffer, args, copyLen);
    buffer[copyLen] = '\0';

    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetCommandArgInt(int index)
{
    if (!CSharpBridge::g_initialized || index < 0)
        return 0;

    const char* arg = CMD_ARGV(index);
    if (!arg)
        return 0;

    return atoi(arg);
}

CSHARP_EXPORT float CSHARP_CALL GetCommandArgFloat(int index)
{
    if (!CSharpBridge::g_initialized || index < 0)
        return 0.0f;

    const char* arg = CMD_ARGV(index);
    if (!arg)
        return 0.0f;

    return (float)atof(arg);
}

// Command query functions implementation
CSHARP_EXPORT bool CSHARP_CALL FindCommand(const char* commandName, CSharpCommandType commandType, CSharpCommandInfo* outInfo)
{
    if (!commandName || !outInfo || !CSharpBridge::g_initialized)
        return false;

    // Convert C# command type to AMX command type
    int amxCmdType;
    switch (commandType)
    {
        case CSHARP_COMMAND_TYPE_CONSOLE:
            amxCmdType = CMD_ConsoleCommand;
            break;
        case CSHARP_COMMAND_TYPE_CLIENT:
            amxCmdType = CMD_ClientCommand;
            break;
        case CSHARP_COMMAND_TYPE_SERVER:
            amxCmdType = CMD_ServerCommand;
            break;
        default:
            return false;
    }

    // Search for command in AMX command manager
    CmdMngr::iterator iter;
    switch (commandType)
    {
        case CSHARP_COMMAND_TYPE_CONSOLE:
            iter = g_commands.concmdbegin();
            break;
        case CSHARP_COMMAND_TYPE_CLIENT:
            iter = g_commands.clcmdbegin();
            break;
        case CSHARP_COMMAND_TYPE_SERVER:
            iter = g_commands.srvcmdbegin();
            break;
        default:
            return false;
    }

    while (iter)
    {
        if ((*iter).matchCommand(commandName))
        {
            // Found the command, fill output structure
            const CmdMngr::Command& cmd = *iter;

            strncpy(outInfo->command, cmd.getCmdLine(), sizeof(outInfo->command) - 1);
            outInfo->command[sizeof(outInfo->command) - 1] = '\0';

            strncpy(outInfo->info, cmd.getCmdInfo(), sizeof(outInfo->info) - 1);
            outInfo->info[sizeof(outInfo->info) - 1] = '\0';

            outInfo->flags = cmd.getFlags();
            outInfo->commandId = cmd.getId();
            outInfo->infoMultiLang = cmd.isInfoML();
            outInfo->listable = true; // Commands found in manager are listable

            return true;
        }
        ++iter;
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL GetCommandsCount(CSharpCommandType commandType, int accessFlags)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    // Convert C# command type to AMX command type
    int amxCmdType;
    switch (commandType)
    {
        case CSHARP_COMMAND_TYPE_CONSOLE:
            amxCmdType = CMD_ConsoleCommand;
            break;
        case CSHARP_COMMAND_TYPE_CLIENT:
            amxCmdType = CMD_ClientCommand;
            break;
        case CSHARP_COMMAND_TYPE_SERVER:
            amxCmdType = CMD_ServerCommand;
            break;
        default:
            return 0;
    }

    return g_commands.getCmdNum(amxCmdType, accessFlags);
}

CSHARP_EXPORT bool CSHARP_CALL GetCommandByIndex(int index, CSharpCommandType commandType, int accessFlags, CSharpCommandInfo* outInfo)
{
    if (!outInfo || !CSharpBridge::g_initialized || index < 0)
        return false;

    // Convert C# command type to AMX command type
    int amxCmdType;
    switch (commandType)
    {
        case CSHARP_COMMAND_TYPE_CONSOLE:
            amxCmdType = CMD_ConsoleCommand;
            break;
        case CSHARP_COMMAND_TYPE_CLIENT:
            amxCmdType = CMD_ClientCommand;
            break;
        case CSHARP_COMMAND_TYPE_SERVER:
            amxCmdType = CMD_ServerCommand;
            break;
        default:
            return false;
    }

    // Get command by index
    CmdMngr::Command* cmd = g_commands.getCmd(index, amxCmdType, accessFlags);
    if (!cmd)
        return false;

    // Fill output structure
    strncpy(outInfo->command, cmd->getCmdLine(), sizeof(outInfo->command) - 1);
    outInfo->command[sizeof(outInfo->command) - 1] = '\0';

    strncpy(outInfo->info, cmd->getCmdInfo(), sizeof(outInfo->info) - 1);
    outInfo->info[sizeof(outInfo->info) - 1] = '\0';

    outInfo->flags = cmd->getFlags();
    outInfo->commandId = cmd->getId();
    outInfo->infoMultiLang = cmd->isInfoML();
    outInfo->listable = true;

    return true;
}

// Event system functions implementation
CSHARP_EXPORT int CSHARP_CALL RegisterEvent(const char* eventName, CSharpEventCallback callback, int flags, const char* conditions)
{
    return CSharpBridge::RegisterEventInternal(eventName, callback, flags, conditions);
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterEvent(int eventHandle)
{
    if (!CSharpBridge::g_initialized || eventHandle <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove the event callback
    for (size_t i = 0; i < CSharpBridge::g_eventCallbacks.length(); i++)
    {
        CSharpBridge::EventCallbackInfo* info = CSharpBridge::g_eventCallbacks[i];
        if (info && info->eventHandle == eventHandle)
        {
            // Unregister from AMX forward system
            if (info->amxForwardId != -1)
            {
                unregisterSPForward(info->amxForwardId);
            }

            delete info;
            CSharpBridge::g_eventCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL GetEventId(const char* eventName)
{
    if (!eventName || !CSharpBridge::g_initialized)
        return -1;

    // Search for event by name in registered events
    CSharpBridge::AutoLock lock;

    for (size_t i = 0; i < CSharpBridge::g_eventCallbacks.length(); i++)
    {
        CSharpBridge::EventCallbackInfo* info = CSharpBridge::g_eventCallbacks[i];
        if (info && strcmp(info->eventName.chars(), eventName) == 0)
        {
            return info->eventId;
        }
    }

    return -1;
}

CSHARP_EXPORT bool CSHARP_CALL GetEventInfo(int eventId, CSharpEventInfo* outInfo)
{
    if (!outInfo || !CSharpBridge::g_initialized || eventId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find event by ID
    for (size_t i = 0; i < CSharpBridge::g_eventCallbacks.length(); i++)
    {
        CSharpBridge::EventCallbackInfo* info = CSharpBridge::g_eventCallbacks[i];
        if (info && info->eventId == eventId)
        {
            strncpy(outInfo->eventName, info->eventName.chars(), sizeof(outInfo->eventName) - 1);
            outInfo->eventName[sizeof(outInfo->eventName) - 1] = '\0';

            outInfo->eventId = info->eventId;
            outInfo->flags = info->flags;
            outInfo->numParams = 0; // Would be determined by event type
            outInfo->isActive = (info->amxForwardId != -1);

            return true;
        }
    }

    return false;
}

// Event parameter reading functions implementation
CSHARP_EXPORT int CSHARP_CALL GetEventArgCount()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return CSharpBridge::g_currentEventParams;
}

CSHARP_EXPORT bool CSHARP_CALL GetEventArg(int index, CSharpEventParam* outParam)
{
    if (!outParam || !CSharpBridge::g_initialized || index < 0 || index >= CSharpBridge::g_currentEventParams)
        return false;

    if (index >= 32) // Buffer limit
        return false;

    *outParam = CSharpBridge::g_eventParamBuffer[index];
    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetEventArgInt(int index)
{
    if (!CSharpBridge::g_initialized || index < 0 || index >= CSharpBridge::g_currentEventParams || index >= 32)
        return 0;

    return CSharpBridge::g_eventParamBuffer[index].intValue;
}

CSHARP_EXPORT float CSHARP_CALL GetEventArgFloat(int index)
{
    if (!CSharpBridge::g_initialized || index < 0 || index >= CSharpBridge::g_currentEventParams || index >= 32)
        return 0.0f;

    if (CSharpBridge::g_eventParamBuffer[index].type == 1)
        return CSharpBridge::g_eventParamBuffer[index].floatValue;
    else
        return (float)CSharpBridge::g_eventParamBuffer[index].intValue;
}

CSHARP_EXPORT bool CSHARP_CALL GetEventArgString(int index, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized ||
        index < 0 || index >= CSharpBridge::g_currentEventParams || index >= 32)
        return false;

    if (CSharpBridge::g_eventParamBuffer[index].type == 2)
    {
        strncpy(buffer, CSharpBridge::g_eventParamBuffer[index].stringValue, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
        return true;
    }
    else
    {
        // Convert number to string
        if (CSharpBridge::g_eventParamBuffer[index].type == 0)
        {
            snprintf(buffer, bufferSize, "%d", CSharpBridge::g_eventParamBuffer[index].intValue);
        }
        else
        {
            snprintf(buffer, bufferSize, "%.2f", CSharpBridge::g_eventParamBuffer[index].floatValue);
        }
        return true;
    }
}

// Forward system functions implementation
CSHARP_EXPORT int CSHARP_CALL CreateForward(const char* forwardName, int execType, const int* paramTypes, int numParams)
{
    if (!forwardName || !CSharpBridge::g_initialized || numParams < 0 || numParams > 16)
        return -1;

    CSharpBridge::AutoLock lock;

    int forwardId = CSharpBridge::g_nextForwardId++;

    CSharpBridge::ForwardCallbackInfo* callbackInfo = new CSharpBridge::ForwardCallbackInfo();
    callbackInfo->forwardCallback = nullptr; // Global forward, no specific callback
    callbackInfo->forwardName = forwardName;
    callbackInfo->forwardId = forwardId;
    callbackInfo->execType = execType;
    callbackInfo->amxForwardId = -1;

    // Copy parameter types
    for (int i = 0; i < numParams; i++)
    {
        callbackInfo->paramTypes.append(paramTypes[i]);
    }

    // Create AMX forward
    ForwardParam amxParams[16];
    for (int i = 0; i < numParams; i++)
    {
        amxParams[i] = static_cast<ForwardParam>(paramTypes[i]);
    }

    int amxForwardId = registerForwardC(forwardName, static_cast<ForwardExecType>(execType),
                                       reinterpret_cast<cell*>(amxParams), numParams);

    if (amxForwardId != -1)
    {
        callbackInfo->amxForwardId = amxForwardId;
    }

    CSharpBridge::g_forwardCallbacks.append(callbackInfo);
    return forwardId;
}

CSHARP_EXPORT int CSHARP_CALL CreateSingleForward(const char* functionName, CSharpForwardCallback callback, const int* paramTypes, int numParams)
{
    if (!functionName || !callback || !CSharpBridge::g_initialized || numParams < 0 || numParams > 16)
        return -1;

    CSharpBridge::AutoLock lock;

    int forwardId = CSharpBridge::g_nextForwardId++;

    CSharpBridge::ForwardCallbackInfo* callbackInfo = new CSharpBridge::ForwardCallbackInfo();
    callbackInfo->forwardCallback = callback;
    callbackInfo->forwardName = functionName;
    callbackInfo->forwardId = forwardId;
    callbackInfo->execType = 0; // Single forward
    callbackInfo->amxForwardId = -1;

    // Copy parameter types
    for (int i = 0; i < numParams; i++)
    {
        callbackInfo->paramTypes.append(paramTypes[i]);
    }

    // Create single plugin forward (simplified - would need actual plugin context)
    CPluginMngr::CPlugin* plugin = nullptr;
    for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
    {
        if ((*iter).isValid())
        {
            plugin = &(*iter);
            break;
        }
    }

    if (plugin)
    {
        ForwardParam amxParams[16];
        for (int i = 0; i < numParams; i++)
        {
            amxParams[i] = static_cast<ForwardParam>(paramTypes[i]);
        }

        int amxForwardId = registerSPForwardByNameC(plugin->getAMX(), functionName,
                                                   reinterpret_cast<cell*>(amxParams), numParams);

        if (amxForwardId != -1)
        {
            callbackInfo->amxForwardId = amxForwardId;
        }
    }

    CSharpBridge::g_forwardCallbacks.append(callbackInfo);
    return forwardId;
}

CSHARP_EXPORT bool CSHARP_CALL ExecuteForward(int forwardId, const CSharpEventParam* params, int numParams, int* outResult)
{
    if (!CSharpBridge::g_initialized || forwardId < 0 || numParams < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find forward by ID
    CSharpBridge::ForwardCallbackInfo* info = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_forwardCallbacks.length(); i++)
    {
        if (CSharpBridge::g_forwardCallbacks[i] && CSharpBridge::g_forwardCallbacks[i]->forwardId == forwardId)
        {
            info = CSharpBridge::g_forwardCallbacks[i];
            break;
        }
    }

    if (!info || info->amxForwardId == -1)
        return false;

    // Prepare parameters for AMX forward execution
    cell amxParams[16];
    for (int i = 0; i < numParams && i < 16; i++)
    {
        switch (params[i].type)
        {
            case 0: // int
                amxParams[i] = params[i].intValue;
                break;
            case 1: // float
                amxParams[i] = amx_ftoc(params[i].floatValue);
                break;
            case 2: // string
                // String handling would require more complex parameter preparation
                amxParams[i] = 0; // Simplified
                break;
            default:
                amxParams[i] = 0;
                break;
        }
    }

    // Execute the forward
    cell result = executeForwards(info->amxForwardId, amxParams[0], amxParams[1], amxParams[2],
                                 amxParams[3], amxParams[4], amxParams[5], amxParams[6], amxParams[7]);

    if (outResult)
        *outResult = result;

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterForward(int forwardId)
{
    if (!CSharpBridge::g_initialized || forwardId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove the forward callback
    for (size_t i = 0; i < CSharpBridge::g_forwardCallbacks.length(); i++)
    {
        CSharpBridge::ForwardCallbackInfo* info = CSharpBridge::g_forwardCallbacks[i];
        if (info && info->forwardId == forwardId)
        {
            // Unregister from AMX forward system
            if (info->amxForwardId != -1)
            {
                if (info->forwardCallback) // Single forward
                {
                    unregisterSPForward(info->amxForwardId);
                }
                // Global forwards are not unregistered individually
            }

            delete info;
            CSharpBridge::g_forwardCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetForwardInfo(int forwardId, CSharpForwardInfo* outInfo)
{
    if (!outInfo || !CSharpBridge::g_initialized || forwardId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find forward by ID
    for (size_t i = 0; i < CSharpBridge::g_forwardCallbacks.length(); i++)
    {
        CSharpBridge::ForwardCallbackInfo* info = CSharpBridge::g_forwardCallbacks[i];
        if (info && info->forwardId == forwardId)
        {
            strncpy(outInfo->forwardName, info->forwardName.chars(), sizeof(outInfo->forwardName) - 1);
            outInfo->forwardName[sizeof(outInfo->forwardName) - 1] = '\0';

            outInfo->forwardId = info->forwardId;
            outInfo->numParams = info->paramTypes.length();
            outInfo->execType = info->execType;
            outInfo->isValid = (info->amxForwardId != -1);

            return true;
        }
    }

    return false;
}

// Player information functions implementation
CSHARP_EXPORT bool CSHARP_CALL IsPlayerValid(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    return (clientId >= 1 && clientId <= gpGlobals->maxClients);
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerInfo(int clientId, CSharpPlayerInfo* outInfo)
{
    if (!outInfo || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    // Fill player information
    strncpy(outInfo->name, pPlayer->name.chars(), sizeof(outInfo->name) - 1);
    outInfo->name[sizeof(outInfo->name) - 1] = '\0';

    strncpy(outInfo->ip, pPlayer->ip.chars(), sizeof(outInfo->ip) - 1);
    outInfo->ip[sizeof(outInfo->ip) - 1] = '\0';

    const char* authId = GETPLAYERAUTHID(pPlayer->pEdict);
    if (authId)
    {
        strncpy(outInfo->authId, authId, sizeof(outInfo->authId) - 1);
        outInfo->authId[sizeof(outInfo->authId) - 1] = '\0';
    }
    else
    {
        outInfo->authId[0] = '\0';
    }

    strncpy(outInfo->team, pPlayer->team.chars(), sizeof(outInfo->team) - 1);
    outInfo->team[sizeof(outInfo->team) - 1] = '\0';

    outInfo->index = clientId;
    outInfo->teamId = pPlayer->teamId;
    outInfo->userId = GETPLAYERUSERID(pPlayer->pEdict);
    outInfo->flags = pPlayer->flags[0]; // First flag set
    outInfo->connectTime = pPlayer->time;
    outInfo->playTime = pPlayer->playtime;
    outInfo->isInGame = pPlayer->ingame;
    outInfo->isBot = pPlayer->IsBot();
    outInfo->isAlive = pPlayer->IsAlive();
    outInfo->isAuthorized = pPlayer->authorized;
    outInfo->isConnecting = (!pPlayer->ingame && pPlayer->initialized && (GETPLAYERUSERID(pPlayer->pEdict) > 0));
    outInfo->isHLTV = (pPlayer->pEdict->v.flags & FL_PROXY) ? true : false;
    outInfo->hasVGUI = pPlayer->vgui;

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerStats(int clientId, CSharpPlayerStats* outStats)
{
    if (!outStats || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    // Fill player statistics
    outStats->deaths = pPlayer->deaths;
    outStats->kills = 0; // Would need to be calculated from game state
    outStats->frags = pPlayer->pEdict->v.frags;
    outStats->currentWeapon = pPlayer->current;
    outStats->menu = pPlayer->menu;
    outStats->keys = pPlayer->keys;
    outStats->health = pPlayer->pEdict->v.health;
    outStats->armor = pPlayer->pEdict->v.armorvalue;
    outStats->aiming = pPlayer->aiming;
    outStats->menuExpire = pPlayer->menuexpire;

    // Copy weapon data
    for (int i = 0; i < 32 && i < MAX_WEAPONS; i++)
    {
        outStats->weapons[i] = pPlayer->weapons[i].ammo;
        outStats->clips[i] = pPlayer->weapons[i].clip;
    }

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerName(int clientId, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    strncpy(buffer, pPlayer->name.chars(), bufferSize - 1);
    buffer[bufferSize - 1] = '\0';

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerIP(int clientId, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    strncpy(buffer, pPlayer->ip.chars(), bufferSize - 1);
    buffer[bufferSize - 1] = '\0';

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerAuthId(int clientId, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    const char* authId = GETPLAYERAUTHID(pPlayer->pEdict);
    if (authId)
    {
        strncpy(buffer, authId, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
        return true;
    }

    buffer[0] = '\0';
    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetPlayerTeam(int clientId, char* buffer, int bufferSize)
{
    if (!buffer || bufferSize <= 0 || !CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    strncpy(buffer, pPlayer->team.chars(), bufferSize - 1);
    buffer[bufferSize - 1] = '\0';

    return true;
}

// Player state functions implementation
CSHARP_EXPORT bool CSHARP_CALL IsPlayerInGame(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && pPlayer->ingame);
}

CSHARP_EXPORT bool CSHARP_CALL IsPlayerBot(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && pPlayer->IsBot());
}

CSHARP_EXPORT bool CSHARP_CALL IsPlayerAlive(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && pPlayer->IsAlive());
}

CSHARP_EXPORT bool CSHARP_CALL IsPlayerAuthorized(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && pPlayer->authorized);
}

CSHARP_EXPORT bool CSHARP_CALL IsPlayerConnecting(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && !pPlayer->ingame && pPlayer->initialized && (GETPLAYERUSERID(pPlayer->pEdict) > 0));
}

CSHARP_EXPORT bool CSHARP_CALL IsPlayerHLTV(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer && pPlayer->pEdict && (pPlayer->pEdict->v.flags & FL_PROXY));
}

// Player property getters implementation
CSHARP_EXPORT int CSHARP_CALL GetPlayerUserId(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return 0;

    return GETPLAYERUSERID(pPlayer->pEdict);
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerTeamId(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->teamId : 0;
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerFlags(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->flags[0] : 0;
}

CSHARP_EXPORT float CSHARP_CALL GetPlayerConnectTime(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0.0f;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->time : 0.0f;
}

CSHARP_EXPORT float CSHARP_CALL GetPlayerPlayTime(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0.0f;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->playtime : 0.0f;
}

CSHARP_EXPORT float CSHARP_CALL GetPlayerHealth(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0.0f;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return 0.0f;

    return pPlayer->pEdict->v.health;
}

CSHARP_EXPORT float CSHARP_CALL GetPlayerArmor(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0.0f;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return 0.0f;

    return pPlayer->pEdict->v.armorvalue;
}

CSHARP_EXPORT float CSHARP_CALL GetPlayerFrags(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0.0f;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return 0.0f;

    return pPlayer->pEdict->v.frags;
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerDeaths(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->deaths : 0;
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerCurrentWeapon(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->current : 0;
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerMenu(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->menu : 0;
}

CSHARP_EXPORT int CSHARP_CALL GetPlayerKeys(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return (pPlayer) ? pPlayer->keys : 0;
}

// Player property setters implementation
CSHARP_EXPORT bool CSHARP_CALL SetPlayerHealth(int clientId, float health)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    pPlayer->pEdict->v.health = health;
    return true;
}

// CVar system functions implementation
CSHARP_EXPORT int CSHARP_CALL CreateCvar(const char* name, const char* value, int flags, const char* description)
{
    if (!CSharpBridge::g_initialized || !name || !value)
        return -1;

    CSharpBridge::AutoLock lock;

    // Create CVar using AMX Mod X CVar system
    CPluginMngr::CPlugin* plugin = nullptr;
    for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
    {
        if ((*iter).isValid())
        {
            plugin = &(*iter);
            break;
        }
    }

    if (!plugin)
        return -1;

    CvarInfo* info = g_CvarManager.CreateCvar(name, value, plugin->getName(), plugin->getId(), flags);
    if (!info)
        return -1;

    return reinterpret_cast<int>(info->var);
}

CSHARP_EXPORT bool CSHARP_CALL CvarExists(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    cvar_t* cvar = CVAR_GET_POINTER(name);
    return (cvar != nullptr);
}

CSHARP_EXPORT bool CSHARP_CALL GetCvarString(const char* name, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !name || !buffer || bufferSize <= 0)
        return false;

    cvar_t* cvar = CVAR_GET_POINTER(name);
    if (!cvar)
        return false;

    strncpy(buffer, cvar->string, bufferSize - 1);
    buffer[bufferSize - 1] = '\0';
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarString(const char* name, const char* value)
{
    if (!CSharpBridge::g_initialized || !name || !value)
        return false;

    CVAR_SET_STRING(name, value);
    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetCvarInt(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0;

    return static_cast<int>(CVAR_GET_FLOAT(name));
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarInt(const char* name, int value)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    CVAR_SET_FLOAT(name, static_cast<float>(value));
    return true;
}

CSHARP_EXPORT float CSHARP_CALL GetCvarFloat(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0.0f;

    return CVAR_GET_FLOAT(name);
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarFloat(const char* name, float value)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    CVAR_SET_FLOAT(name, value);
    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetCvarFlags(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0;

    cvar_t* cvar = CVAR_GET_POINTER(name);
    if (!cvar)
        return 0;

    return cvar->flags;
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarFlags(const char* name, int flags)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    cvar_t* cvar = CVAR_GET_POINTER(name);
    if (!cvar)
        return false;

    cvar->flags = flags;
    return true;
}

CSHARP_EXPORT int CSHARP_CALL HookCvarChange(const char* name, CSharpCvarChangeCallback callback)
{
    if (!CSharpBridge::g_initialized || !name || !callback)
        return -1;

    return CSharpBridge::HookCvarChangeInternal(name, callback);
}

CSHARP_EXPORT bool CSHARP_CALL UnhookCvarChange(int hookId)
{
    if (!CSharpBridge::g_initialized || hookId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove the CVar hook
    for (size_t i = 0; i < CSharpBridge::g_cvarCallbacks.length(); i++)
    {
        CSharpBridge::CvarCallbackInfo* info = CSharpBridge::g_cvarCallbacks[i];
        if (info && info->hookId == hookId)
        {
            delete info;
            CSharpBridge::g_cvarCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

// Menu system functions implementation
CSHARP_EXPORT int CSHARP_CALL CreateMenu(const char* title, CSharpMenuSelectCallback selectCallback, CSharpMenuCancelCallback cancelCallback)
{
    if (!CSharpBridge::g_initialized || !title || !selectCallback)
        return -1;

    CSharpBridge::AutoLock lock;

    // Find a plugin to create the menu with
    CPluginMngr::CPlugin* plugin = nullptr;
    for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
    {
        if ((*iter).isValid())
        {
            plugin = &(*iter);
            break;
        }
    }

    if (!plugin)
        return -1;

    // Create a forward for menu selection
    int selectForwardId = registerSPForwardByName(plugin->getAMX(), "CSharpMenuSelectHandler",
                                                 FP_CELL, FP_CELL, FP_CELL, FP_DONE);
    if (selectForwardId == -1)
        return -1;

    // Create the menu using AMX Mod X menu system
    Menu* menu = new Menu(title, plugin->getAMX(), selectForwardId, false);
    if (!menu)
    {
        unregisterSPForward(selectForwardId);
        return -1;
    }

    int menuId = CSharpBridge::g_nextMenuId++;

    // Store callback information
    CSharpBridge::MenuCallbackInfo* callbackInfo = new CSharpBridge::MenuCallbackInfo();
    callbackInfo->menuId = menuId;
    callbackInfo->selectCallback = selectCallback;
    callbackInfo->cancelCallback = cancelCallback;
    callbackInfo->menu = menu;
    callbackInfo->selectForwardId = selectForwardId;

    CSharpBridge::g_menuCallbacks.append(callbackInfo);

    return menuId;
}

CSHARP_EXPORT bool CSHARP_CALL AddMenuItem(int menuId, const char* name, const char* command, int access)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    CSharpBridge::MenuCallbackInfo* menuInfo = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        if (CSharpBridge::g_menuCallbacks[i] && CSharpBridge::g_menuCallbacks[i]->menuId == menuId)
        {
            menuInfo = CSharpBridge::g_menuCallbacks[i];
            break;
        }
    }

    if (!menuInfo || !menuInfo->menu)
        return false;

    // Add item to menu
    menuInfo->menu->AddItem(name, command ? command : "", access);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL AddMenuBlank(int menuId, int slot)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    CSharpBridge::MenuCallbackInfo* menuInfo = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        if (CSharpBridge::g_menuCallbacks[i] && CSharpBridge::g_menuCallbacks[i]->menuId == menuId)
        {
            menuInfo = CSharpBridge::g_menuCallbacks[i];
            break;
        }
    }

    if (!menuInfo || !menuInfo->menu)
        return false;

    // Add blank line to menu
    if (slot >= 0)
        menuInfo->menu->AddBlankLine(slot);
    else
        menuInfo->menu->AddBlankLine();

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL AddMenuText(int menuId, const char* text, int slot)
{
    if (!CSharpBridge::g_initialized || !text)
        return false;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    CSharpBridge::MenuCallbackInfo* menuInfo = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        if (CSharpBridge::g_menuCallbacks[i] && CSharpBridge::g_menuCallbacks[i]->menuId == menuId)
        {
            menuInfo = CSharpBridge::g_menuCallbacks[i];
            break;
        }
    }

    if (!menuInfo || !menuInfo->menu)
        return false;

    // Add text to menu
    if (slot >= 0)
        menuInfo->menu->AddStaticItem(text, slot);
    else
        menuInfo->menu->AddStaticItem(text);

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL DisplayMenu(int menuId, int clientId, int page)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    CSharpBridge::MenuCallbackInfo* menuInfo = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        if (CSharpBridge::g_menuCallbacks[i] && CSharpBridge::g_menuCallbacks[i]->menuId == menuId)
        {
            menuInfo = CSharpBridge::g_menuCallbacks[i];
            break;
        }
    }

    if (!menuInfo || !menuInfo->menu)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->ingame)
        return false;

    // Display menu to player
    menuInfo->menu->Display(clientId, page);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL DestroyMenu(int menuId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove menu
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        CSharpBridge::MenuCallbackInfo* info = CSharpBridge::g_menuCallbacks[i];
        if (info && info->menuId == menuId)
        {
            if (info->selectForwardId != -1)
                unregisterSPForward(info->selectForwardId);

            if (info->menu)
                delete info->menu;

            delete info;
            CSharpBridge::g_menuCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetMenuInfo(int menuId, CSharpMenuInfo* outInfo)
{
    if (!CSharpBridge::g_initialized || !outInfo)
        return false;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    CSharpBridge::MenuCallbackInfo* menuInfo = nullptr;
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        if (CSharpBridge::g_menuCallbacks[i] && CSharpBridge::g_menuCallbacks[i]->menuId == menuId)
        {
            menuInfo = CSharpBridge::g_menuCallbacks[i];
            break;
        }
    }

    if (!menuInfo || !menuInfo->menu)
        return false;

    // Fill menu information
    strncpy(outInfo->title, menuInfo->menu->getTitle(), sizeof(outInfo->title) - 1);
    outInfo->title[sizeof(outInfo->title) - 1] = '\0';

    outInfo->menuId = menuId;
    outInfo->itemCount = menuInfo->menu->getItemCount();
    outInfo->pageCount = menuInfo->menu->getPageCount();
    outInfo->isActive = true;
    outInfo->neverExit = menuInfo->menu->getNeverExit();
    outInfo->forceExit = menuInfo->menu->getForceExit();

    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetMenuPages(int menuId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        CSharpBridge::MenuCallbackInfo* info = CSharpBridge::g_menuCallbacks[i];
        if (info && info->menuId == menuId && info->menu)
        {
            return info->menu->getPageCount();
        }
    }

    return 0;
}

CSHARP_EXPORT int CSHARP_CALL GetMenuItems(int menuId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    CSharpBridge::AutoLock lock;

    // Find menu by ID
    for (size_t i = 0; i < CSharpBridge::g_menuCallbacks.length(); i++)
    {
        CSharpBridge::MenuCallbackInfo* info = CSharpBridge::g_menuCallbacks[i];
        if (info && info->menuId == menuId && info->menu)
        {
            return info->menu->getItemCount();
        }
    }

    return 0;
}

// Game config functions implementation
CSHARP_EXPORT int CSHARP_CALL LoadGameConfig(const char* fileName)
{
    if (!CSharpBridge::g_initialized || !fileName)
        return -1;

    CSharpBridge::AutoLock lock;

    IGameConfig* config = nullptr;
    char error[128];

    if (!ConfigManager.LoadGameConfigFile(fileName, &config, error, sizeof(error)))
    {
        ConfigManager.CloseGameConfigFile(config);
        return -1;
    }

    int configId = CSharpBridge::g_nextGameConfigId++;

    CSharpBridge::GameConfigInfo* configInfo = new CSharpBridge::GameConfigInfo();
    configInfo->configId = configId;
    configInfo->config = config;
    configInfo->fileName = fileName;

    CSharpBridge::g_gameConfigs.append(configInfo);

    return configId;
}

CSHARP_EXPORT bool CSHARP_CALL GetGameConfigOffset(int configId, const char* key, int* offset)
{
    if (!CSharpBridge::g_initialized || !key || !offset)
        return false;

    CSharpBridge::AutoLock lock;

    // Find config by ID
    for (size_t i = 0; i < CSharpBridge::g_gameConfigs.length(); i++)
    {
        CSharpBridge::GameConfigInfo* info = CSharpBridge::g_gameConfigs[i];
        if (info && info->configId == configId && info->config)
        {
            return info->config->GetOffset(key, offset);
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetGameConfigAddress(int configId, const char* key, void** address)
{
    if (!CSharpBridge::g_initialized || !key || !address)
        return false;

    CSharpBridge::AutoLock lock;

    // Find config by ID
    for (size_t i = 0; i < CSharpBridge::g_gameConfigs.length(); i++)
    {
        CSharpBridge::GameConfigInfo* info = CSharpBridge::g_gameConfigs[i];
        if (info && info->configId == configId && info->config)
        {
            return info->config->GetAddress(key, address);
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetGameConfigKeyValue(int configId, const char* key, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !key || !buffer || bufferSize <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find config by ID
    for (size_t i = 0; i < CSharpBridge::g_gameConfigs.length(); i++)
    {
        CSharpBridge::GameConfigInfo* info = CSharpBridge::g_gameConfigs[i];
        if (info && info->configId == configId && info->config)
        {
            const char* value = info->config->GetKeyValue(key);
            if (value)
            {
                strncpy(buffer, value, bufferSize - 1);
                buffer[bufferSize - 1] = '\0';
                return true;
            }
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL CloseGameConfig(int configId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove config
    for (size_t i = 0; i < CSharpBridge::g_gameConfigs.length(); i++)
    {
        CSharpBridge::GameConfigInfo* info = CSharpBridge::g_gameConfigs[i];
        if (info && info->configId == configId)
        {
            if (info->config)
                ConfigManager.CloseGameConfigFile(info->config);

            delete info;
            CSharpBridge::g_gameConfigs[i] = nullptr;
            return true;
        }
    }

    return false;
}

// Native management functions implementation
CSHARP_EXPORT bool CSHARP_CALL RegisterNative(const char* name, CSharpNativeCallback callback)
{
    if (!CSharpBridge::g_initialized || !name || !callback)
        return false;

    CSharpBridge::AutoLock lock;

    CSharpBridge::NativeCallbackInfo* nativeInfo = new CSharpBridge::NativeCallbackInfo();
    nativeInfo->name = name;
    nativeInfo->callback = callback;
    nativeInfo->nativeId = CSharpBridge::g_nextNativeId++;

    CSharpBridge::g_nativeCallbacks.append(nativeInfo);

    // Register with AMX native system would require more complex integration
    // This is a simplified implementation
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterNative(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove native
    for (size_t i = 0; i < CSharpBridge::g_nativeCallbacks.length(); i++)
    {
        CSharpBridge::NativeCallbackInfo* info = CSharpBridge::g_nativeCallbacks[i];
        if (info && strcmp(info->name.chars(), name) == 0)
        {
            delete info;
            CSharpBridge::g_nativeCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL GetNativeParam(int index)
{
    if (!CSharpBridge::g_initialized || index < 0)
        return 0;

    // This would need to be implemented with current AMX context
    // Simplified implementation
    return 0;
}

CSHARP_EXPORT bool CSHARP_CALL GetNativeString(int index, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || index < 0 || !buffer || bufferSize <= 0)
        return false;

    // This would need to be implemented with current AMX context
    // Simplified implementation
    buffer[0] = '\0';
    return false;
}

CSHARP_EXPORT bool CSHARP_CALL SetNativeString(int index, const char* value)
{
    if (!CSharpBridge::g_initialized || index < 0 || !value)
        return false;

    // This would need to be implemented with current AMX context
    // Simplified implementation
    return false;
}

CSHARP_EXPORT bool CSHARP_CALL GetNativeArray(int index, int* buffer, int size)
{
    if (!CSharpBridge::g_initialized || index < 0 || !buffer || size <= 0)
        return false;

    // This would need to be implemented with current AMX context
    // Simplified implementation
    return false;
}

CSHARP_EXPORT bool CSHARP_CALL SetNativeArray(int index, const int* buffer, int size)
{
    if (!CSharpBridge::g_initialized || index < 0 || !buffer || size <= 0)
        return false;

    // This would need to be implemented with current AMX context
    // Simplified implementation
    return false;
}

// Message system functions implementation
CSHARP_EXPORT bool CSHARP_CALL BeginMessage(int msgType, int msgDest, int entityId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    edict_t* pEntity = nullptr;
    if (entityId > 0 && entityId <= gpGlobals->maxClients)
    {
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(entityId);
        if (pPlayer && pPlayer->pEdict)
            pEntity = pPlayer->pEdict;
    }
    else if (entityId > 0)
    {
        pEntity = TypeConversion.id_to_edict(entityId);
    }

    MESSAGE_BEGIN(msgDest, msgType, nullptr, pEntity);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL EndMessage()
{
    if (!CSharpBridge::g_initialized)
        return false;

    MESSAGE_END();
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteByte(int value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_BYTE(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteChar(int value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_CHAR(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteShort(int value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_SHORT(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteLong(int value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_LONG(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteAngle(float value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_ANGLE(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteCoord(float value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_COORD(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteString(const char* value)
{
    if (!CSharpBridge::g_initialized || !value)
        return false;

    WRITE_STRING(value);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL WriteEntity(int entityId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    WRITE_ENTITY(entityId);
    return true;
}

CSHARP_EXPORT int CSHARP_CALL RegisterMessage(int msgId, CSharpMessageCallback callback)
{
    if (!CSharpBridge::g_initialized || !callback)
        return -1;

    return CSharpBridge::RegisterMessageInternal(msgId, callback);
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterMessage(int hookId)
{
    if (!CSharpBridge::g_initialized || hookId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove message hook
    for (size_t i = 0; i < CSharpBridge::g_messageCallbacks.length(); i++)
    {
        CSharpBridge::MessageCallbackInfo* info = CSharpBridge::g_messageCallbacks[i];
        if (info && info->hookId == hookId)
        {
            delete info;
            CSharpBridge::g_messageCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL GetMessageArgs()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    // This would need to be implemented with current message context
    return 0;
}

CSHARP_EXPORT int CSHARP_CALL GetMessageArgType(int argIndex)
{
    if (!CSharpBridge::g_initialized || argIndex < 0)
        return 0;

    // This would need to be implemented with current message context
    return 0;
}

CSHARP_EXPORT int CSHARP_CALL GetMessageArgInt(int argIndex)
{
    if (!CSharpBridge::g_initialized || argIndex < 0)
        return 0;

    // This would need to be implemented with current message context
    return 0;
}

CSHARP_EXPORT float CSHARP_CALL GetMessageArgFloat(int argIndex)
{
    if (!CSharpBridge::g_initialized || argIndex < 0)
        return 0.0f;

    // This would need to be implemented with current message context
    return 0.0f;
}

CSHARP_EXPORT bool CSHARP_CALL GetMessageArgString(int argIndex, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || argIndex < 0 || !buffer || bufferSize <= 0)
        return false;

    // This would need to be implemented with current message context
    buffer[0] = '\0';
    return false;
}

// DataPack functions implementation
CSHARP_EXPORT int CSHARP_CALL CreateDataPack()
{
    if (!CSharpBridge::g_initialized)
        return -1;

    CSharpBridge::AutoLock lock;

    CDataPack* pack = new CDataPack();
    if (!pack)
        return -1;

    int packId = CSharpBridge::g_nextDataPackId++;

    CSharpBridge::DataPackInfo* packInfo = new CSharpBridge::DataPackInfo();
    packInfo->packId = packId;
    packInfo->pack = pack;

    CSharpBridge::g_dataPacks.append(packInfo);

    return packId;
}

CSHARP_EXPORT bool CSHARP_CALL WritePackCell(int packId, int value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            info->pack->PackCell(value);
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL WritePackFloat(int packId, float value)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            info->pack->PackFloat(value);
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL WritePackString(int packId, const char* value)
{
    if (!CSharpBridge::g_initialized || !value)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            info->pack->PackString(value);
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL ReadPackCell(int packId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            return info->pack->ReadCell();
        }
    }

    return 0;
}

CSHARP_EXPORT float CSHARP_CALL ReadPackFloat(int packId)
{
    if (!CSharpBridge::g_initialized)
        return 0.0f;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            return info->pack->ReadFloat();
        }
    }

    return 0.0f;
}

CSHARP_EXPORT bool CSHARP_CALL ReadPackString(int packId, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !buffer || bufferSize <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            const char* str = info->pack->ReadString();
            if (str)
            {
                strncpy(buffer, str, bufferSize - 1);
                buffer[bufferSize - 1] = '\0';
                return true;
            }
        }
    }

    buffer[0] = '\0';
    return false;
}

CSHARP_EXPORT bool CSHARP_CALL ResetPack(int packId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            info->pack->Reset();
            return true;
        }
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL GetPackPosition(int packId)
{
    if (!CSharpBridge::g_initialized)
        return -1;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            return info->pack->GetPosition();
        }
    }

    return -1;
}

CSHARP_EXPORT bool CSHARP_CALL SetPackPosition(int packId, int position)
{
    if (!CSharpBridge::g_initialized || position < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            return info->pack->SetPosition(position);
        }
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL IsPackEnded(int packId)
{
    if (!CSharpBridge::g_initialized)
        return true;

    CSharpBridge::AutoLock lock;

    // Find pack by ID
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId && info->pack)
        {
            return info->pack->IsReadable();
        }
    }

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL DestroyDataPack(int packId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove pack
    for (size_t i = 0; i < CSharpBridge::g_dataPacks.length(); i++)
    {
        CSharpBridge::DataPackInfo* info = CSharpBridge::g_dataPacks[i];
        if (info && info->packId == packId)
        {
            if (info->pack)
                delete info->pack;

            delete info;
            CSharpBridge::g_dataPacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

// Helper functions for internal use
int CSharpBridge::HookCvarChangeInternal(const char* name, CSharpCvarChangeCallback callback)
{
    AutoLock lock;

    // Create CVar hook using AMX Mod X CVar system
    CvarCallbackInfo* info = new CvarCallbackInfo();
    info->hookId = g_nextCvarHookId++;
    info->cvarCallback = nullptr; // We're using the new callback type
    info->cvarName = name;
    info->amxForwardId = -1;

    // Store the C# callback separately
    // This is a simplified implementation - in a real scenario, you'd need to
    // integrate with the AMX Mod X CVar change notification system
    g_cvarCallbacks.append(info);

    return info->hookId;
}

int CSharpBridge::RegisterMessageInternal(int msgId, CSharpMessageCallback callback)
{
    AutoLock lock;

    MessageCallbackInfo* info = new MessageCallbackInfo();
    info->hookId = g_nextMessageHandle++;
    info->messageCallback = nullptr; // We're using the new callback type
    info->msgId = msgId;
    info->amxForwardId = -1;

    // Store the C# callback
    // This is a simplified implementation - in a real scenario, you'd need to
    // integrate with the AMX Mod X message hooking system
    g_messageCallbacks.append(info);

    return info->hookId;
}

CSHARP_EXPORT bool CSHARP_CALL SetPlayerArmor(int clientId, float armor)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    pPlayer->pEdict->v.armorvalue = armor;
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL SetPlayerFrags(int clientId, float frags)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    pPlayer->pEdict->v.frags = frags;
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL SetPlayerTeamInfo(int clientId, int teamId, const char* teamName)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->ingame)
        return false;

    pPlayer->teamId = teamId;
    if (teamName != nullptr)
    {
        pPlayer->team = teamName;
        // Register team if not already registered
        g_teamsIds.registerTeam(teamName, teamId);
    }

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL SetPlayerFlags(int clientId, int flags)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer)
        return false;

    pPlayer->flags[0] = flags;
    return true;
}

// Player utility functions implementation
CSHARP_EXPORT int CSHARP_CALL GetMaxClients()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return gpGlobals->maxClients;
}

CSHARP_EXPORT int CSHARP_CALL GetConnectedPlayersCount()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    int count = 0;
    for (int i = 1; i <= gpGlobals->maxClients; i++)
    {
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(i);
        if (pPlayer && pPlayer->ingame)
            count++;
    }

    return count;
}

CSHARP_EXPORT bool CSHARP_CALL GetConnectedPlayers(int* playerIds, int maxPlayers, int* outCount)
{
    if (!playerIds || !outCount || !CSharpBridge::g_initialized)
        return false;

    int count = 0;
    for (int i = 1; i <= gpGlobals->maxClients && count < maxPlayers; i++)
    {
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(i);
        if (pPlayer && pPlayer->ingame)
        {
            playerIds[count] = i;
            count++;
        }
    }

    *outCount = count;
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL KickPlayer(int clientId, const char* reason)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    // Create kick command
    char kickCmd[256];
    if (reason && strlen(reason) > 0)
    {
        snprintf(kickCmd, sizeof(kickCmd), "kick #%d \"%s\"", GETPLAYERUSERID(pPlayer->pEdict), reason);
    }
    else
    {
        snprintf(kickCmd, sizeof(kickCmd), "kick #%d", GETPLAYERUSERID(pPlayer->pEdict));
    }

    SERVER_COMMAND(kickCmd);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL SlayPlayer(int clientId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict || !pPlayer->IsAlive())
        return false;

    // Kill the player
    pPlayer->pEdict->v.health = 0.0f;
    pPlayer->pEdict->v.deadflag = DEAD_DEAD;

    // Call the killed function to properly handle death
    MDLL_Killed(pPlayer->pEdict, nullptr, 0);

    return true;
}

// ========== 实体管理接口实现 / Entity Management Interface Implementation ==========

CSHARP_EXPORT int CSHARP_CALL CreateEntity(const char* className)
{
    if (!CSharpBridge::g_initialized || !className)
        return 0;

    LOCK_ENTER();

    try
    {
        // Create named entity using engine function
        int iszClass = ALLOC_STRING(className);
        edict_t* pEnt = CREATE_NAMED_ENTITY(iszClass);

        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return 0;
        }

        int entityId = ENTINDEX(pEnt);
        LOCK_LEAVE();
        return entityId;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL RemoveEntity(int entityId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    LOCK_ENTER();

    try
    {
        // Validate entity ID
        if (entityId <= gpGlobals->maxClients || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Remove the entity
        REMOVE_ENTITY(pEnt);
        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityCount()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return NUMBER_OF_ENTITIES();
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByClassName(int startEntity, const char* className)
{
    if (!CSharpBridge::g_initialized || !className)
        return 0;

    LOCK_ENTER();

    try
    {
        edict_t* pStart = (startEntity > 0) ? INDEXENT(startEntity) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "classname", className);

        if (FNullEnt(pFound))
        {
            LOCK_LEAVE();
            return 0;
        }

        int entityId = ENTINDEX(pFound);
        LOCK_LEAVE();
        return entityId;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByTargetName(int startEntity, const char* targetName)
{
    if (!CSharpBridge::g_initialized || !targetName)
        return 0;

    LOCK_ENTER();

    try
    {
        edict_t* pStart = (startEntity > 0) ? INDEXENT(startEntity) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "targetname", targetName);

        if (FNullEnt(pFound))
        {
            LOCK_LEAVE();
            return 0;
        }

        int entityId = ENTINDEX(pFound);
        LOCK_LEAVE();
        return entityId;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityInSphere(int startEntity, const float* origin, float radius)
{
    if (!CSharpBridge::g_initialized || !origin)
        return 0;

    LOCK_ENTER();

    try
    {
        edict_t* pStart = (startEntity > 0) ? INDEXENT(startEntity) : nullptr;
        Vector vecOrigin(origin[0], origin[1], origin[2]);
        edict_t* pFound = FIND_ENTITY_IN_SPHERE(pStart, vecOrigin, radius);

        if (FNullEnt(pFound))
        {
            LOCK_LEAVE();
            return 0;
        }

        int entityId = ENTINDEX(pFound);
        LOCK_LEAVE();
        return entityId;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityInt(int entityId, const char* property)
{
    if (!CSharpBridge::g_initialized || !property)
        return 0;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return 0;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return 0;
        }

        // Map common property names to entity variables
        int result = 0;
        if (strcmp(property, "health") == 0)
            result = (int)pEnt->v.health;
        else if (strcmp(property, "armor") == 0)
            result = (int)pEnt->v.armorvalue;
        else if (strcmp(property, "team") == 0)
            result = pEnt->v.team;
        else if (strcmp(property, "playerclass") == 0)
            result = pEnt->v.playerclass;
        else if (strcmp(property, "deadflag") == 0)
            result = pEnt->v.deadflag;
        else if (strcmp(property, "flags") == 0)
            result = pEnt->v.flags;
        else if (strcmp(property, "effects") == 0)
            result = pEnt->v.effects;
        else if (strcmp(property, "solid") == 0)
            result = pEnt->v.solid;
        else if (strcmp(property, "movetype") == 0)
            result = pEnt->v.movetype;
        else if (strcmp(property, "spawnflags") == 0)
            result = pEnt->v.spawnflags;

        LOCK_LEAVE();
        return result;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityInt(int entityId, const char* property, int value)
{
    if (!CSharpBridge::g_initialized || !property)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        bool success = true;
        if (strcmp(property, "health") == 0)
            pEnt->v.health = (float)value;
        else if (strcmp(property, "armor") == 0)
            pEnt->v.armorvalue = (float)value;
        else if (strcmp(property, "team") == 0)
            pEnt->v.team = value;
        else if (strcmp(property, "playerclass") == 0)
            pEnt->v.playerclass = value;
        else if (strcmp(property, "deadflag") == 0)
            pEnt->v.deadflag = value;
        else if (strcmp(property, "flags") == 0)
            pEnt->v.flags = value;
        else if (strcmp(property, "effects") == 0)
            pEnt->v.effects = value;
        else if (strcmp(property, "solid") == 0)
            pEnt->v.solid = value;
        else if (strcmp(property, "movetype") == 0)
            pEnt->v.movetype = value;
        else if (strcmp(property, "spawnflags") == 0)
            pEnt->v.spawnflags = value;
        else
            success = false;

        LOCK_LEAVE();
        return success;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT float CSHARP_CALL GetEntityFloat(int entityId, const char* property)
{
    if (!CSharpBridge::g_initialized || !property)
        return 0.0f;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return 0.0f;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return 0.0f;
        }

        // Map common property names to entity variables
        float result = 0.0f;
        if (strcmp(property, "health") == 0)
            result = pEnt->v.health;
        else if (strcmp(property, "armor") == 0)
            result = pEnt->v.armorvalue;
        else if (strcmp(property, "speed") == 0)
            result = pEnt->v.maxspeed;
        else if (strcmp(property, "gravity") == 0)
            result = pEnt->v.gravity;
        else if (strcmp(property, "friction") == 0)
            result = pEnt->v.friction;
        else if (strcmp(property, "framerate") == 0)
            result = pEnt->v.framerate;
        else if (strcmp(property, "scale") == 0)
            result = pEnt->v.scale;
        else if (strcmp(property, "takedamage") == 0)
            result = pEnt->v.takedamage;

        LOCK_LEAVE();
        return result;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0.0f;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityFloat(int entityId, const char* property, float value)
{
    if (!CSharpBridge::g_initialized || !property)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        bool success = true;
        if (strcmp(property, "health") == 0)
            pEnt->v.health = value;
        else if (strcmp(property, "armor") == 0)
            pEnt->v.armorvalue = value;
        else if (strcmp(property, "speed") == 0)
            pEnt->v.maxspeed = value;
        else if (strcmp(property, "gravity") == 0)
            pEnt->v.gravity = value;
        else if (strcmp(property, "friction") == 0)
            pEnt->v.friction = value;
        else if (strcmp(property, "framerate") == 0)
            pEnt->v.framerate = value;
        else if (strcmp(property, "scale") == 0)
            pEnt->v.scale = value;
        else if (strcmp(property, "takedamage") == 0)
            pEnt->v.takedamage = value;
        else
            success = false;

        LOCK_LEAVE();
        return success;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL GetEntityString(int entityId, const char* property, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !property || !buffer || bufferSize <= 0)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        const char* result = nullptr;
        if (strcmp(property, "classname") == 0)
            result = STRING(pEnt->v.classname);
        else if (strcmp(property, "targetname") == 0)
            result = STRING(pEnt->v.targetname);
        else if (strcmp(property, "target") == 0)
            result = STRING(pEnt->v.target);
        else if (strcmp(property, "model") == 0)
            result = STRING(pEnt->v.model);
        else if (strcmp(property, "netname") == 0)
            result = STRING(pEnt->v.netname);
        else if (strcmp(property, "message") == 0)
            result = STRING(pEnt->v.message);

        if (result)
        {
            strncpy(buffer, result, bufferSize - 1);
            buffer[bufferSize - 1] = '\0';
            LOCK_LEAVE();
            return true;
        }

        LOCK_LEAVE();
        return false;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityString(int entityId, const char* property, const char* value)
{
    if (!CSharpBridge::g_initialized || !property || !value)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        bool success = true;
        if (strcmp(property, "classname") == 0)
            pEnt->v.classname = ALLOC_STRING(value);
        else if (strcmp(property, "targetname") == 0)
            pEnt->v.targetname = ALLOC_STRING(value);
        else if (strcmp(property, "target") == 0)
            pEnt->v.target = ALLOC_STRING(value);
        else if (strcmp(property, "model") == 0)
            pEnt->v.model = ALLOC_STRING(value);
        else if (strcmp(property, "netname") == 0)
            pEnt->v.netname = ALLOC_STRING(value);
        else if (strcmp(property, "message") == 0)
            pEnt->v.message = ALLOC_STRING(value);
        else
            success = false;

        LOCK_LEAVE();
        return success;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL GetEntityVector(int entityId, const char* property, float* vector)
{
    if (!CSharpBridge::g_initialized || !property || !vector)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        bool success = true;
        if (strcmp(property, "origin") == 0)
        {
            vector[0] = pEnt->v.origin.x;
            vector[1] = pEnt->v.origin.y;
            vector[2] = pEnt->v.origin.z;
        }
        else if (strcmp(property, "angles") == 0)
        {
            vector[0] = pEnt->v.angles.x;
            vector[1] = pEnt->v.angles.y;
            vector[2] = pEnt->v.angles.z;
        }
        else if (strcmp(property, "velocity") == 0)
        {
            vector[0] = pEnt->v.velocity.x;
            vector[1] = pEnt->v.velocity.y;
            vector[2] = pEnt->v.velocity.z;
        }
        else if (strcmp(property, "mins") == 0)
        {
            vector[0] = pEnt->v.mins.x;
            vector[1] = pEnt->v.mins.y;
            vector[2] = pEnt->v.mins.z;
        }
        else if (strcmp(property, "maxs") == 0)
        {
            vector[0] = pEnt->v.maxs.x;
            vector[1] = pEnt->v.maxs.y;
            vector[2] = pEnt->v.maxs.z;
        }
        else
            success = false;

        LOCK_LEAVE();
        return success;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityVector(int entityId, const char* property, const float* vector)
{
    if (!CSharpBridge::g_initialized || !property || !vector)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        // Map common property names to entity variables
        bool success = true;
        if (strcmp(property, "origin") == 0)
        {
            pEnt->v.origin.x = vector[0];
            pEnt->v.origin.y = vector[1];
            pEnt->v.origin.z = vector[2];
        }
        else if (strcmp(property, "angles") == 0)
        {
            pEnt->v.angles.x = vector[0];
            pEnt->v.angles.y = vector[1];
            pEnt->v.angles.z = vector[2];
        }
        else if (strcmp(property, "velocity") == 0)
        {
            pEnt->v.velocity.x = vector[0];
            pEnt->v.velocity.y = vector[1];
            pEnt->v.velocity.z = vector[2];
        }
        else if (strcmp(property, "mins") == 0)
        {
            pEnt->v.mins.x = vector[0];
            pEnt->v.mins.y = vector[1];
            pEnt->v.mins.z = vector[2];
        }
        else if (strcmp(property, "maxs") == 0)
        {
            pEnt->v.maxs.x = vector[0];
            pEnt->v.maxs.y = vector[1];
            pEnt->v.maxs.z = vector[2];
        }
        else
            success = false;

        LOCK_LEAVE();
        return success;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityOrigin(int entityId, const float* origin)
{
    if (!CSharpBridge::g_initialized || !origin)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        Vector vecOrigin(origin[0], origin[1], origin[2]);
        SET_ORIGIN(pEnt, vecOrigin);

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntitySize(int entityId, const float* mins, const float* maxs)
{
    if (!CSharpBridge::g_initialized || !mins || !maxs)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        Vector vecMins(mins[0], mins[1], mins[2]);
        Vector vecMaxs(maxs[0], maxs[1], maxs[2]);
        SET_SIZE(pEnt, vecMins, vecMaxs);

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityModel(int entityId, const char* model)
{
    if (!CSharpBridge::g_initialized || !model)
        return false;

    LOCK_ENTER();

    try
    {
        if (entityId <= 0 || entityId > gpGlobals->maxEntities)
        {
            LOCK_LEAVE();
            return false;
        }

        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
        {
            LOCK_LEAVE();
            return false;
        }

        SET_MODEL(pEnt, model);

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

// ========== 消息系统接口实现 / Message System Interface Implementation ==========

// Message state tracking
static bool g_messageStarted = false;
static int g_currentMsgType = 0;
static int g_currentMsgDest = 0;

CSHARP_EXPORT int CSHARP_CALL RegisterMessage(int msgId, CSharpMessageCallback callback)
{
    if (!CSharpBridge::g_initialized || !callback)
        return 0;

    LOCK_ENTER();

    try
    {
        MessageCallbackInfo* info = new MessageCallbackInfo();
        info->messageCallback = callback;
        info->msgId = msgId;
        info->msgHandle = CSharpBridge::g_nextMessageHandle++;
        info->amxForwardId = -1; // Not used for message callbacks

        CSharpBridge::g_messageCallbacks.append(info);

        LOCK_LEAVE();
        return info->msgHandle;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterMessage(int msgId, int msgHandle)
{
    if (!CSharpBridge::g_initialized)
        return false;

    LOCK_ENTER();

    try
    {
        for (size_t i = 0; i < CSharpBridge::g_messageCallbacks.length(); i++)
        {
            MessageCallbackInfo* info = CSharpBridge::g_messageCallbacks[i];
            if (info && info->msgId == msgId && info->msgHandle == msgHandle)
            {
                delete info;
                CSharpBridge::g_messageCallbacks.remove(i);
                LOCK_LEAVE();
                return true;
            }
        }

        LOCK_LEAVE();
        return false;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetMessageBlock(int msgId, int blockType)
{
    if (!CSharpBridge::g_initialized)
        return false;

    // Use AMX Mod X's message blocking system
    // blockType: 0 = not blocked, 1 = blocked, 2 = once
    return g_msgBlocks.SetBlock(msgId, blockType) != 0;
}

CSHARP_EXPORT int CSHARP_CALL GetMessageBlock(int msgId)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return g_msgBlocks.GetBlock(msgId);
}

CSHARP_EXPORT bool CSHARP_CALL MessageBegin(int msgDest, int msgType, const float* origin, int entityId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    LOCK_ENTER();

    try
    {
        if (g_messageStarted)
        {
            LOCK_LEAVE();
            return false; // Message already started
        }

        edict_t* pEdict = nullptr;
        if (entityId > 0 && entityId <= gpGlobals->maxClients)
        {
            pEdict = INDEXENT(entityId);
            if (FNullEnt(pEdict))
                pEdict = nullptr;
        }

        Vector vecOrigin;
        if (origin)
        {
            vecOrigin.x = origin[0];
            vecOrigin.y = origin[1];
            vecOrigin.z = origin[2];
        }

        MESSAGE_BEGIN(msgDest, msgType, origin ? &vecOrigin : nullptr, pEdict);

        g_messageStarted = true;
        g_currentMsgType = msgType;
        g_currentMsgDest = msgDest;

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT void CSHARP_CALL MessageEnd()
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    LOCK_ENTER();

    try
    {
        MESSAGE_END();
        g_messageStarted = false;
        g_currentMsgType = 0;
        g_currentMsgDest = 0;
    }
    catch (...)
    {
        // Ignore exceptions in cleanup
    }

    LOCK_LEAVE();
}

// Message writing functions
CSHARP_EXPORT void CSHARP_CALL WriteByte(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_BYTE(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteChar(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_CHAR(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteShort(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_SHORT(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteLong(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_LONG(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteAngle(float value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_ANGLE(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteCoord(float value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_COORD(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteString(const char* value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted || !value)
        return;

    WRITE_STRING(value);
}

CSHARP_EXPORT void CSHARP_CALL WriteEntity(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    WRITE_ENTITY(value);
}

// Engine message functions (for engine messages)
CSHARP_EXPORT bool CSHARP_CALL EngineMessageBegin(int msgDest, int msgType, const float* origin, int entityId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    LOCK_ENTER();

    try
    {
        if (g_messageStarted)
        {
            LOCK_LEAVE();
            return false; // Message already started
        }

        edict_t* pEdict = nullptr;
        if (entityId > 0 && entityId <= gpGlobals->maxClients)
        {
            pEdict = INDEXENT(entityId);
            if (FNullEnt(pEdict))
                pEdict = nullptr;
        }

        Vector vecOrigin;
        if (origin)
        {
            vecOrigin.x = origin[0];
            vecOrigin.y = origin[1];
            vecOrigin.z = origin[2];
        }

        (*g_engfuncs.pfnMessageBegin)(msgDest, msgType, origin ? &vecOrigin : nullptr, pEdict);

        g_messageStarted = true;
        g_currentMsgType = msgType;
        g_currentMsgDest = msgDest;

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT void CSHARP_CALL EngineMessageEnd()
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    LOCK_ENTER();

    try
    {
        (*g_engfuncs.pfnMessageEnd)();
        g_messageStarted = false;
        g_currentMsgType = 0;
        g_currentMsgDest = 0;
    }
    catch (...)
    {
        // Ignore exceptions in cleanup
    }

    LOCK_LEAVE();
}

// Engine message writing functions
CSHARP_EXPORT void CSHARP_CALL EngineWriteByte(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteByte)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteChar(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteChar)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteShort(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteShort)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteLong(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteLong)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteAngle(float value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteAngle)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteCoord(float value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteCoord)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteString(const char* value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted || !value)
        return;

    (*g_engfuncs.pfnWriteString)(value);
}

CSHARP_EXPORT void CSHARP_CALL EngineWriteEntity(int value)
{
    if (!CSharpBridge::g_initialized || !g_messageStarted)
        return;

    (*g_engfuncs.pfnWriteEntity)(value);
}

// Message utility functions
CSHARP_EXPORT int CSHARP_CALL GetUserMessageId(const char* msgName)
{
    if (!CSharpBridge::g_initialized || !msgName)
        return 0;

    return GET_USER_MSG_ID(PLID, msgName, nullptr);
}

CSHARP_EXPORT bool CSHARP_CALL GetUserMessageName(int msgId, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !buffer || bufferSize <= 0)
        return false;

    const char* msgName = GET_USER_MSG_NAME(PLID, msgId, nullptr);
    if (msgName)
    {
        strncpy(buffer, msgName, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
        return true;
    }

    return false;
}

// ========== CVars系统接口实现 / CVars System Interface Implementation ==========

CSHARP_EXPORT bool CSHARP_CALL CreateCvar(const char* name, const char* value, int flags, const char* description, bool hasMin, float minValue, bool hasMax, float maxValue)
{
    if (!CSharpBridge::g_initialized || !name || !value)
        return false;

    LOCK_ENTER();

    try
    {
        // Create the CVar using engine function
        cvar_t* pCvar = CVAR_GET_POINTER(name);
        if (pCvar)
        {
            // CVar already exists
            LOCK_LEAVE();
            return false;
        }

        // Register new CVar
        CVAR_REGISTER(name, value, flags);

        // Set bounds if specified (AMX Mod X specific)
        if (hasMin || hasMax)
        {
            // Note: Bounds setting would require AMX Mod X specific implementation
            // This is a simplified version
        }

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL RegisterCvar(const char* name, const char* value, int flags, float floatValue)
{
    if (!CSharpBridge::g_initialized || !name || !value)
        return false;

    LOCK_ENTER();

    try
    {
        CVAR_REGISTER(name, value, flags);
        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL CvarExists(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    cvar_t* pCvar = CVAR_GET_POINTER(name);
    return pCvar != nullptr;
}

CSHARP_EXPORT bool CSHARP_CALL GetCvarString(const char* name, char* buffer, int bufferSize)
{
    if (!CSharpBridge::g_initialized || !name || !buffer || bufferSize <= 0)
        return false;

    LOCK_ENTER();

    try
    {
        const char* value = CVAR_GET_STRING(name);
        if (value)
        {
            strncpy(buffer, value, bufferSize - 1);
            buffer[bufferSize - 1] = '\0';
            LOCK_LEAVE();
            return true;
        }

        LOCK_LEAVE();
        return false;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetCvarInt(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0;

    return (int)CVAR_GET_FLOAT(name);
}

CSHARP_EXPORT float CSHARP_CALL GetCvarFloat(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0.0f;

    return CVAR_GET_FLOAT(name);
}

CSHARP_EXPORT int CSHARP_CALL GetCvarFlags(const char* name)
{
    if (!CSharpBridge::g_initialized || !name)
        return 0;

    cvar_t* pCvar = CVAR_GET_POINTER(name);
    return pCvar ? pCvar->flags : 0;
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarString(const char* name, const char* value)
{
    if (!CSharpBridge::g_initialized || !name || !value)
        return false;

    LOCK_ENTER();

    try
    {
        CVAR_SET_STRING(name, value);
        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarInt(const char* name, int value)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    LOCK_ENTER();

    try
    {
        CVAR_SET_FLOAT(name, (float)value);
        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarFloat(const char* name, float value)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    LOCK_ENTER();

    try
    {
        CVAR_SET_FLOAT(name, value);
        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarFlags(const char* name, int flags)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    LOCK_ENTER();

    try
    {
        cvar_t* pCvar = CVAR_GET_POINTER(name);
        if (pCvar)
        {
            pCvar->flags = flags;
            LOCK_LEAVE();
            return true;
        }

        LOCK_LEAVE();
        return false;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL GetCvarInfo(const char* name, CSharpCvarInfo* outInfo)
{
    if (!CSharpBridge::g_initialized || !name || !outInfo)
        return false;

    LOCK_ENTER();

    try
    {
        cvar_t* pCvar = CVAR_GET_POINTER(name);
        if (!pCvar)
        {
            LOCK_LEAVE();
            return false;
        }

        // Fill the info structure
        strncpy(outInfo->name, pCvar->name ? pCvar->name : "", sizeof(outInfo->name) - 1);
        outInfo->name[sizeof(outInfo->name) - 1] = '\0';

        strncpy(outInfo->value, pCvar->string ? pCvar->string : "", sizeof(outInfo->value) - 1);
        outInfo->value[sizeof(outInfo->value) - 1] = '\0';

        // Default value and description would need AMX Mod X specific implementation
        strncpy(outInfo->defaultValue, pCvar->string ? pCvar->string : "", sizeof(outInfo->defaultValue) - 1);
        outInfo->defaultValue[sizeof(outInfo->defaultValue) - 1] = '\0';

        strcpy(outInfo->description, ""); // Not available in basic cvar_t

        outInfo->flags = pCvar->flags;
        outInfo->floatValue = pCvar->value;
        outInfo->hasMin = false; // Would need AMX Mod X specific implementation
        outInfo->minValue = 0.0f;
        outInfo->hasMax = false;
        outInfo->maxValue = 0.0f;

        LOCK_LEAVE();
        return true;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL HookCvarChange(const char* name, CSharpCvarCallback callback)
{
    if (!CSharpBridge::g_initialized || !name || !callback)
        return 0;

    LOCK_ENTER();

    try
    {
        CvarCallbackInfo* info = new CvarCallbackInfo();
        info->cvarCallback = callback;
        info->cvarName = ke::AString(name);
        info->hookId = CSharpBridge::g_nextCvarHookId++;
        info->amxForwardId = -1; // Not used for CVar callbacks

        CSharpBridge::g_cvarCallbacks.append(info);

        LOCK_LEAVE();
        return info->hookId;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL UnhookCvarChange(int hookId)
{
    if (!CSharpBridge::g_initialized)
        return false;

    LOCK_ENTER();

    try
    {
        for (size_t i = 0; i < CSharpBridge::g_cvarCallbacks.length(); i++)
        {
            CvarCallbackInfo* info = CSharpBridge::g_cvarCallbacks[i];
            if (info && info->hookId == hookId)
            {
                delete info;
                CSharpBridge::g_cvarCallbacks.remove(i);
                LOCK_LEAVE();
                return true;
            }
        }

        LOCK_LEAVE();
        return false;
    }
    catch (...)
    {
        LOCK_LEAVE();
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetCvarBounds(const char* name, bool hasMin, float minValue, bool hasMax, float maxValue)
{
    if (!CSharpBridge::g_initialized || !name)
        return false;

    // This would require AMX Mod X specific implementation
    // For now, just return success if the CVar exists
    cvar_t* pCvar = CVAR_GET_POINTER(name);
    return pCvar != nullptr;
}

CSHARP_EXPORT bool CSHARP_CALL GetCvarBounds(const char* name, bool* hasMin, float* minValue, bool* hasMax, float* maxValue)
{
    if (!CSharpBridge::g_initialized || !name || !hasMin || !minValue || !hasMax || !maxValue)
        return false;

    // This would require AMX Mod X specific implementation
    // For now, just return no bounds if the CVar exists
    cvar_t* pCvar = CVAR_GET_POINTER(name);
    if (pCvar)
    {
        *hasMin = false;
        *minValue = 0.0f;
        *hasMax = false;
        *maxValue = 0.0f;
        return true;
    }

    return false;
}

// Helper functions for callback handling
namespace CSharpBridge
{
    void HandleMessageCallback(int msgType, int msgDest, int entityId)
    {
        LOCK_ENTER();

        try
        {
            for (size_t i = 0; i < g_messageCallbacks.length(); i++)
            {
                MessageCallbackInfo* info = g_messageCallbacks[i];
                if (info && info->msgId == msgType)
                {
                    info->messageCallback(msgType, msgDest, entityId);
                }
            }
        }
        catch (...)
        {
            // Ignore callback exceptions
        }

        LOCK_LEAVE();
    }

    void HandleCvarCallback(const char* cvarName, const char* oldValue, const char* newValue)
    {
        if (!cvarName || !oldValue || !newValue)
            return;

        LOCK_ENTER();

        try
        {
            for (size_t i = 0; i < g_cvarCallbacks.length(); i++)
            {
                CvarCallbackInfo* info = g_cvarCallbacks[i];
                if (info && info->cvarName.compare(cvarName) == 0)
                {
                    info->cvarCallback(cvarName, oldValue, newValue);
                }
            }
        }
        catch (...)
        {
            // Ignore callback exceptions
        }

        LOCK_LEAVE();
    }
}

// Core AMX functions implementation

// Plugin management functions
CSHARP_EXPORT int CSHARP_CALL GetPluginsNum()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return g_plugins.getPluginsNum();
}

CSHARP_EXPORT bool CSHARP_CALL GetPluginInfo(int pluginId, CSharpPluginInfo* outInfo)
{
    if (!CSharpBridge::g_initialized || !outInfo || pluginId < 0)
        return false;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin)
        return false;

    // Fill plugin information
    strncpy(outInfo->fileName, plugin->getName(), sizeof(outInfo->fileName) - 1);
    outInfo->fileName[sizeof(outInfo->fileName) - 1] = '\0';

    strncpy(outInfo->name, plugin->getTitle(), sizeof(outInfo->name) - 1);
    outInfo->name[sizeof(outInfo->name) - 1] = '\0';

    strncpy(outInfo->version, plugin->getVersion(), sizeof(outInfo->version) - 1);
    outInfo->version[sizeof(outInfo->version) - 1] = '\0';

    strncpy(outInfo->author, plugin->getAuthor(), sizeof(outInfo->author) - 1);
    outInfo->author[sizeof(outInfo->author) - 1] = '\0';

    const char* statusStr = plugin->getStatus();
    strncpy(outInfo->status, statusStr ? statusStr : "", sizeof(outInfo->status) - 1);
    outInfo->status[sizeof(outInfo->status) - 1] = '\0';

    strncpy(outInfo->url, plugin->getUrl(), sizeof(outInfo->url) - 1);
    outInfo->url[sizeof(outInfo->url) - 1] = '\0';

    strncpy(outInfo->description, plugin->getDescription(), sizeof(outInfo->description) - 1);
    outInfo->description[sizeof(outInfo->description) - 1] = '\0';

    outInfo->pluginId = pluginId;
    outInfo->statusCode = plugin->getStatusCode();
    outInfo->isValid = plugin->isValid();
    outInfo->isRunning = plugin->isExecutable();
    outInfo->isPaused = plugin->isPaused();

    return true;
}

CSHARP_EXPORT int CSHARP_CALL FindPlugin(const char* fileName)
{
    if (!CSharpBridge::g_initialized || !fileName)
        return -1;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(fileName);
    return plugin ? plugin->getId() : -1;
}

CSHARP_EXPORT bool CSHARP_CALL IsPluginValid(int pluginId)
{
    if (!CSharpBridge::g_initialized || pluginId < 0)
        return false;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    return plugin ? plugin->isValid() : false;
}

CSHARP_EXPORT bool CSHARP_CALL IsPluginRunning(int pluginId)
{
    if (!CSharpBridge::g_initialized || pluginId < 0)
        return false;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    return plugin ? plugin->isExecutable() : false;
}

CSHARP_EXPORT bool CSHARP_CALL PausePlugin(int pluginId)
{
    if (!CSharpBridge::g_initialized || pluginId < 0)
        return false;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin)
        return false;

    plugin->pausePlugin();
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL UnpausePlugin(int pluginId)
{
    if (!CSharpBridge::g_initialized || pluginId < 0)
        return false;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin)
        return false;

    plugin->unpausePlugin();
    return true;
}

// Function call system implementation
CSHARP_EXPORT bool CSHARP_CALL CallFuncBegin(const char* funcName, const char* pluginName)
{
    if (!CSharpBridge::g_initialized || !funcName)
        return false;

    CSharpBridge::AutoLock lock;

    // Clean up any previous call
    CSharpBridge::CleanupCallFunc();

    CPluginMngr::CPlugin* plugin = nullptr;
    if (pluginName && strlen(pluginName) > 0)
    {
        plugin = CSharpBridge::FindPluginByName(pluginName);
    }
    else
    {
        // Find first valid plugin with this function
        for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
        {
            if ((*iter).isValid() && (*iter).isExecutable())
            {
                int funcId = amx_FindPublic((*iter).getAMX(), funcName);
                if (funcId >= 0)
                {
                    plugin = &(*iter);
                    break;
                }
            }
        }
    }

    if (!plugin || !plugin->isValid() || !plugin->isExecutable())
        return false;

    int funcId = amx_FindPublic(plugin->getAMX(), funcName);
    if (funcId < 0)
        return false;

    CSharpBridge::g_callFuncInfo.plugin = plugin;
    CSharpBridge::g_callFuncInfo.funcId = funcId;
    CSharpBridge::g_callFuncInfo.currentParam = 0;
    CSharpBridge::g_callFuncInfo.isActive = true;

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL CallFuncBeginById(int funcId, int pluginId)
{
    if (!CSharpBridge::g_initialized || funcId < 0 || pluginId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Clean up any previous call
    CSharpBridge::CleanupCallFunc();

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin || !plugin->isValid() || !plugin->isExecutable())
        return false;

    CSharpBridge::g_callFuncInfo.plugin = plugin;
    CSharpBridge::g_callFuncInfo.funcId = funcId;
    CSharpBridge::g_callFuncInfo.currentParam = 0;
    CSharpBridge::g_callFuncInfo.isActive = true;

    return true;
}

CSHARP_EXPORT bool CSHARP_CALL CallFuncPushInt(int value)
{
    if (!CSharpBridge::g_initialized || !CSharpBridge::g_callFuncInfo.isActive)
        return false;

    CSharpBridge::AutoLock lock;

    AMX* amx = CSharpBridge::g_callFuncInfo.plugin->getAMX();
    if (!amx)
        return false;

    cell amxValue = static_cast<cell>(value);
    int error = amx_Push(amx, amxValue);

    if (error == AMX_ERR_NONE)
    {
        CSharpBridge::g_callFuncInfo.currentParam++;
        return true;
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL CallFuncPushFloat(float value)
{
    if (!CSharpBridge::g_initialized || !CSharpBridge::g_callFuncInfo.isActive)
        return false;

    CSharpBridge::AutoLock lock;

    AMX* amx = CSharpBridge::g_callFuncInfo.plugin->getAMX();
    if (!amx)
        return false;

    cell amxValue = amx_ftoc(value);
    int error = amx_Push(amx, amxValue);

    if (error == AMX_ERR_NONE)
    {
        CSharpBridge::g_callFuncInfo.currentParam++;
        return true;
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL CallFuncPushString(const char* value)
{
    if (!CSharpBridge::g_initialized || !CSharpBridge::g_callFuncInfo.isActive || !value)
        return false;

    CSharpBridge::AutoLock lock;

    AMX* amx = CSharpBridge::g_callFuncInfo.plugin->getAMX();
    if (!amx)
        return false;

    cell amxAddr;
    int error = amx_PushString(amx, &amxAddr, nullptr, value, 0, 0);

    if (error == AMX_ERR_NONE)
    {
        CSharpBridge::g_callFuncInfo.currentParam++;
        return true;
    }

    return false;
}

CSHARP_EXPORT bool CSHARP_CALL CallFuncPushArray(const int* array, int size)
{
    if (!CSharpBridge::g_initialized || !CSharpBridge::g_callFuncInfo.isActive || !array || size <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    AMX* amx = CSharpBridge::g_callFuncInfo.plugin->getAMX();
    if (!amx)
        return false;

    cell amxAddr;
    cell* physAddr;
    int error = amx_PushArray(amx, &amxAddr, &physAddr, reinterpret_cast<const cell*>(array), size);

    if (error == AMX_ERR_NONE)
    {
        CSharpBridge::g_callFuncInfo.currentParam++;
        return true;
    }

    return false;
}

CSHARP_EXPORT int CSHARP_CALL CallFuncEnd()
{
    if (!CSharpBridge::g_initialized || !CSharpBridge::g_callFuncInfo.isActive)
        return 0;

    CSharpBridge::AutoLock lock;

    AMX* amx = CSharpBridge::g_callFuncInfo.plugin->getAMX();
    if (!amx)
    {
        CSharpBridge::CleanupCallFunc();
        return 0;
    }

    cell retVal = 0;
    int error = amx_Exec(amx, &retVal, CSharpBridge::g_callFuncInfo.funcId);

    // Clean up the call
    CSharpBridge::CleanupCallFunc();

    if (error == AMX_ERR_NONE)
        return static_cast<int>(retVal);

    return 0;
}

CSHARP_EXPORT int CSHARP_CALL GetFuncId(const char* funcName, int pluginId)
{
    if (!CSharpBridge::g_initialized || !funcName || pluginId < 0)
        return -1;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin || !plugin->isValid())
        return -1;

    return amx_FindPublic(plugin->getAMX(), funcName);
}

// Forward system implementation
CSHARP_EXPORT int CSHARP_CALL CreateForward(const char* funcName, int execType, int paramTypes[], int paramCount)
{
    if (!CSharpBridge::g_initialized || !funcName || paramCount < 0 || paramCount > 16)
        return -1;

    CSharpBridge::AutoLock lock;

    int forwardId = registerForward(funcName, static_cast<ForwardExecType>(execType),
                                   paramTypes, paramCount);

    if (forwardId >= 0)
    {
        CSharpBridge::ForwardCallbackInfo* info = new CSharpBridge::ForwardCallbackInfo();
        info->forwardId = forwardId;
        info->callback = nullptr;
        info->name = funcName;
        info->execType = execType;
        info->paramCount = paramCount;

        for (int i = 0; i < paramCount && i < 16; i++)
        {
            info->paramTypes[i] = paramTypes[i];
        }

        CSharpBridge::g_forwardCallbacks.append(info);
    }

    return forwardId;
}

CSHARP_EXPORT int CSHARP_CALL CreateSPForward(const char* funcName, int pluginId, int paramTypes[], int paramCount)
{
    if (!CSharpBridge::g_initialized || !funcName || pluginId < 0 || paramCount < 0 || paramCount > 16)
        return -1;

    CSharpBridge::AutoLock lock;

    CPluginMngr::CPlugin* plugin = g_plugins.findPlugin(pluginId);
    if (!plugin || !plugin->isValid())
        return -1;

    int forwardId = registerSPForward(plugin->getAMX(), funcName, paramTypes, paramCount);

    if (forwardId >= 0)
    {
        CSharpBridge::ForwardCallbackInfo* info = new CSharpBridge::ForwardCallbackInfo();
        info->forwardId = forwardId;
        info->callback = nullptr;
        info->name = funcName;
        info->execType = 0; // Single plugin forward
        info->paramCount = paramCount;

        for (int i = 0; i < paramCount && i < 16; i++)
        {
            info->paramTypes[i] = paramTypes[i];
        }

        CSharpBridge::g_forwardCallbacks.append(info);
    }

    return forwardId;
}

CSHARP_EXPORT bool CSHARP_CALL DestroyForward(int forwardId)
{
    if (!CSharpBridge::g_initialized || forwardId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove forward callback info
    for (size_t i = 0; i < CSharpBridge::g_forwardCallbacks.length(); i++)
    {
        CSharpBridge::ForwardCallbackInfo* info = CSharpBridge::g_forwardCallbacks[i];
        if (info && info->forwardId == forwardId)
        {
            delete info;
            CSharpBridge::g_forwardCallbacks[i] = nullptr;
            break;
        }
    }

    unregisterSPForward(forwardId);
    return true;
}

CSHARP_EXPORT int CSHARP_CALL ExecuteForward(int forwardId, int params[], int paramCount)
{
    if (!CSharpBridge::g_initialized || forwardId < 0 || paramCount < 0)
        return 0;

    CSharpBridge::AutoLock lock;

    cell retVal = 0;
    int result = executeForwards(forwardId, retVal, params, paramCount);

    return (result > 0) ? static_cast<int>(retVal) : 0;
}

CSHARP_EXPORT bool CSHARP_CALL GetForwardInfo(int forwardId, CSharpForwardInfo* outInfo)
{
    if (!CSharpBridge::g_initialized || forwardId < 0 || !outInfo)
        return false;

    CSharpBridge::AutoLock lock;

    // Find forward info
    for (size_t i = 0; i < CSharpBridge::g_forwardCallbacks.length(); i++)
    {
        CSharpBridge::ForwardCallbackInfo* info = CSharpBridge::g_forwardCallbacks[i];
        if (info && info->forwardId == forwardId)
        {
            strncpy(outInfo->name, info->name.chars(), sizeof(outInfo->name) - 1);
            outInfo->name[sizeof(outInfo->name) - 1] = '\0';

            outInfo->forwardId = forwardId;
            outInfo->paramCount = info->paramCount;
            outInfo->execType = info->execType;
            outInfo->isValid = true;

            return true;
        }
    }

    return false;
}

// Server functions implementation
CSHARP_EXPORT bool CSHARP_CALL ServerPrint(const char* message)
{
    if (!CSharpBridge::g_initialized || !message)
        return false;

    SERVER_PRINT(message);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL ServerCmd(const char* command)
{
    if (!CSharpBridge::g_initialized || !command)
        return false;

    SERVER_COMMAND(command);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL ServerExec()
{
    if (!CSharpBridge::g_initialized)
        return false;

    SERVER_EXECUTE();
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL IsDedicatedServer()
{
    if (!CSharpBridge::g_initialized)
        return false;

    return IS_DEDICATED_SERVER() != 0;
}

CSHARP_EXPORT bool CSHARP_CALL IsLinuxServer()
{
    if (!CSharpBridge::g_initialized)
        return false;

// ========== Engine Module C# Bridge Implementation ==========

// Include engine module headers
#include "modules/engine/engine.h"

// Helper function to convert CSharpVector3 to Vector
Vector CSharpVectorToVector(const CSharpVector3* vec)
{
    return Vector(vec->x, vec->y, vec->z);
}

// Helper function to convert Vector to CSharpVector3
void VectorToCSharpVector(const Vector& vec, CSharpVector3* outVec)
{
    outVec->x = vec.x;
    outVec->y = vec.y;
    outVec->z = vec.z;
}

// ========== Entity Lifecycle Management Implementation ==========

CSHARP_EXPORT int CSHARP_CALL CreateEntity(const char* className)
{
    if (!CSharpBridge::g_initialized || !className)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        // Create named entity using engine function
        int iszClass = ALLOC_STRING(className);
        edict_t* pEnt = CREATE_NAMED_ENTITY(iszClass);

        if (FNullEnt(pEnt))
            return 0;

        return ENTINDEX(pEnt);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL RemoveEntity(int entityId)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        REMOVE_ENTITY(pEnt);
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityCount()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        int count = 0;
        for (int i = 1; i <= gpGlobals->maxEntities; i++)
        {
            edict_t* pEnt = INDEXENT(i);
            if (!FNullEnt(pEnt) && !pEnt->free)
                count++;
        }
        return count;
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL IsValidEntity(int entityId)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        return !FNullEnt(pEnt) && !pEnt->free;
    }
    catch (...)
    {
        return false;
    }
}

// ========== Entity Property Read/Write Implementation ==========

CSHARP_EXPORT float CSHARP_CALL GetEntityFloat(int entityId, int property)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return 0.0f;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return 0.0f;

        // Call the engine module's entity_get_float function
        cell params[3];
        params[0] = 2 * sizeof(cell);
        params[1] = entityId;
        params[2] = property;

        // This would need to call the actual engine module function
        // For now, we'll implement basic property access
        switch (property)
        {
            case 0: return pEnt->v.health;           // EV_FL_health
            case 1: return pEnt->v.max_health;       // EV_FL_max_health
            case 2: return pEnt->v.speed;            // EV_FL_speed
            case 3: return pEnt->v.gravity;          // EV_FL_gravity
            case 4: return pEnt->v.takedamage;       // EV_FL_takedamage
            case 5: return pEnt->v.dmg;              // EV_FL_dmg
            default: return 0.0f;
        }
    }
    catch (...)
    {
        return 0.0f;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityFloat(int entityId, int property, float value)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        switch (property)
        {
            case 0: pEnt->v.health = value; break;           // EV_FL_health
            case 1: pEnt->v.max_health = value; break;       // EV_FL_max_health
            case 2: pEnt->v.speed = value; break;            // EV_FL_speed
            case 3: pEnt->v.gravity = value; break;          // EV_FL_gravity
            case 4: pEnt->v.takedamage = value; break;       // EV_FL_takedamage
            case 5: pEnt->v.dmg = value; break;              // EV_FL_dmg
            default: return false;
        }
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityInt(int entityId, int property)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return 0;

        switch (property)
        {
            case 0: return static_cast<int>(pEnt->v.movetype);     // EV_INT_movetype
            case 1: return static_cast<int>(pEnt->v.solid);        // EV_INT_solid
            case 2: return static_cast<int>(pEnt->v.skin);         // EV_INT_skin
            case 3: return static_cast<int>(pEnt->v.body);         // EV_INT_body
            case 4: return static_cast<int>(pEnt->v.effects);      // EV_INT_effects
            case 5: return static_cast<int>(pEnt->v.sequence);     // EV_INT_sequence
            case 6: return static_cast<int>(pEnt->v.modelindex);   // EV_INT_modelindex
            case 7: return static_cast<int>(pEnt->v.waterlevel);   // EV_INT_waterlevel
            case 8: return static_cast<int>(pEnt->v.watertype);    // EV_INT_watertype
            case 9: return static_cast<int>(pEnt->v.spawnflags);   // EV_INT_spawnflags
            case 10: return static_cast<int>(pEnt->v.flags);       // EV_INT_flags
            case 11: return static_cast<int>(pEnt->v.team);        // EV_INT_team
            case 12: return static_cast<int>(pEnt->v.weapons);     // EV_INT_weapons
            case 13: return static_cast<int>(pEnt->v.rendermode);  // EV_INT_rendermode
            case 14: return static_cast<int>(pEnt->v.renderfx);    // EV_INT_renderfx
            case 15: return static_cast<int>(pEnt->v.button);      // EV_INT_button
            case 16: return static_cast<int>(pEnt->v.impulse);     // EV_INT_impulse
            case 17: return static_cast<int>(pEnt->v.deadflag);    // EV_INT_deadflag
            default: return 0;
        }
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityInt(int entityId, int property, int value)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        switch (property)
        {
            case 0: pEnt->v.movetype = value; break;     // EV_INT_movetype
            case 1: pEnt->v.solid = value; break;        // EV_INT_solid
            case 2: pEnt->v.skin = value; break;         // EV_INT_skin
            case 3: pEnt->v.body = value; break;         // EV_INT_body
            case 4: pEnt->v.effects = value; break;      // EV_INT_effects
            case 5: pEnt->v.sequence = value; break;     // EV_INT_sequence
            case 6: pEnt->v.modelindex = value; break;   // EV_INT_modelindex
            case 7: pEnt->v.waterlevel = value; break;   // EV_INT_waterlevel
            case 8: pEnt->v.watertype = value; break;    // EV_INT_watertype
            case 9: pEnt->v.spawnflags = value; break;   // EV_INT_spawnflags
            case 10: pEnt->v.flags = value; break;       // EV_INT_flags
            case 11: pEnt->v.team = value; break;        // EV_INT_team
            case 12: pEnt->v.weapons = value; break;     // EV_INT_weapons
            case 13: pEnt->v.rendermode = value; break;  // EV_INT_rendermode
            case 14: pEnt->v.renderfx = value; break;    // EV_INT_renderfx
            case 15: pEnt->v.button = value; break;      // EV_INT_button
            case 16: pEnt->v.impulse = value; break;     // EV_INT_impulse
            case 17: pEnt->v.deadflag = value; break;    // EV_INT_deadflag
            default: return false;
        }
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL GetEntityVector(int entityId, int property, CSharpVector3* vector)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !vector)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        Vector result;
        switch (property)
        {
            case 0: result = pEnt->v.origin; break;        // EV_VEC_origin
            case 1: result = pEnt->v.oldorigin; break;     // EV_VEC_oldorigin
            case 2: result = pEnt->v.velocity; break;      // EV_VEC_velocity
            case 3: result = pEnt->v.basevelocity; break;  // EV_VEC_basevelocity
            case 4: result = pEnt->v.movedir; break;       // EV_VEC_movedir
            case 5: result = pEnt->v.angles; break;        // EV_VEC_angles
            case 6: result = pEnt->v.avelocity; break;     // EV_VEC_avelocity
            case 7: result = pEnt->v.punchangle; break;    // EV_VEC_punchangle
            case 8: result = pEnt->v.v_angle; break;       // EV_VEC_v_angle
            case 9: result = pEnt->v.endpos; break;        // EV_VEC_endpos
            case 10: result = pEnt->v.startpos; break;     // EV_VEC_startpos
            case 11: result = pEnt->v.absmin; break;       // EV_VEC_absmin
            case 12: result = pEnt->v.absmax; break;       // EV_VEC_absmax
            case 13: result = pEnt->v.mins; break;         // EV_VEC_mins
            case 14: result = pEnt->v.maxs; break;         // EV_VEC_maxs
            case 15: result = pEnt->v.size; break;         // EV_VEC_size
            case 16: result = pEnt->v.rendercolor; break;  // EV_VEC_rendercolor
            case 17: result = pEnt->v.view_ofs; break;     // EV_VEC_view_ofs
            default: return false;
        }

        VectorToCSharpVector(result, vector);
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityVector(int entityId, int property, const CSharpVector3* vector)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !vector)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        Vector value = CSharpVectorToVector(vector);
        switch (property)
        {
            case 0: pEnt->v.origin = value; break;        // EV_VEC_origin
            case 1: pEnt->v.oldorigin = value; break;     // EV_VEC_oldorigin
            case 2: pEnt->v.velocity = value; break;      // EV_VEC_velocity
            case 3: pEnt->v.basevelocity = value; break;  // EV_VEC_basevelocity
            case 4: pEnt->v.movedir = value; break;       // EV_VEC_movedir
            case 5: pEnt->v.angles = value; break;        // EV_VEC_angles
            case 6: pEnt->v.avelocity = value; break;     // EV_VEC_avelocity
            case 7: pEnt->v.punchangle = value; break;    // EV_VEC_punchangle
            case 8: pEnt->v.v_angle = value; break;       // EV_VEC_v_angle
            case 9: pEnt->v.endpos = value; break;        // EV_VEC_endpos
            case 10: pEnt->v.startpos = value; break;     // EV_VEC_startpos
            case 11: pEnt->v.absmin = value; break;       // EV_VEC_absmin
            case 12: pEnt->v.absmax = value; break;       // EV_VEC_absmax
            case 13: pEnt->v.mins = value; break;         // EV_VEC_mins
            case 14: pEnt->v.maxs = value; break;         // EV_VEC_maxs
            case 15: pEnt->v.size = value; break;         // EV_VEC_size
            case 16: pEnt->v.rendercolor = value; break;  // EV_VEC_rendercolor
            case 17: pEnt->v.view_ofs = value; break;     // EV_VEC_view_ofs
            default: return false;
        }
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityString(int entityId, int property, char* buffer, int maxLength)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !buffer || maxLength <= 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return 0;

        const char* result = nullptr;
        switch (property)
        {
            case 0: result = STRING(pEnt->v.classname); break;    // EV_SZ_classname
            case 1: result = STRING(pEnt->v.globalname); break;   // EV_SZ_globalname
            case 2: result = STRING(pEnt->v.model); break;        // EV_SZ_model
            case 3: result = STRING(pEnt->v.target); break;       // EV_SZ_target
            case 4: result = STRING(pEnt->v.targetname); break;   // EV_SZ_targetname
            case 5: result = STRING(pEnt->v.netname); break;      // EV_SZ_netname
            case 6: result = STRING(pEnt->v.message); break;      // EV_SZ_message
            case 7: result = STRING(pEnt->v.noise); break;        // EV_SZ_noise
            case 8: result = STRING(pEnt->v.noise1); break;       // EV_SZ_noise1
            case 9: result = STRING(pEnt->v.noise2); break;       // EV_SZ_noise2
            case 10: result = STRING(pEnt->v.noise3); break;      // EV_SZ_noise3
            default: return 0;
        }

        if (result)
        {
            int len = strlen(result);
            if (len >= maxLength)
                len = maxLength - 1;
            strncpy(buffer, result, len);
            buffer[len] = '\0';
            return len;
        }
        return 0;
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityString(int entityId, int property, const char* value)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !value)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        int stringIndex = ALLOC_STRING(value);
        switch (property)
        {
            case 0: pEnt->v.classname = stringIndex; break;    // EV_SZ_classname
            case 1: pEnt->v.globalname = stringIndex; break;   // EV_SZ_globalname
            case 2: pEnt->v.model = stringIndex; break;        // EV_SZ_model
            case 3: pEnt->v.target = stringIndex; break;       // EV_SZ_target
            case 4: pEnt->v.targetname = stringIndex; break;   // EV_SZ_targetname
            case 5: pEnt->v.netname = stringIndex; break;      // EV_SZ_netname
            case 6: pEnt->v.message = stringIndex; break;      // EV_SZ_message
            case 7: pEnt->v.noise = stringIndex; break;        // EV_SZ_noise
            case 8: pEnt->v.noise1 = stringIndex; break;       // EV_SZ_noise1
            case 9: pEnt->v.noise2 = stringIndex; break;       // EV_SZ_noise2
            case 10: pEnt->v.noise3 = stringIndex; break;      // EV_SZ_noise3
            default: return false;
        }
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT int CSHARP_CALL GetEntityEdict(int entityId, int property)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return 0;

        edict_t* result = nullptr;
        switch (property)
        {
            case 0: result = pEnt->v.chain; break;           // EV_ENT_chain
            case 1: result = pEnt->v.dmg_inflictor; break;   // EV_ENT_dmg_inflictor
            case 2: result = pEnt->v.enemy; break;           // EV_ENT_enemy
            case 3: result = pEnt->v.aiment; break;          // EV_ENT_aiment
            case 4: result = pEnt->v.owner; break;           // EV_ENT_owner
            case 5: result = pEnt->v.groundentity; break;    // EV_ENT_groundentity
            case 6: result = pEnt->v.pContainingEntity; break; // EV_ENT_pContainingEntity
            default: return 0;
        }

        return result ? ENTINDEX(result) : 0;
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityEdict(int entityId, int property, int targetEntityId)
{
    if (!CSharpBridge::g_initialized || entityId <= 0)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        edict_t* pTarget = (targetEntityId > 0) ? INDEXENT(targetEntityId) : nullptr;
        if (targetEntityId > 0 && FNullEnt(pTarget))
            return false;

        switch (property)
        {
            case 0: pEnt->v.chain = pTarget; break;           // EV_ENT_chain
            case 1: pEnt->v.dmg_inflictor = pTarget; break;   // EV_ENT_dmg_inflictor
            case 2: pEnt->v.enemy = pTarget; break;           // EV_ENT_enemy
            case 3: pEnt->v.aiment = pTarget; break;          // EV_ENT_aiment
            case 4: pEnt->v.owner = pTarget; break;           // EV_ENT_owner
            case 5: pEnt->v.groundentity = pTarget; break;    // EV_ENT_groundentity
            case 6: pEnt->v.pContainingEntity = pTarget; break; // EV_ENT_pContainingEntity
            default: return false;
        }
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityOrigin(int entityId, const CSharpVector3* origin)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !origin)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        Vector newOrigin = CSharpVectorToVector(origin);
        SET_ORIGIN(pEnt, newOrigin);
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntityModel(int entityId, const char* modelName)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !modelName)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        SET_MODEL(pEnt, modelName);
        return true;
    }
    catch (...)
    {
        return false;
    }
}

CSHARP_EXPORT bool CSHARP_CALL SetEntitySize(int entityId, const CSharpVector3* mins, const CSharpVector3* maxs)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || !mins || !maxs)
        return false;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt = INDEXENT(entityId);
        if (FNullEnt(pEnt))
            return false;

        Vector vMins = CSharpVectorToVector(mins);
        Vector vMaxs = CSharpVectorToVector(maxs);
        SET_SIZE(pEnt, vMins, vMaxs);
        return true;
    }
    catch (...)
    {
        return false;
    }
}

// ========== Entity Search and Finding Implementation ==========

CSHARP_EXPORT float CSHARP_CALL GetEntityRange(int entityId, int targetEntityId)
{
    if (!CSharpBridge::g_initialized || entityId <= 0 || targetEntityId <= 0)
        return 0.0f;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pEnt1 = INDEXENT(entityId);
        edict_t* pEnt2 = INDEXENT(targetEntityId);
        if (FNullEnt(pEnt1) || FNullEnt(pEnt2))
            return 0.0f;

        Vector diff = pEnt1->v.origin - pEnt2->v.origin;
        return diff.Length();
    }
    catch (...)
    {
        return 0.0f;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityInSphere(int startEntityId, const CSharpVector3* origin, float radius)
{
    if (!CSharpBridge::g_initialized || !origin || radius < 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        Vector searchOrigin = CSharpVectorToVector(origin);
        edict_t* pStart = (startEntityId > 0) ? INDEXENT(startEntityId) : nullptr;

        edict_t* pFound = FIND_ENTITY_IN_SPHERE(pStart, searchOrigin, radius);
        return FNullEnt(pFound) ? 0 : ENTINDEX(pFound);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByClass(int startEntityId, const char* className)
{
    if (!CSharpBridge::g_initialized || !className)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pStart = (startEntityId > 0) ? INDEXENT(startEntityId) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "classname", className);
        return FNullEnt(pFound) ? 0 : ENTINDEX(pFound);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByClassInSphere(int startEntityId, const CSharpVector3* origin, float radius, const char* className)
{
    if (!CSharpBridge::g_initialized || !origin || !className || radius < 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        Vector searchOrigin = CSharpVectorToVector(origin);
        int currentEntity = startEntityId;

        while ((currentEntity = FindEntityInSphere(currentEntity, origin, radius)) > 0)
        {
            edict_t* pEnt = INDEXENT(currentEntity);
            if (!FNullEnt(pEnt))
            {
                const char* entClassName = STRING(pEnt->v.classname);
                if (entClassName && strcmp(entClassName, className) == 0)
                    return currentEntity;
            }
        }
        return 0;
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByModel(int startEntityId, const char* modelName)
{
    if (!CSharpBridge::g_initialized || !modelName)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pStart = (startEntityId > 0) ? INDEXENT(startEntityId) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "model", modelName);
        return FNullEnt(pFound) ? 0 : ENTINDEX(pFound);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByTarget(int startEntityId, const char* targetName)
{
    if (!CSharpBridge::g_initialized || !targetName)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pStart = (startEntityId > 0) ? INDEXENT(startEntityId) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "target", targetName);
        return FNullEnt(pFound) ? 0 : ENTINDEX(pFound);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByTargetName(int startEntityId, const char* targetName)
{
    if (!CSharpBridge::g_initialized || !targetName)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pStart = (startEntityId > 0) ? INDEXENT(startEntityId) : nullptr;
        edict_t* pFound = FIND_ENTITY_BY_STRING(pStart, "targetname", targetName);
        return FNullEnt(pFound) ? 0 : ENTINDEX(pFound);
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindEntityByOwner(int startEntityId, int ownerEntityId)
{
    if (!CSharpBridge::g_initialized || ownerEntityId <= 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pOwner = INDEXENT(ownerEntityId);
        if (FNullEnt(pOwner))
            return 0;

        for (int i = startEntityId + 1; i <= gpGlobals->maxEntities; i++)
        {
            edict_t* pEnt = INDEXENT(i);
            if (!FNullEnt(pEnt) && !pEnt->free && pEnt->v.owner == pOwner)
                return i;
        }
        return 0;
    }
    catch (...)
    {
        return 0;
    }
}

CSHARP_EXPORT int CSHARP_CALL FindGrenadeEntity(int playerId, char* modelName, int maxLength, int startEntityId)
{
    if (!CSharpBridge::g_initialized || playerId <= 0 || !modelName || maxLength <= 0)
        return 0;

    CSharpBridge::AutoLock lock;

    try
    {
        edict_t* pPlayer = INDEXENT(playerId);
        if (FNullEnt(pPlayer))
            return 0;

        for (int i = startEntityId + 1; i <= gpGlobals->maxEntities; i++)
        {
            edict_t* pEnt = INDEXENT(i);
            if (!FNullEnt(pEnt) && !pEnt->free && pEnt->v.owner == pPlayer)
            {
                const char* entClassName = STRING(pEnt->v.classname);
                if (entClassName && strstr(entClassName, "grenade"))
                {
                    const char* entModel = STRING(pEnt->v.model);
                    if (entModel)
                    {
                        int len = strlen(entModel);
                        if (len >= maxLength)
                            len = maxLength - 1;
                        strncpy(modelName, entModel, len);
                        modelName[len] = '\0';
                    }
                    return i;
                }
            }
        }
        return 0;
    }
    catch (...)
    {
        return 0;
    }
}

#ifdef __linux__
    return true;
#else
    return false;
#endif
}

CSHARP_EXPORT bool CSHARP_CALL IsMapValid(const char* mapName)
{
    if (!CSharpBridge::g_initialized || !mapName)
        return false;

    return IS_MAP_VALID(mapName) != 0;
}

// Client functions implementation
CSHARP_EXPORT int CSHARP_CALL GetPlayersNum(bool includeConnecting)
{
    if (!CSharpBridge::g_initialized)
        return 0;

    int count = 0;
    for (int i = 1; i <= gpGlobals->maxClients; i++)
    {
        CPlayer* pPlayer = GET_PLAYER_POINTER_I(i);
        if (pPlayer && (pPlayer->ingame || (includeConnecting && pPlayer->initialized)))
            count++;
    }

    return count;
}

CSHARP_EXPORT bool CSHARP_CALL IsUserBot(int clientId)
{
    if (!CSharpBridge::g_initialized || clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer)
        return false;

    return pPlayer->IsBot();
}

CSHARP_EXPORT bool CSHARP_CALL IsUserConnected(int clientId)
{
    if (!CSharpBridge::g_initialized || clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    return pPlayer && pPlayer->ingame;
}

CSHARP_EXPORT bool CSHARP_CALL IsUserAlive(int clientId)
{
    if (!CSharpBridge::g_initialized || clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    return pPlayer->IsAlive();
}

CSHARP_EXPORT int CSHARP_CALL GetUserTime(int clientId, bool playtime)
{
    if (!CSharpBridge::g_initialized || clientId < 1 || clientId > gpGlobals->maxClients)
        return 0;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer)
        return 0;

    if (playtime)
        return static_cast<int>(gpGlobals->time - pPlayer->time);
    else
        return static_cast<int>(gpGlobals->time - pPlayer->connectTime);
}

CSHARP_EXPORT bool CSHARP_CALL ClientCmd(int clientId, const char* command)
{
    if (!CSharpBridge::g_initialized || !command)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    CLIENT_COMMAND(pPlayer->pEdict, command);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL FakeClientCmd(int clientId, const char* command)
{
    if (!CSharpBridge::g_initialized || !command)
        return false;

    if (clientId < 1 || clientId > gpGlobals->maxClients)
        return false;

    CPlayer* pPlayer = GET_PLAYER_POINTER_I(clientId);
    if (!pPlayer || !pPlayer->pEdict)
        return false;

    FAKE_CLIENT_COMMAND(pPlayer->pEdict, command);
    return true;
}

// Admin management functions implementation
CSHARP_EXPORT bool CSHARP_CALL AdminsPush(const char* authData, const char* password, int access, int flags)
{
    if (!CSharpBridge::g_initialized || !authData)
        return false;

    CSharpBridge::AutoLock lock;

    AdminInfo adminInfo;
    adminInfo.setAccess(access);
    adminInfo.setFlags(flags);
    adminInfo.setAuth(authData);

    if (password && strlen(password) > 0)
        adminInfo.setPassword(password);

    g_admins.push_back(adminInfo);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL AdminsFlush()
{
    if (!CSharpBridge::g_initialized)
        return false;

    CSharpBridge::AutoLock lock;

    g_admins.clear();
    return true;
}

CSHARP_EXPORT int CSHARP_CALL AdminsNum()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    return static_cast<int>(g_admins.size());
}

CSHARP_EXPORT bool CSHARP_CALL AdminsLookup(int index, int property, char* buffer, int bufferSize, int* outValue)
{
    if (!CSharpBridge::g_initialized || index < 0 || index >= static_cast<int>(g_admins.size()))
        return false;

    CSharpBridge::AutoLock lock;

    const AdminInfo& admin = g_admins[index];

    switch (property)
    {
        case 0: // Auth data
            if (buffer && bufferSize > 0)
            {
                strncpy(buffer, admin.getAuth(), bufferSize - 1);
                buffer[bufferSize - 1] = '\0';
                return true;
            }
            break;

        case 1: // Password
            if (buffer && bufferSize > 0)
            {
                strncpy(buffer, admin.getPassword(), bufferSize - 1);
                buffer[bufferSize - 1] = '\0';
                return true;
            }
            break;

        case 2: // Access flags
            if (outValue)
            {
                *outValue = admin.getAccess();
                return true;
            }
            break;

        case 3: // Admin flags
            if (outValue)
            {
                *outValue = admin.getFlags();
                return true;
            }
            break;
    }

    return false;
}

// Logging functions implementation
CSHARP_EXPORT bool CSHARP_CALL LogToFile(const char* fileName, const char* message)
{
    if (!CSharpBridge::g_initialized || !fileName || !message)
        return false;

    UTIL_LogPrintf("[AMXX] %s", message);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL LogAmx(const char* message)
{
    if (!CSharpBridge::g_initialized || !message)
        return false;

    AMXXLOG_Log("[AMXX] %s", message);
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL LogError(int errorCode, const char* message)
{
    if (!CSharpBridge::g_initialized || !message)
        return false;

    LogError(static_cast<AMX*>(nullptr), errorCode, "%s", message);
    return true;
}

CSHARP_EXPORT int CSHARP_CALL RegisterLogCallback(CSharpLogCallback callback)
{
    if (!CSharpBridge::g_initialized || !callback)
        return -1;

    return CSharpBridge::RegisterLogCallbackInternal(callback);
}

CSHARP_EXPORT bool CSHARP_CALL UnregisterLogCallback(int callbackId)
{
    if (!CSharpBridge::g_initialized || callbackId < 0)
        return false;

    CSharpBridge::AutoLock lock;

    // Find and remove log callback
    for (size_t i = 0; i < CSharpBridge::g_logCallbacks.length(); i++)
    {
        CSharpBridge::LogCallbackInfo* info = CSharpBridge::g_logCallbacks[i];
        if (info && info->callbackId == callbackId)
        {
            delete info;
            CSharpBridge::g_logCallbacks[i] = nullptr;
            return true;
        }
    }

    return false;
}

// Library management functions implementation
CSHARP_EXPORT bool CSHARP_CALL RegisterLibrary(const char* libraryName)
{
    if (!CSharpBridge::g_initialized || !libraryName)
        return false;

    CSharpBridge::AutoLock lock;

    g_libraries.push_back(ke::AString(libraryName));
    return true;
}

CSHARP_EXPORT bool CSHARP_CALL LibraryExists(const char* libraryName)
{
    if (!CSharpBridge::g_initialized || !libraryName)
        return false;

    CSharpBridge::AutoLock lock;

    for (size_t i = 0; i < g_libraries.length(); i++)
    {
        if (strcmp(g_libraries[i].chars(), libraryName) == 0)
            return true;
    }

    return false;
}

// Utility functions implementation
CSHARP_EXPORT bool CSHARP_CALL AbortExecution(int errorCode, const char* message)
{
    if (!CSharpBridge::g_initialized)
        return false;

    // This would typically abort the current AMX execution
    // Implementation depends on the current execution context
    if (message)
        AMXXLOG_Log("[ABORT] Error %d: %s", errorCode, message);

    return true;
}

CSHARP_EXPORT int CSHARP_CALL GetHeapSpace()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    // This would need access to current AMX context
    // Simplified implementation
    return 65536; // Default heap space
}

CSHARP_EXPORT int CSHARP_CALL GetNumArgs()
{
    if (!CSharpBridge::g_initialized)
        return 0;

    // This would need access to current AMX context
    // Simplified implementation
    return 0;
}

CSHARP_EXPORT bool CSHARP_CALL SwapChars(char* string, int char1, int char2)
{
    if (!CSharpBridge::g_initialized || !string)
        return false;

    int len = strlen(string);
    for (int i = 0; i < len; i++)
    {
        if (string[i] == char1)
            string[i] = char2;
        else if (string[i] == char2)
            string[i] = char1;
    }

    return true;
}

CSHARP_EXPORT int CSHARP_CALL RandomInt(int max)
{
    if (!CSharpBridge::g_initialized || max <= 0)
        return 0;

    return RANDOM_LONG(0, max - 1);
}

CSHARP_EXPORT int CSHARP_CALL MinInt(int a, int b)
{
    return (a < b) ? a : b;
}

CSHARP_EXPORT int CSHARP_CALL MaxInt(int a, int b)
{
    return (a > b) ? a : b;
}

CSHARP_EXPORT int CSHARP_CALL ClampInt(int value, int min, int max)
{
    if (value < min) return min;
    if (value > max) return max;
    return value;
}

// Internal helper functions implementation
int CSharpBridge::RegisterLogCallbackInternal(CSharpLogCallback callback)
{
    AutoLock lock;

    LogCallbackInfo* info = new LogCallbackInfo();
    info->callbackId = g_nextLogCallbackId++;
    info->callback = callback;

    g_logCallbacks.append(info);

    return info->callbackId;
}

void CSharpBridge::HandleLogCallback(int logLevel, const char* message)
{
    AutoLock lock;

    for (size_t i = 0; i < g_logCallbacks.length(); i++)
    {
        LogCallbackInfo* info = g_logCallbacks[i];
        if (info && info->callback)
        {
            info->callback(logLevel, message);
        }
    }
}

bool CSharpBridge::InitializeCallFunc()
{
    g_callFuncInfo.plugin = nullptr;
    g_callFuncInfo.funcId = -1;
    g_callFuncInfo.currentParam = 0;
    g_callFuncInfo.isActive = false;
    return true;
}

void CSharpBridge::CleanupCallFunc()
{
    g_callFuncInfo.plugin = nullptr;
    g_callFuncInfo.funcId = -1;
    g_callFuncInfo.currentParam = 0;
    g_callFuncInfo.isActive = false;
}

CPluginMngr::CPlugin* CSharpBridge::FindPluginByName(const char* pluginName)
{
    if (!pluginName)
        return nullptr;

    for (CPluginMngr::iterator iter = g_plugins.begin(); iter; ++iter)
    {
        if ((*iter).isValid() && strcmp((*iter).getName(), pluginName) == 0)
        {
            return &(*iter);
        }
    }

    return nullptr;
}

CPluginMngr::CPlugin* CSharpBridge::FindPluginById(int pluginId)
{
    return g_plugins.findPlugin(pluginId);
}
