using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisChromosomes:ChromosomeBase
    {
        public TetrisChromosomes():base(0)
        {
           
        }

        public override IChromosome CreateNew()
        {
            throw new NotImplementedException();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            throw new NotImplementedException();
        }
    }
}
