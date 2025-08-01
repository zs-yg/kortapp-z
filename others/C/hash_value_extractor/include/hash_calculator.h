#ifndef HASH_CALCULATOR_H
#define HASH_CALCULATOR_H

#include <stddef.h>

typedef enum {
    HASH_MD5,
    HASH_SHA256,
    HASH_SHA512
} HashAlgorithm;

// 各算法计算函数
int calculate_md5(const char* filename, char* output);
int calculate_sha256(const char* filename, char* output);
int calculate_sha512(const char* filename, char* output);

/**
 * 计算文件的哈希值
 * @param filename 文件路径
 * @param algorithm 哈希算法
 * @param output 输出缓冲区(必须足够大)
 * @return 成功返回0，失败返回-1
 */
static inline int calculate_file_hash(const char* filename, HashAlgorithm algorithm, char* output) {
    switch (algorithm) {
        case HASH_MD5:
            return calculate_md5(filename, output);
        case HASH_SHA256:
            return calculate_sha256(filename, output);
        case HASH_SHA512:
            return calculate_sha512(filename, output);
        default:
            return -1;
    }
}

#endif // HASH_CALCULATOR_H
