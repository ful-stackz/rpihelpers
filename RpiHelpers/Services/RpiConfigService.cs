using RpiHelpers.Configuration;
using System;

namespace RpiHelpers.Services
{
    internal class RpiConfigService
    {
        private RpiConfig _config;

        /// <summary>
        /// Raised when the Raspberry Pi configuration has been changed.
        /// </summary>
        public event EventHandler<RpiConfig> OnConfigChanged;

        /// <summary>
        /// Gets the current active Raspberry Pi configuration.
        /// </summary>
        public RpiConfig RpiConfig => _config;

        /// <summary>
        /// Changes the current configuration with the specified <paramref name="name"/>
        /// and/or <paramref name="address"/>, and raises an event to notify of the change.
        /// </summary>
        /// <param name="name">
        /// [Optional] The new name as which to try reaching the device.
        /// </param>
        /// <param name="address">
        /// [Optional] The new address at which the device can be reached.
        /// </param>
        public void ChangeConfig(string name = null, string address = null)
        {
            var config = _config.With(name, address);
            _config = config;
            OnConfigChanged?.Invoke(this, config);
        }
    }
}
