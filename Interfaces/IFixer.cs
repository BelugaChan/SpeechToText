using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Interfaces
{
    public interface IFixer
    {
        void FixWav(string pathToWav, string output);
    }
}
