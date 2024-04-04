using Kenwood.PlaylistOrganizer.Constants;


namespace Kenwood.PlaylistOrganizer.Models
{
    internal class Arguments
    {
        private const char trimChar = '/';
        private const char seperator = '=';
        private readonly string playlistPath; 
        private readonly string targetFolder;
        private readonly bool? validateOnCopy;
        private readonly bool? validateOnSkip;
        private readonly bool? noValidate;
        private readonly bool? sortPlaylist;

        internal Arguments()
        { 
            playlistPath = string.Empty;
            targetFolder = string.Empty;
        }

        internal Arguments(string playlistPath, string targetFolder)
        {
            this.playlistPath = playlistPath;
            this.targetFolder = targetFolder;
        }

        internal Arguments(string[] args) 
        {
            try
            {
                foreach (var arg in args)
                {
                    if (string.Equals(arg.Trim(trimChar), Messages.ValidateOnCopy, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validateOnCopy = true;
                        validateOnSkip = false;
                    }
                    else if (string.Equals(arg.Trim(trimChar), Messages.ValidateOnSkip, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validateOnSkip = true;
                        validateOnCopy = false;
                    }
                    else if (string.Equals(arg.Trim(trimChar), Messages.NoValidation, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validateOnSkip = false;
                        validateOnCopy = false;
                        noValidate = true;
                    }
                    else if (string.Equals(arg.Trim(trimChar), Messages.ValidateAll, StringComparison.CurrentCultureIgnoreCase))
                    {
                        validateOnSkip = true;
                        validateOnCopy = true;
                    }
                    else if (string.Equals(arg.Trim(trimChar), Messages.NoSortPlaylist, StringComparison.CurrentCultureIgnoreCase))
                    {
                        sortPlaylist = true;
                    }
                    else if (string.Equals(arg.Trim(trimChar), Messages.SortPlaylist, StringComparison.CurrentCultureIgnoreCase))
                    {
                        sortPlaylist = true;
                    }
                    else
                    {
                        var index = arg.IndexOf(seperator);
                        var argKey = arg.Substring(0, index);
                        var argValue = arg.Substring(index + 1);

                        if (string.Equals(argKey.Trim(trimChar), Messages.Playlist, StringComparison.CurrentCultureIgnoreCase))
                        {
                            playlistPath = argValue;
                        }
                        else if (string.Equals(argKey.Trim(trimChar), Messages.TargetPath, StringComparison.CurrentCultureIgnoreCase))
                        {
                            this.targetFolder = argValue;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("There was a problem parsing the command arguments.");
            }

            if (string.IsNullOrEmpty(playlistPath) ||
                string.IsNullOrEmpty(targetFolder))
            {
                throw new InvalidOperationException("One of the required parameters was missing.");
            }
        }

        internal string PlayListPath => playlistPath;

        internal string TargetFolder => targetFolder;

        internal bool? ValidateOnCopy => validateOnCopy;

        internal bool? ValidateOnSkip => validateOnSkip;

        internal bool? NoValidate => noValidate;

        internal bool? SortPlaylist => sortPlaylist;
    }
}
