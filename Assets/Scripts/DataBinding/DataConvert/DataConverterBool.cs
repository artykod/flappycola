namespace DataBinding
{
    public static partial class DataConverter
    {
        private class BoolFloatConverter : ConverterEntry<bool, float>
        {
            public override float Convert(bool src) => src ? 1f : 0f;
        }

        private class BoolDoubleConverter : ConverterEntry<bool, double>
        {
            public override double Convert(bool src) => src ? 1d : 0d;
        }

        private class BoolIntConverter : ConverterEntry<bool, int>
        {
            public override int Convert(bool src) => src ? 1 : 0;
        }

        private class BoolLongConverter : ConverterEntry<bool, long>
        {
            public override long Convert(bool src) => src ? 1L : 0L;
        }

        private class StringBoolConverter : ConverterEntry<string, bool>
        {
            public override bool Convert(string src) => bool.TryParse(src, out var result);
        }
    }
}