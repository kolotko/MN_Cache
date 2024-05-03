using OutputCache.Dto;
using OutputCache.Models;

namespace OutputCache.Abstractions;

public interface ICustomerRepository
{
    Customer Create(CreateCustomerRequestDto createCustomerRequestDto);
    Customer GetById(int id);
    
    List<Customer> GetCustomers(string name);
}