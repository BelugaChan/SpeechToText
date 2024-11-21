using NAudio.Wave;
using System.Speech.Recognition;
using Concentus.Oggfile;
using Concentus.Structs;
using Vosk;
using AudioToText.Fixers;
using AudioToText.Converters;
using AudioToText.Recognizers;
namespace AudioToText
{
    internal class Program
    {
              
        private static readonly string pathToModel = @"D:\\uFiler\\AudioToText\\model";
        private static readonly string pathToMp4 = @"C:\\Users\\Admin\\Downloads\\input.mp4";
        static void Main(string[] args)
        {
            string wavFile = Path.Combine(Environment.CurrentDirectory, "output.wav");
            NAudioConverter nAudioConverter = new NAudioConverter();
            nAudioConverter.ConvertMp4ToWav(pathToMp4, wavFile);
            
            Vosk.Vosk.SetLogLevel(0);
            var model = new Model(pathToModel);
            
            string pathToFixedWav = Path.Combine(Environment.CurrentDirectory, "fixedWav.wav");
            NAudioFixer nAudioFixer = new NAudioFixer();
            nAudioFixer.FixWav(wavFile, pathToFixedWav);

            VoskRecognize voskRecognize = new VoskRecognize();
            voskRecognize.RecognizeAudio(model, pathToFixedWav);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public static void FixWav(string pathToWav, string output)
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

        public static void ChangeFrequency(string mp3Path, string changedRateWavPath, int newSampleRate)
        {
            using (var reader = new Mp3FileReader(mp3Path))
            {
                var newFormat = new WaveFormat(newSampleRate, reader.WaveFormat.BitsPerSample, reader.WaveFormat.Channels);

                using (var conversionStream = new WaveFormatConversionStream(newFormat,reader))
                {
                    using (var writer = new WaveFileWriter(changedRateWavPath, conversionStream.WaveFormat))
                    {
                        conversionStream.CopyTo(writer);
                        Console.Write("Yosh!");
                    }
                }
            }
        }

        public static int GetSampleRate(string mp3Path)
        {
            int sampleRate = 0;
            using (var mp3Reader = new Mp3FileReader(mp3Path))
            {
                sampleRate = mp3Reader.Mp3WaveFormat.SampleRate;
                Console.WriteLine($"Sample Rate: {sampleRate} Hz");
            }
            return sampleRate;
        }

        public static void ConvertMp4ToWav(string mp4Path, string wavPath)
        {
            using (var reader = new MediaFoundationReader(mp4Path))
            {
                WaveFileWriter.CreateWaveFile(wavPath, reader);
            }
        }

        static void ConvertWavToOgg(string inputPath, string outputPath)
        {
            using (var mp3Reader = new WaveFileReader(inputPath))
            using (var pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
            using (var oggOut = new FileStream(outputPath, FileMode.Create))
            {
                OpusEncoder encoder = new OpusEncoder
                    (pcmStream.WaveFormat.SampleRate,
                    pcmStream.WaveFormat.Channels,
                    Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);
                //var encoder = OpusEncoder.Create(
                //    pcmStream.WaveFormat.SampleRate,
                //    pcmStream.WaveFormat.Channels,
                //    Concentus.Enums.OpusApplication.OPUS_APPLICATION_AUDIO);

                var oggWriter = new OpusOggWriteStream(encoder, oggOut);

                byte[] buffer = new byte[pcmStream.WaveFormat.BlockAlign * 480];
                int bytesRead;

                short[] shortBuffer = new short[buffer.Length / 2];

                while ((bytesRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead / 2; i++)
                    {
                        shortBuffer[i] = BitConverter.ToInt16(buffer, i * 2);
                    }
                    oggWriter.WriteSamples(shortBuffer, 0, bytesRead / pcmStream.WaveFormat.BlockAlign);
                }

                oggWriter.Finish();
            }
        }
    }
}
