using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
namespace DaemonSPC
{
    class Program
    {

        public static readonly NLog.Logger logger = NLog.LogManager.GetLogger("logger");


        static void Main(string[] args)
        {
            logger.Info("Initialisation... ");
            string FolderToWatch = ConfigurationManager.AppSettings.Get("FolderToWatch");
            logger.Info("Folder to watch : " + FolderToWatch);
            var watcher = new FileSystemWatcher(@"" + FolderToWatch)
            {
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size
            };

            watcher.Created += OnCreated;
            logger.Info("Creation of event : OK");

            string Filter = ConfigurationManager.AppSettings.Get("Filter");
            watcher.Filter = Filter;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            while (1==1)
            {

            }
        }
        
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            logger.Info("----   Event triggered   ----");
			string exec = ConfigurationManager.AppSettings.Get("Process");
			Process.Start(exec);
			logger.Info("Process end : OK ");
            
        }
    }
}

