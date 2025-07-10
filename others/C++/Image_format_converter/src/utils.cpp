#include "common.hpp"
#include <algorithm>

ImageFormat get_format_from_extension(const std::string& path) {
    std::string ext = path.substr(path.find_last_of(".") + 1);
    std::transform(ext.begin(), ext.end(), ext.begin(), ::tolower);
    
    if (ext == "png") return ImageFormat::PNG;
    if (ext == "jpg" || ext == "jpeg") return ImageFormat::JPG;
    return ImageFormat::UNKNOWN;
}
