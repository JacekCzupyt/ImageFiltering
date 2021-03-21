using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ImageFiltering.ImageProcessing;
using ImageFiltering.UserControls;

namespace ImageFiltering
{
    partial class MainWindow : Window
    {
        //contians a list of all avalible filters
        ObservableCollection<ImageFilter> FilterList = new ObservableCollection<ImageFilter>();
        FilterListControl FilterListPanel;

        void InitializeFilterList()
        {
            FilterList.Add(MonoFunctionFilter.Identity);

            FilterList.Add(FunctionFilter.Identity);
            FilterList.Add(FunctionFilter.Inversion);

            FilterList.Add(FunctionFilter.BrightnessCorrection(10));
            FilterList.Add(FunctionFilter.BrightnessCorrection(-10));

            FilterList.Add(FunctionFilter.ContrastEnchantment(1.2f));
            FilterList.Add(FunctionFilter.ContrastEnchantment(0.75f));

            FilterList.Add(FunctionFilter.GammaCorrection(1.2f));
            FilterList.Add(FunctionFilter.GammaCorrection(0.75f));

            FilterList.Add(ConvolutionFilter.Blur(3));
            FilterList.Add(ConvolutionFilter.Blur(7));
            FilterList.Add(ConvolutionFilter.GaussSmooth3);
            FilterList.Add(ConvolutionFilter.GaussSmooth5);
            FilterList.Add(ConvolutionFilter.GaussSmooth7);
            FilterList.Add(ConvolutionFilter.HighPassSharpen3());
            FilterList.Add(ConvolutionFilter.MeanRemovalSharpen3());
            FilterList.Add(ConvolutionFilter.SmallHorizontalEdgeDetection(2));
            FilterList.Add(ConvolutionFilter.SmallVerticalEdgeDetection(2));
            FilterList.Add(ConvolutionFilter.LargeHorizontalEdgeDetection());
            FilterList.Add(ConvolutionFilter.LargeVerticalEdgeDetection());
            FilterList.Add(ConvolutionFilter.SmallEdgeDetection(2));
            FilterList.Add(ConvolutionFilter.LargeEdgeDetection());
            FilterList.Add(ConvolutionFilter.EmbossFilterRight);
            FilterList.Add(ConvolutionFilter.EmbossFilterLeft);
            FilterList.Add(ConvolutionFilter.EmbossFilterUp);
            FilterList.Add(ConvolutionFilter.EmbossFilterDown);

            FilterList.Add(new OctreeColorQuantisation(40));
            FilterList.Add(new OctreeColorQuantisation(100));
            FilterList.Add(new OctreeColorQuantisation(300));

            FilterListPanel = new FilterListControl(this, FilterList);
            FilterListPanel.DataContext = FilterList;
            FilterManagementControl.Content = FilterListPanel;
        }

        public void ApplyFilter(ImageFilter filter)
        {
            if(FilteredImage != null)
            {
                filter.Apply(FilteredImage);
                FilteredImageDisplay.Source = BitmapToImageSource(FilteredImage);
            }
        }
    }
}