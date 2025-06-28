#ifndef NETWORK_INFO_H
#define NETWORK_INFO_H

#include <windows.h>
#include <iphlpapi.h>

// 确保GetAdaptersInfo函数声明
#ifndef _IPHLPAPI_
#define _IPHLPAPI_
DWORD GetAdaptersInfo(PIP_ADAPTER_INFO pAdapterInfo, PULONG pOutBufLen);
#endif

typedef struct {
    char adapterName[MAX_ADAPTER_NAME_LENGTH + 4];
    char description[MAX_ADAPTER_DESCRIPTION_LENGTH + 4];
    char ipAddress[16];
    char macAddress[18];
    ULONG speed; // in Mbps
} NetworkAdapterInfo;

void get_network_adapters(NetworkAdapterInfo* adapters, int* count);

#endif // NETWORK_INFO_H
