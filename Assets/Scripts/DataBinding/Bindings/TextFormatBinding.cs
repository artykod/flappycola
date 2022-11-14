using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DataBinding
{
    public class TextFormatBinding : BaseTextBinding
    {
        [Serializable]
        public class ReplacementStatic
        {
            public string Key;
            public string Value;
        }

        [Serializable]
        public class ReplacementConst
        {
            public string Key;
            public string Path;
        }

        [SerializeField] private string _value;
        [SerializeField] private bool _isPath = true;
        [SerializeField] private ReplacementConst[] _replacementConst;
        [SerializeField] private ReplacementStatic[] _replacementStatic;

        private static readonly StringBuilder _cachedStringBuilder = new StringBuilder(128);

        protected override void FillPathToSubscription(List<string> result)
        {
            if (_isPath)
            {
                result.Add(_value);
            }

            foreach (var replacement in _replacementConst)
            {
                result.Add(replacement.Path);
            }
        }

        protected override void BindDataInternal(IDataNode property)
        {
            var valueStr = _value;

            if (_isPath && DataSource.TryGetNodeByPath<IDataProperty>(_value, out var valueProperty))
            {
                valueStr = valueProperty.GetValue<string>();
            }

            _cachedStringBuilder.Clear();
            _cachedStringBuilder.Append(ProcessText(valueStr));

            foreach (var replacement in _replacementConst)
            {
                if (DataSource.TryGetNodeByPath<IDataProperty>(replacement.Path, out var replacementProperty))
                {
                    _cachedStringBuilder.Replace(replacement.Key, replacementProperty.GetValue<string>());
                }
            }

            foreach (var replacement in _replacementStatic)
            {
                _cachedStringBuilder.Replace(replacement.Key, replacement.Value);
            }

            ApplyText(_cachedStringBuilder.ToString());
            _cachedStringBuilder.Clear();
        }
    }
}