using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Microsoft.Extensions.CommandLineUtils.Test
{
    [TestClass]
    public class CommandLineApplicationExtensionsTest
    {
        private static readonly Dictionary<string, TestEnum> valueMap =
        new Dictionary<string, TestEnum>()
        {
            { "abc",TestEnum.abc }
        };

        private const string template = "-t | --test";
        private const string desc = "Much test.";

        private const bool describeValue = false;
        private const bool describeArity = false;
        private const bool inherited = false;
        private const bool allowMultiple = false;
        private const bool throwOnInvalidOption = false;

        private static void Configuration(CommandOption o) { }

        /// <summary>
        /// Checks if the template was used
        /// </summary>
        [TestMethod]
        public void Uses_template()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, desc, describeValue,
                describeArity, inherited, allowMultiple,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.AreEqual(template, opt.Template);
            Assert.AreEqual("t", opt.ShortName);
            Assert.AreEqual("test", opt.LongName);
        }

        /// <summary>
        /// Checks if the arity is correctly mapped when allow multiple is true
        /// </summary>
        [TestMethod]
        public void Allow_Multiple_Option_type()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, desc, describeValue,
                describeArity, inherited, true,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.AreEqual(CommandOptionType.MultipleValue, opt.OptionType);
        }

        /// <summary>
        /// Checks if the arity is correctly mapped when allow multiple is false
        /// </summary>
        [TestMethod]
        public void Not_Allow_Multiple_Option_type()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, desc, describeValue,
                describeArity, inherited, false,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.AreEqual(CommandOptionType.SingleValue, opt.OptionType);
        }

        /// <summary>
        /// The description shouldn't be moddified if not instructed to.
        /// </summary>
        [TestMethod]
        public void Description_unchanged()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, string.Empty, false,
                false, inherited, allowMultiple,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.AreEqual(string.Empty, opt.Description);
        }

        /// <summary>
        /// The description should refelct the arity.
        /// </summary>
        [TestMethod]
        public void Description_single_arity()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, string.Empty, describeValue,
                true, inherited, false,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.IsTrue(opt.Description.Contains("single"));
        }

        /// <summary>
        /// The description should refelct the arity.
        /// </summary>
        [TestMethod]
        public void Description_multiple_arity()
        {
            var cla = new CommandLineApplication();

            var optionConfig = new EnumOptionConfig<TestEnum>(
                valueMap, template, string.Empty, describeValue,
                true, inherited, true,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.IsTrue(opt.Description.Contains("multiple"));
        }

        /// <summary>
        /// The description should refelct the arity.
        /// </summary>
        [TestMethod]
        public void Description_values()
        {
            var cla = new CommandLineApplication();

            var values = new Dictionary<string, TestEnum>
            {
                { "abc",TestEnum.abc},
                { "def",TestEnum.def},
                { "ghi",TestEnum.def},
            };

            var optionConfig = new EnumOptionConfig<TestEnum>(
                values, template, string.Empty, true,
                describeArity, inherited, true,
                throwOnInvalidOption, Configuration
                );

            var opt = cla.EnumOption(optionConfig);

            Assert.IsTrue(opt.Description.Contains("abc"));
            Assert.IsTrue(opt.Description.Contains("def"));
            Assert.IsTrue(opt.Description.Contains("ghi"));
        }
    }
}
