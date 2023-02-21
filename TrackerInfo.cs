using CsvHelper;
using System.Globalization;
using TimeZoneNames;

namespace TrackerData
{
    internal class TrackerInfo
    {
        private static Dictionary<string, string> StateAbbreviations = new();
        private static List<ZipCodeLookup> ZipCodes = new();
        private static int EmptyModels = 0;
        internal static Dictionary<string,Deliveries> DeliversPerQuarter = new();
        internal static int FirstRN = 112744100;
        internal static int FirstRN_2020 = 112808705; // Estimate from tracker spreadsheet
        internal static int LastRN_2019 = FirstRN_2020 - 1;


        internal DateOnly Date;
        internal TimeOnly Time;
        internal string TimeZone = string.Empty;
        internal int ReservationNumber;
        internal string Address = string.Empty;
        internal bool US;
        internal string State = string.Empty;
        internal int ZipCode;
        internal int Count;
        internal string Model = string.Empty;
        internal TrimEnum Trim;
        internal bool FSD;
        internal bool TeslaOwner;

        internal int Qtr
        {
            get
            {
                switch( Date.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                        return 1;
                    case 4:
                    case 5:
                    case 6:
                        return 2;
                    case 7:
                    case 8:
                    case 9:
                        return 3;
                    default:
                        return 4;
                }
            }
        }
        internal bool IsACar
        {
            get
            {
                return
                    this.Trim == TrimEnum.Cybertruck ||
                    this.Trim == TrimEnum.TriMotor ||
                    this.Trim == TrimEnum.SingleMotor ||
                    this.Trim == TrimEnum.DualMotor ||
                    this.Trim == TrimEnum.Quad ||
                    this.Trim == TrimEnum.Model3 ||
                    this.Trim == TrimEnum.ModelS ||
                    this.Trim == TrimEnum.ModelX ||
                    this.Trim == TrimEnum.ModelY;
            }
        }

        static TrackerInfo()
        {
            AddStates();
            AddZipCodes();
            AddDeliveries();
        }

        static internal List<TrackerInfo> Parse(string csvFileName)
        {
            List<TrackerInfo> Results = new();

            using (var reader = new StreamReader(csvFileName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                int i = 0;
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var info = new TrackerInfo();
                    Results.Add(info);
                    try
                    {
                        var successful = ParseDate(info, csv.GetField(0));
                        successful = ParseTime(info, csv.GetField(1));
                        successful = ParseTimeZone(info, csv.GetField(2));

                        info.Address = ParseRegion(info, csv.GetField(3));
                        successful = ParseReservationNumber(info, csv.GetField(4));
                        int count = 0;
                        try
                        {
                            info.Count = csv.GetField<int>(5);
                        }
                        catch( Exception ex)
                        {
                            info.Count = 10;
                        }
                        successful = ParseModel(info, csv.GetField(6));

                        info.FSD = StringToBool(csv.GetField(7));

                        info.TeslaOwner = StringToBool(csv.GetField(8));

                        
                        i++;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            return Results;
        }
        static internal bool ParseReservationNumber(TrackerInfo record, string value)
        {
            return int.TryParse(value, out record.ReservationNumber);
        }

        static internal bool ParseModel(TrackerInfo record, string value)
        {
            string model = value.ToUpper();
            if (model.IndexOf("DUAL") >= 0)
            {
                record.Trim = TrimEnum.DualMotor;
                return true;
            }
            if (model.IndexOf("TRI") >= 0)
            {
                record.Trim = TrimEnum.TriMotor;
                return true;
            }
            if (model.IndexOf("SINGLE") >= 0)
            {
                record.Trim = TrimEnum.SingleMotor;
                return true;
            }
            if (model.IndexOf("QUAD") >= 0)
            {
                record.Trim = TrimEnum.Quad;
                return true;
            }
            if (model == "MOTOR" || model.IndexOf("CYBERTRUCK") >= 0 || model.IndexOf("TBD") >= 0)
            {
                record.Trim = TrimEnum.Cybertruck;
                return true;
            }
            if (model.IndexOf("SOLAR") >= 0)
            {
                record.Trim = TrimEnum.Solar;
                return true;
            }
            if (model.IndexOf("BATTERY") >= 0)
            {
                record.Trim = TrimEnum.Powerwall;
                return true;
            }
            if (model.IndexOf("SEMI") >= 0)
            {
                record.Trim = TrimEnum.Semi;
                return true;
            }
            if (model.IndexOf("ROADSTER") >= 0)
            {
                record.Trim = TrimEnum.Roadster;
                return true;
            }
            if (model.IndexOf("3") >= 0)
            {
                record.Trim = TrimEnum.Model3;
                return true;
            }
            if (model.IndexOf("X") >= 0)
            {
                record.Trim = TrimEnum.ModelX;
                return true;
            }
            if (model.IndexOf("Y") >= 0)
            {
                record.Trim = TrimEnum.ModelY;
                return true;
            }
            if (model.IndexOf("S") >= 0)
            {
                record.Trim = TrimEnum.ModelS;
                return true;
            }

            EmptyModels++;
            return false;
        }

        static internal bool StringToBool( string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            bool result;
            if (bool.TryParse(value, out result))
                return result;

            return false;
        }

        static internal bool ParseDate(TrackerInfo record, string value)
        {
            return DateOnly.TryParse(value, out record.Date);
        }
        static internal bool ParseTime(TrackerInfo record, string value)
        {
            return TimeOnly.TryParse(value, out record.Time);
        }

        static internal bool ParseTimeZone(TrackerInfo record, string value)
        {
            record.TimeZone = value;
            // figure this out later
            //var tzNames = TZNames.GetDisplayNames("en-US");

            return true;
        }

        static internal string ParseRegion(TrackerInfo record, string value)
        {
            var fields = value.Split(", ".ToCharArray());

            var success = FindState(record, fields);

            if (record.State.Length == 0)
            {
                return value;
            }

            record.US = true;

            string address = value.Replace(",","");
            address = address.Replace(record.State, "");
            address = address.Replace(record.ZipCode.ToString(), "").Trim();

            if (!StateAbbreviations.ContainsKey(record.State))
            {
                System.Diagnostics.Debugger.Break();
            }
            var stateName = StateAbbreviations[record.State];
            int pos = address.IndexOf(stateName, StringComparison.OrdinalIgnoreCase);
            if ( pos >= 0)
            {
                var name = address.Substring(pos, stateName.Length);
                address = address.Replace(name, "");  // may be lower case etc what ever.
            }
            if ( address.Length > 0 )
            {
                address += ", ";
            }
            address += record.State;
            if ( record.ZipCode != 0)
            {
                address += " " + record.ZipCode;
            }

            // figure this out later
            //var tzNames = TZNames.GetDisplayNames("en-US");

            // reformat this later
            return address;
        }

        private static bool FindState(TrackerInfo record, string[] fields)
        {
            bool successful = false;
            AddStates();

            foreach (var f in fields)
            {
                var field = f.ToUpper();
                if ( field.Length == 0)
                {
                    continue;
                }
                if (StateAbbreviations.ContainsKey(field))
                {
                    record.State = field;
                    successful = true;
                }
                if (StateAbbreviations.ContainsValue(field))
                {
                    record.State = StateAbbreviations.FirstOrDefault(kvp => kvp.Value == field).Key;
                    successful = true;
                }
                if (field.Length >= 4 && field.Length <= 5)
                {
                    int zip;
                    if ( int.TryParse(field, out zip))
                    {
                        var zipCode = FindZip(zip);
                        if( zipCode != null )
                        {
                            record.State = zipCode.State;
                            record.ZipCode = zip;
                        }
                    }
                }
            }

            return successful;
        }

        private static ZipCodeLookup FindZip( int value)
        {
            AddZipCodes();
            foreach( var item in ZipCodes)
            {
                if ( value >= item.ZipMin && value <= item.ZipMax )
                {
                    return item;
                }
            }

            return null;
        }

        private static void AddStates()
        {
            if (StateAbbreviations.Count != 0)
            {
                return;
            }

            StateAbbreviations.Add("AL", "Alabama".ToUpper());
            StateAbbreviations.Add("AK", "Alaska".ToUpper());
            StateAbbreviations.Add("AZ", "Arizona".ToUpper());
            StateAbbreviations.Add("AR", "Arkansas".ToUpper());
            StateAbbreviations.Add("CA", "California".ToUpper());
            StateAbbreviations.Add("CO", "Colorado".ToUpper());
            StateAbbreviations.Add("CT", "Connecticut".ToUpper());
            StateAbbreviations.Add("DE", "Delaware".ToUpper());
            StateAbbreviations.Add("DC", "District of Columbia".ToUpper());
            StateAbbreviations.Add("FL", "Florida".ToUpper());
            StateAbbreviations.Add("GA", "Georgia".ToUpper());
            StateAbbreviations.Add("HI", "Hawaii".ToUpper());
            StateAbbreviations.Add("ID", "Idaho".ToUpper());
            StateAbbreviations.Add("IL", "Illinois".ToUpper());
            StateAbbreviations.Add("IN", "Indiana".ToUpper());
            StateAbbreviations.Add("IA", "Iowa".ToUpper());
            StateAbbreviations.Add("KS", "Kansas".ToUpper());
            StateAbbreviations.Add("KY", "Kentucky".ToUpper());
            StateAbbreviations.Add("LA", "Louisiana".ToUpper());
            StateAbbreviations.Add("ME", "Maine".ToUpper());
            StateAbbreviations.Add("MD", "Maryland".ToUpper());
            StateAbbreviations.Add("MA", "Massachusetts".ToUpper());
            StateAbbreviations.Add("MI", "Michigan".ToUpper());
            StateAbbreviations.Add("MN", "Minnesota".ToUpper());
            StateAbbreviations.Add("MS", "Mississippi".ToUpper());
            StateAbbreviations.Add("MO", "Missouri".ToUpper());
            StateAbbreviations.Add("MT", "Montana".ToUpper());
            StateAbbreviations.Add("NE", "Nebraska".ToUpper());
            StateAbbreviations.Add("NV", "Nevada".ToUpper());
            StateAbbreviations.Add("NH", "New Hampshire".ToUpper());
            StateAbbreviations.Add("NJ", "New Jersey".ToUpper());
            StateAbbreviations.Add("NM", "New Mexico".ToUpper());
            StateAbbreviations.Add("NY", "New York".ToUpper());
            StateAbbreviations.Add("NC", "North Carolina".ToUpper());
            StateAbbreviations.Add("ND", "North Dakota".ToUpper());
            StateAbbreviations.Add("OH", "Ohio".ToUpper());
            StateAbbreviations.Add("OK", "Oklahoma".ToUpper());
            StateAbbreviations.Add("OR", "Oregon".ToUpper());
            StateAbbreviations.Add("PA", "Pennsylvania".ToUpper());
            StateAbbreviations.Add("RI", "Rhode Island".ToUpper());
            StateAbbreviations.Add("SC", "South Carolina".ToUpper());
            StateAbbreviations.Add("SD", "South Dakota".ToUpper());
            StateAbbreviations.Add("TN", "Tennessee".ToUpper());
            StateAbbreviations.Add("TX", "Texas".ToUpper());
            StateAbbreviations.Add("UT", "Utah".ToUpper());
            StateAbbreviations.Add("VT", "Vermont".ToUpper());
            StateAbbreviations.Add("VA", "Virginia".ToUpper());
            StateAbbreviations.Add("WA", "Washington".ToUpper());
            StateAbbreviations.Add("WV", "West Virginia".ToUpper());
            StateAbbreviations.Add("WI", "Wisconsin".ToUpper());
            StateAbbreviations.Add("WY", "Wyoming".ToUpper());
        }

        private static void AddZipCodes()
        {
            if (ZipCodes.Count != 0)
            {
                return;
            }
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "AK",
                ZipMin = 99501,
                ZipMax = 99950
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "AL",
                ZipMin = 35004,
                ZipMax = 36925
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "AR",
                ZipMin = 71601,
                ZipMax = 72959
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "AR",
                SubArea = "Texarkana",
                ZipMin = 71601,
                ZipMax = 72959
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "AZ",
                ZipMin = 85001,
                ZipMax = 86556
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CA",
                ZipMin = 90001,
                ZipMax = 96162
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CO",
                ZipMin = 80001,
                ZipMax = 81658
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CT",
                ZipMin = 6001,
                ZipMax = 6389
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CT",
                SubArea = "1",
                ZipMin = 6401,
                ZipMax = 6928
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "DC",
                ZipMin = 20001,
                ZipMax = 20039
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "DC",
                SubArea = "1",
                ZipMin = 20042,
                ZipMax = 20599
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "DC",
                SubArea = "2",
                ZipMin = 20799,
                ZipMax = 20799
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "DE",
                ZipMin = 19701,
                ZipMax = 19980
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "FL",
                ZipMin = 32004,
                ZipMax = 34997
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "GA",
                ZipMin = 30001,
                ZipMax = 31999
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "GA",
                SubArea= "1",
                ZipMin = 39901,
                ZipMax = 39901
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "HI",
                ZipMin = 96701,
                ZipMax = 96898
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "IA",
                ZipMin = 50001,
                ZipMax = 52809
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "IA",
                SubArea = "1",
                ZipMin = 68119,
                ZipMax = 68120
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "ID",
                ZipMin = 83201,
                ZipMax = 83876
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CA",
                ZipMin = 90001,
                ZipMax = 96162
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "IL",
                ZipMin = 60001,
                ZipMax = 62999
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "IN",
                ZipMin = 46001,
                ZipMax = 47997
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "KS",
                ZipMin = 66002,
                ZipMax = 67954
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "KY",
                ZipMin = 40003,
                ZipMax = 42788
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "LA",
                ZipMin = 70001,
                ZipMax = 71232
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "LA",
                SubArea = "1",
                ZipMin = 71234,
                ZipMax = 71497
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "CA",
                ZipMin = 90001,
                ZipMax = 96162
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MA",
                SubArea = "Andover",
                ZipMin = 5501,
                ZipMax = 5544
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MD",
                ZipMin = 20331,
                ZipMax = 20331
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MD",
                ZipMin = 20335,
                ZipMax = 20797
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MD",
                ZipMin = 20812,
                ZipMax = 21930
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "ME",
                ZipMin = 3901,
                ZipMax = 4992
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MI",
                ZipMin = 48001,
                ZipMax = 49971
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MN",
                ZipMin = 55001,
                ZipMax = 56763
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MO",
                ZipMin = 63001,
                ZipMax = 65899
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MS",
                ZipMin = 38601,
                ZipMax = 39776
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MS",
                SubArea = "Warren",
                ZipMin = 71233,
                ZipMax = 71233
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "MT",
                ZipMin = 59001,
                ZipMax = 59937
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NC",
                ZipMin = 27006,
                ZipMax = 28909
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "ND",
                ZipMin = 58001,
                ZipMax = 58856
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NE",
                ZipMin = 68001,
                ZipMax = 68118
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NE",
                ZipMin = 68122,
                ZipMax = 69367
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NH",
                ZipMin = 3031,
                ZipMax = 3897
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NJ",
                ZipMin = 7001,
                ZipMax = 8989
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NM",
                ZipMin = 87001,
                ZipMax = 88441
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NV",
                ZipMin = 88901,
                ZipMax = 89883
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NY",
                SubArea = "Fishers Is",
                ZipMin = 6390,
                ZipMax = 6390
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "NY",
                ZipMin = 10001,
                ZipMax = 14975
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "OH",
                ZipMin = 43001,
                ZipMax = 45999
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "OK",
                ZipMin = 73001,
                ZipMax = 73199
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "OK",
                ZipMin = 73401,
                ZipMax = 74966
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "OR",
                ZipMin = 97001,
                ZipMax = 97920
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "PA",
                ZipMin = 15001,
                ZipMax = 19640
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "PR",
                ZipMin = 0,
                ZipMax = 0
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "RI",
                ZipMin = 2801,
                ZipMax = 2940
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "SC",
                ZipMin = 29001,
                ZipMax = 29948
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "SD",
                ZipMin = 57001,
                ZipMax = 57799
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "TN",
                ZipMin = 37010,
                ZipMax = 38589
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "TX",
                SubArea = "Austin", 
                ZipMin = 73301,
                ZipMax = 73301
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "TX",
                ZipMin = 75001,
                ZipMax = 75501
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "TX",
                ZipMin = 75503,
                ZipMax = 79999
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "TX",
                SubArea = "El Paso",
                ZipMin = 88510,
                ZipMax = 88589
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "UT",
                ZipMin = 84001,
                ZipMax = 84784
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VA",
                ZipMin = 20040,
                ZipMax = 20041
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VA",
                ZipMin = 20040,
                ZipMax = 20167
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VA",
                ZipMin = 20042,
                ZipMax = 20042
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VA",
                ZipMin = 22001,
                ZipMax = 24658
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VT",
                ZipMin = 5001,
                ZipMax = 5495
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "VT",
                ZipMin = 5601,
                ZipMax = 5907
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "WA",
                ZipMin = 98001,
                ZipMax = 99403
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "WI",
                ZipMin = 53001,
                ZipMax = 54990
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "WV",
                ZipMin = 24701,
                ZipMax = 26886
            });
            ZipCodes.Add(new ZipCodeLookup
            {
                State = "WY",
                ZipMin = 82001,
                ZipMax = 83128
            });
        }

        static private void AddDeliveries()
        {
            // delivery numbers are from https://ir.tesla.com/#quarterly-disclosure
            var key = "2019.Q4";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 39934, // total nbr of CY reservations in the Tracker
                    ModelSX = 0
                }
            );
            key = "2020.Q1";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 76200,
                    ModelSX = 12200
                }
            );
            key = "2020.Q2";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 80050,
                    ModelSX = 10600
                }
            );
            key = "2020.Q3";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 124100,
                    ModelSX = 15200
                }
            );
            key = "2020.Q4";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 161650,
                    ModelSX = 16097
                }
            );

            key = "2021.Q1";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 182780,
                    ModelSX = 2020
                }
            );
            key = "2021.Q2";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 204081,
                    ModelSX = 2340
                }
            );
            key = "2021.Q3";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 228882,
                    ModelSX = 8941
                }
            );
            key = "2021.Q4";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 296850,
                    ModelSX = 11750
                }
            );

            key = "2022.Q1";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 295324,
                    ModelSX = 14724
                }
            );
            key = "2022.Q2";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 238533,
                    ModelSX = 16162
                }
            );
            key = "2022.Q3";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 325158,
                    ModelSX = 18672
                }
            );
            key = "2022.Q4";
            DeliversPerQuarter.Add(key,
                new Deliveries
                {
                    Model3Y = 388131,
                    ModelSX = 17147
                }
            );


        }
    }

    internal enum TrimEnum
    {
        NotSet = 0,
        Cybertruck,
        DualMotor,
        Model3,
        ModelS,
        ModelX,
        ModelY,
        Powerwall,
        Roadster,
        Quad,
        Semi,
        SingleMotor,
        Solar,
        TriMotor
    }

    internal class ZipCodeLookup
    {
        internal string State = string.Empty;
        internal string SubArea = string.Empty;
        internal int ZipMin;
        internal int ZipMax;
    }

    internal class Deliveries
    {
        internal int Year;
        internal int Quarter;
        internal int ModelSX;
        internal int Model3Y;
        internal int TotalDeliveries
        {
            get
            {
                return this.Model3Y + this.ModelSX;
            }
        }
    }

}

/*
 * 
 6	Alaska	AK	99501	99950
5	Alabama	AL	35004	36925
69	Arkansas	AR	71601	72959
8	Arkansas (Texarkana)	AR	75502	75502
7	Arizona	AZ	85001	86556
9	California	CA	90001	96162
10	Colorado	CO	80001	81658
11	Connecticut	CT	6001	6389
73	Connecticut	CT	6401	6928
13	Dist of Columbia	DC	20001	20039
74	Dist of Columbia	DC	20042	20599
63	Dist of Columbia	DC	20799	20799
12	Delaware	DE	19701	19980
14	Florida	FL	32004	34997
15	Georgia	GA	30001	31999
65	Georga (Atlanta)	GA	39901	39901
16	Hawaii	HI	96701	96898
20	Iowa	IA	50001	52809
66	Iowa (OMAHA)	IA	68119	68120
17	Idaho	ID	83201	83876
18	Illinois	IL	60001	62999
19	Indiana	IN	46001	47997
21	Kansas	KS	66002	67954
22	Kentucky	KY	40003	42788
68	Louisiana	LA	70001	71232
23	Louisiana	LA	71234	71497
26	Massachusetts	MA	1001	2791
58	Massachusetts (Andover)	MA	5501	5544
77	Maryland	MD	20331	20331
78	Maryland	MD	20335	20797
25	Maryland	MD	20812	21930
24	Maine	ME	3901	4992
27	Michigan	MI	48001	49971
28	Minnesota	MN	55001	56763
57	kc96 DataMO	MO	63001	65899
29	Mississippi	MS	38601	39776
59	Mississippi(Warren)	MS	71233	71233
31	Montana	MT	59001	59937
38	North Carolina	NC	27006	28909
39	North Dakota	ND	58001	58856
67	Nebraska	NE	68001	68118
32	Nebraska	NE	68122	69367
34	New Hampshire	NH	3031	3897
35	New Jersey	NJ	7001	8989
36	New Mexico	NM	87001	88441
33	Nevada	NV	88901	89883
60	New York (Fishers Is)	NY	6390	6390
37	New York	NY	10001	14975
40	Ohio	OH	43001	45999
70	Oklahoma	OK	73001	73199
41	Oklahoma	OK	73401	74966
42	Oregon	OR	97001	97920
43	Pennsylvania	PA	15001	19640
44	Puerto Rico	PR	0	0
45	Rhode Island	RI	2801	2940
46	South Carolina	SC	29001	29948
47	South Dakota	SD	57001	57799
48	Tennessee	TN	37010	38589
49	Texas (Austin)	TX	73301	73301
71	Texas	TX	75001	75501
72	Texas	TX	75503	79999
61	Texas (El Paso)	TX	88510	88589
50	Utah	UT	84001	84784
64	Virginia	VA	20040	20041
75	Virginia	VA	20040	20167
76	Virginia	VA	20042	20042
52	Virginia	VA	22001	24658
51	Vermont	VT	5001	5495
62	Vermont	VT	5601	5907
53	Washington	WA	98001	99403
55	Wisconsin	WI	53001	54990
54	West Virginia	WV	24701	26886
56	Wyoming	WY	82001	83128

 * */

