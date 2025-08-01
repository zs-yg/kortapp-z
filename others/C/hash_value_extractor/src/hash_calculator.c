#include "hash_calculator.h"
#include <openssl/md5.h>
#include <openssl/sha.h>
#include <stdio.h>
#include <stdlib.h>

int calculate_file_hash(const char* filename, HashAlgorithm algorithm, char* output) {
    FILE* file = fopen(filename, "rb");
    if (!file) return -1;

    const EVP_MD* md = NULL;
    EVP_MD_CTX* mdctx = EVP_MD_CTX_new();
    unsigned char hash[EVP_MAX_MD_SIZE];
    unsigned int hash_len = 0;

    switch (algorithm) {
        case HASH_MD5:
            md = EVP_md5();
            break;
        case HASH_SHA256:
            md = EVP_sha256();
            break;
        case HASH_SHA512:
            md = EVP_sha512();
            break;
        default:
            fclose(file);
            return -1;
    }

    EVP_DigestInit_ex(mdctx, md, NULL);

    unsigned char buffer[1024];
    size_t bytes_read;
    while ((bytes_read = fread(buffer, 1, sizeof(buffer), file))) {
        EVP_DigestUpdate(mdctx, buffer, bytes_read);
    }

    EVP_DigestFinal_ex(mdctx, hash, &hash_len);
    EVP_MD_CTX_free(mdctx);
    fclose(file);

    hash_to_hex(hash, hash_len, output);
    return 0;
}

void hash_to_hex(const unsigned char* hash, size_t hash_len, char* output) {
    for (size_t i = 0; i < hash_len; i++) {
        sprintf(output + (i * 2), "%02x", hash[i]);
    }
    output[hash_len * 2] = '\0';
}
</fitten_content>
