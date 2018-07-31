﻿using Genome;
using System.Collections.Generic;
using Tabix;
using Xunit;

namespace UnitTests.Tabix
{
    public sealed class SearchTests
    {
        [Fact]
        public void GetMinOffset_Nominal()
        {
            const ulong expectedResults = 3591443256775;
            var linearFileOffsets = new ulong[1630];
            linearFileOffsets[1629] = expectedResults;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [6310] = new[] { new Interval(1, 1) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, linearFileOffsets);
            ulong observedResults = Search.GetMinOffset(refSeq, 26699125);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMinOffset_MissingBin()
        {
            const ulong expectedResults = 3723191187417;
            var linearFileOffsets = new ulong[2196];
            linearFileOffsets[2195] = expectedResults;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [6876] = new[] { new Interval(1, 1) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, linearFileOffsets);
            ulong observedResults = Search.GetMinOffset(refSeq, 35979265);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMinOffset_MissingFirstBin()
        {
            const ulong expectedResults = 4351134646660;
            var linearFileOffsets = new ulong[5353];
            linearFileOffsets[5352] = expectedResults;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [1254] = new[] { new Interval(1, 1) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, linearFileOffsets);
            ulong observedResults = Search.GetMinOffset(refSeq, 87687168);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMaxOffset_Nominal()
        {
            const ulong expectedResults = 3591443312067;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [6311] = new[] { new Interval(3591443312067, 3592132724129) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, null);
            ulong observedResults = Search.GetMaxOffset(refSeq, 26699126);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMaxOffset_MissingBin()
        {
            const ulong expectedResults = 3724057593420;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [6878] = new[] { new Interval(3724057593420, 3724057615020) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, null);
            ulong observedResults = Search.GetMaxOffset(refSeq, 35962881);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMaxOffset_MissingFirstBin()
        {
            const ulong expectedResults = 3724908138137;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [860] = new[] { new Interval(3724908138137, 3724908155075) }
            };

            var refSeq = new ReferenceSequence("2", idToChunks, null);
            ulong observedResults = Search.GetMaxOffset(refSeq, 36028417);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMaxOffset_MissingAllOverlappingBins_ReturnMaxOffset()
        {
            const ulong expectedResults = ulong.MaxValue;

            var idToChunks = new Dictionary<int, Interval[]>();

            var refSeq = new ReferenceSequence("2", idToChunks, null);
            ulong observedResults = Search.GetMaxOffset(refSeq, 243171329);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMinOverlapOffset_SingleBin()
        {
            const long expectedResults = 3591443256857;
            const ulong minOffset      = 3591443256775;
            const ulong maxOffset      = 3591443312067;

            var chunks = new[] { new Interval(3591443256857, 3591443311984) };

            long observedResults = Search.GetMinOverlapOffset(chunks, minOffset, maxOffset);

            Assert.Equal(expectedResults, observedResults);
        }

        [Fact]
        public void GetMinOverlap_SingleBin_NullChunks()
        {
            const ulong minOffset = 3591443256775;
            const ulong maxOffset = 3591443312067;

            long observedResults = Search.GetMinOverlapOffset(null, minOffset, maxOffset);

            Assert.Equal(0, observedResults);
        }

        private static Dictionary<int, Interval[]> GetIdToChunks()
        {
            return new Dictionary<int, Interval[]>
            {
                [0]    = new[] { new Interval(4099908124223, 4099908124304), new Interval(4951477375210, 4951477375293), new Interval(5624484975997, 5624484976080) },
                [1]    = new[] { new Interval(3340253330084, 3340253330164), new Interval(3465184408915, 3465184408994), new Interval(3568724955460, 3568724955542), new Interval(3691147500084, 3691147500165), new Interval(3795841311087, 3795841311169), new Interval(3910417270243, 3910417270325), new Interval(4000555183327, 4000555183408) },
                [12]   = new[] { new Interval(3584204706120, 3584204706202), new Interval(3603789121700, 3603789121782), new Interval(3618810913033, 3618810913115), new Interval(3636616069222, 3636616069304), new Interval(3651735457673, 3651735457755), new Interval(3666758669972, 3666758670054), new Interval(3678665150304, 3678665150385) },
                [98]   = new[] { new Interval(3586357202663, 3586357202745), new Interval(3587723007951, 3587723008032), new Interval(3589980566127, 3589980566208), new Interval(3592834453845, 3592834453927), new Interval(3595721982714, 3595721982795), new Interval(3598606802778, 3598606802860), new Interval(3600879093088, 3600879093169) },
                [788]  = new[] { new Interval(3589980579562, 3589980605258), new Interval(3590735269728, 3590735292546), new Interval(3591443256775, 3591443312067), new Interval(3592132724129, 3592132724210) },
                [6310] = new[] { new Interval(3591443256857, 3591443311984) },
                [6311] = new[] { new Interval(3591443312067, 3592132724129) }
            };
        }

        [Fact]
        public void GetOffset_Nominal()
        {
            var chr2 = new Chromosome("chr2", "2", 1);

            var linearFileOffsets = new ulong[1630];
            linearFileOffsets[1629] = 3591443256775;

            var idToChunks = GetIdToChunks();

            var refSeqs = new ReferenceSequence[2];
            refSeqs[1] = new ReferenceSequence(chr2.EnsemblName, idToChunks, linearFileOffsets);

            var index = new Index(Constants.VcfFormat, 0, 0, 0, '#', 0, refSeqs);

            long observedResult = index.GetOffset(chr2, 26699126);

            Assert.Equal(3591443256857, observedResult);
        }

        [Fact]
        public void GetOffset_UnknownChromosome_ReturnZero()
        {
            var chrUn = new Chromosome("chrUn", "unk", ushort.MaxValue);
            var index = new Index(Constants.VcfFormat, 0, 0, 0, '#', 0, null);

            long observedResult = index.GetOffset(chrUn, 26699126);
            Assert.Equal(0, observedResult);
        }

        [Fact]
        public void GetOffset_FixNegativeBeginCoordinate()
        {
            var chr2 = new Chromosome("chr2", "2", 1);

            var linearFileOffsets = new ulong[1];
            linearFileOffsets[0] = 3213608733669;

            var idToChunks = new Dictionary<int, Interval[]>
            {
                [585]  = new[] { new Interval(3213608740412, 3213608740487) },
                [4681] = new[] { new Interval(3213608733669, 3213608740412) },
                [4682] = new[] { new Interval(3213608740487, 3214303562687) }
            };

            var refSeqs = new ReferenceSequence[2];
            refSeqs[1] = new ReferenceSequence(chr2.EnsemblName, idToChunks, linearFileOffsets);

            var index = new Index(Constants.VcfFormat, 0, 0, 0, '#', 0, refSeqs);

            long observedResult = index.GetOffset(chr2, 0);

            Assert.Equal(3213608733669, observedResult);
        }
    }
}