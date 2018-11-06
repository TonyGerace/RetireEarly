using System.Reflection;

namespace RetireEarly.Registration
{
    /// <summary>
    /// used to support fluent api / builder pattern with assembly manifest
    /// </summary>
    public interface IAssemblyRegistrationBuilder
    {
        /// <summary>
        /// add assembly to manifest
        /// </summary>
        /// <param name="assembly">assembly to add to manifest</param>

        IAssemblyRegistrationBuilder IncludeAssembly(Assembly assembly);

        /// <summary>
        /// since this is builder pattern more appropriate name might be Build although this conflicts in context with other uses of the term build
        /// </summary>
        void Fill();
    }
}