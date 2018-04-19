using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonPublic
{
    /// <summary>
    /// 命令的基类
    /// </summary>
    public class baseCmd : ICommand
    {
        Action act;
        bool isEnable;
        public baseCmd(Action _act = null)
        {
            act = _act;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {

            return true;
        }

        [DebuggerStepThrough]
        public virtual void Execute(object parameter)
        {
            if (act != null)
            {
                act();
            }
        }
    }


    public class baseCmd<T>
    {
        Action<T> act;
        bool isEnable;
        public baseCmd(Action<T> _act = null)
        {
            act = _act;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {

            return true;
        }

        [DebuggerStepThrough]
        public virtual void Execute(object parameter)
        {
            if (act != null)
            {
                if (parameter.GetType() == typeof(T))
                {
                    act((T)parameter);
                }
            }
        }
    }
}
