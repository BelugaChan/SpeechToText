using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vosk;

namespace AudioToText.Recognizers
{
    public class VoskRecognize
    {
        public void RecognizeAudio(Model model, string pathToFixedWav)
        {
            using (var fixedReader = new WaveFileReader(pathToFixedWav))
            {
                var recognizer = new VoskRecognizer(model, fixedReader.WaveFormat.SampleRate);

                byte[] buffer = new byte[4096];
                int bytesRead;
                Console.WriteLine("Распознавание началось");
                using StreamWriter writer = new(Path.Combine(Environment.CurrentDirectory, "res.txt"), true);
                while ((bytesRead = fixedReader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (recognizer.AcceptWaveform(buffer, bytesRead))
                    {
                        string text = recognizer.Result();
                        Console.WriteLine("Partial: " + text);
                        writer.WriteLine(text);
                        writer.Flush();
                    }
                }

            }
        }
    }
}
