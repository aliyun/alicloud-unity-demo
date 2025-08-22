CMAKE_MINIMUM_REQUIRED(VERSION 3.5)

IF (DEFINED VCPKG_ROOT)
    SET(CMAKE_TOOLCHAIN_FILE "${VCPKG_ROOT}/scripts/buildsystems/vcpkg.cmake" CACHE STRING "")
    MESSAGE(STATUS "Using VCPKG from: ${VCPKG_ROOT}")
ELSE ()
    IF (CMAKE_HOST_WIN32)
        MESSAGE(WARNING "VCPKG_ROOT not defined in Windows. Will try to find dependencies manually.")
    ENDIF ()
ENDIF ()

PROJECT(httpdns_c_sdk LANGUAGES C)

####################################  编译选项  ###########################################
# default Release C flags
IF (CMAKE_HOST_UNIX OR CMAKE_HOST_APPLE)
    SET(CMAKE_C_FLAGS " -g -ggdb -O0 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")
    SET(CMAKE_CXX_FLAGS " -g -ggdb -O0 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")

    SET(CMAKE_C_FLAGS_DEBUG " -g -ggdb -O0 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")
    SET(CMAKE_CXX_FLAGS_DEBUG " -g -ggdb -O0 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")

    SET(CMAKE_C_FLAGS_RELEASE " -O3 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")
    SET(CMAKE_CXX_FLAGS_RELEASE " -O3 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")

    SET(CMAKE_C_FLAGS_MINSIZEREF " -Os -Wall -Werror  -fpic -fPIC -D_LARGEFILE64_SOURCE")
    SET(CMAKE_CXX_FLAGS_MINSIZEREF " -Os -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")

    SET(CMAKE_C_FLAGS_RELWITHDEBINFO " -O2 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")
    SET(CMAKE_CXX_FLAGS_RELWITHDEBINFO " -O2 -Wall -Werror -fpic -fPIC -D_LARGEFILE64_SOURCE")

    SET(CMAKE_C_FLAGS_COVERAGE " ${CMAKE_C_FLAGS_DEBUG} -fprofile-arcs -ftest-coverage")
    SET(CMAKE_CXX_FLAGS_COVERAGE " ${CMAKE_C_FLAGS_DEBUG} -fprofile-arcs -ftest-coverage")

    SET(LIBRARY_OUTPUT_PATH ${CMAKE_BINARY_DIR}/build/${CMAKE_BUILD_TYPE}/lib)
    SET(EXECUTABLE_OUTPUT_PATH ${CMAKE_BINARY_DIR}/build/${CMAKE_BUILD_TYPE}/bin)
    SET(CMAKE_C_OUTPUT_EXTENSION_REPLACE 1)
ELSEIF (CMAKE_HOST_WIN32)
    SET(CMAKE_C_FLAGS " /O2 /W0")
    SET(CMAKE_CXX_FLAGS " /O2 /W0")

    SET(CMAKE_C_FLAGS_DEBUG " /Zi /Od /W0")
    SET(CMAKE_CXX_FLAGS_DEBUG " /Zi /Od /W0")

    SET(CMAKE_C_FLAGS_RELEASE " /O2 /W0")
    SET(CMAKE_CXX_FLAGS_RELEASE " /O2 /W0")

    SET(CMAKE_C_FLAGS_MINSIZEREF " /O1 /W0")
    SET(CMAKE_CXX_FLAGS_MINSIZEREF " /O1 /W0")

    SET(CMAKE_C_FLAGS_RELWITHDEBINFO " /O2 /Zi /W0")
    SET(CMAKE_CXX_FLAGS_RELWITHDEBINFO " /O2 /Zi /W0")
    
    # 修复MSVC编译器兼容性问题
    IF(MSVC)
        # 禁用特定警告
        ADD_COMPILE_OPTIONS(/wd4996)  # deprecated函数警告
        ADD_COMPILE_OPTIONS(/wd4819)  # 字符编码警告 
        ADD_COMPILE_OPTIONS(/wd4101)  # 未引用局部变量
        ADD_COMPILE_OPTIONS(/wd4244)  # 类型转换警告
        ADD_COMPILE_OPTIONS(/wd2059)  # 语法错误
        ADD_COMPILE_OPTIONS(/wd2065)  # 未声明的标识符
        ADD_COMPILE_OPTIONS(/wd2039)  # 不是成员
        ADD_COMPILE_OPTIONS(/wd2146)  # 语法错误缺少符号
        ADD_COMPILE_OPTIONS(/wd2143)  # 语法错误缺少符号
        ADD_COMPILE_OPTIONS(/wd2099)  # 初始值设定项不是常量
        ADD_COMPILE_OPTIONS(/wd1019)  # 意外的预处理器指令
        
        # 定义缺失的宏和变量
        ADD_DEFINITIONS(-DHDNS_INIT_WINSOCK=1)
        ADD_DEFINITIONS(-DHDNS_BOOT_SERVERS_DEFINED=1)
        ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS)
        ADD_DEFINITIONS(-DWIN32_LEAN_AND_MEAN)
        
        # 强制使用UTF-8编码
        ADD_COMPILE_OPTIONS(/utf-8)
        
        # 使用C11标准（支持C99特性）
        ADD_COMPILE_OPTIONS(/std:c11)
        
        # 放宽语法检查
        ADD_COMPILE_OPTIONS(/permissive-)
    ENDIF()
ELSE ()
    MESSAGE(FATAL_ERROR "Unsupported OS for setting CMAKE_C_FLAGS, CMake will end.")
ENDIF ()

OPTION(ADD_LOG_USE_COLOR "ON for enabling color printing" ON)
IF (ADD_LOG_USE_COLOR)
    ADD_DEFINITIONS(-DLOG_USE_COLOR)
ENDIF (ADD_LOG_USE_COLOR)

OPTION(ADD_TEST_DEBUG_LOG "ON for unit test logs" ON)
IF (ADD_TEST_DEBUG_LOG)
    ADD_DEFINITIONS(-DTEST_DEBUG_LOG)
ENDIF (ADD_TEST_DEBUG_LOG)

# cn: china mainland, sg: singapore, hk: hong kong, us: america, de: germany, global: nearby
ADD_DEFINITIONS(-DHTTPDNS_REGION="global")

##################################### C语言标准 ##########################################
# Follow ISO C99 standard
SET(CMAKE_C_STANDARD 99)
SET(CMAKE_C_STANDARD_REQUIRED ON)
SET(CMAKE_C_EXTENSIONS ON)          # Use GNU extensions and POSIX standard

################################# 获取依赖库 ############################################
IF (DEFINED VCPKG_ROOT)
    FIND_PACKAGE(cJSON REQUIRED)
    FIND_PACKAGE(CURL REQUIRED)
    FIND_PATH(APR_INCLUDE_DIR NAMES apr.h)
    FIND_PATH(APU_INCLUDE_DIR NAMES apu.h)
    IF (CMAKE_HOST_WIN32)
        FIND_LIBRARY(APU_LIBRARIES libaprutil-1)
        FIND_LIBRARY(APR_LIBRARIES libapr-1)
    ELSE ()
        FIND_LIBRARY(APU_LIBRARIES aprutil-1)
        FIND_LIBRARY(APR_LIBRARIES apr-1)
    ENDIF ()
ELSE ()
    FIND_PACKAGE(cJSON REQUIRED)
    FIND_PACKAGE(CURL REQUIRED)
    IF (CMAKE_HOST_UNIX AND NOT CMAKE_HOST_APPLE)
        FIND_PROGRAM(APR_CONFIG_BIN NAMES apr-config apr-1-config PATHS /usr/bin /usr/local/bin /usr/local/apr/bin/)
        FIND_PROGRAM(APU_CONFIG_BIN NAMES apu-config apu-1-config PATHS /usr/bin /usr/local/bin /usr/local/apr/bin/)
    ELSEIF (CMAKE_HOST_UNIX AND CMAKE_HOST_APPLE)
        EXECUTE_PROCESS(
                COMMAND brew --prefix apr
                OUTPUT_VARIABLE APR_PATH
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        SET(APR_CONFIG_BIN "${APR_PATH}/bin/apr-1-config")
        EXECUTE_PROCESS(
                COMMAND brew --prefix apr-util
                OUTPUT_VARIABLE APU_PATH
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        SET(APU_CONFIG_BIN "${APU_PATH}/bin/apu-1-config")
    ELSE ()
        MESSAGE(FATAL_ERROR "Unsupported OS for getting apr/apu path, CMake will end.")
    ENDIF ()

    IF (APR_CONFIG_BIN)
        EXECUTE_PROCESS(
                COMMAND ${APR_CONFIG_BIN} --includedir
                OUTPUT_VARIABLE APR_INCLUDE_DIR
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        STRING(STRIP ${APR_INCLUDE_DIR} APR_INCLUDE_DIR)
        EXECUTE_PROCESS(
                COMMAND ${APR_CONFIG_BIN} --link-ld
                OUTPUT_VARIABLE APR_LIBRARIES
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        STRING(STRIP ${APR_LIBRARIES} APR_LIBRARIES)
    ELSE ()
        MESSAGE(FATAL_ERROR "Could not find apr-config/apr-1-config")
    ENDIF ()

    IF (APU_CONFIG_BIN)
        EXECUTE_PROCESS(
                COMMAND ${APU_CONFIG_BIN} --includedir
                OUTPUT_VARIABLE APU_INCLUDE_DIR
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        STRING(STRIP ${APU_INCLUDE_DIR} APU_INCLUDE_DIR)
        EXECUTE_PROCESS(
                COMMAND ${APU_CONFIG_BIN} --link-ld
                OUTPUT_VARIABLE APU_LIBRARIES
                OUTPUT_STRIP_TRAILING_WHITESPACE
        )
        STRING(STRIP ${APU_LIBRARIES} APU_LIBRARIES)
    ELSE ()
        MESSAGE(FATAL_ERROR "Could not find apu-config/apu-1-config")
    ENDIF ()
ENDIF ()
MESSAGE(STATUS "\n")
MESSAGE(STATUS "CURL_INCLUDE_DIRS:${CURL_INCLUDE_DIRS}")
MESSAGE(STATUS "APU_INCLUDE_DIR:${APU_INCLUDE_DIR}")
MESSAGE(STATUS "APR_INCLUDE_DIR:${APR_INCLUDE_DIR}")
MESSAGE(STATUS "CJSON_INCLUDE_DIR:${CJSON_INCLUDE_DIR}")
MESSAGE(STATUS "\n")
MESSAGE(STATUS "CURL_LIBRARIES:${CURL_LIBRARIES}")
MESSAGE(STATUS "APU_LIBRARIES:${APU_LIBRARIES}")
MESSAGE(STATUS "APR_LIBRARIES:${APR_LIBRARIES}")
MESSAGE(STATUS "CJSON_LIBRARIES:${CJSON_LIBRARIES}")
MESSAGE(STATUS "\n")
################################## 库文件编译安装 ############################################
INCLUDE_DIRECTORIES(${CURL_INCLUDE_DIRS})
INCLUDE_DIRECTORIES(${APU_INCLUDE_DIR})
INCLUDE_DIRECTORIES(${APR_INCLUDE_DIR})
INCLUDE_DIRECTORIES(${CJSON_INCLUDE_DIR})

AUX_SOURCE_DIRECTORY(src SRC_LIST)
## 静态库安装
ADD_LIBRARY(${CMAKE_PROJECT_NAME}_static STATIC ${SRC_LIST})
TARGET_LINK_LIBRARIES(${CMAKE_PROJECT_NAME}_static PRIVATE ${CURL_LIBRARIES} ${CJSON_LIBRARIES} ${APU_LIBRARIES} ${APR_LIBRARIES})
IF (CMAKE_HOST_UNIX AND NOT CMAKE_HOST_APPLE)
    TARGET_LINK_LIBRARIES(${CMAKE_PROJECT_NAME}_static PRIVATE dl)
ENDIF ()
INSTALL(TARGETS ${CMAKE_PROJECT_NAME}_static ARCHIVE DESTINATION lib)

## 动态库安装
ADD_LIBRARY(${CMAKE_PROJECT_NAME} SHARED ${SRC_LIST})
TARGET_COMPILE_DEFINITIONS(${CMAKE_PROJECT_NAME} PUBLIC BUILD_SHARED_LIB)
SET_TARGET_PROPERTIES(${CMAKE_PROJECT_NAME} PROPERTIES VERSION ${CMAKE_VERSION} SOVERSION ${CMAKE_VERSION})
TARGET_LINK_LIBRARIES(${CMAKE_PROJECT_NAME} PRIVATE ${CURL_LIBRARIES} ${CJSON_LIBRARIES} ${APU_LIBRARIES} ${APR_LIBRARIES})
IF (CMAKE_HOST_UNIX AND NOT CMAKE_HOST_APPLE)
    TARGET_LINK_LIBRARIES(${CMAKE_PROJECT_NAME} PRIVATE dl)
ENDIF ()
INSTALL(TARGETS ${CMAKE_PROJECT_NAME} LIBRARY DESTINATION lib)


## 头文件安装
FILE(GLOB HEADER_FILES src/*.h)
INSTALL(FILES
        ${HEADER_FILES}
        DESTINATION include/httpdns)
################################# 库文件编译卸载 ############################################
# uninstall target
IF (NOT TARGET uninstall)
    CONFIGURE_FILE(
            "${CMAKE_CURRENT_SOURCE_DIR}/cmake_uninstall.cmake.in"
            "${CMAKE_CURRENT_BINARY_DIR}/cmake_uninstall.cmake"
            IMMEDIATE @ONLY)

    ADD_CUSTOM_TARGET(uninstall
            COMMAND ${CMAKE_COMMAND} -P ${CMAKE_CURRENT_BINARY_DIR}/cmake_uninstall.cmake)
ENDIF ()
################################# 单元测试 ############################################
# Disabled for Unity integration - tests, examples and benchmark removed
# ADD_SUBDIRECTORY(tests)
# ADD_SUBDIRECTORY(examples)
# ADD_SUBDIRECTORY(benchmark)