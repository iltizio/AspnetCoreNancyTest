namespace NancyTest.Server.Services
{
    public class InformalHelloService : IHelloService
    {
        public string Hello(string name)
        {
            return string.Join("! ", "Hello", name);
        }
    }
}