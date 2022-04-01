using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks.Maui.Util
{
    internal class EventNotificationHandlerUtil
    {   
        public void ChildAddedHandler(object sender, ElementEventArgs e)
        {
            ItemsCreated++;

            if (ItemsCreated >= MaxItems)
            {
                Tcs.SetResult(true);
            }
        }

        public TaskCompletionSource<bool> Tcs { get; set; }

        public int ItemsCreated { get; private set; }
        public int MaxItems { get; set; } = 100;
    }
}
