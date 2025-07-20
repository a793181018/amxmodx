// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X Ham Sandwich Module - C# Bridge Implementation
//




#include "amxxmodule.h"
#include "csharp_bridge.h"
#include "hook.h"
#include "hooklist.h"
#include "DataHandler.h"
#include "forward.h"
#include "ham_const.h"
#include <string.h>
#include <amtl/am-vector.h>
using namespace ke;
// 外部变量声明
extern ke::Vector<Hook*> hooks[HAM_LAST_ENTRY_DONT_USE_ME_LOL];
extern hook_t hooklist[];
extern bool gDoForwards;

//
//// 全局变量定义
//static ke::Vector<CSharpCallback*> g_CsharpBridge.csharpCallbacks;
//static int g_CsharpBridge.nextForwardId = 1;
//static HamCallbackDelegate g_CsharpBridge.csharpGlobalCallback = nullptr;
//static HamExecuteCallbackDelegate g_CsharpBridge.csharpExecuteCallback = nullptr;
//// 全局返回值存储


CsharpBridge g_CsharpBridge = CsharpBridge();







// 查找钩子信息
static hook_t* FindHookInfo(int hamId)
{
    if (hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return nullptr;
    
    return &hooklist[hamId];
}


//// 清理函数，在模块卸载时调用
// void CleanupCSharpBridge(void){
//    for (size_t i = 0; i < g_CsharpBridge.csharpCallbacks.length(); ++i)
//    {
//        delete g_CsharpBridge.csharpCallbacks.at(i);
//    }
//    g_CsharpBridge.csharpCallbacks.clear();
//}
//
//// 供 hook_native.cpp 调用的函数
// void TriggerCSharpExecuteCallback(int hamId, void* params, int paramCount){
//    if (!g_CsharpBridge.csharpExecuteCallback)
//        return;
//
//    cell* amxParams = (cell*)params;
//    int entity = (paramCount > 1) ? amxParams[2] : 0;
//
//    HamExecuteParams executeParams;
//    executeParams.amxParams = params;
//    executeParams.paramCount = paramCount;
//    executeParams.entity = entity;
//    executeParams.hamId = hamId;
//
//    g_CsharpBridge.csharpExecuteCallback(entity, hamId, &executeParams);
//}
//




CSHARP_EXPORT void CALLING_CONVENTION SetGlobalCSharpCallback(HamCallbackDelegate callback)
{
    g_CsharpBridge.csharpGlobalCallback = callback;
}

CSHARP_EXPORT void CALLING_CONVENTION SetHamExecuteCallback(HamExecuteCallbackDelegate callback)
{
    g_CsharpBridge.csharpExecuteCallback = callback;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_RegisterHamHook(int hamId, const char* className, HamCallbackDelegate callback, int post)
{
    if (!className || !callback || hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return -1;

    hook_t* hookInfo = FindHookInfo(hamId);
    if (!hookInfo || !hookInfo->isset)
        return -1;

    // 存储回调信息
    CSharpCallback* cbInfo = new CSharpCallback();
    cbInfo->callback = callback;
    cbInfo->hamId = hamId;
    cbInfo->entity = 0;
    cbInfo->isPost = post != 0;
    cbInfo->forwardId = g_CsharpBridge.nextForwardId++;
    
    g_CsharpBridge.csharpCallbacks.append(cbInfo);
    
    return cbInfo->forwardId;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_RegisterHamHookFromEntity(int entity, int hamId, HamCallbackDelegate callback, int post)
{
    if (entity <= 0 || entity > gpGlobals->maxEntities || !callback || hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return -1;

    edict_t* pEntity = INDEXENT(entity);
    if (!pEntity || pEntity->free)
        return -1;

    hook_t* hookInfo = FindHookInfo(hamId);
    if (!hookInfo || !hookInfo->isset)
        return -1;

    // 存储回调信息
    CSharpCallback* cbInfo = new CSharpCallback();
    cbInfo->callback = callback;
    cbInfo->hamId = hamId;
    cbInfo->entity = entity;
    cbInfo->isPost = post != 0;
    cbInfo->forwardId = g_CsharpBridge.nextForwardId++;
    
    g_CsharpBridge.csharpCallbacks.append(cbInfo);
    
    return cbInfo->forwardId;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_ExecuteHamHook(int hamId, int entity, void* params)
{
    if (entity <= 0 || entity > gpGlobals->maxEntities || hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return 0;

    edict_t* pEntity = INDEXENT(entity);
    if (!pEntity || pEntity->free || !pEntity->pvPrivateData)
        return 0;

    hook_t* hookInfo = FindHookInfo(hamId);
    if (!hookInfo || !hookInfo->isset)
        return 0;

    // 启用前向回调，这样AMXX插件的Ham钩子会被触发
    gDoForwards = true;
    
    cell amxParams[16];
    amxParams[0] = 2 * sizeof(cell);
    amxParams[1] = hamId;
    amxParams[2] = entity;
    
    // 执行Ham函数，这会触发所有AMXX的RegisterHam回调
    cell result = hooklist[hamId].call(nullptr, amxParams);
    
    gDoForwards = false;
    
    return result;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_ExecuteHamHookDirect(int hamId, int entity, void* params)
{
    if (entity <= 0 || entity > gpGlobals->maxEntities || hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return 0;

    edict_t* pEntity = INDEXENT(entity);
    if (!pEntity || pEntity->free || !pEntity->pvPrivateData)
        return 0;

    hook_t* hookInfo = FindHookInfo(hamId);
    if (!hookInfo || !hookInfo->isset)
        return 0;

    // 不触发前向回调，只执行原生函数
    gDoForwards = false;
    
    cell amxParams[16];
    amxParams[0] = 2 * sizeof(cell);
    amxParams[1] = hamId;
    amxParams[2] = entity;
    
    return hooklist[hamId].call(nullptr, amxParams);
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_IsHamHookValid(int hamId, int entity)
{
    if (entity <= 0 || entity > gpGlobals->maxEntities || hamId < 0 || hamId >= HAM_LAST_ENTRY_DONT_USE_ME_LOL)
        return 0;

    edict_t* pEntity = INDEXENT(entity);
    if (!pEntity || pEntity->free)
        return 0;

    hook_t* hookInfo = FindHookInfo(hamId);
    if (!hookInfo || !hookInfo->isset)
        return 0;

    return 1;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_DisableHamHookForward(int forwardId)
{
    if (forwardId <= 0)
        return 0;

    for (size_t i = 0; i < g_CsharpBridge.csharpCallbacks.length(); ++i)
    {
        CSharpCallback* cb = g_CsharpBridge.csharpCallbacks.at(i);
        if (cb && cb->forwardId == forwardId)
        {
            // 标记为禁用
            cb->callback = nullptr;
            return 1;
        }
    }

    return 0;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_EnableHamHookForward(int forwardId)
{
    // 这里需要重新设置回调，但由于我们没有保存原始回调，暂时返回0
    return 0;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_GetHamReturnIntegerValue()
{
    return g_CsharpBridge.hamReturnInt;
}

CSHARP_EXPORT float CALLING_CONVENTION Csharp_GetHamReturnFloatValue()
{
    return g_CsharpBridge.hamReturnFloat;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_GetHamReturnStringValue(char* buffer, int maxLength)
{
    if (!buffer || maxLength <= 0)
        return 0;
    
    int len = strlen(g_CsharpBridge.hamReturnString);
    if (len >= maxLength)
        len = maxLength - 1;
    
    strncpy(buffer, g_CsharpBridge.hamReturnString, len);
    buffer[len] = '\0';
    return len;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnIntegerValue(int value)
{
    g_CsharpBridge.hamReturnInt = value;
    return 1;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnFloatValue(float value)
{
    g_CsharpBridge.hamReturnFloat = value;
    return 1;
}

CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnStringValue(const char* value){
    if (!value)
    {
         return 0;
    }
       

    strncpy(g_CsharpBridge.hamReturnString, value, sizeof(g_CsharpBridge.hamReturnString) - 1);
    g_CsharpBridge.hamReturnString[sizeof(g_CsharpBridge.hamReturnString) - 1] = '\0';
    return 1;
}







