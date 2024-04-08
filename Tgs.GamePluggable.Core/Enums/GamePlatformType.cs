using System;

namespace Tgs.GamePluggable.Core.Enums;

[Flags]
public enum GamePlatformType : byte
{
	Desktop = 1,
	Mobile = 2,
	DesktopAndMobile = 3
}
