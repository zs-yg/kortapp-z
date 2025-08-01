cmake_minimum_required(VERSION 3.10)
project(hash_value_extractor)

set(CMAKE_C_STANDARD 11)
set(CMAKE_C_STANDARD_REQUIRED ON)

# 静态编译选项
option(BUILD_STATIC "Build with static linking" ON)

# 查找OpenSSL
find_package(OpenSSL REQUIRED)

# 设置Windows子系统
if(WIN32)
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -mwindows")
    if(BUILD_STATIC)
        set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -static")
        set(CMAKE_FIND_LIBRARY_SUFFIXES .a)
        set(CMAKE_EXE_LINK_DYNAMIC_C_FLAGS) 
        set(CMAKE_EXE_LINK_DYNAMIC_CXX_FLAGS)
        set(CMAKE_SHARED_LIBRARY_LINK_C_FLAGS)
        set(CMAKE_SHARED_LIBRARY_LINK_CXX_FLAGS)
    endif()
endif()

# 包含目录
include_directories(include)

# 添加可执行文件
add_executable(hash_value_extractor
    src/main.c
    src/md5.c
    src/sha256.c
    src/sha512.c
    src/string_util.c
)

# 链接OpenSSL库
if(BUILD_STATIC)
    # 显式指定静态库路径和名称
    find_library(OPENSSL_SSL_STATIC_LIBRARY NAMES ssl libssl.a PATHS ${OPENSSL_ROOT_DIR}/lib)
    find_library(OPENSSL_CRYPTO_STATIC_LIBRARY NAMES crypto libcrypto.a PATHS ${OPENSSL_ROOT_DIR}/lib)
    
    target_link_libraries(hash_value_extractor 
        ${OPENSSL_SSL_STATIC_LIBRARY}
        ${OPENSSL_CRYPTO_STATIC_LIBRARY}
        -lcrypt32
        -lws2_32
    )
else()
    target_link_libraries(hash_value_extractor 
        OpenSSL::SSL 
        OpenSSL::Crypto
    )
endif()
