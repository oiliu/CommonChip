using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPublic
{
    public class ImageHelper
    {
        public void test(string imgPath)
        {
            string filePath = imgPath;// @"D:\pingpang\15乒乓球研发\01代码\RigourTech.PingPang\App\RigourTech.Tennisball.App\bin\x64\Release\TempDiagram\11.png";
            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, bytes.Length);
                file.Close();
                // 把 byte[] 转换成 Stream 
                Stream stream = new MemoryStream(bytes);
                System.Drawing.Bitmap b = new System.Drawing.Bitmap(stream);
                b = KiRotate(b, 180);
                b.Save(filePath);
            }
        }
        public System.Drawing.Bitmap KiRotate(System.Drawing.Bitmap bmp, float angle)
        {
            int w = bmp.Width + 2;
            int h = bmp.Height + 2;

            System.Drawing.Imaging.PixelFormat pf;

            pf = bmp.PixelFormat;

            System.Drawing.Bitmap tmp = new System.Drawing.Bitmap(w, h, pf);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmp);
            //g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddRectangle(new System.Drawing.RectangleF(0f, 0f, w, h));
            System.Drawing.Drawing2D.Matrix mtrx = new System.Drawing.Drawing2D.Matrix();
            mtrx.Rotate(angle);
            System.Drawing.RectangleF rct = path.GetBounds(mtrx);

            System.Drawing.Bitmap dst = new System.Drawing.Bitmap((int)rct.Width, (int)rct.Height, pf);
            g = System.Drawing.Graphics.FromImage(dst);
            //g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tmp, 0, 0);
            g.Dispose();

            tmp.Dispose();

            return dst;
        }
    }
}
