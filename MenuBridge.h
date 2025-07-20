#pragma once

#ifdef _WIN32
    #define MENU_BRIDGE_API __declspec(dllexport)
    #define MENU_BRIDGE_CALL __stdcall
    // 避免与Windows API冲突 - 在包含windows.h之前定义
    #define NOMINMAX
    #define WIN32_LEAN_AND_MEAN
    #define NOGDI
    #define NOUSER
#else
    #define MENU_BRIDGE_API __attribute__((visibility("default")))
    #define MENU_BRIDGE_CALL
#endif

#ifdef __cplusplus
extern "C" {
#endif

// 菜单属性枚举
typedef enum {
    MENUPROP_BACKNAME = 0,
    MENUPROP_NEXTNAME,
    MENUPROP_EXITNAME,
    MENUPROP_TITLE,
    MENUPROP_ITEMCOLOR,
    MENUPROP_NEVEREXIT,
    MENUPROP_FORCEEXIT,
    MENUPROP_PERPAGE,
    MENUPROP_SHOWPAGE
} MenuProperty;

// 菜单项访问权限
typedef enum {
    ITEMDRAW_DEFAULT = 0,
    ITEMDRAW_DISABLED,
    ITEMDRAW_RAWLINE,
    ITEMDRAW_NOTEXT,
    ITEMDRAW_SPACER
} ItemDrawType;

// 菜单回调类型
typedef enum {
    MENU_CALLBACK_DISPLAY = 0,
    MENU_CALLBACK_SELECT
} MenuCallbackType;

// 回调函数类型定义
typedef int (MENU_BRIDGE_CALL *MenuHandlerCallback)(int menuId, int playerId, int item);
typedef int (MENU_BRIDGE_CALL *MenuItemCallback)(int menuId, int playerId, int item);

// 菜单管理接口
MENU_BRIDGE_API int MENU_BRIDGE_CALL CreateMenuEx(const char* title, const char* handler, bool useMultilingual);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL DestroyMenuEx(int menuId);
MENU_BRIDGE_API int MENU_BRIDGE_CALL FindMenuById(const char* menuName);

// 菜单项管理接口
MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuItemEx(int menuId, const char* name, const char* command, int access);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuBlank(int menuId, bool slot);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuText(int menuId, const char* name, bool slot);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL InsertMenuItemEx(int menuId, int position, const char* name, const char* command, int access);

// 菜单项属性设置
MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemName(int menuId, int item, const char* name);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemCommand(int menuId, int item, const char* command);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemAccess(int menuId, int item, int access);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemCallback(int menuId, int item, MenuItemCallback callback);

// 菜单项信息获取
MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetMenuItemInfoEx(int menuId, int item, int access, char* name, int nameLen, char* command, int commandLen);
MENU_BRIDGE_API int MENU_BRIDGE_CALL GetMenuItemCountEx(int menuId);
MENU_BRIDGE_API int MENU_BRIDGE_CALL GetMenuPageCount(int menuId);

// 菜单显示和控制
MENU_BRIDGE_API bool MENU_BRIDGE_CALL DisplayMenuEx(int menuId, int playerId, int page);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL CancelMenuEx(int playerId);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL IsMenuActive(int playerId);

// 菜单属性设置
MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuProperty(int menuId, MenuProperty property, const char* value);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetMenuProperty(int menuId, MenuProperty property, char* buffer, int bufferSize);

// 玩家菜单信息
MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetPlayerMenuInfo(int playerId, int* oldMenu, int* newMenu, int* page);

// 回调注册
MENU_BRIDGE_API bool MENU_BRIDGE_CALL RegisterMenuHandler(int menuId, MenuHandlerCallback callback);
MENU_BRIDGE_API bool MENU_BRIDGE_CALL UnregisterMenuHandler(int menuId);

// 菜单回调创建
MENU_BRIDGE_API int MENU_BRIDGE_CALL CreateMenuCallback(MenuItemCallback callback);

#ifdef __cplusplus
}
#endif
