﻿using System;
using System.Collections.Generic;
using Genome;
using OptimizedCore;
using VariantAnnotation.Interface.IO;
using VariantAnnotation.Interface.Positions;

namespace Vcf
{
    public sealed class SimplePosition : ISimplePosition
    {
        public int Start { get; private set; }
        public int End { get; private set; }
        public IChromosome Chromosome { get; private set; }
        public string RefAllele { get; private set; }
        public string[] AltAlleles { get; private set; }
        public string[] VcfFields { get; private set; }
        public bool[] IsDecomposed { get; private set; }
        public bool IsRecomposed { get; private set; }

        public static SimplePosition GetSimplePosition(string[] vcfFields, IDictionary<string, IChromosome> refNameToChromosome, bool isRecomposed = false)
        {
            var simplePosition = new SimplePosition
            {
                Start = Convert.ToInt32(vcfFields[VcfCommon.PosIndex]),
                Chromosome = ReferenceNameUtilities.GetChromosome(refNameToChromosome, vcfFields[VcfCommon.ChromIndex]),
                RefAllele = vcfFields[VcfCommon.RefIndex]
            };
            simplePosition.End = vcfFields[VcfCommon.AltIndex].OptimizedStartsWith('<') || vcfFields[VcfCommon.AltIndex] == "*" ? -1 : simplePosition.Start + simplePosition.RefAllele.Length - 1;
            simplePosition.AltAlleles = vcfFields[VcfCommon.AltIndex].OptimizedSplit(',');
            simplePosition.VcfFields = vcfFields;
            simplePosition.IsRecomposed = isRecomposed;
            simplePosition.IsDecomposed = new bool[simplePosition.AltAlleles.Length]; // fasle by default
            return simplePosition;
        }

        public static SimplePosition GetSimplePosition(string vcfLine,
            IDictionary<string, IChromosome> refNameToChromosome) => vcfLine == null ? null :
            GetSimplePosition(vcfLine.OptimizedSplit('\t'), refNameToChromosome);
    }
}