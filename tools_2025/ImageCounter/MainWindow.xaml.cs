using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ImageCounter
{
    public partial class MainWindow : Window
    {
        private readonly string[] _imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };
        private CancellationTokenSource _cancellationTokenSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel.Instance;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "选择要统计的文件夹"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ViewModel.Instance.ScanFolderPath = dialog.FileName;
            }
        }
        public CsvRecordService CsvRecorder { get; set; }
        private void CheckBlackImgs_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModel.Instance.ScanFolderPath) && Directory.Exists(ViewModel.Instance.ScanFolderPath))
            {
                // 创建新的CancellationTokenSource
                _cancellationTokenSource = new CancellationTokenSource();

                // 初始化CSV记录器
                if (ViewModel.Instance.RecordInfo && !string.IsNullOrEmpty(ViewModel.Instance.CsvSavePath))
                {
                    CsvRecorder = new CsvRecordService(ViewModel.Instance.CsvSavePath);
                }

                StartCountingImages(ViewModel.Instance.ScanFolderPath, _cancellationTokenSource.Token);
            }
        }
        private async void StartCountingImages(string folderPath, CancellationToken cancellationToken)
        {
            try
            {
                // 重置状态
                ViewModel.Instance.ImageCount = 0;
                ViewModel.Instance.StatusMessage = "正在扫描...";
                ProgressBar.Visibility = Visibility.Visible;
                StatusText.Visibility = Visibility.Visible;
                CheckBlackImgs.IsEnabled = false;
                CancelBtn.IsEnabled = true;  // 启用取消按钮
                ThresholdSlider.IsEnabled = false;

                // 使用Progress来报告进度
                var progress = new Progress<ScanProgress>(report =>
                {
                    ViewModel.Instance.ImageCount = report.CurrentCount;
                    ViewModel.Instance.BlackImageCount = report.BlackImageCount;
                    //ViewModel.Instance.StatusMessage = $"正在扫描: {Path.GetFileName(report.CurrentFile)}";
                    ViewModel.Instance.StatusMessage = $"正在扫描: {report.CurrentFile}";
                });

                // 异步执行计数，传递取消令牌
                int totalCount = 0;
                if (ViewModel.Instance.Multiprocessing)
                {
                    totalCount = await Task.Run(() => CountImagesInFolderMulPro(folderPath, progress, cancellationToken));
                }
                else
                {
                    totalCount = await Task.Run(() => CountImagesInFolderSinglePro(folderPath, progress, cancellationToken));
                }

                // 检查是否被取消
                if (cancellationToken.IsCancellationRequested)
                {
                    ViewModel.Instance.StatusMessage = "操作已取消";
                    return;
                }

                ViewModel.Instance.StatusMessage = $"扫描完成！共找到 {totalCount} 张图片";
                ViewModel.Instance.ImageCount = totalCount;
            }
            catch (OperationCanceledException)
            {
                // 用户取消操作
                ViewModel.Instance.StatusMessage = "操作已取消";
            }
            catch (Exception ex)
            {
                ViewModel.Instance.StatusMessage = $"错误: {ex.Message}";
            }
            finally
            {
                ProgressBar.Visibility = Visibility.Collapsed;
                StatusText.Visibility = Visibility.Collapsed;
                CheckBlackImgs.IsEnabled = true;
                CancelBtn.IsEnabled = false;  // 禁用取消按钮
                ThresholdSlider.IsEnabled = true;

                // 清理资源
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }
        /// <summary>
        /// 多进程会有写入csv数据不完整的问题
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private int CountImagesInFolderMulPro(string folderPath, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
        {
            var totalCount = 0;
            var blackImageCount = 0;
            var lockObject = new object();

            try
            {
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
                int maxDegreeOfParallelism = Math.Max(Environment.ProcessorCount - 2, 1);
                // 使用Partitioner来优化负载均衡
                var partitioner = Partitioner.Create(files, EnumerablePartitionerOptions.NoBuffering);
                Parallel.ForEach(partitioner, new ParallelOptions
                {
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                }, (file, state) =>
                {
                    // 处理前检查
                    cancellationToken.ThrowIfCancellationRequested();
                    if (IsImageFile(file))
                    {
                        bool isBlackImage = FastImageBlackPixelChecker.IsBlackPixelsOver30Percent(file, ViewModel.Instance.BlackThreshold / 100);
                        lock (lockObject)
                        {
                            totalCount++;
                            if (isBlackImage)
                            {
                                blackImageCount++;
                                // 记录黑图信息到CSV
                                if (ViewModel.Instance.RecordInfo && CsvRecorder != null)
                                {
                                    try
                                    {
                                        CsvRecorder.RecordImageInfo(file, true, ViewModel.Instance.BlackThreshold);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine($"记录CSV失败: {ex.Message}");
                                    }
                                }
                                if (ViewModel.Instance.DelImg)
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch (Exception ex)
                                    {
                                        // 删除失败，记录到日志或CSV
                                        if (ViewModel.Instance.RecordInfo && CsvRecorder != null)
                                        {
                                            CsvRecorder.RecordImageInfo(file, true, ViewModel.Instance.BlackThreshold);
                                            // 可以添加删除失败的状态标记
                                        }
                                    }
                                }
                            }
                            // 批量报告进度，减少UI更新频率
                            //if (totalCount % 10 == 0)
                            {
                                progress?.Report(new ScanProgress
                                {
                                    CurrentCount = totalCount,
                                    BlackImageCount = blackImageCount,
                                    CurrentFile = file
                                });
                            }
                        }
                    }
                });
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException)
            {
                // 处理异常
            }

            return totalCount;
        }
        private int CountImagesInFolderSinglePro(string folderPath, IProgress<ScanProgress> progress, CancellationToken cancellationToken)
        {
            int totalCount = 0;
            int blackImageCount = 0;

            try
            {
                var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    // 检查取消请求
                    cancellationToken.ThrowIfCancellationRequested();

                    if (IsImageFile(file))
                    {
                        totalCount++;
                        bool isBlackImage = FastImageBlackPixelChecker.IsBlackPixelsOver30Percent(file, ViewModel.Instance.BlackThreshold / 100);
                        if (isBlackImage)
                        {

                            blackImageCount++;
                            // 记录黑图信息到CSV
                            if (ViewModel.Instance.RecordInfo && CsvRecorder != null)
                            {
                                try
                                {
                                    CsvRecorder.RecordImageInfo(file, true, ViewModel.Instance.BlackThreshold);
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine($"记录CSV失败: {ex.Message}");
                                }
                            }
                            if (ViewModel.Instance.DelImg)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception ex)
                                {
                                    // 删除失败，记录到日志或CSV
                                    if (ViewModel.Instance.RecordInfo && CsvRecorder != null)
                                    {
                                        CsvRecorder.RecordImageInfo(file, true, ViewModel.Instance.BlackThreshold);
                                        // 可以添加删除失败的状态标记
                                    }
                                }
                            }
 
                            Thread.Sleep(1);
                        }
                    }
                    // 批量报告进度，减少UI更新频率
                    if (totalCount % 3 == 0)
                    {
                        progress?.Report(new ScanProgress
                        {
                            CurrentCount = totalCount,
                            BlackImageCount = blackImageCount,
                            CurrentFile = file
                        });
                    }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or DirectoryNotFoundException)
            {
                // 处理异常
            }

            return totalCount;
        }

        private bool IsImageFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return Array.Exists(_imageExtensions, ext => ext == extension);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            // 请求取消操作
            _cancellationTokenSource?.Cancel();

            // 立即更新UI状态
            CancelBtn.IsEnabled = false;
            CheckBlackImgs.IsEnabled = true;
        }

        private void SelCsvPath(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog
            {
                Title = "选择CSV文件保存位置",
                DefaultFileName = GetDefaultCsvFileName(),
                DefaultExtension = "csv",
                AlwaysAppendDefaultExtension = true
            };

            dialog.Filters.Add(new CommonFileDialogFilter("CSV文件", "*.csv"));

            // 设置初始目录
            dialog.InitialDirectory = GetInitialDirectory();

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ViewModel.Instance.CsvSavePath = dialog.FileName;
            }
        }

        private string GetDefaultCsvFileName()
        {
            // 如果有扫描文件夹，使用文件夹名作为CSV文件名
            if (!string.IsNullOrEmpty(ViewModel.Instance.ScanFolderPath) && Directory.Exists(ViewModel.Instance.ScanFolderPath))
            {
                string folderName = new DirectoryInfo(ViewModel.Instance.ScanFolderPath).Name;
                return $"{folderName}_检查结果.csv";
            }

            return "ImageCheckResult.csv";
        }

        private string GetInitialDirectory()
        {
            // 优先使用已有CSV路径的目录
            if (!string.IsNullOrEmpty(ViewModel.Instance.CsvSavePath))
            {
                string dir = Path.GetDirectoryName(ViewModel.Instance.CsvSavePath);
                if (Directory.Exists(dir))
                    return dir;
            }

            // 其次使用扫描文件夹
            if (!string.IsNullOrEmpty(ViewModel.Instance.ScanFolderPath) && Directory.Exists(ViewModel.Instance.ScanFolderPath))
            {
                return ViewModel.Instance.ScanFolderPath;
            }

            // 默认使用文档文件夹或C盘
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Directory.Exists(documentsPath) ? documentsPath : @"C:\";
        }

        private void CheckRepetImgs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
