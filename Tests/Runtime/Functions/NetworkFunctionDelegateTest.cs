using System;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Functions;
using Moq;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Functions
{
    public class NetworkFunctionDelegateTest
    {
        private NetworkFunctionData FunctionDataWithInfo(MethodInfo methodInfo, bool isDeferred = false) =>
            new(new NetworkFunction(Groups.Host, Recipients.Clients, isDeferred), methodInfo);

        [Test]
        public void ShouldRetainProperties()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction));
            var function = new NetworkFunction(Groups.Clients, Recipients.Clients);
            var data = new NetworkFunctionData(function, methodInfo);

            var sut = new NetworkFunctionDelegate(target.Object, data);

            Assert.AreEqual(data, sut.Data);
        }

        [Test]
        public void ShouldThrowIfEmptyArgumentsArrayIsProvided()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            Assert.Throws<ArgumentException>(() => sut.Invoke(Array.Empty<object>()));
        }

        [Test]
        public void ShouldCorrectlyHandleInvocationWithDeferral()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.MonadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo, true));

            Assert.DoesNotThrow(() => sut.Invoke(new object[] { "example" }));
        }

        [Test]
        public void ShouldProvideStringWithArguments()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.MonadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));
            var message = sut.ToString(new[] { "example" });

            Assert.True(message.Contains(target.ToString()));
            Assert.True(message.Contains("example"));
            Assert.True(message.Contains(nameof(ISample.MonadicFunction)));
        }

        [Test]
        public void ShouldProvideStringWithNullArguments()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.MonadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));
            var message = sut.ToString(null);

            Assert.True(message.Contains(target.ToString()));
            Assert.True(message.Contains(nameof(ISample.MonadicFunction)));
        }

        [Test]
        public void ShouldProvideEmptyStringWithNoTarget()
        {
            var sut = Function.ExecuteInCollectableScope(() =>
            {
                var target = new Sample();
                var methodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction));

                return new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));
            });

            GC.Collect();

            var message = sut.ToString(null);

            Assert.AreEqual(string.Empty, message);
        }

        [Test]
        public void ShouldThrowIfWrongArrayIsProvided()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            Assert.Throws<ArgumentException>(() => sut.Invoke(new object[] { "example" }));
        }

        [Test]
        public void ShouldDelegateAfterGarbageCollection()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            GC.Collect();
            sut.Invoke(null);

            target.Verify(x => x.NiladicFunction());
        }

        [Test]
        public void ShouldDelegateToNiladicFunction()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.NiladicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            target.Verify(x => x.NiladicFunction());
        }

        [Test]
        public void ShouldDelegateToMonadicFunction()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.MonadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            sut.Invoke(new object[] { "example" });

            target.Verify(x => x.MonadicFunction("example"));
        }

        [Test]
        public void ShouldDelegateToDyadicFunction()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.DyadicFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            sut.Invoke(new object[] { 12, true });

            target.Verify(x => x.DyadicFunction(12, true));
        }

        [Test]
        public void ShouldDelegateToReturningFunction()
        {
            var target = new Mock<ISample>();
            var methodInfo = typeof(ISample).GetDeclaredMethod(nameof(ISample.ReturningFunction));

            var sut = new NetworkFunctionDelegate(target.Object, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            target.Verify(x => x.ReturningFunction());
        }

        [Test]
        public void ShouldDelegateInterfaceToOverwrittenFunction()
        {
            var target = new SampleBase();
            var methodInfo = typeof(SampleBase).GetMethod(nameof(ISample.NiladicFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            Assert.Positive(target.InvocationCounter.InvocationCount(methodInfo));
        }

        [Test]
        public void ShouldDelegateToOverwrittenFunction()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(nameof(Sample.VirtualFunction));
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.VirtualFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            Assert.Positive(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Zero(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldDelegateToOverwrittenFunctionFromBaseType()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(nameof(Sample.VirtualFunction));
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.VirtualFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(baseMethodInfo));

            sut.Invoke(null);

            Assert.Positive(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Zero(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldDelegateToPrivateFunction()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(Sample.PrivateFunctionName);
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(SampleBase.PrivateFunctionName);

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            Assert.Positive(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Zero(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldDelegateToPrivateFunctionOnBaseType()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(Sample.PrivateFunctionName);
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(SampleBase.PrivateFunctionName);

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(baseMethodInfo));

            sut.Invoke(null);

            Assert.Zero(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Positive(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldDelegateToHiddenFunction()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(nameof(Sample.PublicFunction));
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));

            sut.Invoke(null);

            Assert.Positive(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Zero(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldDelegateToHiddenFunctionOnBaseType()
        {
            var target = new Sample();
            var methodInfo = typeof(Sample).GetDeclaredMethod(nameof(Sample.PublicFunction));
            var baseMethodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(baseMethodInfo));

            sut.Invoke(null);

            Assert.Zero(target.InvocationCounter.InvocationCount(methodInfo));
            Assert.Positive(((SampleBase)target).InvocationCounter.InvocationCount(baseMethodInfo));
        }

        [Test]
        public void ShouldHaveReferenceToFunction()
        {
            var target = new Sample();
            var methodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction));

            var sut = new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));

            Assert.True(sut.HasTarget);
        }

        [Test]
        public void ShouldNotHoldStrongReferenceToFunction()
        {
            var sut = Function.ExecuteInCollectableScope(() =>
            {
                var target = new Sample();
                var methodInfo = typeof(SampleBase).GetDeclaredMethod(nameof(SampleBase.PublicFunction));

                return new NetworkFunctionDelegate(target, FunctionDataWithInfo(methodInfo));
            });

            GC.Collect();
            sut.Invoke(null);

            Assert.False(sut.HasTarget);
        }
    }
}
