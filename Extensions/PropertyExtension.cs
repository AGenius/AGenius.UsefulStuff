namespace AGenius.UsefulStuff
{
    /// <summary>Extensions for accessing object properties by string name using reflection</summary>
    public static class PropertyExtension
    {
        /// <summary>Allows the setting of an objects property by string name</summary>
        /// <param name="obj"></param>
        /// <param name="propName">String representing the property name</param>
        /// <param name="value">the new value</param>
        public static void SetPropertyValue(this object obj, string propName, object value)
        {
            var ObjType = obj.GetType().GetProperty(propName, System.Reflection.BindingFlags.IgnoreCase |
                                                       System.Reflection.BindingFlags.Public |
                                                       System.Reflection.BindingFlags.Instance);
            if (ObjType != null)
            {
                ObjType.SetValue(obj, value, null);
            }
        }
        /// <summary>Allows the requesting of a properties value by string name </summary>
        /// <param name="obj"></param>
        /// <param name="propName">String representing the property name</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propName)
        {
            var ObjType = obj.GetType().GetProperty(propName, System.Reflection.BindingFlags.IgnoreCase |
                                                       System.Reflection.BindingFlags.Public |
                                                       System.Reflection.BindingFlags.Instance);
            if (ObjType != null)
            {
                return ObjType.GetValue(obj, null);
            }
            return null;
        }
    }
}
