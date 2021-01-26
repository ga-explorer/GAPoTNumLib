﻿using System;
using System.Linq;
using GAPoTNumLib.GAPoT;
using GAPoTNumLib.Structures;

namespace GAPoTNumLib.Framework.Samples.Validations
{
    public static class ValidationSample1
    {
        public static void Execute1()
        {
            var bv =
                "3.1009536313039 <>, -1.28445705037617 <1,3>, -0.985598559653489 <2,3>, 0.408248290463863 <1,2>"
                    .GaPoTNumParseBiversor();

            Console.WriteLine($"Biversor Squared Norm: {bv.Norm2():G}");
            Console.WriteLine();

            var mv = bv.ToMultivector();

            Console.WriteLine($"Multivector Squared Norm: {mv.Norm2():G}");
            Console.WriteLine();
        }

        public static void Execute2()
        {
            var bitsCount = 4;

            for (var i = 0; i < (1 << bitsCount); i++)
            {
                var patternText1 = 
                    Convert.ToString(i, 2).PadLeft(bitsCount, '0');

                var j = i.ReverseBits(bitsCount);

                var patternText2 = 
                    Convert.ToString(j, 2).PadLeft(bitsCount, '0');

                Console.WriteLine($"{patternText1} <=> {patternText2}, ({i.CountOnes()} bits)");
            }
        }

        public static void Execute()
        {
            var bitsCount = 4;

            var mv = GaPoTNumMultivector.CreateZero().AddTerms(
                Enumerable.Range(0, 1 << bitsCount).Select(id => new GaPoTNumMultivectorTerm(id, 1))
            );

            Console.WriteLine(mv.ToText());
        }
    }
}
