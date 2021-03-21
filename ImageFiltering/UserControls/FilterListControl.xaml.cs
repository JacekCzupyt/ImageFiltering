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

        private void ApplyFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (FilterListView.SelectedItem != null)
            {
                if (FilterListView.SelectedItem is ImageFilter)
                {
                    ParentWindow.ApplyFilter(FilterListView.SelectedItem as ImageFilter);
                }
                else
                {
                    throw new ArgumentException("Item in filter list is not a filter");
                }
            }
        }

        private void DeleteFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (FilterListView.SelectedItem != null)
            {
                if (FilterListView.SelectedItem is ImageFilter)
                {
                    FilterList.Remove(FilterListView.SelectedItem as ImageFilter);
                }
                else
                {
                    throw new ArgumentException("Item in filter list is not a filter");
                }
            }
        }

        //only works on convolution filters
        private void DuplicateFilterButtonClick(object sender, RoutedEventArgs e)
        {

            if (FilterListView.SelectedItem != null)
            {
                if (FilterListView.SelectedItem is ConvolutionFilter)
                {
                    FilterList.Add((FilterListView.SelectedItem as ConvolutionFilter).Clone() as ConvolutionFilter);
                    FilterListView.SelectedItem = FilterList.Last();
                }
            }
        }

        private void EditFilterButtonClick(object sender, RoutedEventArgs e)
        {
            if (FilterListView.SelectedItem != null)
            {
                if (FilterListView.SelectedItem is ImageFilter)
                {
                    ParentWindow.SwitchToEditFilter(FilterListView.SelectedItem as ImageFilter);
                }
                else
                {
                    throw new ArgumentException("Item in filter list is not a filter");
                }
            }
        }

        private void AddNewFilterButtonClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
            //FilterListView.SelectedItem = null;
            //EditedConvolutionFilter = ConvolutionFilter.Blur(3);//arbitrary default filter
            //EditedConvolutionFilter.FilterName = "New Filter";

            //FilterEditPanel.DataContext = EditedConvolutionFilter;
            //MatrixWidthTextBox.Text = EditedConvolutionFilter.Matrix.Width.ToString();
            //MatrixHeightTextBox.Text = EditedConvolutionFilter.Matrix.Height.ToString();
            //ApplyDimensionsButtonClick(null, null);

            //FilterListGrid.Visibility = Visibility.Collapsed;
            //FilterEditPanel.Visibility = Visibility.Visible;
        }

        public void InsertIntoList(ImageFilter filter)
        {
            if (FilterListView.SelectedItem == null)
            {
                FilterList.Add(filter);
            }
            else
            {
                FilterList[FilterList.IndexOf(FilterListView.SelectedItem as ImageFilter)] = filter;
            }
            FilterListView.SelectedItem = filter;
        }
    }
}
