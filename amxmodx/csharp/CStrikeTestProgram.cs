// vim: set ts=4 sw=4 tw=99 noet:
//
// AMX Mod X, based on AMX Mod by Aleksander Naszko ("OLO").
// Copyright (C) The AMX Mod X Development Team.
//
// This software is licensed under the GNU General Public License, version 3 or higher.
// Additional exceptions apply. For full license details, see LICENSE.txt or visit:
//     https://alliedmods.net/amxmodx-license

//
// Counter-Strike C# Test Program
//

using System;
using System.Threading;
using AmxModX.Examples;

namespace AmxModX.CStrike.Test
{
    /// <summary>
    /// Counter-Strike模块测试程序
    /// Counter-Strike module test program
    /// </summary>
    class CStrikeTestProgram
    {
        /// <summary>
        /// 程序入口点
        /// Program entry point
        /// </summary>
        /// <param name="args">命令行参数 / Command line arguments</param>
        static void Main(string[] args)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("AMX Mod X Counter-Strike C# Bridge Test Program");
            Console.WriteLine("AMX Mod X Counter-Strike C# 桥接层测试程序");
            Console.WriteLine("=================================================================");
            Console.WriteLine();

            try
            {
                // 显示测试信息
                // Display test information
                DisplayTestInfo();

                // 运行基础测试
                // Run basic tests
                RunBasicTests();

                // 运行示例
                // Run examples
                RunExamples();

                // 运行压力测试
                // Run stress tests
                RunStressTests();

                Console.WriteLine("\n=== 测试完成 / Test Completed ===");
                Console.WriteLine("所有测试已完成，按任意键退出... / All tests completed, press any key to exit...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== 测试过程中发生错误 / Error occurred during testing ===");
                Console.WriteLine($"错误信息 / Error message: {ex.Message}");
                Console.WriteLine($"堆栈跟踪 / Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // 清理资源
                // Cleanup resources
                try
                {
                    CStrikeExample.Cleanup();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"清理资源时发生错误 / Error during cleanup: {ex.Message}");
                }

                Console.ReadKey();
            }
        }

        /// <summary>
        /// 显示测试信息
        /// Display test information
        /// </summary>
        private static void DisplayTestInfo()
        {
            Console.WriteLine("=== 测试信息 / Test Information ===");
            Console.WriteLine($"测试时间 / Test time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"操作系统 / Operating system: {Environment.OSVersion}");
            Console.WriteLine($".NET版本 / .NET version: {Environment.Version}");
            Console.WriteLine($"处理器架构 / Processor architecture: {Environment.Is64BitProcess} bit");
            Console.WriteLine();

            Console.WriteLine("测试范围 / Test scope:");
            Console.WriteLine("1. 玩家管理类接口 / Player Management Category Interfaces");
            Console.WriteLine("2. 武器系统类接口 / Weapon System Category Interfaces");
            Console.WriteLine("3. 游戏实体类接口 / Game Entity Category Interfaces");
            Console.WriteLine("4. 地图环境类接口 / Map Environment Category Interfaces");
            Console.WriteLine("5. 特殊功能类接口 / Special Features Category Interfaces");
            Console.WriteLine("6. 统计分析类接口 / Statistics Analysis Category Interfaces");
            Console.WriteLine("7. Forward回调接口 / Forward Callback Interfaces");
            Console.WriteLine();
        }

        /// <summary>
        /// 运行基础测试
        /// Run basic tests
        /// </summary>
        private static void RunBasicTests()
        {
            Console.WriteLine("=== 基础测试 / Basic Tests ===");

            // 测试桥接层初始化
            // Test bridge layer initialization
            Console.WriteLine("测试1: 桥接层初始化 / Test 1: Bridge layer initialization");
            bool initResult = TestBridgeInitialization();
            Console.WriteLine($"结果 / Result: {(initResult ? "通过" : "失败")} / {(initResult ? "PASS" : "FAIL")}");
            Console.WriteLine();

            // 测试P/Invoke调用
            // Test P/Invoke calls
            Console.WriteLine("测试2: P/Invoke调用测试 / Test 2: P/Invoke call test");
            bool pinvokeResult = TestPInvokeCalls();
            Console.WriteLine($"结果 / Result: {(pinvokeResult ? "通过" : "失败")} / {(pinvokeResult ? "PASS" : "FAIL")}");
            Console.WriteLine();

            // 测试委托回调
            // Test delegate callbacks
            Console.WriteLine("测试3: 委托回调测试 / Test 3: Delegate callback test");
            bool callbackResult = TestDelegateCallbacks();
            Console.WriteLine($"结果 / Result: {(callbackResult ? "通过" : "失败")} / {(callbackResult ? "PASS" : "FAIL")}");
            Console.WriteLine();
        }

        /// <summary>
        /// 测试桥接层初始化
        /// Test bridge layer initialization
        /// </summary>
        /// <returns>测试是否通过 / Whether test passed</returns>
        private static bool TestBridgeInitialization()
        {
            try
            {
                // 尝试初始化桥接层
                // Try to initialize bridge layer
                bool result = AmxModX.CStrike.CStrikeInterop.Initialize();
                Console.WriteLine($"桥接层初始化: {(result ? "成功" : "失败")} / Bridge layer initialization: {(result ? "Success" : "Failed")}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"桥接层初始化异常 / Bridge layer initialization exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 测试P/Invoke调用
        /// Test P/Invoke calls
        /// </summary>
        /// <returns>测试是否通过 / Whether test passed</returns>
        private static bool TestPInvokeCalls()
        {
            try
            {
                // 测试一些基本的P/Invoke调用
                // Test some basic P/Invoke calls
                Console.WriteLine("测试玩家金钱获取... / Testing player money retrieval...");
                int money = AmxModX.CStrike.CStrikeInterop.PlayerManager.GetMoney(1);
                Console.WriteLine($"玩家1金钱: ${money} / Player 1 money: ${money}");

                Console.WriteLine("测试玩家队伍获取... / Testing player team retrieval...");
                var team = AmxModX.CStrike.CStrikeInterop.PlayerManager.GetTeam(1);
                Console.WriteLine($"玩家1队伍: {team} / Player 1 team: {team}");

                Console.WriteLine("测试实体创建... / Testing entity creation...");
                int entity = AmxModX.CStrike.CStrikeInterop.EntityManager.CreateEntity("weapon_ak47");
                Console.WriteLine($"创建的实体ID: {entity} / Created entity ID: {entity}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"P/Invoke调用异常 / P/Invoke call exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 测试委托回调
        /// Test delegate callbacks
        /// </summary>
        /// <returns>测试是否通过 / Whether test passed</returns>
        private static bool TestDelegateCallbacks()
        {
            try
            {
                // 注册测试回调
                // Register test callbacks
                Console.WriteLine("注册测试回调... / Registering test callbacks...");
                
                int buyAttemptId = AmxModX.CStrike.CStrikeInterop.ForwardManager.RegisterBuyAttempt(TestBuyAttemptCallback);
                Console.WriteLine($"购买尝试回调ID: {buyAttemptId} / Buy attempt callback ID: {buyAttemptId}");

                int buyId = AmxModX.CStrike.CStrikeInterop.ForwardManager.RegisterBuy(TestBuyCallback);
                Console.WriteLine($"购买完成回调ID: {buyId} / Buy completion callback ID: {buyId}");

                int commandId = AmxModX.CStrike.CStrikeInterop.ForwardManager.RegisterInternalCommand(TestInternalCommandCallback);
                Console.WriteLine($"内部命令回调ID: {commandId} / Internal command callback ID: {commandId}");

                // 注销回调
                // Unregister callbacks
                Console.WriteLine("注销测试回调... / Unregistering test callbacks...");
                AmxModX.CStrike.CStrikeInterop.ForwardManager.UnregisterBuyAttempt(buyAttemptId);
                AmxModX.CStrike.CStrikeInterop.ForwardManager.UnregisterBuy(buyId);
                AmxModX.CStrike.CStrikeInterop.ForwardManager.UnregisterInternalCommand(commandId);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"委托回调测试异常 / Delegate callback test exception: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 运行示例
        /// Run examples
        /// </summary>
        private static void RunExamples()
        {
            Console.WriteLine("=== 运行示例 / Running Examples ===");
            
            try
            {
                CStrikeExample.Initialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"示例运行异常 / Example execution exception: {ex.Message}");
            }
        }

        /// <summary>
        /// 运行压力测试
        /// Run stress tests
        /// </summary>
        private static void RunStressTests()
        {
            Console.WriteLine("=== 压力测试 / Stress Tests ===");

            // 测试大量API调用
            // Test massive API calls
            Console.WriteLine("测试大量API调用... / Testing massive API calls...");
            TestMassiveApiCalls();

            // 测试并发调用
            // Test concurrent calls
            Console.WriteLine("测试并发调用... / Testing concurrent calls...");
            TestConcurrentCalls();

            // 测试内存使用
            // Test memory usage
            Console.WriteLine("测试内存使用... / Testing memory usage...");
            TestMemoryUsage();
        }

        /// <summary>
        /// 测试大量API调用
        /// Test massive API calls
        /// </summary>
        private static void TestMassiveApiCalls()
        {
            const int callCount = 1000;
            Console.WriteLine($"执行{callCount}次API调用... / Executing {callCount} API calls...");

            var startTime = DateTime.Now;
            
            for (int i = 0; i < callCount; i++)
            {
                try
                {
                    // 测试各种API调用
                    // Test various API calls
                    AmxModX.CStrike.CStrikeInterop.PlayerManager.GetMoney(1);
                    AmxModX.CStrike.CStrikeInterop.PlayerManager.GetTeam(1);
                    AmxModX.CStrike.CStrikeInterop.WeaponManager.GetCurrentWeapon(1, out _, out _);
                    AmxModX.CStrike.CStrikeInterop.StatisticsManager.GetStatsCount();
                    AmxModX.CStrike.CStrikeInterop.StatisticsManager.GetPlayerStats(1);
                    
                    if (i % 100 == 0)
                    {
                        Console.WriteLine($"已完成 {i} 次调用... / Completed {i} calls...");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"第{i}次调用失败 / Call {i} failed: {ex.Message}");
                }
            }

            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"完成{callCount}次调用，耗时: {duration.TotalMilliseconds:F2}ms / Completed {callCount} calls, duration: {duration.TotalMilliseconds:F2}ms");
        }

        /// <summary>
        /// 测试并发调用
        /// Test concurrent calls
        /// </summary>
        private static void TestConcurrentCalls()
        {
            const int threadCount = 5;
            const int callsPerThread = 100;
            
            Console.WriteLine($"启动{threadCount}个线程，每个线程执行{callsPerThread}次调用... / Starting {threadCount} threads, {callsPerThread} calls per thread...");

            var threads = new Thread[threadCount];
            var startTime = DateTime.Now;

            for (int i = 0; i < threadCount; i++)
            {
                int threadId = i;
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < callsPerThread; j++)
                    {
                        try
                        {
                            AmxModX.CStrike.CStrikeInterop.PlayerManager.GetMoney(threadId + 1);
                            AmxModX.CStrike.CStrikeInterop.PlayerManager.GetTeam(threadId + 1);
                            AmxModX.CStrike.CStrikeInterop.StatisticsManager.GetPlayerStats(threadId + 1);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"线程{threadId}调用失败 / Thread {threadId} call failed: {ex.Message}");
                        }
                    }
                    Console.WriteLine($"线程{threadId}完成 / Thread {threadId} completed");
                });
                
                threads[i].Start();
            }

            // 等待所有线程完成
            // Wait for all threads to complete
            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }

            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            Console.WriteLine($"并发测试完成，总耗时: {duration.TotalMilliseconds:F2}ms / Concurrent test completed, total duration: {duration.TotalMilliseconds:F2}ms");
        }

        /// <summary>
        /// 测试内存使用
        /// Test memory usage
        /// </summary>
        private static void TestMemoryUsage()
        {
            long startMemory = GC.GetTotalMemory(true);
            Console.WriteLine($"测试开始内存使用: {startMemory / 1024:N0} KB / Test start memory usage: {startMemory / 1024:N0} KB");

            // 执行一些内存密集型操作
            // Perform some memory-intensive operations
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    // 创建一些字符串操作
                    // Create some string operations
                    string model = AmxModX.CStrike.CStrikeInterop.PlayerManager.GetModel(1);
                    AmxModX.CStrike.CStrikeInterop.PlayerManager.SetModel(1, "test_model");
                    AmxModX.CStrike.CStrikeInterop.EntityManager.CreateEntity("test_entity");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"内存测试操作失败 / Memory test operation failed: {ex.Message}");
                }
            }

            long endMemory = GC.GetTotalMemory(true);
            Console.WriteLine($"测试结束内存使用: {endMemory / 1024:N0} KB / Test end memory usage: {endMemory / 1024:N0} KB");
            Console.WriteLine($"内存增长: {(endMemory - startMemory) / 1024:N0} KB / Memory growth: {(endMemory - startMemory) / 1024:N0} KB");
        }

        #region Test Callback Methods / 测试回调方法

        /// <summary>
        /// 测试购买尝试回调
        /// Test buy attempt callback
        /// </summary>
        private static int TestBuyAttemptCallback(int playerId, int item)
        {
            Console.WriteLine($"[TestCallback] BuyAttempt - Player: {playerId}, Item: {item}");
            return 0;
        }

        /// <summary>
        /// 测试购买完成回调
        /// Test buy completion callback
        /// </summary>
        private static int TestBuyCallback(int playerId, int item)
        {
            Console.WriteLine($"[TestCallback] Buy - Player: {playerId}, Item: {item}");
            return 0;
        }

        /// <summary>
        /// 测试内部命令回调
        /// Test internal command callback
        /// </summary>
        private static int TestInternalCommandCallback(int playerId, string command)
        {
            Console.WriteLine($"[TestCallback] InternalCommand - Player: {playerId}, Command: {command}");
            return 0;
        }

        #endregion
    }
}
