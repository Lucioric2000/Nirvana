﻿using VariantAnnotation.Interface.SA;

namespace VariantAnnotation.SA
{
    public struct SaIndexOffset : ISaIndexOffset
    {
        public int Position { get; }
        public int Offset { get; }

        public SaIndexOffset(int position, int offset)
        {
            Position = position;
            Offset   = offset;
        }
    }
}