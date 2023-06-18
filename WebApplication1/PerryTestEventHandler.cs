using MyRabbitMQLib;

namespace WebApplication1
{
    public class PerryTestEventHandler : MyEventHandler<PerryTest>
    {
        public override Task OnReceivedAsync(PerryTest data, string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        public override void OnConsumerException(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
