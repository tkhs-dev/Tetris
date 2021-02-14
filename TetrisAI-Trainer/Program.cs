using log4net;
using TetrisAI_Trainer.Source;

namespace TetrisAI_Trainer
{
    public class TetrisAITrainer
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog Logger => logger;

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            Trainer trainer = new Trainer();
            TrainerConfig config = new TrainerConfig();
            if (!config.Load()) config.Save();
            trainer.Start(config);
        }
    }
}