using RpiHelpers.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers.Services
{
    internal class RpiConfigService
    {
        private RpiConfig _config;

        public event EventHandler<RpiConfig> OnConfigChanged;

        public RpiConfig RpiConfig => _config;

        public void ChangeConfig(string name = null, string address = null)
        {
            var config = _config.With(name, address);
            _config = config;
            OnConfigChanged?.Invoke(this, config);
        }
    }
}
