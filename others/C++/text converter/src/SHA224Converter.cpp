#include "../include/SHA224Converter.hpp"
#include <openssl/evp.h>
#include <sstream>
#include <iomanip>
#include <stdexcept>

std::string SHA224Converter::convert(const std::string& input) {
    EVP_MD_CTX* mdctx = EVP_MD_CTX_new();
    if (!mdctx) {
        throw std::runtime_error("Failed to create EVP_MD_CTX");
    }

    const EVP_MD* md = EVP_sha224();
    if (!md) {
        EVP_MD_CTX_free(mdctx);
        throw std::runtime_error("SHA224 not supported by this OpenSSL version");
    }

    unsigned char digest[EVP_MAX_MD_SIZE];
    unsigned int digest_len;

    if (EVP_DigestInit_ex(mdctx, md, nullptr) != 1) {
        EVP_MD_CTX_free(mdctx);
        throw std::runtime_error("Failed to initialize digest");
    }

    if (EVP_DigestUpdate(mdctx, input.c_str(), input.length()) != 1) {
        EVP_MD_CTX_free(mdctx);
        throw std::runtime_error("Failed to update digest");
    }

    if (EVP_DigestFinal_ex(mdctx, digest, &digest_len) != 1) {
        EVP_MD_CTX_free(mdctx);
        throw std::runtime_error("Failed to finalize digest");
    }

    EVP_MD_CTX_free(mdctx);

    std::stringstream ss;
    for(unsigned int i = 0; i < digest_len; i++) {
        ss << std::hex << std::setw(2) << std::setfill('0') << (int)digest[i];
    }

    return ss.str();
}
