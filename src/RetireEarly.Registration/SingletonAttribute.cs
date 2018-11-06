using System;

namespace RetireEarly.Registration
{
    /// marks a component as a singleton for convention based registration
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Class,
        Inherited = true,
        AllowMultiple = false)]
    public class SingletonAttribute : Attribute
    {
    }
}