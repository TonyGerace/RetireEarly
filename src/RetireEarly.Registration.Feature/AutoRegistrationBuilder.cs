using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RetireEarly.Registration.Feature.RegistrationBuilders;
using RetireEarly.Registration.Feature.RegistrationRules;

namespace RetireEarly.Registration.Feature
{
    /// <summary>
    /// used to support fluent api / builder pattern with assembly manifest
    /// </summary>
    public sealed class AutoRegistrationBuilder : IAssemblyRegistrationBuilder
    {
        private readonly HashSet<Assembly> _autoRegistrationAssemblies = new HashSet<Assembly>();
        private readonly IServiceCollection _serviceCollection;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="serviceCollection">service collection</param>
        public AutoRegistrationBuilder(IServiceCollection serviceCollection)
        {
            _serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        }

        /// <summary>
        /// add assembly to manifest
        /// </summary>
        /// <param name="assembly">assembly to add to manifest</param>
        public IAssemblyRegistrationBuilder IncludeAssembly(Assembly assembly)
        {
            if (!_autoRegistrationAssemblies.Contains(assembly))
            {
                _autoRegistrationAssemblies.Add(assembly);
            }

            return this;
        }

        /// <summary>
        /// Fills the IServiceCollection with all required registrations
        /// </summary>
        public void Fill()
        {
            IEnumerable<ServiceRegistrationRule> registrationRules = new ServiceRegistrationRule[]
            {
                new ServiceRegisterInterfaces(),
            };
            IEnumerable<RegistrationBuilderStrategy> registrationBuilders = new RegistrationBuilderStrategy[]
            {
                new TransientRegistrationBuilderStrategy(),
                new SingletonRegistrationBuilderStrategy(),
                new EnumerableRegistrationBuilderStrategy()
            };

            var customRegistrationTypes = _autoRegistrationAssemblies
                .SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsSubclassOf(typeof(CustomRegistration)))
                .ToList();

            var customRegistrations = customRegistrationTypes
                .Select(Activator.CreateInstance)
                .Cast<CustomRegistration>()
                .ToList();

            var excludeAssemblies = new HashSet<Assembly>(
                customRegistrations
                    .SelectMany(x => x.AssembliesToIngnoreForAutoRegistrations)
                    .Distinct()
            );

            var scanAssemblies = _autoRegistrationAssemblies
                .Union(customRegistrations.SelectMany(x => x.AssembliesToScanAutoRegistrations))
                .Where(x => !excludeAssemblies.Contains(x))
                .ToList();

            var registrations =
                customRegistrations
                    .SelectMany(x => x.BuildCustomRegistrations())
                    .Select(x => new ServiceRegistrationItem(x.Service, x.Implementation))
                    .Concat(
                        scanAssemblies
                            .SelectMany(x => x.ExportedTypes)
                            .SelectMany(x => customRegistrations
                                .SelectMany(y => y.BuildCustomRegistrationsBasedOnScannedType(x))
                            )
                            .Select(x => new ServiceRegistrationItem(x.Service, x.Implementation))
                    )
                    .Concat(
                        scanAssemblies
                            .SelectMany(x => x.ExportedTypes)
                            .SelectMany(x => registrationRules.SelectMany(y => y.BuildServiceRegistrations(x, scanAssemblies)))
                    )
                    .ToList();


            foreach (var registrationBuilder in registrationBuilders)
            {
                registrationBuilder.BuildRegistration(registrations, _serviceCollection);
            }
        }
    }
}