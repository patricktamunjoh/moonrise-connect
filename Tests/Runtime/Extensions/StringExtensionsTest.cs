using MoonriseGames.CloudsAhoyConnect.Extensions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Extensions {
    public class StringExtensionsTest {

        [Test]
        public void ShouldRemoveLineBreaksAndIndents() {
            const string sut = @"This string contains
                line breaks and gaps and is stretched over
                multiple lines";

            Assert.AreEqual("This string contains\r\nline breaks and gaps and is stretched over\r\nmultiple lines", sut.TrimIndents());
        }
    }
}
