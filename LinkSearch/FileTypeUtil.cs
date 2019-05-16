using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkSearch
{
    public enum LinkType
    {
        Invalid,
        Junction,
        SymbolicLink,
        HardLink,
    }

    public interface IFileTypeUtil
    {
        LinkType GetLinkType();
        string GetLinkTypeName();
        bool Is(string path);
        bool Valid(string path);
        string[] Targets(string path);
    }
}
