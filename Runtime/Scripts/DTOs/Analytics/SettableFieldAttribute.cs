using System;

namespace Reflectis.CreatorKit.Worlds.Analytics
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SettableFieldAttribute : Attribute
    {
        public bool isRequired = false;

        public Type entryType = null;
    }
}
