using DevTools.MVVM.Model;
using DevTools.MVVM.ViewModel;
using DevTools.Services.ConfigurationFile;
using DevTools.Services.Solution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;

namespace DevTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }
        public IConfiguration? Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            // Load configuration from settings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            // Bind settings to AppSettings class
            services.Configure<SettingsModel>(Configuration.GetSection(nameof(SettingsModel)));

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
            base.OnStartup(e);

            // Resolve MainWindow and set its DataContext to MainViewModel using DI
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register services here
            services.AddSingleton<ISolutionBuilder, SolutionBuilder>();
            services.AddSingleton<IConfigurationFileBuilder, ConfigurationFileBuilder>();

            // Register ViewModels here if needed
            services.AddTransient<DevToolViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<MainViewModel>();

            // Register MainWindow
            services.AddSingleton<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            base.OnExit(e);
        }
    }
    //TODO: Gestione dei settings
    ///Gestire i setting dando la possibilità di parametrizzare:
    ///-Programma di apertura del tool
    ///-Configurazione di default
    ///-Utente e pass del server di business con default sa, nts
    /// (qui potrebbe serviere aggiungerli nel caso anche nella creazione dei progetti... del resto cambiano per server..)
    ///-I file delle chiavi, possono essere presi anche in posti diversi dal percorso predefinito, forse conviene cambiarlo
}
