using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.WebHost.Mappers;

using Domain = Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace CustomerGrpc;
public class CustomerGrpcService : Customer.CustomerBase
{
    private readonly IRepository<Domain.Customer> _customerRepository;
    private readonly IRepository<Domain.Preference> _preferenceRepository;
    public CustomerGrpcService(IRepository<Domain.Customer> customerRepository,
            IRepository<Domain.Preference> preferenceRepository)
    {
        _customerRepository = customerRepository;
        _preferenceRepository = preferenceRepository;
    }

    public override async Task<ListResponse> ListCustomers(Empty request, ServerCallContext context)
    {
        var customers = await _customerRepository.GetAllAsync();
        ListResponse list = new ListResponse();
        list.Customers.AddRange(customers.Select(x => new CustomerShortResponse()
        {
            Id = x.Id.ToString(),
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName
        }).ToList());
        return await Task.FromResult(list);
    }

    public override async Task<CustomerResponse> GetCustomer(CustomerRequest request, ServerCallContext context)
    {
        Guid id;
        if (!Guid.TryParse(request.Id, out id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Incorrect id"));
        }

        var customer = await _customerRepository.GetByIdAsync(id);
        var result = await Task.FromResult(new CustomerResponse()
        {
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Id = customer.Id.ToString()
        });
        result.Preferences.AddRange(customer.Preferences
            .Select(x => new PreferenceResponse()
            {
                Id = x.Preference.Id.ToString(),
                Name = x.Preference.Name
            }).ToList());
        return await Task.FromResult(result);
    }

    public override async Task<CustomerResponse> CreateCustomer(CreateOrEditCustomerRequest request, ServerCallContext context)
    {
        var preferences = await _preferenceRepository
            .GetRangeByIdsAsync(request.PreferenceIds.Select(x => Guid.Parse(x.Id)).ToList());

        var customer = CustomerMapper.MapFromModel(request, preferences);

        await _customerRepository.AddAsync(customer);

        return await this.GetCustomer(new CustomerRequest { Id = customer.Id.ToString() }, context);
    }

    public override async Task<Empty> UpdateCustomer(EditCustomerRequest request, ServerCallContext context)
    {
        Guid id;
        if (!Guid.TryParse(request.Id.Id, out id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Incorrect id"));
        }

        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "customer not found"));
        }

        var preferences = await _preferenceRepository
            .GetRangeByIdsAsync(request.Request.PreferenceIds.Select(x => Guid.Parse(x.Id)).ToList());

        CustomerMapper.MapFromModel(request.Request, preferences, customer);

        await _customerRepository.UpdateAsync(customer);

        return new Empty();
    }

    public override async Task<Empty> DeleteCustomer(CustomerRequest request, ServerCallContext context)
    {
        Guid id;
        if (!Guid.TryParse(request.Id, out id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Incorrect id"));
        }

        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "customer not found"));
        }

        await _customerRepository.DeleteAsync(customer);

        return new Empty();
    }
}