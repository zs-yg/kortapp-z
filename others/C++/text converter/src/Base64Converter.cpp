#include "../include/Base64Converter.hpp"
#include <openssl/bio.h>
#include <openssl/evp.h>
#include <string>

std::string Base64Converter::convert(const std::string& input) {
    // Base64编码
    BIO *b64 = BIO_new(BIO_f_base64());
    BIO *bio = BIO_new(BIO_s_mem());
    BIO_push(b64, bio);
    BIO_set_flags(b64, BIO_FLAGS_BASE64_NO_NL);

    BIO_write(b64, input.c_str(), input.length());
    BIO_flush(b64);

    char* buffer;
    long length = BIO_get_mem_data(bio, &buffer);
    std::string result(buffer, length);
    
    BIO_free_all(b64);
    return result;
}
