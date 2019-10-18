using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using RemotingSample;

namespace ClientUI
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //get UI objects here
            TextBox box = this.Find<TextBox>("box");//find in this window
            box.Text = "banana";

            System.Console.WriteLine(box.Text);

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}