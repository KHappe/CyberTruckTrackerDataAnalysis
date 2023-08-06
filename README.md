CyberTruckTrackerDataAnalysis
The goal of the application is to try and do a better analysis of Reservations being tracked by
the CyberTruck Data Tracker https://docs.google.com/spreadsheets/d/1--6OR9ECwSwZdkOtWkuslJVCyAAfQv1eJal1fdngfsk

It uses published Tesla Deliver numbers by Quarter to try and estimate what is the estimated place in line at the beginning of each Quarter



Using Tracker data from middle of June 2023

CyberTruck tracker data analysis.

Determining place in line by Qtr

Calculations for 2019.Q4 and 2020.Q1 are not very accurate. Post 2020.Q1 the calculations are better, but are still rough.
From that point we use Tesla's quarterly delivery numbers to better adjust the starting position for each quarter.

Including a break out of Tx totals.
The Tx based reservations number have a higher chance of being moved up in the line, due to Tesla's standard practices of delivering first to local locations first.
However, the adjustments should not be significant.

The total for Tx is a running total.

    Quarter: 2019 Q4
        112,000 Deliveries for quarter. ( 3/Y: 92,550 S/X: 19,450) Data provided by Tesla.
        62,769 deliveries prorated for first Qtr of CT orders

        Tracker reservation numbers: ~112744101 thru ~113206814 =
        ~    462,713 Possible reservations.
               1,881 Texas reservations. (info only - running total)
        =          0 Estimated place in line from start of Quarter.


    Quarter: 2020 Q1
        88,400 Deliveries for quarter. ( 3/Y: 76,200 S/X: 12,200) Data provided by Tesla.

        Tracker reservation numbers: ~113206861 thru ~113382179 =
        ~    175,318 Possible reservations.
               2,103 Texas reservations. (info only - running total)

             462,761 Place in line. RN:113206861 - 112744100
        -     62,769 Previous Qtr deliveries.
        =    399,992 Estimated place in line from start of Quarter.


    Quarter: 2020 Q2
        90,650 Deliveries for quarter. ( 3/Y: 80,050 S/X: 10,600) Data provided by Tesla.

        Tracker reservation numbers: ~113382208 thru ~113616808 =
        ~    234,600 Possible reservations.
               2,400 Texas reservations. (info only - running total)

             638,108 Place in line. RN:113382208 - 112744100
        -    151,169 Previous Qtr deliveries.
        =    486,939 Estimated place in line from start of Quarter.


    Quarter: 2020 Q3
       139,300 Deliveries for quarter. ( 3/Y: 124,100 S/X: 15,200) Data provided by Tesla.

        Tracker reservation numbers: ~113616875 thru ~114046673 =
        ~    429,798 Possible reservations.
               2,680 Texas reservations. (info only - running total)

             872,775 Place in line. RN:113616875 - 112744100
        -    241,819 Previous Qtr deliveries.
        =    630,956 Estimated place in line from start of Quarter.


    Quarter: 2020 Q4
       180,570 Deliveries for quarter. ( 3/Y: 161,650 S/X: 18,920) Data provided by Tesla.

        Tracker reservation numbers: ~114046780 thru ~114353977 =
        ~    307,197 Possible reservations.
               2,888 Texas reservations. (info only - running total)

           1,302,680 Place in line. RN:114046780 - 112744100
        -    381,119 Previous Qtr deliveries.
        =    921,561 Estimated place in line from start of Quarter.


    Quarter: 2021 Q1
       184,800 Deliveries for quarter. ( 3/Y: 182,780 S/X: 2,020) Data provided by Tesla.

        Tracker reservation numbers: ~114354074 thru ~114751107 =
        ~    397,033 Possible reservations.
               3,096 Texas reservations. (info only - running total)

           1,609,974 Place in line. RN:114354074 - 112744100
        -    561,689 Previous Qtr deliveries.
        =  1,048,285 Estimated place in line from start of Quarter.


    Quarter: 2021 Q2
       201,250 Deliveries for quarter. ( 3/Y: 199,360 S/X: 1,890) Data provided by Tesla.

        Tracker reservation numbers: ~114751370 thru ~115273263 =
        ~    521,893 Possible reservations.
               3,319 Texas reservations. (info only - running total)

           2,007,270 Place in line. RN:114751370 - 112744100
        -    746,489 Previous Qtr deliveries.
        =  1,260,781 Estimated place in line from start of Quarter.


    Quarter: 2021 Q3
       241,300 Deliveries for quarter. ( 3/Y: 232,025 S/X: 9,275) Data provided by Tesla.

        Tracker reservation numbers: ~115273266 thru ~115781318 =
        ~    508,052 Possible reservations.
               3,482 Texas reservations. (info only - running total)

           2,529,166 Place in line. RN:115273266 - 112744100
        -    947,739 Previous Qtr deliveries.
        =  1,581,427 Estimated place in line from start of Quarter.


    Quarter: 2021 Q4
       308,600 Deliveries for quarter. ( 3/Y: 296,850 S/X: 11,750) Data provided by Tesla.

        Tracker reservation numbers: ~115781399 thru ~116331975 =
        ~    550,576 Possible reservations.
               3,589 Texas reservations. (info only - running total)

           3,037,299 Place in line. RN:115781399 - 112744100
        -  1,189,039 Previous Qtr deliveries.
        =  1,848,260 Estimated place in line from start of Quarter.


    Quarter: 2022 Q2
       254,695 Deliveries for quarter. ( 3/Y: 238,533 S/X: 16,162) Data provided by Tesla.

        Tracker reservation numbers: ~116914822 thru ~117514426 =
        ~    599,604 Possible reservations.
               3,657 Texas reservations. (info only - running total)

           4,170,722 Place in line. RN:116914822 - 112744100
        -  1,497,639 Previous Qtr deliveries.
        =  2,673,083 Estimated place in line from start of Quarter.


    Quarter: 2022 Q1
       310,048 Deliveries for quarter. ( 3/Y: 295,324 S/X: 14,724) Data provided by Tesla.

        Tracker reservation numbers: ~116332376 thru ~116914469 =
        ~    582,093 Possible reservations.
               3,708 Texas reservations. (info only - running total)

           3,588,276 Place in line. RN:116332376 - 112744100
        -  1,752,334 Previous Qtr deliveries.
        =  1,835,942 Estimated place in line from start of Quarter.


    Quarter: 2022 Q3
       343,830 Deliveries for quarter. ( 3/Y: 325,158 S/X: 18,672) Data provided by Tesla.

        Tracker reservation numbers: ~117516035 thru ~117957600 =
        ~    441,565 Possible reservations.
               3,737 Texas reservations. (info only - running total)

           4,771,935 Place in line. RN:117516035 - 112744100
        -  2,062,382 Previous Qtr deliveries.
        =  2,709,553 Estimated place in line from start of Quarter.


    Quarter: 2022 Q4
       405,278 Deliveries for quarter. ( 3/Y: 388,131 S/X: 17,147) Data provided by Tesla.

        Tracker reservation numbers: ~117960090 thru ~118278384 =
        ~    318,294 Possible reservations.
               3,761 Texas reservations. (info only - running total)

           5,215,990 Place in line. RN:117960090 - 112744100
        -  2,406,212 Previous Qtr deliveries.
        =  2,809,778 Estimated place in line from start of Quarter.


    Quarter: 2023 Q1
       422,875 Deliveries for quarter. ( 3/Y: 412,180 S/X: 10,695) Data provided by Tesla.

        Tracker reservation numbers: ~118279957 thru ~118850539 =
        ~    570,582 Possible reservations.
               3,781 Texas reservations. (info only - running total)

           5,535,857 Place in line. RN:118279957 - 112744100
        -  2,811,490 Previous Qtr deliveries.
        =  2,724,367 Estimated place in line from start of Quarter.


    Quarter: 2023 Q2
       466,140 Deliveries for quarter. ( 3/Y: 446,915 S/X: 19,225) Data provided by Tesla.  

        Tracker reservation numbers: ~118880732 thru ~119282705 =
        ~    401,973 Possible reservations.
               3,794 Texas reservations. (info only - running total)

           6,136,632 Place in line. RN:118880732 - 112744100
        -  3,234,365 Previous Qtr deliveries.
        =  2,902,267 Estimated place in line from start of Quarter.