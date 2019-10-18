
using System.Data.Entity;

namespace EmployeeArrivalTrackerClient
{
    /// <summary>
    /// Class which is custom implementation of IUnitOfWork interface.
    /// </summary>
    public class UnitOfWork : IUnitOfWork, System.IDisposable
    {
        private readonly DbContext _context;
        private IGenericRepository<Employee> _employeeRepository; 
        private IGenericRepository<EmployeeDailyArrivalTime> _employeeDailyArrivalTimeRepository;

        /// <summary>
        /// UnitOfWork parameterized constructor.
        /// </summary>
        /// <param name="context">DbContext instance.</param>
        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets EmployeeRepository.
        /// </summary>
        public IGenericRepository<Employee> EmployeeRepository
        {
            get { return _employeeRepository ?? (_employeeRepository = new GenericRepository<Employee>(_context)); }
        }

        /// <summary>
        /// Gets EmployeeDailyArrivalTimeRepository.
        /// </summary>
        public IGenericRepository<EmployeeDailyArrivalTime> EmployeeDailyArrivalTimeRepository
        {
            get { return _employeeDailyArrivalTimeRepository ?? (_employeeDailyArrivalTimeRepository = new GenericRepository<EmployeeDailyArrivalTime>(_context)); }
        }

        /// <summary>
        /// Saves all current _context queries in one transaction.
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        /// <summary>
        /// Protected method which Disposes the context.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Disposes the context.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}