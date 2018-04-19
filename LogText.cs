using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//using System.Windows;

namespace BroadCommon.Common
{
    /// <summary>
    /// 日志事件委托
    /// </summary>
    /// <param name="Level">日志级别</param>
    /// <param name="ErrorDescripton">日志描述</param>
    public delegate void LogHandler(LogTextLevel Level, string srcName, string logDescripton);

    /// <summary>
    /// 日志文本的级别
    /// </summary>
    public enum LogTextLevel
    {
        /// <summary>
        /// 最详细的调试信息
        /// </summary>
        DebugDetail = 1,

        /// <summary>
        /// 普通级别的调试信息
        /// </summary>
        Debug,

        /// <summary>
        /// 正常运行时输出的信息
        /// </summary>
        Info,

        /// <summary>
        /// 警告信息
        /// </summary>
        Warning,

        /// <summary>
        /// 错误信息
        /// </summary>
        Error,

        /// <summary>
        /// 严重错误信息
        /// </summary>
        Fatal,
    };

    /// <summary>
    /// 写入日志文本的类
    /// </summary>
    public class LogText
    {
        /// <summary>
        /// 日志的内容
        /// </summary>
        public class MemLog
        {
            public DateTime LogTime { get; set; }

            public String LogTimeDisplay
            {
                get
                {
                    return LogTime.ToString("yyyy-MM-dd hh:mm:ss.fff");
                }
            }
            public LogTextLevel LogLevel { get; set; }
            public string LogSrcName { get; set; }
            public string LogDesc { get; set; }
        }

        // 日志文件路径
        private static string _logPath = "";

        // 带更新通知的内存日志集合，多线程写同步使用Application的队列维护。
        //  public static   ObservableCollection<MemLog> _logMem = new ObservableCollection<MemLog>();

        public static ConcurrentQueue<MemLog> _logMem = new ConcurrentQueue<MemLog>();

        // 写日志文件线程
        //private static Thread _logSaveThread = null;

        //退出线程
        static CancellationTokenSource cts = new CancellationTokenSource();

        // 写日志文件挂起信号
        private static AutoResetEvent _logSaveWait = new AutoResetEvent(false);

        //内存中最大存差异的日志条数，当达到最大条数后，会按照时间自动删除内存中的记录,读取配置文件
        static int maxLogCount = 3000;

        /// <summary>
        /// 使能或者关闭记录
        /// </summary>
        private static bool _isLogging;
        public static bool IsLogging
        {
            get
            {
                return _isLogging;
            }
            set
            {
                _isLogging = value;
                ModifyConfigFile("IsLogging", _isLogging.ToString());
            }
        }

        /// <summary>
        /// 是否输出到日志文件。
        /// </summary>
        private static bool _isPrintToFile;
        public static bool IsPrintToFile
        {
            get
            {
                return _isPrintToFile;
            }
            set
            {
                _isPrintToFile = value;
                ModifyConfigFile("IsPrintToFile", _isPrintToFile.ToString());
            }
        }

        /// <summary>
        /// 是否输出到输出窗口。
        /// </summary>
        private static bool _isPrintToOutput;
        public static bool IsPrintToOutput
        {
            get
            {
                return _isPrintToOutput;
            }
            set
            {
                _isPrintToOutput = value;
                ModifyConfigFile("IsPrintToOutput", _isPrintToOutput.ToString());
            }
        }

        /// <summary>
        /// 记录到内存中的日志级别
        /// </summary>
        private static LogTextLevel _logLevel;
        public static LogTextLevel LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
                ModifyConfigFile("LogLevel", _logLevel.ToString());
            }
        }

        public static void Close()
        {
            _logSaveWait.Set();
            cts.Cancel();
        }

        /// <summary>
        /// 静态构造函数, 在第一次访问时调用
        /// </summary>
        static LogText()
        {
            _logPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Logs";

            try
            {
                // 读取配置文件设置
                Configuration AppCfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                // 获取自定义的logSettings配置区
                AppSettingsSection logSec = AppCfg.Sections["logSettings"] as AppSettingsSection;
                if (logSec == null)
                {
                    logSec = new AppSettingsSection();
                    AppCfg.Sections.Add("logSettings", logSec);
                    AppCfg.Save(ConfigurationSaveMode.Modified);
                }

                // 读是否记录日志配置
                if (logSec.Settings.AllKeys.Contains("IsLogging"))
                {
                    string CfgValue = logSec.Settings["IsLogging"].Value;
                    _isLogging = Convert.ToBoolean(CfgValue);
                }
                else
                {
                    IsLogging = true;
                }

                // 读是否输出到Output窗口配置
                if (logSec.Settings.AllKeys.Contains("IsPrintToOutput"))
                {
                    string CfgValue = logSec.Settings["IsPrintToOutput"].Value;
                    _isPrintToOutput = Convert.ToBoolean(CfgValue);
                }
                else
                {
                    IsPrintToOutput = true;
                }

                // 读是否输出到日志文件配置
                if (logSec.Settings.AllKeys.Contains("IsPrintToFile"))
                {
                    string CfgValue = logSec.Settings["IsPrintToFile"].Value;
                    _isPrintToFile = Convert.ToBoolean(CfgValue);
                }
                else
                {
                    IsPrintToFile = true;
                }

                //从配置文件中读取最大的缓存日志的记录条数
                if (logSec.Settings.AllKeys.Contains("maxLogCount"))
                {
                    string maxLogCountStr = logSec.Settings["maxLogCount"].Value;
                    if (!string.IsNullOrEmpty(maxLogCountStr))
                    {
                        maxLogCount = int.Parse(maxLogCountStr);
                    }

                }

                // 读日志记录级别配置
                if (logSec.Settings.AllKeys.Contains("LogLevel"))
                {
                    string CfgValue = logSec.Settings["LogLevel"].Value;
                    _logLevel = (LogTextLevel)Enum.Parse(typeof(LogTextLevel), CfgValue, true);
                }
                else
                {
                    //LogLevel = LogTextLevel.Info;
                    LogLevel = LogTextLevel.Debug;
                }
            }
            catch (System.Exception)
            {
                _isLogging = true;
                _isPrintToOutput = false;
                _isPrintToFile = true;
                _logLevel = LogTextLevel.Info;
            }

            // 创建日志文件
            try
            {
                DirectoryInfo directory = new DirectoryInfo(_logPath);
                if (!directory.Exists)
                {
                    directory.Create();
                    //FileOperate.addpathPower(directory.FullName);
                }
                _logPath = _logPath + "\\Log_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";

                using (StreamWriter logFile = File.CreateText(_logPath))
                {
                    logFile.Close();
                }

                Task.Factory.StartNew(WriteThread, cts.Token);

                //_logSaveThread = new Thread(WriteThread);
                //_logSaveThread.Name = "LogWriteThread";
                //_logSaveThread.IsBackground = true;
                //_logSaveThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // 如果创建目录失败, 例如没有目录权限, 忽略
            }
        }

        /// <summary>
        /// 修改log相关的配置文件
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="KeyValue"></param>
        private static void ModifyConfigFile(string KeyName, string KeyValue)
        {
            Configuration AppCfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection logSec = AppCfg.Sections["logSettings"] as AppSettingsSection;
            if (logSec != null)
            {
                if (logSec.Settings.AllKeys.Contains(KeyName))
                {
                    logSec.Settings[KeyName].Value = KeyValue;
                }
                else
                {
                    logSec.Settings.Add(KeyName, KeyValue);
                }
                AppCfg.Save(ConfigurationSaveMode.Minimal);
            }
        }

        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="srcName">日志源</param>
        /// <param name="logDescripton">日志描述</param>
        public static void OnLogEvent(LogTextLevel level, string srcName, string logDescripton)
        {
            // 没有使能日志, 快速返回
            if (!_isLogging)
            {
                return;
            }

            //去掉空格和数字
            //   string functionName = Regex.Replace(srcName, @"[\s<\d>]", string.Empty);

            DateTime NowTime = DateTime.Now;
            // 判断级别，是否将该条日志记录到日志文件中
            if ((int)LogLevel <= (int)level)
            {
                MemLog NewLog = new MemLog();
                NewLog.LogTime = NowTime;
                NewLog.LogLevel = level;
                NewLog.LogSrcName = srcName;
                NewLog.LogDesc = logDescripton;

                //添加重复日志的过滤
                _logMem.Enqueue(NewLog);

                // 激活写日志文件线程
                _logSaveWait.Set();
            }
        }

        /// <summary>
        /// 内存日志最大条数上线检测
        /// </summary>
        //private static void CheckOverMaxLogCount()
        //{

        //    if (!(_logMem.Count > 0))
        //    {
        //        return;
        //    }

        //    if (_logMem.Count > maxLogCount)
        //    {
        //        //一次清空500条
        //        while (_logMem.Count > (maxLogCount-500))
        //        {
        //            //在首减少日志
        //            _logMem.RemoveAt(0);

        //            if (_logSaveIndex > 0)
        //            {
        //                _logSaveIndex--;
        //            }
        //            else
        //            {
        //                _logSaveIndex = _logMem.Count;
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// 日志文件写线程，没有可写的日志文件时会自己挂起
        /// </summary>
        private static void WriteThread()
        {

            try
            {


                while (true)
                {


                    cts.Token.ThrowIfCancellationRequested();

                    // 当前线程在日志队列写完后自动挂起
                    if (_logMem.IsEmpty && !cts.IsCancellationRequested)
                    {
                        _logSaveWait.WaitOne();
                    }
                    else
                    {
                        try
                        {
                            MemLog LogToSave = null;
                            if (_logMem.TryDequeue(out LogToSave))
                            {

                                FileInfo fileInfo = new FileInfo(_logPath);

                                // 没有日志文件
                                if (!string.IsNullOrEmpty(_logPath) && IsPrintToFile)
                                {
                                    using (StreamWriter logFile = File.AppendText(_logPath))
                                    {
                                        logFile.Write(LogToSave.LogTime.ToString("[yyyy-MM-dd HH:mm:ss.fff]"));
                                        logFile.Write("[" + LogToSave.LogLevel.ToString() + "]");
                                        logFile.Write("[" + LogToSave.LogSrcName + "]");
                                        logFile.Write(LogToSave.LogDesc + "\r\n");
                                        logFile.Close();
                                    }
                                }

                                //// 显示到输出框
                                if (IsPrintToOutput)
                                {
                                    Console.WriteLine("[{0}]<{1}>{2}", LogToSave.LogLevel.ToString(), LogToSave.LogSrcName, LogToSave.LogDesc);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("LogText.WriteThread() 失败 : {0}", e.Message);
                        }
                    }

                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("日志线程正常退出");
            }

        }

        /// <summary>
        /// 记录错误的日志
        /// </summary>
        /// <param name="source">记录日志的来源</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Error(string source, string format, params Object[] args)
        {
            if (!_isLogging)
            {
                // 没有使能日志, 快速返回
                return;
            }

            OnLogEvent(LogTextLevel.Error, source, (args != null && args.Length != 0) ? string.Format(format, args) : format);
        }

        /// <summary>
        /// 记录警告的日志
        /// </summary>
        /// <param name="source">记录日志的来源</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Warning(string source, string format, params Object[] args)
        {
            if (!_isLogging)
            {
                // 没有使能日志, 快速返回
                return;
            }

            OnLogEvent(LogTextLevel.Warning, source, string.Format(format, args));
        }

        static string lastInfoSource = string.Empty;
        static string lastInfoDiscription = string.Empty;

        /// <summary>
        /// 记录正常输出的日志
        /// </summary>
        /// <param name="source">记录日志的来源</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Info(string source, string format, params Object[] args)
        {
            if (!_isLogging)
            {
                // 没有使能日志, 快速返回
                return;
            }

            string tempdiscription = args == null || args.Length == 0 ? format : string.Format(format, args);
            //添加重复消息过滤
            if (!lastInfoSource.Equals(source) || !lastInfoDiscription.Equals(tempdiscription))
            {
                lastInfoSource = source;
                lastInfoDiscription = tempdiscription;

                OnLogEvent(LogTextLevel.Info, lastInfoSource, lastInfoDiscription);
            }
        }

        /// <summary>
        /// 记录调试输出的日志
        /// </summary>
        /// <param name="source">记录日志的来源</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Debug(string source, string format, params Object[] args)
        {
            if (!_isLogging)
            {
                // 没有使能日志, 快速返回
                return;
            }

            OnLogEvent(LogTextLevel.Debug, source, string.Format(format, args));
        }

        /// <summary>
        /// 记录最详细的调试输出的日志
        /// </summary>
        /// <param name="source">记录日志的来源</param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void DebugDetail(string source, string format, params Object[] args)
        {
            if (!_isLogging)
            {
                // 没有使能日志, 快速返回
                return;
            }

            OnLogEvent(LogTextLevel.DebugDetail, source, string.Format(format, args));
        }

        /// <summary>
        /// 清空当前系统内容中的日志
        /// </summary>
        //public static void Clear()
        //{
        //    showLog.Clear();
        //}



        //static ObservableCollection<string> functionDic = new ObservableCollection<string>();
        ///// <summary>
        ///// 日志中所有函数名的列表
        ///// </summary>
        //public static ObservableCollection<string> FunctionDic
        //{
        //    get { return functionDic; }
        //    set { functionDic = value; }
        //}




    }

    /// <summary>
    /// 函数控制信息
    /// </summary>
    public class FunctionInfo
    {
        public string FunctionName { get; set; }
        public bool IsIgnor { get; set; }
    }
}
