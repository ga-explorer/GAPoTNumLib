﻿using System;
using System.Diagnostics;

namespace GAPoTNumLib.GAPoT
{
    public sealed class GaPoTNumBivectorTerm
    {
        public static GaPoTNumBivectorTerm operator -(GaPoTNumBivectorTerm t)
        {
            return new GaPoTNumBivectorTerm(t.TermId1, t.TermId2, -t.Value);
        }

        public static GaPoTNumBivectorTerm operator +(GaPoTNumBivectorTerm t1, GaPoTNumBivectorTerm t2)
        {
            if (t1.TermId1 != t2.TermId1 || t1.TermId2 != t2.TermId2)
                throw new InvalidOperationException();

            return new GaPoTNumBivectorTerm(t1.TermId1, t1.TermId2, t1.Value + t2.Value);
        }

        public static GaPoTNumBivectorTerm operator -(GaPoTNumBivectorTerm t1, GaPoTNumBivectorTerm t2)
        {
            if (t1.TermId1 != t2.TermId1 || t1.TermId2 != t2.TermId2)
                throw new InvalidOperationException();

            return new GaPoTNumBivectorTerm(t1.TermId1, t1.TermId2, t1.Value - t2.Value);
        }


        public int TermId1 { get; }

        public int TermId2 { get; }

        public double Value { get; }

        public bool IsScalar 
            => TermId1 == TermId2;

        public bool IsNonScalar
            => TermId1 != TermId2;

        public bool IsPhasor
            => TermId1 % 2 == 1 && TermId2 == TermId1 + 1;


        internal GaPoTNumBivectorTerm(int id1, int id2, double value)
        {
            Debug.Assert(id1 > 0 && id2 > 0);

            if (id1 == id2)
            {
                TermId1 = 1;
                TermId2 = 1;
                Value = value;
            }
            else if (id1 < id2)
            {
                TermId1 = id1;
                TermId2 = id2;
                Value = value;
            }
            else
            {
                TermId1 = id2;
                TermId2 = id1;
                Value = -value;
            }
        }


        public double Norm()
        {
            return Math.Abs(Value);
        }

        public double Norm2()
        {
            return Value * Value;
        }

        public GaPoTNumBivectorTerm Reverse()
        {
            return IsScalar 
                ? this 
                : new GaPoTNumBivectorTerm(TermId1, TermId2, -Value);
        }

        public GaPoTNumBivectorTerm ScaledReverse(double s)
        {
            return IsScalar 
                ? new GaPoTNumBivectorTerm(TermId1, TermId2, Value * s)
                : new GaPoTNumBivectorTerm(TermId1, TermId2, -Value * s);
        }


        public string ToText()
        {
            if (Value == 0)
                return "0";

            return IsScalar
                ? $"{Value:G} <>"
                : $"{Value:G} <{TermId1},{TermId2}>";
        }

        public string ToLaTeX()
        {
            if (Value.IsNearZero())
                return "0";

            var valueText = Value.GetLaTeXNumber();
            var basisText = $"{TermId1},{TermId2}".GetLaTeXBasisName();

            return IsScalar
                ? $@"{valueText}"
                : $@"\left( {valueText} \right) {basisText}";
        }
 

        public GaPoTNumBivectorTerm OffsetId(int delta)
        {
            if (IsScalar)
                return this;

            var id1 = TermId1 + delta;
            var id2 = TermId2 + delta;

            return new GaPoTNumBivectorTerm(id1, id2, Value);
        }

        public override string ToString()
        {
            return ToText();
        }
    }
}