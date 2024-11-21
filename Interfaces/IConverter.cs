using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Interfaces
{
    public interface IConverter
    {
        void ConvertMp4ToWav(string mp4Path, string wavPath);
    }
}
