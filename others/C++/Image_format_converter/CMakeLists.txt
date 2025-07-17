cmake_minimum_required(VERSION 3.10)
project(ImageFormatConverter)

# 设置静态编译
set(CMAKE_EXE_LINKER_FLAGS "-static")
set(BUILD_SHARED_LIBS OFF)
set(CMAKE_FIND_LIBRARY_SUFFIXES .a)

# 设置C++标准
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 查找依赖库(静态版本)
find_package(PkgConfig REQUIRED)
pkg_check_modules(TIFF REQUIRED libtiff-4)
find_library(TIFF_STATIC_LIB NAMES libtiff.a PATHS ${TIFF_LIBRARY_DIRS})

pkg_check_modules(WEBP REQUIRED libwebp)
find_library(WEBP_STATIC_LIB NAMES libwebp.a PATHS ${WEBP_LIBRARY_DIRS})

# 使用find_package查找FLTK(静态版本)
find_package(FLTK REQUIRED)
find_library(FLTK_STATIC_LIB NAMES libfltk.a PATHS ${FLTK_LIBRARY_DIRS})

# 包含头文件目录
include_directories(include ${TIFF_INCLUDE_DIRS} ${WEBP_INCLUDE_DIRS} ${FLTK_INCLUDE_DIRS})

# 收集所有源文件
file(GLOB SOURCES "src/*.cpp")

# 创建可执行文件(设置为WIN32应用程序避免控制台窗口)
add_executable(ImageFormatConverter WIN32 ${SOURCES})

# 查找更多依赖库
find_library(JPEG_STATIC_LIB NAMES libjpeg.a)
find_library(ZLIB_STATIC_LIB NAMES libz.a)
find_library(ZSTD_STATIC_LIB NAMES libzstd.a)
find_library(SHARPYUV_STATIC_LIB NAMES libsharpyuv.a)
find_library(JBIG_STATIC_LIB NAMES libjbig.a)
find_library(LZMA_STATIC_LIB NAMES liblzma.a)
find_library(LIBDEFLATE_STATIC_LIB NAMES libdeflate.a)
find_library(LERC_STATIC_LIB NAMES liblerc.a)

# 链接静态库
target_link_libraries(ImageFormatConverter 
    ${TIFF_STATIC_LIB} 
    ${WEBP_STATIC_LIB} 
    ${FLTK_STATIC_LIB}
    ${JPEG_STATIC_LIB}
    ${ZLIB_STATIC_LIB}
    ${ZSTD_STATIC_LIB}
    ${SHARPYUV_STATIC_LIB}
    ${JBIG_STATIC_LIB}
    ${LZMA_STATIC_LIB}
    ${LIBDEFLATE_STATIC_LIB}
    ${LERC_STATIC_LIB}
    -lstdc++ -lgcc -lwinpthread -lcomctl32 -lole32 -luuid -lws2_32)

# 设置输出目录
set(EXECUTABLE_OUTPUT_PATH ${CMAKE_BINARY_DIR}/bin)
