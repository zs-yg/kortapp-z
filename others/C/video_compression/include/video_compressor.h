#ifndef VIDEO_COMPRESSOR_H
#define VIDEO_COMPRESSOR_H

/**
 * 压缩视频文件
 * @param input_file 输入文件路径
 * @param output_file 输出文件路径
 * @param quality 压缩质量(1-10000)
 * @return 0表示成功，非0表示失败
 */
int compress_video(const char* input_file, const char* output_file, int quality);

/**
 * 初始化视频压缩模块
 * @return 0表示成功，非0表示失败
 */
int init_video_compressor();

/**
 * 清理视频压缩模块资源
 */
void cleanup_video_compressor();

#endif // VIDEO_COMPRESSOR_H
