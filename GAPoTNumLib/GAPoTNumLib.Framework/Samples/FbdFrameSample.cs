using System;
using GAPoTNumLib.GAPoT;
using GAPoTNumLib.Text.LaTeX;

namespace GAPoTNumLib.Framework.Samples
{
    public static class FbdFrameSample
    {
        public static void DisplayFrame(GaPoTNumFrame frame)
        {
            var n = frame.Count;

            Console.WriteLine("Frame Matrix:");
            Console.WriteLine(frame.GetMatrix().GetLaTeXArray());
            Console.WriteLine();

            Console.WriteLine("Frame Inner Products Matrix:");
            Console.WriteLine(frame.GetInnerProductsMatrix().GetLaTeXArray());
            Console.WriteLine();

            Console.WriteLine("Frame Inner Angles Matrix:");
            Console.WriteLine(frame.GetInnerAnglesMatrix().GetLaTeXArray());
            Console.WriteLine();

            Console.WriteLine("Auto Vector Projection on Frame:");
            Console.WriteLine("$" + GaPoTNumVector.CreateAutoVector(n).GetProjectionOnFrame(frame).TermsToLaTeX() + "$");
            Console.WriteLine();
        }

        

        public static void Execute()
        {
            var refVectorIndex = 0;

            for (var n = 3; n <= 8; n++)
            {
                var uFrame = 
                    GaPoTNumFrame.CreateBasisFrame(n);

                var uPseudoScalar = 
                    GaPoTNumMultivector
                        .CreateZero()
                        .SetTerm((1 << n) - 1, 1.0d);

                var eFrame = 
                    GaPoTNumFrame.CreateKirchhoffFrame(n, refVectorIndex);

                var pFrame = 
                    uFrame.GetProjectionOnFrame(eFrame);

                var fbdFrame = GaPoTNumFrame.CreateFbdFrame(n);

                var rotorsSequence = GaPoTNumRotorsSequence.CreateFromFrames(
                    n, 
                    pFrame, 
                    fbdFrame
                );

                
                //var pFrame1 = pFrame
                //    .GetSubFrame(0, n - 1)
                //    .PrependVector(GaPoTNumVector.CreateAutoVector(n))
                //    .GetOrthogonalFrame(true);

                //var fbdFrame1 = fbdFrame
                //    .GetSubFrame(0, n - 1)
                //    .PrependVector(GaPoTNumVector.CreateAutoVector(n))
                //    .GetOrthogonalFrame(true);

                //var rs = GaPoTNumRotorsSequence.Create(
                //    pFrame1.GetRotorsToFrame(fbdFrame1)
                //);

                var pFrame2 = rotorsSequence.Rotate(pFrame);

                Console.Write(@"\section{Dimensions: " + n + "}");
                Console.WriteLine();

                Console.Write(@"\subsection{FBD Frame:}");
                Console.WriteLine();

                DisplayFrame(fbdFrame);

                Console.Write(@"\subsection{Projected Frame:}");
                Console.WriteLine();

                DisplayFrame(pFrame);

                Console.Write(@"\subsection{Rotated Projected Frame:}");
                Console.WriteLine();

                DisplayFrame(pFrame2);

                Console.WriteLine("Rotors Sequence:");

                for (var i = 0; i < rotorsSequence.Count; i++)
                {
                    var rotorEquation = 
                        rotorsSequence[i].ToLaTeXEquationsArray(
                            $"R_{{{i + 1}}}", 
                            @"\mu"
                        );

                    Console.WriteLine(rotorEquation);
                    Console.WriteLine();
                }
            }
        }
    }
}