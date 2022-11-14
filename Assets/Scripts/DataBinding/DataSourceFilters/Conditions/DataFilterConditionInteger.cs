using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionInteger : BaseEqualityDataFilterCondition<int>
    {
        public DataFilterConditionInteger(string path, int value, EqualTypes equalType):
            base(path, ConditionTypes.Integer, value, equalType)
        {}

        protected override int GetValue(IDataProperty property) => property.GetValue<int>();
        protected override bool IsEqual(int a, int b) => a == b;
        protected override bool IsLower(int a, int b) => a < b;
        protected override bool IsGreater(int a, int b) => a > b;
    }
}