using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Services
{
    internal class DropDataService
    {
        public event EventHandler<DropDataEventArgs> OnDrop;

        public void Drop(string[] fileNames, bool isDirectory) =>
            OnDrop?.Invoke(this, new DropDataEventArgs(fileNames, isDirectory));
    }
}
