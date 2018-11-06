using Microsoft.Extensions.DependencyInjection;

namespace RetireEarly.Registration.Feature
{
    /// <summary>
    /// Extends the IServiceCollection to add auto-registrations
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// This method is called to perform automatic registrations
        /// </summary>
        /// <param name="serviceCollection">serviceCollection</param>
        public static IAssemblyRegistrationBuilder UseAutoRegistration(this IServiceCollection serviceCollection)
        {
            return new AutoRegistrationBuilder(serviceCollection);
        }
    }
}