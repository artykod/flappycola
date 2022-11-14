using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionBool : BaseEqualityDataFilterCondition<bool>
    {
        public DataFilterConditionBool(string path, bool value, EqualTypes equalType): base(path, ConditionTypes.Bool, value, equalType)
        {
            EqualType = equalType;
        }

        protected override bool GetValue(IDataProperty property) => property.GetValue<bool>();
        protected override bool IsEqual(bool a, bool b) => a == b;
        protected override bool IsLower(bool a, bool b) => false;
        protected override bool IsGreater(bool a, bool b) => false;
    }
}