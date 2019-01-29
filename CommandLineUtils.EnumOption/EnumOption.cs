using System;
using System.Collections.Generic;
using System.Linq;
namespace Microsoft.Extensions.CommandLineUtils
{

    /// <summary>
    /// A wrapper for <see cref="CommandOption"/> that automatically maps values to enum value.
    /// </summary>
    /// <typeparam name="T">Type of the enum</typeparam>
    public class EnumOption<T>
    where T : struct, System.Enum // requires c# 7.3
    {
        private readonly CommandOption _opt;
        private readonly IReadOnlyDictionary<string, T> _optionValueMap;
        private readonly bool _throwOnInvalidOption;

        public string Template { get => _opt.Template; set => _opt.Template = value; }
        public string ShortName { get => _opt.ShortName; set => _opt.ShortName = value; }
        public string LongName { get => _opt.LongName; set => _opt.LongName = value; }
        public string SymbolName { get => _opt.SymbolName; set => _opt.SymbolName = value; }
        public string ValueName { get => _opt.ValueName; set => _opt.ValueName = value; }
        public string Description { get => _opt.Description; set => _opt.Description = value; }
        public CommandOptionType OptionType => _opt.OptionType;
        public bool ShowInHelpText { get => _opt.ShowInHelpText; set => _opt.ShowInHelpText = value; }
        public bool Inherited { get => _opt.Inherited; set => _opt.Inherited = value; }

        /// <summary>
        /// The values passed by argument to the option.
        /// </summary>
        public IEnumerable<T> Values => _opt
            .Values
            .Select(v => GetEnumValue(v))
            .Where(v => v != null)
            .Select(v => v.Value);

        /// <summary>
        /// The single value passed by argument to the option.
        /// </summary>
        public T Value => Values.Single();



        public EnumOption(
            CommandOption opt,
            IReadOnlyDictionary<string, T> optionValueMap,
            bool throwOnInvalidOption
        )
        {
            _opt = opt;
            _optionValueMap = optionValueMap;
            _throwOnInvalidOption = throwOnInvalidOption;
        }

        private T? GetEnumValue(string option)
        {
            if (_optionValueMap.TryGetValue(option, out T value))
            {
                return value;
            }

            if (_throwOnInvalidOption)
            {
                throw new ArgumentException(
                    $"No option configured for '{option}'.", nameof(option)
                );
            }

            return null;
        }
    }
}
