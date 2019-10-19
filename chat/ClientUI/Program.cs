using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using RemotingSample;
using ClientUI;
using System.Threading;
using Avalonia.Threading;

namespace ClientUI
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            app.Run(new MainWindow());
            //the code gets stuck here unitl the window is closed

        }
    }
}

namespace RemotingSample{

    class ClientChat : MarshalByRefObject, IClientChat{

        private MainWindow window;

        public ClientChat(MainWindow w){
            window = w;
        }

        public void SendClient(string message){

            //we need to send to dispatcher because the UI only works on one thread
            Dispatcher dispatcher = Dispatcher.UIThread;
            Action action = new Action( () => window.updateChat(message));
            dispatcher.InvokeAsync(action);

        }
    }
}
