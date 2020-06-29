%Always begin with this
gapotInit;

clc;

u1 = 5;
u2 = 10;

%Create GAPoT vector using MATLAB array of terms
mv1 = gapotTermsArrayToVector([
    100 * sqrt(2) / u1; 
    0; 
    200 * sqrt(3) / u2 * cos(60 * pi / 180); 
    -200 * sqrt(3) / u2 * sin(60 * pi / 180)
]);
gapotDisplayPhasors(mv1)

mv1.Item(1)

%Create GAPoT vector using MATLAB array of polar phasors
mv2 = gapotPolarPhasorsArrayToVector([
    100 * sqrt(2) / u1, 30 * pi / 180; 
    200 * sqrt(3) / u2, 60 * pi / 180; 
]);
gapotDisplayPhasors(mv2)

%Perform operations on vectors
mv3 = gapotAdd(mv1, mv2);

gapotDisplayTerms(mv3)

%Convert GAPoT vector terms to MATLAB array
a1 = full( gapotVectorToTermsArray(mv3, 4) )

%Convert GAPoT vector polar phasors to MATLAB array
a2 = full( gapotVectorToPolarPhasorsArray(mv3, 2) )

%Compute power bivector
mvM = gapotGp(mv1, mv2);

gapotDisplayTerms(mvM)

%Convert GAPoT bivector terms into MATLAB array
a3 = full( gapotBiversorToTermsArray(mvM, 4) )


