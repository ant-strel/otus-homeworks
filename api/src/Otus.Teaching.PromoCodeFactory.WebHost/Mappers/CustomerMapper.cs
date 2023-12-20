using CustomerGrpc;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.WebHost.Mappers
{
    public class CustomerMapper
    {

        public static Core.Domain.PromoCodeManagement.Customer MapFromModel(Models.CreateOrEditCustomerRequest model, IEnumerable<Preference> preferences, Core.Domain.PromoCodeManagement.Customer customer = null)
        {
            if(customer == null)
            {
                customer = new Core.Domain.PromoCodeManagement.Customer();
                customer.Id = Guid.NewGuid();
            }
            
            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferences.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                Preference = x,
                PreferenceId = x.Id
            }).ToList();
            
            return customer;
        }

        public static Core.Domain.PromoCodeManagement.Customer MapFromModel(CustomerGrpc.CreateOrEditCustomerRequest model, IEnumerable<Preference> preferences, Core.Domain.PromoCodeManagement.Customer customer = null)
        {
            if (customer == null)
            {
                customer = new Core.Domain.PromoCodeManagement.Customer();
                customer.Id = Guid.NewGuid();
            }

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Email = model.Email;

            customer.Preferences = preferences.Select(x => new CustomerPreference()
            {
                CustomerId = customer.Id,
                Preference = x,
                PreferenceId = x.Id
            }).ToList();

            return customer;
        }

    }
}
