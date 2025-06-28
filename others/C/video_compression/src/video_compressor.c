#include "video_compressor.h"
#include "string_utils.h"
#include <libavcodec/avcodec.h>
#include <libavformat/avformat.h>
#include <libavutil/opt.h>
#include <libswscale/swscale.h>
#include <stdio.h>
#include <sys/stat.h>  // For mkdir()
#include <errno.h>     // For errno

// 将质量参数(1-10000)转换为FFmpeg的CRF值(0-51)
static int convert_quality_to_crf(int quality) {
    // 更激进的映射关系，确保中等质量也能强力压缩
    if (quality < 3000) return 45 - (quality * 15) / 3000;  // 低质量区间(30-45)
    if (quality < 7000) return 30 - ((quality-3000) * 10) / 4000; // 中等质量区间(20-30)
    return 20 - ((quality-7000) * 2) / 3000;  // 高质量区间(18-20)
}

int compress_video(const char* input_file, const char* output_file, int quality) {
    AVFormatContext *input_ctx = NULL;
    AVFormatContext *output_ctx = NULL;
    AVCodecContext *encoder_ctx = NULL;
    const AVCodec *encoder = NULL;
    AVPacket *packet = NULL;
    int ret = 0;
    int video_stream_index = -1;
    int crf = convert_quality_to_crf(quality);

    // 打开输入文件
    if (avformat_open_input(&input_ctx, input_file, NULL, NULL) < 0) {
        fprintf(stderr, "无法打开输入文件\n");
        ret = -1;
        goto cleanup;
    }

    // 获取流信息
    if (avformat_find_stream_info(input_ctx, NULL) < 0) {
        fprintf(stderr, "无法获取流信息\n");
        ret = -1;
        goto cleanup;
    }

    // 查找视频流
    for (unsigned int i = 0; i < input_ctx->nb_streams; i++) {
        if (input_ctx->streams[i]->codecpar->codec_type == AVMEDIA_TYPE_VIDEO) {
            video_stream_index = i;
            break;
        }
    }

    if (video_stream_index == -1) {
        fprintf(stderr, "未找到视频流\n");
        ret = -1;
        goto cleanup;
    }

    // 创建输出上下文
    if (avformat_alloc_output_context2(&output_ctx, NULL, NULL, output_file) < 0) {
        fprintf(stderr, "无法创建输出上下文\n");
        ret = -1;
        goto cleanup;
    }

    // 打开输出文件
    if (!(output_ctx->oformat->flags & AVFMT_NOFILE)) {
        if (avio_open(&output_ctx->pb, output_file, AVIO_FLAG_WRITE) < 0) {
            fprintf(stderr, "无法打开输出文件\n");
            ret = -1;
            goto cleanup;
        }
    }

    // 配置编码器
    encoder = avcodec_find_encoder(AV_CODEC_ID_H264);
    if (!encoder) {
        fprintf(stderr, "未找到H.264编码器\n");
        ret = -1;
        goto cleanup;
    }

    encoder_ctx = avcodec_alloc_context3(encoder);
    if (!encoder_ctx) {
        fprintf(stderr, "无法分配编码器上下文\n");
        ret = -1;
        goto cleanup;
    }

    // 设置更激进的编码参数
    int64_t input_bit_rate = input_ctx->bit_rate;
    if (input_bit_rate <= 0) {
        input_bit_rate = 400000;  // 默认值
    }
    // 设置严格的编码参数确保播放兼容性
    encoder_ctx->bit_rate = input_bit_rate * 0.5;
    encoder_ctx->rc_max_rate = input_bit_rate * 0.7;
    encoder_ctx->rc_min_rate = input_bit_rate * 0.3;
    encoder_ctx->rc_buffer_size = input_bit_rate * 0.7 / 25;
    encoder_ctx->level = 31;  // 明确设置level 3.1确保广泛兼容
    encoder_ctx->width = input_ctx->streams[video_stream_index]->codecpar->width;
    encoder_ctx->height = input_ctx->streams[video_stream_index]->codecpar->height;
    encoder_ctx->time_base = (AVRational){1, 25};
    encoder_ctx->framerate = (AVRational){25, 1};
    encoder_ctx->gop_size = 10;
    encoder_ctx->max_b_frames = 1;
    encoder_ctx->pix_fmt = AV_PIX_FMT_YUV420P;
    
    // 设置更激进的x264参数
    av_opt_set_int(encoder_ctx->priv_data, "crf", crf, 0);
    av_opt_set(encoder_ctx->priv_data, "preset", "slow", 0);  // 使用slow预设提高压缩率
    // 设置强制兼容性参数
    av_opt_set(encoder_ctx->priv_data, "profile", "main", 0);
    av_opt_set(encoder_ctx->priv_data, "movflags", "+faststart", 0);  // 添加流式播放支持
    av_opt_set(encoder_ctx->priv_data, "tune", "zerolatency", 0);
    av_opt_set(encoder_ctx->priv_data, "x264-params", "ref=1:bframes=0:subme=2:me=dia:analyse=none:trellis=0", 0);
    encoder_ctx->flags |= AV_CODEC_FLAG_GLOBAL_HEADER;
    encoder_ctx->thread_count = 4;  // 启用4线程编码
    encoder_ctx->thread_type = FF_THREAD_FRAME;  // 帧级多线程
    encoder_ctx->gop_size = 250;  // 更大的GOP提高压缩率
    encoder_ctx->max_b_frames = 0;  // 禁用B帧提高解码兼容性

    // 打开编码器
    if (avcodec_open2(encoder_ctx, encoder, NULL) < 0) {
        fprintf(stderr, "无法打开编码器\n");
        ret = -1;
        goto cleanup;
    }

    // 添加视频流到输出文件
    AVStream *out_stream = avformat_new_stream(output_ctx, NULL);
    if (!out_stream) {
        fprintf(stderr, "无法创建输出流\n");
        ret = -1;
        goto cleanup;
    }

    // 复制编码器参数到输出流
    if (avcodec_parameters_from_context(out_stream->codecpar, encoder_ctx) < 0) {
        fprintf(stderr, "无法复制编码器参数\n");
        ret = -1;
        goto cleanup;
    }

    // 写入文件头
    if (avformat_write_header(output_ctx, NULL) < 0) {
        fprintf(stderr, "写入文件头失败\n");
        ret = -1;
        goto cleanup;
    }

    packet = av_packet_alloc();
    if (!packet) {
        fprintf(stderr, "无法分配AVPacket\n");
        ret = -1;
        goto cleanup;
    }

    // 初始化解码器
    const AVCodec *decoder = avcodec_find_decoder(input_ctx->streams[video_stream_index]->codecpar->codec_id);
    if (!decoder) {
        fprintf(stderr, "未找到解码器\n");
        ret = -1;
        goto cleanup;
    }

    AVCodecContext *decoder_ctx = avcodec_alloc_context3(decoder);
    if (!decoder_ctx) {
        fprintf(stderr, "无法分配解码器上下文\n");
        ret = -1;
        goto cleanup;
    }

    if (avcodec_parameters_to_context(decoder_ctx, input_ctx->streams[video_stream_index]->codecpar) < 0) {
        fprintf(stderr, "无法复制解码器参数\n");
        ret = -1;
        goto cleanup;
    }

    if (avcodec_open2(decoder_ctx, decoder, NULL) < 0) {
        fprintf(stderr, "无法打开解码器\n");
        ret = -1;
        goto cleanup;
    }

    // 初始化帧和转换上下文
    AVFrame *frame = av_frame_alloc();
    AVFrame *tmp_frame = av_frame_alloc();
    if (!frame || !tmp_frame) {
        fprintf(stderr, "无法分配帧\n");
        ret = -1;
        goto cleanup;
    }

    // 分配输入帧缓冲区
    frame->format = decoder_ctx->pix_fmt;
    frame->width = decoder_ctx->width;
    frame->height = decoder_ctx->height;
    if (av_frame_get_buffer(frame, 32) < 0) {
        fprintf(stderr, "无法分配输入帧缓冲区\n");
        ret = -1;
        goto cleanup;
    }

    // 分配输出帧缓冲区
    tmp_frame->format = encoder_ctx->pix_fmt;
    tmp_frame->width = encoder_ctx->width;
    tmp_frame->height = encoder_ctx->height;
    if (av_frame_get_buffer(tmp_frame, 32) < 0) {
        fprintf(stderr, "无法分配输出帧缓冲区\n");
        ret = -1;
        goto cleanup;
    }

    struct SwsContext *sws_ctx = NULL;
    sws_ctx = sws_getContext(
        decoder_ctx->width, decoder_ctx->height, decoder_ctx->pix_fmt,
        encoder_ctx->width, encoder_ctx->height, encoder_ctx->pix_fmt,
        SWS_BILINEAR, NULL, NULL, NULL);
    if (!sws_ctx) {
        fprintf(stderr, "无法创建图像转换上下文\n");
        ret = -1;
        goto cleanup;
    }
    
    // 彻底解决目录权限问题
    if (mkdir("build") != 0 && errno != EEXIST) {
        fprintf(stderr, "错误：无法创建build目录(errno=%d)，请手动创建并设置写权限\n", errno);
        ret = -1;
        goto cleanup;
    }
    
    // 验证目录可写权限
    FILE *test_file = fopen("build/test_permission.tmp", "w");
    if (!test_file) {
        fprintf(stderr, "致命错误：无法写入build目录(errno=%d)，请确保：\n1. 目录存在且有写权限\n2. 没有其他程序锁定该目录\n3. 磁盘空间充足\n", errno);
        ret = -1;
        goto cleanup;
    }
    fclose(test_file);
    remove("build/test_permission.tmp");

    // 解码和编码循环
    while (av_read_frame(input_ctx, packet) >= 0) {
        if (packet->stream_index == video_stream_index) {
            // 发送包到解码器
            if (avcodec_send_packet(decoder_ctx, packet) < 0) {
                fprintf(stderr, "解码错误\n");
                continue;
            }

            // 接收解码后的帧
            while (avcodec_receive_frame(decoder_ctx, frame) >= 0) {
                // 转换像素格式
                sws_scale(sws_ctx, (const uint8_t * const*)frame->data,
                          frame->linesize, 0, decoder_ctx->height,
                          tmp_frame->data, tmp_frame->linesize);

                // 设置帧参数
                tmp_frame->format = encoder_ctx->pix_fmt;
                tmp_frame->width = encoder_ctx->width;
                tmp_frame->height = encoder_ctx->height;
                tmp_frame->pts = frame->pts;

                // 发送帧到编码器
                if (avcodec_send_frame(encoder_ctx, tmp_frame) < 0) {
                    fprintf(stderr, "编码错误\n");
                    break;
                }

                // 接收编码后的包
                while (avcodec_receive_packet(encoder_ctx, packet) >= 0) {
                    // 写入输出文件
                    if (av_interleaved_write_frame(output_ctx, packet) < 0) {
                        fprintf(stderr, "写入帧失败\n");
                        av_packet_unref(packet);
                        ret = -1;
                        goto cleanup;
                    }
                    av_packet_unref(packet);
                }
            }
        }
        av_packet_unref(packet);
    }

    // 刷新编码器
    avcodec_send_frame(encoder_ctx, NULL);
    while (avcodec_receive_packet(encoder_ctx, packet) >= 0) {
        if (av_interleaved_write_frame(output_ctx, packet) < 0) {
            fprintf(stderr, "写入帧失败\n");
            av_packet_unref(packet);
            ret = -1;
            goto cleanup;
        }
        av_packet_unref(packet);
    }

    // 写入文件尾
    av_write_trailer(output_ctx);

cleanup:
    if (packet) av_packet_free(&packet);
    if (frame) av_frame_free(&frame);
    if (tmp_frame) av_frame_free(&tmp_frame);
    if (sws_ctx) sws_freeContext(sws_ctx);
    if (decoder_ctx) avcodec_free_context(&decoder_ctx);
    if (input_ctx) avformat_close_input(&input_ctx);
    if (output_ctx && !(output_ctx->oformat->flags & AVFMT_NOFILE)) {
        avio_closep(&output_ctx->pb);
    }
    if (output_ctx) avformat_free_context(output_ctx);
    if (encoder_ctx) avcodec_free_context(&encoder_ctx);

    return ret;
}

int init_video_compressor() {
    // 新版本FFmpeg不需要显式初始化
    return 0;
}

void cleanup_video_compressor() {
    // 当前无需特殊清理
}
