namespace Skinet.Core.Specifications;

public class ProductSpecParams : PaginParams
{

    private List<string> _brands = [];

    public List<string> Brands
    {
        get => _brands;
        set
        {
            _brands = value.SelectMany(b => b.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }

    private List<string> _types = [];

    public List<string> Types
    {
        get => _types;
        set
        {
            _types = value.SelectMany(b => b.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList();
        }
    }

    private string? _search;

    public string? Search
    {
        get => _search ?? "";
        set => _search = value?.ToLower();
    }
    public string? SortBy { get; set; }
}