%Get the term part of a GAPoT power bivector
function value = gapotGetTermPart(mvM, id1, id2)
    value = mvM.GetTermPart(int32(id1), int32(id2));
end
