using UnityEngine;

namespace DataBinding
{
    public static partial class DataConverter
    {
        private class Color32StringConverter : ConverterEntry<Color32, string>
        {
            public override string Convert(Color32 src) => ColorUtility.ToHtmlStringRGBA(src);
        }

        private class StringColor32Converter : ConverterEntry<string, Color32>
        {
            public override Color32 Convert(string src)
            {
                return ColorUtility.TryParseHtmlString(src, out var color) ? color : new Color32(255, 255, 255, 255);
            }
        }
    }
}