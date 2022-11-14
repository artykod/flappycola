using UnityEngine;

namespace DataBinding
{
    public static partial class DataConverter
    {
        private class ColorStringConverter : ConverterEntry<Color, string>
        {
            public override string Convert(Color src) => ColorUtility.ToHtmlStringRGBA(src);
        }

        private class StringColorConverter : ConverterEntry<string, Color>
        {
            public override Color Convert(string src) => ColorUtility.TryParseHtmlString(src, out var color) ? color : Color.white;
        }
    }
}