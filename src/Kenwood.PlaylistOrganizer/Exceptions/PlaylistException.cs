using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Exceptions
{
    internal class PlaylistException : Exception
    {
        internal PlaylistException(string message) : base(message) { }
    }
}
