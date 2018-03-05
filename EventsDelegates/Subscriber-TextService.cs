using System;

namespace EventsDelegates
{
    public class TextService
    {
        public void OnVideoEncoded(object source, VideoEventArgs e)
        {
            Console.WriteLine("TextService: Sending a text message..." + e.Video.Title);
        }
    }
}
