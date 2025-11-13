using Azure;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Saplin.xOPS.UI.Misc;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Microsoft.UI;
using Benchmarks.Shared;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace dopesbench
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>`
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void NavigateToDopesPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DopesPage));
        }
    }

}
