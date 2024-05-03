using OutputCache.Abstractions;
using OutputCache.Dto;
using OutputCache.Models;

namespace OutputCache.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private static List<Customer> _customers; 
    public CustomerRepository()
    {
        if (_customers is null)
        {
            _customers = new();
            _customers.Add(new Customer()
            {
                Id = 1,
                FullName = "Jan testowy",
                Email = "jantestowy@gmail.com"
            });
        }
    }


    public Customer Create(CreateCustomerRequestDto createCustomerRequestDto)
    {
        int customerId = _customers.Count + 1;
        if (_customers.FirstOrDefault(x => x.Id == customerId) is not null)
        {
            return null;
        }

        var customer = new Customer()
        {
            Id = _customers.Count + 1,
            FullName = createCustomerRequestDto.FullName,
            Email = createCustomerRequestDto.Email
        };
        
        _customers.Add(customer);
        
        return customer;
    }

    public Customer GetById(int id)
    {
        return _customers.FirstOrDefault(x => x.Id == id);
    }

    public List<Customer> GetCustomers(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return _customers;
        }
        
        return _customers.Where(x => x.FullName.Contains(name)).ToList();
    }
}