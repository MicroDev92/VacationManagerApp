using System.Collections.ObjectModel;
using System.Windows.Input;
using VacationManagerApp.Entities;
using VacationManagerApp.Services;
using VacationManagerApp.Views;

namespace VacationManagerApp.ViewModels;

public class EmployeeListViewModel : BaseViewModel
{
    private readonly IVacationManagerService _vacationService;

    public EmployeeListViewModel(IVacationManagerService vacationService)
    {
        _vacationService = vacationService;
        LoadEmployees();
    }

    public ObservableCollection<Employee> Employees { get; set; }
    public Employee SelectedEmployee { get; set; }

    public ICommand OpenEmployeeDetailsCommand => new RelayCommand(OpenEmployeeDetails, CanOpenEmployeeDetails);

    public ICommand OpenAddVacationCommand => new RelayCommand(OpenAddVacation, CanOpenAddVacation);

    private async void LoadEmployees()
    {
        var employees = await _vacationService.GetEmployeesAsync();

        if (Employees == null)
        {
            Employees = new ObservableCollection<Employee>(employees);
        }
        else
        {
            Employees.Clear();
            employees.ForEach(x => Employees.Add(x));
        }
    }

    private bool CanOpenEmployeeDetails(object parameter) => SelectedEmployee != null;

    private void OpenEmployeeDetails(object parameter)
    {
        if (SelectedEmployee == null) return;

        var employeeDetailViewModel = new EmployeeDetailViewModel(SelectedEmployee.Id, _vacationService);

        var employeeDetailView = new EmployeeDetailView
        {
            DataContext = employeeDetailViewModel
        };
        employeeDetailView.Closed += (s, e) => LoadEmployees();
        employeeDetailView.Show();
    }

    private bool CanOpenAddVacation(object parameter)
    {
        return Employees != null && Employees.Count > 0;
    }

    private void OpenAddVacation(object parameter)
    {
        var addVacationView = new AddVacationView
        {
            DataContext = new AddVacationViewModel(Employees, _vacationService)
        };

        addVacationView.Closed += (s, e) => LoadEmployees();
        addVacationView.ShowDialog();
    }
}