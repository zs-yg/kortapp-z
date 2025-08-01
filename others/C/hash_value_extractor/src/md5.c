#include "hash_calculator.h"
#include "string_util.h"
#include <openssl/evp.h>
#include <stdio.h>
#include <string.h>

int calculate_md5(const char* filename, char* output) {
    FILE* file = fopen(filename, "rb");
    if (!file) return -1;

    EVP_MD_CTX* mdctx = EVP_MD_CTX_new();
    const EVP_MD* md = EVP_md5();
    unsigned char hash[EVP_MAX_MD_SIZE];
    unsigned int hash_len = 0;

    EVP_DigestInit_ex(mdctx, md, NULL);

    unsigned char buffer[1024];
    size_t bytes_read;
    while ((bytes_read = fread(buffer, 1, sizeof(buffer), file))) {
        EVP_DigestUpdate(mdctx, buffer, bytes_read);
    }

    EVP_DigestFinal_ex(mdctx, hash, &hash_len);
    EVP_MD_CTX_free(mdctx);
    fclose(file);

    char* hex = bin_to_hex(hash, hash_len);
    if (hex) {
        strcpy(output, hex);
        str_free(hex);
        return 0;
    }
    return -1;
}
