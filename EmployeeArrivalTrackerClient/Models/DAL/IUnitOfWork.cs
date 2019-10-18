
namespace EmployeeArrivalTrackerClient
{
    public interface IUnitOfWork
    {
        IGenericRepository<Employee> EmployeeRepository { get; }
        IGenericRepository<EmployeeDailyArrivalTime> EmployeeDailyArrivalTimeRepository { get; }
        void Save();
    }
}