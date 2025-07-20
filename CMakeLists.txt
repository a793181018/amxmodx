cmake_minimum_required(VERSION 3.16)



project(MenuBridge)







# 设置C++标准



set(CMAKE_CXX_STANDARD 17)



set(CMAKE_CXX_STANDARD_REQUIRED ON)







# 设置编译选项



if(WIN32)



    # Windows特定设置



    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /W3")



    add_definitions(-D_WIN32 -DWIN32_LEAN_AND_MEAN)



else()



    # Linux/Unix特定设置



    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Wextra -fPIC")



    add_definitions(-DLINUX -D_LINUX)



endif()







# 包含目录



include_directories(



    ${CMAKE_CURRENT_SOURCE_DIR}



    ${CMAKE_CURRENT_SOURCE_DIR}/amxmodx



    ${CMAKE_CURRENT_SOURCE_DIR}/public/sdk



)







# 源文件



set(SOURCES



    MenuBridge.cpp



    MenuBridge.h



)







# 创建动态库



add_library(MenuBridge SHARED ${SOURCES})







# 设置输出目录



set_target_properties(MenuBridge PROPERTIES



    RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin



    LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin



    ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib



)







# 平台特定链接设置



if(WIN32)



    # Windows链接设置



    target_link_libraries(MenuBridge)



    set_target_properties(MenuBridge PROPERTIES



        PREFIX ""



        SUFFIX ".dll"



    )



else()



    # Linux链接设置



    target_link_libraries(MenuBridge dl)



    set_target_properties(MenuBridge PROPERTIES



        PREFIX "lib"



        SUFFIX ".so"



    )



endif()







# 安装规则



install(TARGETS MenuBridge



    RUNTIME DESTINATION bin



    LIBRARY DESTINATION lib



    ARCHIVE DESTINATION lib



)







install(FILES MenuBridge.h



    DESTINATION include



)



