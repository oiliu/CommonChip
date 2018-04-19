using CommonPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPublic
{
    public class ImgStruct:baseVm
    {
        string imgUrl = "";
        string progressStr = "";

        public string ImgUrl
        {
            get
            {
                return imgUrl;
            }

            set
            {
                imgUrl = value;
                OnProperyChanged("ImgUrl");
            }
        }

        public string ProgressStr
        {
            get
            {
                return progressStr;
            }

            set
            {
                progressStr = value;
                OnProperyChanged("ProgressStr");
            }
        }
    }
}
