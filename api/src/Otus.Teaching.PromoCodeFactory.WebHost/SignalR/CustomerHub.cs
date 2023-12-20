using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Mappers;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using Otus.Teaching.PromoCodeFactory.WebHost.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.WebHost.SignalR
{
    public class CustomerHub: Hub
    {
        private readonly ICustomerService _customerService;
        public CustomerHub(IRepository<Customer> customerRepository,
            IRepository<Preference> preferenceRepository, ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task GetCustomers()
        {
             var response = await _customerService.GetCustomers();
             await Clients.Caller.SendAsync("GetCustomers",JsonConvert.SerializeObject(response));
        }

        public async Task GetCustomer(Guid id)
        {
            var response = await _customerService.GetCustomer(id);
            await Clients.Caller.SendAsync("GetCustomer", JsonConvert.SerializeObject(response));
        }

        public async Task CreateCustomer(CreateOrEditCustomerRequest request)
        {
            var response = await _customerService.CreateCustomer(request);
            await Clients.Caller.SendAsync("CreateCustomer", JsonConvert.SerializeObject(response));
        }

        public async Task EditCustomers(Guid id, CreateOrEditCustomerRequest request)
        {
            try
            {
                await _customerService.EditCustomers(id, request);
                await Clients.Caller.SendAsync("EditCustomer", "Ok");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("EditCustomer",ex.Message);
            }

        }

        public async Task DeleteCustomer(Guid id)
        {
            try
            {
                await _customerService.DeleteCustomer(id);
                await Clients.Caller.SendAsync("DeleteCustomer", "Ok");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("DeleteCustomer", ex.Message);
            }
        }
    }
}
