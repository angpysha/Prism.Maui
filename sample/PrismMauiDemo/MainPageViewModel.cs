using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismMauiDemo
{
    public class MainPageViewModel : BindableBase
    {

        public string Text => "Hello from prism for MAUI";

        public MainPageViewModel(INavigationService navigationService)
        {
            int iiii = 0;
        }
    }
}
