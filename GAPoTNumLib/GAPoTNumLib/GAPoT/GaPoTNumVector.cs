﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GAPoTNumLib.Interop.MATLAB;
using GAPoTNumLib.Text; //using CodeComposerLib.MathML.Values;

namespace GAPoTNumLib.GAPoT
{
    public sealed class GaPoTNumVector : IReadOnlyList<double>
    {
        public static GaPoTNumVector operator -(GaPoTNumVector v)
        {
            var result = new GaPoTNumVector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(term.TermId, -term.Value);

            return result;
        }

        public static GaPoTNumVector operator +(GaPoTNumVector v1, GaPoTNumVector v2)
        {
            var result = new GaPoTNumVector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value
                );

            return result;
        }

        public static GaPoTNumVector operator -(GaPoTNumVector v1, GaPoTNumVector v2)
        {
            var result = new GaPoTNumVector();

            foreach (var term in v1._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value
                );

            foreach (var term in v2._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    -term.Value
                );

            return result;
        }

        /// <summary>
        /// The geometric product of two single phase GAPoT vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static GaPoTNumBiversor operator *(GaPoTNumVector v1, GaPoTNumVector v2)
        {
            var biversor = new GaPoTNumBiversor();

            foreach (var term1 in v1.GetTerms())
            {
                foreach (var term2 in v2.GetTerms())
                {
                    var scalarValue = term1.Value * term2.Value;

                    biversor.AddTerm(
                        term1.TermId, 
                        term2.TermId, 
                        scalarValue
                    );
                }
            }

            return biversor;
        }

        public static GaPoTNumVector operator *(GaPoTNumBiversor bv1, GaPoTNumVector v2)
        {
            var vector = new GaPoTNumVector();

            foreach (var term1 in bv1.GetTerms())
            {
                foreach (var term2 in v2.GetTerms())
                {
                    var scalarValue = term1.Value * term2.Value;

                    if (term1.IsScalar)
                    {
                        vector.AddTerm(
                            term2.TermId, 
                            scalarValue
                        );

                        continue;
                    }

                    if (term1.TermId1 == term2.TermId)
                    {
                        vector.AddTerm(
                            term1.TermId2, 
                            -scalarValue
                        );

                        continue;
                    }
                    
                    if (term1.TermId2 == term2.TermId)
                    {
                        vector.AddTerm(
                            term1.TermId1, 
                            scalarValue
                        );
                    }
                }
            }

            return vector;
        }

        public static GaPoTNumVector operator *(GaPoTNumVector v1, GaPoTNumBiversor bv2)
        {
            var vector = new GaPoTNumVector();

            foreach (var term1 in v1.GetTerms())
            {
                foreach (var term2 in bv2.GetTerms())
                {
                    var scalarValue = term1.Value * term2.Value;

                    if (term2.IsScalar)
                    {
                        vector.AddTerm(
                            term1.TermId, 
                            scalarValue
                        );

                        continue;
                    }

                    if (term1.TermId == term2.TermId1)
                    {
                        vector.AddTerm(
                            term2.TermId2, 
                            scalarValue
                        );

                        continue;
                    }
                    
                    if (term1.TermId == term2.TermId2)
                    {
                        vector.AddTerm(
                            term2.TermId1, 
                            -scalarValue
                        );

                    }
                }
            }

            return vector;
        }

        public static GaPoTNumVector operator *(GaPoTNumVector v, double s)
        {
            var result = new GaPoTNumVector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value * s
                );

            return result;
        }

        public static GaPoTNumVector operator *(double s, GaPoTNumVector v)
        {
            var result = new GaPoTNumVector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value * s
                );

            return result;
        }

        public static GaPoTNumVector operator /(GaPoTNumVector v, double s)
        {
            s = 1.0d / s;

            var result = new GaPoTNumVector();

            foreach (var term in v._termsDictionary.Values)
                result.AddTerm(
                    term.TermId,
                    term.Value * s
                );

            return result;
        }


        private readonly SortedDictionary<int, GaPoTNumVectorTerm> _termsDictionary
            = new SortedDictionary<int, GaPoTNumVectorTerm>();


        public int Count 
            => _termsDictionary.Count;

        public double this[int index] 
            => _termsDictionary.TryGetValue(index, out var value) 
                ? value.Value 
                : 0;


        public GaPoTNumVector()
        {
        }

        public GaPoTNumVector(IEnumerable<GaPoTNumVectorTerm> termsList)
        {
            AddTerms(termsList);
        }


        public GaPoTNumVector SetToZero()
        {
            _termsDictionary.Clear();

            return this;
        }


        public bool IsZero()
        {
            return Norm2().IsNearZero();
        }

        //public bool HasDcTerm()
        //{
        //    return _termsDictionary.TryGetValue(0, out var term) && 
        //           term.Value != 0;
        //}

        public GaPoTNumVector SetTerm(int id, double value)
        {
            Debug.Assert(id > 0);

            if (_termsDictionary.ContainsKey(id))
                _termsDictionary[id].Value = value;
            else
                _termsDictionary.Add(id, new GaPoTNumVectorTerm(id, value));

            return this;
        }

        public GaPoTNumVector SetPolarPhasor(int id, double magnitude, double phase)
        {
            return SetRectPhasor(
                id,
                magnitude * Math.Cos(phase),
                magnitude * Math.Sin(phase)
            );
        }

        public GaPoTNumVector SetRectPhasor(int id, double x, double y)
        {
            Debug.Assert(id > 0 && id % 2 == 1);

            SetTerm(id, x);

            SetTerm(id + 1, -y);

            return this;
        }


        public GaPoTNumVector AddTerm(int id, double value)
        {
            Debug.Assert(id > 0);

            if (_termsDictionary.ContainsKey(id))
                _termsDictionary[id].Value += value;
            else
                _termsDictionary.Add(id, new GaPoTNumVectorTerm(id, value));

            return this;
        }
        
        public GaPoTNumVector AddTerm(GaPoTNumVectorTerm term)
        {
            return AddTerm(term.TermId, term.Value);
        }
        
        public GaPoTNumVector AddTerms(IEnumerable<GaPoTNumVectorTerm> termsList)
        {
            foreach (var term in termsList)
                AddTerm(term.TermId, term.Value);

            return this;
        }

        public GaPoTNumVector AddTerms(GaNumMatlabSparseMatrixData matlabArray)
        {
            for (var sparseIndex = 0; sparseIndex < matlabArray.ItemsCount; sparseIndex++)
                AddTerm(
                    matlabArray.RowIndicesArray[sparseIndex],
                    matlabArray.ValuesArray[sparseIndex]
                );

            return this;
        }

        public GaPoTNumVector AddPolarPhasor(int id, double magnitude, double phase)
        {
            return AddRectPhasor(
                id,
                magnitude * Math.Cos(phase),
                magnitude * Math.Sin(phase)
            );
        }

        public GaPoTNumVector AddPolarPhasor(GaPoTNumPolarPhasor phasor)
        {
            return AddPolarPhasor(phasor.Id, phasor.Magnitude, phasor.Phase);
        }

        public GaPoTNumVector AddPolarPhasors(GaNumMatlabSparseMatrixData matlabArray)
        {
            var array = matlabArray.GetArray();

            for (var i = 0; i < matlabArray.RowsCount; i++)
                AddPolarPhasor(
                    i * 2 + 1, 
                    array[i, 0],
                    array[i, 1]
                );

            return this;
        }

        public GaPoTNumVector AddRectPhasor(int id, double x, double y)
        {
            Debug.Assert(id > 0 && id % 2 == 1);
            
            AddTerm(id, x);

            AddTerm(id + 1, -y);

            return this;
        }

        public GaPoTNumVector AddRectPhasor(GaPoTNumRectPhasor phasor)
        {
            return AddRectPhasor(phasor.Id, phasor.XValue, phasor.YValue);
        }


        public GaPoTNumVectorTerm GetTerm(int id)
        {
            Debug.Assert(id > 0);
            
            if (_termsDictionary.TryGetValue(id, out var term))
                return term;

            return new GaPoTNumVectorTerm(id);
        }

        public GaPoTNumPolarPhasor GetPolarPhasor(int id)
        {
            return GetRectPhasor(id).ToPolarPhasor();
        }

        public GaPoTNumRectPhasor GetRectPhasor(int id)
        {
            Debug.Assert(id > 0 && id % 2 == 1);
            
            var x = this[id];
            var y = -this[id + 1];

            return new GaPoTNumRectPhasor(id, x, y);
        }

        public GaPoTNumVector AddRectPhasors(GaNumMatlabSparseMatrixData matlabArray)
        {
            var array = matlabArray.GetArray();

            for (var i = 0; i < matlabArray.RowsCount; i++)
                AddRectPhasor(
                    i * 2 + 1, 
                    array[i, 0],
                    array[i, 1]
                );

            return this;
        }

        public GaPoTNumVector GetPartByTermIDs(params int[] termIDsList)
        {
            return new GaPoTNumVector(
                GetTerms().Where(t => termIDsList.Contains(t.TermId))
            );
        }
        
        public GaPoTNumVector GetPartByTermIDsRange(int minTermId, int maxTermId)
        {
            var termsList = GetTerms()
                .Where(t => t.TermId >= minTermId && t.TermId <= maxTermId);

            return new GaPoTNumVector(termsList);
        }
        
        public GaPoTNumVector GetOffsetPartByTermIDsRange(int minTermId, int maxTermId)
        {
            var termsList = GetTerms()
                .Where(t => t.TermId >= minTermId && t.TermId <= maxTermId)
                .Select(t => t.OffsetTermId(1 - minTermId));

            return new GaPoTNumVector(termsList);
        }

        public GaPoTNumVector[] GetParts(params int[] partLengthsArray)
        {
            var results = new GaPoTNumVector[partLengthsArray.Length];

            var termId1 = 1;
            for (var i = 0; i < partLengthsArray.Length; i++)
            {
                var termId2 = termId1 + partLengthsArray[i] - 1;

                results[i] = GetPartByTermIDsRange(termId1, termId2);

                termId1 = termId2 + 1;
            }

            return results;
        }

        public GaPoTNumVector[] GetOffsetParts(params int[] partLengthsArray)
        {
            var results = new GaPoTNumVector[partLengthsArray.Length];

            var termId1 = 1;
            for (var i = 0; i < partLengthsArray.Length; i++)
            {
                var termId2 = termId1 + partLengthsArray[i] - 1;

                results[i] = GetOffsetPartByTermIDsRange(termId1, termId2);

                termId1 = termId2 + 1;
            }

            return results;
        }

        public GaPoTNumBiversor[] GetPartsImpedance(GaPoTNumVector current, params int[] partLengthsArray)
        {
            var mvU = GetParts(partLengthsArray);
            var mvI = current.GetParts(partLengthsArray).Inverse();

            return mvU.Gp(mvI);
        }

        public IEnumerable<GaPoTNumVectorTerm> GetTerms()
        {
            return _termsDictionary.Values;
        }

        //public GaPoTNumVectorTerm GetDcTerm()
        //{
        //    return GetTerm(0);
        //}

        public IEnumerable<GaPoTNumPolarPhasor> GeTPolarPhasors()
        {
            return GeTRectPhasors().Select(p => p.ToPolarPhasor());
        }

        public IEnumerable<GaPoTNumRectPhasor> GeTRectPhasors()
        {
            var phasorsDict = new Dictionary<int, GaPoTNumRectPhasor>();

            foreach (var term in _termsDictionary.Values)
            {
                var id = term.TermId;
                var x = 0.0d;
                var y = 0.0d;

                if (id % 2 == 1)
                {
                    x = term.Value;
                }
                else
                {
                    id--;
                    y = -term.Value;
                }

                if (phasorsDict.TryGetValue(id, out var phasor))
                {
                    phasor.XValue += x;
                    phasor.YValue += y;
                }
                else
                {
                    phasorsDict.Add(id, new GaPoTNumRectPhasor(id, x, y));
                }
            }

            return phasorsDict.Values;
        }


        public GaPoTNumBiversor Gp(GaPoTNumVector v)
        {
            return this * v;
        }

        public GaPoTNumVector Gp(GaPoTNumBiversor bv)
        {
            return this * bv;
        }

        public GaPoTNumVector Add(GaPoTNumVector v)
        {
            return this + v;
        }

        public GaPoTNumVector Subtract(GaPoTNumVector v)
        {
            return this - v;
        }

        public GaPoTNumVector Negative()
        {
            return -this;
        }

        public GaPoTNumVector ScaleBy(double s)
        {
            return s * this;
        }

        public GaPoTNumVector Reverse()
        {
            return this;
        }

        public double Norm()
        {
            return Math.Sqrt(Norm2());
        }

        public double Norm2()
        {
            return _termsDictionary
                .Values
                .Select(p => p.Norm2())
                .Sum();
        }

        public GaPoTNumVector Inverse()
        {
            var norm2 = Norm2();

            var result = new GaPoTNumVector();

            if (norm2 == 0)
                throw new DivideByZeroException();

            var value = 1.0d / norm2;

            foreach (var term in _termsDictionary.Values)
                result.SetTerm(
                    term.TermId,
                    term.Value * value
                );

            return result;
        }

        public GaPoTNumVector OffsetTermIDs(int delta)
        {
            return new GaPoTNumVector(
                _termsDictionary
                    .Values
                    .Select(t => t.OffsetTermId(delta))
            );
        }


        public GaNumMatlabSparseMatrixData TermsToMatlabArray(int rowsCount)
        {
            return GetTerms().TermsToMatlabArray(rowsCount);
        }

        public GaNumMatlabSparseMatrixData PartsTermsToMatlabArray(int rowsCount, params int[] partLengthArray)
        {
            return GetOffsetParts(partLengthArray).TermsToMatlabArray(rowsCount);
        }

        public GaNumMatlabSparseMatrixData PolarPhasorsToMatlabArray(int rowsCount)
        {
            return GeTPolarPhasors().PolarPhasorsToMatlabArray(rowsCount);
        }

        public GaNumMatlabSparseMatrixData RectPhasorsToMatlabArray(int rowsCount)
        {
            return GeTRectPhasors().RectPhasorsToMatlabArray(rowsCount);
        }


        public string ToText()
        {
            return PolarPhasorsToText();
        }

        public string TermsToText()
        {
            var termsArray = 
                GetTerms().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string PolarPhasorsToText()
        {
            //var dcTerm = GetTerm(0);

            var termsArray = 
                GeTPolarPhasors().Where(t => !t.IsZero()).ToArray();

            //if (dcTerm.IsZero())
                return termsArray.Length == 0
                    ? "0"
                    : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);

            //return termsArray.Length == 0
            //    ? dcTerm.ToText()
            //    : dcTerm.ToText() + ", " + termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }

        public string RectPhasorsToText()
        {
            //var dcTerm = GetTerm(0);

            var termsArray = 
                GeTRectPhasors().Where(t => !t.IsZero()).ToArray();

            //if (dcTerm.IsZero())
                return termsArray.Length == 0
                    ? "0"
                    : termsArray.Select(t => t.ToText()).Concatenate(", ", 80);

            //return termsArray.Length == 0
            //    ? dcTerm.ToText()
            //    : dcTerm.ToText() + ", " + termsArray.Select(t => t.ToText()).Concatenate(", ", 80);
        }


        public string ToLaTeX()
        {
            return PolarPhasorsToLaTeX();
        }

        public string TermsToLaTeX()
        {
            var termsArray = 
                GetTerms().Where(t => !t.IsZero()).ToArray();

            return termsArray.Length == 0
                ? "0"
                : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string PolarPhasorsToLaTeX()
        {
            //var dcTerm = GetTerm(0);

            var termsArray = 
                GeTPolarPhasors().Where(t => !t.IsZero()).ToArray();

            //if (dcTerm.IsZero())
                return termsArray.Length == 0
                    ? "0"
                    : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");

            //return termsArray.Length == 0
            //    ? dcTerm.ToLaTeX()
            //    : dcTerm.ToLaTeX() + " + " + termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }

        public string RectPhasorsToLaTeX()
        {
            //var dcTerm = GetTerm(0);

            var termsArray = 
                GeTRectPhasors().Where(t => !t.IsZero()).ToArray();

            //if (dcTerm.IsZero())
                return termsArray.Length == 0
                    ? "0"
                    : termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");

            //return termsArray.Length == 0
            //    ? dcTerm.ToLaTeX()
            //    : dcTerm.ToLaTeX() + " + " + termsArray.Select(t => t.ToLaTeX()).Concatenate(" + ");
        }


        public IEnumerator<double> GetEnumerator()
        {
            return _termsDictionary
                .Values
                .Select(v => v.Value)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _termsDictionary
                .Values
                .Select(v => v.Value)
                .GetEnumerator();
        }

        public override string ToString()
        {
            return ToText();
        }
    }
}
