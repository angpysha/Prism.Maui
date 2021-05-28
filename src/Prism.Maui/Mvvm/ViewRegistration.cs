using System;

namespace Prism.Mvvm
{
    public record ViewRegistration
    {
        public Type View { get; init; }
        public Type ViewModel { get; init; }
        public string Name { get; init; }
    }
}
