using Temple.Application.Core;

namespace Temple.Application.People;

public class PersonParams : PagingParams
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Dead { get; set; }
    public string Sorting { get; set; }

    public PersonParams()
    {
        Sorting = "name";
    }
}
