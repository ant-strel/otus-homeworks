using MassTransit;
using Newtonsoft.Json;
using Otus.Teaching.Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Otus.Teaching.Pcf.GivingToCustomer.Core.Domain;
using Otus.Teaching.Pcf.GivingToCustomer.WebHost.Mappers;
using Otus.Teaching.Pcf.GivingToCustomer.WebHost.Models;
using RMQModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Otus.Teaching.Pcf.GivingToCustomer.WebHost.RabbitMQ.Consumer
{
    public class GivePromocodeToCustomerConsumer : IConsumer<GivePromoCodeToCustomerDto>
    {
        private IPromocodeService _customerService;
        public GivePromocodeToCustomerConsumer(IPromocodeService customerService)
        {
            _customerService = customerService;
        }
        public async Task Consume(ConsumeContext<GivePromoCodeToCustomerDto> context)
        {
           await  _customerService.GivePromoCodesToCustomersWithPreferenceAsync(context.Message);
        }
    }

    public interface IPromocodeService
    {
        public Task GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeToCustomerDto obj);
        public Task GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request);
    }

    public class PromocodeService : IPromocodeService
    {
        private readonly IRepository<PromoCode> _promoCodesRepository;
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly IRepository<Customer> _customersRepository;
        public PromocodeService(IRepository<PromoCode> promoCodesRepository,
            IRepository<Preference> preferencesRepository, IRepository<Customer> customersRepository)
        {
            _promoCodesRepository = promoCodesRepository;
            _preferencesRepository = preferencesRepository;
            _customersRepository = customersRepository;
        }
        public async Task GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeToCustomerDto request)
        {
            try
            {
                var preferenceAndCustomers = await GetEntities(request.PreferenceId);
                PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preferenceAndCustomers.Preference, preferenceAndCustomers.Customers);
                await GivePromoCodesToCustomersWithPreferenceAsync(promoCode);
            }
            catch
            {

            }
        }

        public async Task GivePromoCodesToCustomersWithPreferenceAsync(GivePromoCodeRequest request)
        {
            try
            {
                var preferenceAndCustomers = await GetEntities(request.PreferenceId);
                PromoCode promoCode = PromoCodeMapper.MapFromModel(request, preferenceAndCustomers.Preference, preferenceAndCustomers.Customers);
                await GivePromoCodesToCustomersWithPreferenceAsync(promoCode);
            }
            catch
            {

            }
        }

        private async Task GivePromoCodesToCustomersWithPreferenceAsync(PromoCode promoCode)
        {
            await _promoCodesRepository.AddAsync(promoCode);
        }

        private async Task<PreferenceCustomers> GetEntities(Guid id)
        {
            var preference = await _preferencesRepository.GetByIdAsync(id);
            if (preference == null)
                throw new NullReferenceException($"preference is null");
            //  Получаем клиентов с этим предпочтением:
            var customers = await _customersRepository
                .GetWhere(d => d.Preferences.Any(x =>
                    x.Preference.Id == preference.Id));
            return new PreferenceCustomers(preference, customers);
        }

    }

    internal class PreferenceCustomers
    {
        public PreferenceCustomers(Preference preference, IEnumerable<Customer> customers)
        {
            Preference = preference;
            Customers = customers;
        }

        public Preference Preference { get; set; }
        public IEnumerable<Customer> Customers { get; set; }
    }
}
