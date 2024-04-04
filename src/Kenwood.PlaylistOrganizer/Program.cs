
using Kenwood.PlaylistOrganizer.Constants;
using Kenwood.PlaylistOrganizer.Exceptions;
using Kenwood.PlaylistOrganizer.Extensions;
using Kenwood.PlaylistOrganizer.Helpers;
using Kenwood.PlaylistOrganizer.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Kenwood.PlaylistOrganizer
{
    internal class Program
    {

        private const int sleep = 1500;
        private const int ioLength = 4096;
        private static string[] currentFile = new string[1];
        private static string targetFolder = string.Empty;
        private static int cursorTop = 0;
        private static int copyCount = 1;
        private static int toCopyCount = 0;
        private static long elapsedTime = 0;
        private static long timeCheckIncrement = 1000;
        private static int timeTop = 0;
        private static Stopwatch stopWatch = new Stopwatch();
        private static LayoutWriter? updateWriter;
        private static ConsoleColor originalConsoleColor;
        private static ConsoleColor originalBackgroundColor;

        static void Main(string[] args)
        {
            originalConsoleColor = Console.ForegroundColor;
            originalBackgroundColor = Console.BackgroundColor;
            Task timeTask;

            // configure the window
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = Constants.Messages.Title;
            Console.SetWindowSize(140, 20);
            Console.WindowTop = 50;
            Console.CursorVisible = false;

            PrintHeader();

            cursorTop = Console.CursorTop;

            try
            {
                var vars = new Arguments();
                var msg = new string[1];
                var playlistChk = false;
                var targetChk = false;

                if (args.Count() > 0)
                {
                    vars = new Arguments(args);

                    if (!string.IsNullOrEmpty(vars.PlayListPath)
                        && File.Exists(vars.PlayListPath))
                    {
                        RewriteLine($"Found playlist '{vars.PlayListPath}'", cursorTop);
                        Thread.Sleep(sleep);
                        playlistChk = true;
                    }
                    else
                    {
                        RewriteLine($"From parameter: '{vars.PlayListPath}' not found.", cursorTop);
                        Thread.Sleep(1500);
                    }

                    var userRootChk = Path.GetPathRoot(vars.TargetFolder);

                    if (!string.IsNullOrEmpty(vars.TargetFolder)
                        && !string.IsNullOrEmpty(userRootChk)
                        && Path.Exists(userRootChk))
                    {
                        if (!Path.Exists(vars.TargetFolder))
                        {
                            Directory.CreateDirectory(vars.TargetFolder);
                            RewriteLine($"Created target folder '{vars.TargetFolder}'", cursorTop);
                            Thread.Sleep(sleep);
                        }
                        else
                        {
                            RewriteLine($"Found target folder '{vars.TargetFolder}'", cursorTop);
                            Thread.Sleep(sleep);
                        }

                        targetChk = true;
                    }
                    else
                    {
                        throw new PlaylistException($"Path '{vars.TargetFolder}' not found.");
                    }
                }

                var userPlaylist = string.Empty;
                var userTarget = string.Empty;

                if (!playlistChk)
                {
                    while (true)
                    {
                        cursorTop = Console.CursorTop;
                        RewriteLine("Enter path to playlist file (i.e. 'c:\\temp\\playlist.m3u'): ", cursorTop);
                        userPlaylist = Console.ReadLine();

                        if (string.IsNullOrEmpty(userPlaylist))
                        {
                            continue;
                        }

                        if (File.Exists(userPlaylist))
                        {
                            break;
                        }
                        else
                        {
                            RewriteLine($"File '{userPlaylist}' not found.", cursorTop);
                            Thread.Sleep(sleep);
                        }
                    }
                }

                if (!targetChk)
                {
                    while (true)
                    {
                        RewriteLine($"Found playlist: {userPlaylist}", cursorTop);
                        Thread.Sleep(1000);
                        RewriteLine("Enter path to copy music files to: ", cursorTop);
                        userTarget = Console.ReadLine();

                        if (string.IsNullOrEmpty(userTarget))
                        {
                            continue;
                        }

                        var userRoot = Path.GetPathRoot(userTarget);

                        if (Path.Exists(userRoot))
                        {
                            if (!Path.Exists(userTarget))
                            {
                                Directory.CreateDirectory(userTarget);
                                RewriteLine($"Created target folder: {userTarget}", cursorTop);
                                Thread.Sleep(1500);
                                vars = new Arguments(userPlaylist, userTarget);

                                break;
                            }
                            else
                            {
                                RewriteLine($"Found target path: {userTarget}", cursorTop);
                                Thread.Sleep(1500);
                                vars = new Arguments(userPlaylist, userTarget);

                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to find drive '{userRoot}'");
                            Thread.Sleep(1000);
                        }
                    }
                }

                if (!vars.ValidateOnCopy.HasValue)
                {

                }

                var m3uPath = vars.PlayListPath;
                if (!File.Exists(m3uPath))
                {
                    RewriteLine($"'{m3uPath}' not found. Press any key to exit.", cursorTop);
                    Console.ReadKey();
                    RestoreConsole();
                    Environment.Exit(1);
                }

                bool validateOnCopy = false;
                bool validateonSkip = false;

                static void WipeMenu(int count)
                {
                    Console.CursorTop = cursorTop;

                    for (var cnt = 0; cnt <= count; cnt++)
                    {
                        Console.WriteLine($"{Messages.Blank}{Messages.LineStart}");
                    }

                    Console.CursorTop = cursorTop;
                }

                if (!vars.ValidateOnCopy.HasValue && !vars.ValidateOnSkip.HasValue)
                {
                    var run = true;
                    while (run)
                    {
                        RewriteLine(Menus.ValidationExplaination, cursorTop);

                        Console.Write(Menus.AskValidationMenu);

                        var response = Console.ReadKey();

                        switch (response.KeyChar)
                        {
                            case '1':
                                {
                                    validateOnCopy = true;
                                    validateonSkip = true;
                                    run = false;
                                    break;
                                }
                            case '2':
                                {
                                    validateOnCopy = true;
                                    run = false;
                                    break;
                                }
                            case '3':
                                {
                                    validateonSkip = true;
                                    run = false;
                                    break;
                                }
                            default:
                                {
                                    WipeMenu(Console.CursorTop - cursorTop);
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    validateOnCopy = vars.ValidateOnCopy.HasValue ? vars.ValidateOnCopy.Value : false;
                    validateonSkip = vars.ValidateOnSkip.HasValue ? vars.ValidateOnSkip.Value : false;
                }

                WipeMenu(Console.CursorTop - cursorTop);

                var sortPlaylist = false;
                if (vars.SortPlaylist is null)
                {
                    RewriteLine(Menus.AskSortPlaylistMenu, cursorTop);
                    var response = Console.ReadKey();

                    sortPlaylist = response.KeyChar == '1' ? true : false;
                }
                else
                {
                    sortPlaylist = true;
                }

                // read the m3u data
                var playlistText = File.ReadAllLines(m3uPath);
                var sourceFiles = new List<string>();
                var regexPath = new Regex(Constants.Messages.M3uPathRegex, RegexOptions.Multiline);

                RewriteLine("Parsing playlist data.", cursorTop);

                var healCount = 0;
                var failHealCount = 0;
                var animationIncrement = playlistText.Length / 5;
                var animationCount = animationIncrement;
                var progressCount = 0;

                // build the song list
                foreach (var entry in playlistText)
                {
                    progressCount++;

                    if (progressCount >= animationCount)
                    {
                        Thread.Sleep(500);
                        Console.Write(".");
                        animationCount += animationIncrement;
                    }

                    if (regexPath.IsMatch(entry))
                    {
                        var songPath = new List<string> { regexPath.Match(entry).Value };

                        foreach (var item in songPath)
                        {
                            if (!sourceFiles.Contains(item))
                            {
                                sourceFiles.Add(item);
                            }
                        }
                    }
                }

                RewriteLine("Verifying path to music files exists.", cursorTop);
                Thread.Sleep(sleep);

                // select a song path value to validate
                // if (songPath[0].Any(s => char.Equals(invalidChar, s)))
                var checkPath = sourceFiles.First(s => s.ToCharArray().Any(c => !char.Equals(Messages.InvalidChar, c)));

                string? updatePath = null;
                //var isFullyQualifiedPath = false;
                // check the file path is fully qualified
                if (!Path.IsPathFullyQualified(checkPath))
                {
                    RewriteLine("M3u music file path is not fully qualified. Searching.", cursorTop);
                    Thread.Sleep(sleep);


                    // try the m3u folder
                    var chkIndex = m3uPath.LastIndexOf('\\');
                    var m3uDir = m3uPath.Substring(0, chkIndex);

                    RewriteLine($"Trying m3u path '{m3uDir}'", cursorTop);
                    Thread.Sleep(1000);

                    // check
                    var m3uChkPath = Path.Combine(m3uDir, checkPath);
                    if (Path.Exists(m3uChkPath))
                    {
                        RewriteLine($"Found '{m3uChkPath}'", cursorTop);
                        updatePath = m3uDir;
                    }
                    else
                    {
                        RewriteLine($"'{m3uChkPath}' not found.", cursorTop);
                    }

                    Thread.Sleep(sleep);

                    var m3uDrive = Path.GetPathRoot(m3uPath);
                    // try the m3u drive
                    if (updatePath is null)
                    {
                        if (!string.IsNullOrEmpty(m3uDrive))
                        {
                            RewriteLine($"Trying m3u drive '{m3uDrive}'", cursorTop);
                            Thread.Sleep(sleep);

                            var m3uTest = Path.Combine(m3uDrive, checkPath);

                            if (File.Exists(m3uTest))
                            {
                                RewriteLine($"Found '{m3uTest}', updating file list.", cursorTop);
                                Thread.Sleep(sleep);

                                updatePath = m3uDrive;
                            }
                            else
                            {
                                RewriteLine($"'{checkPath}' does not exist on drive '{m3uDrive}'", cursorTop);
                                Thread.Sleep(sleep);
                            }
                        }
                    }

                    // not same drive as m3u, try enumerating drives
                    if (updatePath is null)
                    {
                        RewriteLine("Enumerating logical drives.", cursorTop);
                        Thread.Sleep(sleep);

                        foreach (var drive in DriveInfo.GetDrives())
                        {
                            if (!string.Equals(drive.Name, m3uDrive, StringComparison.CurrentCultureIgnoreCase))
                            {
                                RewriteLine($"Checking for '{checkPath}' on drive '{drive.Name}'", cursorTop);
                                Thread.Sleep(sleep);

                                if (File.Exists(Path.Combine(drive.Name, checkPath)))
                                {
                                    RewriteLine($"Found '{checkPath}' on drive '{drive.Name}'", cursorTop);
                                    Thread.Sleep(sleep);

                                    updatePath = drive.Name;
                                    break;
                                }
                                else
                                {
                                    RewriteLine($"'{checkPath}' not found on drive '{drive.Name}'", cursorTop);
                                    Thread.Sleep(sleep);
                                }
                            }
                        }
                    }

                    // did not find path
                    if (updatePath is null)
                    {
                        var chk = false;
                        while (chk)
                        {
                            RewriteLine($"Please enter the path to '{checkPath}': ", cursorTop);
                            var userPath = Console.ReadLine();

                            if (string.IsNullOrEmpty(userPath))
                            {
                                Thread.Sleep(500);
                                continue;
                            }
                            else
                            {
                                var updated = Path.Combine(userPath, checkPath);

                                if (Path.Exists(updated))
                                {
                                    RewriteLine($"Found '{updated}': ", cursorTop);
                                    Thread.Sleep(1000);
                                    break;
                                }
                            }
                        }
                    }
                }

                RewriteLine($"Cleaning up playlist file paths.", cursorTop);
                Thread.Sleep(1000);

                // clean up music file paths
                var tempStore = new List<string>();
                foreach (var path in sourceFiles)
                {
                    var pathTemp = new string[1] { path };

                    // prepend found path if it exists
                    if(updatePath is not null)
                    {
                        pathTemp[0] = Path.Join(updatePath, pathTemp[0]);
                    }

                    // fix invalid chars
                    var pathContainer = new List<string>();
                    if (pathTemp[0].Any(s => char.Equals(Messages.InvalidChar, s)))
                    {
                        var directory = Path.GetDirectoryName(path);

                        if (string.IsNullOrEmpty(directory))
                        {
                            throw new InvalidOperationException($"Unable to find directory {path}");
                        }

                        var badSongName = Path.GetFileName(pathTemp[0]);
                        var searchName = badSongName.Replace(Messages.InvalidChar, Messages.WildCard);
                        var enumerated = Directory.EnumerateFiles(directory, searchName);

                        
                        if (enumerated.Any())
                        {
                            foreach (var healed in enumerated)
                            {
                                if (File.Exists(healed))
                                {
                                    healCount++;
                                    pathContainer.AddRange(enumerated.Select(s => s.Trim()));
                                }
                            }
                        }
                        else // failed to heal
                        {
                            failHealCount++;
                        }
                    }
                    else
                    {
                        pathContainer.Add(pathTemp[0]);
                    }

                    foreach (var item in pathContainer)
                    {
                        if (!tempStore.Contains(item))
                        {
                            tempStore.Add(item);
                        }
                    }
                }

                sourceFiles = new List<string>(tempStore);

                // if requested, sort the list alphabetically
                if (sortPlaylist)
                {
                    sourceFiles.Sort();
                }

                RewriteLine($"Healed {healCount} file names, {failHealCount} were skipped. See log for details.", cursorTop);
                Thread.Sleep(sleep);
                RewriteLine($"Found {sourceFiles.Count} song entries in playlist.", cursorTop);
                targetFolder = vars.TargetFolder;

                if (!Path.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                    RewriteLine($"Created target directory '{targetFolder}'", cursorTop);
                }

                toCopyCount = sourceFiles.Count - failHealCount;

                RewriteLine($"Copying {toCopyCount} file(s) to: '{targetFolder}'", cursorTop);
                Thread.Sleep(sleep);

                stopWatch.Start();

                var successCount = 0;
                var iteration = 0;
                var skipCount = 0;
                var layout = Layout.PrintLayout(m3uPath, targetFolder);

                updateWriter = new LayoutWriter(layout);
                var timeHelper = new TimeHelper(ref updateWriter, ref stopWatch);
                // launch time updater in new thread
                timeTask = Task.Run(() => timeHelper.Run());

                foreach (var file in sourceFiles)
                {
                    updateWriter.UpdateCopy("-");
                    updateWriter.UpdateValidation("-");
                    iteration++;

                    var fileName = Regex.Match(file, Messages.M3uFileRegex, RegexOptions.Multiline).Groups[1].Value;
                    var targetPath = Path.Join(targetFolder, fileName);

                    updateWriter.UpdateFile(fileName, iteration, toCopyCount);

                    if (File.Exists(file))
                    {
                        if (File.Exists(targetPath))
                        {
                            if (validateonSkip)
                            {
                                updateWriter.UpdateMessage(Messages.FileExistsValidating);

                                PrintValidateLoop();

                                if (!ValidateWithRetry(file, targetPath, out var skipped))
                                {
                                    updateWriter.UpdateMessage($"Failed to copy {fileName}", true);
                                    Thread.Sleep(sleep);
                                }

                                if (skipped) { skipCount++; }
                            }
                            else
                            {
                                updateWriter.UpdateMessage(Messages.FileExistsSkipping);
                                Thread.Sleep(500);
                            }

                            skipCount++;
                        }
                        else
                        {
                            copyCount++;
                            currentFile[0] = fileName;

                            updateWriter.UpdateMessage(Messages.CopyFile);

                            CopyWithProgress(file, targetPath);

                            if (validateOnCopy)
                            {
                                updateWriter.UpdateMessage(Messages.VerifyFile);
                                PrintValidateLoop();

                                if (!ValidateWithRetry(file, targetPath, out var skipped))
                                {
                                    updateWriter.UpdateMessage($"Failed to copy {fileName}", true);
                                    Thread.Sleep(sleep);
                                }
                            }
                            else
                            {
                                updateWriter.UpdateMessage(Messages.SkippingValidation);
                                Thread.Sleep(500);
                            }
                        }
                    }
                    else
                    {
                        updateWriter.UpdateMessage($"Could not find source file '{file}'; skipping.", true);
                        Thread.Sleep(sleep);
                        // TODO - log
                    }
                }

                timeHelper.Stop();
                stopWatch.Stop();

                // TODO - cleanup screen
                var clear = "\r                                                                                                             ";
                Console.CursorTop = layout.FilePosition;
                Console.Write(clear);
                Console.CursorTop = layout.ProgressPosition;
                Console.Write(clear);
                Console.CursorTop = layout.ValidationPosition;
                Console.Write(clear);
                Console.CursorTop = layout.MessagePosition;
                Console.Write(clear);
                Console.CursorTop = layout.TimePosition;
                Console.Write(clear);

                Console.CursorTop = layout.FilePosition;

                var hours = stopWatch.Elapsed.Hours;
                var minutes = stopWatch.Elapsed.Minutes;
                var seconds = stopWatch.Elapsed.Seconds;

                var sb = new StringBuilder();

                var hasHours = false;
                var hasMinutes = false;
                if (hours == 1)
                {
                    hasHours = true;
                    sb.Append($"{hours} Hour");
                }
                else if(hours >= 2)
                {
                    hasHours = true;
                    sb.Append($"{hours} Hours");
                }

                if (minutes == 1)
                {
                    hasMinutes = true;
                    if (hasHours)
                    {
                        sb.Append(" ");
                    };

                    sb.Append($"{minutes} Minute");
                }
                else if (minutes >= 2)
                {
                    hasMinutes = true;
                    if (hasHours)
                    {
                        sb.Append(" ");
                    };
                    sb.Append($"{minutes} Minutes");
                }

                if (seconds == 1)
                {
                    if (hasMinutes)
                    {
                        sb.Append(" ");
                    };
                    sb.Append($"{seconds} Second");
                }
                else if (seconds >= 2)
                {
                    if (hasMinutes)
                    {
                        sb.Append(" ");
                    };
                    sb.Append($"{seconds} Seconds");
                }

                Console.WriteLine("\r\n");
                Console.WriteLine($"{Messages.LineStart}Completed. Copied {successCount} out of {toCopyCount} file(s).\r\n");
                Console.WriteLine($"{Messages.LineStart}Time Elapsed: {sb}\r\n");
                Console.WriteLine($"{Messages.LineStart}{skipCount} file(s) already existed and were skipped.\r\n");
                Console.WriteLine($"{Messages.LineStart}Press any key to exit.");
                Console.ReadKey();
                RestoreConsole();
                Environment.Exit(0);
            }
            catch (PlaylistException ex)
            {
                Console.WriteLine($"\r\n\r\n{Messages.LineStart  }{Messages.Title} encountered an error: {ex.Message}");
                Console.WriteLine($"{Messages.LineStart}Press any key to exit.");
                Console.ReadKey();
                RestoreConsole();
                Environment.Exit(1);
            }
            catch (Exception)
            {
                Console.WriteLine($"\r\n\r\n{Messages.LineStart}{Messages.Title} encountered an error. Please see the log for more details.");
                Console.WriteLine($"{Messages.LineStart}Press any key to exit.");
                Console.ReadKey();
                RestoreConsole();
                Environment.Exit(1);
            }
        }

        #region old

        //private static void CopyPlaylist(CopyProperties props)
        //{

        //var m3uPath = args.PlayListPath;
        //if (!File.Exists(m3uPath))
        //{
        //    RewriteLine($"'{m3uPath}' not found. Press any key to exit.", cursorTop);
        //    Console.ReadKey();
        //    Environment.Exit(1);
        //}

        //var playlistText = File.ReadAllLines(m3uPath);
        //var sourceFiles = new List<string>();
        //var regexPath = new Regex(Constants.M3uPathRegex, RegexOptions.Multiline);

        //RewriteLine("Parsing playlist data", cursorTop);

        //var healCount = 0;
        //var failHealCount = 0;
        //var animationIncrement = playlistText.Length / 5;
        //var animationCount = animationIncrement;
        //var progressCount = 0;
        //foreach (var entry in playlistText)
        //{ 
        //    progressCount++;

        //    if (progressCount >= animationCount)
        //    {
        //        Console.Write(".");
        //        animationCount += animationIncrement;
        //    }

        //    if (regexPath.IsMatch(entry))
        //    {
        //        var songPath = new List<string> { regexPath.Match(entry).Value };

        //        if (songPath[0].Any(s => Path.GetInvalidFileNameChars().Contains(s)))
        //        {
        //            foreach (var chr in songPath[0])
        //            {
        //                // TODO
        //            }
        //        }

        //        if (songPath[0].Any(s => char.Equals(invalidChar, s)))
        //        {
        //            var directory = Path.GetDirectoryName(songPath[0]);

        //            if (string.IsNullOrEmpty(directory))
        //            {
        //                throw new InvalidOperationException($"Unable to find directory {songPath[0]}");
        //            }

        //            var badSongName = Path.GetFileName(songPath[0]);
        //            var searchName = badSongName.Replace(invalidChar, '*');
        //            var enumerated = Directory.EnumerateFiles(directory, searchName);

        //            if (enumerated.Any())
        //            {
        //                foreach (var healed in enumerated)
        //                {
        //                    if (File.Exists(healed))
        //                    {
        //                        healCount++;
        //                        songPath = new List<string>(enumerated.Select(s => s.Trim()));
        //                    }
        //                }
        //            }
        //            else // failed to heal
        //            {
        //                failHealCount++;
        //            }
        //        }

        //        foreach (var item in songPath)
        //        {
        //            if (!sourceFiles.Contains(item))
        //            {
        //                sourceFiles.Add(item);
        //            }
        //        }

        //    }
        //}

        //RewriteLine($"Healed {healCount} file names, {failHealCount} were skipped. See log for details.", cursorTop);

        //Thread.Sleep(3000);

        //RewriteLine($"Found {sourceFiles.Count} song entries in playlist.", cursorTop);

        //targetFolder = args.TargetFolder;

        //if (!Path.Exists(targetFolder))
        //{
        //    Directory.CreateDirectory(targetFolder);

        //    RewriteLine($"Created target directory '{targetFolder}'", cursorTop);
        //}

        //toCopyCount = sourceFiles.Count - failHealCount;

        //RewriteLine($"Copying {toCopyCount} file(s) to: '{targetFolder}'", cursorTop);
        //Thread.Sleep(sleep);
        //stopWatch.Start();



        //var regexFile = new Regex(Constants.M3uFileRegex, RegexOptions.Multiline);
        //var successCount = 0;

        //timeTop = cursorTop;
        //cursorTop = cursorTop + 4;
        //var skipCount = 0;

        //foreach (var file in props.FileList)
        //{
        //    PrintElapsedTime(true);

        //    if (File.Exists(file))
        //    {
        //        var fileName = Regex.Match(file, Constants.M3uFileRegex, RegexOptions.Multiline).Groups[1].Value;
        //        var targetPath = Path.Join(targetFolder, fileName);

        //        if (File.Exists(targetPath))
        //        {
        //            RewriteLine($"File '{fileName}' exists, validating target.", cursorTop);

        //            if (ValidateTargetFile(file, targetPath))
        //            {
        //                RemoveValidationMessage(cursorTop);
        //                copyCount++;
        //                skipCount++;
        //            }
        //            {
        //                // loop retries
        //            }

        //        }
        //        else
        //        {
        //            copyCount++;
        //            currentFile[0] = fileName;
        //            CopyWithProgress(file, targetPath);
        //            Console.Write(nLine);

        //            // loop retries

        //            if (ValidateTargetFile(file, targetPath))
        //            {
        //                RemoveValidationMessage(cursorTop);
        //                successCount++;
        //            }
        //            else
        //            {
        //                // loop
        //            }
        //        }
        //    }
        //    else
        //    {
        //        RewriteLine($"{nLine}Could not find source file '{file}'; skipping.", cursorTop);

        //        // TODO - log
        //    }
        //}

        //stopWatch.Stop();

        //// TODO - cleanup screen

        //Console.WriteLine("\r\n");
        //Console.WriteLine($"{nLine}Completed. Copied {successCount} out of {toCopyCount} file(s) in {stopWatch.Elapsed.TotalMinutes} minute(s).\r\n");
        //Console.WriteLine($"{nLine}{skipCount} file(s) already existed and were skipped.");
        //Console.WriteLine($"{nLine}Press any key to exit.");
        //Console.ReadKey();

        //Environment.Exit(0);
        // }

        #endregion

        private static bool ValidateWithRetry(string source, string target, out bool skipped)
        {
            var result = false;
            skipped = false;

            if (ValidationHelpers.CompareFiles(source, target))
            {
                updateWriter.UpdateMessage(Messages.ValidationSuccess);
                Thread.Sleep(1000);

                copyCount++;
                skipped = true;
                result = true;
            }
            else
            {
                updateWriter.UpdateMessage(Messages.ValidationFail, true);
                Thread.Sleep(sleep);
                updateWriter.UpdateMessage("Re-copying file.");
                // one retry
                CopyWithProgress(source, target);

                updateWriter.UpdateMessage("Validating file.");

                PrintValidateLoop();

                if (ValidationHelpers.CompareFiles(source, target))
                {
                    updateWriter.UpdateMessage(Messages.ValidationSuccess);
                    Thread.Sleep(1000);

                    copyCount++;
                    result = true;
                }
            }

            return result;
        }

        private static void RestoreConsole()
        {
            Console.ForegroundColor = originalConsoleColor;
            Console.BackgroundColor = originalBackgroundColor;
            Console.CursorVisible = true;
        }

        private static void CopyWithProgress(string source, string target)
        {
            using (var outStream = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None, ioLength))
            {
                using (var inStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.None, ioLength))
                {
                    using (ProgressStream progressStream = new ProgressStream(inStream))
                    {
                        progressStream.UpdateProgress += ProgressStream_UpdateProgress;
                        progressStream.CopyTo(outStream);
                    }
                }

                outStream.Flush();
            }
        }

        private static void PrintValidationProgress(long progress, int cursor)
        {
            float val = progress * 100.0f;

            updateWriter.UpdateValidation($"{val.ToString("0.00")}% complete.");
        }

        private static void ProgressStream_UpdateProgress(object? sender, ProgressEventArgs e)
        {
            float val = e.Progress * 100.0f;

            updateWriter.UpdateCopy($"{val.ToString("0.00")}%");
        }

        private static void RewriteLine(string text, int top)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = top;
            Console.Write(Messages.Blank);
            Console.Write($"{Messages.LineStart}{text}");
        }

        private static void PrintHeader()
        {
            var titleArray = Messages.Title.Split(' ');

            Console.Write(titleArray[0]);
            Thread.Sleep(500);
            Console.Write($" {titleArray[1]}");
            Thread.Sleep(500);
            Console.Write($" {titleArray[2]}");
            Console.Write("\r\n\r\n");
            Thread.Sleep(500);
        }

        private static void PrintValidateLoop(bool second = false)
        {
            var start = second ? 50 : 0;
            var end = second ? 50 : 100;

            for (var cnt = 0; cnt <= 100; cnt++)
            {
                updateWriter.UpdateValidation($"{cnt}%");
                Thread.Sleep(13);
            }
        }
    }
}
