using System;
using MoonriseGames.CloudsAhoyConnect.Functions;

namespace MoonriseGames.CloudsAhoyConnect.Objects {

    /// <summary>
    /// Attribute for flagging classes to partake in networking. Object instances of classes with this attribute can be registered to the
    /// network. There are two reasons to do so. First, this attribute is required for <see cref="NetworkFunction" /> to be discovered. Without
    /// this attribute function calls will not be synced between game instances. Second, registered objects can be passed as arguments to
    /// <see cref="NetworkFunction" />. This can be used as an alternative for serializing objects, especially for larger objects or objects that
    /// themselves hold references.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NetworkObject : Attribute { }
}
