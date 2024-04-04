using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Constants
{
    internal class Messages
    {
        #region switches
        internal const string Playlist = "playlist";

        internal const string TargetPath = "target";

        internal const string ValidateOnly = "validateonly";

        internal const string ValidateAll = "validateall";

        internal const string ValidateOnCopy = "validateoncopy";

        internal const string ValidateOnSkip = "validateonskip";

        internal const string NoValidation = "novalidation";

        internal const string NoSortPlaylist = "nosortplaylist";

        internal const string SortPlaylist = "sortplaylist";
        #endregion

        #region regex
        internal const string M3uFileRegex = @".+\\(.+\.[a-zA-Z0-9]+)";

        internal const string M3uPathRegex = @"(.+\\.+\.[a-zA-Z0-9]+)";
        #endregion

        #region labels
        internal const string Title = "\r\n\r\n\t\t\t\tKenwood Playlist Organizer\r\n";

        internal const string FileLabel = "File: -";

        internal const string CopyLabel = "Copy Progress: -";

        internal const string ValidationLabel = "Validation Progress: -";

        internal const string MessageLabel = "Message: -";

        internal const string PlaylistLabel = "Playlist: ";

        internal const string TargetPathLabel = "Target: ";

        internal const string TimeLabel = "Elapsed Time: -";
        #endregion

        #region messages
        internal const string ValidationSuccess = "Varified byte perfect.";

        internal const string ValidationFail = "Validation failed.";

        internal const string Disabled = "Disabled";

        internal const string FileExistsSkipping = "File exists, skipping";

        internal const string FileExistsValidating = "File exists, validating.";

        internal const string CopyFile = "Copying file.";

        internal const string VerifyFile = "Verifying file.";

        internal const string SkippingValidation = "Skipping Validation.";
        #endregion

        internal const char InvalidChar = '�';

        internal const char WildCard = '*';

        internal const string Blank = "                                                                                                                     ";

        internal const string LineStart = "\r\t";
    }
}
