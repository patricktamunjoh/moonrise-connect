using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Assertions
{
    internal static class SampleAssertions
    {
        public static void VerifyInvocations(object sut, InvocationCounter counter, string name, params object[] args)
        {
            var method = sut.GetType().GetDeclaredMethod(name, args?.Select(x => x.GetType()).ToArray());

            Assert.Zero(counter.InvocationCount(name));

            method.Invoke(sut, args);
            Assert.AreEqual(1, counter.InvocationCount(name));

            if (args == null)
                Assert.Null(counter.Arguments(name, 0));
            else
                Assert.True(args.SequenceEqual(counter.Arguments(name, 0)));

            method.Invoke(sut, args);
            Assert.AreEqual(2, counter.InvocationCount(name));

            if (args == null)
                Assert.Null(counter.Arguments(name, 1));
            else
                Assert.True(args.SequenceEqual(counter.Arguments(name, 1)));
        }
    }
}
