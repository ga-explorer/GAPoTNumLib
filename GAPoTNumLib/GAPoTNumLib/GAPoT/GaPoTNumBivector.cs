using System;
using System.Collections.Generic;
using System.Linq;
using GAPoTNumLib.Interop.MATLAB;
using GAPoTNumLib.Structures;
using GAPoTNumLib.Text;

namespace GAPoTNumLib.GAPoT
{
    public sealed class GaPoTNumBivector
    {
        public static GaPoTNumBivector operator -(GaPoTNumBivector v)
        {
            return new GaPoTNumBivector(
                v._termsDictionary.Values.Select(t => -t)
            );
        }

        public static GaPoTNumBivector operator +(GaPoTNumBivector v1, GaPoTNumBivector v2)
        {
            var biVector = new GaPoTNumBivector();

            biVector.AddTerms(v1._termsDictionary.Values);
            biVector.AddTerms(v2._termsDictionary.Values);

            return biVector;
        }

        public static GaPoTNumBivector operator -(GaPoTNumBivector v1, GaPoTNumBivector v2)
        {
            var biVector = new GaPoTNumBivector();

            biVector.AddTerms(v1._termsDictionary.Values);
            biVector.AddTerms(v2._termsDictionary.Values.Select(t => -t));

            return biVector;
        }


        private readonly Dictionary2Keys<int, GaPoTNumBivectorTerm> _termsDictionary
            = new Dictionary2Keys<int, GaPoTNumBivectorTerm>();


        public double this[int id1, int id2]
            => _termsDictionary
                .Values
                .Where(t => t.TermId1 == id1 && t.TermId2 == id2)
                .Select(v => v.Value)
                .Sum();


        internal GaPoTNumBivector()
        {
        }

        internal GaPoTNumBivector(IEnumerable<GaPoTNumBivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);
        }


        public GaPoTNumBivector AddTerm(GaPoTNumBivectorTerm term)
        {
            if (_termsDictionary.TryGetValue(term.TermId1, term.TermId2, out var oldTerm))
                _termsDictionary[term.TermId1, term.TermId2] = new GaPoTNumBivectorTerm(
                    term.TermId1,
                    term.TermId2,
                    oldTerm.Value + term.Value
                );
            else
                _termsDictionary.Add(term.TermId1, term.TermId2, term);

            return this;
        }

        public GaPoTNumBivector AddTerm(int id1, int id2, double value)
        {
            return AddTerm(
                new GaPoTNumBivectorTerm(id1, id2, value)
            );
        }

        public GaPoTNumBivector AddTerms(IEnumerable<GaPoTNumBivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);

            return this;
        }


        public IEnumerable<GaPoTNumBivectorTerm> GetTerms()
        {
            return _termsDictionary.Values.Where(t => !t.Value.IsNearZero());
        }

        public double GetTermValuesSum()
        {
            return _termsDictionary
                .Values
                .Select(v => v.Value)
                .Sum();
        }


        public double GetTermValue(int id1, int id2)
        {
            return _termsDictionary
                .Values
                .Where(t => t.TermId1 == id1 && t.TermId2 == id2)
                .Select(v => v.Value)
                .Sum();
        }

        public double GetActiveTotal()
        {
            return _termsDictionary
                .Values
                .Where(t => t.IsScalar)
                .Select(v => v.Value)
                .Sum();
        }

        public double GetNonActiveTotal()
        {
            return _termsDictionary
                .Values
                .Where(t => t.IsNonScalar)
                .Select(v => v.Value)
                .Sum();
        }

        public double GetReactiveTotal()
        {
            return _termsDictionary
                .Values
                .Where(t => t.IsPhasor)
                .Select(v => v.Value)
                .Sum();
        }

        public double GetReactiveFundamentalTotal()
        {
            return _termsDictionary
                .Values
                .Where(t => t.TermId1 == 1 && t.TermId2 == 2)
                .Select(v => v.Value)
                .Sum();
        }

        public double GetHarmTotal()
        {
            return _termsDictionary
                .Values
                .Where(t => t.IsNonScalar && (t.TermId1 != 1 || t.TermId2 != 2))
                .Select(v => v.Value)
                .Sum();
        }


        public GaPoTNumBivector GetTermPart(int id1, int id2)
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.TermId1 == id1 && t.TermId2 == id2)
            );
        }

        public GaPoTNumBivector GetActivePart()
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.IsScalar)
            );
        }

        public GaPoTNumBivector GetNonActivePart()
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.IsNonScalar)
            );
        }

        public GaPoTNumBivector GetReactivePart()
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.IsPhasor)
            );
        }

        public GaPoTNumBivector GetReactiveFundamentalPart()
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.TermId1 == 1 && t.TermId2 == 2)
            );
        }

        public GaPoTNumBivector GetHarmPart()
        {
            return new GaPoTNumBivector(
                _termsDictionary.Values.Where(t => t.IsNonScalar && (t.TermId1 != 1 || t.TermId2 != 2))
            );
        }


        public GaPoTNumBivector Reverse()
        {
            var result = new GaPoTNumBivector();

            foreach (var pair in _termsDictionary)
            {
                if (pair.Value.IsScalar)
                    result.AddTerm(pair.Value);
                else
                    result.AddTerm(-pair.Value);
            }

            return result;
        }

        public GaPoTNumBivector NegativeReverse()
        {
            var result = new GaPoTNumBivector();

            foreach (var pair in _termsDictionary)
            {
                if (pair.Value.IsScalar)
                    result.AddTerm(-pair.Value);
                else
                    result.AddTerm(pair.Value);
            }

            return result;
        }

        public double Norm()
        {
            return Math.Sqrt(Norm2());
        }

        public double Norm2()
        {
            return Math.Abs(
                _termsDictionary
                    .Values
                    .Select(t => t.Value * t.Value)
                    .Sum()
            );
        }

        public GaPoTNumBivector Inverse()
        {
            var norm2 = Norm2();

            if (norm2 == 0)
                throw new DivideByZeroException();

            var value = 1.0d / norm2;

            return new GaPoTNumBivector(
                _termsDictionary
                    .Values
                    .Select(t => t.ScaledReverse(value))
            );
        }

        public GaPoTNumVector Gp(GaPoTNumVector v)
        {
            return this * v;
        }


        public GaNumMatlabSparseMatrixData TermsToMatlabArray(int rowsCount)
        {
            return GetTerms().TermsToMatlabArray(rowsCount);
        }


        public string ToText()
        {
            return TermsToText();
        }

        public string TermsToText()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string ToLaTeX()
        {
            return TermsToLaTeX();
        }

        public string TermsToLaTeX()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : string.Join(" + ", termsArray.Select(t => t.ToLaTeX()));
        }
 

        public override string ToString()
        {
            return TermsToText();
        }
    }
}