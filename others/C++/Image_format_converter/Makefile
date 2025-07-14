CXX = g++
CXXFLAGS = -I"C:/msys64/ucrt64/include" -I"./include" -std=c++17 -Wall -mwindows
LDFLAGS = -L"C:/msys64/ucrt64/lib" -lfltk -lfltk_images -ltiff -ljpeg -lz -lzstd -lwebp -llerc -ljbig -llzma -ldeflate -lsharpyuv -lcomctl32 -lgdi32 -lole32 -luuid -lws2_32 -lwinspool -lcomdlg32 -static

SRC = src/main.cpp src/gui.cpp src/utils.cpp src/stb_impl.cpp \
      src/png_to_jpg.cpp src/jpg_to_png.cpp src/image_loader.cpp \
      src/bmp_to_png.cpp src/bmp_to_jpg.cpp src/png_to_bmp.cpp \
      src/jpg_to_bmp.cpp src/tiff_to_bmp.cpp src/bmp_to_tiff.cpp \
      src/tiff_to_png.cpp src/png_to_tiff.cpp src/tiff_to_jpg.cpp \
      src/jpg_to_tiff.cpp src/webp_to_jpg.cpp src/webp_to_png.cpp \
      src/webp_to_bmp.cpp src/webp_to_tiff.cpp src/jpg_to_webp.cpp \
      src/png_to_webp.cpp src/bmp_to_webp.cpp src/tiff_to_webp.cpp \
      src/jpeg_to_png.cpp src/jpeg_to_bmp.cpp src/jpeg_to_tiff.cpp \
      src/jpeg_to_webp.cpp src/png_to_jpeg.cpp src/bmp_to_jpeg.cpp \
      src/tiff_to_jpeg.cpp src/webp_to_jpeg.cpp
OBJ = $(SRC:src/%.cpp=obj/%.o)
TARGET = image_converter

all: $(TARGET)

$(TARGET): $(OBJ)
	$(CXX) $(CXXFLAGS) $^ -o $@ $(LDFLAGS)

obj/%.o: src/%.cpp
	$(CXX) $(CXXFLAGS) -c $< -o $@

clean:
	rm -f $(OBJ) $(TARGET)

.PHONY: all clean
