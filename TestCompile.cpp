// 测试编译文件 - 验证MenuBridge是否能正确编译
// Test compilation file - verify MenuBridge compiles correctly

// 避免与Windows API冲突
#ifdef _WIN32
#define NOMINMAX
#define WIN32_LEAN_AND_MEAN
#define NOGDI
#define NOUSER
#endif

#include "MenuBridge.h"
#include <iostream>

// 简单的测试函数
int TestMenuBridgeCompilation()
{
    std::cout << "MenuBridge compilation test" << std::endl;
    
    // 测试函数声明是否正确
    // 注意：这些函数调用会失败，因为没有AMXX环境，但编译应该成功
    
    // 测试菜单创建
    int menuId = CreateMenuEx("Test Menu", "TestHandler", false);
    std::cout << "CreateMenuEx declared correctly" << std::endl;
    
    // 测试菜单项添加
    bool result = AddMenuItemEx(menuId, "Test Item", "test_cmd", 0);
    std::cout << "AddMenuItemEx declared correctly" << std::endl;
    
    // 测试菜单显示
    result = DisplayMenuEx(menuId, 1, 0);
    std::cout << "DisplayMenuEx declared correctly" << std::endl;
    
    // 测试菜单销毁
    result = DestroyMenuEx(menuId);
    std::cout << "DestroyMenuEx declared correctly" << std::endl;
    
    std::cout << "All function declarations are correct!" << std::endl;
    return 0;
}

// 如果这是独立编译，提供main函数
#ifdef TEST_STANDALONE
int main()
{
    return TestMenuBridgeCompilation();
}
#endif
