using Microsoft.AspNetCore.Mvc;

namespace ResponseCaching.Controllers;

[Route("api/test")]
[ApiController]
[ResponseCache(
    Duration = 60, 
    Location = ResponseCacheLocation.Any
)]
public class TestController : ControllerBase
{
    private static List<int> _randomintList;
    public TestController()
    {
        if (_randomintList is null)
        {
            _randomintList = new List<int>();
            Random random = new Random(15);
            foreach (var enumerator in Enumerable.Range(0, 50))
            {
                _randomintList.Add(random.Next(1, 100));
            }
        }
    }
    
    [HttpPost]
    public ActionResult<List<int>> Post()
    {
        return Ok();
    }

    [HttpGet]
    public ActionResult<List<int>> Get()
    {
        return Ok(_randomintList);
    }

    [HttpGet("{id:int}")]
    public ActionResult<int> GetOne(int id)
    {
        return Ok(_randomintList[id]);
    }
}