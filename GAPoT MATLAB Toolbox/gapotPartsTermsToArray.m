%Create a sparse MATLAB column array from a GAPoT vector's parts
function sparseArray = gapotPartsTermsToArray(mv, rowsCount, partLengthsArray)
    sparseMatrixData = mv.PartsTermsToMatlabArray(rowsCount, int32(partLengthsArray(:)));
    
    sparseArray = gapotSparseMatrixDataToArray(sparseMatrixData);
end
