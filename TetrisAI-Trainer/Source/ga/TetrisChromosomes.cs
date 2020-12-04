using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using static TetrisAI.Source.Evaluator;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisChromosome : ChromosomeBase
    {
        private EvaluationNNParameter parameter;
        private const float MIN_VALUE = -1f;
        private const float MAX_VALUE = 1f;

        public TetrisChromosome() : base(NumInput * NumMiddle + NumMiddle * NumOutput)
        {
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }
        public TetrisChromosome(EvaluationNNParameter param): base(NumInput * NumMiddle + NumMiddle * NumOutput)
        {
            float[] p = param.Flatten();
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, new Gene(p[i]));
            }
        }

        public override IChromosome CreateNew()
        {
            return new TetrisChromosome();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;
            return new Gene(rnd.GetFloat(MIN_VALUE, MAX_VALUE));
        }

        public EvaluationNNParameter GetParameter()
        {
            float[] mw = GetGenes().Take(NumInput * NumMiddle).Select(x => (float)x.Value).ToArray();
            float[] ow = GetGenes().Skip(NumInput * NumMiddle).Take(NumMiddle * NumOutput).Select(x => (float)x.Value).ToArray();

            return new EvaluationNNParameter(mw, ow);
        }
    }
}