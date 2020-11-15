﻿using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TetrisAI.Source;
using TetrisCore.Source;
using static TetrisAI.Source.Evaluator;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisFitness : IFitness
    {
        public int Sample { get; set; }
        public int MaxRound { get; set; }

        public TetrisFitness(int sample,int maxRound)
        {
            Sample = sample;
            MaxRound = maxRound;
        }

        public TetrisFitness() : this(2,200)
        {
        }

        public double Evaluate(IChromosome chromosome)
        {
            EvaluationNNParameter parameter = (chromosome as TetrisChromosome).GetParameter();
            Evaluator evaluator = new Evaluator(parameter);
            List<TetrisGame> games = Enumerable.Range(0, Sample).Select(x => new TetrisGame(TetrisAITrainer.Logger)).ToList();
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
            double av = results.Average(x => x.Score+x.Round*x.Round);
            TetrisAITrainer.Logger.Info(av);
            return av;
        }
    }
}