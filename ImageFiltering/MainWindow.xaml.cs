﻿using System;
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

namespace ImageFiltering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeFilterList();

            InitializeFilterEditPanel();



            //MatrixWidthTextBox.DataContext = testContainer;
            //MatrixHeightTextBox.DataContext = testContainer;
            //ApplyDimensionsButton.DataContext = testContainer;
            //temp
            //testContainer c = new testContainer();
        }

    }
}