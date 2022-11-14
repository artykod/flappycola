using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace DataBinding
{
    public static partial class DataConverter
    {
        private interface IConverterEntry
        {
            long GetEntryHash();
        }

        private abstract class ConverterEntry<SrcT, DestT> : IConverterEntry
        {
            public long GetEntryHash() => GetEntryHash<SrcT, DestT>();

            public abstract DestT Convert(SrcT src);
        }

        static Dictionary<long, IConverterEntry> ConverterEntries = new Dictionary<long, IConverterEntry>(32);

        static DataConverter()
        {
            foreach (var nestedType in typeof(DataConverter).GetTypeInfo().DeclaredNestedTypes)
            {
                if (!nestedType.IsGenericType && !nestedType.IsAbstract && !nestedType.IsInterface &&
                    typeof(IConverterEntry).IsAssignableFrom(nestedType))
                {
                    var converter = (IConverterEntry)Activator.CreateInstance(nestedType);
                    var hash = converter.GetEntryHash();

                    if (ConverterEntries.TryGetValue(hash, out var existingConverter))
                    {
                        Debug.LogError($"Hash duplicate: {converter.GetType().Name} and {existingConverter.GetType().Name}");
                    }
                    else
                    {
                        ConverterEntries.Add(hash, converter);
                    }
                }
            }
        }

        private static long GetEntryHash<SrcT, DestT>()
        {
            var srcHash = (long)typeof(SrcT).GetHashCode();
            var destHash = (long)typeof(DestT).GetHashCode();

            return (srcHash << 32) + destHash;
        }

        public static DestT Convert<SrcT, DestT>(SrcT src)
        {
            var entryHash = GetEntryHash<SrcT, DestT>();
            var converter = default(ConverterEntry<SrcT, DestT>);

            if (ConverterEntries.TryGetValue(entryHash, out var converterRef))
            {
                converter = converterRef as ConverterEntry<SrcT, DestT>;
            }
            else
            {
                if (typeof(DestT) == typeof(SrcT))
                {
                    converter = new AnyIdenticalConverter<DestT>() as ConverterEntry<SrcT, DestT>;

                    ConverterEntries.Add(entryHash, converter);
                }
                else if (typeof(DestT) == typeof(string))
                {
                    converter = new AnyStringConverter<SrcT>() as ConverterEntry<SrcT, DestT>;

                    ConverterEntries.Add(entryHash, converter);
                }
            }

            if (converter != null)
            {
                var dest = converter.Convert(src);

                //Debug.Log($"{converter.GetType().Name}: {src}({typeof(SrcT).Name}) -> {dest}({typeof(DestT).Name})");

                return dest;
            }

            try
            {
                var fallbackDest = (DestT)System.Convert.ChangeType(src, typeof(DestT));

                //Debug.Log($"Fallback: {src}({typeof(SrcT).Name}) -> {fallbackDest}({typeof(DestT).Name})");

                return fallbackDest;
            }
            catch
            {
                Debug.LogWarning($"No DataConverter<{typeof(SrcT).Name}, {typeof(DestT).Name}> and cannot fallback via boxing");
            }

            return default;
        }
    }
}