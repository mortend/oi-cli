using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Oi.Options;

namespace Oi
{
    public class Program : Command
    {
        const string GitProcExe = "TortoiseGitProc.exe";

        public static int Main(params string[] args)
        {
            try
            {
                new Program().Execute(args);
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: " + e.Message);
                return 1;
            }
        }

        public override void Help(IEnumerable<string> args)
        {
            WriteUsage("COMMAND [options] [path ...]", "--version");

            WriteLine("\nTortoiseGit productivity tool.");
            WriteLine("\nCan handle \"fuzzy\" input and launches TortoiseGit straight from your command line.");
            WriteLine("Without showing any unnecessary alerts, and no more ALT+TAB -> explorer -> right-clicks.");

            ColumnWidth = 15;
            WriteLine("\nCommon examples:");
            WriteRow("oi .",      "Opens the sync dialog");
            WriteRow("oi a",      "Opens the add files dialog");
            WriteRow("oi c -g",   "Opens the commit dialog, showing all files in repository");
            WriteRow("oi l",      "Opens the log dialog");
            WriteRow("oi m",      "Opens the merge dialog");
            WriteRow("oi pul",    "Opens the pull dialog");
            WriteRow("oi pus",    "Opens the push dialog");
            WriteRow("oi reb",    "Opens the rebase dialog");

            ColumnWidth = 20;
            WriteLine("\nAvailable options:");
            WriteRow("-g, --global",      "Run command in git repository root");
            WriteRow("-v, --verbose",     "1) Print debug info\n2) Accept all commands\n3) Show alert dialogs");
            WriteRow("-a, --amen(d)",     "Enable 'Amend Last Commit' in 'commit' (ALT+L)");
            WriteRow("-f, --force",       "Press ENTER in the opened dialog");
            WriteRow("-m, --msg=STRING",  "Specify message for 'commit'");
            WriteRow("-u, --url=URL",     "Specify URL for 'clone'");

            ColumnWidth = 20;
            WriteLine("\nTortoiseGit commands:");

            foreach (var cmd in GitCommands.Items)
                WriteRow(cmd.Name, cmd.Description);

            WriteLine("\nAdditional arguments to TortoiseGit can be passed using '/arg1 /arg2'.");
            WriteLine("See https://tortoisegit.org/docs/tortoisegit/tgit-automation.html for more information.");
        }

        public override void Execute(IEnumerable<string> args)
        {
            var showHelp = false;
            var showVersion = false;

            var p = new OptionSet
            {
                { "h|?|help", x => showHelp = x != null },
                { "version", x => showVersion = x != null },
            };

            var extra = p.Parse(args);

            if (showVersion)
            {
                WriteLine("TortoiseGit trainer version " + ExeVersion);
                WriteLine("Copyright (C) 2016 Nazareth");
            }
            else if (showHelp || extra.Count == 0) // Implicit help on root command
                Help(extra);
            else
                Oi(extra);
        }

        void Oi(IEnumerable<string> input)
        {
            var args = new List<string>();
            var paths = new List<string>();
            var verbose = false;
            var amend = false;
            var force = false;

            var p = new OptionSet
            {
                { "g|global", x => paths.Add(GetGitRepository()) },
                { "v|verbose", x => verbose = x != null },
                { "a|amen|amend", x => amend = x != null },
                { "f|force", x => force = x != null },
                { "m=|msg=", x => args.Add("/logmsg:\"" + x + "\"") },
                { "u|url=", x => args.Add("/url:\"" + x + "\"") },
            };

            var extra = p.Parse(input);

            if (extra.Count == 0)
                extra.Add(".");

            var command = GetCommand(extra[0], !verbose);

            for (int i = 1; i < extra.Count; i++)
            {
                var arg = extra[i];
                if (File.Exists(arg) || Directory.Exists(arg))
                    paths.Add(arg);
                else if (arg.StartsWith("-") || arg.StartsWith("/"))
                {
                    // add remaining args
                    args.AddRange(extra.Skip(i));
                    break;
                }
                else
                    throw new ArgumentException("Mistyped argument? -- " + arg);
            }

            if (paths.Count == 0)
                paths.Add(Directory.GetCurrentDirectory());
            else
                args.Add("/exactpath");

            args.Add("/command:" + command);
            args.Add("/path:\"" + string.Join("*", paths) + "\"");

            if (!verbose)
            {
                args.Add("/noquestion");
                args.Add("/closeonend:2");
            }

            var tortoise = new ProcessStartInfo
            {
                FileName = GetGitProcExe(),
                Arguments = string.Join(" ", args),
                UseShellExecute = true,
            };

            if (verbose)
                WriteLine("\"" + tortoise.FileName + "\" " + tortoise.Arguments);

            new Process { StartInfo = tortoise }.Start();
            Thread.Sleep(300);

            var hWnd = IntPtr.Zero;

            // Make sure window is in foreground
            foreach (var wnd in GetWindows())
            {
                try
                {
                    var ptr = (IntPtr)wnd;

                    if (!GetText(ptr).Contains("TortoiseGit"))
                        continue;
                    if (verbose)
                        WriteLine("ForceForegroundWindow: " + ptr);

                    ForceForegroundWindow(hWnd = ptr);
                }
                catch (Exception e)
                {
                    if (verbose)
                        WriteLine("Exception: " + e.Message);
                }
            }

            if (hWnd == IntPtr.Zero)
            {
                Console.Error.WriteLine("WARNING: A window handle was not found");
                return;
            }

            if (command == "branch")
            {
                Thread.Sleep(300);
                SendKeys.SendWait("%S");
                SendKeys.SendWait("{TAB}{TAB}{TAB}{TAB}");
            }

            if (amend)
            {
                Thread.Sleep(800);
                SendKeys.SendWait("%L");
            }

            if (force)
            {
                Thread.Sleep(600);
                if (command == "commit")
                    SendKeys.SendWait("{TAB}");
                SendKeys.SendWait("{ENTER}");
            }
        }

        string GetGitRepository()
        {
            var dir = Directory.GetCurrentDirectory();

            while (dir.Length > 1)
            {
                if (Directory.Exists(Path.Combine(dir, ".git")))
                    return dir;

                dir = Path.GetDirectoryName(dir);
            }

            throw new ArgumentException("No git repository was found");
        }

        string GetCommand(string input, bool throwOnError = true)
        {
            foreach (var c in GitCommands.Items)
                if (c.Name == input)
                    return input;

            var candidates = new List<string>();
            if (input.Length > 0 && input[0] != '-')
                foreach (var c in GitCommands.Items)
                    if (c.Name.StartsWith(input))
                        candidates.Add(c.Name);

            if (candidates.Count == 1)
                return candidates[0];
            if (input == ".")
                return "sync";
            if (candidates.Contains("add"))
                return "add";
            if (candidates.Contains("branch"))
                return "branch";
            if (candidates.Contains("commit"))
                return "commit";
            if (candidates.Contains("diff"))
                return "diff";

            if (candidates.Count != 0)
                WriteLine(string.Join(", ", candidates));

            if (!throwOnError)
                return input;

            throw new ArgumentException("Mistyped command? -- " + input);
        }

        string GetGitProcExe()
        {
            return GetGitProcExe(Environment.SpecialFolder.ProgramFiles) ?? GetGitProcExe(Environment.SpecialFolder.ProgramFilesX86) ?? GitProcExe;
        }

        string GetGitProcExe(Environment.SpecialFolder folder)
        {
            var exe = Path.Combine(
                Environment.GetFolderPath(folder),
                "TortoiseGit", "bin", GitProcExe);
            return File.Exists(exe) ? exe : null;
        }

        // The following code was found on pinvoke.net.

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);


        public static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            var length = GetWindowTextLength(hWnd);
            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);

        public static ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);
            return windowHandles;
        }

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }

        internal static void ForceForegroundWindow(IntPtr hWnd)
        {
            var foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
            var appThread = GetWindowThreadProcessId(hWnd, IntPtr.Zero);

            if (foreThread != appThread)
            {
                AttachThreadInput(foreThread, appThread, true);
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
                AttachThreadInput(foreThread, appThread, false);
            }
            else
            {
                BringWindowToTop(hWnd);
                ShowWindow(hWnd, SW_SHOW);
            }
        }

        const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr pid);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool BringWindowToTop(IntPtr hWnd);
    }
}
