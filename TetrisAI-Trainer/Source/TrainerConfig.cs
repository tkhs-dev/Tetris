using GeneticSharp.Domain.Selections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TetrisCore.Source.Config;

namespace TetrisAI_Trainer.Source
{
    public class TrainerConfig : ConfigBase
    {
        /// <summary>
        /// サンプリングされるゲームの数
        /// </summary>
        public int NumSample { get; set; }

        /// <summary>
        /// Populationの数
        /// </summary>
        public int PopulationSize { get; set; }

        /// <summary>
        /// 最大のラウンド数
        /// </summary>
        public int MaxRound { get; set; }

        public float CrossoverProbability { get; set; }

        public float MutationProbability { get; set; }
        public TrainerConfig() : base("TrainerConfig")
        {

        }

        public override ConfigBase GetDefault()
        {
            return new TrainerConfig()
            {
                PopulationSize = 50,
                NumSample = 2,
                MaxRound = 50,
                CrossoverProbability = 0.5f,
                MutationProbability = 1f / 13f
            };
        }
    }
}
