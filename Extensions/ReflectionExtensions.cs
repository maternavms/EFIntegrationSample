using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.Extensions
{
    /// <summary>
    /// These extensions exist in ArcSqlOData.Core, and are used to simplify reflection operations.
    /// </summary>
    public static class ReflectionExtensions
    {
        public static bool Implements(this Type? targetType, Type? baseType)
        {
            // If the targetType is null or we reach the Object type, return false
            while (targetType != null && targetType != typeof(object))
            {
                // Check if targetType is a constructed generic of the openGenericType
                var currentType = targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : targetType;
                if (baseType == currentType)
                {
                    return true;
                }

                // Check interfaces for the type
                foreach (var interfaceType in targetType.GetInterfaces())
                {
                    if (interfaceType.Implements(baseType))
                        return true;
                }

                // Move on to the base type
                targetType = targetType.BaseType;
            }
            return false;
        }

        public static IEnumerable<Type> ByImplements(this IEnumerable<Type> types, Type baseType)
        {
            try
            {
                var name = baseType.Name;
                var results = types.Where(t => t.Implements(baseType));
                return results;
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        public static IEnumerable<Type> ByImplements<T>(this IEnumerable<Type> types)
        {
            return types.ByImplements(typeof(T));
        }

        public static IEnumerable<Type> ByNotImplements(this IEnumerable<Type> types, Type baseType)
        {
            try
            {
                var results = types.Where(t => !t.Implements(baseType));
                return results;
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        public static IEnumerable<Type> ByNotImplements<T>(this IEnumerable<Type> types)
        {
            return types.ByNotImplements(typeof(T));
        }

        public static IEnumerable<Type> ByConcrete(this IEnumerable<Type> types)
        {
            return types.Where(t => t.IsConcrete());
        }

        public static bool IsConcrete(this Type type)
        {
            return type.IsClass && !type.IsAbstract && !type.IsInterface;
        }

        public static IEnumerable<Type> AllTypes<T>(this Assembly assembly)
        {
            try
            {
                //ignore errors as some assemblies cannot be resolved, and we don't care.
                return assembly.AllTypes();
            }
            catch
            {
                return new List<Type>();
            }
        }


        public static IEnumerable<Type> AllTypes(this Assembly assembly)
        {
            try
            {
                return assembly.GetModules()
                    .AllTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }

        public static IEnumerable<Type> AllTypes(this IEnumerable<Module> modules)
        {
            return modules
                    .SelectMany(m => m.AllTypes())
                    .ToList();
        }

        public static IEnumerable<Type> AllTypes(this Module module)
        {
            {
                try
                {
                    return module.GetTypes();
                }
                catch
                {
                    return Enumerable.Empty<Type>();
                }
            }
        }

        public static IEnumerable<Type> AllTypes(this AppDomain appDomain)
        {
            return appDomain.GetAssemblies().AllTypes();
        }

        public static IEnumerable<Type> AllTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.AllTypes());
        }

        public static IEnumerable<Assembly> ByReferencesODataCore(this IEnumerable<Assembly> assemblies)
        {
            return assemblies
                .Where(a => a.ReferencesODataCore());
        }

        /// <summary>
        /// Checks if the given assembly references this assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool ReferencesODataCore(this Assembly assembly)
        {
            var testAssemblyName = typeof(DIExtensions).Assembly.GetName().Name;
            try
            {
                //include ArcSqlOData.Core
                if (assembly.GetName().Name == testAssemblyName)
                {
                    return true;
                }

                return assembly.GetReferencedAssemblies()
                    .Any(a => a.Name == testAssemblyName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves the type, or null from the assemblies enumerable by the given fullName
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        public static Type? FindType(this IEnumerable<Assembly> assemblies, string fullName)
        {
            foreach (var assembly in assemblies)
            {
                var type = assembly.FindType(fullName);
                if (type != null) return type;
            }
            return null;
        }
        /// <summary>
        /// Retrieves the type, or null from the assembly by the given fullName
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        public static Type? FindType(this Assembly assembly, string fullName)
        {
            return assembly.GetType(fullName);
        }

        /// <summary>
        /// Resolves all possible permutations of the open generic type, and returns a list of closed generic types.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        public static IEnumerable<Type> ResolveClosedGenericTypeDefinitions(this IEnumerable<Assembly> assemblies, Type genericType) // JobTrigger<T> where T: Resource
        {
            return assemblies.SelectMany(a => a.ResolveClosedGenericTypeDefinitions(genericType)).ToList();
        }

        public static IEnumerable<Type> ResolveClosedGenericTypeDefinitions(this Assembly assembly, Type genericType)
        {
            if (genericType.GetGenericArguments().Length != 1)
            {
                yield break;
            }

            var genericParameterTypeArg = genericType.GetGenericParameterConstraints(0);

            var genericParameterTypes = assembly.AllTypes().ByImplements(genericParameterTypeArg.First()).ByConcrete();


            foreach (var argType in genericParameterTypes)
            {
                var closedGenericType = genericType.GetGenericTypeDefinition().MakeGenericType(argType); //something like JobTrigger<SqlServerInstance>
                yield return closedGenericType;
            }
        }

        /// <summary>
        /// Retrieves the generic parameter constraints for the specified type at the index specified by parameterIndex. If the type is not generic, returns an empty array.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterIndex">relative index of the GenericParameter.</param>
        /// <returns></returns>
        public static Type[] GetGenericParameterConstraints(this Type type, int parameterIndex = 0)
        {
            return type.GetGenericArguments()[parameterIndex].GetGenericParameterConstraints();
        }

        /// <summary>
        /// Retrieves the first generic argument from the specified type, or the first generic argument from the base type if the specified type is not generic.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetGenericArgumentsFromSelfOrBase(this Type? type)
        {
            ArgumentNullException.ThrowIfNull(type);

            if (!type.IsGenericType)
            {
                if (type.BaseType == null)
                {
                    return Enumerable.Empty<Type>();
                }
                return type.BaseType?.GetGenericArgumentsFromSelfOrBase() ?? Enumerable.Empty<Type>();
            }

            return type.GenericTypeArguments;
        }

        /// <summary>
        /// Invokes the named method with no return
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        public static void InvokeMethodByName(this object sourceObj, string methodName, params object[] args)
        {
            sourceObj.InvokeMethodByName(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, args);
        }

        /// <summary>
        /// Invokes the named method with no return
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="args"></param>
        public static void InvokeMethodByName(this object sourceObj, string methodName, BindingFlags bindingFlags, object[] args)
        {
            var types = args.Select(t => t.GetType()).ToArray();
            sourceObj.InvokeMethodByName(methodName, bindingFlags, types, args);
        }

        /// <summary>
        /// Invokes the named method returning the expected type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObj"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T? InvokeMethodByName<T>(this object sourceObj, string methodName, params object[] args)
        {
            return sourceObj.InvokeMethodByName<T>(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, args);
        }

        /// <summary>
        /// Invokes the named method returning the expected type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObj"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingFlags"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T? InvokeMethodByName<T>(this object sourceObj, string methodName, BindingFlags bindingFlags, object[] args)
        {
            var types = args.Select(t => t.GetType()).ToArray();
            return sourceObj.InvokeMethodByName<T>(methodName, bindingFlags, types, args);
        }

        /// <summary>
        /// Retrieves the generic method definition for a given method name and parameter types. That may be invoked.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo? GetGenericMethod(this Type type, string name, params Type[] parameterTypes)
        {
            var methods = type.GetMethods();
            foreach (var method in methods.Where(m => m.Name == name))
            {
                var methodParameterTypes = method.GetGenericArguments().Select(p =>
                {
                    if (p.IsGenericParameter)
                    {
                        return p.GetGenericParameterConstraints().FirstOrDefault() ?? typeof(object);
                    }
                    else
                    {
                        return p;
                    }
                }).ToArray();

                if (methodParameterTypes.SequenceEqual(parameterTypes, (t1, t2) => t1.IsAssignableFrom(t2)))
                {
                    return method.MakeGenericMethod(parameterTypes);
                }
            }

            return null;
        }

        /// <summary>
        /// Compairs 2 sequences of types using a comparer functions.
        /// </summary>
        /// <param name="types"></param>
        /// <param name="otherTypes"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool SequenceEqual(this Type[] types, Type[] otherTypes, Func<Type, Type, bool> comparer)
        {
            if (types.Length != otherTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < types.Length; i++)
            {
                if (!comparer(types[i], otherTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
