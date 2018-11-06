using System;
using System.Collections.Generic;
using System.Reflection;

namespace RetireEarly.Registration
{
    public abstract class CustomRegistration
    {
        /// <summary>
        /// This property should return the list of assemblies which should be scanned for auto-registration.  By default, it will only include the current assembly.  Override to 
        /// provide other assemblies.
        /// </summary>
        public virtual IEnumerable<Assembly> AssembliesToScanAutoRegistrations => new[] {GetType().Assembly};
        /// <summary>
        /// This property should return the list of assemblies which should skip auto-registration.  By default, this will be empty.  Override to 
        /// provide additional assemblies to skip.
        /// </summary>
        public virtual IEnumerable<Assembly> AssembliesToIngnoreForAutoRegistrations => new Assembly[0];

        /// <summary>
        /// For each publicly exposed type from assemblies enlisted in automatic registration, this method will be involed to 
        /// allow the implementer to perform custom registration based on the specific discovered type.  By default this method does nothing.
        /// </summary>
        /// <param name="type">type found in scanned assembly</param>
        /// <returns>enumeration of custom service registration items</returns>
        public virtual IEnumerable<CustomServiceRegistrationItem> BuildCustomRegistrationsBasedOnScannedType(
            Type type)
        {
            return new CustomServiceRegistrationItem[0];
        }
        /// <summary>
        /// This is provided to allow any desired custom registrations.  By default this method does nothing.
        /// </summary>
        /// <returns>enumeration of custom service registration items</returns>
        public virtual IEnumerable<CustomServiceRegistrationItem> BuildCustomRegistrations()
        {
            return new CustomServiceRegistrationItem[0];
        }
    
    }
}