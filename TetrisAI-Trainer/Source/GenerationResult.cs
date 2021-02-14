using GeneticSharp.Domain;
using System;
using TetrisCore.Source.Config;

namespace TetrisAI_Trainer.Source
{
    public class GenerationResult : SerializableBase
    {
        /// <summary>
        /// 学習終了時の時間
        /// </summary>
        public DateTime Date { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int Generation { get; set; }
        public double? Fitness { get; set; }
        public GenerationResult()
        {
        }
        public string CreateFileName()
        {
            return $"{Generation}.xml";
        }
        public static GenerationResult Create(GeneticAlgorithm ga, TimeSpan time)
        {
            return new GenerationResult()
            {
                Date = DateTime.Now,
                ElapsedTime = time,
                TotalTime = ga.TimeEvolving,
                Generation = ga.GenerationsNumber,
                Fitness = ga.BestChromosome.Fitness
            };
        }
    }
}
