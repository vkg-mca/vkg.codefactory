using System;

namespace EventsDelegates
{
    class Program
    {
        static void Main(string[] args)
        {

            var video = new Video() { Title = "Video-1" };
            var videoEncoder = new VideoEncoder();//publisher

            var mailService = new MailService();//subscriber
            var textService = new TextService();//subsriber
            videoEncoder.VideoEncoded += mailService.OnVideoEncoded;
            videoEncoder.VideoEncoded += textService.OnVideoEncoded;
            videoEncoder.Encode(video);
            Console.ReadLine();
        }
    }
}
