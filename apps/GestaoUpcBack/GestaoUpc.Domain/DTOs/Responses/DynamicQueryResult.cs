using GestaoUpc.Domain.Enums;
using System.Text.Json.Serialization;

namespace GestaoUpc.Domain.DTOs.Responses;

public class DynamicQueryResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize => _pageSize ?? Result?.Count() ?? 0;
    public int TotalRows { get; set; }
    public int TotalPages
    {
        get
        {
            if (TotalRows == 0)
            {
                return 0;
            }

            int pages = TotalRows / PageSize;
            if (TotalRows % PageSize > 0)
            {
                pages += 1;
            }

            return pages;
        }
    }

    [JsonIgnore]
    public DynamicQueryResultType ResultType { get; set; }

    public IEnumerable<T> Result { get; set; } = [];

    public string QueryString { get; set; } = null!;
    private readonly int? _pageSize;

    public DynamicQueryResult(int? pageSize = 20) => _pageSize = pageSize;
}