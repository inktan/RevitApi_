using SkiaSharp;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using SkiaSharp; // 或者 SixLabors.ImageSharpusing System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using SkiaSharp; // 或者 SixLabors.ImageSharp
using System.Diagnostics; // 用于高性能计时

namespace ImgIsBlack
{
    public class ImageProcessor
    {
        public static bool IsBlackPixelPercentageExceeded(string imagePath, double threshold = 0.3)
        {
            // 从文件中加载图片
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            using (var skBitmap = SKBitmap.Decode(stream))
            {
                if (skBitmap == null)
                {
                    // 如果解码失败，返回 false
                    return false;
                }

                // 获取图像总像素数
                int totalPixels = skBitmap.Width * skBitmap.Height;
                if (totalPixels == 0)
                {
                    return false;
                }

                // 获取像素数组，这比 GetPixel() 快得多
                SKColor[] pixels = skBitmap.Pixels;

                // 使用并行处理来提高性能
                int blackPixels = 0;

                Parallel.ForEach(Partitioner.Create(0, pixels.Length), range =>
                {
                    int localBlackPixels = 0;
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        SKColor color = pixels[i];
                        // 判断是否为黑色，这里定义为 RGB 都接近 0
                        if (color.Red <= 1 && color.Green <= 1 && color.Blue <= 1)
                        {
                            localBlackPixels++;
                        }
                    }
                    Interlocked.Add(ref blackPixels, localBlackPixels);
                });

                // 计算黑色像素比例
                double percentage = (double)blackPixels / totalPixels;

                // 返回判断结果
                return percentage > threshold;
            }
        }
    }
    public class Program
    {
        // 定义常量，方便管理
        private const int BatchSize = 100;
        private const string CsvFileName = "deleted_images.csv";
        /// <summary>
        /// 将文件路径列表以追加模式写入 CSV 文件
        /// </summary>
        /// <param name="filePaths">文件路径列表</param>
        private static void AppendToCsv(List<string> filePaths)
        {
            try
            {
                using (var writer = new StreamWriter(CsvFileName, true)) // true 表示追加
                {
                    foreach (var path in filePaths)
                    {
                        writer.WriteLine(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入 CSV 文件时发生错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 动态更新控制台进度条
        /// </summary>
        private static void UpdateProgressBar(int completed, int total, Stopwatch stopwatch)
        {
            // 进度百分比
            double progress = (double)completed / total;

            // 进度条的宽度
            int barWidth = 50;
            int filledWidth = (int)(progress * barWidth);

            // 进度条字符串
            string progressBar = $"[{new string('#', filledWidth)}{new string('-', barWidth - filledWidth)}]";

            // 时间计算
            TimeSpan elapsed = stopwatch.Elapsed;
            string elapsedStr = $"{elapsed:hh\\:mm\\:ss}";

            string remainingStr = "??:??:??";
            if (progress > 0)
            {
                TimeSpan estimatedTotal = TimeSpan.FromSeconds(elapsed.TotalSeconds / progress);
                TimeSpan remaining = estimatedTotal - elapsed;
                remainingStr = $"{remaining:hh\\:mm\\:ss}";
            }

            // 打印到控制台，不换行，并使用 \r 符将光标移到行首
            // \r 类似于 Unix/Linux 中的 `\r` (Carriage Return)
            Console.Write($"\r{progressBar} {progress:P0} ({completed}/{total}) | 已用: {elapsedStr} | 剩余: {remainingStr}");
        }
        public static void Main(string[] args)
        {
            string imageFolder = @"C:\\Users\\wang.tan.GOA\\Pictures";
            //string imageFolder = @"C:\\Users\\wang.tan.GOA\\Pictures\\qwe";

            // 检查路径是否存在
            if (!Directory.Exists(imageFolder))
            {
                Console.WriteLine($"指定文件夹不存在: {imageFolder}");
                return;
            }

            // 存储要写入 CSV 的文件路径
            var deletedFilePaths = new List<string>();
            int counter = 0;

            Console.WriteLine("开始扫描和处理图片...");

            // 获取所有图片文件，包括子文件夹
            var allFiles = Directory.EnumerateFiles(imageFolder, "*.*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                s.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                s.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase)).ToList(); // 将结果立即转为列表，以便获取总数

            int totalFiles = allFiles.Count;
            if (totalFiles == 0)
            {
                Console.WriteLine("未找到图片文件。");
                return;
            }

            Console.WriteLine($"共找到 {totalFiles} 张图片。开始处理...");

            // 用于计时和进度条的变量
            int completedCount = 0;
            var stopwatch = Stopwatch.StartNew();

            // 遍历每个图片文件
            foreach (var imageFile in allFiles)
            {

                // 更新进度
                completedCount++;
                UpdateProgressBar(completedCount, totalFiles, stopwatch);


                // 使用我们之前的判断方法
                if (ImageProcessor.IsBlackPixelPercentageExceeded(imageFile))
                {
                    Console.WriteLine($"    > {Path.GetFileName(imageFile)} 黑色像素超过30%。正在删除...");

                    try
                    {
                        //File.Delete(imageFile);
                        Console.WriteLine("    > 删除成功。");

                        // 将完整路径添加到列表中
                        deletedFilePaths.Add(imageFile);
                        counter++;

                        // 达到批量大小，立即写入文件
                        if (deletedFilePaths.Count >= BatchSize)
                        {
                            Console.WriteLine($"    > 达到 {BatchSize} 个文件，正在更新 {CsvFileName}...");
                            AppendToCsv(deletedFilePaths);
                            deletedFilePaths.Clear(); // 清空列表
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"    > 删除文件失败 {imageFile}: {ex.Message}");
                    }
                }
                else
                {
                    // Console.WriteLine($"    > {Path.GetFileName(imageFile)} 黑色像素未超过30%。");
                }
            }

            // 循环结束后，将剩余的路径写入 CSV
            if (deletedFilePaths.Count > 0)
            {
                Console.WriteLine($"    > 正在写入剩余的 {deletedFilePaths.Count} 个文件到 {CsvFileName}...");
                AppendToCsv(deletedFilePaths);
            }

            Console.WriteLine("\n所有图片处理完毕。");
        }
    }
}