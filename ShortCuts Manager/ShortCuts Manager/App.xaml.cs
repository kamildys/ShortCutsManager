using Microsoft.Extensions.DependencyInjection;
using ShortCuts_Manager.Interfaces;
using System.Windows;
using ShortCuts_Manager.Open;
using ShortCuts_Manager.DataBase;
using Application = System.Windows.Application;

namespace ShortCuts_Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static System.Threading.Mutex singleton = new Mutex(true, "ShortCuts Manager");

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!singleton.WaitOne(TimeSpan.Zero, true))
            {
                this.Shutdown();
            }

            var services = new ServiceCollection();

            services.AddScoped<IFileOpen, FileOpen>();
            services.AddScoped<IFolderOpen, FolderOpen>();
            services.AddScoped<IUrlOpen, UrlOpen>();
            services.AddScoped<IDataBase, FileXMLDataBase>();

            //MainWindow
            services.AddScoped<MainWindow>();
            services.AddScoped<MainWindowViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<MainWindow>()?.Show();
        }
    }
}
