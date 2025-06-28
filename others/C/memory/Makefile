CC = gcc
CFLAGS = -Wall -Wextra -I./include
LDFLAGS = -Wl,-subsystem,windows -municode -luser32 -lgdi32 -lcomctl32 -lpsapi
SRC = src/main.c src/memory_ops.c src/ui.c src/init.c \
      src/utils.c src/config.c src/benchmark.c src/log.c \
      src/error.c src/version.c
OBJ = $(SRC:.c=.o)
TARGET = memory_trainer.exe

all: $(TARGET)

$(TARGET): $(OBJ)
	$(CC) $(CFLAGS) -o $(TARGET) $(OBJ) $(LDFLAGS)

.c.o:
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	del /Q $(subst /,\,$(OBJ)) $(TARGET)

.PHONY: all clean

