#include "error.h"
#include "logger.h"
#include <string.h>
#include <stdlib.h>

static ErrorCode current_error = ERR_NONE;
static char* error_message = NULL;

void error_set(ErrorCode code, const char* message) {
    current_error = code;
    
    if (error_message != NULL) {
        free(error_message);
        error_message = NULL;
    }
    
    if (message != NULL) {
        error_message = strdup(message);
    }
    
    // 记录错误日志
    if (code != ERR_NONE) {
        const char* error_name;
        switch (code) {
            case ERR_FILE_NOT_FOUND: error_name = "File not found"; break;
            case ERR_INVALID_ARGUMENT: error_name = "Invalid argument"; break;
            case ERR_MEMORY_ALLOC: error_name = "Memory allocation failed"; break;
            case ERR_FFMPEG: error_name = "FFmpeg error"; break;
            default: error_name = "Unknown error"; break;
        }
        
        if (message != NULL) {
            logger_log(LOG_ERROR, "%s: %s", error_name, message);
        } else {
            logger_log(LOG_ERROR, "%s", error_name);
        }
    }
}

ErrorCode error_get_code() {
    return current_error;
}

const char* error_get_message() {
    return error_message;
}

void error_clear() {
    current_error = ERR_NONE;
    if (error_message != NULL) {
        free(error_message);
        error_message = NULL;
    }
}
