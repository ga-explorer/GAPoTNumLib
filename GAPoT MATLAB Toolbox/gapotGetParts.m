function mvParts = gapotGetParts(mv, partLengthsArray)
    mvParts = mv.GetParts(int32(partLengthsArray));
end