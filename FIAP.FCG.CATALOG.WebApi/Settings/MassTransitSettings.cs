using FCG.Core.Messages.Integration;
using FCG.Core.Objects;
using FIAP.FCG.CATALOG.Application.Consumers;
using FIAP.FCG.CATALOG.WebApi.Settings.Dtos;
using MassTransit;
using Microsoft.Extensions.Options;

namespace FIAP.FCG.CATALOG.WebApi.Settings
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

            builder.Services.AddMassTransitHostedService();

            return builder;
        }
    }
}
