using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VacationManagerApp.Entities;
using VacationManagerApp.ViewModels;

namespace VacationManagerApp.Views;

/// <summary>
///     Interaction logic for EmployeeListView.xaml
/// </summary>
public partial class EmployeeListView : Window
{
    public EmployeeListView()
    {
        InitializeComponent();
    }

    private void Column_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid dataGrid || dataGrid.SelectedItem is not Employee selectedEmployee) return;
        if (DataContext is EmployeeListViewModel viewModel) viewModel.OpenEmployeeDetailsCommand.Execute(selectedEmployee);
    }
}