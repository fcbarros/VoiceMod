using ChatClient.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
        : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(App));
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ChatService>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
