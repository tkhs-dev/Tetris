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
            const int sample = 2;
            EvaluationNNParameter parameter = (chromosome as TetrisChromosome).GetParameter();
            Evaluator evaluator = new Evaluator(parameter);
            List<TetrisGame> games = Enumerable.Range(0, sample).Select(x => new TetrisGame(TetrisAITrainer.Logger)).ToList();
            List<GameResult> results = new List<GameResult>();
            /*
            Parallel.ForEach(games, x => 
            {
                x.SetController(new AITetrisController(evaluator,0));
                x.MaxRound = 200;
                x.Start();
                results.Add(x.WhenGameEnd().Result);
            });*/
            games.ForEach(x => {
                x.SetController(new AITetrisController(evaluator, 0));
                x.TimerEnabled = true;
                x.TimerSpan = 10;
                x.MaxRound = 200;
                x.Start();
                results.Add(x.WhenGameEnd().Result);
                x.Dispose();
            });
            double av = results.Average(x => x.Score+x.Round*x.Round*100);
            TetrisAITrainer.Logger.Info(av);
            return av;
        }
    }
}
