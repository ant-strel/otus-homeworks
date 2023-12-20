using Otus.Teaching.Pcf.Administration.Core.Abstractions.Repositories;
using Otus.Teaching.Pcf.Administration.Core.Domain.Administration;

namespace Otus.Teaching.Pcf.Administration.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly IRepository<Role> _roleCollection;
        private readonly IRepository<Employee> _employeeCollection;
        public MongoDbInitializer(IRepository<Role> roleCollection, IRepository<Employee> employeeCollection)
        {
            _roleCollection = roleCollection;
            _employeeCollection = employeeCollection;
        }
        public async void InitializeDb()
        {
            foreach (var role in FakeDataFactory.Roles)
            {
                var temp = await _roleCollection.GetByIdAsync(role.Id);
                if (temp != null)
                    continue;
                await _roleCollection.AddAsync(role);
            }

            foreach (var employee in FakeDataFactory.Employees)
            {
                var temp = await _employeeCollection.GetByIdAsync(employee.Id);
                if (temp != null)
                    continue;
                await _employeeCollection.AddAsync(employee);
            }
        }
    }
}
