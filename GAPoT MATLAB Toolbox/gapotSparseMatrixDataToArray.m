function sparseArray = gapotSparseMatrixDataToArray(sparseMatrixData)
    iArray = int32(sparseMatrixData.RowIndicesArray);
    jArray = int32(sparseMatrixData.ColumnIndicesArray);
    
    vArray = double(sparseMatrixData.ValuesArray);
    
    m = int32(sparseMatrixData.RowsCount);
    n = int32(sparseMatrixData.ColumnsCount);
    
    sparseArray = sparse(iArray, jArray, vArray, m, n);
end