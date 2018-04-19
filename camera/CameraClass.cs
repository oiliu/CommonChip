using CommonPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonPublic
{
    public class CameraClass : baseVm
    {
        public string id { get; set; }
        string name;
        public string ip { get; set; }
        public int port { get; set; }
        public string remark { get; set; }
        private bool alive = false;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnProperyChanged("Name");
            }
        }

        public baseCmd SwitchCmd
        {
            get
            {
                return new baseCmd(SwitchFun);
            }
        }

        public bool Alive
        {
            get
            {
                return alive;
            }

            set
            {
                alive = value;
                OnProperyChanged("Alive");
            }
        }

        void SwitchFun()
        {
            
        }
    }
}
