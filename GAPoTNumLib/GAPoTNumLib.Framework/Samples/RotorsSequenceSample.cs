using System;
using GAPoTNumLib.GAPoT;

namespace GAPoTNumLib.Framework.Samples
{
    public static class RotorsSequenceSample
    {
        public static void DisplayClarkeFrames()
        {
            for (var n = 2; n <= 12; n++)
            {
                var sourceFrame = GaPoTNumFrame.CreateBasisFrame(n);
                var targetFrame = GaPoTNumFrame.CreateClarkeFrame(n);

                var matrix = targetFrame.GetMatrix(n);
                
                Console.Write(@"\section{" + n + "-Dimensional Case}");
                Console.WriteLine();
                
                Console.WriteLine("Orthonormal Frame:");
                Console.WriteLine();
                
                var orthoFrameEquation = targetFrame.ToLaTeXEquationsArray(
                    "c", 
                    @"\mu"
                );

                Console.WriteLine(orthoFrameEquation);
                Console.WriteLine();

                Console.WriteLine("Rotation Matrix:");
                Console.WriteLine();

                Console.WriteLine(@"\[");
                Console.WriteLine($"{matrix.GetLaTeXArray()}");
                Console.WriteLine(@"\]");
                Console.WriteLine();

                if (n > 4) 
                    continue;

                var rotorsSequence = 
                    GaPoTNumRotorsSequence.Create(
                        sourceFrame.GetRotorsToFrame(targetFrame)
                    );

                if (!rotorsSequence.ValidateRotation(sourceFrame, targetFrame))
                    throw new InvalidOperationException("Error in rotation sequence");

                Console.WriteLine("Rotors Sequence:");
                Console.WriteLine();

                for (var i = 0; i < rotorsSequence.Count; i++)
                {
                    var rotorEquation = rotorsSequence[i].ToLaTeXEquationsArray(
                        $"R_{i + 1}",
                        @"\mu"
                    );

                    Console.WriteLine(rotorEquation);
                    Console.WriteLine();
                }

                if (n > 4) 
                    continue;

                Console.WriteLine("Final Rotor:");
                Console.WriteLine();

                var finalRotorEquation = rotorsSequence.GetFinalRotor().ToLaTeXEquationsArray(
                    "R", 
                    @"\mu"
                );

                Console.WriteLine(finalRotorEquation);
                Console.WriteLine();
            }
        }

        public static void DisplayGramSchmidtFrames()
        {
            for (var n = 2; n <= 12; n++)
            {
                var sourceFrame = GaPoTNumFrame.CreateBasisFrame(n);
                var targetFrame = GaPoTNumFrame.CreateGramSchmidtFrame(n, out var kirchhoffFrame);

                var matrix = targetFrame.GetMatrix(n);
                
                Console.Write(@"\section{" + n + "-Dimensional Case}");
                Console.WriteLine();
                
                Console.WriteLine("Kirchhoff Frame:");
                Console.WriteLine();
                
                var kirchhoffFrameEquation = kirchhoffFrame.ToLaTeXEquationsArray(
                    "e", 
                    @"\mu"
                );

                Console.WriteLine(kirchhoffFrameEquation);
                Console.WriteLine();
                
                Console.WriteLine("Orthonormal Frame:");
                Console.WriteLine();
                
                var orthoFrameEquation = targetFrame.ToLaTeXEquationsArray(
                    "c", 
                    @"\mu"
                );

                Console.WriteLine(orthoFrameEquation);
                Console.WriteLine();

                Console.WriteLine("Rotation Matrix:");
                Console.WriteLine();

                Console.WriteLine(@"\[");
                Console.WriteLine($"{matrix.GetLaTeXArray()}");
                Console.WriteLine(@"\]");
                Console.WriteLine();

                var rotorsSequence = 
                    GaPoTNumRotorsSequence.Create(
                        sourceFrame.GetRotorsToFrame(targetFrame)
                    );

                //if (!rotorsSequence.ValidateRotation(sourceFrame, targetFrame))
                //    throw new InvalidOperationException("Error in rotation sequence");

                //for (var i = 0; i < sourceFrame.Count - 1; i++)
                //{
                //    var f1 = sourceFrame.GetSubFrame(0, i + 1);
                //    var f2 = targetFrame.GetSubFrame(0, i + 1);
                //    var rs = rotorsSequence.GetSubSequence(0, i + 1);

                //    if (!rs.ValidateRotation(f1, f2))
                //        throw new InvalidOperationException("Error in rotation sequence");
                //}

                Console.WriteLine("Rotors Sequence:");
                Console.WriteLine();

                for (var i = 0; i < rotorsSequence.Count; i++)
                {
                    var rotorEquation = rotorsSequence[i].ToLaTeXEquationsArray(
                        $"R_{i + 1}",
                        @"\mu"
                    );

                    Console.WriteLine(rotorEquation);
                    Console.WriteLine();
                }

                if (n > 4) 
                    continue;

                Console.WriteLine("Final Rotor:");
                Console.WriteLine();

                var finalRotorEquation = rotorsSequence.GetFinalRotor().ToLaTeXEquationsArray(
                    "R", 
                    @"\mu"
                );

                Console.WriteLine(finalRotorEquation);
                Console.WriteLine();
            }
        }

        //public static void DisplayGramSchmidtToClarkeMatrices()
        //{
        //    for (var n = 2; n <= 12; n++)
        //    {
        //        var matrix1 = GaPoTNumFrame.CreateGramSchmidtFrame(n).GetMatrix(n);
        //        var matrix2 = GaPoTNumFrame.CreateClarkeFrame(n).GetMatrix(n);
        //        var matrix = Mfs.Dot[matrix2, Mfs.Transpose[matrix1]];

        //        Console.WriteLine($"m{n} = {matrix};");
        //        Console.WriteLine($"MatrixForm[FullSimplify[m{n}]]");
        //        Console.WriteLine($"MatrixForm[FullSimplify[Dot[m{n}, Transpose[m{n}]]]]");
        //        Console.WriteLine();
        //    }
        //}

        public static void DisplayGramSchmidtToClarkeRotors()
        {
            var n = 4;

            var frame1 = GaPoTNumFrame.CreateGramSchmidtFrame(n).GetSwappedPairsFrame();
            var frame2 = GaPoTNumFrame.CreateClarkeFrame(n);

            var rotorsSequence = GaPoTNumRotorsSequence.Create(
                frame1.GetRotorsToFrame(frame2)
            );

            var i = 1;
            foreach (var rotor in rotorsSequence)
            {
                Console.WriteLine($"R_{i} = {rotor.TermsToLaTeX()}");
                Console.WriteLine();

                i++;
            }
        }

        public static void Execute()
        {
            DisplayGramSchmidtFrames();
        }
    }
}