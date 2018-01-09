using AllProbe1.Droid;
using Android.App;
using Android.Media;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Linq;


namespace AllProbe1.Services
{
    public static class GlobalServices
    {
        private static List<string> severityList = new List<string>
            {
                Android.App.Application.Context.Resources.GetString(Resource.String.severity1),
                Android.App.Application.Context.Resources.GetString(Resource.String.severity2),
                Android.App.Application.Context.Resources.GetString(Resource.String.severity3),
                Android.App.Application.Context.Resources.GetString(Resource.String.severity4),
                Android.App.Application.Context.Resources.GetString(Resource.String.severity5),
            };

        private static Dictionary<string, List<string>> resultsTranslations = null;
        private static void CreateResultsDictionary()
        {
            resultsTranslations.Add("port", new List<string> { "Port Status", "Response Time" });
            resultsTranslations.Add("http", new List<string> { "Response Time", "HTTP check status", "Response Code", "Page Size" });
            resultsTranslations.Add("http-deep", new List<string> { "Response Time", "HTTP check status", "Response Code", "Page Size", "Any Sub Element Response Time", "ny Sub Element Response Code" });
            resultsTranslations.Add("icmp", new List<string> { "Packet Loss", "Round Trip Average" });
            resultsTranslations.Add("discovery-bw", new List<string> { "Traffic In", "Traffic Out" });
            resultsTranslations.Add("discovery-ds", new List<string> { "Free Disk Space", "Used Disk Space", "Total Disk Space", "Free disk space %", "Used disk space %" });
            resultsTranslations.Add("traceroute", new List<string> { "Any Router Hop Response Time", "Destenation/last Hop Response Time" });
            resultsTranslations.Add("snmp", new List<string> { "OID result" });
            resultsTranslations.Add("rbl", new List<string> { "RBL status" });
            resultsTranslations.Add("sql", new List<string> { "SQL Result" });
        }

        public static string GetResultString(string probeType, string jsonArrayResults)
        {
            if (resultsTranslations == null)
            {
                resultsTranslations = new Dictionary<string, List<string>>();
                CreateResultsDictionary();
            }

            jsonArrayResults = jsonArrayResults.Trim().Substring(1, jsonArrayResults.Length - 2);
            string[] resultsArray = jsonArrayResults.Split(',');
            StringBuilder results = new StringBuilder();

            for (int i = 0; i < resultsArray.Length - 1; i++)
            {
                if (resultsTranslations[probeType].Count > i)
                    results.Append("\n\t\t\t" + resultsTranslations[probeType][i] + ": ");
                else
                    continue;
                string result = resultsArray[i + 1];

                switch (probeType)
                {
                    case "rbl":
                        if (result == "0")
                            results.Append("Not listed");
                        else if (result == "1")
                            results.Append("Listed");
                        else
                            results.Append(result);
                        break;
                    case "http":
                        if (i == 0)
                            results.Append(result + " ms");
                        else if (i == 1)
                        {
                            if (result == "1")
                                results.Append("TimeOut");
                            else if (result == "2")
                                results.Append("Response code issue");
                            else if (result == "3")
                                results.Append(" All Ok");
                            else
                                results.Append(result + " - Not OK");
                        }
                        else if (i == 2)
                            results.Append(result);
                        else if (i == 3)
                            results.Append(result + " bytes");
                        else
                            results.Append(result);
                        break;
                    case "http-deep":
                        if (i == 0)
                            results.Append(result + " ms");
                        else if (i == 1)
                        {
                            if (result == "1")
                                results.Append("TimeOut");
                            else if (result == "2")
                                results.Append("Response code issue");
                            else if (result == "3")
                                results.Append(" All Ok");
                            else
                                results.Append(result + " Not OK");
                        }
                        else if (i == 2)
                            results.Append(result);
                        else if (i == 3)
                            results.Append(result + " bytes");
                        else if (i == 4)
                            results.Append(result);
                        else if (i == 5)
                            results.Append(result + "ms");
                        else
                            results.Append(result);
                        break;
                    case "port":
                        if (i == 0)
                        {
                            if (results.Equals("0"))
                                results.Append("Not OK");
                            else
                                results.Append("OK");
                        }
                        else
                            results.Append(result + " ms");
                        break;
                    case "icmp":
                        if (i == 0)
                            results.Append(result + "%");
                        else if (i == 1)
                            results.Append(result + " ms");
                        break;
                    case "discovery-bw":
                        results.Append(result.ToString());
                        //if (i == 0)
                        //    results.Append(result + "%");
                        //else if (i == 1)
                        //    results.Append(result + "ms");
                        break;
                    case "discovery-ds":
                        results.Append(result.ToString());
                        //if (i == 0)
                        //    results.Append(result + "%");
                        //else if (i == 1)
                        //    results.Append(result + "ms");
                        break;
                    case "traceroute":
                        results.Append(result.ToString());
                        if (i == 0)
                            results.Append(result + "ms");
                        else if (i == 1)
                            results.Append(result + "ms");
                        break;
                    case "snmp":
                        results.Append(result.ToString());
                        break;
                    default:
                        results.Append(result);
                        break;
                }
            }

            return results.ToString();
        }

        public static List<string> SeverityList
        {
            get
            {
                return severityList;
            }
        }
        public static bool IsValid(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string GetSevrity(int index)
        {
            return severityList[index];
        }

        public static bool IsSeveritiesFeet(string severity, int index)
        {
            if (index == 0)
                return true;
            else if (index == 1)
                return !severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity1).ToLower());
            else if (index == 2)
                return !severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity1).ToLower()) &&
                       !severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity2).ToLower());
            else if (index == 3)
                return severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity4).ToLower()) ||
                       severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity5).ToLower());
            else if (index == 4)
                return severity.ToLower().Equals(Android.App.Application.Context.Resources.GetString(Resource.String.severity5).ToLower());
            return false;
        }

        public static void SetOrientation(int OrientationToSelect)
        {
            Xamarin.Forms.Application.Current.Properties["userOrientation"] = OrientationToSelect;
        }

        public static int GetOrientation()
        {
            if (Xamarin.Forms.Application.Current.Properties.ContainsKey("userOrientation"))
                return Convert.ToInt32(Xamarin.Forms.Application.Current.Properties["userOrientation"]);
            else
                return 0;
        }
    }
}
