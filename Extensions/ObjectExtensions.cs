using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AGenius.UsefulStuff
{
    #region object extension for comparision
    public static class ObjectExtensions
    {
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
                            System.ComponentModel.AttributeCollection attributes = System.ComponentModel.TypeDescriptor
                                .GetProperties(sourceObject)[property.Name].Attributes;
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
                        }
                    }
                }
                return variances;
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }

        public class Variance
        {
            public string PropertyName { get; set; }
            public string PropertyCategory { get; set; } // Holds the System.ComponentModel.Category value if any
            public string PropertyDisplayName { get; set; }// Holds the System.ComponentModel.DisplayName value if any
            public string PropertyDescription { get; set; }// Holds the System.ComponentModel.Description value if any
            public object OldValue { get; set; }
            public object NewValue { get; set; }
        }

    }
    #endregion
}