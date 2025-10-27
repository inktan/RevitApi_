using SkiaSharp;
using System;

public class FastImageBlackPixelChecker
{
    public static unsafe bool IsBlackPixelsOver30Percent(string imagePath,double threshold)
    {
        using var bitmap = SKBitmap.Decode(imagePath);
        if (bitmap == null)
        {
            throw new ArgumentException("无法加载图片文件");
        }

        int totalPixels = bitmap.Width * bitmap.Height;
        int blackPixelCount = 0;

        // 锁定位图数据
        var info = bitmap.Info;
        byte* ptr = (byte*)bitmap.GetPixels();

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                // 计算像素位置（假设是RGBA格式）
                int index = (y * bitmap.Width + x) * 4;

                byte r = ptr[index];
                byte g = ptr[index + 1];
                byte b = ptr[index + 2];
                byte a = ptr[index + 3]; // 透明度

                // 判断条件：RGB都为0，且透明度不为0（排除完全透明像素）
                if (r == 0 && g == 0 && b == 0 && a > 0)
                {
                    blackPixelCount++;
                }
            }
        }

        double blackPixelRatio = (double)blackPixelCount / totalPixels;
        return blackPixelRatio > threshold;
    }

    public static unsafe double GetBlackPixelPercentage(string imagePath)
    {
        using var bitmap = SKBitmap.Decode(imagePath);
        if (bitmap == null)
        {
            throw new ArgumentException("无法加载图片文件");
        }

        int totalPixels = bitmap.Width * bitmap.Height;
        int blackPixelCount = 0;

        // 锁定位图数据
        byte* ptr = (byte*)bitmap.GetPixels();

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                int index = (y * bitmap.Width + x) * 4;

                byte r = ptr[index];
                byte g = ptr[index + 1];
                byte b = ptr[index + 2];
                byte a = ptr[index + 3];

                // 排除完全透明像素(0,0,0,0)
                if (r == 0 && g == 0 && b == 0 && a > 0)
                {
                    blackPixelCount++;
                }
            }
        }

        return (double)blackPixelCount / totalPixels * 100;
    }
}

// 对应的非指针版本（简单易懂）
public class ImageBlackPixelChecker
{
    public static bool IsBlackPixelsOver30Percent(string imagePath, double threshold)
    {
        using var bitmap = SKBitmap.Decode(imagePath);
        if (bitmap == null)
        {
            throw new ArgumentException("无法加载图片文件");
        }

        int totalPixels = bitmap.Width * bitmap.Height;
        int blackPixelCount = 0;

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                SKColor pixel = bitmap.GetPixel(x, y);

                // 判断是否为黑色像素：RGB都为0且透明度不为0
                if (pixel.Red == 0 && pixel.Green == 0 && pixel.Blue == 0 && pixel.Alpha > 0)
                {
                    blackPixelCount++;
                }
            }
        }

        double blackPixelRatio = (double)blackPixelCount / totalPixels;
        return blackPixelRatio > threshold;
    }

    public static double GetBlackPixelPercentage(string imagePath)
    {
        using var bitmap = SKBitmap.Decode(imagePath);
        if (bitmap == null)
        {
            throw new ArgumentException("无法加载图片文件");
        }

        int totalPixels = bitmap.Width * bitmap.Height;
        int blackPixelCount = 0;

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                SKColor pixel = bitmap.GetPixel(x, y);

                if (pixel.Red == 0 && pixel.Green == 0 && pixel.Blue == 0 && pixel.Alpha > 0)
                {
                    blackPixelCount++;
                }
            }
        }

        return (double)blackPixelCount / totalPixels * 100;
    }
}







