using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace LinkSearch
{
    class Program
    {
        static JunctionUtil junctionUtil = new JunctionUtil();
        static SymbolicLinkUtil symbolicLinkUtil = new SymbolicLinkUtil();
        static HardLinkUtil hardLinkUtil = new HardLinkUtil();

        static void Error(string fileName, Exception e)
        {
            //Console.WriteLine($"Error: {fileName}\r\n{e}");
        }

        static string Process(string path)
        {
            string s = null;
            if (junctionUtil.Is(path))
            {
                // ジャンクション
                var target = junctionUtil.Target(path);
                if (junctionUtil.Valid(path))
                    s = $"Junction: {path} -> {target}";
                else
                    s = $"Invalid Junction: {path} -> {target}";
            }
            else if (symbolicLinkUtil.Is(path))
            {
                // シンボリックリンク
                var target = symbolicLinkUtil.Target(path);
                if (symbolicLinkUtil.Valid(path))
                    s = $"Symbolic Link: {path} -> {target}";
                else
                    s = $"Invalid Symbolic Link: {path} -> {target}";
            }
            else if (hardLinkUtil.Is(path))
            {
                // ハードリンク
                var target = hardLinkUtil.Target(path);
                if (hardLinkUtil.Valid(path))
                    s = $"Hard Link: {path} -> {target}";
                else
                    s = $"Invalid Hard Link: {path} -> {target}";
            }
            return s;
        }

        static IEnumerable<string> SearchAllFiles(string path)
        {
            {
                // file
                var r = new List<string>();
                try
                {
                    var files = Directory.EnumerateFiles(path);
                    foreach (var file in files)
                    {
                        try
                        {
                            var s = Process(file);
                            if (s != null)
                                r.Add(s);
                        }
                        catch (Exception e)
                        {
                            Error(file, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Error(path, e);
                }
                foreach (var file in r)
                    yield return file;
            }
            {
                // sub directories
                var r = new List<string>();
                try
                {
                    var directories = Directory.EnumerateDirectories(path);
                    foreach (var directory in directories)
                    {
                        try
                        {
                            var s = Process(directory);
                            if (s != null)
                                r.Add(s);
                            else
                                r.AddRange(SearchAllFiles(directory));
                        }
                        catch (Exception e)
                        {
                            Error(directory, e);
                        }
                    }
                }
                catch (Exception e)
                {
                    Error(path, e);
                }
                foreach (var file in r)
                    yield return file;
            }
        }

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                // エラー終了
                System.Environment.Exit(-1);
            }
            var path = args[0];
            var files = SearchAllFiles(path);
            foreach (var file in files)
                Console.WriteLine(file);
        }
    }
}
