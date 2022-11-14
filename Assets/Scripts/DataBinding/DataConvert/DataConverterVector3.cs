using UnityEngine;

namespace DataBinding
{
    public static partial class DataConverter
    {
        private class Vector3StringConverter : ConverterEntry<Vector3, string>
        {
            public override string Convert(Vector3 src) => src.ToString();
        }

        private class StringVector3Converter : ConverterEntry<string, Vector3>
        {
            public override Vector3 Convert(string src)
            {
                if (src.StartsWith("(") && src.EndsWith(")"))
                {
                    src = src.Substring(1, src.Length - 2);
                }
        
                var sArray = src.Split(',');
        
                return new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
            }
        }
    }
}