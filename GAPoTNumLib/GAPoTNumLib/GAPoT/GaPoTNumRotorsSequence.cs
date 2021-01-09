using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GAPoTNumLib.Text;

namespace GAPoTNumLib.GAPoT
{
    public sealed class GaPoTNumRotorsSequence : IReadOnlyList<GaPoTNumMultivector>
    {
        public static GaPoTNumRotorsSequence CreateIdentity()
        {
            return new GaPoTNumRotorsSequence();

            //return new GaPoTNumRotorsSequence().AppendRotor(
            //    GaPoTNumMultivector.CreateZero().SetTerm(0, double.INT_ONE)
            //);
        }

        public static GaPoTNumRotorsSequence Create(IEnumerable<GaPoTNumMultivector> rotorsList)
        {
            return new GaPoTNumRotorsSequence(rotorsList);
        }


        private readonly List<GaPoTNumMultivector> _rotorsList
            = new List<GaPoTNumMultivector>();


        public int Count 
            => _rotorsList.Count;
        
        public GaPoTNumMultivector this[int index]
        {
            get => _rotorsList[index];
            set => _rotorsList[index] = value;
        }

        
        internal GaPoTNumRotorsSequence()
        {
        }

        internal GaPoTNumRotorsSequence(IEnumerable<GaPoTNumMultivector> rotorsList)
        {
            _rotorsList.AddRange(rotorsList);
        }


        public bool ValidateRotation(GaPoTNumFrame sourceFrame, GaPoTNumFrame targetFrame)
        {
            if (sourceFrame.Count != targetFrame.Count)
                return false;

            var rotatedFrame = Rotate(sourceFrame);

            return !rotatedFrame.Where(
                (v, i) => !(targetFrame[i] - v).IsZero()
            ).Any();
        }

        public bool IsRotorsSequence()
        {
            return _rotorsList.All(r => r.IsRotor());
        }

        public bool IsSimpleRotorsSequence()
        {
            return _rotorsList.All(r => r.IsSimpleRotor());
        }

        public GaPoTNumMultivector GetRotor(int index)
        {
            return _rotorsList[index];
        }

        public GaPoTNumRotorsSequence AppendRotor(GaPoTNumMultivector rotor)
        {
            _rotorsList.Add(rotor);

            return this;
        }

        public GaPoTNumRotorsSequence PrependRotor(GaPoTNumMultivector rotor)
        {
            _rotorsList.Insert(0, rotor);

            return this;
        }

        public GaPoTNumRotorsSequence InsertRotor(int index, GaPoTNumMultivector rotor)
        {
            _rotorsList.Insert(index, rotor);

            return this;
        }

        public GaPoTNumRotorsSequence GetSubSequence(int startIndex, int count)
        {
            return new GaPoTNumRotorsSequence(
                _rotorsList.Skip(startIndex).Take(count)
            );
        }

        public IEnumerable<GaPoTNumVector> GetRotations(GaPoTNumVector vector)
        {
            var v = vector;

            yield return v;

            foreach (var rotor in _rotorsList)
            {
                v = v.ApplyRotor(rotor);

                yield return v;
            }
        }

        public IEnumerable<GaPoTNumMultivector> GetRotations(GaPoTNumMultivector multivector)
        {
            var mv = multivector;

            yield return mv;

            foreach (var rotor in _rotorsList)
            {
                mv = mv.ApplyRotor(rotor);

                yield return mv;
            }
        }

        public IEnumerable<GaPoTNumFrame> GetRotations(GaPoTNumFrame frame)
        {
            var f = frame;

            yield return f;

            foreach (var rotor in _rotorsList)
            {
                f = f.ApplyRotor(rotor);

                yield return f;
            }
        }

        public IEnumerable<double[,]> GetRotationMatrices(int rowsCount)
        {
            var f = GaPoTNumFrame.CreateBasisFrame(rowsCount);

            yield return f.GetMatrix(rowsCount);

            foreach (var rotor in _rotorsList)
                yield return f.ApplyRotor(rotor).GetMatrix(rowsCount);
        }

        public GaPoTNumVector Rotate(GaPoTNumVector vector)
        {
            return _rotorsList
                .Aggregate(
                    vector, 
                    (current, rotor) => current.ApplyRotor(rotor)
                );
        }

        public GaPoTNumMultivector Rotate(GaPoTNumMultivector multivector)
        {
            return _rotorsList
                .Aggregate(
                    multivector, 
                    (current, rotor) => current.ApplyRotor(rotor)
                );
        }

        public GaPoTNumFrame Rotate(GaPoTNumFrame frame)
        {
            return _rotorsList
                .Aggregate(
                    frame, 
                    (current, rotor) => current.ApplyRotor(rotor)
                );
        }

        public GaPoTNumMultivector GetFinalRotor()
        {
            return _rotorsList
                .Skip(1)
                .Aggregate(
                    _rotorsList[0], 
                    (current, rotor) => rotor.Gp(current)
                );
        }

        public double[,] GetFinalMatrix(int rowsCount)
        {
            return Rotate(
                GaPoTNumFrame.CreateBasisFrame(rowsCount)
            ).GetMatrix(rowsCount);
        }

        public IEnumerator<GaPoTNumMultivector> GetEnumerator()
        {
            return _rotorsList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string RotorsToText()
        {
            return _rotorsList
                .Select((r, i) => $"R{i + 1} = {r.TermsToText()}")
                .Concatenate(Environment.NewLine);
        }

        public override string ToString()
        {
            return RotorsToText();
        }
    }
}