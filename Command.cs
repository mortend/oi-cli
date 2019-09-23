using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Oi
{
    public abstract class Command
    {
        public virtual string Name
        {
            get { return null; }
        }

        public virtual string Description
        {
            get { return null; }
        }

        public abstract void Help(IEnumerable<string> args);
        public abstract void Execute(IEnumerable<string> args);

        protected int ColumnWidth
        {
            get; set;
        }

        protected void WriteRow(string option, string description = null)
        {
            var width = ColumnWidth > 0 ? ColumnWidth : 20;
            WriteLine(string.IsNullOrEmpty(description)
                ? "  " + option
                : "  " + option.PadRight(width) + " " + description.Replace("\n", "\n   " + new string(' ', width)));
        }

        protected void WriteUsage(params string[] usages)
        {
            var first = true;

            foreach (var args in usages)
            {
                Write(first ? "Usage: " : "  or   ");
                WriteLine(Root + " " + (Name != null ? Name + " " : "") + args);
                first = false;
            }

            if (Description != null)
                WriteLine("\n" + Description);
        }

        protected void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        protected void Write(string line)
        {
            Console.Write(line);
        }

        private static string _root;
        public static string Root
        {
            get { return _root ?? (_root = ExeName); }
            set { _root = value; }
        }

        public static string ExeName
        {
            get { return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location).Replace('-', ' '); }
        }

        public static string ExeVersion
        {
            get
            {
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                var versionParts = new List<string>(fileVersionInfo.ProductVersion.Split('.'));

                while (versionParts.Count > 2 && versionParts[versionParts.Count - 1] == "0")
                    versionParts.RemoveAt(versionParts.Count - 1);

                return string.Join(".", versionParts);
            }
        }
    }
}
