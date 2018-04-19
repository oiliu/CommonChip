using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonPublic
{
    /// <summary>
    /// 用于获取位图像素的类
    /// </summary>
    public class BitmapPixelHelper
    {
        /// <summary>
        /// 位图宽度
        /// </summary>
        public int Width { get; protected set; }
        /// <summary>
        /// 位图高度
        /// </summary>
        public int Height { get; protected set; }
        /// <summary>
        /// 像素
        /// </summary>
        public Color[][] Pixels { get; protected set; }

        /// <summary>
        /// 根据指定的位图生成BitmapPixelHelper类的新实例。
        /// </summary>
        /// <param name="bitmap">指定的位图</param>
        public BitmapPixelHelper(BitmapSource bitmap)
        {
            FormatConvertedBitmap newBitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgra32, BitmapPalettes.WebPaletteTransparent, 0);
            const int bytesPerPixel = 4;
            Height = newBitmap.PixelHeight;
            Width = newBitmap.PixelWidth;
            byte[] data = new byte[Height * Width * bytesPerPixel];
            newBitmap.CopyPixels(data, Width * bytesPerPixel, 0);

            Pixels = new Color[Height][];
            for (int i = 0; i < Height; ++i)
            {
                Pixels[i] = new Color[Width];
                for (int j = 0; j < Width; ++j)
                {
                    Pixels[i][j] = Color.FromArgb(
                        data[(i * Width + j) * bytesPerPixel + 3],
                        data[(i * Width + j) * bytesPerPixel + 2],
                        data[(i * Width + j) * bytesPerPixel + 1],
                        data[(i * Width + j) * bytesPerPixel + 0]);
                }
            }
        }

        /// <summary>
        /// 获取图片的平均色
        /// </summary>
        public Color GetAverageColor()
        {
            int a = 0, r = 0, g = 0, b = 0;
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    a += Pixels[i][j].A;
                    r += Pixels[i][j].R;
                    g += Pixels[i][j].G;
                    b += Pixels[i][j].B;
                }
            }
            a = a / Height / Width;
            r = r / Height / Width;
            g = g / Height / Width;
            b = b / Height / Width;
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
}
