using System;
using System.Collections.Generic;
using System.Reflection;

namespace RetireEarly.Registration.Feature.RegistrationRules
{
    /// <summary>
    /// define service registration rule
    /// </summary>
    public abstract class ServiceRegistrationRule
    {
        /// <summary>
        /// determine registrations based on type
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="assembliesScanned">assemblies scanned for registrations</param>
        /// <returns>enumeration of registrations</returns>
        public abstract IEnumerable<ServiceRegistrationItem> BuildServiceRegistrations(Type type, IEnumerable<Assembly> assembliesScanned);
    }
}