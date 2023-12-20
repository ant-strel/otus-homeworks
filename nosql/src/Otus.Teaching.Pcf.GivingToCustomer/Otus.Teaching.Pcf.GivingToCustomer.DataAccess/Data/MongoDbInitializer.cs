

using Otus.Teaching.Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Otus.Teaching.Pcf.GivingToCustomer.Core.Domain;

namespace Otus.Teaching.Pcf.GivingToCustomer.DataAccess.Data
{
    public class MongoDbInitializer : IDbInitializer
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;
        public MongoDbInitializer(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }
        public async void InitializeDb()
        {
            foreach (var customer in FakeDataFactory.Customers)
            {
                var temp = await _customerRepository.GetByIdAsync(customer.Id);
                if (temp != null)
                    continue;
                await _customerRepository.AddAsync(customer);
            }

            foreach (var employee in FakeDataFactory.Preferences)
            {
                var temp = await _preferenceRepository.GetByIdAsync(employee.Id);
                if (temp != null)
                    continue;
                await _preferenceRepository.AddAsync(employee);
            }
        }
    }
}
