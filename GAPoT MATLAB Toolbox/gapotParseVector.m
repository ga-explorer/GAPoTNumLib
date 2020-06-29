%Parse a GAPoT vector expression
function mv = gapotParseVector(mvText)
    mv = GeometricAlgebraNumericsLib.Applications.GAPoT.GaPoTNumUtils.GaPoTNumParseVector(mvText);
end