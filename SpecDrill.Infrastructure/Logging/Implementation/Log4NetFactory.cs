﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using SpecDrill.Infrastructure.Logging.Interfaces;
using L4nHierarchy = log4net.Repository.Hierarchy;
using log4net.Layout;
using log4net.Appender;
using System.IO;

namespace SpecDrill.Infrastructure.Logging.Implementation
{
    internal class Log4NetFactory : ILoggerFactory
    {
        private object SyncRoot = new object();
        private static Dictionary<string, ILogger> loggers = new Dictionary<string,ILogger>();

        //static Log4NetFactory()
        //{
        //    L4nHierarchy.Hierarchy hierarchy = (L4nHierarchy.Hierarchy)LogManager.GetRepository();

        //    hierarchy.Root.RemoveAllAppenders();

        //    var logDirName = "Logs";
        //    var fileName = "sd_log.log";
        //    if (!Directory.Exists(logDirName))
        //        Directory.CreateDirectory(logDirName);

        //    PatternLayout patternLayout = new PatternLayout();
        //    patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
        //    patternLayout.ActivateOptions();

        //    RollingFileAppender roller = new RollingFileAppender();
        //    roller.AppendToFile = false;
        //    roller.File = Path.Combine(logDirName, fileName);
        //    roller.DatePattern = "'_'dd'-'MM'-'yyyy";
        //    roller.Layout = patternLayout;
        //    roller.MaxSizeRollBackups = 5;
        //    roller.AppendToFile = true;
        //    roller.MaximumFileSize = "1GB";
        //    roller.PreserveLogFileNameExtension = true;
        //    roller.RollingStyle = RollingFileAppender.RollingMode.Date;
        //    roller.StaticLogFileName = false;

        //    roller.ActivateOptions();
        //    hierarchy.Root.AddAppender(roller);

        //    MemoryAppender memory = new MemoryAppender();
        //    memory.ActivateOptions();
            
        //    hierarchy.Root.AddAppender(memory);

        //    hierarchy.Root.Level = log4net.Core.Level.Info;
        //    hierarchy.Configured = true;
        //}

        public ILogger Get(string name)
        {
            if (!loggers.ContainsKey(name))
            {
                lock (SyncRoot)
                {
                    if (!loggers.ContainsKey(name))
                    {
                        var log4NetLogger = LogManager.GetLogger(name);
                        loggers[name] = new Log4NetLogger(log4NetLogger);
                    }
                }
            }

            return loggers[name];
        }
    }
}
