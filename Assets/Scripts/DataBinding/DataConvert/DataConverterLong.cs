namespace DataBinding
{
    public static partial class DataConverter
    {
        private class LongIntConverter : ConverterEntry<long, int>
        {
            public override int Convert(long src) => (int)src;
        }

        private class LongFloatConverter : ConverterEntry<long, float>
        {
            public override float Convert(long src) => src;
        }

        private class LongDoubleConverter : ConverterEntry<long, double>
        {
            public override double Convert(long src) => src;
        }

        private class LongBoolConverter : ConverterEntry<long, bool>
        {
            public override bool Convert(long src) => src != 0L;
        }

        private class StringLongConverter : ConverterEntry<string, long>
        {
            public override long Convert(string src) => long.TryParse(src, out var result) ? result : default;
        }
    }
}