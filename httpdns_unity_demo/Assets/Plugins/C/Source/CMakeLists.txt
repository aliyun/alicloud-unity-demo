cmake_minimum_required(VERSION 3.5)

# VCPKG support - must be set before project()
if(DEFINED ENV{VCPKG_ROOT})
    set(VCPKG_ROOT $ENV{VCPKG_ROOT})
elseif(EXISTS "C:/Users/Administrator/Desktop/vcpkg/scripts/buildsystems/vcpkg.cmake")
    set(VCPKG_ROOT "C:/Users/Administrator/Desktop/vcpkg")
elseif(EXISTS "C:/Users/Administrator/Desktop/vcpkg-2025.07.25/vcpkg-2025.07.25/scripts/buildsystems/vcpkg.cmake")
    set(VCPKG_ROOT "C:/Users/Administrator/Desktop/vcpkg-2025.07.25/vcpkg-2025.07.25")
endif()

if(DEFINED VCPKG_ROOT)
    set(CMAKE_TOOLCHAIN_FILE "${VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake" CACHE STRING "")
    message(STATUS "Using VCPKG from: ${VCPKG_ROOT}")
endif()

project(HttpDnsUnityBridgeMain)

# Set C++ standard
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Platform detection
if(WIN32)
    set(PLATFORM "Windows")
elseif(APPLE)
    set(PLATFORM "macOS")
elseif(UNIX AND NOT APPLE)
    set(PLATFORM "Linux")
endif()

message(STATUS "Building for platform: ${PLATFORM}")

# Build configuration
if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Release)
endif()

# Define ENABLE_HTTPDNS_C_SDK to enable full C SDK integration
option(ENABLE_HTTPDNS_C_SDK "Enable full HttpDNS C SDK integration" ON)

if(ENABLE_HTTPDNS_C_SDK)
    message(STATUS "HttpDNS C SDK integration enabled")
    add_definitions(-DENABLE_HTTPDNS_C_SDK)
else()
    message(STATUS "HttpDNS C SDK integration disabled (fallback mode)")
endif()

# Platform-specific compiler settings
if(WIN32)
    # Windows specific settings
    add_definitions(-DWIN32_LEAN_AND_MEAN)
    add_definitions(-D_CRT_SECURE_NO_WARNINGS)
    add_definitions(-DBUILDING_DLL)
    add_definitions(-DHDNS_INIT_WINSOCK=1)
    set(CMAKE_WINDOWS_EXPORT_ALL_SYMBOLS ON)
    
    # 修复MSVC编译器兼容性问题
    if(MSVC)
        # 启用C99变量声明位置
        set(CMAKE_C_STANDARD 99)
        set(CMAKE_C_STANDARD_REQUIRED OFF)
        
        # 禁用特定警告
        add_compile_options(/wd4996)  # deprecated函数警告
        add_compile_options(/wd4819)  # 字符编码警告
        add_compile_options(/wd4101)  # 未引用局部变量
        add_compile_options(/wd4244)  # 类型转换警告
        
        # 启用C99特性
        add_compile_options(/std:c11)
        
        # 定义缺失的宏
        add_definitions(-DHDNS_BOOT_SERVERS_DEFINED)
    endif()
elseif(APPLE)
    # macOS specific settings
    set(CMAKE_OSX_DEPLOYMENT_TARGET "10.12")
    set(CMAKE_CXX_VISIBILITY_PRESET hidden)
    set(CMAKE_C_VISIBILITY_PRESET hidden)
elseif(UNIX)
    # Linux specific settings
    set(CMAKE_CXX_VISIBILITY_PRESET hidden)
    set(CMAKE_C_VISIBILITY_PRESET hidden)
    set(CMAKE_POSITION_INDEPENDENT_CODE ON)
endif()

# Include directories
include_directories(
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${CMAKE_CURRENT_SOURCE_DIR}/alibabacloud-httpdns-c-sdk/src
)

# Unity Bridge source files
set(UNITY_BRIDGE_SOURCES
    HttpDnsUnityBridge.cpp
    HttpDnsUnityBridge.h
)

if(ENABLE_HTTPDNS_C_SDK)
    # Find dependencies for C SDK
    if(WIN32)
        if(DEFINED VCPKG_ROOT)
            message(STATUS "Using VCPKG dependencies from: ${VCPKG_ROOT}")
            # Use VCPKG installed packages with direct linking for Windows
            find_package(CURL REQUIRED)
            find_package(cjson REQUIRED)
            find_package(OpenSSL REQUIRED)
            # APR libraries will be linked directly by library name
            set(APR_LIBRARIES "apr-1")
            set(APR_UTIL_LIBRARIES "aprutil-1")
        else()
            message(WARNING "VCPKG_ROOT not set. Please install dependencies manually or set VCPKG_ROOT")
        endif()
    else()
        # Unix-like systems (macOS, Linux)
        find_package(PkgConfig REQUIRED)
        
        # Find libcurl
        find_package(CURL REQUIRED)
        
        # Find APR and APR-Util
        pkg_check_modules(APR REQUIRED apr-1)
        pkg_check_modules(APR_UTIL REQUIRED apr-util-1)
        
        # Find cJSON
        pkg_check_modules(CJSON REQUIRED libcjson)
    endif()
    
    # Add C SDK subdirectory if it exists
    if(EXISTS ${CMAKE_CURRENT_SOURCE_DIR}/alibabacloud-httpdns-c-sdk/CMakeLists.txt)
        message(STATUS "Adding HttpDNS C SDK subdirectory")
        # Disable tests and examples for the C SDK
        set(BUILD_TESTS OFF CACHE BOOL "Disable tests")
        set(BUILD_EXAMPLES OFF CACHE BOOL "Disable examples")
        add_subdirectory(alibabacloud-httpdns-c-sdk)
        set(HTTPDNS_C_SDK_LIBRARIES httpdns_c_sdk_static)
    else()
        message(WARNING "HttpDNS C SDK not found. Building in fallback mode.")
        set(ENABLE_HTTPDNS_C_SDK OFF)
        remove_definitions(-DENABLE_HTTPDNS_C_SDK)
    endif()
endif()

# Create the Unity plugin library
if(WIN32)
    add_library(UnityHttpDnsBridge SHARED ${UNITY_BRIDGE_SOURCES})
    set_target_properties(UnityHttpDnsBridge PROPERTIES 
        OUTPUT_NAME "HttpDnsUnityBridge"
        SUFFIX ".dll"
        PREFIX ""
    )
elseif(APPLE)
    add_library(UnityHttpDnsBridge SHARED ${UNITY_BRIDGE_SOURCES})
    set_target_properties(UnityHttpDnsBridge PROPERTIES 
        OUTPUT_NAME "HttpDnsUnityBridge"
        SUFFIX ".dylib"
        PREFIX "lib"
        MACOSX_RPATH ON
    )
else()
    add_library(UnityHttpDnsBridge SHARED ${UNITY_BRIDGE_SOURCES})
    set_target_properties(UnityHttpDnsBridge PROPERTIES 
        OUTPUT_NAME "HttpDnsUnityBridge"
        SUFFIX ".so" 
        PREFIX "lib"
    )
endif()

# Link libraries
if(ENABLE_HTTPDNS_C_SDK)
    target_link_libraries(UnityHttpDnsBridge ${HTTPDNS_C_SDK_LIBRARIES})
    
    if(WIN32)
        if(DEFINED VCPKG_ROOT)
            message(STATUS "Linking with VCPKG libraries")
            # Link with VCPKG installed libraries
            target_link_libraries(UnityHttpDnsBridge 
                CURL::libcurl
                ${APR_LIBRARIES}
                ${APR_UTIL_LIBRARIES}
                cjson
                OpenSSL::SSL
                OpenSSL::Crypto
            )
            
            # Add Windows system libraries
            target_link_libraries(UnityHttpDnsBridge 
                ws2_32
                winmm
                wldap32
                crypt32
                normaliz
            )
        endif()
    elseif(UNIX)
        target_link_libraries(UnityHttpDnsBridge 
            ${CURL_LIBRARIES}
            ${APR_LIBRARIES}
            ${APR_UTIL_LIBRARIES}
        )
        
        # Link cjson statically to avoid runtime dependencies
        find_library(CJSON_STATIC_LIB libcjson.a PATHS /opt/homebrew/lib)
        if(CJSON_STATIC_LIB)
            target_link_libraries(UnityHttpDnsBridge ${CJSON_STATIC_LIB})
            message(STATUS "Using static cjson library: ${CJSON_STATIC_LIB}")
        else()
            # Fallback to dynamic linking if static not found
            find_library(CJSON_LIB cjson PATHS /opt/homebrew/lib)
            if(CJSON_LIB)
                target_link_libraries(UnityHttpDnsBridge ${CJSON_LIB})
                message(WARNING "Using dynamic cjson library: ${CJSON_LIB}")
            endif()
        endif()
        
        target_include_directories(UnityHttpDnsBridge PRIVATE
            ${CURL_INCLUDE_DIRS}
            ${APR_INCLUDE_DIRS}
            ${APR_UTIL_INCLUDE_DIRS}
            ${CJSON_INCLUDE_DIRS}
        )
        
        # Also add include directories to the target directly
        target_include_directories(UnityHttpDnsBridge PRIVATE
            ${CMAKE_CURRENT_SOURCE_DIR}/alibabacloud-httpdns-c-sdk/src
            /opt/homebrew/Cellar/apr/1.7.6/include/apr-1
            /opt/homebrew/Cellar/apr-util/1.6.3_1/include/apr-1
        )
    endif()
endif()

# Set output directory based on platform
if(WIN32)
    set(UNITY_PLUGINS_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../x86_64")
elseif(APPLE)
    set(UNITY_PLUGINS_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../macOS")
else()
    set(UNITY_PLUGINS_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../Linux")
endif()

# Create output directory
file(MAKE_DIRECTORY ${UNITY_PLUGINS_DIR})

# Set output location
set_target_properties(UnityHttpDnsBridge PROPERTIES
    RUNTIME_OUTPUT_DIRECTORY ${UNITY_PLUGINS_DIR}
    LIBRARY_OUTPUT_DIRECTORY ${UNITY_PLUGINS_DIR}
    ARCHIVE_OUTPUT_DIRECTORY ${UNITY_PLUGINS_DIR}
)

# Copy output to Unity plugins directory
add_custom_command(TARGET UnityHttpDnsBridge POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:UnityHttpDnsBridge> ${UNITY_PLUGINS_DIR}/
    COMMENT "Copying HttpDnsUnityBridge to Unity plugins directory: ${UNITY_PLUGINS_DIR}"
)

# Print build information
message(STATUS "Build type: ${CMAKE_BUILD_TYPE}")
message(STATUS "C++ standard: ${CMAKE_CXX_STANDARD}")
message(STATUS "Output directory: ${UNITY_PLUGINS_DIR}")

if(ENABLE_HTTPDNS_C_SDK)
    message(STATUS "Building with full HttpDNS C SDK support")
else()
    message(STATUS "Building in fallback mode (no C SDK integration)")
endif()