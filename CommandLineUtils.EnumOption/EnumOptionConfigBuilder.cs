using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.CommandLineUtils
{
    /// <summary>
    /// <para>
    /// Builder for an <see cref="EnumOptionConfig{T}"/> using a fluent interface.
    /// </para>
    /// <para>
    /// Configures the underlying <see cref="CommandOption"/> for the <see cref="CommandLineApplication"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Type of the enum the configured option will reresent.</typeparam>
    public class EnumOptionConfigBuilder<T>
        where T : struct, System.Enum // requires c# 7.3
    {
        private readonly Dictionary<string, T> _optionValueMap;
        private readonly string _template;
        private readonly string _description;

        private bool? _describeValues; // default true
        private bool? _describeArity; // default true
        private bool? _allowMultiple; // default true
        private bool? _inherited; // default false
        private bool? _throwOnInvalidOption; // default false

        private Action<CommandOption> _configuration;

        /// <summary>
        /// Initialize the <see cref="EnumOptionConfig{T}"/> with a template and description for the <see cref="CommandOption"/>.
        /// Choose a comparison method for matching option string. If left null, will default to <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="template">Template for the <see cref="CommandOption"/></param>
        /// <param name="description">Description for the <see cref="CommandOption"/>. To be show on help screen.</param>
        /// <param name="valueComparer"><see cref="StringComparer"/> for matching option values. If left null, will default to <see cref="StringComparer.OrdinalIgnoreCase"/></param>
        public EnumOptionConfigBuilder(
            string template,
            string description,
            StringComparer valueComparer = null
            )
        {
            _template = template ?? throw new ArgumentNullException(nameof(template));
            _description = description ?? throw new ArgumentNullException(nameof(description));
            valueComparer = valueComparer ?? StringComparer.OrdinalIgnoreCase;
            _optionValueMap = new Dictionary<string, T>(valueComparer);
        }

        /// <summary>
        /// Configure if the possible values for the option should be shown on the help page.
        /// <remarks>If not configured, will default to <c>true</c></remarks>
        /// </summary>
        /// <param name="describeValues">If true, the possible values for the option will be listed on the help page.</param>
        /// <exception cref="InvalidOperationException">If DescribeValues has already been configured.</exception>
        public EnumOptionConfigBuilder<T> DescribeValues(bool describeValues)
        {
            ThrowIfNotNull(_describeValues, nameof(DescribeValues));
            _describeValues = describeValues;
            return this;
        }

        /// <summary>
        /// Configure if the arity (single value or multi value) of the option should be shown on the help page.
        /// <remarks>If not configured, will default to <c>true</c></remarks>
        /// </summary>
        /// <param name="describeValues">If true, the possible values for the option will be listed on the help page.</param>
        /// <exception cref="InvalidOperationException">If DescribeArity has already been configured.</exception>
        public EnumOptionConfigBuilder<T> DescribeArity(bool describeArity)
        {
            ThrowIfNotNull(_describeArity, nameof(DescribeArity));
            _describeArity = describeArity;
            return this;
        }

        /// <summary>
        /// Configure if the the option can have multiple value.
        /// <remarks>If not configured, will default to <c>true</c></remarks>
        /// <example>A multi-valued option can be called like "<c>-name Benedict -name Carol</c>"</example>
        /// </summary>
        /// <param name="describeValues">If true, the option may have multiple value. Otherwise, it will be limited to one.</param>
        /// <exception cref="InvalidOperationException">If AllowMultiple has already been configured.</exception>
        public EnumOptionConfigBuilder<T> AllowMultiple(bool allowMultiple)
        {
            ThrowIfNotNull(_allowMultiple, nameof(AllowMultiple));
            _allowMultiple = allowMultiple;
            return this;
        }

        /// <summary>
        /// Configure if the the option will be inherited by sub-commands of the 
        /// <see cref="CommandLineApplication"/>.
        /// <remarks>If not configured, will default to <c>false</c></remarks>
        /// </summary>
        /// <param name="inherited">
        /// If true, the option will be inherited by sub-commands. 
        /// Otherwise, it will be limited the command for which it is configured.
        /// </param>
        /// <exception cref="InvalidOperationException"> If Inherited has already been configured.</exception>
        public EnumOptionConfigBuilder<T> Inherited(bool inherited)
        {
            ThrowIfNotNull(_inherited, nameof(Inherited));
            _inherited = inherited;
            return this;
        }

        /// <summary>
        /// Configure if the the option should throw if it encounters a value for which it isn't configured.
        /// <remarks>If not configured, will default to <c>false</c></remarks>
        /// </summary>
        /// <param name="throwOnInvalidOption">
        /// If true, the option will throw if it receives a value for which it isn't configured. 
        /// Otherwise, it will silently ignore the value when parsing to <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">If ThrowOnInvalidOption has already been configured.</exception>
        public EnumOptionConfigBuilder<T> ThrowOnInvalidOption(bool throwOnInvalidOption)
        {
            ThrowIfNotNull(_throwOnInvalidOption, nameof(ThrowOnInvalidOption));
            _throwOnInvalidOption = throwOnInvalidOption;
            return this;
        }

        /// <summary>
        /// Configure additional configuration for the underlying <see cref="CommandOption"/>.
        /// <remarks>Will be executed before argument parsing. No values will be present.</remarks>
        /// <remarks>If not configured, will default to doing nothing.</remarks>
        /// </summary>
        /// <param name="configuration">
        /// Additional configuration for the underlying <see cref= "CommandOption" />
        /// </param>
        /// <exception cref="InvalidOperationException">If Configuration has already been configured.</exception>
        public EnumOptionConfigBuilder<T> Configuration(Action<CommandOption> configuration)
        {
            ThrowIfNotNull(_configuration, nameof(Configuration));
            _configuration = configuration;
            return this;
        }

        /// <summary>
        /// Configure the option to use <paramref name="value"/>, with the default <c>ToString</c> as the textual representation.
        /// </summary>
        /// <param name="value">The value to use for the option, with the default <c>ToString</c> as the textual representation.</param>
        /// <exception cref="ArgumentException">If <c><paramref name="value"/>.ToString()</c> is already used by another option value.</exception>
        public EnumOptionConfigBuilder<T> Use(T value)
        {
            var representation = value.ToString();

            if (_optionValueMap.TryGetValue(representation, out T existing))
            {
                throw new InvalidOperationException(
                    $"The option {representation} is already in use for the value {existing}."
                );
            }

            _optionValueMap.Add(representation, value);

            return this;
        }

        /// <summary>
        /// Configure the option to use <paramref name="enumValue"/>, with <paramref name="value"/> as the textual representation.
        /// </summary>
        /// <param name="value">The textual representation of the option value.</param>
        /// <param name="enumValue">The value of the option value</param>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is already used by another option value.</exception>
        public EnumOptionConfigBuilder<T> Use(string value, T enumValue)
        {
            if (_optionValueMap.TryGetValue(value, out T existing))
            {
                throw new InvalidOperationException(
                    $"The option {value} is already in use for the value {existing}."
                );
            }

            _optionValueMap.Add(value, enumValue);

            return this;
        }

        /// <summary>
        /// Builds the configured <see cref="EnumOptionConfig{T}"/>
        /// </summary>
        public EnumOptionConfig<T> Build()
        {
            if (_optionValueMap.Count == 0)
            {
                throw new InvalidOperationException("No values configured for the option.");
            }

            // A "do nothing" config to use by default if no configuration is set.
            void nullConfig(CommandOption o) { }

            return new EnumOptionConfig<T>(
                _optionValueMap,
                _template,
                _description,
                _describeValues ?? true,
                _describeArity ?? true,
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
}