using System;

namespace DataBinding
{
    [Serializable]
    public class DataFilterConditionHasKey : BaseDataFilterCondition
    {
        public DataFilterConditionHasKey(string path): base(path, ConditionTypes.HasKey)
        {}

        public override bool IsMatch(IDataSource dataSource)
        {
            return dataSource.TryGetNodeByPath<IDataNode>(Path, out _);
        }
    }
}