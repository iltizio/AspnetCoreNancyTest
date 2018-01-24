namespace NancyTest.Server.Services
{
    public class FormalHelloService : IHelloService
    {
        public string Hello(string name)
        {
            return string.Join(", ", "Good Morning", name);
        }
    }
}