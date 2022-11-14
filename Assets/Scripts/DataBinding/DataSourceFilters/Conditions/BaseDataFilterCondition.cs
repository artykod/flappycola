using System;

namespace DataBinding
{
    public enum ConditionTypes
    {
        Integer,
        Float,
        Bool,
        String,
        HasKey,
        NotHasKey,
    }

    [Serializable]
    public abstract class BaseDataFilterCondition
    {
        public string Path;
        public ConditionTypes Type;

        protected BaseDataFilterCondition(string path, ConditionTypes type)
        {
            Path = path;
            Type = type;
        }

        public abstract bool IsMatch(IDataSource dataSource);
    }

    [Serializable]
    public abstract class BaseDataFilterCondition<T> : BaseDataFilterCondition
    {
        public T Value;

        protected BaseDataFilterCondition(string path, ConditionTypes type, T value): base(path, type)
        {
            Value = value;
        }

        public override bool IsMatch(IDataSource dataSource)
        {
            if (dataSource.TryGetNodeByPath<IDataProperty>(Path, out var property))
            {
                return IsMatchProperty(property);
            }

            if (dataSource.TryGetNodeByPath<IDataSource>(Path, out var childDataSource))
            {
                return childDataSource.FirstOrDefault<IDataProperty>(IsMatchProperty) != null;
            }

            return false;
        }

        protected abstract bool IsMatchProperty(IDataProperty property);
    }
}