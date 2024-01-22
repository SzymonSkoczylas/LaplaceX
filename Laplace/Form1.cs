using System.Runtime.InteropServices;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO;

namespace Laplace
{
    public partial class Form1 : Form
    {
        public String imagePath = @"C:\Users\Achim\Desktop\studia\Laplace\Laplace\smallX.bmp";
        public String finalPath = @"C:\Users\Achim\Desktop\studia\Laplace\Laplace\smallO.bmp";
        public int threads = 64;
        Bitmap image;
        Stopwatch stopwatch;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(imagePath);
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool tmp = false;
            try
            {
                image = new Bitmap(imagePath);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {
                image = new Bitmap(imagePath);
                String threadsText = textBox1.Text;
                if (threadsText == "")
                    threadsText = "1";
                threads = Int32.Parse(threadsText);
                byte[] bitmap = new byte[image.Width * image.Height * 3];
                byte[] final = new byte[image.Width * image.Height * 3];
                int pixIndex = 0;
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color pixel = image.GetPixel(j, i);
                        bitmap[pixIndex] = pixel.B;
                        bitmap[pixIndex + 1] = pixel.G;
                        bitmap[pixIndex + 2] = pixel.R;
                        pixIndex += 3;
                    }
                }

                stopwatch = new Stopwatch();
                stopwatch.Start();
                AsyncFunctions.UseAsmAlgorithm(bitmap, threads, final, image.Width, image.Height);
                stopwatch.Stop();
                Bitmap result = (Bitmap)image.Clone();


                int pixIndex2 = 0;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixelColor = new();
                        pixelColor = Color.FromArgb(255, final[pixIndex2 + 2], final[pixIndex2 + 1], final[pixIndex2]);
                        result.SetPixel(x, y, pixelColor);
                        pixIndex2 += 3;
                    }
                }
                pictureBox2.Image = result;
                label1.Text = "Time: " + stopwatch.ElapsedMilliseconds.ToString() + "ms on " + threadsText + " threads";
            }
        }

        private void button2_ClickAsync(object sender, EventArgs e)
        {
            bool tmp = false;
            try
            {
                image = new Bitmap(imagePath);
                tmp = true;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("An error occured");
            }
            if (tmp)
            {
                image = new Bitmap(imagePath);
                String threadsText = textBox1.Text;
                if (threadsText == "")
                    threadsText = "1";
                threads = Int32.Parse(threadsText);
                byte[] bitmap = new byte[image.Width * image.Height * 3];
                byte[] final = new byte[image.Width * image.Height * 3];
                int pixIndex = 0;
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color pixel = image.GetPixel(j, i);
                        bitmap[pixIndex] = pixel.B;
                        bitmap[pixIndex + 1] = pixel.G;
                        bitmap[pixIndex + 2] = pixel.R;
                        pixIndex += 3;
                    }
                }
                stopwatch = new Stopwatch();
                stopwatch.Start();
                AsyncFunctions.UseCppAlgorithm(bitmap, threads, final, image.Width, image.Height);
                stopwatch.Stop();
                Bitmap result = (Bitmap)image.Clone();


                int pixIndex2 = 0;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixelColor = new();
                        pixelColor = Color.FromArgb(255, final[pixIndex2 + 2], final[pixIndex2 + 1], final[pixIndex2]);
                        result.SetPixel(x, y, pixelColor);
                        pixIndex2 += 3;
                    }
                }
                pictureBox2.Image = result;
                label1.Text = "Time: " + stopwatch.ElapsedMilliseconds.ToString() + "ms on " + threadsText + " threads";
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


    }
}