using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;

public class EnumOptionConfigBuilder<T>
        where T : struct, System.Enum // requires c# 7.3
{
    private readonly Dictionary<string, T> _optionValueMap;
    private readonly string _template;
    private readonly string _description;
    private bool? _allowMultiple; // default true
    private bool? _inherited; // default false
    private bool? _throwOnInvalidOption; // default false

    private Action<CommandOption> _configuration;

    public EnumOptionConfigBuilder(
        string template,
        string description,
        StringComparer flagComparer = null
        )
    {
        flagComparer = flagComparer ?? StringComparer.OrdinalIgnoreCase;
        _optionValueMap = new Dictionary<string, T>(flagComparer);
        _template = template;
        _description = description;
    }

    public EnumOptionConfigBuilder<T> AllowMultiple(bool allowMultiple)
    {
        ThrowIfNotNull(_allowMultiple, nameof(AllowMultiple));
        _allowMultiple = allowMultiple;
        return this;
    }

    public EnumOptionConfigBuilder<T> Inherited(bool inherited)
    {
        ThrowIfNotNull(_inherited, nameof(Inherited));
        _inherited = inherited;
        return this;
    }

    public EnumOptionConfigBuilder<T> ThrowOnInvalidOption(bool throwOnInvalidOption)
    {
        ThrowIfNotNull(_throwOnInvalidOption, nameof(ThrowOnInvalidOption));
        _throwOnInvalidOption = throwOnInvalidOption;
        return this;
    }

    public EnumOptionConfigBuilder<T> Configuration(Action<CommandOption> configuration)
    {
        ThrowIfNotNull(_configuration, nameof(Configuration));
        _configuration = configuration;
        return this;
    }

    public EnumOptionConfigBuilder<T> Use(T option)
    {
        var representation = option.ToString();

        if (_optionValueMap.TryGetValue(representation, out T existing))
        {
            throw new ArgumentException(
                $"The option {representation} is already in use for the value {existing}."
            );
        }

        _optionValueMap.Add(representation, option);

        return this;
    }

    public EnumOptionConfigBuilder<T> Use(string option, T value)
    {
        if (_optionValueMap.TryGetValue(option, out T existing))
        {
            throw new ArgumentException(
                $"The option {option} is already in use for the value {existing}."
            );
        }

        _optionValueMap.Add(option, value);

        return this;
    }

    public EnumOptionConfig<T> Build()
    {
        void nullConfig(CommandOption o) { }

        return new EnumOptionConfig<T>(
            _optionValueMap,
            _template,
            _description,
            _inherited ?? false,
            _allowMultiple ?? true,
            _throwOnInvalidOption ?? false,
            _configuration ?? nullConfig
        );
    }

    private void ThrowIfNotNull(object o, string name)
    {
        if (o != null)
        {
            throw new InvalidOperationException($"{name} has already been set");
        }
    }
}
