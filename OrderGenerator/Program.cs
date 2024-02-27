using OrderGenerator;
using QuickFix;
using QuickFix.Transport;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
                args = new[] { "../../../OrderGenerator.cfg" };

            SessionSettings settings = new(args[0]);
            var orderGenerator = new FixOrderGenerator();

            SocketInitiator initiator = new(
                orderGenerator,
                new FileStoreFactory(settings),
                settings,
                new FileLogFactory(settings));

            initiator.Start();

            System.Timers.Timer timer = new()
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(orderGenerator.Run);
        }
        catch(Exception ex)
        {
            Console.WriteLine("Houve um erro ao iniciar o gerador. - " + ex.Message);
        }
    }
}