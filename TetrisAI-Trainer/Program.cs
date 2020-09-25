using log4net;
using System;
using System.Threading.Tasks;
using TetrisAI_Trainer.Source;
using TetrisCore.Source;

namespace TetrisAI_Trainer
{
    public class TetrisAITrainer
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog Logger => logger;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            Trainer trainer = new Trainer();
            trainer.Start();
        }
    }
}
