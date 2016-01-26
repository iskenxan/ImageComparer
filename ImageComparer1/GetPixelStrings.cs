using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;
namespace ImageComparer1
{
    class GetPixelStrings
    {
        UpdateProgressDelegate UpdatePRogress;//Calls delegate that updates ProgressBar
        public GetPixelStrings(UpdateProgressDelegate UpdateProgressBar)
        {
            UpdatePRogress = UpdateProgressBar;

        }
        public List<string> GetHashCode(BitmapImage bitmap)
        {//takes a bitmap and translates it into the hashcode list
            List<string> hashCode= new List<string>();

            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            for (int i = 0; i < bitmap.PixelHeight; i++)//divides an image into rows 
            {
                string row="";
                for (int x = 0; x < bitmap.PixelWidth; x++)//iterates through each pixel in the row
                {
                    byte[] pixel = new byte[bitmap.PixelHeight];//holds color values of a single pixel
                    bitmap.CopyPixels(new Int32Rect(x, i, 1, 1), pixel, stride, 0);//assigns color values of a single pixel to the pixel array
                    Color singlePixel = new Color();//creates new color objects and assigns the color values found in pixel array to it
                    singlePixel.B = pixel[0];
                    singlePixel.G = pixel[1];
                    singlePixel.R = pixel[2];
                    singlePixel.A = pixel[3];
                    row += singlePixel.GetHashCode().ToString();//converst the color value into the hashcode and converts it to the string
                }
                hashCode.Add(row);
                if (i % 20 == 0)//Updates programm only after each tenth iteration, this makes program run a bit faster
                UpdatePRogress();
            }
            return hashCode;
        }
    }

}
