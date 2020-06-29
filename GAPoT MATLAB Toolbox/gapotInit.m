global gapotAssembly gapotAssemblyPath

if (isempty(gapotAssembly))
    gapotAssemblyPath = 'D:\Projects\Active\GeometricAlgebraNumericsLib\GeometricAlgebraNumericsLibSamples\bin\x64\Debug\';
    
    if ~contains(path, gapotAssemblyPath, 'IgnoreCase', true)
        addpath(gapotAssemblyPath);
    end
    
    gapotAssembly = NET.addAssembly(strcat(gapotAssemblyPath, 'GeometricAlgebraNumericsLib.dll'));
end
