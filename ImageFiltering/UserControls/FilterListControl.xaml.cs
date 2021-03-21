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
using System.Collections.ObjectModel;

namespace ImageFiltering.UserControls
{
    /// <summary>
    /// Interaction logic for FilterListControl.xaml
    /// </summary>
    public partial class FilterListControl : UserControl
    {
        MainWindow ParentWindow;
        ObservableCollection<ImageFilter> FilterList;
        public FilterListControl(MainWindow ParentWindow, ObservableCollection<ImageFilter> FilterList)
        {
            InitializeComponent();
            this.ParentWindow = ParentWindow;
            this.FilterList = FilterList;

            FilterListView.ItemsSource = FilterList;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(FilterListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("FilterType");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void EditFilterButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewFilterButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteFilterButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void DuplicateFilterButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ApplyFilterButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
