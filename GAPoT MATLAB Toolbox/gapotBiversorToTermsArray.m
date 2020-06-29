%Create a sparse MATLAB array from a GAPoT bivector terms
function sparseArray = gapotBiversorToTermsArray(mv, rowsCount)
    sparseMatrixData = mv.TermsToMatlabArray(rowsCount);
    
    sparseArray = gapotSparseMatrixDataToArray(sparseMatrixData);
end
