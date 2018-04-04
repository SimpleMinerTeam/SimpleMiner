using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCPUMiner
{
    public static class Log
    {
        private static log4net.ILog log;

        public static void SetLogger(log4net.ILog _logger)
        {
            log = _logger;
        }

        public static void InsertDebug(string _message)
        {
            log.Debug(_message);
        }

        public static void InsertInfo(string _message)
        {
            log.Info(_message);
        }

        public static void InsertError(string _message)
        {
            log.Error(_message);
        }

        public static void InsertError(string _message, Exception _exception)
        {
            log.Error(_message, _exception);
        }

        public static void InsertFatal(string _message)
        {
            log.Fatal(_message);
        }
    }
}
