using System;
using System.Collections.Generic;
using System.Text;

namespace RpiHelpers
{
    internal static class IoT
    {
        private static IDictionary<string, object> _registrations = new Dictionary<string, object>();

        public static void Register<T>(T instance) =>
            _registrations.Add(typeof(T).FullName, instance);

        public static T Get<T>() =>
            _registrations.TryGetValue(typeof(T).FullName, out var instance)
            ? (T)instance
            : default;
    }
}
