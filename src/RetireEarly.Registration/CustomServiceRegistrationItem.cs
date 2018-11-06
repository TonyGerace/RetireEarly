using System;

namespace RetireEarly.Registration
{
    /// <summary>
    /// registration item
    /// </summary>
    public sealed class CustomServiceRegistrationItem
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="service">abstraction</param>
        /// <param name="implementation">implementation</param>
        public CustomServiceRegistrationItem(Type service, Type implementation)
        {
            Service = service;
            Implementation = implementation;
        }
        /// <summary>
        /// create custom service registration item
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <typeparam name="TImplementation">implementation type</typeparam>
        /// <returns></returns>
        public static CustomServiceRegistrationItem Create<TService, TImplementation>()
        {
            return new CustomServiceRegistrationItem(typeof(TService), typeof(TImplementation));
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

