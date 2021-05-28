using System;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Prism.Unity
{
    partial class UnityContainerExtension : IServiceCollectionAware
    {
        public IServiceProvider CreateServiceProvider()
        {
            return new ServiceProvider(Instance);
        }

        public void Populate(IServiceCollection services)
        {
            Instance.AddServices(services);
        }
    }
}
