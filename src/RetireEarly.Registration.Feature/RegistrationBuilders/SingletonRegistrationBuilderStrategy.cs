using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RetireEarly.Registration.Feature.RegistrationRules;

namespace RetireEarly.Registration.Feature.RegistrationBuilders
{
    /// <summary>
    /// Build transient registrations
    /// </summary>
    public class SingletonRegistrationBuilderStrategy : RegistrationBuilderStrategy
    {
        /// <inheritdoc />
        public override void BuildRegistration(IEnumerable<ServiceRegistrationItem> serviceRegistrations, IServiceCollection serviceCollection)
        {
            var singletons = serviceRegistrations
                .GroupBy(x => x.Service)
                .Where(x => !x.Skip(1).Any())
                .SelectMany(x => x
                    .Select(y => y)
                )
                .Where(y => y.Implementation.GetCustomAttributes().OfType<SingletonAttribute>().Any())
                .ToList();

            foreach (var singleton in singletons)
            {
                serviceCollection.TryAddSingleton(singleton.Service, singleton.Implementation);
            }
        }
    }
}