using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VacationManagerApp.Entities;
using VacationManagerApp.Services;

namespace VacationManagerApp.ViewModels;

public class AddVacationViewModel : BaseViewModel
{
    private readonly IVacationManagerService _vacationManagerService;

    private DateTime _endDate = DateTime.Now;

    private DateTime _startDate = DateTime.Now;
    
    private string _validationMessage;
    
    private Employee _selectedEmployee;
    
    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            _startDate = value;
            OnPropertyChanged();
            ValidateDates();
        }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            _endDate = value;
            OnPropertyChanged();
            ValidateDates();
        }
    }
    
    public ObservableCollection<Employee> Employees { get; }

    public Employee SelectedEmployee
    {
        get => _selectedEmployee;
        set
        {
            _selectedEmployee = value;
            OnPropertyChanged();
            ValidateDates();
        }
    }

    public AddVacationViewModel(ObservableCollection<Employee> employees,
        IVacationManagerService vacationManagerService)
    {
        _vacationManagerService = vacationManagerService;
        Employees = employees;
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        set
        {
            _validationMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsValidationMessageVisible));
        }
    }

    public bool IsValidationMessageVisible => !string.IsNullOrEmpty(ValidationMessage);

    public ICommand SaveVacationCommand => new RelayCommand(SaveVacation, CanSaveVacation);

    private bool CanSaveVacation(object parameter) => SelectedEmployee != null && string.IsNullOrEmpty(ValidationMessage);

    private async void SaveVacation(object parameter)
    {
        if (!CanSaveVacation(parameter)) return;

        var newVacation = new Vacation
        {
            EmployeeId = SelectedEmployee.Id,
            StartDate = StartDate,
            EndDate = EndDate,
            Duration = CalculateVacationDuration()
        };
        
        SelectedEmployee.RemainingVacationDays -= newVacation.Duration;

        await _vacationManagerService.AddVacationAsync(newVacation);

        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    private void ValidateDates()
    {
        if (StartDate.Date > EndDate.Date)
            ValidationMessage = "The start date cannot be after the end date.";
        else if (!HasSufficientVacationDays())
            ValidationMessage = "The vacation duration exceeds the employee's remaining vacation days.";
        else if (StartDate.Date == EndDate.Date && (StartDate.DayOfWeek == DayOfWeek.Saturday || StartDate.DayOfWeek == DayOfWeek.Sunday))
        {
            ValidationMessage = "Cannot set the date to a weekend.";
        }
        else if (VacationDatesExist())
            ValidationMessage = "Vacation with supplied dates already exist.";
        else
            ValidationMessage = string.Empty;
    }

    private bool HasSufficientVacationDays()
    {
        if (SelectedEmployee == null) return true;

        var newDuration = CalculateVacationDuration();
        return newDuration <= SelectedEmployee.RemainingVacationDays;
    }
    
    private int CalculateVacationDuration()
    {
        var duration = 0;
        var current = StartDate.Date;

        while (current <= EndDate.Date)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday) duration++;
            current = current.AddDays(1);
        }

        return duration;
    }

    private bool VacationDatesExist()
    {
        if (SelectedEmployee == null) return false;

        return SelectedEmployee.Vacations.Any(x =>
            x.StartDate.Date == StartDate.Date || x.EndDate.Date == EndDate.Date);
    }
}