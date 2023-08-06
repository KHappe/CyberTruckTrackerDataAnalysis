// See https://aka.ms/new-console-template for more information
using CsvHelper;
using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrackerData;

SortedDictionary<string, List<TrackerInfo>> ReservationsByYearQtr = new();
SortedDictionary<string, int> TotalsByYearQtr = new();
Dictionary<string, QtrTotals> QtrTotals = new();
SortedDictionary<string, QtrTotals> monthlyTotals = new();

List<TrackerInfo> BadRecords = new();

var startingPlaceInLineByQtr = TrackerInfo.FirstRN;
var currentQtrKey = string.Empty;
var first2020Q1RNumber = 0;



Console.WriteLine("CyberTruck tracker data analysis.");

string fileName = "Reservation Tracker - Cybertruck";

string dataFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../../App_Data/" + fileName + ".csv";
var results = TrackerInfo.Parse(dataFileName);

results = TrackerInfo.Normalize(results);


//var sortedList = results.OrderBy(r => r.ReservationNumber);

//WriteMassagedFile(sortedList, "byRN.raw");

fixRecords(results);
//fixRecords(sortedList);

ShowAssumptions();

ShowQtrResults();

var sortedList = results.OrderBy(r => r.Year)
    //.ThenBy(r => r.Date.Month)
    .ThenBy(r => r.Qtr)
    .ThenBy(r => r.ReservationNumber);

WriteMassagedFile(sortedList, "YearQtrRN");


//void fixRecords(IOrderedEnumerable<TrackerInfo> sortedList)
//{

//}

void fixRecords(List<TrackerInfo> sortedList)
{

    DateOnly lastDate = TrackerInfo.FirstDate;

    var theList = sortedList.ToList<TrackerInfo>();


    for (var index = 0; index < theList.Count; index++)
    {
        var record = theList[index];
        
        //lastDate = GetMedianDate(theList, index);

        //var dateDelta = Math.Abs(record.Date.DayNumber - lastDate.DayNumber);

        //if (record.Date < TrackerInfo.FirstDate || dateDelta > 2)
        //{
        //    record.OriginalDate = record.Date;
        //    record.Date = lastDate;
        //}

        UpdateYearQuarterTotals(record, index);
        //UpdateMonthlyTotals(record);
    }

    WriteMassagedFile(sortedList, "byRN");

    for ( var i = 0; i < (QtrTotals.Count-1); i++)
    {
        var kvp = QtrTotals.ElementAt(i);
        var nextKvp = QtrTotals.ElementAt(i + 1);


        adjustDates(theList, kvp, nextKvp);
    }
    WriteMassagedFile(sortedList, "byRNDateAdjusted");

    QtrTotals.Clear();

    for (var index = 0; index < theList.Count; index++)
    {
        var record = theList[index];
        UpdateYearQuarterTotals(record, index);
    }

}

void adjustDates(List<TrackerInfo> theList, KeyValuePair<string, QtrTotals> kvp, KeyValuePair<string, QtrTotals> nextKvp)
{
    var firstRN = nextKvp.Value.MinResNbr;
    var lastRN = kvp.Value.MaxResNbr;
    if (firstRN > lastRN)
    {
        return;
    }
    var firstIndex = nextKvp.Value.MinResIndex - 1;
    var lastIndex = kvp.Value.MaxResIndex;

    var firstQtrKey = theList[firstIndex].Key;
    var lastQtrKey = theList[lastIndex].Key;

    var dates = new List<int>();

    for( var i = firstIndex; i <= lastIndex; i++)
    {
        dates.Add(theList[i].Date.DayNumber);
    }

    var sortedDates = dates.OrderBy(x => x).ToList();

    var j = 0;
    for (var i = firstIndex; i <= lastIndex; i++)
    {
        theList[i].OriginalDate = theList[i].Date;
        theList[i].Date = getDateFromNumberOfDays(sortedDates[j++]);
    }
}

void UpdateYearQuarterTotals(TrackerInfo info, int index)
{
    var key = $"{info.Year:d4} Q{info.Qtr}";
    if (ReservationsByYearQtr.ContainsKey(key) == false)
    {
        ReservationsByYearQtr.Add(key, new List<TrackerInfo>());
    }

    ReservationsByYearQtr[key].Add(info);
    if (QtrTotals.ContainsKey(key) == false)
    {
        QtrTotals.Add(key, new TrackerData.QtrTotals());
    }

    var qtrTotals = QtrTotals[key];

    if (info.ReservationNumber != TrackerInfo.FirstRN)
    {
        if (info.ReservationNumber < qtrTotals.MinResNbr)
        {
            qtrTotals.MinResNbr = info.ReservationNumber;
            qtrTotals.MinResNbrDate = info.Date;
            qtrTotals.MinResIndex = index;
        }
        if (info.ReservationNumber > qtrTotals.MaxResNbr)
        {
            qtrTotals.MaxResNbr = info.ReservationNumber;
            qtrTotals.MaxResNbrDate = info.Date;
            qtrTotals.MaxResIndex = index;
        }

        //qtrTotals.MinResNbr = Math.Min(qtrTotals.MinResNbr, info.ReservationNumber);
        //qtrTotals.MaxResNbr = Math.Max(qtrTotals.MaxResNbr, info.ReservationNumber);
    }

    if (info.State == "TX")
    {
        qtrTotals.InTx++;
    }

    if (info.US == false)
    {
        qtrTotals.NonUS++;
    }

    if (info.Trim == TrimEnum.TriMotor)
    {
        qtrTotals.TriMotor++;
    }

    if (info.Trim == TrimEnum.Quad)
    {
        qtrTotals.QuadMotor++;
    }

    if (info.Trim == TrimEnum.SingleMotor)
    {
        qtrTotals.SingleMotor++;
    }

    if (info.Trim == TrimEnum.DualMotor)
    {
        qtrTotals.DualMotor++;
    }

    if (info.Trim == TrimEnum.Cybertruck)
    {
        qtrTotals.CyberTruck++;
    }
}


string GetNextQuarter(string key, bool nextQtr = true)
{
    var keyParts = key.Split(" Q");
    int year = int.Parse(keyParts[0]);
    int qtr = int.Parse(keyParts[1]);

    if (nextQtr)
    {
        qtr++;
        if (qtr > 4)
        {
            qtr = 1;
            year++;
        }
    }
    else
    {
        qtr--;
        if (qtr < 1)
        {
            qtr = 4;
            year--;
        }
    }
    return $"{year:d4} Q{qtr}";
}

string GetNextMonth(string key, bool nextMonth = true)
{
    var keyParts = key.Split(" ");
    int year = int.Parse(keyParts[0]);
    int month = int.Parse(keyParts[1]);

    if (nextMonth)
    {
        month++;
        if (month > 12)
        {
            month = 1;
            year++;
        }
    }
    else
    {
        month--;
        if (month < 1)
        {
            month = 12;
            year--;
        }
    }
    return $"{year:d4} {month:d2}";
}

void UpdateMonthlyTotals(TrackerInfo info)
{
    var key = $"{info.Year:d4} {info.Date.Month:d2}";
    if (monthlyTotals.ContainsKey(key) == false)
    {
        monthlyTotals.Add(key, new TrackerData.QtrTotals());
    }

    var month = monthlyTotals[key];
    month.MaxResNbr = 0;

    if (info.ReservationNumber != TrackerInfo.FirstRN && info.ReservationNumber < month.MinResNbr)
    {
        month.MinResNbr = info.ReservationNumber;
        month.MinResNbrDate = info.Date;
    }
}




DateOnly GetMedianDate(List<TrackerInfo> theList, int index, int range=3)
{
    List<int> dates = new List<int>();

    var currentDayNumber = 0;

    if (theList[index].Date.Year > 1)
    {
        currentDayNumber = theList[index].Date.DayNumber;
        dates.Add(currentDayNumber);
    }

    for ( var r = 1; r <= range; r++ )
    {
        if (index >= r)
        {
            if (theList[index - r].Date.Year > 1)
            {
                var record = theList[index - r];

                var hasOriginal = record.OriginalDate.Year != 1;
                var dayNumber = 0;

                if (hasOriginal)
                {
                    var originalDelta = Math.Abs(record.OriginalDate.DayNumber - currentDayNumber);
                    var dayDelta = Math.Abs(record.Date.DayNumber - currentDayNumber);
                    dayNumber =  originalDelta < dayDelta ? record.OriginalDate.DayNumber : record.Date.DayNumber;
                }
                else
                {
                    dayNumber = record.Date.DayNumber;
                }
                dates.Add(dayNumber);
            }
        }
        if (index + r < theList.Count)
        {
            if (theList[index + r].Date.Year > 1)
            {
                var record = theList[index + r];

                var hasOriginal = record.OriginalDate.Year != 1;
                var dayNumber = 0;

                if (hasOriginal)
                {
                    var originalDelta = Math.Abs(record.OriginalDate.DayNumber - currentDayNumber);
                    var dayDelta = Math.Abs(record.Date.DayNumber - currentDayNumber);
                    dayNumber = originalDelta < dayDelta ? originalDelta : dayDelta;
                }
                else
                {
                    dayNumber = record.Date.DayNumber;
                }
                dates.Add(dayNumber);
            }
        }
    }
    var normalizeDates = RemoveOutliers(dates);
    var median = (int)Math.Round(GetMedian(normalizeDates));

    return getDateFromNumberOfDays(median);
}

DateOnly getDateFromNumberOfDays(int numberOfDays)
{
    DateOnly result = new DateOnly(1, 1, 1);
    result = result.AddDays(numberOfDays);
    return result;
}


double GetMedian(IEnumerable<int> sourceNumbers)
{
     var sortedNumbers = sourceNumbers.OrderBy(x => x).ToArray();

     int size = sortedNumbers.Length;
    int mid = size / 2;
    double median = (size % 2 != 0) ? (double)sortedNumbers[mid] : ((double)sortedNumbers[mid] + (double)sortedNumbers[mid - 1]) / 2;
    return median;
}

// https://math.stackexchange.com/questions/377584/what-is-a-simple-way-to-find-the-outliers-of-an-array
IEnumerable<int> RemoveOutliers(IEnumerable<int> dates)
{
    var sortedDates = dates.OrderBy(x => x).ToList();

    var evenSize = sortedDates.Count % 2 == 0;

    var sideSize = evenSize ? sortedDates.Count / 2 : (sortedDates.Count - 1) / 2;
    var rightSideStart = evenSize ? sideSize + 1 : sideSize + 2;

    var leftDates = sortedDates.Take(sideSize);
    var rightDates = sortedDates.Skip( rightSideStart - 1).Take(sideSize);

    var Q2 = GetMedian(sortedDates);
    var Q1 = GetMedian(leftDates);
    var Q3 = GetMedian(rightDates);

    var IQR = Q3 - Q1;
    var Tlo = Q1 - 1.5 * IQR;
    var Thi = Q3 + 1.5 * IQR;

    var toRemove = new List<int>();

    for ( var i = 0; i < dates.Count(); i++)
    {
        if (sortedDates[i] < Tlo || sortedDates[i] > Thi)
        {
            toRemove.Add(i);
        }
    }
    toRemove.Reverse();

    foreach (var i in toRemove)
    {
        sortedDates.RemoveAt(i);
    }
    return sortedDates;
}


void WriteMassagedFile(IEnumerable<TrackerInfo> results, string detailName = "")
{


    if (detailName.Length > 0)
    {
        detailName = fileName + "." + detailName;
    }
    else
    {
        detailName = fileName;
    }
    string dataFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../../App_Data/" + detailName + ".csv";

    var headers = new List<dynamic>();

    foreach ( var kvp in QtrTotals)
    {
        dynamic header = new ExpandoObject();
        var key = kvp.Key;

        header.Qtr = key;
        header.MinReservationNumber = kvp.Value.MinResNbr;
        header.MaxReservationNumber = kvp.Value.MaxResNbr;
        header.MinRNDate = kvp.Value.MinResNbrDate;
        header.MaxRNDate = kvp.Value.MaxResNbrDate;
        headers.Add(header);
    }

    var records = new List<dynamic>();

    foreach (var result in results)
    {
        dynamic record = new ExpandoObject();
        record.RNNumber = result.ReservationNumber;
        record.Year = result.Year;
        record.Qtr = result.Qtr;

        record.RNDate = result.Date;
        record.RNTime = result.Time;
        record.RNTimezone = result.TimeZone;

        record.Address = result.Address;
        record.Count = result.Count;
        record.Model = result.Trim.ToString();
        record.USOrder = result.US;
        record.FSD = result.FSD;
        record.TeslaOwner = result.TeslaOwner;
        if (result.OriginalDate.Year > 1)
        {
            record.OriginalDate = result.OriginalDate.ToString();
        }
        else
        {
            record.OriginalDate = "";
        }
       //record.BadData = result.BadData;


        records.Add(record);
    }
    using (var writer = new StreamWriter(dataFileName))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        //csv.WriteRecords(headers);

        csv.WriteRecords(records);

        writer.Flush();
    }
}


void ShowQtrResults()
{
    var totalDeliveries = 0;
    var totalsForTx = 0;
    var totalsForNonUs = 0;
    var previousCTOrders = 0;
    var qtrTotal = 0;
    int prevQtrDeliveries = 0;
    foreach (var kvp in QtrTotals)
    {
        int deliveries = 0;
        int cancelledDeliveries = 0;
        var model3YDeliveries = 0;
        var modelSXDeliveries = 0;

        //if ( kvp.Key != "2019 Q4" && TrackerInfo.DeliversPerQuarter.ContainsKey(kvp.Key))
        if (TrackerInfo.DeliversPerQuarter.ContainsKey(kvp.Key))
        {
            model3YDeliveries = TrackerInfo.DeliversPerQuarter[kvp.Key].Model3Y;
            modelSXDeliveries = TrackerInfo.DeliversPerQuarter[kvp.Key].ModelSX;
            deliveries = TrackerInfo.DeliversPerQuarter[kvp.Key].TotalDeliveries;
            cancelledDeliveries = (int)Math.Round(deliveries * 0.05);
        }
        totalsForTx += kvp.Value.InTx;
        totalsForNonUs += kvp.Value.NonUS;

        qtrTotal = kvp.Value.TotalReservations - cancelledDeliveries - totalsForNonUs;

        var message =
           $"\r\nQuarter: {kvp.Key}\r\n" +
           $"{deliveries,10:N0} Deliveries for quarter. ( 3/Y: {model3YDeliveries:N0} S/X: {modelSXDeliveries:N0}) Data provided by Tesla.\r\n";

        if ( kvp.Key == "2019 Q4")
        {
            var beginQtr = new DateOnly(2019, 10, 1).DayNumber;
            var endQtr = new DateOnly(2019, 12, 31).DayNumber;
            float proratedPercentage = (float)(TrackerInfo.FirstOrderDate.DayNumber - beginQtr) / (float)( endQtr - beginQtr);
            deliveries = (int)Math.Floor((float)deliveries * proratedPercentage);
            message += $"{deliveries,10:N0} deliveries prorated for first Qtr of CT orders\r\n";
        }

        message +=
        //$"{prevQtrDeliveries,10:N0} Total previous deliveries.\r\n" +
        $"\r\n\tTracker reservation numbers: ~{kvp.Value.MinResNbr:d6} thru ~{kvp.Value.MaxResNbr} = \r\n" +
        $"\t~ {kvp.Value.TotalReservations,10:N0} Possible reservations.\r\n" +
        $"\t  {totalsForTx,10:N0} Texas reservations. (info only - running total)\r\n";
           //$"\t- {kvp.Value.NonUS,10:N0} Non US reservations.\r\n" +
           //$"\t- {cancelledDeliveries,10:N0} Estimated 5% cancelled orders. ={deliveries:N0} * .05\r\n" +
           //$"\t= {qtrTotal,10:N0} Cyber truck orders for quarter.\r\n";

        var placeInLine = 0;
        if (kvp.Key != "2019 Q4")
        {
            //placeInLine = qtrTotal + previousCTOrders - prevQtrDeliveries;
            placeInLine = kvp.Value.MinResNbr - TrackerInfo.FirstRN - prevQtrDeliveries;
        }

        if (kvp.Key == "2019 Q4")
        {
            message += $"\t= {0,10:N0} Estimated place in line from start of Quarter.\r\n";
        }
        else
        {
            //message += $"\r\n\t+ {previousCTOrders,10:N0} Previous CT orders.\r\n" +
            //           $"\t- {prevQtrDeliveries,10:N0} Previous Qtr deliveries.\r\n" + 
            //           $"\t= {placeInLine,10:N0} Estimated place in line from start of Quarter.\r\n";
            message += $"\r\n\t  {kvp.Value.MinResNbr - TrackerInfo.FirstRN,10:N0} Place in line. RN:{kvp.Value.MinResNbr} - {TrackerInfo.FirstRN}\r\n" +
                       $"\t- {prevQtrDeliveries,10:N0} Previous Qtr deliveries.\r\n" +
                       $"\t= {placeInLine,10:N0} Estimated place in line from start of Quarter.\r\n";
        }
        

        //if (kvp.Key == "2019 Q4")
        //{
        //    message += $"\t= {totalDeliveries,10:N0} Estimated place in line from start of Quarter.";
        //}
        //else
        


        //$"{totalDeliveries,10:N0} Previous CT orders.\r\n";

        Console.WriteLine(message);

        prevQtrDeliveries += deliveries;

        previousCTOrders += qtrTotal;

        totalDeliveries += kvp.Value.TotalReservations;
        totalDeliveries -= kvp.Value.NonUS;
        totalDeliveries -= deliveries;
        totalDeliveries -= cancelledDeliveries;
    }
}


void ShowAssumptions()
{
    TrackerInfo.FirstRN_2020 = first2020Q1RNumber;

    Console.WriteLine("\r\nDetermining place in line by Qtr");
    Console.WriteLine("\r\nCalculations for 2019.Q4 and 2020.Q1 are not very accurate. Post 2020.Q1 the calculations are better, but are still rough.");
    //Console.WriteLine("From the tracker data, the first RN for 2020.01.01 is " + first2020Q1RNumber);
    Console.WriteLine("From that point we use Tesla's quarterly delivery numbers to better adjust the starting position for each quarter.");
    //Console.WriteLine("Assumption that some orders are cancelled, adding a 5% estimate on the number of deliveries cancelled per quarter.");
    //Console.WriteLine("\r\nIncluding a break out of the Non Us and Tx totals. These values may adjust position in line. ");
    Console.WriteLine("\r\nIncluding a break out of Tx totals.");
    //Console.WriteLine("Most likely we could use the NonUs totals to adjust your place in line.");
    Console.WriteLine("The Tx based reservations number have a higher chance of being moved up in the line, due to Tesla's standard practices of delivering first to local locations first.");
    Console.WriteLine("However, the adjustments should not be significant.");
    //Console.WriteLine("\r\nThe Delivery number for 2019 Q4 is calculated from the total nbr of CT reservations for 2019 from the Tracking data.");
    Console.WriteLine("\r\nThe total for Tx is a running total.");
}