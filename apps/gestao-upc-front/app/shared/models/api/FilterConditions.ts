/**
 * Enum representing filter conditions for API queries
 * These values should match the backend FilterCondition enum
 */
export enum FilterCondition {
    Equals = 13,
    NotEquals = 1,
    GreaterThan = 2,
    GreaterThanOrEqual = 3,
    LessThan = 4,
    LessThanOrEqual = 5,
    Contains = 6,
    StartsWith = 7,
    EndsWith = 8,
    IsNull = 9,
    IsNotNull = 10,
    In = 11,
    NotIn = 12,
    Or = 36,
    OrAssign = 70,
}

/**
 * Helper function to get filter condition description
 */
export const getFilterConditionDescription = (condition: FilterCondition): string => {
    switch (condition) {
        case FilterCondition.Equals:
            return 'Equals';
        case FilterCondition.NotEquals:
            return 'Not Equals';
        case FilterCondition.GreaterThan:
            return 'Greater Than';
        case FilterCondition.GreaterThanOrEqual:
            return 'Greater Than or Equal';
        case FilterCondition.LessThan:
            return 'Less Than';
        case FilterCondition.LessThanOrEqual:
            return 'Less Than or Equal';
        case FilterCondition.Contains:
            return 'Contains';
        case FilterCondition.StartsWith:
            return 'Starts With';
        case FilterCondition.EndsWith:
            return 'Ends With';
        case FilterCondition.IsNull:
            return 'Is Null';
        case FilterCondition.IsNotNull:
            return 'Is Not Null';
        case FilterCondition.In:
            return 'In';
        case FilterCondition.NotIn:
            return 'Not In';
        default:
            return 'Unknown';
    }
};
