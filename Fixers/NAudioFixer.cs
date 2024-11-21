using AudioToText.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioToText.Fixers
{
    public class NAudioFixer : IFixer
    {
        public void FixWav(string pathToWav, string output)
        {
            using (var waveReader = new WaveFileReader(pathToWav)) //фикс wav файла по умолчанию
            {
                using var reader = new AudioFileReader(pathToWav);

                var desiredFormat = new WaveFormat(16000, 16, 1);
                using var resampler = new MediaFoundationResampler(reader, desiredFormat)
                {
                    ResamplerQuality = 60
                };
                WaveFileWriter.CreateWaveFile(output, resampler);
                Console.WriteLine("Преобразование завершено.");
            }
            
        }
    }
}
