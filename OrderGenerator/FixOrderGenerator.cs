using QuickFix;
using QuickFix.Fields;
using System.Timers;

namespace OrderGenerator
{
    public class FixOrderGenerator : MessageCracker, IApplication
    {
        private SessionID? SessionID { get; set; }

        public void FromAdmin(Message message, SessionID sessionID) { }

        public void OnLogon(SessionID sessionID) { }

        public void ToAdmin(Message message, SessionID sessionID) { }

        public void ToApp(Message message, SessionID sessionID) { }

        public void OnLogout(SessionID sessionID) 
        {
            Console.WriteLine("Acceptor interrompido. Inicie-o novamente para retomar o envio das ordens");
        }

        public void FromApp(Message message, SessionID sessionID)
        {
            Crack(message, sessionID);
        }

        public void OnCreate(SessionID sessionID)
        {
            SessionID = sessionID;
        }

        public void OnMessage(QuickFix.FIX50.ExecutionReport orderResponse, SessionID sessionID)
        {
            if (orderResponse.ExecType.Obj == ExecType.REJECTED)
                Console.WriteLine("Retorno de ordem rejeitada.");
            else
                Console.WriteLine("Retorno de ordem aceita.");
        }

        public void Run(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (Session.LookupSession(SessionID).IsLoggedOn)
                {
                    var order = CreateOrder();
                    Session.SendToTarget(order, SessionID);
                }
            }
            catch
            {
                Console.WriteLine("Houve um erro ao enviar a ordem.");
            }
        }

        private QuickFix.FIX50.NewOrderSingle CreateOrder()
        {
            string[] symbols = new[] { "PETR4", "VALE3", "VIIA4" };
            Random rand = new Random();

            var x = rand.Next(1, 100000);
            decimal price = Convert.ToDecimal(x * 0.01);

            var order = new QuickFix.FIX50.NewOrderSingle();
            order.Side = new Side(Convert.ToChar(rand.Next(1, 3).ToString()));
            order.Price = new Price(Math.Round(price, 2));
            order.Symbol = new Symbol(symbols[rand.Next(0, 3)]);
            order.OrderQty = new OrderQty(rand.Next(1, 100000));

            return order;
        }
    }
}
