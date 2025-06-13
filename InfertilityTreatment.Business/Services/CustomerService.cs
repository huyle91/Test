using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        public CustomerService(ICustomerRepository customerRepository,IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        public async Task<CustomerDetailDto?> GetCustomerProfileAsync(int customerId)
        {
            var customer = await _customerRepository.GetWithUserAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            var customerProfileDto =  _mapper.Map<CustomerDetailDto>(customer);
            return customerProfileDto;
        }

        public Task<string> UpdateCustomerProfileAsync(int customerId, CustomerProfileDto customerProfileDto)
        {
            var updatedCustomer = _customerRepository.UpdateCustomerProfileAsync(customerId, customerProfileDto);
            if (updatedCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            return Task.FromResult("Customer profile updated successfully.");
        }


        public Task<string> UpdateMedicalHistoryAsync(int customerId, string medicalHistory)
        {
            var customer = _customerRepository.UpdateMedicalHistoryAsync(customerId,medicalHistory);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            return Task.FromResult("Customer profile updated successfully.");
        }
    }
}
