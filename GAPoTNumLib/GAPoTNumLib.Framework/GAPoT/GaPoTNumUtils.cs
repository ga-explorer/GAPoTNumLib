using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using GAPoTNumLib.Interop.MATLAB;
using GAPoTNumLib.Structures;
using Irony.Parsing;

namespace GAPoTNumLib.GAPoT
{
    public static class GaPoTNumUtils
    {
        public static int LaTeXDecimalPlaces { get; set; }
            = 4;

        public static double Epsilon { get; internal set; } 
            = Math.Pow(2, -25);

        public static bool IsNearZero(this double d)
        {
            return Math.Abs(d) <= Epsilon;
        }

        public static bool IsNearZero(this double d, double epsilon)
        {
            return Math.Abs(d) <= epsilon;
        }

        public static bool IsNearEqual(this double d1, double d2)
        {
            return Math.Abs(d2 - d1) <= Epsilon;
        }

        public static string GetLaTeXBasisName(this string basisSubscript)
        {
            //return $@"\boldsymbol{{\sigma_{{{basisSubscript}}}}}";
            return $@"\sigma_{{{basisSubscript}}}";
        }

        public static string GetLaTeXNumber(this double value)
        {
            var valueText = value.ToString("G");

            if (valueText.Contains('E'))
            {
                var ePosition = valueText.IndexOf('E');

                var magnitude = double.Parse(valueText.Substring(0, ePosition));
                var magnitudeText = Math.Round(magnitude, LaTeXDecimalPlaces).ToString("G");
                var exponentText = valueText.Substring(ePosition + 1);

                return $@"{magnitudeText} \times 10^{{{exponentText}}}";
            }

            return Math.Round(value, LaTeXDecimalPlaces).ToString("G");
        }


        private static GaPoTNumVector GaPoTNumParseVector(IronyParsingResults parsingResults, ParseTreeNode rootNode)
        {
            if (rootNode.ToString() != "spVector")
                throw new SyntaxErrorException(parsingResults.ToString());

            var vector = new GaPoTNumVector();

            var vectorNode = rootNode;
            foreach (var vectorElementNode in vectorNode.ChildNodes)
            {
                if (vectorElementNode.ToString() == "spTerm")
                {
                    //Term Form
                    var value = double.Parse(vectorElementNode.ChildNodes[0].FindTokenAndGetText());
                    var id = int.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());

                    if (id < 0)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddTerm(id, value);
                }
                else if (vectorElementNode.ToString() == "spPolarPhasor")
                {
                    //Polar Phasor Form
                    var magnitude = double.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());
                    var phase = double.Parse(vectorElementNode.ChildNodes[2].FindTokenAndGetText());
                    var id1 = int.Parse(vectorElementNode.ChildNodes[3].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[4].FindTokenAndGetText());

                    if (id1 < 0 || id2 != id1 + 1)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddPolarPhasor(id1, magnitude, phase);
                }
                else if (vectorElementNode.ToString() == "spRectPhasor")
                {
                    //Rectangular Phasor Form
                    var xValue = double.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());
                    var yValue = double.Parse(vectorElementNode.ChildNodes[2].FindTokenAndGetText());
                    var id1 = int.Parse(vectorElementNode.ChildNodes[3].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[4].FindTokenAndGetText());

                    if (id1 < 0 || id2 != id1 + 1)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    vector.AddRectPhasor(id1, xValue, yValue);
                }
                else
                {
                    throw new SyntaxErrorException(parsingResults.ToString());
                }
            }

            return vector;
        }

        public static GaPoTNumVector GaPoTNumParseVector(this string sourceText)
        {
            var parsingResults = new IronyParsingResults(
                new GaPoTNumVectorConstructorGrammar(), 
                sourceText
            );

            if (parsingResults.ContainsErrorMessages || !parsingResults.ContainsParseTreeRoot)
                throw new SyntaxErrorException(parsingResults.ToString());

            return GaPoTNumParseVector(parsingResults, parsingResults.ParseTreeRoot);
        }


        private static GaPoTNumBiversor GaPoTNumParseBiversor(IronyParsingResults parsingResults, ParseTreeNode rootNode)
        {
            if (rootNode.ToString() != "bivector")
                throw new SyntaxErrorException(parsingResults.ToString());

            var bivector = new GaPoTNumBiversor();

            var vectorNode = rootNode;
            foreach (var vectorElementNode in vectorNode.ChildNodes)
            {
                if (vectorElementNode.ToString() == "bivectorTerm0")
                {
                    //Scalar term
                    var value = double.Parse(vectorElementNode.ChildNodes[0].FindTokenAndGetText());

                    bivector.AddTerm(1, 1, value);
                }
                else if (vectorElementNode.ToString() == "bivectorTerm2")
                {
                    //Biversor term
                    var value = double.Parse(vectorElementNode.ChildNodes[0].FindTokenAndGetText());
                    var id1 = int.Parse(vectorElementNode.ChildNodes[1].FindTokenAndGetText());
                    var id2 = int.Parse(vectorElementNode.ChildNodes[2].FindTokenAndGetText());

                    if (id1 < 0 || id2 < 0)
                        throw new SyntaxErrorException(parsingResults.ToString());

                    bivector.AddTerm(id1, id2, value);
                }
                else
                {
                    throw new SyntaxErrorException(parsingResults.ToString());
                }
            }

            return bivector;
        }

        public static GaPoTNumBiversor GaPoTNumParseBiversor(this string sourceText)
        {
            var parsingResults = new IronyParsingResults(
                new GaPoTNumBiversorConstructorGrammar(), 
                sourceText
            );

            if (parsingResults.ContainsErrorMessages || !parsingResults.ContainsParseTreeRoot)
                throw new SyntaxErrorException(parsingResults.ToString());

            return GaPoTNumParseBiversor(parsingResults, parsingResults.ParseTreeRoot);
        }


        public static GaPoTNumVector[] Negative(this IEnumerable<GaPoTNumVector> vectorsList)
        {
            return vectorsList.Select(v => v.Negative()).ToArray();
        }

        public static GaPoTNumVector[] Inverse(this IEnumerable<GaPoTNumVector> vectorsList)
        {
            return vectorsList.Select(v => v.Inverse()).ToArray();
        }

        public static GaPoTNumVector[] Reverse(this IEnumerable<GaPoTNumVector> vectorsList)
        {
            return vectorsList.Select(v => v.Reverse()).ToArray();
        }

        public static double[] Norm(this IEnumerable<GaPoTNumVector> vectorsList)
        {
            return vectorsList.Select(v => v.Norm()).ToArray();
        }

        public static double[] Norm2(this IEnumerable<GaPoTNumVector> vectorsList)
        {
            return vectorsList.Select(v => v.Norm2()).ToArray();
        }

        public static GaPoTNumVector[] Add(this GaPoTNumVector[] vectorsList1, GaPoTNumVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTNumVector[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Add(vectorsList2[i]);

            return results;
        }

        public static GaPoTNumVector[] Subtract(this GaPoTNumVector[] vectorsList1, GaPoTNumVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTNumVector[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Subtract(vectorsList2[i]);

            return results;
        }

        public static GaPoTNumBiversor[] Gp(this GaPoTNumVector[] vectorsList1, GaPoTNumVector[] vectorsList2)
        {
            if (vectorsList1.Length != vectorsList2.Length)
                throw new InvalidOperationException();

            var results = new GaPoTNumBiversor[vectorsList1.Length];

            for (var i = 0; i < vectorsList1.Length; i++)
                results[i] = vectorsList1[i].Gp(vectorsList2[i]);

            return results;
        }


        public static GaNumMatlabSparseMatrixData PolarPhasorsToMatlabArray(this IEnumerable<GaPoTNumPolarPhasor> phasorsList, int rowsCount)
        {
            var termsArray = 
                phasorsList
                    .OrderBy(t => t.Id)
                    .ToArray();

            var result = GaNumMatlabSparseMatrixData.CreateMatrix(
                rowsCount, 
                2,
                termsArray.Length * 2
            );

            var sparseIndex = 0;
            foreach (var term in termsArray)
            {
                var row = (term.Id - 1) / 2 + 1;

                result.SetItem(sparseIndex, row, 1, term.Magnitude);
                result.SetItem(sparseIndex + 1, row, 2, term.Phase);

                sparseIndex += 2;
            }

            return result;
        }

        public static GaNumMatlabSparseMatrixData TermsToMatlabArray(this IEnumerable<GaPoTNumVectorTerm> termsList, int rowsCount)
        {
            var termsArray = 
                termsList
                    .OrderBy(t => t.TermId)
                    .ToArray();

            var result = GaNumMatlabSparseMatrixData.CreateColumnMatrix(
                rowsCount, 
                termsArray.Length
            );

            var sparseIndex = 0;
            foreach (var term in termsArray)
            {
                result.SetItem(sparseIndex, term.TermId, term.Value);

                sparseIndex++;
            }

            return result;
        }

        public static GaNumMatlabSparseMatrixData TermsToMatlabArray(this GaPoTNumVector[] vectorsList, int rowsCount)
        {
            var columnsCount = vectorsList.Length;

            var termsList = 
                vectorsList
                    .Select(v => v.GetTerms()
                        .OrderBy(t => t.TermId)
                        .ToArray()
                    ).ToArray();

            var result = GaNumMatlabSparseMatrixData.CreateMatrix(
                rowsCount,
                columnsCount,
                termsList.Sum(t => t.Length)
            );

            var sparseIndex = 0;
            for (var j = 0; j < columnsCount; j++)
            {
                var termsArray = 
                    termsList[j];

                foreach (var term in termsArray)
                {
                    result.SetItem(sparseIndex, term.TermId, j, term.Value);

                    sparseIndex++;
                }
            }

            return result;
        }

        public static GaNumMatlabSparseMatrixData TermsToMatlabArray(this IEnumerable<GaPoTNumBiversorTerm> termsList, int rowsCount)
        {
            var termsArray = termsList
                .OrderBy(t => t.TermId1)
                .ThenBy(t => t.TermId2)
                .ToArray();

            var result = GaNumMatlabSparseMatrixData.CreateSquareMatrix(
                rowsCount, 
                termsArray.Length
            );

            var sparseIndex = 0;
            foreach (var term in termsArray)
            {
                result.SetItem(
                    sparseIndex,
                    term.TermId1,
                    term.TermId2,
                    term.Value
                );

                sparseIndex++;
            }

            return result;
        }
    }
}
