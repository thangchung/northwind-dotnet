using Northwind.IntegrationEvents.Contracts;
using Shipping.Data;

namespace Shipping;

public static class Extensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connStringKey,
        IConfiguration config)
    {
        services.AddPostgresDbContext<MainDbContext>(
                config.GetConnectionString(connStringKey),
                options => options.UseModel(Shipping.MainDbContextModel.Instance),
                svc => svc.AddRepository(typeof(Repository<>)))
            .AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddCdCConsumers(this IServiceCollection services)
    {
        services.AddKafkaConsumer(o =>
        {
            o.Topic = "shipper_cdc_events";
            o.GroupId = "shipper_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(ShipperCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperCreated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ShipperUpdated).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperUpdated>(schemaRegistryClient);
                }
                else if (eventFullName == typeof(ShipperDeleted).FullName)
                {
                    result = await bytes.DeserializeAsync<ShipperDeleted>(schemaRegistryClient);
                }

                return result;
            };
        });

        services.AddKafkaConsumer(o =>
        {
            o.Topic = "order_cdc_events";
            o.GroupId = "order_cdc_events_group";
            o.EventResolver = async (eventFullName, bytes, schemaRegistryClient) =>
            {
                ISpecificRecord? result = null;
                if (eventFullName == typeof(OrderCreated).FullName)
                {
                    result = await bytes.DeserializeAsync<OrderCreated>(schemaRegistryClient);
                }

                return result;
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration config)
    {
        services.AddMassTransit(mt =>
        {
            mt.SetKebabCaseEndpointNameFormatter();

            mt.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            mt.AddRider(rider =>
            {
                rider.AddProducer<ShipmentDispatched>(nameof(ShipmentDispatched));
                rider.AddProducer<ShipmentDispatchedFailed>(nameof(ShipmentDispatchedFailed));
                rider.AddProducer<ShipmentDelivered>(nameof(ShipmentDelivered));
                rider.AddProducer<ShipmentDeliveredFailed>(nameof(ShipmentDeliveredFailed));
                rider.AddProducer<ShipmentCancelled>(nameof(ShipmentCancelled));

                // todo: review to move it to SalePayment service
                rider.AddProducer<OrderCompleted>(nameof(OrderCompleted));

                rider.UsingKafka((context, k) =>
                {
                    k.Host(config.GetValue("Kafka:BootstrapServers", "localhost:9092"));
                });
            });
        });

        services.AddMassTransitHostedService(true);

        return services;
    }
}
