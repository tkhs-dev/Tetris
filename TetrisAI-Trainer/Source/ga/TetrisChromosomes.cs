using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisAI_Trainer.Source.ga
{
    public class TetrisChromosomes
    {
        FloatingPointChromosome chromosome;
        public TetrisChromosomes()
        {
            chromosome = new FloatingPointChromosome(new double[] {-10,-10,-10 },new double[] {10,10,10 },new int[] {32,32,32 },new int[] { });
        }
    }
}
