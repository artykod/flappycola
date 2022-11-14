using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionFloat : BaseEqualityDataFilterCondition<float>
    {
        public DataFilterConditionFloat(string path, float value, EqualTypes equalType):
            base(path, ConditionTypes.Float, value, equalType)
        {}

        protected override float GetValue(IDataProperty property) => property.GetValue<float>();
        protected override bool IsEqual(float a, float b) => Math.Abs(a - b) < float.Epsilon;
        protected override bool IsLower(float a, float b) => a - b < float.Epsilon;
        protected override bool IsGreater(float a, float b) => a - b > float.Epsilon;
    }
}