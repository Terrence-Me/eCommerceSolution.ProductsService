namespace BusinessLogicLayer.RabbitMQ;
public record ProductDeletionMessage(Guid ProductId, string? ProductName);
