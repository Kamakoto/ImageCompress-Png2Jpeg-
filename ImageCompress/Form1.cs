using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageCompress
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int quality = 50;
            if (int.TryParse(textBox1.Text, out quality))
            {
                string[] files = Directory.GetFiles(Application.StartupPath, "*.png");
                MessageBox.Show($"Pngs Summary:{files.Length}");
                int summary = 0;
                foreach (var file in files)
                {
                    compressImage(file,quality).Save(Application.StartupPath + "/" + Path.GetFileNameWithoutExtension(file) + ".jpg", ImageFormat.Jpeg);
                    summary++;
                    if(checkBox1.Checked)
                    {
                        File.Delete(file);
                    }
                    label1.Text = $"{summary}/{files.Length}";
                }
            }
            else
            {
                MessageBox.Show("ERROR QUALITY");
            }


        }


        private System.Drawing.Image compressImage(string fileName, int newQuality)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(fileName))
            using (System.Drawing.Image memImage = new Bitmap(image, image.Width, image.Height)) //如果需要自定义长宽，请把参数提取至构造方法
            {
                ImageCodecInfo myImageCodecInfo;
                System.Drawing.Imaging.Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");
                myEncoder = System.Drawing.Imaging.Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                MemoryStream memStream = new MemoryStream();
                memImage.Save(memStream, myImageCodecInfo, myEncoderParameters);
                System.Drawing.Image newImage = System.Drawing.Image.FromStream(memStream);
                ImageAttributes imageAttributes = new ImageAttributes();
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.InterpolationMode =
                      System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;  //**
                    g.DrawImage(newImage, new Rectangle(System.Drawing.Point.Empty, newImage.Size), 0, 0,
                      newImage.Width, newImage.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                return newImage;
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }
    }
}
