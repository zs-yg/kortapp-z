# 编译器设置
CXX = g++
CXXFLAGS = -std=c++17 -Wall -Wextra -Iinclude
LDFLAGS = -lfltk -ltesseract -lleptonica

# 源文件和目标文件
SRCS = src/main.cpp src/gui_window.cpp src/ocr_engine.cpp \
       src/file_io.cpp src/error_handler.cpp src/image_processor.cpp
OBJS = $(patsubst src/%.cpp,obj/%.o,$(SRCS))

# 目标可执行文件
TARGET = ocr_app.exe

# 默认目标
all: $(TARGET)

# 链接规则
$(TARGET): $(OBJS)
	$(CXX) $(CXXFLAGS) -o $@ $^ $(LDFLAGS)

# 编译规则
obj/%.o: src/%.cpp
	@mkdir -p obj
	$(CXX) $(CXXFLAGS) -c -o $@ $<

# 清理
clean:
	rm -rf obj $(TARGET)

.PHONY: all clean
