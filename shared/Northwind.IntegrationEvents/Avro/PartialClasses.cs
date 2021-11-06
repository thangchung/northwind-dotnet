using System;
using MediatR;

namespace Northwind.IntegrationEvents
{
    public partial class CustomerCreated : INotification
    {
    }

    public partial class CustomerUpdated : INotification
    {
    }

    public partial class CustomerDeleted : INotification
    {
    }

    public partial class EmployeeCreated : INotification
    {
    }

    public partial class EmployeeUpdated : INotification
    {
    }

    public partial class EmployeeDeleted : INotification
    {
    }

    public partial class ProductCreated : INotification
    {
    }

    public partial class ProductUpdated : INotification
    {
    }

    public partial class ProductDeleted : INotification
    {
    }

    public partial class ShipperCreated : INotification
    {
    }

    public partial class ShipperUpdated : INotification
    {
    }

    public partial class ShipperDeleted : INotification
    {
    }

    public partial class SupplierCreated : INotification
    {
    }

    public partial class SupplierUpdated : INotification
    {
    }

    public partial class SupplierDeleted : INotification
    {
    }

    public class DefaultValues
    {
        public static Guid AdminId { get; } = new Guid("870d16a3-bbf9-4f73-93b1-5146ce33039c");
        public static Guid CustomerId { get; } = new Guid("1584eae0-dd9e-40d2-ba01-e9a8a05db68f");
        public static Guid ShipperId { get; } = new Guid("f0ad8acf-adcd-4a08-9810-eeb24bbd7807");
        public static Guid SupplierId { get; } = new Guid("d346eaa4-9b1c-4ecd-bc1e-1093e0b9106e");
        public static Guid EmployeeId { get; } = new Guid("4b01c764-8204-4a3e-af72-d19a40e3db29");
    }
}
