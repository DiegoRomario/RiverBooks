using RiverBooks.OrderProcessing.Domain;

namespace RiverBooks.OrderProcessing.Infrastructure;

// This is the materialized view's data model
internal record OrderAddress(Guid Id, Address Address);
