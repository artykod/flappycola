using System;
using System.Collections.Generic;

namespace DataBinding
{
    public interface IDataSource : IDataNode
    {
        void AddNode(IDataNode node);
        void RemoveNode(string nodeName);
        bool TryGetNode<T>(ulong nodeHash, out T result) where T : IDataNode;
        bool TryGetNode<T>(string nodeName, out T result) where T : IDataNode;
        bool TryGetNodeByPath<T>(string nodePath, out T result) where T : IDataNode;
        void ForEach<T>(Action<T> action) where T : IDataNode;
        T FirstOrDefault<T>(Func<T, bool> predicate) where T: IDataNode;
        T GetDataValueOrDefault<T>(string nodePath);
        void PatchWith(IDataSource dataSource);

        static ulong HashNodeName(string value)
        {
            var hashedValue = 3074457345618258791ul;

            foreach (var ch in value)
            {
                hashedValue += ch;
                hashedValue *= 3074457345618258799ul;
            }

            return hashedValue;
        }

        static ulong HashNodeName(string value, int idx, int length)
        {
            var hashedValue = 3074457345618258791ul;

            for (int i = idx, l = idx + length; i < l; ++i)
            {
                hashedValue += value[i];
                hashedValue *= 3074457345618258799ul;
            }

            return hashedValue;
        }
    }

    public class DataSource : IDataSource
    {
        private readonly string _name;
        private readonly Dictionary<ulong, IDataNode> _dataNodes;
        private DataObservatorsContainer _subscriberContainer;

        public string Name => _name;

        public DataSource() : this(null)
        { }

        public DataSource(string name)
        {
            _name = name ?? GetType().Name;
            _dataNodes = new Dictionary<ulong, IDataNode>();
        }

        public void AddNode(IDataNode node)
        {
            _dataNodes[IDataSource.HashNodeName(node.Name)] = node;

            _subscriberContainer?.HandleChanges();
        }

        public void RemoveNode(string name)
        {
            _dataNodes.Remove(IDataSource.HashNodeName(name));

            _subscriberContainer?.HandleChanges();
        }

        public bool TryGetNode<T>(ulong nodeHash, out T result) where T : IDataNode
        {
            if (_dataNodes.TryGetValue(nodeHash, out var node) && node is T resultNode)
            {
                result = resultNode;

                return true;
            }

            result = default;

            return false;
        }

        public bool TryGetNode<T>(string nodeName, out T result) where T : IDataNode
        {
            return TryGetNode(IDataSource.HashNodeName(nodeName), out result);
        }

        public bool TryGetNodeByPath<T>(string nodePath, out T result) where T : IDataNode
        {
            result = default;

            var dataSource = (IDataSource)this;
            var node = default(IDataNode);
            var nameIdx = 0;
            var nameLength = 0;

            for (int i = 0, l = nodePath.Length; i < l; ++i)
            {
                var ch = nodePath[i];

                if (ch != '.' && ch != '[' && ch != ']')
                {
                    ++nameLength;

                    if (i < l - 1)
                    {
                        continue;
                    }
                }

                var nameHash = IDataSource.HashNodeName(nodePath, nameIdx, nameLength);

                if (nameLength > 0 && !dataSource.TryGetNode<IDataNode>(nameHash, out node))
                {
                    node = null;

                    break;
                }

                nameIdx = i + 1;
                nameLength = 0;

                if (node is IDataSource)
                {
                    dataSource = node as IDataSource;
                }
            }

            if (node is T)
            {
                result = (T)node;

                return true;
            }

            return false;
        }

        public void ForEach<T>(Action<T> action) where T : IDataNode
        {
            if (action == null)
            {
                return;
            }

            foreach (var pair in _dataNodes)
            {
                if (pair.Value is T target)
                {
                    action.Invoke(target);
                }
            }
        }

        public T FirstOrDefault<T>(Func<T, bool> predicate) where T: IDataNode
        {
            foreach (var pair in _dataNodes)
            {
                if (pair.Value is T targetProperty && (predicate == null || predicate.Invoke(targetProperty)))
                {
                    return targetProperty;
                }
            }

            return default;
        }

        public T GetDataValueOrDefault<T>(string nodePath)
        {
            return TryGetNodeByPath<IDataProperty>(nodePath, out var property) ? property.GetValue<T>() : default;
        }

        public void Subscribe(IDataObservator subscriber)
        {
            _subscriberContainer = _subscriberContainer ?? new DataObservatorsContainer(this);

            _subscriberContainer.Subscribe(subscriber);
        }

        public void Unsubscribe(IDataObservator subscriber)
        {
            _subscriberContainer?.Unsubscribe(subscriber);
        }

        public IDataNode Clone(string name)
        {
            var dataSource = new DataSource(name);

            foreach (var kvp in _dataNodes)
            {
                var node = kvp.Value;

                dataSource.AddNode(node.Clone(node.Name));
            }

            return dataSource;
        }

        public void PatchWith(IDataSource dataSource)
        {
            dataSource?.ForEach<IDataNode>(i => AddNode(i.Clone(i.Name)));
        }
    }
}