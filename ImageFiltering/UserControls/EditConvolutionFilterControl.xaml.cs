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
    /// Interaction logic for EditConvolutionFilterControl.xaml
    /// </summary>
    public partial class EditConvolutionFilterControl : UserControl
    {
        MainWindow ParentWindow;

        const int MaxMatrixGridSize = 9;
        ConvolutionFilter EditedConvolutionFilter;
        TextBox[,] MatrixTextBoxes = new TextBox[MaxMatrixGridSize, MaxMatrixGridSize];

        public EditConvolutionFilterControl(MainWindow ParentWindow, ConvolutionFilter EditedConvolutionFilter)
        {
            InitializeComponent();
            this.ParentWindow = ParentWindow;
            this.EditedConvolutionFilter = EditedConvolutionFilter;

            this.DataContext = EditedConvolutionFilter;

            //initialize the grid representing the matrix
            for (int i = 0; i < MaxMatrixGridSize; i++)
            {
                for (int j = 0; j < MaxMatrixGridSize; j++)
                {
                    MatrixTextBoxes[i, j] = new TextBox();
                    MatrixTextBoxes[i, j].TextAlignment = TextAlignment.Center;
                    MatrixTextBoxes[i, j].Height = 20;
                    MatrixTextBoxes[i, j].Width = 20;
                    MatrixTextBoxes[i, j].SetValue(Grid.ColumnProperty, i);
                    MatrixTextBoxes[i, j].SetValue(Grid.RowProperty, j);
                    MatrixTextBoxes[i, j].Visibility = Visibility.Collapsed;
                    EditConvolutionMatrixGrid.Children.Add(MatrixTextBoxes[i, j]);
                    Binding binding = new Binding($"Matrix[{i},{j}]");
                    binding.Mode = BindingMode.TwoWay;
                    MatrixTextBoxes[i, j].SetBinding(TextBox.TextProperty, binding);
                }
            }

            MatrixWidthTextBox.Text = EditedConvolutionFilter.Matrix.Width.ToString();
            MatrixHeightTextBox.Text = EditedConvolutionFilter.Matrix.Height.ToString();
            ApplyDimensionsButtonClick(null, null);
        }

        private void ApplyDimensionsButtonClick(object sender, RoutedEventArgs e)
        {
            int NewWidth, NewHeight;
            try
            {
                NewWidth = int.Parse(MatrixWidthTextBox.Text);
                NewHeight = int.Parse(MatrixHeightTextBox.Text);
            }
            catch
            {
                MatrixWidthTextBox.Text = EditedConvolutionFilter.Matrix.Width.ToString();
                MatrixHeightTextBox.Text = EditedConvolutionFilter.Matrix.Height.ToString();
                return;
            }

            NewWidth = Math.Max(1, Math.Min(NewWidth, MaxMatrixGridSize));
            NewHeight = Math.Max(1, Math.Min(NewHeight, MaxMatrixGridSize));
            MatrixWidthTextBox.Text = NewWidth.ToString();
            MatrixHeightTextBox.Text = NewHeight.ToString();

            if (NewWidth != EditedConvolutionFilter.Matrix.Width || NewHeight != EditedConvolutionFilter.Matrix.Height)
            {
                ConvolutionMatrix m = EditedConvolutionFilter.Matrix;
                int[,] newValues = new int[NewWidth, NewHeight];
                for (int i = 0; i < NewWidth; i++)
                {
                    for (int j = 0; j < NewHeight; j++)
                    {
                        //ConvolutionMatrix[i, j] returns 0 if i, j is out of range, so this works correctly
                        newValues[i, j] = m[i, j];
                    }
                }
                EditedConvolutionFilter.Matrix = new ConvolutionMatrix(newValues, m.Divisor, m.Offset, (m.AnchorX, m.AnchorY));

                for (int i = 0; i < NewWidth; i++)
                    for (int j = 0; j < NewHeight; j++)
                        MatrixTextBoxes[i, j].GetBindingExpression(TextBox.TextProperty).UpdateTarget();

                DivisorTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                OffsetTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                AnchorXTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                AnchorYTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            }

            EditConvolutionMatrixGrid.ColumnDefinitions.Clear();
            EditConvolutionMatrixGrid.RowDefinitions.Clear();

            for (int i = 0; i < NewWidth; i++)
                EditConvolutionMatrixGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < NewHeight; i++)
                EditConvolutionMatrixGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    MatrixTextBoxes[i, j].Visibility = (i < NewWidth && j < NewHeight) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private void ComputeDivisorButtonClick(object sender, RoutedEventArgs e)
        {
            EditedConvolutionFilter.Matrix.ComputeDivisor();
            DivisorTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void CenterAnchorButtonClick(object sender, RoutedEventArgs e)
        {
            EditedConvolutionFilter.Matrix.AnchorX = EditedConvolutionFilter.Matrix.Width / 2;
            EditedConvolutionFilter.Matrix.AnchorY = EditedConvolutionFilter.Matrix.Height / 2;
            AnchorXTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            AnchorYTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }

        private void SaveFilterButtonClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.FilterListPanel.InsertIntoList(EditedConvolutionFilter);
            ParentWindow.SwitchToListView();
        }

        private void CancelFilterButtonClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.SwitchToListView();
        }
    }
}
