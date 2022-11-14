using SimpleJSON;

namespace DataBinding
{
    public static class DataSourceFactory
    {
        public static IDataSource FromJson(string dataSourceName, string jsonString)
        {
            var jsonRoot = JSON.Parse(jsonString);
            var dataSource = new DataSource(dataSourceName);

            PopulateWithJsonNode(dataSource, jsonRoot);

            return dataSource;
        }

        private static void PopulateWithJsonNode(IDataSource dataSource, JSONNode jsonNode)
        {
            if (jsonNode.IsObject)
            {
                foreach (var key in jsonNode.Keys)
                {
                    var keyNode = jsonNode[key];
                    var keyDataNode = ConvertJsonNodeToDataNode(key, keyNode);

                    if (keyDataNode != null)
                    {
                        dataSource.AddNode(keyDataNode);

                        if (keyDataNode is IDataSource keyDataSource)
                        {
                            PopulateWithJsonNode(keyDataSource, keyNode);
                        }
                    }
                }
            }
            else if (jsonNode.IsArray)
            {
                var idx = 0;

                foreach (var itemNode in jsonNode.Values)
                {
                    var key = $"{idx++}";
                    var itemDataNode = ConvertJsonNodeToDataNode(key, itemNode);

                    if (itemDataNode != null)
                    {
                        dataSource.AddNode(itemDataNode);

                        if (itemDataNode is IDataSource itemDataSource)
                        {
                            PopulateWithJsonNode(itemDataSource, itemNode);
                        }
                    }
                }
            }
        }

        private static IDataNode ConvertJsonNodeToDataNode(string name, JSONNode jsonNode)
        {
            switch (jsonNode.Tag)
            {
                case JSONNodeType.Number:
                {
                    if (jsonNode.Value.Contains('.'))
                    {
                        return jsonNode.Value.Length > 8
                            ? new DataProperty<double>(name, jsonNode.AsDouble)
                            : new DataProperty<float>(name, jsonNode.AsFloat);
                    }

                    return jsonNode.Value.Length > 6
                            ? new DataProperty<long>(name, jsonNode.AsLong)
                            : new DataProperty<int>(name, jsonNode.AsInt);
                }

                case JSONNodeType.String:
                    return new DataProperty<string>(name, jsonNode.Value);

                case JSONNodeType.Boolean:
                    return new DataProperty<bool>(name, jsonNode.AsBool);

                case JSONNodeType.NullValue:
                    return new DataProperty<object>(name, null);

                case JSONNodeType.Array:
                    return new DataSource(name);

                case JSONNodeType.Object:
                    return new DataSource(name);

                default:
                    return null;
            }
        }
    }
}