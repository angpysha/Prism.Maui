using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Prism.Mvvm
{
    public class ViewModelLocationProvider2
    {
        private static readonly List<ViewRegistration> _registrations = new List<ViewRegistration>();

        public static void Register<TView, TViewModel>(string name) =>
            Register(typeof(TView), typeof(TViewModel), name);

        public static void Register(Type viewType, Type viewModelType, string name)
        {
            if (_registrations.Any(x => x.Name == name))
                throw new DuplicateNameException($"A view with the name '{name}' has already been registered");

            var registration = new ViewRegistration
            {
                View = viewType,
                ViewModel = viewModelType,
                Name = name
            };
            _registrations.Add(registration);
        }

        public static Type GetPageType(string name) =>
            _registrations.FirstOrDefault(x => x.Name == name)?.View;

        public static ViewRegistration GetPageNavigationInfo(Type viewType) =>
            _registrations.FirstOrDefault(x => x.View == viewType);


        ///// <summary>
        ///// A dictionary that contains all the registered factories for the views.
        ///// </summary>
        //static Dictionary<string, Func<object>> _factories = new Dictionary<string, Func<object>>();

        ///// <summary>
        ///// A dictionary that contains all the registered ViewModel types for the views.
        ///// </summary>
        static Dictionary<string, Type> _typeFactories = new Dictionary<string, Type>();

        ///// <summary>
        ///// The default view model factory which provides the ViewModel type as a parameter.
        ///// </summary>
        //static Func<Type, object> _defaultViewModelFactory = type => Activator.CreateInstance(type);

        /// <summary>
        /// ViewModelFactory that provides the View instance and ViewModel type as parameters.
        /// </summary>
        static Func<object, Type, object> _defaultViewModelFactoryWithViewParameter;

        /// <summary>
        /// Default view type to view model type resolver, assumes the view model is in same assembly as the view type, but in the "ViewModels" namespace.
        /// </summary>
        static Func<Type, Type> _defaultViewTypeToViewModelTypeResolver =
            viewType =>
            {
                var registration = _registrations.FirstOrDefault(x => x.View == viewType);
                if (registration?.ViewModel != null)
                    return registration.View;

                var viewName = viewType.FullName;
                viewName = viewName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
                var viewModelType = Type.GetType(viewModelName);

                if(registration != null)
                {
                    var newRegistration = registration with { ViewModel = viewModelType };
                    _registrations[_registrations.IndexOf(registration)] = newRegistration;
                }

                return viewModelType;
            };

        ///// <summary>
        ///// Sets the default view model factory.
        ///// </summary>
        ///// <param name="viewModelFactory">The view model factory which provides the ViewModel type as a parameter.</param>
        //public static void SetDefaultViewModelFactory(Func<Type, object> viewModelFactory)
        //{
        //    _defaultViewModelFactory = viewModelFactory;
        //}

        /// <summary>
        /// Sets the default view model factory.
        /// </summary>
        /// <param name="viewModelFactory">The view model factory that provides the View instance and ViewModel type as parameters.</param>
        public static void SetDefaultViewModelFactory(Func<object, Type, object> viewModelFactory)
        {
            _defaultViewModelFactoryWithViewParameter = viewModelFactory;
        }

        /// <summary>
        /// Sets the default view type to view model type resolver.
        /// </summary>
        /// <param name="viewTypeToViewModelTypeResolver">The view type to view model type resolver.</param>
        public static void SetDefaultViewTypeToViewModelTypeResolver(Func<Type, Type> viewTypeToViewModelTypeResolver)
        {
            _defaultViewTypeToViewModelTypeResolver = viewTypeToViewModelTypeResolver;
        }

        /// <summary>
        /// Automatically looks up the viewmodel that corresponds to the current view, using two strategies:
        /// It first looks to see if there is a mapping registered for that view, if not it will fallback to the convention based approach.
        /// </summary>
        /// <param name="view">The dependency object, typically a view.</param>
        /// <param name="setDataContextCallback">The call back to use to create the binding between the View and ViewModel</param>
        public static void AutoWireViewModelChanged(object view, Action<object, object> setDataContextCallback)
        {
            // Try mappings first
            object viewModel = null;// GetViewModelForView(view);

            // try to use ViewModel type
            //if (viewModel == null)
            {
                //check type mappings
                //var viewModelType = GetViewModelTypeForView(view.GetType());

                //// fallback to convention based
                //if (viewModelType == null)
                var viewModelType = _defaultViewTypeToViewModelTypeResolver(view.GetType());

                if (viewModelType == null)
                    return;

                viewModel = _defaultViewModelFactoryWithViewParameter(view, viewModelType);
            }


            setDataContextCallback(view, viewModel);
        }

        ///// <summary>
        ///// Gets the view model for the specified view.
        ///// </summary>
        ///// <param name="view">The view that the view model wants.</param>
        ///// <returns>The ViewModel that corresponds to the view passed as a parameter.</returns>
        //private static object GetViewModelForView(object view)
        //{
        //    var viewKey = view.GetType().ToString();

        //    // Mapping of view models base on view type (or instance) goes here
        //    if (_factories.ContainsKey(viewKey))
        //        return _factories[viewKey]();

        //    return null;
        //}

        ///// <summary>
        ///// Gets the ViewModel type for the specified view.
        ///// </summary>
        ///// <param name="view">The View that the ViewModel wants.</param>
        ///// <returns>The ViewModel type that corresponds to the View.</returns>
        //private static Type GetViewModelTypeForView(Type view)
        //{
        //    var viewKey = view.ToString();

        //    if (_typeFactories.ContainsKey(viewKey))
        //        return _typeFactories[viewKey];

        //    return null;
        //}

        ///// <summary>
        ///// Registers the ViewModel factory for the specified view type.
        ///// </summary>
        ///// <typeparam name="T">The View</typeparam>
        ///// <param name="factory">The ViewModel factory.</param>
        //public static void Register<T>(Func<object> factory)
        //{
        //    Register(typeof(T).ToString(), factory);
        //}

        ///// <summary>
        ///// Registers the ViewModel factory for the specified view type name.
        ///// </summary>
        ///// <param name="viewTypeName">The name of the view type.</param>
        ///// <param name="factory">The ViewModel factory.</param>
        //public static void Register(string viewTypeName, Func<object> factory)
        //{
        //    _factories[viewTypeName] = factory;
        //}

        ///// <summary>
        ///// Registers a ViewModel type for the specified view type.
        ///// </summary>
        ///// <typeparam name="T">The View</typeparam>
        ///// <typeparam name="VM">The ViewModel</typeparam>
        //public static void Register<T, VM>()
        //{
        //    var viewType = typeof(T);
        //    var viewModelType = typeof(VM);

        //    Register(viewType.ToString(), viewModelType);
        //}

        ///// <summary>
        ///// Registers a ViewModel type for the specified view.
        ///// </summary>
        ///// <param name="viewTypeName">The View type name</param>
        ///// <param name="viewModelType">The ViewModel type</param>
        //public static void Register(string viewTypeName, Type viewModelType)
        //{
        //    _typeFactories[viewTypeName] = viewModelType;
        //}

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ClearRegistrationCache() => _registrations.Clear();
    }
}
