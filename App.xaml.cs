using Caliburn.Micro;
using ChiclanaRecordsNET.Core;
using ChiclanaRecordsNET.MVVM.View;
using ChiclanaRecordsNET.MVVM.ViewModel;
using ChiclanaRecordsNET.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ChiclanaRecordsNET
{
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<IWindowManager, WindowManager>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<SearchListViewModel>();
            services.AddSingleton<RecordViewModel>();
            services.AddSingleton<AcercaDeViewModel>();
            services.AddSingleton<SessionViewModel>();
            services.AddSingleton<CreateUserViewModel>();
            services.AddSingleton<CollectionViewModel>();

            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

            services.AddSingleton(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ViewLocator.AddNamespaceMapping("ChiclanaRecordsNET.MVVM.ViewModel", "ChiclanaRecordsNET.MVVM.View");

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}