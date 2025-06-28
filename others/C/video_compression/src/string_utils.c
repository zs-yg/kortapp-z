#include "string_utils.h"
#include <ctype.h>
#include <string.h>
#include <stdlib.h>

char* str_copy(char* dest, const char* src, size_t dest_size) {
    if (dest == NULL || src == NULL || dest_size == 0) {
        return NULL;
    }

    size_t i;
    for (i = 0; i < dest_size - 1 && src[i] != '\0'; i++) {
        dest[i] = src[i];
    }
    dest[i] = '\0';

    return dest;
}

char* str_concat(char* dest, const char* src, size_t dest_size) {
    if (dest == NULL || src == NULL || dest_size == 0) {
        return NULL;
    }

    size_t dest_len = strlen(dest);
    if (dest_len >= dest_size) {
        return dest;
    }

    size_t i;
    for (i = 0; i < dest_size - dest_len - 1 && src[i] != '\0'; i++) {
        dest[dest_len + i] = src[i];
    }
    dest[dest_len + i] = '\0';

    return dest;
}

char* str_trim(char* str) {
    if (str == NULL) {
        return NULL;
    }

    char* end;
    
    // 去除前导空白字符
    while (isspace((unsigned char)*str)) {
        str++;
    }

    // 如果全是空白字符
    if (*str == '\0') {
        return str;
    }

    // 去除尾部空白字符
    end = str + strlen(str) - 1;
    while (end > str && isspace((unsigned char)*end)) {
        end--;
    }

    // 写入终止符
    *(end + 1) = '\0';

    return str;
}

int str_starts_with(const char* str, const char* prefix) {
    if (str == NULL || prefix == NULL) {
        return 0;
    }

    size_t len_str = strlen(str);
    size_t len_prefix = strlen(prefix);

    if (len_prefix > len_str) {
        return 0;
    }

    return strncmp(str, prefix, len_prefix) == 0;
}

int str_ends_with(const char* str, const char* suffix) {
    if (str == NULL || suffix == NULL) {
        return 0;
    }

    size_t len_str = strlen(str);
    size_t len_suffix = strlen(suffix);

    if (len_suffix > len_str) {
        return 0;
    }

    return strncmp(str + len_str - len_suffix, suffix, len_suffix) == 0;
}

char* str_to_lower(char* str) {
    if (str == NULL) {
        return NULL;
    }

    for (char* p = str; *p != '\0'; p++) {
        *p = tolower((unsigned char)*p);
    }

    return str;
}

char* str_to_upper(char* str) {
    if (str == NULL) {
        return NULL;
    }

    for (char* p = str; *p != '\0'; p++) {
        *p = toupper((unsigned char)*p);
    }

    return str;
}
