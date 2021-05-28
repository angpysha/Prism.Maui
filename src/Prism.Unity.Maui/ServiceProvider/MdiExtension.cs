using Unity;
using Unity.Extension;
using Unity.Lifetime;

namespace Prism.Unity
{
    internal class MdiExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
        }

        //public ILifetimeContainer Lifetime => Context.Lifetime;
        public IUnityContainer Container { get; }
    }
}
