using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.CommandLineUtils.Test
{
    [TestClass]
    public class EnumOptionTest
    {
        private static readonly Dictionary<string, TestEnum> valueMap =
            new Dictionary<string, TestEnum>()
            {
                        { "abc",TestEnum.abc },
                        { "def",TestEnum.def }
            };
        private const string template = "-t | --test";
        private const CommandOptionType optionType = CommandOptionType.MultipleValue;
        private const bool throwOnInvalidOption = false;

        /// <summary>
        /// Checks that the parsing throws if we asked it to
        /// </summary>
        [TestMethod]
        public void Throws_if_invalid_option()
        {
            CommandOption opt = new CommandOption(template, optionType);

            EnumOption<TestEnum> enumOpt =
                new EnumOption<TestEnum>(opt, valueMap, true);

            opt.TryParse("whut");

            Assert.ThrowsException<ArgumentException>(() => enumOpt.Value);
        }

        /// <summary>
        /// Checks that the parsing ignores invalid values if we asked it to.
        /// </summary>
        [TestMethod]
        public void Ignores_invalid_option()
        {
            CommandOption opt = new CommandOption(template, optionType);

            EnumOption<TestEnum> enumOpt =
                new EnumOption<TestEnum>(opt, valueMap, false);

            opt.TryParse("whut");

            Assert.IsTrue(!enumOpt.Values.Any());
        }

        /// <summary>
        /// Checks that the works for specified values
        /// </summary>
        [TestMethod]
        public void Parses_values()
        {
            CommandOption opt = new CommandOption(template, optionType);

            EnumOption<TestEnum> enumOpt =
                new EnumOption<TestEnum>(opt, valueMap, throwOnInvalidOption);

            opt.TryParse("abc");
            opt.TryParse("def");

            Assert.IsTrue(enumOpt.Values.Contains(TestEnum.abc));
            Assert.IsTrue(enumOpt.Values.Contains(TestEnum.def));
        }

        /// <summary>
        /// Checks that the works for specified value
        /// </summary>
        [TestMethod]
        public void Parses_single_value()
        {
            CommandOption opt = new CommandOption(template, optionType);

            EnumOption<TestEnum> enumOpt =
                new EnumOption<TestEnum>(opt, valueMap, throwOnInvalidOption);

            opt.TryParse("abc");

            Assert.AreEqual(TestEnum.abc, enumOpt.Value);
        }

        /// <summary>
        /// Checks that getter/setter passe through to the option
        /// </summary>
        [TestMethod]
        public void get_set_passtrough()
        {
            CommandOption opt = new CommandOption(template, optionType);

            EnumOption<TestEnum> enumOpt =
                new EnumOption<TestEnum>(opt, valueMap, throwOnInvalidOption);


            const string templ = "-h | --hello | -!";
            enumOpt.Template = templ;
            Assert.AreEqual(templ, enumOpt.Template);
            Assert.AreEqual(templ, opt.Template);

            const string shortName = "h";
            enumOpt.ShortName = shortName;
            Assert.AreEqual(shortName, enumOpt.ShortName);
            Assert.AreEqual(shortName, opt.ShortName);

            const string longName = "hello";
            enumOpt.LongName = longName;
            Assert.AreEqual(longName, enumOpt.LongName);
            Assert.AreEqual(longName, opt.LongName);

            const string symbolName = "^";
            enumOpt.SymbolName = symbolName;
            Assert.AreEqual(symbolName, enumOpt.SymbolName);
            Assert.AreEqual(symbolName, opt.SymbolName);

            const string valueName = "val";
            enumOpt.ValueName = valueName;
            Assert.AreEqual(valueName, enumOpt.ValueName);
            Assert.AreEqual(valueName, opt.ValueName);

            const string description = "Hi.";
            enumOpt.Description = description;
            Assert.AreEqual(description, enumOpt.Description);
            Assert.AreEqual(description, opt.Description);

            const bool showInHelpText = false;
            enumOpt.ShowInHelpText = showInHelpText;
            Assert.AreEqual(showInHelpText, enumOpt.ShowInHelpText);
            Assert.AreEqual(showInHelpText, opt.ShowInHelpText);

            const bool inherited = true;
            enumOpt.Inherited = inherited;
            Assert.AreEqual(inherited, enumOpt.Inherited);
            Assert.AreEqual(inherited, opt.Inherited);
        }
    }
}
