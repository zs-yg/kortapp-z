#include <iostream>
#include <filesystem>
#include <chrono>
#include <vector>
#include <windows.h>

namespace fs = std::filesystem;

bool IsAdmin() {
    BOOL isAdmin = FALSE;
    SID_IDENTIFIER_AUTHORITY NtAuthority = SECURITY_NT_AUTHORITY;
    PSID AdminGroup;
    
    if (AllocateAndInitializeSid(&NtAuthority, 2,
        SECURITY_BUILTIN_DOMAIN_RID, DOMAIN_ALIAS_RID_ADMINS,
        0, 0, 0, 0, 0, 0, &AdminGroup)) {
        if (!CheckTokenMembership(NULL, AdminGroup, &isAdmin)) {
            isAdmin = FALSE;
        }
        FreeSid(AdminGroup);
    }
    return isAdmin == TRUE;
}

void CleanDirectory(const fs::path& dir, size_t& deletedCount, size_t& errorCount) {
    if (!fs::exists(dir)) {
        std::cout << "目录不存在: " << dir << std::endl;
        return;
    }

    for (const auto& entry : fs::directory_iterator(dir)) {
        try {
            if (fs::is_regular_file(entry)) {
                fs::remove(entry);
                deletedCount++;
            }
        } catch (const std::exception& e) {
            std::cerr << "删除文件失败: " << entry.path() << " - " << e.what() << std::endl;
            errorCount++;
        }
    }
}

int main() {
    try {
        if (!IsAdmin()) {
            std::cerr << "请以管理员身份运行此程序以清理所有目录" << std::endl;
        }

        auto start = std::chrono::high_resolution_clock::now();
        size_t totalDeleted = 0;
        size_t totalErrors = 0;

        // 定义要清理的目录列表
        std::vector<fs::path> directories = {
            "C:\\Windows\\Temp",
            fs::path(std::getenv("USERPROFILE")) / "AppData" / "Local" / "Temp",
            "C:\\Windows\\SoftwareDistribution\\Download",
            "C:\\Windows\\Logs",
            "C:\\Windows\\Minidump"
        };

        // 只有管理员才能清理Prefetch目录
        if (IsAdmin()) {
            directories.push_back("C:\\Windows\\Prefetch");
        }

        // 清理每个目录
        for (const auto& dir : directories) {
            size_t dirDeleted = 0;
            size_t dirErrors = 0;
            
            std::cout << "正在清理: " << dir << std::endl;
            CleanDirectory(dir, dirDeleted, dirErrors);
            
            totalDeleted += dirDeleted;
            totalErrors += dirErrors;
            
            std::cout << "已删除 " << dirDeleted << " 个文件" << std::endl;
        }

        auto end = std::chrono::high_resolution_clock::now();
        auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);

        std::cout << "\n清理完成:" << std::endl;
        std::cout << "总共删除文件数: " << totalDeleted << std::endl;
        std::cout << "总共错误数: " << totalErrors << std::endl;
        std::cout << "耗时: " << duration.count() << " 毫秒" << std::endl;

    } catch (const std::exception& e) {
        std::cerr << "发生错误: " << e.what() << std::endl;
        return 1;
    }

    return 0;
}
