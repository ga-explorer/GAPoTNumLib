using GAPoTNumLib.GAPoT;
using GAPoTNumLib.Interop.MATLAB;

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

        public static GaPoTNumVector RectPhasorsArrayToVector(int rowsCount, int columnsCount, int[] rowIndicesArray, int[] columnIndicesArray, double[] valuesArray)
        {
            var matrixData = GaNumMatlabSparseMatrixData.CreateMatrix(rowsCount, columnsCount, rowIndicesArray, columnIndicesArray, valuesArray);

            var mv = new GaPoTNumVector();
    
            mv.AddRectPhasors(matrixData);

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
