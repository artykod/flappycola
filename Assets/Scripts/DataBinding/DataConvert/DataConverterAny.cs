namespace DataBinding
{
    public static partial class DataConverter
    {
        private class AnyStringConverter<SrcT> : ConverterEntry<SrcT, string>
        {
            public override string Convert(SrcT src) => src?.ToString() ?? "null";
        }

        private class AnyIdenticalConverter<T> : ConverterEntry<T, T>
        {
            public override T Convert(T src) => src;
        }
    }
}