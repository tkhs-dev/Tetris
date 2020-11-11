using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using log4net;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.IO;
using TetrisAI.Source;
using TetrisAI_Trainer.Source.ga;
using TetrisCore.Source;
using TetrisCore.Source.Config;
using static TetrisAI.Source.Evaluator;

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
            controller = new AITetrisController(new Evaluator(EvaluationNNParameter.CreateNew()), 2);

            game.SetController(controller);
        }

        public void Start()
        {
            var dirInfo = Directory.CreateDirectory(DateTime.Now.ToString("yyyy-MM-dd-HHmmss"));
            bool training = true;
            while (training)
            {
                var termination = new FitnessStagnationTermination(5);
                GeneticAlgorithm ga = new GeneticAlgorithm(new Population(10, 20, new TetrisChromosome()), new TetrisFitness(), new EliteSelection(), new UniformCrossover(), new UniformMutation());
                ga.Termination = termination;
                var terminationName = ga.Termination.GetType().Name;
                ga.GenerationRan += delegate
                {
                    var bestChromosome = ga.Population.BestChromosome;
                    logger.Info($"Termination: {terminationName}");
                    logger.Info($"Generations: {ga.Population.GenerationsNumber}");
                    logger.Info($"Fitness: {bestChromosome.Fitness}");
                    logger.Info($"Time: {ga.TimeEvolving}");
                    logger.Info($"Speed (gen/sec): {ga.Population.GenerationsNumber / ga.TimeEvolving.TotalSeconds}");

                    if (ga.Population.GenerationsNumber % 1 == 0)
                    {
                        ParameterConfig config = new ParameterConfig() { Parameter = (bestChromosome as TetrisChromosome).GetParameter() };
                        config.Save(dirInfo.FullName,ga.Population.GenerationsNumber.ToString());
                    }
                };
                ga.TerminationReached += delegate
                {
                    var param = (ga.Population.BestChromosome as TetrisChromosome).GetParameter();
                    logger.Info("Training Finished!!");
                    logger.Info("-----Result-----");
                    logger.Info(param.InputLayerWeight);
                    logger.Info(param.MiddleLayerWeight);
                    logger.Info(param.OutputLayerWeight);
                    training = false;
                };
                ga.Start();
            }
            //game.Start();
            //game.WhenGameEnd().Wait();
        }
    }
}