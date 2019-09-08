using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Configuration
{
    internal struct RpiConfig
    {
        private RpiConfig(string name, string address)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public static RpiConfig Empty =>
            new RpiConfig(string.Empty, string.Empty);

        public static RpiConfig Default =>
            new RpiConfig("pi", "raspberrypi.local");

        public string Name { get; private set; }
        public string Address { get; private set; }
        public string ConnectionString =>
            $"{Name}@{Address}";

        public RpiConfig With(string name = null, string address = null) =>
            new RpiConfig(name ?? Name, address ?? Address);
    }
}
