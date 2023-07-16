# CyberTruckTrackerDataAnalysis
The goal of the application is to try and do a better analysis of Reservations being tracked by
the CyberTruck Data Tracker https://docs.google.com/spreadsheets/d/1--6OR9ECwSwZdkOtWkuslJVCyAAfQv1eJal1fdngfsk

It uses published Tesla Deliver numbers by Quarter to try and estimate what is the estimated place in line at the beginning of each Quarter



Using Tracker data from middle of June 2023

CyberTruck tracker data analysis.

Determining place in line by Qtr

Calculations for 2019.Q4 and 2020.Q1 are not very accurate. Post 2020.Q1 the calculations are better, but are still rough.
From that point we use Tesla's quarterly delivery numbers to better adjust the starting position for each quarter.
Assumption that some orders are cancelled, adding a 5% estimate on the number of deliveries cancelled per quarter.

Including a break out of the Non Us and Tx totals. These values may adjust position in line.
The Tx based reservations number have a higher chance of being moved up in the line, due to Tesla's standard practices of delivering first to local locations first.
However, the adjustments should not be significant.

The total for Tx is a running total.

Quarter: 2019 Q4
   112,000 Deliveries for quarter. ( 3/Y: 92,550 S/X: 19,450) Data provided by Tesla.
        Tracker reservation numbers: ~112744124 thru ~113200332 =
        ~    456,208 Possible reservations.
               1,850 Texas reservations. (info only - running total)
        -      1,386 Non US reservations.
        -      5,600 Estimated 5% cancelled orders.
        =    449,222 Cyber truck orders for quarter.
        =          0 Estimated place in line from start of Quarter.


Quarter: 2020 Q1
    88,400 Deliveries for quarter. ( 3/Y: 76,200 S/X: 12,200) Data provided by Tesla.
        Tracker reservation numbers: ~113200353 thru ~113367251 =
        ~    166,898 Possible reservations.
               2,064 Texas reservations. (info only - running total)
        -        171 Non US reservations.
        -      4,420 Estimated 5% cancelled orders.
        =    160,921 Cyber truck orders for quarter.

        +    449,222 Previous CT orders.
        -    112,000 Previous Qtr deliveries.
        =    498,143 Estimated place in line from start of Quarter.


Quarter: 2020 Q2
    90,650 Deliveries for quarter. ( 3/Y: 80,050 S/X: 10,600) Data provided by Tesla.
        Tracker reservation numbers: ~113367312 thru ~113597270 =
        ~    229,958 Possible reservations.
               2,364 Texas reservations. (info only - running total)
        -        263 Non US reservations.
        -      4,532 Estimated 5% cancelled orders.
        =    223,606 Cyber truck orders for quarter.

        +    610,143 Previous CT orders.
        -     88,400 Previous Qtr deliveries.
        =    745,349 Estimated place in line from start of Quarter.


Quarter: 2020 Q3
   139,300 Deliveries for quarter. ( 3/Y: 124,100 S/X: 15,200) Data provided by Tesla.
        Tracker reservation numbers: ~113597364 thru ~114005887 =
        ~    408,523 Possible reservations.
               2,647 Texas reservations. (info only - running total)
        -        150 Non US reservations.
        -      6,965 Estimated 5% cancelled orders.
        =    399,588 Cyber truck orders for quarter.

        +    833,749 Previous CT orders.
        -     90,650 Previous Qtr deliveries.
        =  1,142,687 Estimated place in line from start of Quarter.


Quarter: 2020 Q4
   180,570 Deliveries for quarter. ( 3/Y: 161,650 S/X: 18,920) Data provided by Tesla.
        Tracker reservation numbers: ~114005887 thru ~114335662 =
        ~    329,775 Possible reservations.
               2,852 Texas reservations. (info only - running total)
        -        100 Non US reservations.
        -      9,028 Estimated 5% cancelled orders.
        =    318,677 Cyber truck orders for quarter.

        +  1,233,337 Previous CT orders.
        -    139,300 Previous Qtr deliveries.
        =  1,412,714 Estimated place in line from start of Quarter.


Quarter: 2021 Q1
   184,800 Deliveries for quarter. ( 3/Y: 182,780 S/X: 2,020) Data provided by Tesla.
        Tracker reservation numbers: ~114335662 thru ~114730961 =
        ~    395,299 Possible reservations.
               3,066 Texas reservations. (info only - running total)
        -        100 Non US reservations.
        -      9,240 Estimated 5% cancelled orders.
        =    383,889 Cyber truck orders for quarter.

        +  1,552,014 Previous CT orders.
        -    180,570 Previous Qtr deliveries.
        =  1,755,333 Estimated place in line from start of Quarter.


Quarter: 2021 Q2
   201,250 Deliveries for quarter. ( 3/Y: 199,360 S/X: 1,890) Data provided by Tesla.
        Tracker reservation numbers: ~114731065 thru ~115239436 =
        ~    508,371 Possible reservations.
               3,300 Texas reservations. (info only - running total)
        -        138 Non US reservations.
        -     10,062 Estimated 5% cancelled orders.
        =    496,001 Cyber truck orders for quarter.

        +  1,935,903 Previous CT orders.
        -    184,800 Previous Qtr deliveries.
        =  2,247,104 Estimated place in line from start of Quarter.


Quarter: 2021 Q3
   241,300 Deliveries for quarter. ( 3/Y: 232,025 S/X: 9,275) Data provided by Tesla.
        Tracker reservation numbers: ~115239499 thru ~115750658 =
        ~    511,159 Possible reservations.
               3,477 Texas reservations. (info only - running total)
        -         96 Non US reservations.
        -     12,065 Estimated 5% cancelled orders.
        =    496,690 Cyber truck orders for quarter.

        +  2,431,904 Previous CT orders.
        -    201,250 Previous Qtr deliveries.
        =  2,727,344 Estimated place in line from start of Quarter.


Quarter: 2021 Q4
   308,600 Deliveries for quarter. ( 3/Y: 296,850 S/X: 11,750) Data provided by Tesla.
        Tracker reservation numbers: ~115750778 thru ~116307813 =
        ~    557,035 Possible reservations.
               3,596 Texas reservations. (info only - running total)
        -         77 Non US reservations.
        -     15,430 Estimated 5% cancelled orders.
        =    539,124 Cyber truck orders for quarter.

        +  2,928,594 Previous CT orders.
        -    241,300 Previous Qtr deliveries.
        =  3,226,418 Estimated place in line from start of Quarter.


Quarter: 2022 Q1
   310,048 Deliveries for quarter. ( 3/Y: 295,324 S/X: 14,724) Data provided by Tesla.
        Tracker reservation numbers: ~116308235 thru ~116880204 =
        ~    571,969 Possible reservations.
               3,652 Texas reservations. (info only - running total)
        -         35 Non US reservations.
        -     15,502 Estimated 5% cancelled orders.
        =    553,951 Cyber truck orders for quarter.

        +  3,467,718 Previous CT orders.
        -    308,600 Previous Qtr deliveries.
        =  3,713,069 Estimated place in line from start of Quarter.


Quarter: 2022 Q2
   254,695 Deliveries for quarter. ( 3/Y: 238,533 S/X: 16,162) Data provided by Tesla.
        Tracker reservation numbers: ~116882431 thru ~117490485 =
        ~    608,054 Possible reservations.
               3,732 Texas reservations. (info only - running total)
        -         49 Non US reservations.
        -     12,735 Estimated 5% cancelled orders.
        =    592,754 Cyber truck orders for quarter.

        +  4,021,669 Previous CT orders.
        -    310,048 Previous Qtr deliveries.
        =  4,304,375 Estimated place in line from start of Quarter.


Quarter: 2022 Q3
   343,830 Deliveries for quarter. ( 3/Y: 325,158 S/X: 18,672) Data provided by Tesla.
        Tracker reservation numbers: ~117490970 thru ~117947824 =
        ~    456,854 Possible reservations.
               3,763 Texas reservations. (info only - running total)
        -         10 Non US reservations.
        -     17,192 Estimated 5% cancelled orders.
        =    437,087 Cyber truck orders for quarter.

        +  4,614,423 Previous CT orders.
        -    254,695 Previous Qtr deliveries.
        =  4,796,815 Estimated place in line from start of Quarter.


Quarter: 2022 Q4
   405,278 Deliveries for quarter. ( 3/Y: 388,131 S/X: 17,147) Data provided by Tesla.
        Tracker reservation numbers: ~117948189 thru ~118267942 =
        ~    319,753 Possible reservations.
               3,788 Texas reservations. (info only - running total)
        -          7 Non US reservations.
        -     20,264 Estimated 5% cancelled orders.
        =    296,907 Cyber truck orders for quarter.

        +  5,051,510 Previous CT orders.
        -    343,830 Previous Qtr deliveries.
        =  5,004,587 Estimated place in line from start of Quarter.


Quarter: 2023 Q1
   422,875 Deliveries for quarter. ( 3/Y: 412,180 S/X: 10,695) Data provided by Tesla.
        Tracker reservation numbers: ~118269364 thru ~118840092 =
        ~    570,728 Possible reservations.
               3,811 Texas reservations. (info only - running total)
        -         22 Non US reservations.
        -     21,144 Estimated 5% cancelled orders.
        =    546,980 Cyber truck orders for quarter.

        +  5,348,417 Previous CT orders.
        -    405,278 Previous Qtr deliveries.
        =  5,490,119 Estimated place in line from start of Quarter.


Quarter: 2023 Q2
         0 Deliveries for quarter. ( 3/Y: 0 S/X: 0) Data provided by Tesla.
        Tracker reservation numbers: ~118840271 thru ~119282704 =
        ~    442,433 Possible reservations.
               3,828 Texas reservations. (info only - running total)
        -          9 Non US reservations.
        -          0 Estimated 5% cancelled orders.
        =    439,820 Cyber truck orders for quarter.

        +  5,895,397 Previous CT orders.
        -    422,875 Previous Qtr deliveries.
        =  5,912,342 Estimated place in line from start of Quarter.
