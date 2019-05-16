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
        private static IFileTypeUtil[] fileTypeUtils = new IFileTypeUtil[]
        {
            new JunctionUtil(),
            new SymbolicLinkUtil(),
            new HardLinkUtil(),
        };

        class FileTypeInfo
        {
            public FileTypeInfo(string path, LinkType linkType, string linkTypeName, bool valid, string[] targets)
            {
                this.Path = path;
                this.LinkType = linkType;
                this.LinkTypeName = linkTypeName;
                this.Valid = valid;
                this.Targets = targets;
            }

            public string Path { get; private set; }
            public LinkType LinkType { get; private set; }
            public string LinkTypeName { get; private set; }
            public bool Valid { get; private set; }
            public string[] Targets { get; private set; }
        }

        static void Error(string fileName, Exception e)
        {
            //Console.WriteLine($"Error: {fileName}\r\n{e}");
        }

        static FileTypeInfo Process(string path)
        {
            foreach (var util in fileTypeUtils)
            {
                if (util.Is(path))
                {
                    var linkType = util.GetLinkType();
                    var linkTypeName = util.GetLinkTypeName();
                    var valid = util.Valid(path);
                    var targets = new string[0];
                    if (valid)
                        targets = util.Targets(path);
                    return new FileTypeInfo(path, linkType, linkTypeName, valid, targets);
                }
            }

            return null;
        }

        static IEnumerable<FileTypeInfo> SearchAllFiles(string path)
        {
            {
                // file
                var r = new List<FileTypeInfo>();
                try
                {
                    var files = Directory.EnumerateFiles(path);
                    foreach (var file in files)
                    {
                        try
                        {
                            var info = Process(file);
                            if (info != null)
                                r.Add(info);
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
                var r = new List<FileTypeInfo>();
                try
                {
                    var directories = Directory.EnumerateDirectories(path);
                    foreach (var directory in directories)
                    {
                        try
                        {
                            var info = Process(directory);
                            if (info != null)
                                r.Add(info);
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
            var infos = SearchAllFiles(path);
            foreach (var info in infos)
            {
                // リンクを出力
                foreach (var target in info.Targets)
                {
                    Console.WriteLine($"{info.LinkTypeName},{info.Valid},{info.Path},{target}");
                }
            }
#if DEBUG
            Console.WriteLine("[END]");
            Console.ReadKey();
#endif
        }
    }
}
