using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace NancyTest.Server.Services
{
    public interface IHelloService
    {
        string Hello(string name);
    }
}
