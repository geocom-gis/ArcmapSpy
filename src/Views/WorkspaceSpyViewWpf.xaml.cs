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

namespace ArcmapSpy.Views
{
    /// <summary>
    /// Interaction logic for WorkspaceSpyViewWpf.xaml
    /// </summary>
    public partial class WorkspaceSpyViewWpf : UserControl
    {
        public WorkspaceSpyViewWpf()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            WorkspacePath.Focus();
        }
    }
}
