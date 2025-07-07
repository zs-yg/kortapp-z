# Makefile for Zero-Width Steganography tool

CXX = g++
CXXFLAGS = -Iinclude -Wall -Wextra -std=c++11 -static
LDFLAGS = -static -lfltk -lfltk_images -lole32 -luuid -lcomctl32 -lgdi32 -lws2_32 -ldwmapi -lwinspool

SRC_DIR = src
SRCS = $(SRC_DIR)/main.cpp \
        $(SRC_DIR)/GUI.cpp \
        $(SRC_DIR)/Steganography.cpp \
        $(SRC_DIR)/ZeroWidthEncoder.cpp \
        $(SRC_DIR)/CommonDefsImpl.cpp
OBJS = $(SRCS:.cpp=.o)
EXEC = zwsteg.exe

# FLTK路径设置
FLTK_INCLUDE = "C:/msys64/ucrt64/include"
FLTK_LIB = "C:/msys64/ucrt64/lib"

all: $(EXEC)

$(EXEC): $(OBJS)
	$(CXX) $(CXXFLAGS) -o $@ $^ -L$(FLTK_LIB) $(LDFLAGS) \
		-ljpeg -lpng -lz -lgdiplus -lgdi32 -limm32

%.o: %.cpp
	$(CXX) $(CXXFLAGS) -I$(FLTK_INCLUDE) -c $< -o $@

clean:
	rm -f $(OBJS) $(EXEC)

.PHONY: all clean