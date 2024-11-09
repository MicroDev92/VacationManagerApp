
# Vacation Manager Application

A WPF desktop application for managing employee vacation days, allowing HR or managers to add, edit, and delete vacation records while ensuring no more than the allocated days are used as well as additional checks on date/date range validity, as per Task requirements.

## Features

- Track employees’ remaining and used vacation days.
- Schedule, edit, and delete vacation days.
- Integrated validations to prevent exceeding allotted vacation days.
- Uses SQLite for persistent, local storage.
- Use ICommand in conjunction with **Custom** RelayCommand implementation.
- Main View (EmployeeListView) is injected using DI into the View.

## Libraries and Dependencies

The application relies on the following libraries:

- **MaterialDesignInXAML**: For a clean UI/UX experience.
- **Entity Framework Core (EF Core)**: For SQLite database access and ORM capabilities.
- **SQLite**: For persistent storage.
- **Microsoft.Extensions.DependencyInjection & Hosting**: For DI setup and application lifecycle management.

## Database Configuration

- The SQLite database is located at `AppData/vacationManager.db` and is copied to the output directory on build.
- The application uses a relative path, so no additional configuration is required.
- Initial seed will contain 3 users, if you wish to add more, specify them in `Data/VacationManagerContext.cs`
- Initial seed contains no Vacations

## Additional Notes
- I have taken the liberty of modifying the UI a little, Windows do not look exactly the same as in the Task Description. Hope that's okay! :)
