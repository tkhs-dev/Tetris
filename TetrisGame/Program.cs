using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TetrisCore.Source;
using TetrisCore.Source.Extension;
using static TetrisCore.Source.BlockObject;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
namespace TetrisGame
{
    public class Program
    {
        static void Main(string[] args)
        {
            GetLogger().Info("Start up TetrisGame...");
        }
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLogger()
        {
            return logger;
        }
    }
}
