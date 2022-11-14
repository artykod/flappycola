namespace DataBinding
{
    public static partial class DataConverter
    {
        private class DoubleFloatConverter : ConverterEntry<double, float>
        {
            public override float Convert(double src) => (float)src;
        }

        private class DoubleIntConverter : ConverterEntry<double, int>
        {
            public override int Convert(double src) => (int)src;
        }

        private class DoubleLongConverter : ConverterEntry<double, long>
        {
            public override long Convert(double src) => (long)src;
        }

        private class DoubleBoolConverter : ConverterEntry<double, bool>
        {
            public override bool Convert(double src) => src != 0d;
        }

        private class StringDoubleConverter : ConverterEntry<string, double>
        {
            public override double Convert(string src) => double.TryParse(src, out var result) ? result : default;
        }
    }
}