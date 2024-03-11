using System;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Payloads;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Functions
{
    public class NetworkFunctionDataTest
    {
        [Test]
        public void ShouldRetainPropertyValues()
        {
            var methodInfo = GetType().GetMethod(nameof(ShouldRetainPropertyValues));
            var function = new NetworkFunction(Groups.Clients, Recipients.Clients);

            var sut = new NetworkFunctionData(function, methodInfo);

            Assert.AreEqual(methodInfo, sut.MethodInfo);
            Assert.AreEqual(function, sut.Function);
        }

        [Test]
        public void ShouldDeriveMethodParameterTypes()
        {
            var types = typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction)).GetParameters().Select(p => p.ParameterType).ToArray();
            var sut = new NetworkFunctionData(null, typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction)));

            Assert.True(sut.ParameterTypes.SequenceEqual(types));
        }

        [Test]
        public void ShouldDeriveMethodParameterAndReturnTypes()
        {
            var types = typeof(ISample)
                .GetDeclaredMethod(nameof(ISample.DyadicFunction))
                .GetParameters()
                .Select(p => p.ParameterType)
                .Append(typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction)).ReturnType)
                .ToArray();

            var sut = new NetworkFunctionData(null, typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction)));

            Assert.True(sut.ParameterAndReturnTypes.SequenceEqual(types));
        }

        [Test]
        public void ShouldDerivePayloadType()
        {
            var sut = new NetworkFunctionData(null, typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction)));

            Assert.AreEqual((Type)new NetworkPayloadType(sut.ParameterTypes), (Type)sut.PayloadType);
        }
    }
}
