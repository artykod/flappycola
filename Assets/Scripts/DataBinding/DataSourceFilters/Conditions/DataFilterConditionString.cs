using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionString : BaseEqualityDataFilterCondition<string>
    {
        public DataFilterConditionString(string path, string value, EqualTypes equalType):
            base(path, ConditionTypes.String, value, equalType)
        {}

        protected override string GetValue(IDataProperty property) => property.GetValue<string>();

        protected override bool IsEqual(string a, string b) =>
            string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b) || string.Equals(a, b, StringComparison.Ordinal);

        protected override bool IsLower(string a, string b) => (a?.Length ?? 0) < (b?.Length ?? 0);
        protected override bool IsGreater(string a, string b) => (a?.Length ?? 0) >= (b?.Length ?? 0);
    }
}