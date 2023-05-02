// -----------------------------------------------------------------------
//  <copyright file="ObjectExtensions.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Reflection;
#pragma warning disable CS8600
#pragma warning disable CS8603

namespace GLSLRenderer;

public static class ObjectExtensions
{
    public static T DeepClone<T>(this T obj)
    {
        if (obj == null)
            return default(T);

        var type = obj.GetType();

        // Check for custom clone method
        var customCloneMethod = type.GetMethod("Clone");
        if (customCloneMethod != null)
            return (T)customCloneMethod.Invoke(obj, null);

        // If the type is a value type or a string, it's already deep cloned
        if (type.IsValueType || type == typeof(string))
            return obj;

        var clonedObject = (T)Activator.CreateInstance(type);

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var originalValue = field.GetValue(obj);
            var clonedValue = originalValue.DeepClone();
            field.SetValue(clonedObject, clonedValue);
        }

        return clonedObject;
    }
}