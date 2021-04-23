using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.Mathematics
{
    public interface IRandomProvider
    {
        public int Next();
        public int Next(int maxValue);
        public int Next(int minValue, int maxValue);
        public bool NextBias(float chance);
        public bool NextBoolean();
        public void NextBytes(byte[] buffer);
        public double NextDouble();
        public double NextDouble(bool includeOne);
        public double NextDoublePositive();
        public float NextSingle();
        public float NextSingle(float minValue, float maxValue);
        public float NextSingle(bool includeOne);
        public float NextSinglePositive();
        public uint NextUnsignedInteger();
        public uint NextUnsignedInteger(uint maxValue);
        public uint NextUnsignedInteger(uint minValue, uint maxValue);
    }
}
