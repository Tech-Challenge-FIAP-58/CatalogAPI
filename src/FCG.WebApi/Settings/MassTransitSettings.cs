using FCG.Application.Consumers;
using FCG.Core.Objects;
using FCG.WebApi.Settings.Dtos;
using MassTransit;
using Microsoft.Extensions.Options;

namespace FCG.WebApi.Settings
{
    public static class MassTransitSettings
    {
        public static WebApplicationBuilder AddMassTransitSettings(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<PaymentProcessedEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitSettings = context
                        .GetRequiredService<IOptions<RabbitMqSettings>>()
                        .Value;

                    cfg.Host(rabbitSettings.Host, rabbitSettings.VirtualHost, h =>
                    {
                        h.Username(rabbitSettings.Username);
                        h.Password(rabbitSettings.Password);
                    });

                    cfg.UseMessageRetry(r =>
                    {
                        r.Interval(
                            RetrySettings.MaxRetryAttempts,
                            TimeSpan.FromSeconds(RetrySettings.DelayBetweenRetriesInSeconds)
                        );
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            return builder;
        }
    }
}
