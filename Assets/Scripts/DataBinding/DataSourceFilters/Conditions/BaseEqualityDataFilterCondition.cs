using System;

namespace DataBinding
{
    public enum EqualTypes
    {
        Equal,
        NotEqual,
        Lower,
        LowerOrEqual,
        Greater,
        GreaterOrEqual,
    }

    [Serializable]
    public abstract class BaseEqualityDataFilterCondition<T> : BaseDataFilterCondition<T>
    {
        public EqualTypes EqualType;

        protected BaseEqualityDataFilterCondition(string path, ConditionTypes type, T value, EqualTypes equalType):
            base(path, type, value)
        {
            EqualType = equalType;
        }

        protected override bool IsMatchProperty(IDataProperty property)
        {
            var propertyValue = GetValue(property);

            switch (EqualType)
            {
                case EqualTypes.Equal:
                    return IsEqual(propertyValue, Value);
                case EqualTypes.NotEqual:
                    return !IsEqual(propertyValue, Value);
                case EqualTypes.Lower:
                    return IsLower(propertyValue, Value);
                case EqualTypes.GreaterOrEqual:
                    return !IsLower(propertyValue, Value);
                case EqualTypes.Greater:
                    return IsGreater(propertyValue, Value);
                case EqualTypes.LowerOrEqual:
                    return !IsGreater(propertyValue, Value);
                default:
                    throw new NotImplementedException($"unknown equal type {EqualType}");
            }
        }

        protected abstract T GetValue(IDataProperty property);
        protected abstract bool IsEqual(T a, T b);
        protected abstract bool IsLower(T a, T b);
        protected abstract bool IsGreater(T a, T b);
    }
}