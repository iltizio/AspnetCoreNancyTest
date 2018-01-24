using System;
using FluentAssertions;
using NancyTest.Server.Services;
using NUnit.Framework;

namespace NancyTest.Server.Test.Services
{
    [TestFixture]
    public class HelloServicesTest
    {
        [Test]
        public void FormalHelloService_Hello_Ok()
        {
            //Arrange
            var service = new FormalHelloService();
            var name = "John";

            //Act
            string result = null;
            Action act = () => result = service.Hello(name);

            //Assert
            act.ShouldNotThrow();
            result.Should().Be(string.Join(", ", "Good Morning", name));
        }

        [Test]
        public void InformalHelloService_Hello_Ok()
        {
            //Arrange
            var service = new InformalHelloService();
            var name = "John";

            //Act
            string result = null;
            Action act = () => result = service.Hello(name);

            //Assert
            act.ShouldNotThrow();
            result.Should().Be(string.Join("! ", "Hello", name));
        }
    }
}
