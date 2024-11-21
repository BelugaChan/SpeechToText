using AudioToText.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Converters
{
    public class NAudioConverter : IConverter
    {
        public void ConvertMp4ToWav(string mp4Path, string wavPath)
        {
            using (var reader = new MediaFoundationReader(mp4Path))
            {
                WaveFileWriter.CreateWaveFile(wavPath, reader);
            }
        }
    }
}
