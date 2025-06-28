#ifndef DISK_INFO_H
#define DISK_INFO_H

#include <windows.h>

typedef struct {
    char driveLetter;
    DWORD64 totalBytes;
    DWORD64 freeBytes;
    char fileSystem[32];
} DiskInfo;

void get_disk_info(DiskInfo* disks, int* count);

#endif // DISK_INFO_H
