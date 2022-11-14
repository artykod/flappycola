using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionNotHasKey : BaseDataFilterCondition
    {
        public DataFilterConditionNotHasKey(string path): base(path, ConditionTypes.NotHasKey)
        {}

        public override bool IsMatch(IDataSource dataSource)
        {
            return !dataSource.TryGetNodeByPath<IDataNode>(Path, out _);
        }
    }
}