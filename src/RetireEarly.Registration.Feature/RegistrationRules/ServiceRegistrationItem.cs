using System;

namespace RetireEarly.Registration.Feature.RegistrationRules
{
    /// <summary>
    /// registration item
    /// </summary>
    public class ServiceRegistrationItem
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="service">abstraction</param>
        /// <param name="implementation">implementation</param>
        public ServiceRegistrationItem(Type service, Type implementation)
        {
            Service = service;
            Implementation = implementation;
        }

        /// <summary>
        /// abstraction / service
        /// </summary>
        public Type Service { get; }
        /// <summary>
        /// implementation of abstraction / service
        /// </summary>
        public Type Implementation { get; }
    }
}