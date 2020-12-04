using GAPoTNumLib.Framework.GAPoT;
using GAPoTNumLib.Framework.Interop.MATLAB;

namespace GAPoTNumLib.Framework
{
    public static class GaPoTNumMatlabUtils
    {
        public static GaPoTNumVector ParseVector(string sourceText)
        {
            return sourceText.GaPoTNumParseVector();
        }

        public static GaPoTNumBiversor ParseBiversor(string sourceText)
        {
            return sourceText.GaPoTNumParseBiversor();
        }

        public static GaPoTNumVector PolarPhasorsArrayToVector(int rowsCount, int columnsCount, int[] rowIndicesArray, int[] columnIndicesArray, double[] valuesArray)
        {
            var matrixData = GaNumMatlabSparseMatrixData.CreateMatrix(rowsCount, columnsCount, rowIndicesArray, columnIndicesArray, valuesArray);

            var mv = new GaPoTNumVector();
    
            mv.AddPolarPhasors(matrixData);

            return mv;
        }

        public static GaPoTNumVector TermsArrayToVector(int rowsCount, int columnsCount, int[] rowIndicesArray, int[] columnIndicesArray, double[] valuesArray)
        {
            var matrixData = GaNumMatlabSparseMatrixData.CreateMatrix(rowsCount, columnsCount, rowIndicesArray, columnIndicesArray, valuesArray);

            var mv = new GaPoTNumVector();
    
            mv.AddTerms(matrixData);

            return mv;
        }
    }
}
