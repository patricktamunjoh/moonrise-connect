using System;
using System.Linq;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions
{
    public class TypeExtensionsTest
    {
        [Test]
        public void ShouldProvideCompleteTypeHierarchy()
        {
            var expected = new[] { typeof(Sample), typeof(SampleBase), typeof(object) };

            Assert.True(expected.SequenceEqual(typeof(Sample).InheritedTypes()));
        }

        [Test]
        public void ShouldProvideEmptyHierarchyForNullType()
        {
            var types = ((Type)null).InheritedTypes();
            Assert.IsEmpty(types);
        }
    }
}
