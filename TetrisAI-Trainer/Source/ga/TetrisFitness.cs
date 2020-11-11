using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisAI.Source;
using TetrisCore.Source;
using static TetrisAI.Source.Evaluator;

namespace TetrisAI_Trainer.Source.ga
{
    class TetrisFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            const int sample = 3;
            EvaluationNNParameter parameter = (chromosome as TetrisChromosome).GetParameter();
            Evaluator evaluator = new Evaluator(parameter);
            List<TetrisGame> games = Enumerable.Range(0, sample).Select(x => new TetrisGame(TetrisAITrainer.Logger)).ToList();
            List<Task<GameResult>> tasks = new List<Task<GameResult>>(); 
            Parallel.ForEach(games, async x => 
            {
                x.SetController(new AITetrisController(evaluator));
                x.MaxRound = 200;
                x.Start();
                tasks.Add(x.WhenGameEnd());
            });
            Task<GameResult[]> task = Task.WhenAll(tasks);
            task.Wait();
            GameResult[] results = task.Result;
            double av = results.Average(x => x.Score+x.Round*x.Round*100);
            TetrisAITrainer.Logger.Info(av);
            return av;
        }
    }
}
