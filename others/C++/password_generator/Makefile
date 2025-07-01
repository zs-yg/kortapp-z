CXX = g++
CXXFLAGS = -std=c++11 -Iinclude
LDFLAGS = -static -mwindows -lfltk -lole32 -luuid -lcomctl32 -lgdi32 -lwsock32 -lcomdlg32 -lwinspool

SRC = $(wildcard src/*.cpp)
OBJ = $(patsubst src/%.cpp,obj/%.o,$(SRC))
TARGET = password_generator

all: $(TARGET)

$(TARGET): $(OBJ)
	$(CXX) $^ -o $@ $(LDFLAGS)

obj/%.o: src/%.cpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	rm -f $(OBJ) $(TARGET)

.PHONY: all clean
