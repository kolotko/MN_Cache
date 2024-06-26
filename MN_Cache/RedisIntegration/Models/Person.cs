using Redis.OM.Modeling;

namespace RedisIntegration.Models;

[Document]
public class Person
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string LastName { get; set; }
    
    public int Age { get; set; }
}