using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VacationManagerApp.Data;
using VacationManagerApp.Services;
using VacationManagerApp.ViewModels;
using VacationManagerApp.Views;

namespace VacationManagerApp;

public partial class App : Application
{
    public App()
    {
        Host = CreateHostBuilder().Build();
    }

    public static IHost Host { get; private set; }

    public static IHostBuilder CreateHostBuilder()
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "vacationManager.db");
                
                var dbDirectory = Path.GetDirectoryName(dbPath);

                if (!Directory.Exists(dbDirectory))
                    Directory.CreateDirectory(dbDirectory);
                
                services.AddDbContext<VacationManagerContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));

                services.AddSingleton<IVacationManagerService, VacationManagerService>();

                services.AddTransient<EmployeeListViewModel>();
            });
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await Host.StartAsync();

        using (var scope = Host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<VacationManagerContext>();

            // This will create the database and tables if they don't exist
            await context.Database.EnsureCreatedAsync();
        }

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await Host.StopAsync();
        Host.Dispose();
        base.OnExit(e);
    }
}