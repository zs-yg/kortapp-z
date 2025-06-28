#include "network_info.h"
#include "config.h"
#include <stdio.h>

// 定义函数指针类型
typedef DWORD (WINAPI *GetAdaptersInfoFunc)(_Out_ PIP_ADAPTER_INFO, _Inout_ PULONG);

// 安全的函数指针转换函数
static FARPROC safe_get_proc_address(HMODULE module, const char* name) {
    FARPROC proc = GetProcAddress(module, name);
    if (!proc) {
        return NULL;
    }
    return proc;
}

// 辅助宏用于安全的函数指针转换
#define GET_PROC_ADDRESS(module, name, type) \
    ((type)(void*)safe_get_proc_address(module, name))

void get_network_adapters(NetworkAdapterInfo* adapters, int* count) {
    HMODULE hModule = LoadLibrary("iphlpapi.dll");
    if (!hModule) return;

    // 使用类型安全的函数指针转换
    GetAdaptersInfoFunc pGetAdaptersInfo = GET_PROC_ADDRESS(hModule, "GetAdaptersInfo", GetAdaptersInfoFunc);
    if (!pGetAdaptersInfo) {
        FreeLibrary(hModule);
        return;
    }

    PIP_ADAPTER_INFO pAdapterInfo = NULL;
    PIP_ADAPTER_INFO pAdapter = NULL;
    DWORD dwRetVal = 0;
    ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
    
    *count = 0;

    pAdapterInfo = (IP_ADAPTER_INFO*)malloc(sizeof(IP_ADAPTER_INFO));
    if (pAdapterInfo == NULL) {
        FreeLibrary(hModule);
        return;
    }

    // 第一次调用获取缓冲区大小
    if (pGetAdaptersInfo(pAdapterInfo, &ulOutBufLen) == ERROR_BUFFER_OVERFLOW) {
        free(pAdapterInfo);
        pAdapterInfo = (IP_ADAPTER_INFO*)malloc(ulOutBufLen);
        if (pAdapterInfo == NULL) {
            FreeLibrary(hModule);
            return;
        }
    }

    if ((dwRetVal = pGetAdaptersInfo(pAdapterInfo, &ulOutBufLen)) == NO_ERROR) {
        pAdapter = pAdapterInfo;
        while (pAdapter && *count < MAX_ADAPTERS) {
            NetworkAdapterInfo* adapter = &adapters[(*count)++];
            
            // 复制适配器名称和描述
            strncpy(adapter->adapterName, pAdapter->AdapterName, MAX_ADAPTER_NAME_LENGTH);
            strncpy(adapter->description, pAdapter->Description, MAX_ADAPTER_DESCRIPTION_LENGTH);
            
            // 格式化IP地址
            if (pAdapter->IpAddressList.IpAddress.String[0] != '\0') {
                strncpy(adapter->ipAddress, pAdapter->IpAddressList.IpAddress.String, 15);
                adapter->ipAddress[15] = '\0';
            } else {
                strcpy(adapter->ipAddress, "N/A");
            }
            
            // 格式化MAC地址
            sprintf(adapter->macAddress, "%02X:%02X:%02X:%02X:%02X:%02X",
                   pAdapter->Address[0], pAdapter->Address[1],
                   pAdapter->Address[2], pAdapter->Address[3],
                   pAdapter->Address[4], pAdapter->Address[5]);
            
            adapter->speed = pAdapter->DhcpEnabled ? 100 : 10; // 简化处理，实际项目中应使用更精确的方法
            
            pAdapter = pAdapter->Next;
        }
    }

    free(pAdapterInfo);
}
