using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataBinding
{
    public abstract class BaseTextBinding : BaseDataBinding
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Text _textUnity;

        private bool _isInitialized;

        protected string ProcessText(string value)
        {
            return value;
        }

        protected void ApplyText(string value)
        {
            EnsureInitialized();

            if (_text != null)
            {
                _text.text = value;
            }

            if (_textUnity != null)
            {
                _textUnity.text = value;
            }
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
            }

            if (_textUnity == null)
            {
                _textUnity = GetComponent<Text>();
            }

            _isInitialized = true;
        }
    }
}