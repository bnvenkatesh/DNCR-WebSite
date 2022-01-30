using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Facebook;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.POCO;
using SD.ACMA.POCO.PetaPoco;
using TweetSharp;

namespace SD.ACMA.BusinessLogic.SocialMedia
{
    public interface ISocialMediaHandler
    {
        IEnumerable<Post> GetNewTwitterPosts(SocialMediaAccount twitterAccount, TwitterCredentials twitterCredentials);
        void InsertNewTwitterPosts(IEnumerable<Post> latestTweets);
        List<FacebookPostModel> GetNewFacebookPosts(SocialMediaAccount facebookAccount, FacebookCredentials facebookCredentials);
        void InsertNewFacebookPosts(List<FacebookPostModel> facebookPosts);
        IEnumerable<Post> GetNewYouTubeVideos(SocialMediaAccount youTubeUser, YouTubeCredentials youtubeCredentials, int youTubeMaxRequest);
        void InserNewYouTubeVideos(IEnumerable<Post> youTubeVideos);
        List<Post> Get10LatestPost(int mediaID);
        SocialMediaModel GetSocialMediaCredentials(string socialMediaCredentialsURL);
    }

    public class SocialMediaHandler : ISocialMediaHandler
    {
        private PostService _postService;

        public SocialMediaHandler(PostService postService)
        {
            _postService = postService;
        }

        public IEnumerable<Post> GetNewTwitterPosts(SocialMediaAccount twitterAccount, TwitterCredentials twitterCredentials)
        {
            var twitterposts = new List<TwitterStatus>();

            try
            {
                long numericID = 0;
                if (long.TryParse(twitterAccount.SocialMediaId, out numericID))
                {
                    Post lastPost = _postService.GetLatestPost((int)Enums.Media.Twitter, numericID.ToString());

                    if (lastPost != null)
                    {
                        var tweets = GetTwitterPosts(twitterAccount, twitterCredentials);

                        if (tweets != null && tweets.Any())
                        {
                            twitterposts.AddRange(tweets.Where(x => x.CreatedDate.ToLocalTime() > lastPost.CreatedOn));
                        }
                    }
                    else
                    {
                        var tweets = GetTwitterPosts(twitterAccount, twitterCredentials);

                        if (tweets != null && tweets.Any())
                        {
                            twitterposts.AddRange(tweets);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (twitterposts.Count > 0)
            {
                return ConvertTwitterStatusToPost(twitterposts);
            }
            else
                return null;
        }

        private IEnumerable<Post> ConvertTwitterStatusToPost(IEnumerable<TwitterStatus> latestTweets)
        {
            var posts = new List<Post>();

            foreach (var item in latestTweets)
            {
                int mediaId = 0;

                var text = string.IsNullOrWhiteSpace(item.TextDecoded) ? "" : item.TextDecoded;
                if (text.Length > 1000)
                {
                    text = text.Substring(0, 995) + "... ";
                }
                if (!string.IsNullOrEmpty(item.Source) && item.Source.ToLower().Contains("vine"))
                    mediaId = (int)Enums.Media.Vine;
                else
                    mediaId = (int)Enums.Media.Twitter;
                if (item.User == null)
                {
                    continue;
                }
                var tweetLink = string.Format("https://twitter.com/{0}/status/{1}/", item.User.ScreenName, item.Id);

                var newPost = new Post()
                {
                    MediaId = mediaId,
                    UniqueId = item.Id.ToString(),
                    UserId = item.User.Id.ToString(),
                    Username = item.User.ScreenName,
                    Name = item.User.Name,
                    ProfilePic = item.User.ProfileImageUrl.Replace("_normal", ""),
                    CreatedOn = Convert.ToDateTime(item.CreatedDate).ToLocalTime(),
                    Source = tweetLink,
                    Text = text,
                    Title = null
                };

                posts.Add(newPost);
            }

            return posts;
        }

        private IEnumerable<TwitterStatus> GetTwitterPosts(SocialMediaAccount twitterAccount, TwitterCredentials twitterCredentials)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                TwitterService twitterService = new TwitterService(twitterCredentials.ConsumerKey, twitterCredentials.ConsumerSecret);
               twitterService.AuthenticateWith(twitterCredentials.Token, twitterCredentials.TokenSecret);

                long numericID = 0;
                if (long.TryParse(twitterAccount.SocialMediaId, out numericID))
                {                 
                    return twitterService.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions { UserId = numericID });
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void InsertNewTwitterPosts(IEnumerable<Post> latestTweets)
        {
            foreach (var item in latestTweets)
            {
                _postService.InsertPost(item);
            }
        }

        public List<FacebookPostModel> GetNewFacebookPosts(SocialMediaAccount facebookAccount, FacebookCredentials facebookCredentials)
        {
            List<FacebookPostModel> fbPosts = new List<FacebookPostModel>();
            var lastPost = _postService.GetLatestPost((int)Enums.Media.Facebook, facebookAccount.SocialMediaId);

            try
            {
                List<FacebookPostModel> fbPostsForAccount = new List<FacebookPostModel>();
                var fbDictionary2 = GetFacebookPosts(10, facebookAccount, facebookCredentials.ClientId, facebookCredentials.ClientSecret);

                if (fbDictionary2.Count() > 1)
                {
                    Facebook.JsonObject fbJsonObject = (Facebook.JsonObject)fbDictionary2.ElementAt(0);
                    IEnumerable<KeyValuePair<string, object>> fbDictionary3 = (IEnumerable<KeyValuePair<string, object>>)fbJsonObject;
                    Facebook.JsonArray facebookPosts = (Facebook.JsonArray)fbDictionary3.ElementAt(0).Value;

                    if (lastPost == null)
                    {//posible the acc.SocialMediaId is a name not a number
                        var obj = facebookPosts.FirstOrDefault();
                        if (obj != null)
                        {
                            var foundPost = FacebookPostModel.FromJson(obj.ToString());
                            lastPost = foundPost == null ? null : _postService.GetLatestPost((int)Enums.Media.Facebook, foundPost.Id.Split('_')[0]);
                        }
                    }

                    if (lastPost != null)
                    {
                        fbPostsForAccount = ConvertObjectToFacebookPostModel(facebookPosts, lastPost.CreatedOn);
                    }
                    else
                    {
                        fbPostsForAccount = ConvertObjectToFacebookPostModel(facebookPosts, null);
                    }

                    fbPosts.AddRange(fbPostsForAccount);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fbPosts;
        }

        public void InsertNewFacebookPosts(List<FacebookPostModel> facebookPosts)
        {
            foreach (var item in facebookPosts)
            {
                var text = item.Message != null ? item.Message : "";
                if (text.Length > 1000)
                {
                    text = text.Substring(0, 995) + "... ";
                }
                Post facebookPost;
                if(item.From==null)
                {
                     facebookPost = new Post
                     {
                        MediaId = (int)Enums.Media.Facebook,
                        UniqueId = item.Id,                    
                        UserId =item.Id.Split('_')[0],
                        Name ="acmadotgov",
                        Username = "acmadotgov",
                        ProfilePic = string.Format("https://graph.facebook.com/{0}/picture?type=large", item.Id.Split('_')[0]),
                        CreatedOn = Convert.ToDateTime(item.Created_time),
                        Text = text,
                        Source = item.Link
                     };   
                }
                else
                {
                    facebookPost = new Post
                    {
                        MediaId = (int)Enums.Media.Facebook,
                        UniqueId = item.Id,
                        UserId = item.From.Id,
                        Name = item.From.Name,
                        Username = item.From.Name,
                        ProfilePic = string.Format("https://graph.facebook.com/{0}/picture?type=large", item.From.Id),
                        CreatedOn = Convert.ToDateTime(item.Created_time),
                        Text = text,
                        Source = item.Link
                    };
                }
               

                var newPost = _postService.InsertPost(facebookPost);

                if (item.Type != Enums.TYPE_STATUS)
                {
                    var newAttachment = new Attachment
                    {
                        PostId = newPost,
                        LowResImage = item.Picture,
                        StandardImage = item.Picture,
                        Thumbnail = item.Picture,
                        Link = item.Link,
                        Type = item.Type
                    };

                    _postService.InsertAttachment(newAttachment);
                }
            }
        }

        public IEnumerable<Post> GetNewYouTubeVideos(SocialMediaAccount youTubeUser, YouTubeCredentials youtubeCredentials, int youTubeMaxRequest)
        {
            Post lastPost = _postService.GetLatestPost((int)Enums.Media.YouTube, youTubeUser.SocialMediaId);
            var videoFeed = GetYouTubeVids(youTubeUser, youtubeCredentials, youTubeMaxRequest);

            if (lastPost != null)
            {
                videoFeed = videoFeed.Where(x => x.CreatedOn > lastPost.CreatedOn);
            }

            return videoFeed;
        }

        private IEnumerable<Post> GetYouTubeVids(SocialMediaAccount youTubeUser, YouTubeCredentials youtubeCredentials, int youTubeMaxRequest)
        {
            try
            {
                var resp = new SearchListResponse();
                var resp2 = new ChannelListResponse();
                var isUser = false;

                YouTubeService youtube = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApplicationName = youtubeCredentials.ApplicationName,
                    ApiKey = youtubeCredentials.DeveloperKey,
                });

                SearchResource.ListRequest listRequest = youtube.Search.List("snippet");
                listRequest.Q = youTubeUser.SocialMediaId;
                listRequest.MaxResults = 1;
                listRequest.Type = "channel";
                
                try
                {
                    resp = listRequest.Execute();
                }
                catch(Exception exx)
                {
                }                  

                if(resp.Items != null && resp.Items.Count > 0)
                {
                    ChannelsResource.ListRequest channelRequest = youtube.Channels.List("contentDetails");
                    channelRequest.Id = resp.Items.FirstOrDefault().Snippet.ChannelId;
                    resp2 = channelRequest.Execute();
                    isUser = true;
                }
                else
                {
                    ChannelsResource.ListRequest channelRequest = youtube.Channels.List("contentDetails");
                    channelRequest.Id = youTubeUser.SocialMediaId;
                    resp2 = channelRequest.Execute();
                }

                var playlistRequest = youtube.PlaylistItems.List("snippet");
                playlistRequest.PlaylistId = resp2.Items.FirstOrDefault().ContentDetails.RelatedPlaylists.Uploads;
                playlistRequest.MaxResults = youTubeMaxRequest; 
                var resp3 = playlistRequest.Execute();

                var videoIds = new List<string>();

                foreach (var item in resp3.Items)
                {
                    videoIds.Add(item.Snippet.ResourceId.VideoId);
                }

                var videoRequest = youtube.Videos.List("snippet");
                videoRequest.Id = string.Join(",", videoIds);
                var resp4 = videoRequest.Execute();

                var listPlaylistItems = new List<Post>();

                foreach (var item in resp4.Items)
                {
                    var p = new Post
                            {
                                CreatedOn = item.Snippet.PublishedAt.Value,
                                MediaId = (int) Enums.Media.YouTube,
                                UniqueId = item.Id,
                                UserId = youTubeUser.SocialMediaId,
                                Username = youTubeUser.SocialMediaId,
                                Title = item.Snippet.Title,
                                Text =
                                    item.Snippet.Description.Length > 1000
                                        ? item.Snippet.Description.Substring(0, 1000)
                                        : item.Snippet.Description,
                                ProfilePic =
                                    isUser ? resp.Items.FirstOrDefault().Snippet.Thumbnails.Default.Url : string.Empty,
                                Thumbnail = item.Snippet.Thumbnails.Default.Url,
                                Attachments = new List<Attachment>
                                              {
                                                  new Attachment
                                                  {
                                                      Link =
                                                          string.Format("https://www.youtube.com/watch?v={0}",
                                                              item.Id),
                                                      Type = Enums.TYPE_VIDEO
                                                  }
                                              },
                            };

                    listPlaylistItems.Add(p);
                }

                return listPlaylistItems;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void InserNewYouTubeVideos(IEnumerable<Post> youTubeVideos)
        {
            foreach (var item in youTubeVideos)
            {
                var postId = _postService.InsertPost(item);

                item.Attachments.FirstOrDefault().PostId = postId;

                _postService.InsertAttachment(item.Attachments.FirstOrDefault());
            }
        }

        public List<Post> Get10LatestPost(int mediaID)
        {
            return _postService.Get10LatestPosts(mediaID);
        }

        private IEnumerable<object> GetFacebookPosts(int numberOfPosts, SocialMediaAccount facebookAccount, string clientId, string clientSecret)
        {
            var fbClient = new FacebookClient();

            dynamic fbresult = fbClient.Get("oauth/access_token", new
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "client_credentials",
            });

            try
            {
                fbClient.AccessToken = fbresult.access_token;
                dynamic target = fbClient.Get(String.Format("{0}/?access_token={1}&fields=posts.limit({2})", facebookAccount.SocialMediaId, fbresult.access_token, numberOfPosts));
                Dictionary<string, object>.ValueCollection fbDictionary = target.Values;
                IEnumerable<object> fbDictionary2 = fbDictionary;

                return fbDictionary2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<FacebookPostModel> ConvertObjectToFacebookPostModel(JsonArray facebookPosts, DateTime? compareDate)
        {
            var fbPostsForAccount = new List<FacebookPostModel>();

            if (compareDate != null)
            {
                foreach (var item in facebookPosts)
                {
                    FacebookPostModel post = FacebookPostModel.FromJson(item.ToString());
                    if (Convert.ToDateTime(post.Created_time) > compareDate.Value && !string.IsNullOrWhiteSpace(post.Message))
                    {
                        fbPostsForAccount.Add(post);
                    }
                }
            }
            else
            {

                foreach (var item in facebookPosts)
                {
                    FacebookPostModel post = FacebookPostModel.FromJson(item.ToString());
                    if (!string.IsNullOrWhiteSpace(post.Message))
                    {
                        fbPostsForAccount.Add(post);
                    }
                }
            }

            return fbPostsForAccount;
        }

        public SocialMediaModel GetSocialMediaCredentials(string socialMediaCredentialsURL)
        {
            var socialModel = new SocialMediaModel();
            
            try
            {
                WebRequest webRequest = WebRequest.Create(socialMediaCredentialsURL);
                Stream objStream = webRequest.GetResponse().GetResponseStream();
                using (StreamReader sr = new StreamReader(objStream))
                {
                    XDocument xdoc = XDocument.Load(sr);

                    socialModel.TwitterDetails = new TwitterCredentials
                    {
                        UserId = xdoc.Elements("socialmedia").Elements("twitter").Attributes("userid").First().Value,
                        ConsumerKey = xdoc.Elements("socialmedia").Elements("twitter").Attributes("consumerkey").First().Value,
                        ConsumerSecret = xdoc.Elements("socialmedia").Elements("twitter").Attributes("consumersecret").First().Value,
                        Token = xdoc.Elements("socialmedia").Elements("twitter").Attributes("token").First().Value,
                        TokenSecret = xdoc.Elements("socialmedia").Elements("twitter").Attributes("tokensecret").First().Value
                    };

                    socialModel.FacebookDetails = new FacebookCredentials
                    {
                        UserId = xdoc.Elements("socialmedia").Elements("facebook").Attributes("userid").First().Value,
                        ClientId = xdoc.Elements("socialmedia").Elements("facebook").Attributes("clientId").First().Value,
                        ClientSecret = xdoc.Elements("socialmedia").Elements("facebook").Attributes("clientSecret").First().Value
                    };

                    socialModel.YouTubeDetails = new YouTubeCredentials
                    {
                        Username = xdoc.Elements("socialmedia").Elements("youtube").Attributes("username").First().Value,
                        ApplicationName = xdoc.Elements("socialmedia").Elements("youtube").Attributes("applicationname").First().Value,
                        DeveloperKey = xdoc.Elements("socialmedia").Elements("youtube").Attributes("developerkey").First().Value
                    };
                }
            }
            catch(Exception ex) 
            { 
                throw ex;
            }

            return socialModel;
        }
    }
}
