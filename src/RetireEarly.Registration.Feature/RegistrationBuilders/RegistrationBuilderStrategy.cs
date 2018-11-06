using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using RetireEarly.Registration.Feature.RegistrationRules;

namespace RetireEarly.Registration.Feature.RegistrationBuilders
{
    /// <summary>
    /// define registration builder
    /// </summary>
    public abstract class RegistrationBuilderStrategy
    {
        /// <summary>
        /// Adds the required registrations to IServiceCollection
        /// </summary>
        public abstract void BuildRegistration(IEnumerable<ServiceRegistrationItem> serviceRegistrations, IServiceCollection serviceCollection);
    }
}