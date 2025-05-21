using WholeApplication.Models;

namespace WholeApplication.Interfaces
{
    public interface IEmployeeService
    {
        int AddEmployee(Employee employee);
        List<Employee>? SearchEmployee(SearchModel searchModel);
    }
}