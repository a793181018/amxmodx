using System;
using System.Threading;

/// <summary>
/// 核心AMX功能完整演示程序 / Complete Core AMX Features Demo Program
/// </summary>
public class CoreAmxDemo
{
    /// <summary>
    /// 程序入口点 / Program entry point
    /// </summary>
    public static void Main(string[] args)
    {
        Console.WriteLine("AMX Mod X 核心功能 C# API 演示程序");
        Console.WriteLine("AMX Mod X Core Features C# API Demo Program");
        Console.WriteLine(new string('=', 60));

        try
        {
            // 运行所有演示 / Run all demonstrations
            RunAllDemonstrations();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"演示程序执行出错 / Demo program error: {ex.Message}");
            Console.WriteLine($"详细信息 / Details: {ex.StackTrace}");
        }

        Console.WriteLine(new string('=', 60));
        Console.WriteLine("演示程序执行完成 / Demo program completed");
        Console.WriteLine("按任意键退出 / Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// 运行所有演示 / Run all demonstrations
    /// </summary>
    private static void RunAllDemonstrations()
    {
        // 1. 基础系统演示 / Basic systems demonstration
        Console.WriteLine("1. 基础系统演示 / Basic Systems Demo");
        DemonstrateBasicSystems();
        WaitForUser();

        // 2. 核心AMX功能演示 / Core AMX features demonstration
        Console.WriteLine("\n2. 核心AMX功能演示 / Core AMX Features Demo");
        DemonstrateCoreAmxFeatures();
        WaitForUser();

        // 3. 综合应用演示 / Comprehensive application demonstration
        Console.WriteLine("\n3. 综合应用演示 / Comprehensive Application Demo");
        DemonstrateComprehensiveApplication();
        WaitForUser();

        // 4. 性能测试 / Performance testing
        Console.WriteLine("\n4. 性能测试 / Performance Testing");
        DemonstratePerformanceTesting();
    }

    /// <summary>
    /// 演示基础系统 / Demonstrate basic systems
    /// </summary>
    private static void DemonstrateBasicSystems()
    {
        Console.WriteLine("运行基础系统演示...");
        
        // 运行CVar系统示例
        CvarExamples.RunAllExamples();
        
        // 运行菜单系统示例
        MenuExamples.RunAllExamples();
        
        // 运行游戏配置示例
        GameConfigExamples.RunAllExamples();
        
        // 运行Native系统示例
        NativeExamples.RunAllExamples();
        
        // 运行消息系统示例
        MessageExamples.RunAllExamples();
        
        // 运行数据包示例
        DataPackExamples.RunAllExamples();
    }

    /// <summary>
    /// 演示核心AMX功能 / Demonstrate core AMX features
    /// </summary>
    private static void DemonstrateCoreAmxFeatures()
    {
        Console.WriteLine("运行核心AMX功能演示...");
        
        // 运行核心AMX示例
        CoreAmxExamples.RunAllExamples();
    }

    /// <summary>
    /// 演示综合应用 / Demonstrate comprehensive application
    /// </summary>
    private static void DemonstrateComprehensiveApplication()
    {
        Console.WriteLine("运行综合应用演示...");
        
        // 运行完整的示例插件演示
        SamplePlugin.RunCompleteDemo();
    }

    /// <summary>
    /// 演示性能测试 / Demonstrate performance testing
    /// </summary>
    private static void DemonstratePerformanceTesting()
    {
        Console.WriteLine("运行性能测试...");
        
        // CVar性能测试
        PerformCvarPerformanceTest();
        
        // 函数调用性能测试
        PerformFunctionCallPerformanceTest();
        
        // 数据包性能测试
        PerformDataPackPerformanceTest();
    }

    /// <summary>
    /// CVar性能测试 / CVar performance test
    /// </summary>
    private static void PerformCvarPerformanceTest()
    {
        Console.WriteLine("\n--- CVar性能测试 / CVar Performance Test ---");
        
        const int iterations = 1000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 创建测试CVar
        int cvarId = CvarManager.CreateCvar("perf_test_cvar", "0", 0, "Performance test CVar");
        
        // 测试设置和获取操作
        for (int i = 0; i < iterations; i++)
        {
            CvarManager.SetCvarInt("perf_test_cvar", i);
            int value = CvarManager.GetCvarInt("perf_test_cvar");
        }
        
        stopwatch.Stop();
        Console.WriteLine($"CVar操作 {iterations * 2} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每次操作: {(double)stopwatch.ElapsedMilliseconds / (iterations * 2):F3}ms");
    }

    /// <summary>
    /// 函数调用性能测试 / Function call performance test
    /// </summary>
    private static void PerformFunctionCallPerformanceTest()
    {
        Console.WriteLine("\n--- 函数调用性能测试 / Function Call Performance Test ---");
        
        const int iterations = 100;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 测试函数调用操作
        for (int i = 0; i < iterations; i++)
        {
            if (CoreAmxManager.CallFuncBegin("test_function", ""))
            {
                CoreAmxManager.CallFuncPushInt(i);
                CoreAmxManager.CallFuncPushString($"test_{i}");
                CoreAmxManager.CallFuncEnd();
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine($"函数调用 {iterations} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每次调用: {(double)stopwatch.ElapsedMilliseconds / iterations:F3}ms");
    }

    /// <summary>
    /// 数据包性能测试 / DataPack performance test
    /// </summary>
    private static void PerformDataPackPerformanceTest()
    {
        Console.WriteLine("\n--- 数据包性能测试 / DataPack Performance Test ---");
        
        const int iterations = 1000;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 创建数据包
        int packId = DataPackManager.CreateDataPack();
        
        if (packId != -1)
        {
            // 测试写入操作
            for (int i = 0; i < iterations; i++)
            {
                DataPackManager.WritePackCell(packId, i);
                DataPackManager.WritePackFloat(packId, i * 1.5f);
                DataPackManager.WritePackString(packId, $"data_{i}");
            }
            
            // 重置并测试读取操作
            DataPackManager.ResetPack(packId);
            
            for (int i = 0; i < iterations; i++)
            {
                int intVal = DataPackManager.ReadPackCell(packId);
                float floatVal = DataPackManager.ReadPackFloat(packId);
                string stringVal = DataPackManager.ReadPackString(packId);
            }
            
            // 清理
            DataPackManager.DestroyDataPack(packId);
        }
        
        stopwatch.Stop();
        Console.WriteLine($"数据包操作 {iterations * 6} 次耗时: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"平均每次操作: {(double)stopwatch.ElapsedMilliseconds / (iterations * 6):F3}ms");
    }

    /// <summary>
    /// 等待用户输入 / Wait for user input
    /// </summary>
    private static void WaitForUser()
    {
        Console.WriteLine("\n按回车键继续下一个演示... / Press Enter to continue to next demo...");
        Console.ReadLine();
    }
}

/// <summary>
/// 演示程序配置 / Demo program configuration
/// </summary>
public static class DemoConfig
{
    /// <summary>
    /// 是否启用详细输出 / Whether to enable verbose output
    /// </summary>
    public static bool VerboseOutput { get; set; } = true;

    /// <summary>
    /// 是否启用性能测试 / Whether to enable performance testing
    /// </summary>
    public static bool EnablePerformanceTests { get; set; } = true;

    /// <summary>
    /// 演示延迟时间（毫秒）/ Demo delay time (milliseconds)
    /// </summary>
    public static int DemoDelayMs { get; set; } = 1000;
}
