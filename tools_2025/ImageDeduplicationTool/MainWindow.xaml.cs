using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Collections.Concurrent;

namespace ImageDeduplicationTool
{
    public partial class MainWindow : Window
    {
        private List<string> imageFiles = new List<string>();
        private readonly string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };

        // 控制变量
        private CancellationTokenSource cancellationTokenSource;
        private bool isPaused = false;
        private readonly object pauseLock = new object();
        private int currentGroupIndex = 0;
        private int totalGroups = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // 使用WPF的OpenFileDialog来模拟文件夹选择
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.Description = "选择包含图片的文件夹";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        FolderPathTextBox.Text = selectedPath;
                        FolderPathTextBlock.Text = $"已选择文件夹: {selectedPath}";

                        // 清空之前的统计
                        TotalImagesText.Text = "0";
                        DuplicateImagesText.Text = "0";
                        CurrentGroupText.Text = "0/0";
                        ProgressBar.Value = 0;
                        ProgressText.Text = "等待开始...";
                        LogTextBox.Text = "";
                    }
                }
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FolderPathTextBox.Text))
            {
                System.Windows.MessageBox.Show("请先选择文件夹！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Directory.Exists(FolderPathTextBox.Text))
            {
                System.Windows.MessageBox.Show("选择的文件夹不存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 初始化取消令牌
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // 更新按钮状态
            UpdateButtonState(true);

            try
            {
                await StartDeduplicationProcess(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogMessage("处理已被用户停止");
                ProgressText.Text = "已停止";
            }
            catch (Exception ex)
            {
                LogMessage($"处理过程中发生错误: {ex.Message}");
                System.Windows.MessageBox.Show($"处理失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // 重新启用按钮
                UpdateButtonState(false);
                isPaused = false;
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            lock (pauseLock)
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    PauseButton.Content = "继续";
                    PauseButton.Background = System.Windows.Media.Brushes.LightBlue;
                    ProgressText.Text = "已暂停...";
                    LogMessage("处理已暂停");
                }
                else
                {
                    PauseButton.Content = "暂停";
                    PauseButton.Background = System.Windows.Media.Brushes.LightYellow;
                    ProgressText.Text = "继续处理...";
                    LogMessage("处理继续");
                    Monitor.PulseAll(pauseLock); // 唤醒等待的线程
                }
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
            {
                var result = System.Windows.MessageBox.Show("确定要停止处理吗？", "确认停止",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    cancellationTokenSource.Cancel();
                    LogMessage("正在停止处理...");

                    // 如果处于暂停状态，先恢复再停止
                    lock (pauseLock)
                    {
                        if (isPaused)
                        {
                            isPaused = false;
                            Monitor.PulseAll(pauseLock);
                        }
                    }
                }
            }
        }

        private void UpdateButtonState(bool isProcessing)
        {
            StartButton.IsEnabled = !isProcessing;
            SelectFolderButton.IsEnabled = !isProcessing;
            PauseButton.IsEnabled = isProcessing;
            StopButton.IsEnabled = isProcessing;

            if (!isProcessing)
            {
                PauseButton.Content = "暂停";
                PauseButton.Background = System.Windows.Media.Brushes.LightYellow;
            }
        }

        private async Task StartDeduplicationProcess(CancellationToken cancellationToken)
        {
            // 步骤1：获取所有图片文件
            LogMessage("正在搜索图片文件...");
            ProgressText.Text = "搜索图片文件中...";

            imageFiles = await GetAllImageFilesAsync(FolderPathTextBox.Text, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            int totalImages = imageFiles.Count;

            TotalImagesText.Text = totalImages.ToString();
            ProgressBar.Maximum = totalImages;
            ProgressBar.Value = 0;

            LogMessage($"找到 {totalImages} 个图片文件");

            if (totalImages == 0)
            {
                LogMessage("没有找到图片文件，处理结束");
                return;
            }

            // 步骤2：按组处理图片去重
            int groupSize = 100000;
            totalGroups = (int)Math.Ceiling((double)totalImages / groupSize);
            int totalDuplicates = 0;

            LogMessage($"开始分组处理，共 {totalGroups} 组");

            for (currentGroupIndex = 0; currentGroupIndex < totalGroups; currentGroupIndex++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // 等待暂停状态
                WaitIfPaused(cancellationToken);

                int startIndex = currentGroupIndex * groupSize;
                int endIndex = Math.Min(startIndex + groupSize, totalImages);
                int groupCount = endIndex - startIndex;

                // 更新当前组显示
                Dispatcher.Invoke(() =>
                {
                    CurrentGroupText.Text = $"{currentGroupIndex + 1}/{totalGroups}";
                });

                LogMessage($"处理第 {currentGroupIndex + 1}/{totalGroups} 组，包含 {groupCount} 个图片");

                var groupFiles = imageFiles.GetRange(startIndex, groupCount);
                int groupDuplicates = await ProcessImageGroupAsync(groupFiles, currentGroupIndex + 1, totalGroups, cancellationToken);

                totalDuplicates += groupDuplicates;
                Dispatcher.Invoke(() =>
                {
                    DuplicateImagesText.Text = totalDuplicates.ToString();
                });

                LogMessage($"第 {currentGroupIndex + 1} 组处理完成，发现 {groupDuplicates} 个重复图片");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                LogMessage("处理已被用户停止");
                ProgressText.Text = "已停止";
            }
            else
            {
                LogMessage($"处理完成！总共发现并删除了 {totalDuplicates} 个重复图片");
                ProgressText.Text = "处理完成！";
                System.Windows.MessageBox.Show($"处理完成！总共发现并删除了 {totalDuplicates} 个重复图片", "完成",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void WaitIfPaused(CancellationToken cancellationToken)
        {
            lock (pauseLock)
            {
                while (isPaused && !cancellationToken.IsCancellationRequested)
                {
                    Monitor.Wait(pauseLock, 100); // 每100ms检查一次
                }
            }
        }

        private async Task<List<string>> GetAllImageFilesAsync(string folderPath, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var files = new List<string>();
                try
                {
                    var searchOption = SearchOption.AllDirectories;
                    foreach (var extension in imageExtensions)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        var foundFiles = Directory.GetFiles(folderPath, $"*{extension}", searchOption)
                                                .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
                        files.AddRange(foundFiles);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"搜索文件时出错: {ex.Message}");
                }
                return files;
            }, cancellationToken);
        }

        private async Task<int> ProcessImageGroupAsync(List<string> imageFiles, int currentGroup, int totalGroups, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                int duplicatesFound = 0;
                var fileHashes = new ConcurrentDictionary<string, string>();
                var filesToDelete = new ConcurrentBag<string>();

                try
                {
                    // 使用并行处理提高效率
                    Parallel.ForEach(imageFiles, new ParallelOptions
                    {
                        CancellationToken = cancellationToken
                    }, (file, state) =>
                    {
                        // 检查取消请求
                        if (cancellationToken.IsCancellationRequested)
                        {
                            state.Stop();
                            return;
                        }

                        // 等待暂停状态
                        WaitIfPaused(cancellationToken);

                        try
                        {
                            // 更新进度
                            Dispatcher.Invoke(() =>
                            {
                                ProgressBar.Value++;
                                ProgressText.Text = $"处理中: {ProgressBar.Value}/{ProgressBar.Maximum} (组 {currentGroup}/{totalGroups})";
                            });

                            string fileHash = CalculateFileHash(file, cancellationToken);

                            // 如果哈希已存在，则标记为重复
                            if (!fileHashes.TryAdd(fileHash, file))
                            {
                                filesToDelete.Add(file);
                                duplicatesFound++;
                                Dispatcher.Invoke(() => LogMessage($"发现重复图片: {Path.GetFileName(file)}"));
                            }
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(() => LogMessage($"处理文件 {file} 时出错: {ex.Message}"));
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    LogMessage($"第 {currentGroup} 组处理被取消");
                }

                // 删除重复文件（如果未被取消）
                if (!cancellationToken.IsCancellationRequested)
                {
                    foreach (var file in filesToDelete)
                    {
                        try
                        {
                            File.Delete(file);
                            LogMessage($"已删除重复文件: {Path.GetFileName(file)}");
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"删除文件 {file} 时出错: {ex.Message}");
                        }
                    }
                }

                return duplicatesFound;
            }, cancellationToken);
        }

        private string CalculateFileHash(string filePath, CancellationToken cancellationToken)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    cancellationToken.ThrowIfCancellationRequested();
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogTextBox.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
                LogTextBox.ScrollToEnd();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 窗口关闭时停止所有操作
            cancellationTokenSource?.Cancel();
        }
    }
}