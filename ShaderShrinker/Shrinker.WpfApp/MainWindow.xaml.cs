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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Shrinker.WpfApp.Properties;

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

        private void OnOpenProjectPage(object sender, RequestNavigateEventArgs e) =>
            Process.Start("explorer.exe", e.Uri.AbsoluteUri);

        private void OnMaxRadioButtonLoaded(object sender, RoutedEventArgs e) => InitRadioButton(sender, "Max");
        private void OnMinRadioButtonLoaded(object sender, RoutedEventArgs e) => InitRadioButton(sender, "Min");
        private void OnCustomRadioButtonLoaded(object sender, RoutedEventArgs e) => InitRadioButton(sender, "Custom");

        private static void InitRadioButton(object sender, string level)
        {
            var button = (RadioButton)sender;
            button.IsChecked = Settings.Default.DefaultLevel == level;
            button.Click += (o, args) =>
            {
                Settings.Default.DefaultLevel = level;
                button.IsChecked = true;
            };
        }
    }
}
