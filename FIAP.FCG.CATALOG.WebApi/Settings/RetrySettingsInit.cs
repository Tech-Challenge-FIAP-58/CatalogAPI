using FCG.Core.Objects;

namespace FIAP.FCG.CATALOG.WebApi.Settings;

public static class RetrySettingsInit
{
    public static void InitilizeRetrySettings(this WebApplicationBuilder builder)
    {
        RetrySettings.MaxRetryAttempts = builder.Configuration.GetValue<int>("RetrySettings:MaxRetryAttempts");
        RetrySettings.DelayBetweenRetriesInSeconds = builder.Configuration.GetValue<int>("RetrySettings:DelayBetweenRetriesInSeconds");
    }
}
