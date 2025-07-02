#include "disk_info.h"
#include <tchar.h>

void get_disk_info(DiskInfo* disks, int* count) {
    DWORD drives = GetLogicalDrives();
    *count = 0;

    for (char drive = 'A'; drive <= 'Z'; drive++) {
        if (drives & (1 << (drive - 'A'))) {
            TCHAR rootPath[4] = {drive, ':', '\\', '\0'};
            TCHAR fileSystem[32];
            DWORD serialNumber, maxComponentLength, fileSystemFlags;

            if (GetVolumeInformation(
                rootPath,
                NULL, 0,
                &serialNumber,
                &maxComponentLength,
                &fileSystemFlags,
                fileSystem, sizeof(fileSystem))) {

                DiskInfo* disk = &disks[(*count)++];
                disk->driveLetter = drive;

                ULARGE_INTEGER freeBytes, totalBytes, totalFreeBytes;
                if (GetDiskFreeSpaceEx(
                    rootPath,
                    &freeBytes,
                    &totalBytes,
                    &totalFreeBytes)) {
                    disk->totalBytes = totalBytes.QuadPart;
                    disk->freeBytes = freeBytes.QuadPart;
                }

                strncpy(disk->fileSystem, fileSystem, sizeof(disk->fileSystem) - 1);
                disk->fileSystem[sizeof(disk->fileSystem) - 1] = '\0';
            }
        }
    }
}
