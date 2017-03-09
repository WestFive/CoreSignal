using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data.Common
{
    public class Log
    {
        /// <summary>
        /// 路径
        /// </summary>
        private string filePath;

        /// <summary>
        /// 路径字段
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filepath"></param>
        public Log(string filepath)
        {
            this.FilePath = filepath;
        }

        /// <summary>
        /// 默认路径是文件开始路径
        /// </summary>

        /// <summary>
        /// 追加到日志的方法。存在文件则追加，没有则创建并追加信息。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string AddLogText(string message)
        {
            HasLogDirectory();//先判断在路径文件夹下是否有LOG文件夹。
            try
            {
                if (!File.Exists(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt")) //判断是否存在日志文件
                {

                    using (FileStream fs1 = new FileStream(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt", FileMode.Create, FileAccess.Write))
                    {//创建写入文件 
                        using (StreamWriter sw = new StreamWriter(fs1))
                        {
                            sw.WriteLine(DateTime.Now.ToString() + "  " + message);//开始写入值
                            return "创建日志成功";
                        }
                    }
                }
                else //存在则追加
                {
                    using (FileStream fs = new FileStream(FilePath + @"\LOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt", FileMode.Open, FileAccess.Write))
                    {
                        using (StreamWriter sr = new StreamWriter(fs))
                        {

                            sr.BaseStream.Seek(0, SeekOrigin.End);
                            sr.WriteLine(DateTime.Now.ToString() + "  " + message);//开始写入值
                            return "追加到日志成功";
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                return "追加到日志失败";
            }

            // return "追加到日志成功";
        }





        /// <summary>
        /// 异常信息追加到日志
        /// </summary>
        /// <param name="name">异常模块名</param>
        /// <param name="ex">异常</param>
        public string AddErrorText(string name, Exception ex)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【模块名称】：" + name);
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }

            sb.AppendLine("***************************************************************");

            string message = sb.ToString();
            HasErrorLogDirectory();//先判断在路径文件夹下是否有LOG文件夹。
            try
            {
                if (!File.Exists(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt")) //判断是否存在日志文件
                {
                    FileStream fs1 = new FileStream(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt", FileMode.Create, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine(DateTime.Now.ToString() + "  " + message);//开始写入值
                    return "创建日志成功";
                }
                else //存在则追加
                {
                    FileStream fs = new FileStream(FilePath + @"\ERRORLOG\" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".txt", FileMode.Open, FileAccess.Write);
                    StreamWriter sr = new StreamWriter(fs);
                    sr.BaseStream.Seek(0, SeekOrigin.End);
                    sr.WriteLine(DateTime.Now.ToString() + "  " + message);//开始写入值
                    return "追加到日志成功";
                }
            }
            catch
            {
                return "追加到日志失败";
            }






        }

        private void HasErrorLogDirectory()
        {
            if (Directory.Exists(FilePath + @"\ERRORLOG"))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(FilePath + @"\ERRORLOG");
                return;
            }
        }
        /// <summary>
        /// 是否有日志文件夹 没有则创建
        /// </summary>
        /// <returns></returns>
        public void HasLogDirectory()
        {
            if (Directory.Exists(FilePath + @"\LOG"))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(FilePath + @"\LOG");
                return;
            }


        }


    }
}
