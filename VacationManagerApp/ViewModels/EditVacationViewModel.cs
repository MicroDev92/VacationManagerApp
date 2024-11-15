using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VacationManagerApp.Entities;
using VacationManagerApp.Services;

namespace VacationManagerApp.ViewModels;

public class EditVacationViewModel : BaseViewModel
{
    private readonly IVacationManagerService _vacationManagerService;

    private DateTime _endDate;

    private DateTime _startDate;

    private string _validationMessage;

    public EditVacationViewModel(Vacation vacation, IVacationManagerService vacationManagerService)
    {
        _vacationManagerService = vacationManagerService;
        Vacation = vacation;
        Employee = vacation.Employee;
        StartDate = vacation.StartDate;
        EndDate = vacation.EndDate;
    }

    public Vacation Vacation { get; }

    public Employee Employee { get; }
    
    public string FullName => $"{Employee.FirstName} {Employee.LastName}";

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

    public ICommand SaveChangesCommand => new RelayCommand(SaveChanges, CanSaveChanges);

    private bool CanSaveChanges(object parameter)
    {
        return string.IsNullOrEmpty(ValidationMessage);
    }

    private async void SaveChanges(object parameter)
    {
        if (!CanSaveChanges(parameter)) return;

        var initialVacationDuration = Vacation.Duration;

        Vacation.StartDate = StartDate;
        Vacation.EndDate = EndDate;
        Vacation.Duration = CalculateVacationDuration();
        
        if (Vacation.Duration > initialVacationDuration)
            Employee.RemainingVacationDays -= Vacation.Duration - initialVacationDuration;
        else
            Employee.RemainingVacationDays += initialVacationDuration - Vacation.Duration;

        await _vacationManagerService.UpdateVacationAsync(Vacation);

        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    private void ValidateDates()
    {
        if (StartDate > EndDate)
            ValidationMessage = "The start date cannot be after the end date.";
        else if (StartDate.DayOfWeek == DayOfWeek.Saturday && EndDate.DayOfWeek == DayOfWeek.Sunday && (EndDate - StartDate).Days == 1)
            ValidationMessage = "Cannot set a vacation that spans only weekend days.";
        else if ((StartDate.Date == EndDate.Date && StartDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday))
            ValidationMessage = "Cannot set the date to a weekend.";
        else if (!HasSufficientVacationDays(out var validationMessage))
            ValidationMessage = validationMessage;
        else
            ValidationMessage = string.Empty;
    }

    private bool HasSufficientVacationDays(out string validationMessage)
    {
        if (Employee.RemainingVacationDays == 0)
        {
            validationMessage = "Employee has no vacation days left.";
            return false;
        }
        
        var newDuration = CalculateVacationDuration();
        var usedVacationDays = Employee.TotalVacationDays - Employee.RemainingVacationDays;
        var totalVacationAfterEdit = usedVacationDays - Vacation.Duration + newDuration;

        if (totalVacationAfterEdit <= Employee.TotalVacationDays)
        {
            validationMessage = string.Empty;
            return true;
        }

        validationMessage = "The edited vacation duration exceeds available vacation days.";
        return false;
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
}