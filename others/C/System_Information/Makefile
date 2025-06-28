# 编译器设置
CC = gcc
CFLAGS = -Wall -Wextra -Iinclude -static
LDFLAGS = -static -mwindows -Wl,--subsystem,windows \
          -L"C:/msys64/ucrt64/lib" \
          -L"C:/msys64/ucrt64/x86_64-w64-mingw32/lib" \
          -lws2_32 -lole32 -loleaut32 -luuid \
          -Wl,-Bstatic -lstdc++ -lgcc -lwinpthread

# 目录设置
SRC_DIR = src
OBJ_DIR = obj
BUILD_DIR = build
INCLUDE_DIR = include

# 源文件和目标文件
SRCS = $(wildcard $(SRC_DIR)/*.c)
OBJS = $(patsubst $(SRC_DIR)/%.c,$(OBJ_DIR)/%.o,$(SRCS))
EXEC = $(BUILD_DIR)/system_info.exe

# 默认目标
all: $(EXEC)

# 链接目标文件生成可执行文件
$(EXEC): $(OBJS) | $(BUILD_DIR)
	$(CC) $(LDFLAGS) -o $@ $^

# 编译源文件生成目标文件
$(OBJ_DIR)/%.o: $(SRC_DIR)/%.c | $(OBJ_DIR)
	$(CC) $(CFLAGS) -c $< -o $@

# 创建必要的目录
$(BUILD_DIR):
	mkdir -p $(BUILD_DIR)

$(OBJ_DIR):
	mkdir -p $(OBJ_DIR)

# 清理生成的文件
clean:
	rm -rf $(OBJ_DIR) $(BUILD_DIR)

.PHONY: all clean
