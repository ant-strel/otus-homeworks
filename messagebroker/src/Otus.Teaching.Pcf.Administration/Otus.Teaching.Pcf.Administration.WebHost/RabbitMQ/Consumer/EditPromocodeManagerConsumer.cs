using MassTransit;
using Otus.Teaching.Pcf.Administration.Core.Abstractions.Repositories;
using Otus.Teaching.Pcf.Administration.Core.Domain.Administration;
using Otus.Teaching.Pcf.Administration.WebHost.Models;
using RMQModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Otus.Teaching.Pcf.Administration.WebHost.RabbitMQ.Consumer
{
    public class EditPromocodeManagerConsumer : IConsumer<PartnerManagerGivedPromocodeDTO>
    {
        private readonly IEmployeeService _newPromocodeService;
        public EditPromocodeManagerConsumer(IEmployeeService promocodeService)
        {
            _newPromocodeService = promocodeService;
        }
        public async Task Consume(ConsumeContext<PartnerManagerGivedPromocodeDTO> context)
        {
            await _newPromocodeService.UpdateAppliedPromocodeAsync(context.Message.PartnerManagerId);
        }
    }

    public class EmployeeService:IEmployeeService
    {
        private IRepository<Employee> _employeeRepository;

        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task UpdateAppliedPromocodeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                throw new NullReferenceException($"employee is null");

            employee.AppliedPromocodesCount++;

            await _employeeRepository.UpdateAsync(employee);
            Debug.WriteLine($"AddPromocodeEmployee message {id}");
        }
    }

    public interface IEmployeeService
    {
        public Task UpdateAppliedPromocodeAsync(Guid id);
    }
}
