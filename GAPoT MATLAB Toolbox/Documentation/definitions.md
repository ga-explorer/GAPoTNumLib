# Type definitions and conversion 

To operate with vectors and multivectors, it is necessary to define them in a particular way. For example, the definition of vectors can be carried out in several distinct ways (rectangular or polar), so a
text parser is needed for an easy and flexible data entry. Once defined, they are stored as objects that can be displayed as text strings or arrays using specific functions.

### `gapotBivectorToTermsArray.m`
Create a sparse MATLAB array from a GAPoT multivector terms. It is used to convert a multivector object into a readable sparse array. For example, a power multivector M with 6 terms:

````matlab
>> gapotBivectorToTermsArray(M,6)
ans =
 (1,1) 35951.5264416066
 (1,2) 12441.9102309493
 (1,3) -7248.47355839335
 (1,4) -12499.6213980426
 (1,5) -7248.47355839341
 (1,6) 37383.4418599411
 ````
 
### `gapotParseBivector.m`
Parse a GAPoT bivector expression. Creates a multivector object using term components. For example, for the multivector `M = 2 − 5σ12 + 5σ34`

````matlab
>> M = gapotParseBivector("2 <>, -5 <1,2>, 5<3,4>")
M =
 GaPoTNumBivector with no properties
````
