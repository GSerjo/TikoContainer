using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Tiko
{
    public static class TikoContainer
    {
        private static readonly ConcurrentDictionary<Type, RegisteredObject> _registeredObjects =
            new ConcurrentDictionary<Type, RegisteredObject>();

        /// <summary>
        /// Resolve dependencies on existing object.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="existing">Instance of the object.</param>
        /// <exception cref="DependencyMissingException">Throw exception if unable to resove a dependency.</exception>
        /// <returns>Result object.</returns>
        public static T BuildUp<T>(T existing)
        {
            return DoBuildUp(existing);
        }

        /// <summary>
        /// Clear the container.
        /// </summary>
        public static void Clear()
        {
            _registeredObjects.Clear();
        }

        /// <summary>
        /// Register class.
        /// </summary>
        /// <typeparam name="T">Type of class.</typeparam>
        public static void Register<T>()
            where T : new()
        {
            Register<T, T>();
        }

        /// <summary>
        /// Register object.
        /// </summary>
        /// <typeparam name="TFrom">That will be requested.</typeparam>
        /// <typeparam name="TTo">That will actually be returned.</typeparam>
        public static void Register<TFrom, TTo>()
            where TTo : TFrom, new()
        {
            var registeredObject = new RegisteredObject(() => new TTo());
            _registeredObjects[typeof (TFrom)] = registeredObject;
        }

        /// <summary>
        /// Resolve an instance and dependencies.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <exception cref="DependencyMissingException">Throw exception if unable to resove a dependency.</exception>
        /// <returns>Result object.</returns>
        public static T Resolve<T>()
        {
            object resolvedObject;
            bool isResolved = ResolveObject(typeof (T), out resolvedObject);
            if (isResolved)
            {
                return (T) resolvedObject;
            }
            var instance = Activator.CreateInstance<T>();
            return DoBuildUp(instance);
        }

        private static T DoBuildUp<T>(T instance)
        {
            PropertyInfo[] properties = typeof (T).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!IsResolveProperty(property))
                {
                    continue;
                }
                object resolvedObject;
                bool isResolved = ResolveObject(property.PropertyType, out resolvedObject);
                if (!isResolved)
                {
                    throw new DependencyMissingException(string.Format(
                        "Could not resolve dependency for {0}", typeof (T).Name));
                }
                property.SetValue(instance, resolvedObject, null);
            }
            return instance;
        }

        private static bool IsResolveProperty(PropertyInfo property)
        {
            object[] attributes = property.GetCustomAttributes(typeof (DependencyAttribute), false);
            return attributes.Length != 0;
        }

        private static bool ResolveObject(Type typeToResolve, out object resolvedObject)
        {
            RegisteredObject result;
            if (!_registeredObjects.TryGetValue(typeToResolve, out result))
            {
                resolvedObject = null;
                return false;
            }
            resolvedObject = result.Instance;
            return true;
        }

        private sealed class RegisteredObject
        {
            private readonly Lazy<object> _concreteValue;

            public RegisteredObject(Func<object> func)
            {
                _concreteValue = new Lazy<object>(func);
            }

            public object Instance
            {
                get { return _concreteValue.Value; }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DependencyAttribute : Attribute
    {
    }

    [Serializable]
    public sealed class DependencyMissingException : Exception
    {
        public DependencyMissingException()
        {
        }

        public DependencyMissingException(string message) : base(message)
        {
        }

        public DependencyMissingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}