// See https://aka.ms/new-console-template for more information
using CsvHelper;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using TrackerData;

SortedDictionary<string, List<TrackerInfo>> ReservationsByYearQtr = new();
SortedDictionary<string, int> TotalsByYearQtr = new();
Dictionary<string, QtrTotals> QtrTotals = new();
var RunningQtrTotals = new QtrTotals();

Console.WriteLine("CyberTruck tracker data analysis.");

string dataFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../../App_Data/Reservation Tracker Data.csv";

var results = TrackerInfo.Parse(dataFileName);

var count = 0;
var nonUsCount = 0;
var txCount = 0;
var needLocCount = 0;
var lastDif = 0;
var lastResNbr = 112744100;
var maxResNbr = 0;

var singleMotorCnt = 0;
var dualMotorCnt = 0;
var triMotorCnt = 0;
var quadMotorCnt = 0;
var ctCount = 0;

var qtrYear = 2019;
var qtr = 4;
var qtrDeliveries = 0;
var RnQtrStart = TrackerInfo.FirstRN;
var RnQtrEnd = TrackerInfo.LastRN_2019;

var PrevQtrPlaceInLine = 0;
var RnPrevQtrEnd = 0;

var startingPlaceInLineByQtr = RnQtrStart;
var prevQtrDeliveries = 0;
var currentQtrKey = string.Empty;


var sortedList = results.OrderBy(r => r.ReservationNumber);
Console.WriteLine("\r\nDetermining place in line by Qtr");
Console.WriteLine("\r\nCalculations for 2019.Q4 and 2020.Q1 are not very accurate. Post 2020.Q1 the calculations are better, but are still rough");
Console.WriteLine("From the tracker data, the first RN for 2020.01.01 is 112808705");
Console.WriteLine("From that point we use Tesla's quarterly delivery numbers to better track the starting position in line for each quarter.");
Console.WriteLine("\r\nThe NonUs and Tx totals may adjust position in line. That is why they are broken out of the existing tracker data.");
Console.WriteLine("Most likely we could use the NonUs totals to adjust your place in line.");
Console.WriteLine("The Tx based reservations number have a higher chance of being moved up in the line, due to Tesla's standard practices of delivering first to local locations first.");
Console.WriteLine("However, the adjustments should not be significant.");
Console.WriteLine("\r\nThe Delivery number for 2019 Q4 is calculated from the total nbr of CT reservations for 2019 from the Tracking data.");
Console.WriteLine("\r\n.The totals for Tx, NonUs etc. are running totals.");

var missMatchedQtrDate = 0;

SortByYearQuarter(sortedList);

void SortByYearQuarter(IOrderedEnumerable<TrackerInfo> sortedList)
{
    foreach (var result in sortedList)
    {
        if (!result.IsACyberTruck || result.ReservationNumber < 112744100 || (result.Date.Year <= 2019 && result.Qtr < 4))
        {
            continue;
        }

        var key = $"{result.Date.Year:d4} Q{result.Qtr}";
        if (!ReservationsByYearQtr.ContainsKey(key))
        {
            ReservationsByYearQtr.Add(key, new List<TrackerInfo>());
        }

        ReservationsByYearQtr[key].Add(result);
        if (!QtrTotals.ContainsKey(key))
        {
            QtrTotals.Add(key, new TrackerData.QtrTotals());
        }

        var qtrTotals = QtrTotals[key];

        qtrTotals.MinResNbr = Math.Min(qtrTotals.MinResNbr, result.ReservationNumber);
        qtrTotals.MaxResNbr = Math.Max(qtrTotals.MaxResNbr, result.ReservationNumber);

        if (result.State == "TX")
        {
            qtrTotals.InTx++;
        }

        if (result.US == false)
        {
            qtrTotals.NonUS++;
        }

        if (result.Trim == TrimEnum.TriMotor)
        {
            qtrTotals.TriMotor++;
        }

        if (result.Trim == TrimEnum.Quad)
        {
            qtrTotals.QuadMotor++;
        }

        if (result.Trim == TrimEnum.SingleMotor)
        {
            qtrTotals.SingleMotor++;
        }

        if (result.Trim == TrimEnum.DualMotor)
        {
            qtrTotals.DualMotor++;
        }

        if (result.Trim == TrimEnum.Cybertruck)
        {
            qtrTotals.CyberTruck++;
        }
    }

    foreach(var kvp in ReservationsByYearQtr)
    {
        var deliveries = 0;
        if (TrackerInfo.DeliversPerQuarter.ContainsKey(kvp.Key))
        {
            deliveries = TrackerInfo.DeliversPerQuarter[kvp.Key].TotalDeliveries;
        }
        var total = deliveries;
        if ( kvp.Key != "2019 Q4")
        {
            total += kvp.Value.Count;
        }

        TotalsByYearQtr.Add(kvp.Key, total);
    }

    var prevTotal = 0;
    foreach( var i in TotalsByYearQtr)
    {
        var qtrTotals = QtrTotals[i.Key];
        if (qtrTotals != null)
        {
            RunningQtrTotals.InTx += qtrTotals.InTx;
            RunningQtrTotals.NonUS += qtrTotals.NonUS;
            RunningQtrTotals.SingleMotor += qtrTotals.SingleMotor;
            RunningQtrTotals.DualMotor += qtrTotals.DualMotor;
            RunningQtrTotals.TriMotor += qtrTotals.TriMotor;
            RunningQtrTotals.QuadMotor += qtrTotals.QuadMotor;
            RunningQtrTotals.CyberTruck += qtrTotals.CyberTruck;
        }
        Console.WriteLine($"\r\nEst. place in line from start of {i.Key}: {prevTotal:N0}");
        Console.WriteLine($"\rReservation numbers ~{qtrTotals.MinResNbr} through ~{qtrTotals.MaxResNbr}");
        Console.WriteLine($"\tTx addr: {RunningQtrTotals.NonUS,7:N0}    NonUS :{RunningQtrTotals.NonUS,7:N0}");
        Console.WriteLine($"\t Single: {RunningQtrTotals.SingleMotor,7:N0}    Dual:  {RunningQtrTotals.DualMotor,7:N0}");
        Console.WriteLine($"\t    Tri: {RunningQtrTotals.TriMotor,7:N0}    Quad:  {RunningQtrTotals.QuadMotor,7:N0} ");

        prevTotal += i.Value;
    }
}

WriteMassagedFile(results);
/*
foreach (var result in sortedList)
{
    ++count;
    maxResNbr = Math.Max(maxResNbr, result.ReservationNumber);

    if (!result.IsACar)
    {
        continue;
    }

    if (result.ReservationNumber > RnQtrEnd)
    {
        //startingPlaceInLineByQtr = RnPrevQtrEnd;

        var key = $"{qtrYear:d4} Q{qtr}";
        var deliveries = TrackerInfo.DeliversPerQuarter[key].TotalDeliveries;

        Console.WriteLine(
            $"\r\nTotals for Qtr: {qtrYear} Q{qtr}: Reservation numbers: ~{RnQtrStart:d6} - ~{RnQtrEnd}, NonUS: {nonUsCount:d6}, Tx: {txCount}, Deliveries:{deliveries:N0} \r\n" +
            $"\tEst. place in line from start of Qtr: {prevQtrDeliveries:N0}");

        prevQtrDeliveries += deliveries;

        if ( ++qtr > 4)
        {
            qtrYear++;
            qtr = 1;
        }
        RnQtrStart = RnQtrEnd + 1;
        key = $"{qtrYear:d4} Q{qtr}";

        if (TrackerInfo.DeliversPerQuarter.ContainsKey(key))
        {
            RnQtrEnd = RnQtrStart + TrackerInfo.DeliversPerQuarter[key].TotalDeliveries;
            RnPrevQtrEnd += RnQtrEnd - RnQtrStart;
        } 
        else
        {
            RnQtrEnd = int.MaxValue;
        }
    }

    if (qtrYear != result.Date.Year && qtr != result.Qtr)
    {
        missMatchedQtrDate++;
    }

    if (result.State == "TX")
    {
        txCount++;
    }

    if (result.US == false)
    {
        nonUsCount++;
    }
    if (result.Trim == TrimEnum.TriMotor)
    {
        triMotorCnt++;
    }
    if (result.Trim == TrimEnum.Quad)
    {
        quadMotorCnt++;
    }
    if (result.Trim == TrimEnum.SingleMotor)
    {
        singleMotorCnt++;
    }
    if (result.Trim == TrimEnum.DualMotor)
    {
        dualMotorCnt++;
    }
    if (result.Trim == TrimEnum.Cybertruck)
    {
        ctCount++;
    }

    
    
    //{
    //    Console.WriteLine($"Reservation numbers: {lastResNbr:d6} - {result.ReservationNumber:d6}, NonUS: {nonUsCount:d6}, Tx: {txCount}");
    //    lastResNbr = result.ReservationNumber;
    //}




    //if ( count % 1000 == 0)
    //{
    //    Console.WriteLine($"Count: {count:d6}, NonUS:{nonUsCount:d6}, Tx: {txCount}");
    //}
    //if ( String.IsNullOrWhiteSpace(result.State) )
    //{
    //    Console.WriteLine($"Reservation#: {result.ReservationNumber}, address: {result.Address}");
    //}
    if ( result.Address == "Need Location")
    {
        needLocCount++;
    }
}
*/
//Console.WriteLine($"Totals for Qtr: {qtrYear} Q{qtr}: Reservation numbers: ~{RnQtrStart:d6} - ~{RnQtrEnd} = {RnQtrEnd - RnQtrStart}, NonUS: {nonUsCount:d6}, Tx: {txCount}");

//Console.WriteLine($"To find your ~position in line (by Qtr)");

//Console.WriteLine($"SingleMotor: {singleMotorCnt}, Dual: {dualMotorCnt}, Tri: {triMotorCnt}, Quad: {quadMotorCnt}, GenericCT: {ctCount}");





void WriteMassagedFile(IEnumerable<TrackerInfo> results)
{
    string dataFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../../App_Data/Reservation Tracker Data.reformatted.csv";

    var records = new List<dynamic>();

    foreach (var result in results)
    {
        dynamic record = new ExpandoObject();
        record.RNDate = result.Date;
        record.RNTime = result.Time;
        record.RNTimezone = result.TimeZone;
        record.RNNumber = result.ReservationNumber;
        record.Address = result.Address;
        record.Count = result.Count;
        record.Model = result.Trim.ToString();
        record.FSD = result.FSD;
        record.TeslaOwner = result.TeslaOwner;

        records.Add(record);
    }
    using (var writer = new StreamWriter(dataFileName))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(records);

        writer.Flush();
    }
}