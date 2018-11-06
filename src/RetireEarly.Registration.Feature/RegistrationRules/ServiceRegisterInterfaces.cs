using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RetireEarly.Registration.Feature.RegistrationRules
{
    /// <summary>
    /// registeration rule for interfaces
    /// </summary>
    public class ServiceRegisterInterfaces : ServiceRegistrationRule
    {
        /// <inheritdoc />
        public override IEnumerable<ServiceRegistrationItem> BuildServiceRegistrations(Type type,
            IEnumerable<Assembly> assembliesScanned)
        {
            if (type.IsGenericTypeDefinition ||
                type.IsAbstract)
            {
                return new ServiceRegistrationItem[0];
            }

            var assembliesScannedHashSet = new HashSet<Assembly>(assembliesScanned);
            var items = type.GetInterfaces()
                .Where(y => assembliesScannedHashSet.Contains(y.Assembly))
                .Select(y => new ServiceRegistrationItem(
                    service: y,
                    implementation: type
                ))
                .ToList();

            return items;
        }
    }
}
