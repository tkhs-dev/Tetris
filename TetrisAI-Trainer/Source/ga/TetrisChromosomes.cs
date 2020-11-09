using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System.Linq;
using static TetrisAI.Source.Evaluation;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisChromosomes : ChromosomeBase
    {
        private EvaluationNNParameter parameter;
        private const float MIN_VALUE = -1f;
        private const float MAX_VALUE = 1f;

        public TetrisChromosomes() : base(NumInput * NumInput + NumInput * NumMiddle + NumMiddle * NumOutput)
        {
            for (int i = 0; i < Length; i++)
            {
                ReplaceGene(i, GenerateGene(i));
            }
        }

        public override IChromosome CreateNew()
        {
            return new TetrisChromosomes();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var rnd = RandomizationProvider.Current;
            return new Gene(rnd.GetFloat(MIN_VALUE, MAX_VALUE));
        }

        private EvaluationNNParameter GetParameter()
        {
            float[] iw = GetGenes().Skip(0).Take(NumInput * NumInput).Select(x => (float)x.Value).ToArray();
            float[] mw = GetGenes().Skip(NumInput * NumInput).Take(NumInput * NumMiddle).Select(x => (float)x.Value).ToArray();
            float[] ow = GetGenes().Skip(NumInput * NumInput+ NumInput * NumMiddle).Take(NumMiddle * NumOutput).Select(x => (float)x.Value).ToArray();

            return new EvaluationNNParameter(iw,mw,ow);
        }
    }
}