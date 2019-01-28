using Microsoft.Extensions.CommandLineUtils;

public static class CommandLineApplicationExtensions
{

    public static EnumOption<T> EnumOption<T>(
        this CommandLineApplication cla,
        EnumOptionConfig<T> optionConfig
    ) where T : struct, System.Enum // requires c# 7.3
    {
        var optionType = optionConfig.AllowMultiple ?
            CommandOptionType.MultipleValue :
            CommandOptionType.SingleValue;

        var opt = cla.Option(
            optionConfig.Template,
            optionConfig.Description,
            optionType,
            optionConfig.Configuration,
            optionConfig.Inherited
        );

        return new EnumOption<T>(
            opt,
            optionConfig.OptionValueMap,
            optionConfig.ThrowOnInvalidOption
        );
    }
}
