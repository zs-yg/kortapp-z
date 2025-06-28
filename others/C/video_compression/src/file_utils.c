#include "file_utils.h"
#include <sys/stat.h>
#include <string.h>
#include <stdio.h>

bool file_exists(const char* path) {
    if (path == NULL) {
        return false;
    }

    struct stat buffer;
    return stat(path, &buffer) == 0;
}

long file_size(const char* path) {
    if (path == NULL) {
        return -1;
    }

    struct stat buffer;
    if (stat(path, &buffer) != 0) {
        return -1;
    }

    return (long)buffer.st_size;
}

const char* file_extension(const char* path) {
    if (path == NULL) {
        return NULL;
    }

    const char* dot = strrchr(path, '.');
    if (dot == NULL || dot == path) {
        return NULL;
    }

    return dot;
}
