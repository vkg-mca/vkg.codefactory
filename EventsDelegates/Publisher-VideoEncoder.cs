using System;
using System.Threading;

namespace EventsDelegates
{
    public class VideoEncoder
    {
        //1. Define Delegate
        //2. Define Event
        //3. Raise Event

        //NOTE: Below two declarations can be replaced by single declation
        //public delegate void VideoEncodedEventHandler(object source, VideoEventArgs args);
        //public event VideoEncodedEventHandler VideoEncoded;
        public EventHandler<VideoEventArgs> VideoEncoded;


        public void Encode(Video video)
        {
            Console.WriteLine("Encoding Video...");
            Thread.Sleep(3000);
            OnVideoEncoded(video);
        }

        protected virtual void OnVideoEncoded(Video video)
        {
            if (!(null == VideoEncoded))
            {
                VideoEncoded(this, new VideoEventArgs() { Video = video });
            }
        }
    }
}
