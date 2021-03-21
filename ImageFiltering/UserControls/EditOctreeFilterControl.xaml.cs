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
    /// Interaction logic for EditOctreeFilterControl.xaml
    /// </summary>
    public partial class EditOctreeFilterControl : UserControl
    {
        MainWindow ParentWindow;
        OctreeColorQuantisation filter;

        public EditOctreeFilterControl(MainWindow ParentWindow, OctreeColorQuantisation filter)
        {
            InitializeComponent();
            this.ParentWindow = ParentWindow;
            this.filter = filter;

            this.DataContext = filter;
        }

        private void SaveFilterButtonClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.FilterListPanel.InsertIntoList(filter);
            ParentWindow.SwitchToListView();
        }

        private void CancelFilterButtonClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.SwitchToListView();
        }
    }
}
