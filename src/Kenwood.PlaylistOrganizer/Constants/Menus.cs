using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Constants
{
    internal static class Menus
    {
        private static string tab = Messages.LineStart;
        private static string nLine = "\r\n";

        //internal static string AskValidation = /*$"{tab}Option:{nLine}"*/ 
        //     $"{tab}1) Validate All{nLine}" 
        //    + $"{tab}2) Validate on Copy{nLine}" 
        //    + $"{tab}3) Validate on Skip{nLine}" 
        //    + $"{tab}4) Don't Validate Files{nLine}" 
        //    + $"{nLine}{tab}Enter (1-4): ";

        internal static string AskSortPlaylistMenu = /*$"{tab}Option:{nLine}"*/
             $"{tab}1) Sort Playlist Alphabetically{nLine}" 
            + $"{tab}2) Don't Sort{nLine}"
            + $"{nLine}{tab}Enter (1-2): ";

        internal static string ValidationExplaination = $"{tab}This application can do a byte-level comparison of the source file{nLine}"
            + $"{tab}to the copied (or existing) file. "
            + $"This ensures your music was copied perfectly.{nLine}{nLine}";

        internal static string AskValidationMenu = /*$"{tab}Option:{nLine}"*/
             $"{tab}1)Validate all operation{nLine}"
            + $"{tab}2)Validate on copy only{nLine}"
            + $"{tab}3)Validate on skip only{nLine}"
            + $"{tab}4)Do not validate{nLine}"
            + $"{nLine}{tab}Enter (1-4): ";

    }
}
