using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Mappers;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Services
{
    public class CustomerService:ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;

        public CustomerService(IRepository<Customer> customerRepository,IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        public async Task<List<CustomerShortResponse>> GetCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();

            var response = customers.Select(x => new CustomerShortResponse()
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();
            return response;
        }

        public async Task<CustomerResponse> GetCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            var response = new CustomerResponse(customer);
            return response;
        }

        public async Task<CustomerResponse> CreateCustomer(CreateOrEditCustomerRequest request)
        {
            var preferences = await _preferenceRepository
                .GetRangeByIdsAsync(request.PreferenceIds);

            Customer customer = CustomerMapper.MapFromModel(request, preferences);

            await _customerRepository.AddAsync(customer);
            var response = new CustomerResponse(customer);
            return response;
        }

        public async Task EditCustomers(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) 
                throw new Exception("NotFound");
            var preferences = await _preferenceRepository.GetRangeByIdsAsync(request.PreferenceIds);
            CustomerMapper.MapFromModel(request, preferences, customer);
            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomer(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null) 
                throw new Exception("NotFound");

            await _customerRepository.DeleteAsync(customer);
        }
    }

    public interface ICustomerService
    {
        public Task<List<CustomerShortResponse>> GetCustomers();

        public Task<CustomerResponse> GetCustomer(Guid id);
        public Task<CustomerResponse> CreateCustomer(CreateOrEditCustomerRequest request);
        public Task EditCustomers(Guid id, CreateOrEditCustomerRequest request);
        public Task DeleteCustomer(Guid id);

    }
}
