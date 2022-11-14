namespace DataBinding
{
    public static partial class DataConverter
    {
        private class IntFloatConverter : ConverterEntry<int, float>
        {
            public override float Convert(int src) => src;
        }

        private class IntLongConverter : ConverterEntry<int, long>
        {
            public override long Convert(int src) => src;
        }

        private class IntDoubleConverter : ConverterEntry<int, double>
        {
            public override double Convert(int src) => src;
        }

        private class IntBoolConverter : ConverterEntry<int, bool>
        {
            public override bool Convert(int src) => src != 0;
        }

        private class StringIntConverter : ConverterEntry<string, int>
        {
            public override int Convert(string src) => int.TryParse(src, out var result) ? result : default;
        }
    }
}