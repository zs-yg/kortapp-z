#ifndef PROGRESS_H
#define PROGRESS_H

/**
 * 初始化进度显示
 * @param total 总工作量
 */
void progress_init(long total);

/**
 * 更新进度
 * @param current 当前进度
 */
void progress_update(long current);

/**
 * 完成进度显示
 */
void progress_finish();

#endif // PROGRESS_H
