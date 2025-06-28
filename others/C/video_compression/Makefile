# 编译器设置
CC = gcc
CFLAGS = -Wall -Wextra -O2 -Iinclude
LDFLAGS = -lavcodec -lavformat -lavutil -lswscale -lgdi32 -lcomdlg32 -mwindows

# 目录设置
OBJ_DIR = obj
BUILD_DIR = build

# 源文件和目标文件
SRC = $(wildcard src/*.c)
OBJ = $(patsubst src/%.c,$(OBJ_DIR)/%.o,$(SRC))
EXEC = $(BUILD_DIR)/video_compressor.exe

# 创建目录
$(shell mkdir -p $(OBJ_DIR))
$(shell mkdir -p $(BUILD_DIR))

# 默认目标
all: $(EXEC)

# 链接可执行文件
$(EXEC): $(OBJ)
	$(CC) -o $@ $^ $(LDFLAGS)

# 编译规则
$(OBJ_DIR)/%.o: src/%.c
	$(CC) $(CFLAGS) -c $< -o $@

# 清理规则
clean:
	del /Q $(OBJ_DIR)\*.o $(EXEC)

.PHONY: all clean
