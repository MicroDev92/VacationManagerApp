using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VacationManagerApp.Entities;
using VacationManagerApp.Services;
using VacationManagerApp.Views;

namespace VacationManagerApp.ViewModels;

public class EmployeeDetailViewModel : BaseViewModel
{
    private readonly int _employeeId;
    private readonly IVacationManagerService _vacationService;
    
    public Employee Employee { get; set; }
    public ObservableCollection<Vacation> Vacations { get; set; }

    private int _vacationDaysUsed;
    public int VacationDaysUsed
    {
        get => _vacationDaysUsed;
        set
        {
            _vacationDaysUsed = value;
            OnPropertyChanged();
        }
    }

    public EmployeeDetailViewModel(int employeeId, IVacationManagerService vacationService)
    {
        _vacationService = vacationService;
        _employeeId = employeeId;
        LoadVacations();
    }
    
    public ICommand EditVacationCommand => new RelayCommand(EditVacation, CanEditOrDeleteVacation);

    public ICommand DeleteVacationCommand => new RelayCommand(DeleteVacation, CanEditOrDeleteVacation);

    private async void LoadVacations()
    {
        var vacations = await _vacationService.GetEmployeeVacationsAsync(_employeeId);

        if (Vacations == null)
        {
            Vacations = new ObservableCollection<Vacation>(vacations);
        }
        else
        {
            Vacations.Clear();
            vacations.ForEach(v => Vacations.Add(v));
        }

        Employee = await _vacationService.GetEmployeeAsync(_employeeId);
        OnPropertyChanged(nameof(Employee));
        
        VacationDaysUsed = Employee.TotalVacationDays - Employee.RemainingVacationDays;
    }

    private static bool CanEditOrDeleteVacation(object parameter) => parameter is Vacation;

    private void EditVacation(object parameter)
    {
        if (parameter == null) return;
        
        var vacation = (Vacation)parameter;

        var editVacationView = new EditVacationView
        {
            DataContext = new EditVacationViewModel(vacation, _vacationService)
        };

        editVacationView.Closed += (s, e) => LoadVacations();
        editVacationView.ShowDialog();
    }

    private async void DeleteVacation(object parameter)
    {
        if (parameter == null) return;

        var vacation = (Vacation)parameter;

        var result = MessageBox.Show("Are you sure you want to delete?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        Employee.RemainingVacationDays += vacation.Duration;

        await _vacationService.DeleteVacationAsync(vacation.Id);

        LoadVacations();
    }
}