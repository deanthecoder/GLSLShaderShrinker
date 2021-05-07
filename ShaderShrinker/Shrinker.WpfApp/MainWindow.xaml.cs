// -----------------------------------------------------------------------
//  <copyright file="MainWindow.xaml.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Windows;

namespace Shrinker.WpfApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var appViewModel = new AppViewModel();
            DataContext = appViewModel;

            Closing += (sender, args) => appViewModel.SaveCustomOptions();
            appViewModel.GlslLoaded += (sender, args) =>
            {
                importHost.IsOpen = false;

                if (args.originalCode != DiffControl.OldText)
                    DiffControl.OldText = args.originalCode;

                if (args.newCode != DiffControl.NewText)
                    DiffControl.NewText = args.newCode;

                DiffControl.GoTo(0);
            };
        }

        private void OnSettingsButtonClicked(object sender, RoutedEventArgs e) => rootHost.IsOpen = true;
    }
}
