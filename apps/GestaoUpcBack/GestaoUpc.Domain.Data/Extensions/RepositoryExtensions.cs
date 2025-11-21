using GestaoUpc.Domain.DTOs.Requests.PagedRequest;
using GestaoUpc.Domain.DTOs.Responses;
using GestaoUpc.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace GestaoUpc.Domain.Data.Extensions;

public static class RepositoryExtensions
{
    public static async Task<DynamicQueryResult<TEntity>> ToPagedAsync<TEntity>(this IQueryable<TEntity> query, DynamicQuery filter) where TEntity : class
    {
        var pagedResult = new DynamicQueryResult<TEntity>(filter.PageSize);

        if (filter.Filter != null)
            query = query.Filter(filter.Filter, filter.Operator);

        if (filter.ResultType == DynamicQueryResultType.Paginated)
        {
            pagedResult.TotalRows = await Task.Run(() => query.Count());
            pagedResult.PageNumber = filter.PageNumber;

            if (filter.OrderBy != null)
                query = query.OrderBy(filter.OrderBy);

            query = query.Skip(filter.Skip).Take(filter.PageSize);
        }
        else if (filter.OrderBy != null)
            query = query.OrderBy(filter.OrderBy);

        if (filter.ResultType == DynamicQueryResultType.QueryString)
            pagedResult.QueryString = query.ToQueryString();
        else
            pagedResult.Result = await Task.Run(() => query.ToList());

        return pagedResult;
    }

    public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, IList<PropertyFilter> propertiesToFilter, ExpressionType operatorType)
    {
        if (propertiesToFilter == null || propertiesToFilter.Count == 0)
            return query;

        var entityType = typeof(TEntity);
        var parameter = Expression.Parameter(typeof(TEntity), "p");
        List<Expression> expressions = new List<Expression>();

        var groupedProperties = propertiesToFilter.GroupBy(p => new { p.Name, p.Condition }).ToList();
        foreach (var groupedProperty in groupedProperties)
        {
            PropertyFilter propertyToFilter = groupedProperty.First();
            var propertyInfo = entityType.GetProperty(propertyToFilter.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
                continue;
            var member = Expression.Property(parameter, propertyInfo.Name);

            Expression expression = null;

            if (groupedProperty.Count() == 1)
            {
                Expression valueComparator = null;
                if (propertyToFilter.Condition == ExpressionType.Equal && propertyInfo.PropertyType == typeof(string))
                {
                    var value = ChangeType(propertyToFilter.GetValue(), propertyInfo.PropertyType);
                    var propertyValue = new { value };
                    valueComparator = Expression.Property(Expression.Constant(propertyValue), nameof(value));
                }
                else if (propertyToFilter.Condition != ExpressionType.OrAssign)
                {
                    valueComparator = Expression.Constant(ChangeType(propertyToFilter.GetValue(), propertyInfo.PropertyType), propertyInfo.PropertyType);
                }

                switch (propertyToFilter.Condition)
                {
                    case ExpressionType.OrAssign:
                        expression = GetContainsExpression(groupedProperty.ToList(), propertyInfo, member);
                        break;
                    case ExpressionType.Equal:
                        expression = Expression.Equal(member, valueComparator);
                        break;
                    case ExpressionType.NotEqual:
                        expression = Expression.NotEqual(member, valueComparator);
                        break;
                    case ExpressionType.GreaterThan:
                        expression = Expression.GreaterThan(member, valueComparator);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(member, valueComparator);
                        break;
                    case ExpressionType.LessThan:
                        expression = Expression.LessThan(member, valueComparator);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(member, valueComparator);
                        break;
                }
            }
            else
                expression = GetContainsExpression(groupedProperty.ToList(), propertyInfo, member);

            expressions.Add(expression);
        }

        var condition = Expression.Lambda<Func<TEntity, bool>>(GenerateExpression(expressions, operatorType), parameter);

        return query.Where(condition);
    }

    private static Expression GetContainsExpression(List<PropertyFilter> groupedProperties, PropertyInfo propertyInfo, MemberExpression member)
    {
        if (propertyInfo.PropertyType == typeof(string))
        {
            groupedProperties.ForEach(x => x.Value = x.Value.ToString().ToLower());
            MethodInfo methodTolower = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            Expression callToLower = Expression.Call(member, methodTolower);

            var containsMethod = typeof(string).GetMethods()
                .First(p =>
                    p.Name == "Contains" &&
                    p.GetParameters().Length == 1 &&
                    p.GetParameters()[0].ParameterType == typeof(string));
            var expressions = new List<Expression>();

            groupedProperties.ForEach(p =>
                expressions.Add(Expression.Call(callToLower, containsMethod, Expression.Constant(ChangeType(p.GetValue(), propertyInfo.PropertyType)))));

            return GenerateExpression(expressions, ExpressionType.Or);
        }
        else
        {
            var listType = typeof(List<>).MakeGenericType(propertyInfo.PropertyType);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");

            groupedProperties.ForEach(p =>
            {
                var value = p.GetValue();
                if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in jsonElement.EnumerateArray())
                    {
                        object? itemValue = item.ValueKind switch
                        {
                            JsonValueKind.String => item.GetString(),
                            JsonValueKind.Number => item.GetInt32(),
                            JsonValueKind.True => true,
                            JsonValueKind.False => false,
                            _ => item.ToString()
                        };
                        if (itemValue != null)
                        {
                            addMethod?.Invoke(list, new object[] { ChangeType(itemValue, propertyInfo.PropertyType) });
                        }
                    }
                }
                else
                {
                    addMethod?.Invoke(list, new object[] { ChangeType(value, propertyInfo.PropertyType) });
                }
            });

            return Expression.Call(Expression.Constant(list), listType.GetMethod("Contains"), member);
        }
    }

    private static object ChangeType(object value, Type propertyType)
    {
        try
        {
            if (value.GetType() == propertyType)
                return value;

            var nullableType = Nullable.GetUnderlyingType(propertyType);
            if (nullableType != null)
            {
                if (value == null)
                    return null;
                else
                    propertyType = nullableType;
            }

            if (propertyType.IsEnum)
                return Enum.Parse(propertyType, value.ToString(), true);

            if (propertyType == typeof(string) && value != null)
                return value.ToString().Trim();

            if (propertyType == typeof(Guid))
                return new Guid(value.ToString());

            return Convert.ChangeType(value, propertyType);
        }
        catch
        {
            return Activator.CreateInstance(propertyType);
        }
    }

    private static Expression GenerateExpression(List<Expression> expressions, ExpressionType operatorType, int index = 0)
    {
        if (index == expressions.Count - 1)
            return expressions[index];
        else
            return operatorType == ExpressionType.And ?
                Expression.And(expressions[index], GenerateExpression(expressions, operatorType, index + 1)) :
                Expression.Or(expressions[index], GenerateExpression(expressions, operatorType, index + 1));
    }

    private static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, IList<PropertySort> propertiesToSort)
    {
        var entityType = typeof(TEntity);
        bool firstOrder = true;

        if (propertiesToSort == null || !propertiesToSort.Any(p => p.IsValid()))
            return query;

        foreach (var propertyToSort in propertiesToSort.Where(p => p.IsValid()))
        {
            var propertyInfo = entityType.GetProperty(propertyToSort.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyToSort.Name);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            var methodName = firstOrder ?
                propertyToSort.Ascending ? "OrderBy" : "OrderByDescending" :
                propertyToSort.Ascending ? "ThenBy" : "ThenByDescending";

            firstOrder = false;

            var enumarableType = typeof(Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();

                     return parameters.Count == 2;
                 }).Single();

            MethodInfo genericMethod = method
                 .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            query = (IQueryable<TEntity>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
        }

        return query;
    }
}