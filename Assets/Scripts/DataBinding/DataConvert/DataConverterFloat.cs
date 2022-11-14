namespace DataBinding
{
    public static partial class DataConverter
    {
        private class FloatIntConverter : ConverterEntry<float, int>
        {
            public override int Convert(float src) => (int)src;
        }

        private class FloatLongConverter : ConverterEntry<float, long>
        {
            public override long Convert(float src) => (long)src;
        }

        private class FloatDoubleConverter : ConverterEntry<float, double>
        {
            public override double Convert(float src) => src;
        }

        private class FloatBoolConverter : ConverterEntry<float, bool>
        {
            public override bool Convert(float src) => src != 0f;
        }

        private class StringFloatConverter : ConverterEntry<string, float>
        {
            public override float Convert(string src) => float.TryParse(src, out var result) ? result : default;
        }
    }
}