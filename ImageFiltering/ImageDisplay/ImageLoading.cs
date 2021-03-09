using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageFiltering
{
    partial class MainWindow : Window
    {
        String ImagePath = null;
        Bitmap OriginalImage = null;
        Bitmap FilteredImage = null;

        //select image via drag and drop
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                ImagePath = files[0];
                SetBitmapsFromFileName(files[0]);
            }
        }

        //select image using file dialog
        private void SelectImageButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                SetBitmapsFromFileName(ImagePath);
            }
        }

        private void ResetImageButtonClick(object sender, RoutedEventArgs e)
        {
            FilteredImage = new Bitmap(OriginalImage);
            FilteredImageDisplay.Source = BitmapToImageSource(FilteredImage);
        }

        private void ClearImageButtonClick(object sender, RoutedEventArgs e)
        {
            ImagePath = null;

            OriginalImage = null;
            FilteredImage = null;

            FindImageButton.Visibility = Visibility.Visible;

            OriginalImageDisplay.Source = null;
            FilteredImageDisplay.Source = null;
        }

        //preapare images and bitmaps from filename
        private void SetBitmapsFromFileName(string filename)
        {
            try
            {
                OriginalImage = (Bitmap)Image.FromFile(filename);
                FilteredImage = new Bitmap(OriginalImage);

                FindImageButton.Visibility = Visibility.Collapsed;

                OriginalImageDisplay.Source = BitmapToImageSource(OriginalImage);
                FilteredImageDisplay.Source = BitmapToImageSource(FilteredImage);
            }
            catch (System.NotSupportedException)
            {
                throw new System.NotSupportedException();
            }
        }

        //private void SaveButtonClick(object sender, RoutedEventArgs e)
        //{
        //    BitmapEncoder encoder;
        //    switch (Path.GetExtension(ImagePath))
        //    {
        //        case ".jpg":
        //            encoder = bitmapencoder
        //    }
        //}

        private void SaveAsButtonClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPeg Image|*.jpg|Png Image|*.png";
            dialog.Title = "Save an Image File";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                using (System.IO.FileStream fs =
                    (System.IO.FileStream)dialog.OpenFile())
                {
                    BitmapEncoder encoder;
                    switch (dialog.FilterIndex)
                    {
                        case 1:
                            encoder = new JpegBitmapEncoder();
                            
                            break;
                        case 2:
                            encoder = new PngBitmapEncoder();
                            break;
                        default:
                            throw new Exception();
                    }
                    encoder.Frames.Add(BitmapFrame.Create(BitmapToImageSource(FilteredImage)));
                    encoder.Save(fs);
                }
            }
        }

        //from https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

       
    }
}
