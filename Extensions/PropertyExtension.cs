using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff
{
    public static class PropertyExtension
    {
        /// <summary>Allows the setting of an objects property by string name</summary>
        /// <param name="obj"></param>
        /// <param name="propName">String representing the property name</param>
        /// <param name="value">the new value</param>
        public static void SetPropertyValue(this object obj, string propName, object value)
        {
            obj.GetType().GetProperty(propName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(obj, value, null);
        }
        /// <summary>Allows the requesting of a properties value by string name </summary>
        /// <param name="obj"></param>
        /// <param name="propName">String representing the property name</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propName)
        {
            return obj.GetType().GetProperty(propName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(obj, null);
        }
    }
}
