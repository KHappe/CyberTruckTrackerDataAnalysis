// See https://aka.ms/new-console-template for more information
using CsvHelper;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using TrackerData;

Console.WriteLine("Reformatting the Cybertruck tracker data.");

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
Console.WriteLine("\r\nDeterminig place in line by Qtr");
Console.WriteLine("\r\nCalculations for 2019.Q4 and 2020.Q1 are not very accurate. Post 2020.Q1 the calculations are better, but are still rough");
Console.WriteLine("From the tracker data, the first RN for 2020.01.01 is 112808705");
Console.WriteLine("From that point we use Tesla's quarterly delivery numbers to better track the starting position in line for each quarter.");
Console.WriteLine("\r\nThe NonUs and Tx totals may adjust position in line. That is why they are broken out of the existing tracker data.");
Console.WriteLine("Most likely we could use the NonUs totals to adjust your place in line.");
Console.WriteLine("The Tx based reservations number have a higher chance of being moved up in the line, due to Tesla's standard practices of delivering first to local locations first.");
Console.WriteLine("However, the adjustments should not be significant.");
Console.WriteLine("\r\nThe Delivery number for 2019 Q4 is calculated from the total nbr of CT reservations for 2019 from the Tracking data.");

var missMatchedQtrDate = 0;

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

        var key = $"{qtrYear:d4}.Q{qtr}";
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
        key = $"{qtrYear:d4}.Q{qtr}";

        if ( qtrYear != result.Date.Year && qtr != result.Qtr)
        {
            missMatchedQtrDate++;
        }

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
//Console.WriteLine($"Totals for Qtr: {qtrYear} Q{qtr}: Reservation numbers: ~{RnQtrStart:d6} - ~{RnQtrEnd} = {RnQtrEnd - RnQtrStart}, NonUS: {nonUsCount:d6}, Tx: {txCount}");

//Console.WriteLine($"To find your ~position in line (by Qtr)");

//Console.WriteLine($"SingleMotor: {singleMotorCnt}, Dual: {dualMotorCnt}, Tri: {triMotorCnt}, Quad: {quadMotorCnt}, GenericCT: {ctCount}");

WriteMassagedFile(results);

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