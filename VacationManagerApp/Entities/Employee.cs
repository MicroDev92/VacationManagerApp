using System.Collections.Generic;

namespace VacationManagerApp.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int TotalVacationDays { get; set; }
    public int RemainingVacationDays { get; set; }
    public List<Vacation> Vacations { get; set; } = new();
}