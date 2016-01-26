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
using System.ComponentModel;

/*
 * Declare two global variables for for bitmap images, and two global variables for lists<string> that will contain hashcode data on each image
 * Create event handlers for buttons that upload images and assign them to the image controllers
 * Create GetPixelStrings class that takes a bitmap as an argument and returns a list of strings that contain hashcode
 * Create CheckIfMatch class that takes two list<String> and compares them to see if one is the subset of the other and returns a string that says if it's subset or not
 * Create a delegate that will be called when the progress bar is updated , the delegate will be called in classes GetPixel string and CheckIfMatch
 * And finally create a different thread for calculations to make the programm responsive even while it's gathering and calculating data
 */

namespace ImageComparer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public delegate void UpdateProgressDelegate();
    public partial class MainWindow : Window
    {
        BitmapImage image1, image2;
        List<string> images;//this lists will be needed in background worker
        OpenFileDialog dialog;
        Nullable<bool> result;
        CheckIfMatch check;
        GetPixelStrings getPixel;
        List<string> hashcode1, hashcode2;
        public UpdateProgressDelegate UpdateProgress;
        public MainWindow()
        {
            InitializeComponent();
            UpdateProgress = UpdateProgressBar;//this delegate will be called to update progress bar
            image1 = new BitmapImage();
            image2 = new BitmapImage();
            images = new List<string>();
            dialog = new OpenFileDialog();
            dialog.Filter = "JPEG Files (*.jpg)|*.jpg";
            check = new CheckIfMatch(UpdateProgress);
            getPixel = new GetPixelStrings(UpdateProgress);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LoadImage(ref image1,ref ImageControl1);
            CheckImages();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LoadImage(ref image2,ref ImageControl2);
            CheckImages();
        }

        public void LoadImage(ref BitmapImage image,ref Image ImageControl)
        {//Loads images from the path defined by user and displays them using Image Controls
            result = dialog.ShowDialog();
            if (result == true)
            {
                BitmapImage imagetemp = new BitmapImage();
                imagetemp.BeginInit();
                Uri path = new Uri(dialog.FileName);
                imagetemp.UriSource = path;
                imagetemp.EndInit();
                ImageControl.Source = imagetemp;
                image = imagetemp;
                images.Add(imagetemp.ToString());
            }
        }

        public void CheckImages()
        {//Checks if Both bitmap images have been initialized , if true enables compare button
            if ((image1.UriSource) != null && (image2.UriSource) != null)
            {
                CompareButton.IsEnabled = true;
            }
        }

        private void CompareButton_Click(object sender, RoutedEventArgs e)
        {
            //the comparison of two images is accomplished using Compare method which is called asynchroniously using background worker
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync(images);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //once the comparison is completed all the elelments and controls are asigned their default value again
            ProgressBar.Value = 0;
            image1.UriSource = null;
            image2.UriSource = null;
            CompareButton.IsEnabled = false;
            images.Clear();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Creates two instances of bitmapImage class and passes them as arguments to Compare method , 
            //the reason I didn't just passed image1 and image2 as arguments
            //is because they were instantiated in the main thread and passing them would cause exception

            List<string> strings = (List<string>)e.Argument;
            string string1 = strings[0];
            string string2 = strings[1];
            BitmapImage image1 = new BitmapImage();
            image1.BeginInit();
            image1.UriSource = new Uri(string1);
            image1.EndInit();

            BitmapImage image2 = new BitmapImage();
            image2.BeginInit();
            image2.UriSource = new Uri(string2);
            image2.EndInit();
            Compare(image1, image2);
        }



        private void UpdateProgressBar()
        {//this delegate updates progress bar
            Action a = () =>
            {
                if (!ProgressBar.IsVisible)
                {
                    ProgressBar.Visibility = Visibility.Visible;
                }
                if (ProgressBar.Maximum == 0)
                {
                    ProgressBar.Maximum = (image1.PixelHeight + image2.PixelHeight);
                }
                ProgressBar.Value += 18;
            };
            Dispatcher.Invoke(a);
        }


        private void Compare(BitmapImage image1,BitmapImage image2)
        {
            //gets a hashcode using getpixel string instance and checks if image is cropped using CompareStrings method of check instance

                 hashcode1 = getPixel.GetHashCode(image1);
                 hashcode2 = getPixel.GetHashCode(image2);
                 bool match = check.CompareStrings(hashcode2, hashcode1);
                 if (match)
                 {
                     MessageBox.Show("The image is Cropped");
                 }
                 else
                 {
                     MessageBox.Show("These are two different images");
                 }
            
           
        }
    }
}
