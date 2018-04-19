using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CommonPublic
{
    public class TextHelper
    {
        public static TextHelper instance;
        static TextHelper()
        {
            if (instance == null)
            {
                instance = new TextHelper();
            }
        }

        public void ReadText()
        {
            try
            {
                StreamReader sr = new StreamReader("D:\\test.txt", Encoding.Default);
                String line;
                int i = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    i++;
                    SaveImageFromWeb("", "D:/test0/", line);
                    Console.WriteLine(i + "：" + line.ToString());
                    //35个休息5秒
                    if (i % 35 == 0)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string SaveImageFromWeb(
            string fileName,
            string path,
            string imgUrl)
        {
            try
            {
                if (path.Equals(""))
                    throw new Exception("未指定保存文件的路径");
                string imgName = imgUrl.ToString().Substring(imgUrl.ToString().LastIndexOf("/") + 1);
                string defaultType = ".jpg";
                string[] imgTypes = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                string imgType = imgUrl.ToString().Substring(imgUrl.ToString().LastIndexOf("."));
                string imgPath = "";
                foreach (string it in imgTypes)
                {
                    if (imgType.ToLower().Equals(it))
                        break;
                    if (it.Equals(".bmp"))
                        imgType = defaultType;
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUrl);
                request.UserAgent = "Mozilla/6.0 (MSIE 6.0; Windows NT 5.1; Natas.Robot)";
                request.Timeout = 3000;

                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();

                if (response.ContentType.ToLower().StartsWith("image/"))
                {
                    byte[] arrayByte = new byte[1024];
                    int imgLong = (int)response.ContentLength;
                    int l = 0;

                    if (fileName == "")
                        fileName = imgName;

                    FileStream fso = new FileStream(path + fileName + imgType, FileMode.Create);
                    while (l < imgLong)
                    {
                        int i = stream.Read(arrayByte, 0, 1024);
                        fso.Write(arrayByte, 0, i);
                        l += i;
                    }
                    fso.Close();
                    stream.Close();
                    response.Close();
                    imgPath = fileName + imgType;
                    return imgPath;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                Console.WriteLine("ERROR!");
                return "";
            }
        }
        static void backspace(int n)
        {
            for (var i = 0; i < n; ++i)
                Console.Write((char)0x8);
        }
    }
}
