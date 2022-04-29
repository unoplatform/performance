using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Benchmarks.WinUI.Shared.Controls;

namespace Benchmarks.WinUI
{
    public sealed partial class MainPage : Page
    {
        private UserControl _asyncUIBenchmarkControl = new AsyncUIBenchmarkControl();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            var type = ((RadioButton)sender).Content;

            switch (type)
            {
                case "Async UI Benchmarks":
                    BenchmarkControl.Content = _asyncUIBenchmarkControl;
                    break;
                default:
                    BenchmarkControl.Content = null;
                    break;
            }
        }
    }
}
