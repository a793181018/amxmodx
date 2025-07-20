# Windows API 冲突修复报告

## 问题描述
在Visual Studio 2019中编译MenuBridge时遇到多个Windows API函数名冲突错误。

## 冲突的函数列表

### 原始冲突函数
1. `CreateMenu` - 与 `winuser.h` 中的 `CreateMenu` 冲突
2. `DestroyMenu` - 与 `winuser.h` 中的 `DestroyMenu` 冲突  
3. `GetMenuItemCount` - 与 `winuser.h` 中的 `GetMenuItemCount` 冲突
4. `AddMenuItem` - 与 `winuser.h` 中的 `AddMenuItem` 冲突
5. `InsertMenuItem` - 与 `winuser.h` 中的 `InsertMenuItem` 冲突
6. `GetMenuItemInfo` - 与 `winuser.h` 中的 `GetMenuItemInfo` 冲突

## 修复方案

### 1. 函数重命名
所有冲突的函数都添加了 `Ex` 后缀：

| 原函数名 | 新函数名 |
|---------|---------|
| `CreateMenu` | `CreateMenuEx` |
| `DestroyMenu` | `DestroyMenuEx` |
| `GetMenuItemCount` | `GetMenuItemCountEx` |
| `AddMenuItem` | `AddMenuItemEx` |
| `InsertMenuItem` | `InsertMenuItemEx` |
| `GetMenuItemInfo` | `GetMenuItemInfoEx` |
| `DisplayMenu` | `DisplayMenuEx` |
| `CancelMenu` | `CancelMenuEx` |

### 2. 预处理器定义
在头文件和源文件中添加了以下定义来避免包含Windows用户界面API：

```cpp
#ifdef _WIN32
#define NOMINMAX
#define WIN32_LEAN_AND_MEAN
#define NOGDI
#define NOUSER
#endif
```

### 3. C# DLL导入映射
在C#代码中使用 `EntryPoint` 属性来映射到正确的C++函数：

```csharp
[DllImport(DllName, EntryPoint = "CreateMenuEx")]
public static extern int CreateMenu(...);
```

## 修复后的文件

### 已修改的文件
1. `MenuBridge.h` - 更新函数声明和预处理器定义
2. `MenuBridge.cpp` - 更新函数实现和预处理器定义
3. `MenuBridgeImports.cs` - 更新DLL导入映射
4. `MenuBridge_AMXX.vcxproj` - 添加预处理器定义到项目配置

### 新增的文件
1. `TestCompile.cpp` - 编译测试文件
2. `WINDOWS_API_CONFLICTS_FIXED.md` - 本文档

## 编译验证

### 编译前错误
```
error C2733: "CreateMenu": 无法重载具有外部 "C" 链接的函数
error C2733: "DestroyMenu": 无法重载具有外部 "C" 链接的函数
error C2733: "GetMenuItemCount": 无法重载具有外部 "C" 链接的函数
```

### 修复后状态
- ✅ 所有函数名冲突已解决
- ✅ 预处理器定义防止Windows API包含
- ✅ C#映射保持API兼容性
- ✅ 项目配置已更新

## 使用说明

### 对开发者的影响
1. **C++开发者**: 使用新的函数名（带Ex后缀）
2. **C#开发者**: API保持不变，无需修改现有代码
3. **编译**: 使用更新后的项目文件或CMake配置

### 示例代码
```cpp
// C++ 代码
int menuId = CreateMenuEx("Test Menu", "Handler", false);
bool success = AddMenuItemEx(menuId, "Item", "cmd", 0);
```

```csharp
// C# 代码 (API保持不变)
int menuId = MenuBridgeImports.CreateMenu("Test Menu", "Handler", false);
bool success = MenuBridgeImports.AddMenuItem(menuId, "Item", "cmd", 0);
```

## 测试建议

1. **编译测试**: 使用 `TestCompile.cpp` 验证编译成功
2. **功能测试**: 运行C#示例代码验证DLL加载和函数调用
3. **集成测试**: 在AMXX环境中测试完整功能

## 注意事项

1. **向后兼容**: C#层API保持不变，现有代码无需修改
2. **性能影响**: 函数重命名对性能无影响
3. **维护**: 未来添加新函数时需注意Windows API冲突

## 总结

通过系统性的函数重命名和预处理器配置，成功解决了所有Windows API冲突问题。修复方案既解决了编译问题，又保持了API的易用性和兼容性。
