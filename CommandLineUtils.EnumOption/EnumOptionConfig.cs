using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.CommandLineUtils
{

    public class EnumOptionConfig<T>
        where T : struct, System.Enum // requires c# 7.3
    {
        public IReadOnlyDictionary<string, T> OptionValueMap { get; }
        public string Template { get; }
        public string Description { get; }
        public bool DescribeValues { get; }
        public bool DescribeArity { get; }
        public bool Inherited { get; }
        public bool AllowMultiple { get; }
        public bool ThrowOnInvalidOption { get; }
        public Action<CommandOption> Configuration { get; }
        public EnumOptionConfig(
            IReadOnlyDictionary<string, T> optionValueMap,
            string template,
            string description,
            bool describeValues,
            bool describeArity,
            bool inherited,
            bool allowMultiple,
            bool throwOnInvalidOption,
            Action<CommandOption> configuration
            )
        {
            OptionValueMap = optionValueMap;
            Template = template;
            Description = description;
            DescribeValues = describeValues;
            DescribeArity = describeArity;
            Inherited = inherited;
            AllowMultiple = allowMultiple;
            ThrowOnInvalidOption = throwOnInvalidOption;
            Configuration = configuration;
        }
    }
}
