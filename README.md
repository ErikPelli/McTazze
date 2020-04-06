# McPromo
Simple C# class to generate a valid italian McDonald's promocode


You have to import the class contained in the "McPromo" folder.
There are two public methods: generatePromocode that, with inputs, generate a promocode and decodePromocode that take a promo as string and does the reversed procedure.


Pos value is 30 by default but it could be changed.
Site is the McDonald Location int (for example one took from [here](https://github.com/ErikPelli/McCafe-Locations)).
Tx_id is the number of transaction in the day (first is one, second is two, ecc.)
