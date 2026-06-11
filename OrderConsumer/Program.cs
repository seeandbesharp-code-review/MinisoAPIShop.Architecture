using Confluent.Kafka;

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "order-log-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

using var consumer = new ConsumerBuilder<string, string>(config).Build();
consumer.Subscribe("order-events");

Console.WriteLine("OrderConsumer started. Listening for messages on 'order-events'...");

try
{
    while (!cts.Token.IsCancellationRequested)
    {
        var result = consumer.Consume(cts.Token);

        Console.WriteLine($"[Key]       {result.Message.Key}");
        Console.WriteLine($"[Value]     {result.Message.Value}");
        Console.WriteLine($"[Partition] {result.Partition.Value}  [Offset] {result.Offset.Value}");
        Console.WriteLine(new string('-', 60));
    }
}
catch (OperationCanceledException)
{
    Console.WriteLine("Consumer stopped.");
}
finally
{
    consumer.Close();
}
