using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO
{
    public class SocialMediaModel
    {
        public TwitterCredentials TwitterDetails { get; set; }
        public FacebookCredentials FacebookDetails { get; set; }
        public YouTubeCredentials YouTubeDetails { get; set; }
    }

    public class TwitterCredentials
    {
        public string UserId { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Token { get; set; }
        public string TokenSecret { get; set; }
    }

    public class FacebookCredentials
    {
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class YouTubeCredentials
    {
        public string Username { get; set; }
        public string ApplicationName { get; set; }
        public string DeveloperKey { get; set; }
    }

    public class SocialMediaDTO
    {
        public int Counter { get; set; }
        public StringBuilder SQLQuery { get; set; }
        public List<object> ParameterList { get; set; }

        public string SearchTerm { get; set; }
        public int MediaId { get; set; }
        public int NumberOfPosts { get; set; }
    }
}
