% Compute the GAPoT voltage vector given the power bivector and current vector
function mvU = gapotVoltage(mvM, mvI)
    mvU = gapotGp(mvM, gapotInverse(mvI));
end
