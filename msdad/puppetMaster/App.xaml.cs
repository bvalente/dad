using Avalonia;
using Avalonia.Markup.Xaml;

namespace puppetMaster
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}