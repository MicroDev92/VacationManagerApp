using Microsoft.Extensions.DependencyInjection;

namespace VacationManagerApp.ViewModels;

public class ViewModelLocator
{
    public static EmployeeListViewModel EmployeeListViewModel => App.Host.Services.GetRequiredService<EmployeeListViewModel>();
}