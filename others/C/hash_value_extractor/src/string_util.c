#include "string_util.h"
#include <stdlib.h>
#include <string.h>

// 内存池块大小
#define MEMORY_BLOCK_SIZE 4096

// 内存池结构
typedef struct MemoryBlock {
    char* buffer;
    size_t used;
    struct MemoryBlock* next;
} MemoryBlock;

static MemoryBlock* memory_pool = NULL;

// 初始化内存池
static void init_memory_pool() {
    if (memory_pool == NULL) {
        memory_pool = malloc(sizeof(MemoryBlock));
        memory_pool->buffer = malloc(MEMORY_BLOCK_SIZE);
        memory_pool->used = 0;
        memory_pool->next = NULL;
    }
}

// 从内存池分配
static char* pool_alloc(size_t size) {
    init_memory_pool();
    
    MemoryBlock* block = memory_pool;
    while (block != NULL) {
        if (MEMORY_BLOCK_SIZE - block->used >= size) {
            char* ptr = block->buffer + block->used;
            block->used += size;
            return ptr;
        }
        if (block->next == NULL) {
            block->next = malloc(sizeof(MemoryBlock));
            block = block->next;
            block->buffer = malloc(MEMORY_BLOCK_SIZE);
            block->used = 0;
            block->next = NULL;
        } else {
            block = block->next;
        }
    }
    return NULL;
}

char* str_alloc(size_t size) {
    // 小内存从池分配，大内存直接分配
    if (size <= MEMORY_BLOCK_SIZE / 4) {
        char* ptr = pool_alloc(size + 1); // +1 for null terminator
        if (ptr != NULL) {
            ptr[size] = '\0';
            return ptr;
        }
    }
    char* ptr = malloc(size + 1);
    if (ptr != NULL) {
        ptr[size] = '\0';
    }
    return ptr;
}

void str_free(char* str) {
    // 池分配的内存不单独释放
    if (str == NULL) return;
    
    // 检查是否在内存池中
    MemoryBlock* block = memory_pool;
    while (block != NULL) {
        if (str >= block->buffer && str < block->buffer + MEMORY_BLOCK_SIZE) {
            return; // 池内存不释放
        }
        block = block->next;
    }
    
    free(str);
}

char* str_copy(const char* src) {
    if (src == NULL) return NULL;
    
    size_t len = strlen(src);
    char* dest = str_alloc(len);
    if (dest != NULL) {
        memcpy(dest, src, len);
    }
    return dest;
}

char* str_concat(const char* str1, const char* str2) {
    if (str1 == NULL) return str_copy(str2);
    if (str2 == NULL) return str_copy(str1);
    
    size_t len1 = strlen(str1);
    size_t len2 = strlen(str2);
    char* result = str_alloc(len1 + len2);
    if (result != NULL) {
        memcpy(result, str1, len1);
        memcpy(result + len1, str2, len2);
    }
    return result;
}

char* bin_to_hex(const unsigned char* data, size_t len) {
    if (data == NULL || len == 0) return NULL;
    
    char* hex = str_alloc(len * 2);
    if (hex != NULL) {
        static const char hex_chars[] = "0123456789abcdef";
        for (size_t i = 0; i < len; i++) {
            hex[i * 2] = hex_chars[(data[i] >> 4) & 0x0F];
            hex[i * 2 + 1] = hex_chars[data[i] & 0x0F];
        }
    }
    return hex;
}
