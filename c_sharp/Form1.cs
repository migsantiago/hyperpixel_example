using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GetImageRawBytes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const int EXPECTED_WIDTH = 800;
        const int EXPECTED_HEIGHT = 480;
        const string EXPECTED_OUTPUT_NAME = "rawImage.cpp";

        private void btnConvert_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "*.*";
            var result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                System.Drawing.Bitmap bmp = new Bitmap(openFileDialog.FileName);

                if ((bmp.Width != EXPECTED_WIDTH) || (bmp.Height != EXPECTED_HEIGHT))
                {
                    MessageBox.Show("Unexpected size. Expect 800x480");
                }
                else
                {
                    generateFile(bmp);
                }
            }
        }

        void generateFile(Bitmap bmp)
        {
            FileStream archivoNuevo;
            try
            {
                if (File.Exists(EXPECTED_OUTPUT_NAME))
                {
                    File.Delete(EXPECTED_OUTPUT_NAME);
                }

                archivoNuevo = new FileStream(EXPECTED_OUTPUT_NAME, FileMode.OpenOrCreate, FileAccess.Write);
            }
            catch
            {
                MessageBox.Show("Cannot create file");
                return;
            }

            StreamWriter escribir = new StreamWriter(archivoNuevo, ASCIIEncoding.ASCII);

            escribir.Write("const unsigned int rawImageAARRGGBB[] = \n{\n");

            UInt32[] pixels = new UInt32[EXPECTED_WIDTH * EXPECTED_HEIGHT];

            for (int row = 0; row < EXPECTED_HEIGHT; row++)
            {
                for (int column = 0; column < EXPECTED_WIDTH; column++)
                {
                    UInt32 pixel =
                        (UInt32)(bmp.GetPixel(column, row).R << 16)
                        | (UInt32)(bmp.GetPixel(column, row).G << 8)
                        | (UInt32)(bmp.GetPixel(column, row).B);
                    pixels[(row * EXPECTED_WIDTH) + column] = pixel;
                }
            }

            int count = 0;
            StringBuilder textPixels = new StringBuilder();

            foreach (UInt32 pix in pixels)
            {
                textPixels.Append(String.Format("0x{0:X8}, ", pix));
                ++count;

                if (count == 16)
                {
                    count = 0;
                    textPixels.Append("\n");
                }
            }

            escribir.Write(textPixels);
            escribir.Write("\n};\n");

            escribir.Close();
            archivoNuevo.Close();
        }
    }
}
