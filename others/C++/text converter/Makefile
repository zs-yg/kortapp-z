# 编译器设置
CXX = g++
FLTK_CONFIG = fltk-config

# 编译选项
CXXFLAGS = -std=c++11 -Wall -Iinclude
LDFLAGS = -static -mwindows `$(FLTK_CONFIG) --use-images --ldstaticflags` -lcrypto -lws2_32 -lcrypt32

# 源文件和目标文件
SRC_DIR = src
OBJ_DIR = obj
SRCS = $(wildcard $(SRC_DIR)/*.cpp)
OBJS = $(patsubst $(SRC_DIR)/%.cpp,$(OBJ_DIR)/%.o,$(SRCS))

# 可执行文件
TARGET = text_converter.exe

# 默认目标
all: $(OBJ_DIR) $(TARGET)

# 创建obj目录
$(OBJ_DIR):
	mkdir -p $(OBJ_DIR)

# 生成目标文件
$(OBJ_DIR)/%.o: $(SRC_DIR)/%.cpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

# 链接生成可执行文件
$(TARGET): $(OBJS)
	$(CXX) $^ -o $@ $(LDFLAGS)

# 清理
clean:
	rm -rf $(OBJ_DIR) $(TARGET)

.PHONY: all clean
