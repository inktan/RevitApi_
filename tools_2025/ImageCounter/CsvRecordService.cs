using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public class CsvRecordService
{
    private readonly string _csvFilePath;
    private readonly object _fileLock = new object();
    private readonly bool _recordAllImages; // 是否记录所有图片（不仅是黑图）

    public CsvRecordService(string csvFilePath, bool recordAllImages = false)
    {
        _csvFilePath = csvFilePath;
        _recordAllImages = recordAllImages;
        InitializeCsvFile();
    }

    private void InitializeCsvFile()
    {
        try
        {
            lock (_fileLock)
            {
                string directory = Path.GetDirectoryName(_csvFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 如果文件不存在或为空，写入表头
                //if (!File.Exists(_csvFilePath) || new FileInfo(_csvFilePath).Length == 0)
                //{
                //string header = "序号,文件名,文件路径,是否黑图,黑像素比例,文件大小(字节),检查时间,状态,操作结果\n";
                //File.WriteAllText(_csvFilePath, header, Encoding.UTF8);
                //}

                string header = "序号,文件名,文件路径,黑像素比例,检查时间\n";
                File.WriteAllText(_csvFilePath, header, Encoding.UTF8);

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"初始化CSV文件失败: {ex.Message}");
        }
    }

    public void RecordImageInfo(string filePath, bool isBlackImage, double blackPixelPercentage = 0, string operationResult = "")
    {
        // 如果只记录黑图，且当前不是黑图，则直接返回
        if (!_recordAllImages && !isBlackImage)
            return;

        try
        {
            var fileInfo = new FileInfo(filePath);
            var record = new ImageRecord
            {
                FileName = Path.GetFileName(filePath),
                FilePath = filePath,
                //IsBlackImage = isBlackImage,
                BlackPixelPercentage = blackPixelPercentage,
                //FileSize = fileInfo.Exists ? fileInfo.Length : 0,
                CheckTime = DateTime.Now,
                //Status = isBlackImage ? "黑图" : "正常",
                //OperationResult = operationResult
            };

            RecordToCsv(record);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"记录图像信息失败: {ex.Message}");
        }
    }

    private void RecordToCsv(ImageRecord record)
    {
        try
        {
            lock (_fileLock)
            {
                // 获取当前行号
                int lineNumber = GetCurrentLineCount() + 1;

                //string csvLine = $"{lineNumber},\"{EscapeCsvField(record.FileName)}\",\"{EscapeCsvField(record.FilePath)}\",{record.IsBlackImage},{record.BlackPixelPercentage:F2},{record.FileSize},\"{record.CheckTime:yyyy-MM-dd HH:mm:ss}\",{record.Status},\"{EscapeCsvField(record.OperationResult)}\"\n";
                string csvLine = $"{lineNumber},\"{EscapeCsvField(record.FileName)}\",\"{EscapeCsvField(record.FilePath)}\",{record.BlackPixelPercentage:F2},\"{record.CheckTime:yyyy-MM-dd HH:mm:ss}\",\n";

                File.AppendAllText(_csvFilePath, csvLine, Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"写入CSV失败: {ex.Message}");
        }
    }

    private int GetCurrentLineCount()
    {
        if (!File.Exists(_csvFilePath))
            return 0;

        try
        {
            // 读取文件行数（减去除去表头）
            var lines = File.ReadAllLines(_csvFilePath);
            return Math.Max(0, lines.Length - 1);
        }
        catch
        {
            return 0;
        }
    }

    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
            return "";

        // 如果字段包含逗号、引号或换行，需要用引号包围并转义内部引号
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return field.Replace("\"", "\"\"");
        }

        return field;
    }

    private class ImageRecord
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsBlackImage { get; set; }
        public double BlackPixelPercentage { get; set; }
        public long FileSize { get; set; }
        public DateTime CheckTime { get; set; }
        public string Status { get; set; }
        public string OperationResult { get; set; }
    }
}