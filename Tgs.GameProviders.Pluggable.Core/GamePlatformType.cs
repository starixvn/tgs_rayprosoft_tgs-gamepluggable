using System;

namespace Tgs.GameProviders.Pluggable.Core
{
    [Flags]
    public enum GamePlatformType : byte
    {
        Desktop = 1,
        Mobile = 2,
        DesktopAndMobile = Desktop | Mobile
    }
}
