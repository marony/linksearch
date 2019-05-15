using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkSearch
{
    public interface IFileTypeUtil
    {
        bool Is(string path);
        bool Valid(string path);
        string Target(string path);
    }
}
