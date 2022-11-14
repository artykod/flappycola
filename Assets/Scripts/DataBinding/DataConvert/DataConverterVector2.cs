using UnityEngine;

namespace DataBinding
{
    public static partial class DataConverter
    {
        private class Vector2StringConverter : ConverterEntry<Vector2, string>
        {
            public override string Convert(Vector2 src) => src.ToString();
        }

        private class StringVector2Converter : ConverterEntry<string, Vector2>
        {
            public override Vector2 Convert(string src)
            {
                if (src.StartsWith("(") && src.EndsWith(")"))
                {
                    src = src.Substring(1, src.Length - 2);
                }
        
                var sArray = src.Split(',');
        
                return new Vector2(float.Parse(sArray[0]), float.Parse(sArray[1]));
            }
        }
    }
}