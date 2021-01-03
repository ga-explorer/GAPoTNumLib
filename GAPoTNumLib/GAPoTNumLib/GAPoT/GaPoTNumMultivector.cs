using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GAPoTNumLib.Text;

namespace GAPoTNumLib.GAPoT
{
    public sealed class GaPoTNumMultivector
    {
        public static GaPoTNumMultivector CreateZero()
        {
            return new GaPoTNumMultivector();
        }
        
        public static GaPoTNumMultivector CreateOne()
        {
            return new GaPoTNumMultivector(new []
            {
                new GaPoTNumMultivectorTerm(0, 1.0d)
            });
        }
        
        
        public static GaPoTNumMultivector operator -(GaPoTNumMultivector v)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(term.IDsPattern, -term.Value);

            return result;
        }

        public static GaPoTNumMultivector operator +(GaPoTNumMultivector v1, GaPoTNumMultivector v2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            return result;
        }

        public static GaPoTNumMultivector operator +(GaPoTNumMultivector v1, double v2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            result.AddTerm(0, v2);

            return result;
        }

        public static GaPoTNumMultivector operator +(double v2, GaPoTNumMultivector v1)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            result.AddTerm(0, v2);

            return result;
        }

        public static GaPoTNumMultivector operator -(GaPoTNumMultivector v1, GaPoTNumMultivector v2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    -term.Value
                );

            return result;
        }

        public static GaPoTNumMultivector operator -(GaPoTNumMultivector v1, double v2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value
                );

            result.AddTerm(0, -v2);

            return result;
        }

        public static GaPoTNumMultivector operator -(double v1, GaPoTNumMultivector v2)
        {
            var result = new GaPoTNumMultivector();

            result.AddTerm(0, v1);

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    -term.Value
                );

            return result;
        }
        
        public static GaPoTNumMultivector operator *(GaPoTNumMultivector v1, GaPoTNumMultivector v2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term1 in v1._termsDictionary.Values)
            foreach (var term2 in v2._termsDictionary.Values)
            {
                var term = term1.Gp(term2);

                if (term.Value != 0)
                    result.AddTerm(term);
            }

            return result;
        }
        
        public static GaPoTNumMultivector operator *(GaPoTNumMultivector v, double s)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value * s
                );

            return result;
        }

        public static GaPoTNumMultivector operator *(double s, GaPoTNumMultivector v)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value * s
                );

            return result;
        }

        public static GaPoTNumMultivector operator /(GaPoTNumMultivector v, double s)
        {
            s = 1.0d / s;
            
            var result = new GaPoTNumMultivector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.IDsPattern,
                    term.Value * s
                );

            return result;
        }

        
        
        private readonly Dictionary<int, GaPoTNumMultivectorTerm> _termsDictionary
            = new Dictionary<int, GaPoTNumMultivectorTerm>();


        public int Count 
            => _termsDictionary.Count;

        
        internal GaPoTNumMultivector()
        {
        }

        internal GaPoTNumMultivector(IEnumerable<GaPoTNumMultivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);
        }


        public GaPoTNumMultivector SetToZero()
        {
            _termsDictionary.Clear();

            return this;
        }
        
        public bool IsZero()
        {
            return Norm2().IsNearZero();
        }

        public GaPoTNumMultivector SetTerm(int idsPattern, double value)
        {
            Debug.Assert(idsPattern >= 0);

            if (_termsDictionary.ContainsKey(idsPattern))
                _termsDictionary[idsPattern].Value = value;
            else
                _termsDictionary.Add(idsPattern, new GaPoTNumMultivectorTerm(idsPattern, value));

            return this;
        }

        public GaPoTNumMultivector AddTerm(GaPoTNumMultivectorTerm term)
        {
            var idsPattern = term.IDsPattern;

            if (_termsDictionary.TryGetValue(idsPattern, out var oldTerm))
                _termsDictionary[idsPattern] = 
                    new GaPoTNumMultivectorTerm(idsPattern, oldTerm.Value + term.Value);
            else
                _termsDictionary.Add(idsPattern, new GaPoTNumMultivectorTerm(idsPattern, term.Value));

            return this;
        }

        public GaPoTNumMultivector AddTerm(int idsPattern, double value)
        {
            Debug.Assert(idsPattern >= 0);

            if (_termsDictionary.TryGetValue(idsPattern, out var oldTerm))
                _termsDictionary[idsPattern] = 
                    new GaPoTNumMultivectorTerm(idsPattern, oldTerm.Value + value);
            else
                _termsDictionary.Add(idsPattern, new GaPoTNumMultivectorTerm(idsPattern, value));

            return this;
        }

        public GaPoTNumMultivector AddTerms(IEnumerable<GaPoTNumMultivectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term);

            return this;
        }

        
        public IEnumerable<GaPoTNumMultivectorTerm> GetTerms()
        {
            return _termsDictionary.Values.Where(t => !t.Value.IsNearZero());
        }

        public IEnumerable<GaPoTNumMultivectorTerm> GetTermsOfGrade(int grade)
        {
            Debug.Assert(grade >= 0);
            
            return GetTerms().Where(t => t.GetGrade() == grade);
        }

        public double GetTermValue(int idsPattern)
        {
            Debug.Assert(idsPattern >= 0);
            
            return _termsDictionary.TryGetValue(idsPattern, out var term) 
                ? term.Value 
                : 0.0d;
        }

        public GaPoTNumMultivectorTerm GetTerm(int idsPattern)
        {
            var value = GetTermValue(idsPattern);

            return new GaPoTNumMultivectorTerm(idsPattern, value);
        }

        public GaPoTNumVector GetVectorPart()
        {
            return new GaPoTNumVector(
                GetTermsOfGrade(1).Select(t => t.ToVectorTerm())
            );
        }

        public GaPoTNumBiversor GetBiversorPart()
        {
            var biversor = new GaPoTNumBiversor();

            var scalarValue = GetTermValue(0);

            if (scalarValue != 0.0d)
                biversor.AddTerm(new GaPoTNumBiversorTerm(scalarValue));

            biversor.AddTerms(
                GetTermsOfGrade(2).Select(t => t.ToBiversorTerm())
            );

            return biversor;
        }


        public GaPoTNumMultivector Op(GaPoTNumMultivector mv2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term1 in _termsDictionary.Values)
            foreach (var term2 in mv2._termsDictionary.Values)
            {
                var term = term1.Op(term2);

                if (term.Value != 0)
                    result.AddTerm(term);
            }

            return result;
        }
        
        public GaPoTNumMultivector Op(GaPoTNumVector v)
        {
            return Op(v.ToMultivector());
        }

        public GaPoTNumMultivector Sp(GaPoTNumMultivector mv2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term1 in _termsDictionary.Values)
            {
                if (!mv2._termsDictionary.TryGetValue(term1.IDsPattern, out var term2)) 
                    continue;
                
                var value = term1.Value * term2.Value;

                if (value == 0)
                    continue;
                    
                if (GaPoTNumUtils.ComputeIsNegativeEGp(term1.IDsPattern, term2.IDsPattern))
                    value = -value;

                result.AddTerm(new GaPoTNumMultivectorTerm(0, value));
            }

            return result;
        }

        public GaPoTNumMultivector Lcp(GaPoTNumMultivector mv2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term1 in _termsDictionary.Values)
            foreach (var term2 in mv2._termsDictionary.Values)
            {
                var term = term1.Lcp(term2);

                if (term.Value != 0)
                    result.AddTerm(term);
            }

            return result;
        }

        public GaPoTNumMultivector Gp(GaPoTNumMultivector mv2)
        {
            var result = new GaPoTNumMultivector();

            foreach (var term1 in _termsDictionary.Values)
            foreach (var term2 in mv2._termsDictionary.Values)
            {
                var term = term1.Gp(term2);

                if (term.Value != 0)
                    result.AddTerm(term);
            }

            return result;
        }

        public GaPoTNumMultivector Add(GaPoTNumMultivector v)
        {
            return this + v;
        }

        public GaPoTNumMultivector Subtract(GaPoTNumMultivector v)
        {
            return this - v;
        }

        public GaPoTNumMultivector Negative()
        {
            return -this;
        }

        public GaPoTNumMultivector ScaleBy(double s)
        {
            return s * this;
        }

        public GaPoTNumMultivector MapScalars(Func<double, double> mappingFunc)
        {
            return new GaPoTNumMultivector(
                _termsDictionary.Values.Select(
                    t => new GaPoTNumMultivectorTerm(t.IDsPattern, mappingFunc(t.Value))
                )
            );
        }

        public GaPoTNumMultivector Reverse()
        {
            return new GaPoTNumMultivector(
                _termsDictionary.Values.Select(t => t.Reverse())
            );
        }

        public GaPoTNumMultivector ScaledReverse(double s)
        {
            return new GaPoTNumMultivector(
                _termsDictionary.Values.Select(t => t.ScaledReverse(s))
            );
        }

        public double Norm()
        {
            return Math.Sqrt(Norm2());
        }

        public double Norm2()
        {
            return _termsDictionary.Values.Sum(term => 
                term.IDsPattern.BasisBladeHasNegativeReverse()
                    ? -term.Value * term.Value
                    : term.Value * term.Value
            );
        }

        public GaPoTNumMultivector Inverse()
        {
            var norm2 = 0.0d;
            var termsArray = new GaPoTNumMultivectorTerm[_termsDictionary.Count];
            var i = 0;
            foreach (var term in _termsDictionary.Values)
            {
                if (term.IDsPattern.BasisBladeHasNegativeReverse())
                {
                    termsArray[i] =  new GaPoTNumMultivectorTerm(term.IDsPattern, -term.Value);
                    norm2 += -term.Value * term.Value;
                }
                else
                {
                    termsArray[i] =  new GaPoTNumMultivectorTerm(term.IDsPattern, term.Value);
                    norm2 += term.Value * term.Value;
                }
                
                i++;
            }

            var invNorm2 = 1.0d / norm2;

            foreach (var term in termsArray)
                term.Value *= invNorm2;
            
            return new GaPoTNumMultivector(termsArray);
        }
        
        
        public string TermsToText()
        {
            var termsArray = 
                GetTerms().ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
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