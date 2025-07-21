// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X Ham Sandwich Module - C# Bridge
//
#include <amtl/am-vector.h>


#ifndef CSHARP_BRIDGE_H
#define CSHARP_BRIDGE_H

#ifdef _WIN32
    #define CSHARP_EXPORT extern "C" __declspec(dllexport)
    #define CALLING_CONVENTION __stdcall
#else
    #define CSHARP_EXPORT extern "C" __attribute__((visibility("default")))
    #define CALLING_CONVENTION
#endif



// 参数结构体定义

struct HamExecuteParams {
    void* amxParams;
    int paramCount;
    int entity;
    int hamId;
};



// 回调函数类型定义
typedef void (CALLING_CONVENTION* HamCallbackDelegate)(int entity, int hookId, void* params);
typedef void (CALLING_CONVENTION* HamExecuteCallbackDelegate)(int entity, int hamId, HamExecuteParams* params);


// C#回调信息结构体

struct CSharpCallback
{
    HamCallbackDelegate callback;
    int hamId;
    int entity;
    bool isPost;
    int forwardId;
};

using VectorCSharpCallback = ke::Vector<CSharpCallback*>;


 struct HamTakeDamageParams {
    int attacker;
    int inflictor;
    float damage;
    int damageType;
};


class CsharpBridge
{
public:
    VectorCSharpCallback csharpCallbacks;
    int nextForwardId = 1;
    HamCallbackDelegate csharpGlobalCallback = nullptr;
    HamExecuteCallbackDelegate csharpExecuteCallback = nullptr;
    int hamReturnInt = 0;
    float hamReturnFloat = 0.0f;
    char hamReturnString[512];
    CsharpBridge()
    {
        csharpCallbacks = VectorCSharpCallback();
    }
    void CleanupCSharpBridge(void)
    {
        for (size_t i = 0; i < csharpCallbacks.length(); ++i)
        {
            delete csharpCallbacks.at(i);
        }
        csharpCallbacks.clear();
    }

    void TriggerCSharpExecuteCallback(int hamId, void* params, int paramCount) {
        if (!csharpExecuteCallback)
            return;

        cell* amxParams = (cell*)params;
        int entity = (paramCount > 1) ? amxParams[2] : 0;

        HamExecuteParams executeParams;
        executeParams.amxParams = params;
        executeParams.paramCount = paramCount;
        executeParams.entity = entity;
        executeParams.hamId = hamId;

        csharpExecuteCallback(entity, hamId, &executeParams);
    }


};
extern CsharpBridge g_CsharpBridge;

// 导出的C#接口函数
CSHARP_EXPORT int CALLING_CONVENTION Csharp_RegisterHamHook(int hamId, const char* className, HamCallbackDelegate callback, int post);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_RegisterHamHookFromEntity(int entity, int hamId, HamCallbackDelegate callback, int post);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_ExecuteHamHook(int hamId, int entity, void* params);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_ExecuteHamHookDirect(int hamId, int entity, void* params);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_IsHamHookValid(int hamId, int entity);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_DisableHamHookForward(int forwardId);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_EnableHamHookForward(int forwardId);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_GetHamReturnIntegerValue();
CSHARP_EXPORT float CALLING_CONVENTION Csharp_GetHamReturnFloatValue();
CSHARP_EXPORT int CALLING_CONVENTION Csharp_GetHamReturnStringValue(char* buffer, int maxLength);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnIntegerValue(int value);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnFloatValue(float value);
CSHARP_EXPORT int CALLING_CONVENTION Csharp_SetHamReturnStringValue(const char* value);
CSHARP_EXPORT void CALLING_CONVENTION Csharp_SetGlobalCSharpCallback(HamCallbackDelegate callback);
CSHARP_EXPORT void CALLING_CONVENTION Csharp_SetHamExecuteCallback(HamExecuteCallbackDelegate callback);



#endif // CSHARP_BRIDGE_H





