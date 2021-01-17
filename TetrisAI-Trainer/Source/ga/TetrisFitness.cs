using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using TetrisAI.Source;
using TetrisCore.Source;
using static TetrisAI.Source.Evaluator;
using TetrisCore.Source.Util;
using Item = TetrisCore.Source.Util.WeightedPool<TetrisCore.Source.BlockUnit>.WeightedItem;
using static TetrisCore.Source.BlockUnit;
using TetrisCore.Source.Extension;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisFitness : IFitness
    {
        public int Sample { get; set; }
        public int MaxRound { get; set; }
        public bool UseVariance { get; set; }

        public TetrisFitness(int sample,int maxRound,bool useVariance)
        {
            Sample = sample;
            MaxRound = maxRound;
        }

        public TetrisFitness() : this(2,200,true)
        {
        }

        public double Evaluate(IChromosome chromosome)
        {
            EvaluationNNParameter parameter = (chromosome as TetrisChromosome).GetParameter();
            Evaluator evaluator = new Evaluator(parameter);
            //S,Zオブジェクトの排出率を二倍にする
            WeightedPool<BlockUnit> pool = new WeightedPool<BlockUnit>(new List<Item>() { 
                new Item(1,Kind.I.GetObject()),
                new Item(1,Kind.J.GetObject()),
                new Item(1,Kind.L.GetObject()),
                new Item(2,Kind.S.GetObject()),
                new Item(2,Kind.Z.GetObject()),
                new Item(1,Kind.T.GetObject()),
                new Item(1,Kind.O.GetObject()),
            });
            List<TetrisGame> games = Enumerable.Range(0, Sample).Select(x => new TetrisGame(TetrisAITrainer.Logger,10,20,pool)).ToList();
            List<GameResult> results = new List<GameResult>();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            ParallelOptions options = new ParallelOptions();
            options.CancellationToken = tokenSource.Token;
            try
            {
                Parallel.ForEach(games, options, x =>
                {
                    x.SetController(new AITetrisController(evaluator, 0));
                    x.TimerEnabled = true;
                    x.TimerSpan = 100;
                    x.MaxRound = MaxRound;
                    x.Start();
                    var result = x.WhenGameEnd().Result;
                    if (result.Score == 0)
                    {
                        //tokenSource.Cancel();
                    }
                    results.Add(result);
                    x.Dispose();
                });
            }
            catch (OperationCanceledException e)
            {
                TetrisAITrainer.Logger.Info(0);
                return 0;
            }
            /*
             games.ForEach(x =>
            {
                x.SetController(new AITetrisController(evaluator, 0));
                x.TimerEnabled = true;
                x.TimerSpan = 100;
                x.MaxRound = MaxRound;
                x.Start();
                results.Add(x.WhenGameEnd().Result);
                x.Dispose();
            });
             */
            double av = results.Average(x => x.Score);
            if (UseVariance)
            {
                double pv = results.Select(x => (double)x.Score).PopulationVariance() / 100;
                double sm = results.Select(x => x.Score).Sum();
                TetrisAITrainer.Logger.Info("pv:" + pv);
                pv /= (sm + 1);
                double bonus = 500 / ((1 / (sm * 0.05 + 50)) * pv * pv + 500);
                av *= bonus;
            }
            TetrisAITrainer.Logger.Info(av);
            return av;
        }
    }
}