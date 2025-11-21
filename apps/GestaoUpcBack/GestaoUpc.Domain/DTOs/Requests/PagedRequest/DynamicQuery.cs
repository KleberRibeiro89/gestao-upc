using GestaoUpc.Domain.Enums;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestaoUpc.Domain.DTOs.Requests.PagedRequest;

public abstract record DynamicQuery
{
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 20;

    public ExpressionType Operator { get; set; } = ExpressionType.And;

    [JsonIgnore]
    public DynamicQueryResultType ResultType { get; set; } = DynamicQueryResultType.Paginated;

    [JsonIgnore]
    public int Skip => PageNumber * PageSize;

    public IList<PropertySort>? OrderBy { get; set; }
    public string? OrderByString { get; set; } = null!;
    public IList<PropertyFilter>? Filter { get; set; }
    public string? FilterString { get; set; } = null!;

    public override string ToString() => JsonSerializer.Serialize(this);
}

public record PropertySort
{
    public string Name { get; set; } = null!;
    public string Order { get; set; } = null!;

    [JsonIgnore]
    public bool Ascending => Order?.Trim().ToLower() != "desc";

    public bool IsValid() => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Order);
}

public record PropertyFilter
{
    public string Name { get; set; } = null!;
    public ExpressionType Condition { get; set; }
    public object Value { get; set; } = null!;

    public object GetValue()
    {
        if (Value is JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Array:
                    return jsonElement.EnumerateArray();
                case JsonValueKind.String:
                    return jsonElement!.GetString()!;
                case JsonValueKind.Number:
                    return jsonElement.GetInt32();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null!;
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                default:
                    return jsonElement.ToString();
            }
        }

        return Value;
    }
}