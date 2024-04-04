using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Helpers
{
    internal class ValidationHelpers
    {
        internal static bool CompareFiles(string source, string target, bool maxMem = false)
        {
            var byteLength = 4024;
            long length = 0;
            var sourceBuffer = new Span<byte>();
            var targetBuffer = new Span<byte>();

            using (var sourceStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var targetStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                length = sourceStream.Length;
                if (length == targetStream.Length)
                {

                    sourceBuffer = new Span<byte>(new byte[length]);
                    targetBuffer = new Span<byte>(new byte[length]);
                    sourceStream.ReadExactly(sourceBuffer);
                    targetStream.ReadExactly(targetBuffer);
                }
            }

            return ByteHelpers.ByteArrayCompare(sourceBuffer.ToArray(), targetBuffer.ToArray()); ;
        }
    }
}
