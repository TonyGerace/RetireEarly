using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RetireEarly.Registration.Feature.RegistrationRules;

namespace RetireEarly.Registration.Feature.RegistrationBuilders
{
/// <summary>
    /// Build transient registrations
    /// </summary>
    public class EnumerableRegistrationBuilderStrategy : RegistrationBuilderStrategy
    {
        interface IEnumerationFactory
        {
            object BuildEnumeration(IServiceProvider serviceProvider, IEnumerable<Type> types);
        }

        /// <summary>
        /// type parameter is used for registration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        // ReSharper disable once UnusedTypeParameter
        interface ITypedEnumerationFactory<T> : IEnumerationFactory
        {
        }

        class EnumerationFactory<T> : ITypedEnumerationFactory<T>
        {
            Dictionary<Type, T> _singletons;
            readonly object _lock = new object();
            public object BuildEnumeration(IServiceProvider serviceProvider, IEnumerable<Type> types)
            {
                List<T> items = new List<T>();

                lock (_lock)
                {
                    if (_singletons == null)
                    {
                        _singletons = new Dictionary<Type, T>();
                        // ReSharper disable once PossibleMultipleEnumeration
                        foreach (Type singleton in types
                            .Where(x =>
                                x.GetCustomAttributes()
                                    .OfType<SingletonAttribute>()
                                    .Any())
                        )
                        {
                            ConstructorInfo ctor = singleton.GetConstructors().Single();
                            object[] ctorParams = ctor.GetParameters()
                                .Select(x => x.ParameterType)
                                .Select(serviceProvider.GetService)
                                .ToArray();

                            object instance = ctor.Invoke(ctorParams);
                            _singletons.Add(singleton, (T) instance);

                        }
                    }
                }

                // ReSharper disable once PossibleMultipleEnumeration
                foreach (Type t in types)
                {
                    if (!_singletons.TryGetValue(t, out T item))
                    {
                        ConstructorInfo ctor = t.GetConstructors().Single();
                        object[] ctorParams = ctor.GetParameters()
                            .Select(x => x.ParameterType)
                            .Select(serviceProvider.GetService)
                            .ToArray();
                        item = (T) ctor.Invoke(ctorParams);
                    }

                    items.Add(item);
                }

                return items;
            }
        }

        /// <inheritdoc />
        public override void BuildRegistration(IEnumerable<ServiceRegistrationItem> serviceRegistrations, IServiceCollection serviceCollection)
        {
            var multipleItems = serviceRegistrations
                .GroupBy(x => x.Service)
                .Where(x => x.Skip(1).Any())
                .ToList();
            foreach (var multipleItem in multipleItems)
            {
                Type enumerableType = typeof(IEnumerable<>).MakeGenericType(multipleItem.Key);

                Type enumerationFactoryInterface = typeof(ITypedEnumerationFactory<>).MakeGenericType(multipleItem.Key);

                Type enumerationFactoryImplementation = typeof(EnumerationFactory<>).MakeGenericType(multipleItem.Key);

                serviceCollection.AddSingleton(enumerationFactoryInterface, enumerationFactoryImplementation);

                serviceCollection.TryAddTransient(enumerableType, serviceProvider =>
                {
                    IEnumerationFactory enumerationFactory =
                        (IEnumerationFactory) serviceProvider.GetService(enumerationFactoryInterface);

                    return enumerationFactory.BuildEnumeration(serviceProvider, multipleItem
                        .Select(x => x.Implementation)
                        .ToList());
                });
            }
        }
    }
}
