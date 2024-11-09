using System;

namespace VacationManagerApp.Entities;

public class Vacation
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Duration { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}