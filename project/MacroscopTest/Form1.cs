using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MacroscopTest
{
    public partial class Form1 : Form
    {
        Annotations annotations;

        public Form1()
        {
            InitializeComponent();

            annotations = new Annotations();
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.png) | *.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Load(dialog.FileName);
            }
        }

        private void loadAnnotationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            annotations.LoadAnnotations();
        }

        private void cropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем загружена ли картинка и аннотации
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Image is not loaded. Please, load an image first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            if (annotations.areas.Count == 0)
            {
                MessageBox.Show("Annotations is not loaded. Please, load the annotations first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Сохраняем вырезанные картинки
            foreach (var item in annotations.areas)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Image Files (*.png) | *.png";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (Bitmap bitmap = new Bitmap(pictureBox1.Image))
                    {
                        Image cropedImage = bitmap.Clone(new Rectangle
                        {
                            X = item.left.X,
                            Y = item.left.Y,
                            Width = item.right.X - item.left.X,
                            Height = item.right.Y - item.left.Y
                        },
                        bitmap.PixelFormat);
                        cropedImage.Save(dialog.FileName);
                    }
                }
            }
        }

        private void greyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем загружена ли картинка
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Image is not loaded. Please, load an image first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            pictureBox1.Image = Greyscale((Bitmap)pictureBox1.Image);
        }

        private Bitmap Greyscale(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    int avg = (color.R + color.G + color.B) / 3;
                    bitmap.SetPixel(i, j, Color.FromArgb(avg, avg, avg));
                }
            }

            return bitmap;
        }

        private Bitmap Flip(Bitmap bitmap)
        {
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);

            return bitmap;
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Image Files (*.png) | *.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(dialog.FileName);
            }
        }

        public static Bitmap Noise(Bitmap bmp, float noisePower = 30)
        {
            var res = (Bitmap)bmp.Clone();
            var rnd = new Random();

            using (var wr = new ImageWrapper(res))
            {
                foreach (var p in wr)
                {
                    var c = wr[p];
                    var noise = (rnd.NextDouble() + rnd.NextDouble() + rnd.NextDouble() + rnd.NextDouble() - 2) * noisePower;
                    wr.SetPixel(p, c.R + noise, c.G + noise, c.B + noise);
                }
            }

            return res;
        }

        public static Bitmap Normalize(Bitmap bitmap, float min, float max)
        {
            float newMin = 1.0f;
            float newMax = 1.0f;

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    float I = color.GetBrightness();
                    float newI = (I - min) * ((newMax - newMin) / (max - min)) + newMin;
                    bitmap.SetPixel(i, j, Color.FromArgb(
                        (int)(color.R * (1 + newI)) > 255 ? 255 : (int)(color.R * (1 + newI)),
                        (int)(color.G * (1 + newI)) > 255 ? 255 : (int)(color.G * (1 + newI)),
                        (int)(color.B * (1 + newI)) > 255 ? 255 : (int)(color.B * (1 + newI))));
                }
            }

            return bitmap;
        }

        private void flipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем загружена ли картинка
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Image is not loaded. Please, load an image first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            pictureBox1.Image = Flip((Bitmap)pictureBox1.Image);
        }

        private void noiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем загружена ли картинка
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Image is not loaded. Please, load an image first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            pictureBox1.Image = Noise((Bitmap)pictureBox1.Image);
        }

        private void normalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Проверяем загружена ли картинка
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("Image is not loaded. Please, load an image first.", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            float min = 1;
            float max = 0;
            for (int i = 0; i < pictureBox1.Image.Width; i++)
            {
                for (int j = 0; j < pictureBox1.Image.Height; j++)
                {
                    var color = ((Bitmap)pictureBox1.Image).GetPixel(i, j);
                    if (min > color.GetBrightness())
                    {
                        min = color.GetBrightness();
                    }
                    if (max < color.GetBrightness())
                    {
                        max = color.GetBrightness();
                    }
                }
            }

            pictureBox1.Image = Normalize((Bitmap)pictureBox1.Image, min, max);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
