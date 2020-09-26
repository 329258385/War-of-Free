using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solarmax
{
    public class LoggerSystem : Singleton<LoggerSystem>, Lifecycle
    {
        public enum LogLevel
        {
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4,
            FATAL = 5,
            ALWAYS = 6,
        }

		private static string[] LOGTITLE = {"UNKNOW", "DEBUG", "INFO", "WARN", "ERROR", "FATAL", "ALWAYS"};

        private bool mConsoleLogMode;
        private Logger mConsoleLogger;
        private LogLevel mConsoleLogLevel;
        private bool mFileLogMode;
        private LogLevel mFileLogLevel;

        public LoggerSystem()
        {
            mConsoleLogMode = true;
            mConsoleLogger = null;
			mConsoleLogLevel = LogLevel.DEBUG;

			mFileLogMode = true;
			mFileLogLevel = LogLevel.DEBUG;
        }

        public bool Init()
		{
			// 读取配置
            string val = string.Empty;
            if (ConfigSystem.Instance.TryGetConfig("consolelogmode", out val))
            {
                SetConsoleLogMode(Converter.ConvertBool(val));
            }
            if (ConfigSystem.Instance.TryGetConfig("consoleloglevel", out val))
            {
                SetConsoleLogLevel(Converter.ConvertNumber<int>(val));
            }
            if (ConfigSystem.Instance.TryGetConfig("filelogmode", out val))
            {
                SetFileLogMode(Converter.ConvertBool(val));
            }
            if (ConfigSystem.Instance.TryGetConfig("fileloglevel", out val))
            {
                SetFileLogLevel(Converter.ConvertNumber<int>(val));
            }
            
            return true;
        }

        public void Tick(float interval)
        {

        }

        public void Destroy()
		{
	
        }

		public void Debug(string message, params object[] args)
        {
			if (args.Length > 0)
				message = string.Format (message, args);
			
            WriteLog(LogLevel.DEBUG, message);
        }

		public void Info(string message, params object[] args)
		{
			if (args.Length > 0)
				message = string.Format (message, args);
			
            WriteLog(LogLevel.INFO, message);
        }

		public void Warn(string message, params object[] args)
		{
			if (args.Length > 0)
				message = string.Format (message, args);
			
            WriteLog(LogLevel.WARN, message);
        }

		public void Error(string message, params object[] args)
		{
			if (args.Length > 0)
				message = string.Format (message, args);
			
            WriteLog(LogLevel.ERROR, message);
            WriteLog(LogLevel.ERROR, UtilTools.GetCallStack());
        }

		public void Fatal(string message, params object[] args)
		{
			if (args.Length > 0)
				message = string.Format (message, args);
			
            WriteLog(LogLevel.FATAL, message);
            WriteLog(LogLevel.FATAL, UtilTools.GetCallStack());
        }
        private void WriteLog(LogLevel level, string message)
        {
			message = string.Format("[{0}], [{1}],\t\t [frame:{2}]", LOGTITLE[(int)level], message, TimeSystem.Instance.GetFrame());
            
            // console log
            ConsoleLog(level, message);

            // file log
            FileLog(level, message);
        }

        private void ConsoleLog(LogLevel level, string message)
        {
            if (mConsoleLogMode && mConsoleLogLevel <= level && null != mConsoleLogger)
            {
                mConsoleLogger.Write(message);
            }
        }

        private void FileLog(LogLevel level, string message)
        {
  
        }

        /**
         * 设置console log的方法
         * */
        public void SetConsoleLogger(Logger logger)
        {
            mConsoleLogger = logger;
        }
        private void SetConsoleLogMode(bool status)
        {
            mConsoleLogMode = status;
        }
        private void SetConsoleLogLevel(int level)
        {
            mConsoleLogLevel = (LogLevel)level;
        }

        /**
         * 设置file log的方法
         * */
        private void SetFileLogger(Logger logger)
        {
           
        }
        private void SetFileLogMode(bool status)
        {
            mFileLogMode = status;
        }
        private void SetFileLogLevel(int level)
        {
            mFileLogLevel = (LogLevel)level;
        }
    }
}
