using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GamePluggable.Core.Enums;
[Flags]
public enum RoundStatus
{
	New = 0,
	Open = 1,
	Closed = 2
}
