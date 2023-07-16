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
Tracker reservation numbers: ~112744124 thru ~113200332 =
~ 456,208 Possible reservations.
1,850 Texas reservations. (info only - running total)
- 1,386 Non US reservations.
= 0 Estimated place in line from start of Quarter.

Quarter: 2020 Q1
Tracker reservation numbers: ~113200353 thru ~113367251 =
~ 166,898 Possible reservations.
2,064 Texas reservations. (info only - running total)
- 171 Non US reservations.
- 88,400 Deliveries for quarter. Data provided by Tesla.
- 4,420 Estimated 5% cancelled orders.
= 337,222 Estimated place in line from start of Quarter.

Quarter: 2020 Q2
Tracker reservation numbers: ~113367312 thru ~113597270 =
~ 229,958 Possible reservations.
2,364 Texas reservations. (info only - running total)
- 263 Non US reservations.
- 90,650 Deliveries for quarter. Data provided by Tesla.
- 4,532 Estimated 5% cancelled orders.
= 411,129 Estimated place in line from start of Quarter.

Quarter: 2020 Q3
Tracker reservation numbers: ~113597364 thru ~114005887 =
~ 408,523 Possible reservations.
2,647 Texas reservations. (info only - running total)
- 150 Non US reservations.
- 139,300 Deliveries for quarter. Data provided by Tesla.
- 6,965 Estimated 5% cancelled orders.
= 545,642 Estimated place in line from start of Quarter.

Quarter: 2020 Q4
Tracker reservation numbers: ~114005887 thru ~114335662 =
~ 329,775 Possible reservations.
2,852 Texas reservations. (info only - running total)
- 100 Non US reservations.
- 180,570 Deliveries for quarter. Data provided by Tesla.
- 9,028 Estimated 5% cancelled orders.
= 807,750 Estimated place in line from start of Quarter.

Quarter: 2021 Q1
Tracker reservation numbers: ~114335662 thru ~114730961 =
~ 395,299 Possible reservations.
3,066 Texas reservations. (info only - running total)
- 100 Non US reservations.
- 184,800 Deliveries for quarter. Data provided by Tesla.
- 9,240 Estimated 5% cancelled orders.
= 947,827 Estimated place in line from start of Quarter.

Quarter: 2021 Q2
Tracker reservation numbers: ~114731065 thru ~115239436 =
~ 508,371 Possible reservations.
3,300 Texas reservations. (info only - running total)
- 138 Non US reservations.
- 201,250 Deliveries for quarter. Data provided by Tesla.
- 10,062 Estimated 5% cancelled orders.
= 1,148,986 Estimated place in line from start of Quarter.

Quarter: 2021 Q3
Tracker reservation numbers: ~115239499 thru ~115750658 =
~ 511,159 Possible reservations.
3,477 Texas reservations. (info only - running total)
- 96 Non US reservations.
- 241,300 Deliveries for quarter. Data provided by Tesla.
- 12,065 Estimated 5% cancelled orders.
= 1,445,907 Estimated place in line from start of Quarter.

Quarter: 2021 Q4
Tracker reservation numbers: ~115750778 thru ~116307813 =
~ 557,035 Possible reservations.
3,596 Texas reservations. (info only - running total)
- 77 Non US reservations.
- 308,600 Deliveries for quarter. Data provided by Tesla.
- 15,430 Estimated 5% cancelled orders.
= 1,703,605 Estimated place in line from start of Quarter.

Quarter: 2022 Q1
Tracker reservation numbers: ~116308235 thru ~116880204 =
~ 571,969 Possible reservations.
3,652 Texas reservations. (info only - running total)
- 35 Non US reservations.
- 310,048 Deliveries for quarter. Data provided by Tesla.
- 15,502 Estimated 5% cancelled orders.
= 1,936,533 Estimated place in line from start of Quarter.

Quarter: 2022 Q2
Tracker reservation numbers: ~116882431 thru ~117490485 =
~ 608,054 Possible reservations.
3,732 Texas reservations. (info only - running total)
- 49 Non US reservations.
- 254,695 Deliveries for quarter. Data provided by Tesla.
- 12,735 Estimated 5% cancelled orders.
= 2,182,917 Estimated place in line from start of Quarter.

Quarter: 2022 Q3
Tracker reservation numbers: ~117490970 thru ~117947824 =
~ 456,854 Possible reservations.
3,763 Texas reservations. (info only - running total)
- 10 Non US reservations.
- 343,830 Deliveries for quarter. Data provided by Tesla.
- 17,192 Estimated 5% cancelled orders.
= 2,523,492 Estimated place in line from start of Quarter.

Quarter: 2022 Q4
Tracker reservation numbers: ~117948189 thru ~118267942 =
~ 319,753 Possible reservations.
3,788 Texas reservations. (info only - running total)
- 7 Non US reservations.
- 405,278 Deliveries for quarter. Data provided by Tesla.
- 20,264 Estimated 5% cancelled orders.
= 2,619,314 Estimated place in line from start of Quarter.

Quarter: 2023 Q1
Tracker reservation numbers: ~118269364 thru ~118840092 =
~ 570,728 Possible reservations.
3,811 Texas reservations. (info only - running total)
- 22 Non US reservations.
- 422,875 Deliveries for quarter. Data provided by Tesla.
- 21,144 Estimated 5% cancelled orders.
= 2,513,518 Estimated place in line from start of Quarter.

Quarter: 2023 Q2
Tracker reservation numbers: ~118840271 thru ~119282704 =
~ 442,433 Possible reservations.
3,828 Texas reservations. (info only - running total)
- 9 Non US reservations.
- 0 Deliveries for quarter. Data provided by Tesla.
- 0 Estimated 5% cancelled orders.
= 2,640,205 Estimated place in line from start of Quarter.
