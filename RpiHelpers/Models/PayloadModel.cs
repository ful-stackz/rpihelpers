using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Models
{
    internal abstract class PayloadModel
    {
        protected PayloadModel(string fullPath, string name)
        {
            FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string FullPath { get; }
        public string Name { get; }
        public abstract bool IsDirectory { get; }
        public abstract bool IsFile { get; }
    }
}
