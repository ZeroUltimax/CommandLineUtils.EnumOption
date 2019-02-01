using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Microsoft.Extensions.CommandLineUtils.Test
{
    [TestClass]
    public class EnumOptionConfigBuilderTest
    {
        private const string template = "-t | --test";
        private const string desc = "Much test.";

        /// <summary>
        /// Template and description should be obligatory.
        /// </summary>
        [TestMethod]
        public void Required_constructor_argument()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new EnumOptionConfigBuilder<TestEnum>(null, desc)
            );

            Assert.ThrowsException<ArgumentNullException>(
                () => new EnumOptionConfigBuilder<TestEnum>(template, null)
            );
        }

        /// <summary>
        /// Shouldn't allow configuring the same property twice.
        /// </summary>
        [TestMethod]
        public void Prevents_Double_Configure()
        {
            var builder = new EnumOptionConfigBuilder<TestEnum>(template, desc);

            var actions = new Func<EnumOptionConfigBuilder<TestEnum>>[]
            {
                ()=>builder.AllowMultiple(true),
                ()=>builder.Configuration((x) => { }),
                ()=>builder.DescribeArity(true),
                ()=>builder.DescribeValues(true),
                ()=>builder.Inherited(true),
                ()=>builder.ThrowOnInvalidOption(true),
            };

            foreach (var action in actions)
            {
                action();
                Assert.ThrowsException<InvalidOperationException>(() => action());
            }
        }

        /// <summary>
        /// Prevents building a configuration with no possible values.
        /// </summary>
        [TestMethod]
        public void Requires_at_least_one_use()
        {
            var builder = new EnumOptionConfigBuilder<TestEnum>(template, desc);

            Assert.ThrowsException<InvalidOperationException>(() => builder.Build());
        }

        /// <summary>
        /// Use configures with toString by default.
        /// </summary>
        [TestMethod]
        public void Use_default()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .Use(TestEnum.abc)
                .Build()
                ;

            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["abc"]);
        }

        /// <summary>
        /// Use configures with a specified string to match.
        /// </summary>
        [TestMethod]
        public void Use_specified()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .Use("xyz", TestEnum.abc)
                .Use("opq", TestEnum.abc)
                .Build()
                ;

            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["opq"]);
            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["xyz"]);
        }

        /// <summary>
        /// Use should throw if a key is configured twice.
        /// </summary>
        [TestMethod]
        public void Use_exception_on_duplicate_key()
        {
            var builder = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .Use("def", TestEnum.abc)
                .Use("qwerty", TestEnum.abc)
                ;

            Assert.ThrowsException<InvalidOperationException>(
                () => builder.Use(TestEnum.def)
            );
            Assert.ThrowsException<InvalidOperationException>(
                () => builder.Use("qwerty", TestEnum.def)
            );
        }

        /// <summary>
        /// Use configures with a specified string to match.
        /// </summary>
        [TestMethod]
        public void Use_all_uses_all_enum_values()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .UseAll()
                .Build()
                ;

            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["abc"]);
            Assert.AreEqual(TestEnum.def, cfg.OptionValueMap["def"]);
        }

        /// <summary>
        /// Ignore case by default.
        /// </summary>
        [TestMethod]
        public void Default_ignore_case()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .Use(TestEnum.abc)
                .Build()
                ;

            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["abc"]);
            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["ABC"]);
            Assert.AreEqual(TestEnum.abc, cfg.OptionValueMap["AbC"]);
        }

        /// <summary>
        /// Uses the comparison provided if there's one
        /// </summary>
        [TestMethod]
        public void Uses_comparer()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc, StringComparer.Ordinal)
                .Use(TestEnum.abc)
                .Build()
                ;

            Assert.IsTrue(cfg.OptionValueMap.ContainsKey("abc"));
            Assert.IsFalse(cfg.OptionValueMap.ContainsKey("ABC"));
        }

        /// <summary>
        /// The expected default values for the configuration
        /// </summary>
        [TestMethod]
        public void Sets_default_values()
        {
            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .Use(TestEnum.abc)
                .Build();

            Assert.AreEqual(template, cfg.Template);
            Assert.AreEqual(desc, cfg.Description);

            Assert.AreEqual(true, cfg.AllowMultiple);
            Assert.AreEqual(true, cfg.DescribeArity);
            Assert.AreEqual(true, cfg.DescribeValues);
            Assert.AreEqual(false, cfg.Inherited);
            Assert.AreEqual(false, cfg.ThrowOnInvalidOption);

            // No exact way to test it's a NoOp, but at least this tries.
            cfg.Configuration(null);
        }

        /// <summary>
        /// Test the configuration actually works
        /// </summary>
        [TestMethod]
        public void Actually_Configures()
        {
            bool marker = false;
            void op(CommandOption o) { marker = true; }

            var cfg = new EnumOptionConfigBuilder<TestEnum>(template, desc)
                .AllowMultiple(false)
                .Configuration(op)
                .DescribeArity(false)
                .DescribeValues(false)
                .Inherited(true)
                .ThrowOnInvalidOption(true)
                .Use(TestEnum.abc)
                .Build();

            Assert.AreEqual(false, cfg.AllowMultiple);
            Assert.AreEqual(false, cfg.DescribeArity);
            Assert.AreEqual(false, cfg.DescribeValues);
            Assert.AreEqual(true, cfg.Inherited);
            Assert.AreEqual(true, cfg.ThrowOnInvalidOption);

            cfg.Configuration(null);
            Assert.IsTrue(marker);
        }
    }
}
