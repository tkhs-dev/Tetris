using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TetrisAI.Source;
using TetrisAI_Trainer.Source.ga;
using TetrisCore.Source;

namespace TetrisAI_Trainer.Source
{
    public class Trainer
    {
        private ILog logger;

        private TetrisGame game;
        private AITetrisController controller;

        public Trainer()
        {
            logger = TetrisAITrainer.Logger;
            game = new TetrisGame(logger);
        }

        public void Start(TrainerConfig config)
        {
            var dirInfo = "results/" + DateTime.Now.ToString("yyyy-MM-dd-HHmmss");

            logger.Info(config.ToString());
            logger.Info("Start training...");
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw1.Start();
            var termination = new FitnessStagnationTermination(100);
            GeneticAlgorithm ga = new GeneticAlgorithm(new TplPopulation(config.PopulationSize,config.PopulationSize*2, new TetrisChromosome()), new TetrisFitness(config.NumSample,config.MaxRound), new EliteSelection(), new AlternatingPositionCrossover(), new InsertionMutation());
            ga.Termination = termination;
            var terminationName = ga.Termination.GetType().Name;
            ga.CrossoverProbability = config.CrossoverProbability;
            ga.MutationProbability = config.MutationProbability;
            ga.TaskExecutor = new TplTaskExecutor() { };
            ga.GenerationRan += delegate
            {
                var time = sw2.Elapsed;
                if (ga.Population.GenerationsNumber % 1 == 0)
                {
                    var genResult= GenerationResult.Create(ga,time);
                    genResult.Save(dirInfo, genResult.CreateFileName());
                }
                var bestChromosome = ga.Population.BestChromosome;
                logger.Info($"Termination: {terminationName}");
                logger.Info($"Generations: {ga.Population.GenerationsNumber}");
                logger.Info($"Fitness: {bestChromosome.Fitness}");
                logger.Info($"Time:{time}");
                logger.Info($"EvolvingTime: {ga.TimeEvolving}");
                logger.Info($"Speed (gen/sec): {ga.Population.GenerationsNumber / ga.TimeEvolving.TotalSeconds}");

                sw2.Restart();
                logger.Info($"Start Generation {ga.GenerationsNumber}....");
            };
                ga.TerminationReached += delegate
                {
                    var total_time = sw1.Elapsed;
                    sw1.Stop();
                    sw2.Stop();
                    var param = (ga.Population.BestChromosome as TetrisChromosome).GetParameter();
                    logger.Info("Training Finished!!");
                    logger.Info($"TotalGeneration:{ga.GenerationsNumber}");
                    logger.Info($"Time:{total_time}");
                    logger.Info("-----Result-----");
                    logger.Info(param.MiddleLayerWeight);
                    logger.Info(param.OutputLayerWeight);
                };
                logger.Info($"Start Generation {ga.GenerationsNumber}....");
                sw2.Start();
                ga.Start();
        }
    }
}