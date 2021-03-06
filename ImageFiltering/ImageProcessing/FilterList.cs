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
        public FilterListControl FilterListPanel;

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

            FilterList.Add(new RandomDitheringFilter(2));
            FilterList.Add(new RandomDitheringFilter(4));
            FilterList.Add(new RandomDitheringFilter(8));

            FilterList.Add(new YCbCrFilter(100));


            FilterListPanel = new FilterListControl(this, FilterList);
            FilterListPanel.DataContext = FilterList;
            FilterManagementControl.Content = FilterListPanel;
        }

        public void ApplyFilter(ImageFilter filter)
        {
            if(FilteredImage != null)
            {
                FilteredImage = filter.Apply(FilteredImage);
                FilteredImageDisplay.Source = BitmapToImageSource(FilteredImage);
            }
        }

        public void SwitchToEditFilter(ImageFilter filter)
        {
            if (filter is ConvolutionFilter)
                FilterManagementControl.Content = new EditConvolutionFilterControl(this, (filter as ConvolutionFilter).Clone() as ConvolutionFilter);
            else if (filter is OctreeColorQuantisation)
                FilterManagementControl.Content = new EditOctreeFilterControl(this, (filter as OctreeColorQuantisation).Clone() as OctreeColorQuantisation);
            else if (filter is RandomDitheringFilter)
                FilterManagementControl.Content = new EditRandomDitheringFilter(this, (filter as RandomDitheringFilter).Clone() as RandomDitheringFilter);
        }

        public void SwitchToListView()
        {
            FilterManagementControl.Content = FilterListPanel;
        }

        public void SwitchToNewFilterSelect()
        {
            FilterManagementControl.Content = new SelectNewFilterTypeControl(this);
        }
    }
}