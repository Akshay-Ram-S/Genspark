using Adapter.interfaces;

namespace Adapter.models
{
    public class MediaPlayerAdapter : IMediaPlayer
    {
        private readonly Mp3Player _mp3Player;
        private readonly WavPlayer _wavPlayer;

        public MediaPlayerAdapter(Mp3Player mp3Player, WavPlayer wavPlayer)
        {
            _mp3Player = mp3Player;
            _wavPlayer = wavPlayer;
        }

        public void Play(string fileName, string format)
        {
            switch (format)
            {
                case "mp3":
                    _mp3Player.PlayMp3(fileName);
                    break;
                case "wav":
                    _wavPlayer.PlayWav(fileName);
                    break;
                default:
                    Console.WriteLine($"Unsupported format: {format}. Cannot play {fileName}.");
                    break;
            }
        }
    }
}
