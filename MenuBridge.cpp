// 避免与Windows API冲突 - 在包含任何头文件之前定义
#ifdef _WIN32
#define NOMINMAX
#define WIN32_LEAN_AND_MEAN
#define NOGDI
#define NOUSER
#endif

#include "MenuBridge.h"
#include "amxmodx.h"
#include "newmenus.h"
//#include "CPlayer.h"
#include <string.h>
#include <map>
#include <vector>
#include <cstdio>

// 全局回调映射
static std::map<int, MenuHandlerCallback> g_MenuHandlers;
static std::map<int, MenuItemCallback> g_ItemCallbacks;
static int g_NextCallbackId = 1;

// 内部辅助函数
static Menu* GetMenuById(int menuId) {
    if (menuId < 0 || menuId >= static_cast<int>(g_NewMenus.length())) {
        return nullptr;
    }
    return g_NewMenus[menuId];
}

static bool IsValidPlayer(int playerId) {
    return playerId >= 1 && playerId <= gpGlobals->maxClients;
}

// 菜单管理接口实现
extern "C" {

MENU_BRIDGE_API int MENU_BRIDGE_CALL CreateMenuEx(const char* title, const char* handler, bool useMultilingual) {
    if (!title || !handler) {
        return -1;
    }

    // 创建菜单对象，使用空的AMX和回调ID
    // 在实际使用中，这些应该从当前插件上下文获取
    Menu* menu = new Menu(title, nullptr, -1, useMultilingual);
    if (!menu) {
        return -1;
    }

    // 添加到全局菜单列表
    int menuId = static_cast<int>(g_NewMenus.length());
    g_NewMenus.append(menu);

    return menuId;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL DestroyMenuEx(int menuId) {
    Menu* menu = GetMenuById(menuId);
    if (!menu) {
        return false;
    }
    
    // 清理回调
    g_MenuHandlers.erase(menuId);
    
    // 销毁菜单
    delete menu;
    
    // 从列表中移除（这里简化处理，实际可能需要更复杂的管理）
    if (menuId < static_cast<int>(g_NewMenus.length())) {
        g_NewMenus[menuId] = nullptr;
    }
    
    return true;
}

MENU_BRIDGE_API int MENU_BRIDGE_CALL FindMenuById(const char* menuName) {
    if (!menuName) {
        return -1;
    }

    for (size_t i = 0; i < g_NewMenus.length(); i++) {
        Menu* menu = g_NewMenus[i];
        if (menu && strcmp(menu->m_Title.chars(), menuName) == 0) {
            return static_cast<int>(i);
        }
    }

    return -1;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuItemEx(int menuId, const char* name, const char* command, int access) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !name) {
        return false;
    }

    const char* cmd = command ? command : "";
    return menu->AddItem(name, cmd, access) != nullptr;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuBlank(int menuId, bool slot) {
    Menu* menu = GetMenuById(menuId);
    if (!menu) {
        return false;
    }

    // 创建一个空白项
    menuitem* item = new menuitem();
    item->isBlank = true;
    item->access = 0;
    item->handler = -1;
    item->pfn = nullptr;
    item->id = menu->m_Items.length();

    // 添加空白项到菜单
    BlankItem blank;
    blank.SetBlank();
    blank.SetEatNumber(slot);
    item->blanks.append(ke::Move(blank));

    menu->m_Items.append(item);
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL AddMenuText(int menuId, const char* name, bool slot) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !name) {
        return false;
    }

    // 创建一个文本项
    menuitem* item = new menuitem();
    item->isBlank = true;
    item->access = 0;
    item->handler = -1;
    item->pfn = nullptr;
    item->id = menu->m_Items.length();

    // 添加文本项到菜单
    BlankItem textItem;
    textItem.SetText(name);
    textItem.SetEatNumber(slot);
    item->blanks.append(ke::Move(textItem));

    menu->m_Items.append(item);
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL InsertMenuItemEx(int menuId, int position, const char* name, const char* command, int access) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !name || position < 0) {
        return false;
    }

    // 创建新的菜单项
    menuitem* item = new menuitem();
    item->name = name;
    item->cmd = command ? command : "";
    item->access = access;
    item->handler = -1;
    item->pfn = nullptr;
    item->isBlank = false;
    item->id = menu->m_Items.length();

    // 插入到指定位置
    if (position >= static_cast<int>(menu->m_Items.length())) {
        menu->m_Items.append(item);
    } else {
        menu->m_Items.insert(position, item);
        // 更新后续项目的ID
        for (size_t i = position + 1; i < menu->m_Items.length(); i++) {
            menu->m_Items[i]->id = i;
        }
    }

    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemName(int menuId, int item, const char* name) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !name || item < 0) {
        return false;
    }

    menuitem* pItem = menu->GetMenuItem(item);
    if (!pItem) {
        return false;
    }

    pItem->name = name;
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemCommand(int menuId, int item, const char* command) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || item < 0) {
        return false;
    }
    
    menuitem* pItem = menu->GetMenuItem(item);
    if (!pItem) {
        return false;
    }
    
    pItem->cmd = command ? command : "";
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemAccess(int menuId, int item, int access) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || item < 0) {
        return false;
    }
    
    menuitem* pItem = menu->GetMenuItem(item);
    if (!pItem) {
        return false;
    }
    
    pItem->access = access;
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuItemCallback(int menuId, int item, MenuItemCallback callback) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || item < 0) {
        return false;
    }
    
    menuitem* pItem = menu->GetMenuItem(item);
    if (!pItem) {
        return false;
    }
    
    // 创建回调ID并存储
    int callbackId = g_NextCallbackId++;
    g_ItemCallbacks[callbackId] = callback;
    pItem->handler = callbackId;
    
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetMenuItemInfoEx(int menuId, int item, int access, char* name, int nameLen, char* command, int commandLen) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || item < 0) {
        return false;
    }

    menuitem* pItem = menu->GetMenuItem(item);
    if (!pItem) {
        return false;
    }

    if (name && nameLen > 0) {
        const char* itemName = pItem->name.chars();
        if (itemName) {
            strncpy(name, itemName, nameLen - 1);
            name[nameLen - 1] = '\0';
        } else {
            name[0] = '\0';
        }
    }

    if (command && commandLen > 0) {
        const char* itemCmd = pItem->cmd.chars();
        if (itemCmd) {
            strncpy(command, itemCmd, commandLen - 1);
            command[commandLen - 1] = '\0';
        } else {
            command[0] = '\0';
        }
    }

    return true;
}

MENU_BRIDGE_API int MENU_BRIDGE_CALL GetMenuItemCountEx(int menuId) {
    Menu* menu = GetMenuById(menuId);
    if (!menu) {
        return -1;
    }
    
    return menu->GetItemCount();
}

MENU_BRIDGE_API int MENU_BRIDGE_CALL GetMenuPageCount(int menuId) {
    Menu* menu = GetMenuById(menuId);
    if (!menu) {
        return -1;
    }
    
    return menu->GetPageCount();
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL DisplayMenuEx(int menuId, int playerId, int page) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !IsValidPlayer(playerId)) {
        return false;
    }
    
    return menu->Display(playerId, page);
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL CancelMenuEx(int playerId) {
    if (!IsValidPlayer(playerId)) {
        return false;
    }
    
    CPlayer* player = GET_PLAYER_POINTER_I(playerId);
    if (!player) {
        return false;
    }
    
    player->menu = 0;
    player->newmenu = -1;
    
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL IsMenuActive(int playerId) {
    if (!IsValidPlayer(playerId)) {
        return false;
    }
    
    CPlayer* player = GET_PLAYER_POINTER_I(playerId);
    if (!player) {
        return false;
    }
    
    return (player->menu != 0 && player->menu != -1) || (player->newmenu != -1);
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL SetMenuProperty(int menuId, MenuProperty property, const char* value) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !value) {
        return false;
    }

    switch (property) {
        case MENUPROP_BACKNAME:
            menu->m_OptNames[0] = value;
            break;
        case MENUPROP_NEXTNAME:
            menu->m_OptNames[1] = value;
            break;
        case MENUPROP_EXITNAME:
            menu->m_OptNames[2] = value;
            break;
        case MENUPROP_TITLE:
            menu->m_Title = value;
            break;
        case MENUPROP_ITEMCOLOR:
            menu->m_ItemColor = value;
            break;
        case MENUPROP_NEVEREXIT:
            menu->m_NeverExit = (atoi(value) != 0);
            break;
        case MENUPROP_FORCEEXIT:
            menu->m_ForceExit = (atoi(value) != 0);
            break;
        case MENUPROP_PERPAGE:
            menu->items_per_page = atoi(value);
            break;
        case MENUPROP_SHOWPAGE:
            menu->showPageNumber = (atoi(value) != 0);
            break;
        default:
            return false;
    }

    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetMenuProperty(int menuId, MenuProperty property, char* buffer, int bufferSize) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !buffer || bufferSize <= 0) {
        return false;
    }

    const char* result = nullptr;
    char temp[32];

    switch (property) {
        case MENUPROP_BACKNAME:
            result = menu->m_OptNames[0].chars();
            break;
        case MENUPROP_NEXTNAME:
            result = menu->m_OptNames[1].chars();
            break;
        case MENUPROP_EXITNAME:
            result = menu->m_OptNames[2].chars();
            break;
        case MENUPROP_TITLE:
            result = menu->m_Title.chars();
            break;
        case MENUPROP_ITEMCOLOR:
            result = menu->m_ItemColor.chars();
            break;
        case MENUPROP_NEVEREXIT:
            snprintf(temp, sizeof(temp), "%d", menu->m_NeverExit ? 1 : 0);
            result = temp;
            break;
        case MENUPROP_FORCEEXIT:
            snprintf(temp, sizeof(temp), "%d", menu->m_ForceExit ? 1 : 0);
            result = temp;
            break;
        case MENUPROP_PERPAGE:
            snprintf(temp, sizeof(temp), "%d", menu->items_per_page);
            result = temp;
            break;
        case MENUPROP_SHOWPAGE:
            snprintf(temp, sizeof(temp), "%d", menu->showPageNumber ? 1 : 0);
            result = temp;
            break;
        default:
            return false;
    }

    if (result) {
        strncpy(buffer, result, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
        return true;
    }

    return false;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL GetPlayerMenuInfo(int playerId, int* oldMenu, int* newMenu, int* page) {
    if (!IsValidPlayer(playerId)) {
        return false;
    }

    CPlayer* player = GET_PLAYER_POINTER_I(playerId);
    if (!player) {
        return false;
    }

    if (oldMenu) *oldMenu = player->menu;
    if (newMenu) *newMenu = player->newmenu;
    if (page) *page = player->page;

    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL RegisterMenuHandler(int menuId, MenuHandlerCallback callback) {
    Menu* menu = GetMenuById(menuId);
    if (!menu || !callback) {
        return false;
    }

    g_MenuHandlers[menuId] = callback;
    return true;
}

MENU_BRIDGE_API bool MENU_BRIDGE_CALL UnregisterMenuHandler(int menuId) {
    auto it = g_MenuHandlers.find(menuId);
    if (it != g_MenuHandlers.end()) {
        g_MenuHandlers.erase(it);
        return true;
    }
    return false;
}

MENU_BRIDGE_API int MENU_BRIDGE_CALL CreateMenuCallback(MenuItemCallback callback) {
    if (!callback) {
        return -1;
    }

    int callbackId = g_NextCallbackId++;
    g_ItemCallbacks[callbackId] = callback;
    return callbackId;
}

// 内部回调处理函数
int HandleMenuCallback(int menuId, int playerId, int item) {
    auto it = g_MenuHandlers.find(menuId);
    if (it != g_MenuHandlers.end()) {
        return it->second(menuId, playerId, item);
    }
    return 0;
}

int HandleItemCallback(int callbackId, int menuId, int playerId, int item) {
    auto it = g_ItemCallbacks.find(callbackId);
    if (it != g_ItemCallbacks.end()) {
        return it->second(menuId, playerId, item);
    }
    return 0;
}

} // extern "C"
