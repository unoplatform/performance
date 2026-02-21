using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Benchmarks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace dopesbench.Components;
public sealed partial class AppBar : UserControl
{
    public AppBar()
    {
        this.InitializeComponent();
    }

    private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {

        if (args.SelectedItem is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "Dopes":
                    ContentFrame.Navigate(typeof(DopesPage));
                    break;
                case "Home":
                    ContentFrame.Navigate(typeof(HomePage));
                    break;
                case "Bench":
                    ContentFrame.Navigate(typeof(MainPage));
                    break;
            }

        }
    }

    public void Navigate(Type pageType, object parameter = null)
    {
        ContentFrame.Navigate(pageType, parameter);
    }
}
