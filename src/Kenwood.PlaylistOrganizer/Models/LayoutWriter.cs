using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Models
{
    internal class LayoutWriter
    {
        private readonly Layout _layout;
        private static readonly object ConsoleWriterLock = new object();

        internal LayoutWriter(Layout layout) 
        {
            _layout = layout;
        }

        internal void UpdatePlaylist(string msg)
        {
            lock(ConsoleWriterLock)
            {
                //// position the cursor
                //Console.CursorTop = _layout.PlayListPosition;
                //Console.CursorLeft = _layout.PlaylistPadding;
                //// erase any current message
                //var spc = "                                                                                           ";
                //Console.Write(spc);
                //// write new message
                //Console.CursorLeft = _layout.PlaylistPadding;
                //var color = Console.ForegroundColor;
                //Console.ForegroundColor = ConsoleColor.DarkGray; 
                //Console.Write($"{msg} | ");
                //Console.ForegroundColor= color;
            }
        }

        internal void UpdatePath(string msg)
        {
            lock (ConsoleWriterLock)
            {
                //// position the cursor
                //Console.CursorTop = _layout.TargetPosition;
                //Console.CursorLeft = _layout.PathPadding;
                //// erase any current message
                //var spc = "                                                                                           ";
                //Console.Write(spc);
                //// write new message
                //Console.CursorLeft = _layout.PlaylistPadding + _layout.TargetPadding;
                //var color = Console.ForegroundColor;
                //Console.ForegroundColor = ConsoleColor.DarkGray;
                //Console.Write(msg);
                //Console.ForegroundColor = color;
            }
        }

        internal void UpdateFile(string msg, int count, int total)
        {
            lock (ConsoleWriterLock)
            {
                // position the cursor
                Console.CursorTop = _layout.FilePosition;
                Console.CursorLeft = _layout.FilePadding;
                // erase any current message
                var spc = "                                                                                           ";
                Console.Write(spc);
                // write new message
                Console.CursorLeft = _layout.FilePadding;
                Console.Write($"{msg} ({count}/{total})");
            }
        }

        internal void UpdateCopy(string msg)
        {
            lock (ConsoleWriterLock)
            {
                // position the cursor
                Console.CursorTop = _layout.ProgressPosition;
                Console.CursorLeft = _layout.ProgressPadding;
                // erase any current message
                var spc = "       ";
                Console.Write(spc);
                // write new message
                Console.CursorLeft = _layout.ProgressPadding;
                Console.Write(msg);
            }
        }

        internal void UpdateValidation(string msg)
        {
            lock (ConsoleWriterLock)
            {
                // position the cursor
                Console.CursorTop = _layout.ValidationPosition;
                Console.CursorLeft = _layout.ValidationPadding;
                // erase any current message
                var spc = "       ";
                Console.Write(spc);
                // write new message
                Console.CursorLeft = _layout.ValidationPadding;
                Console.Write(msg);
            }
        }

        internal void UpdateMessage(string msg, bool isError = false)
        {
            lock (ConsoleWriterLock)
            {
                // position the cursor
                Console.CursorTop = _layout.MessagePosition;
                Console.CursorLeft = _layout.MessagePadding;
                // erase any current message
                var spc = "                                                                                               ";
                Console.Write(spc);
                // write new message
                Console.CursorLeft = _layout.MessagePadding;
                var currentColor = Console.ForegroundColor;
                Console.ForegroundColor = isError ? ConsoleColor.Red : ConsoleColor.Green;
                Console.Write(msg);
                Console.ForegroundColor = currentColor;
            }
        }

        internal void UpdateTime(string msg)
        {
            lock (ConsoleWriterLock)
            {
                // position the cursor
                Console.CursorTop = _layout.TimePosition;
                Console.CursorLeft = _layout.TimePadding;
                // write new message
                Console.Write(msg);
            }
        }
    }
}
