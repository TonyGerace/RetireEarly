using System.Reflection;

namespace RetireEarly.Registration.Feature
{
    public static class AssemblyRegistrationBuilderExtensions
    {
        /// <summary>
        /// This method is called to perform automatic registrations
        /// </summary>
        /// <param name="assemblyRegistrationBuilder">assembly registration builder</param>
        public static IAssemblyRegistrationBuilder IncludeCurrentAssembly(this IAssemblyRegistrationBuilder assemblyRegistrationBuilder)
        {
            assemblyRegistrationBuilder.IncludeAssembly(Assembly.GetCallingAssembly());
            return assemblyRegistrationBuilder;
        }
    }
}
