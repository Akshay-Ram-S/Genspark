using Adapter.interfaces;
using Adapter.models;

class Program
{
    static void Main()
    {
        IMediaPlayer player = new MediaPlayerAdapter(new Mp3Player(), new WavPlayer());

        player.Play("audio1.mp3", "mp3"); 
        player.Play("audio2.wav", "wav"); 
        player.Play("audio3.ogg", "ogg"); 
    }
}