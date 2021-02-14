using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Randomizations;
using System.Collections.Generic;
using System.Linq;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisCrossover : CrossoverBase
    {
        public int SeparatePointIndex { get; set; }
        public float HorizontalMixProbability { get; set; }
        public float VerticalMixProbability { get; set; }
        public TetrisCrossover(int separatePointIndex, float horizontalMixProbability, float verticalMixProbability) : base(2, 2)
        {
            SeparatePointIndex = separatePointIndex;
            HorizontalMixProbability = horizontalMixProbability;
            VerticalMixProbability = verticalMixProbability;
        }
        public TetrisCrossover(int separatePointIndex) : this(separatePointIndex, 0.5f, 0.1f)
        {
        }
        public TetrisCrossover() : this(0, 0.5f, 0.1f)
        {
        }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
        {
            var rightParent = parents[0];
            var leftParent = parents[1];

            var cutGenesCount = SeparatePointIndex + 1;
            var left_first = leftParent.CreateNew();
            left_first.ReplaceGenes(0, leftParent.GetGenes().Take(cutGenesCount).ToArray());
            var left_last = leftParent.CreateNew();
            left_last.ReplaceGenes(cutGenesCount, leftParent.GetGenes().Skip(cutGenesCount).ToArray());
            var right_first = rightParent.CreateNew();
            right_first.ReplaceGenes(0, rightParent.GetGenes().Take(cutGenesCount).ToArray());
            var right_last = rightParent.CreateNew();
            right_last.ReplaceGenes(cutGenesCount, rightParent.GetGenes().Skip(cutGenesCount).ToArray());

            //前半
            for (int i = 0; i < cutGenesCount; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < HorizontalMixProbability)
                {
                    left_first.ReplaceGene(i, left_first.GetGene(i));
                    right_first.ReplaceGene(i, right_first.GetGene(i));
                }
                else
                {
                    left_first.ReplaceGene(i, right_first.GetGene(i));
                    right_first.ReplaceGene(i, left_first.GetGene(i));
                }
            }
            //後半
            for (int i = cutGenesCount; i < left_last.Length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() < HorizontalMixProbability)
                {
                    left_last.ReplaceGene(i, left_last.GetGene(i));
                    right_last.ReplaceGene(i, right_last.GetGene(i));
                }
                else
                {
                    left_last.ReplaceGene(i, right_last.GetGene(i));
                    right_last.ReplaceGene(i, left_last.GetGene(i));
                }
            }
            //縦シャッフル
            for (int i = cutGenesCount; i < left_first.Length; i++)
            {
                if (RandomizationProvider.Current.GetDouble() >= HorizontalMixProbability)
                {
                    int random = RandomizationProvider.Current.GetInt(0, cutGenesCount);
                    left_first.ReplaceGene(random, left_last.GetGene(i));
                    left_last.ReplaceGene(i, left_first.GetGene(random));
                    right_first.ReplaceGene(random, right_last.GetGene(i));
                    right_last.ReplaceGene(i, right_first.GetGene(random));
                }
            }
            var leftChild = leftParent.CreateNew();
            leftChild.ReplaceGenes(0, left_first.GetGenes().Take(cutGenesCount).ToArray());
            leftChild.ReplaceGenes(cutGenesCount, left_last.GetGenes().Skip(cutGenesCount).ToArray());
            var rightChild = rightParent.CreateNew();
            rightChild.ReplaceGenes(0, right_first.GetGenes().Take(cutGenesCount).ToArray());
            rightChild.ReplaceGenes(cutGenesCount, right_last.GetGenes().Skip(cutGenesCount).ToArray());
            return new List<IChromosome> { leftChild, rightChild };
        }
    }
}
