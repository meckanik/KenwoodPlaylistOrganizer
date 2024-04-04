using Kenwood.PlaylistOrganizer.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Models
{
    internal class Layout
    {
        private const int _playlistPadding = 18;
        private const int _targetPadding = 15;
        private const int _filePadding = 14;
        private const int _copyProgressPadding = 23;
        private const int _validationProgressPadding = 29;
        private const int _messagePadding = 17;
        private const int _timePadding = 22;

        private readonly int _playlistPos;
        private readonly int _targetPos;
        private readonly int _filePos; 
        private readonly int _copyProgressPos;
        private readonly int _validationProgressPos;
        private readonly int _messagePos;
        private readonly int _timePos;


        internal static Layout PrintLayout(string playlist, string target)
        {
            var clear = "\r                                                                                                        ";
            var origForeColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(clear);
            Console.CursorLeft = 0;
            Console.CursorLeft = 25;
            Console.Write($"{Messages.PlaylistLabel}{playlist} | {Messages.TargetPathLabel}{target}");

            Console.ForegroundColor = origForeColor;

            var playPos = Console.CursorTop;
            var targetPos = Console.CursorTop;
            Console.CursorTop = targetPos + 2;

            Console.Write(clear);
            Console.Write($"{Messages.LineStart}{Messages.FileLabel}");

            var filePos = Console.CursorTop;
            Console.CursorTop = filePos + 2;

            Console.Write(clear);
            Console.Write($"{Messages.LineStart}{Messages.CopyLabel}");

            var copyPos = Console.CursorTop;
            Console.CursorTop = copyPos + 2;

            Console.Write(clear);
            Console.Write($"{Messages.LineStart}{Messages.ValidationLabel}");

            var validationPos = Console.CursorTop;
            Console.CursorTop = validationPos + 2;

            Console.Write(clear);
            Console.Write($"{Messages.LineStart}{Messages.MessageLabel}");

            var messagePos = Console.CursorTop;
            Console.CursorTop = messagePos + 2;

            Console.Write(clear);
            Console.Write($"{Messages.LineStart}{Messages.TimeLabel}");

            var timePos = Console.CursorTop;

            return new Layout(playPos, targetPos,
                filePos, copyPos, validationPos, messagePos, timePos);
        }

        private Layout(
            int playlistPos,
            int targetPos,
            int filePos,
            int progressPos,
            int validationPos,
            int messagePos,
            int timePos) 
        {
            _playlistPos = playlistPos;
            _targetPos = targetPos;
            _filePos = filePos;
            _copyProgressPos = progressPos;
            _validationProgressPos = validationPos;
            _messagePos = messagePos;
            _timePos = timePos;
        }

        internal int FilePosition => _filePos;

        internal int FilePadding => _filePadding;

        internal int PlayListPosition => _playlistPos;

        internal int PlaylistPadding => _playlistPadding;

        internal int TargetPosition => _targetPos;
        
        internal int TargetPadding => _targetPadding;

        internal int ProgressPosition => _copyProgressPos;

        internal int ProgressPadding => _copyProgressPadding;

        internal int ValidationPosition => _validationProgressPos;

        internal int ValidationPadding => _validationProgressPadding;

        internal int MessagePosition => _messagePos;

        internal int MessagePadding => _messagePadding;

        internal int TimePosition => _timePos;

        internal int TimePadding => _timePadding;
    }
}
