using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AGenius.UsefulStuff
{
    /// <summary>
    /// Attribute to mark properties that should be ignored during mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreMapAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeFromListReads : Attribute
    {
    }
    #region object extension for comparision
    /// <summary>Object Extensions</summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Maps properties from one object to another of the same type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void MapPropertiesFrom<T>(this T target, T source) where T : class
        {
            if (target == null || source == null)
                return;

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.CanWrite &&
                                    p.GetCustomAttribute(typeof(IgnoreMapAttribute)) == null);

            foreach (var prop in props)
            {
                var value = prop.GetValue(source, null);
                prop.SetValue(target, value, null);
            }
        }
        /// <summary>
        /// Maps properties from one object to another of different types.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void MapPropertiesFrom<TTarget, TSource>(this TTarget target, TSource source, bool ignoreNulls = false)
            where TTarget : class
            where TSource : class
        {
            if (target == null || source == null)
                return;

            var targetProps = typeof(TTarget).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetCustomAttribute(typeof(IgnoreMapAttribute)) == null);

            var sourceProps = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var targetProp in targetProps)
            {
                var sourceProp = sourceProps.FirstOrDefault(p => p.Name == targetProp.Name && p.PropertyType == targetProp.PropertyType && p.CanRead);
                if (sourceProp != null)
                {
                    var value = sourceProp.GetValue(source, null);
                    if (ignoreNulls && value == null)
                        continue;
                    targetProp.SetValue(target, value, null);
                }
            }
        }
        /// <summary>
        /// This will return the properties of a class with any annotations decorating it
        /// </summary>
        /// <param name="type">The Class entity</param>
        /// <returns>List of properties</returns>
        public static List<ObjectPropertyInfo> GetPropertiesWithAnnotations(this Type type)
        {
            var properties = type.GetProperties();
            var classAnnotations = type.GetCustomAttributes(false);
            var result = new List<ObjectPropertyInfo>();

            // Add top level annotations as first entry
            var topitem = new ObjectPropertyInfo
            {
                PropertyName = type.Name,
                PropertyType = type,
                PropertyCategory = "CLASS",
                Attributes = new List<object>()
            };

            foreach (var attribute in classAnnotations)
            {
                topitem.Attributes.Add(attribute);
            }
            result.Add(topitem);

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                System.ComponentModel.AttributeCollection otherAttributes = System.ComponentModel.TypeDescriptor.GetProperties(type)[property.Name].Attributes;
                System.ComponentModel.CategoryAttribute propertyCategory = (System.ComponentModel.CategoryAttribute)otherAttributes[typeof(System.ComponentModel.CategoryAttribute)];
                System.ComponentModel.DisplayNameAttribute propertyDisplayName = (System.ComponentModel.DisplayNameAttribute)otherAttributes[typeof(System.ComponentModel.DisplayNameAttribute)];
                System.ComponentModel.DescriptionAttribute propertyDescription = (System.ComponentModel.DescriptionAttribute)otherAttributes[typeof(System.ComponentModel.DescriptionAttribute)];

                var item = new ObjectPropertyInfo
                {
                    PropertyName = property.Name,
                    PropertyType = property.PropertyType,
                    PropertyCategory = propertyCategory.Category,
                    PropertyDescription = propertyDescription.Description,
                    PropertyDisplayName = (!string.IsNullOrEmpty(propertyDisplayName.DisplayName) ? propertyDisplayName.DisplayName : property.Name),
                    Attributes = new List<object>(),
                    isKeyField = property.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.KeyAttribute)) != null,
                };

                foreach (var attribute in attributes)
                {
                    item.Attributes.Add(attribute);
                }
                result.Add(item);
            }

            return result;
        }
        /// <summary>
        /// Return a List of Variance records representing the differences between two entity objects
        /// </summary>
        /// <typeparam name="T">The Entity Type</typeparam>
        /// <param name="sourceObject">The Source object</param>
        /// <param name="targetObject">The Taget object</param>
        /// <param name="ignoreID">set to true to ensure the ID property is not compared</param>
        /// <returns>Lists of Variance records <see cref="List{T}"/> <seealso cref="Variance"/></returns>
        public static List<Variance> DetailedCompare<T>(this T sourceObject, T targetObject, bool ignoreID = false)
        {
            try
            {
                List<Variance> variances = new List<Variance>();
                if (sourceObject != null && targetObject != null)
                {
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        string propertyName = property.Name;
                        bool isEditable = true; // used to skip properties 
                        var ComputedATT = property.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.ComputedAttribute));
                        var KeyAtt = property.GetCustomAttribute(typeof(Dapper.Contrib.Extensions.KeyAttribute));
                        var EditableATT = property.GetCustomAttribute(typeof(EditableAttribute));
                        if (EditableATT != null)
                        {
                            isEditable = property.GetCustomAttribute<EditableAttribute>().AllowEdit;
                        }
                        if (ComputedATT != null)
                        {
                            isEditable = false;
                        }
                        if (isEditable)
                        {
                            if (property.Name.ToUpper() == "ID" && ignoreID)
                            {
                                continue;
                            }
                            string category = "Misc";
                            System.ComponentModel.AttributeCollection attributes = System.ComponentModel.TypeDescriptor.GetProperties(sourceObject)[property.Name].Attributes;
                            System.ComponentModel.CategoryAttribute propertyCategory = (System.ComponentModel.CategoryAttribute)attributes[typeof(System.ComponentModel.CategoryAttribute)];
                            System.ComponentModel.DisplayNameAttribute propertyDisplayName = (System.ComponentModel.DisplayNameAttribute)attributes[typeof(System.ComponentModel.DisplayNameAttribute)];
                            System.ComponentModel.DescriptionAttribute propertyDescription = (System.ComponentModel.DescriptionAttribute)attributes[typeof(System.ComponentModel.DescriptionAttribute)];
                            category = propertyCategory.Category;
                            if (property.PropertyType.IsClass)
                            {
                                category = "Class";
                            }
                            var v = new Variance
                            {
                                PropertyName = property.Name,
                                PropertyType = property.PropertyType,
                                PropertyCategory = propertyCategory.Category,
                                PropertyDescription = propertyDescription.Description,
                                PropertyDisplayName = (!string.IsNullOrEmpty(propertyDisplayName.DisplayName) ? propertyDisplayName.DisplayName : property.Name),
                                OldValue = property.GetValue(sourceObject),
                                NewValue = property.GetValue(targetObject)
                            };

                            if (v.OldValue == null && v.NewValue == null)
                            {
                                continue;
                            }
                            if ((v.OldValue == null && v.NewValue != null) ||
                                (v.OldValue != null && v.NewValue == null))
                            {
                                variances.Add(v);
                                continue;
                            }
                            if (!v.OldValue.Equals(v.NewValue))
                            {
                                variances.Add(v);
                            }
                            if (v.OldValue.Equals(v.NewValue) && KeyAtt != null)
                            {
                                v.isKeyField = true;
                                variances.Add(v);
                            }
                        }
                    }
                }
                return variances;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Variance Record holding details of the fields that have been changed on an object
        /// </summary>
        public class Variance : ObjectPropertyInfo
        {
            /// <summary>The Old Value of the property</summary>
            public object OldValue { get; set; }
            /// <summary>The New Value of the property</summary>
            public object NewValue { get; set; }
        }
        public class ObjectPropertyInfo
        {
            /// <summary>Name of the Property</summary>
            public string PropertyName { get; set; }
            /// <summary>The property Type <see cref="System.Type"/></summary>
            public System.Type PropertyType { get; set; }
            /// <summary>True if the field is the Key field (Used by Dapper) <see cref="Dapper.Contrib.Extensions.KeyAttribute"/></summary>
            public bool isKeyField { get; set; }
            /// <summary>The Properties Category. Defaults to Misc <see cref="System.ComponentModel.CategoryAttribute"/></summary>
            public string PropertyCategory { get; set; }
            /// <summary>The Display name of the Property <see cref="System.ComponentModel.DisplayNameAttribute"/></summary>
            public string PropertyDisplayName { get; set; }
            /// <summary>The Properties Description <see cref="System.ComponentModel.DescriptionAttribute"/></summary>
            public string PropertyDescription { get; set; }
            public List<object> Attributes { get; set; }
        }
    }
    #endregion
}