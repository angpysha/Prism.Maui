using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace Prism
{
    public class PrismApplicationWindow :  Window
    {
        public new IView Content { get; set; }
        public new  string Title { get; set; }
    }
}
