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
using ImageFiltering.ImageProcessing;

namespace ImageFiltering.UserControls
{
    /// <summary>
    /// Interaction logic for SelectNewFilterTypeControl.xaml
    /// </summary>
    public partial class SelectNewFilterTypeControl : UserControl
    {
        MainWindow ParentWindow;
        public SelectNewFilterTypeControl(MainWindow ParentWindow)
        {
            InitializeComponent();

            this.ParentWindow = ParentWindow;
            FilterTypeListView.ItemsSource = FilterTypeDictionary.Keys;
        }

        private static Dictionary<string, Func<ImageFilter>> FilterTypeDictionary = new Dictionary<string, Func<ImageFilter>>()
        {
            {"Convolution filter", () => ConvolutionFilter.DefaultTemplate},
            {"Octree color quantizer", () => new OctreeColorQuantisation(100) }
        };

        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            if(FilterTypeListView.SelectedItem != null)
            {
                ParentWindow.SwitchToEditFilter(FilterTypeDictionary[FilterTypeListView.SelectedItem as string]());
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.SwitchToListView();
        }

        private void ListItemClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                SelectButtonClick(sender, e);
        }
    }
}
