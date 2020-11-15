﻿using log4net;
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
            TrainerConfig config = new TrainerConfig() {
                PopulationSize = 100,
                NumSample = 8, 
                MaxRound = 200, 
                CrossoverProbability = 0.95f, 
                MutationProbability = 1f / 13f
            };
            config.Save();
            trainer.Start(config);
        }
    }
}