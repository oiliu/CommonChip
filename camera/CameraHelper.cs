using CommonPublic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPublic
{
    public class CameraHelper
    {
        public static CameraHelper instance;
        static CameraHelper()
        {
            if (instance == null)
            {
                instance = new CameraHelper();
            }
        }
        string iniFilePath = Path.Combine(Environment.CurrentDirectory, @"Config.ini"); //Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"RigourTech\ScreenConfig\Config");
        
        public List<CameraClass> GetCameraList()
        {
            List<CameraClass> CameraList = new List<CameraClass>();
            //一号相机:192.168.6.1:8080, ---- 四号相机:192.168.6.1:8080
            string StrCameraList = IniFileHelper.GetValue("Camera", "List", iniFilePath);
            string[] strs = StrCameraList.Split(',');
            for (int i = 0; i < strs.Length; i++)
            {
                string[] StrCameraInfo = strs[i].Split(':');
                CameraClass c = new CameraClass();
                c.id = Guid.NewGuid().ToString();
                c.Name = StrCameraInfo[0];
                c.ip = StrCameraInfo[1];
                c.port = Convert.ToInt32(StrCameraInfo[2]);
                c.remark = "";
                CameraList.Add(c);
            }
            return CameraList;
        }
    }
}
