using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SD.ACMA.BusinessLogic.SocialMedia;
using SD.ACMA.DNCR.Infrastructure;
using SD.ACMA.DNCR.Infrastructure.Configuration;
using SD.ACMA.POCO;
using SD.ACMA.POCO.PetaPoco;
using SD.ADMC.DNCRProject.SocialMediaWindowsService;

namespace SD.ACMA.DNCRProject.SocialMediaWindowsService
{
    public partial class SocialMediaService : ServiceBase
    {
        private ISocialMediaHandler _socialMediaHandler;

        Timer timer1 = new Timer();

        public SocialMediaService()
        {
            InitializeComponent();

            _socialMediaHandler = Factory.CreateSocialMediaHandlerInstance();
        }

        public void OnDebug()
        {
            StartProcess();
           // OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            int loopInterval = ConfigurationHelper.Instance.LoopInterval;

            timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
            timer1.Interval = loopInterval; //86400000;
            timer1.Enabled = true;
            timer1.Start();
        }

        private void timer1_Elapsed(object sender, EventArgs e)
        {
            StartProcess();
        }

        protected override void OnStop()
        {
            timer1.Enabled = false;
        }

        public void StartProcess()
        {
            WriteToEventViewer("SD.ACMA.DNCRProject.SocialMediaWindowsService has started.");
            timer1.Stop();

            try
            {
                var socialCredentials = _socialMediaHandler.GetSocialMediaCredentials(ConfigurationHelper.Instance.SocialMediaCredentialsURL);
                var twitterAccount = new SocialMediaAccount { Id = 1, MediaId = (int)Enums.Media.Twitter, SocialMediaId = socialCredentials.TwitterDetails.UserId };
                var facebookAccount = new SocialMediaAccount { Id = 2, MediaId = (int)Enums.Media.Facebook, SocialMediaId = socialCredentials.FacebookDetails.UserId };
                var youTubeAccount = new SocialMediaAccount { Id = 3, MediaId = (int)Enums.Media.YouTube, SocialMediaId = socialCredentials.YouTubeDetails.Username };

                try
                {
                    var newTwitterPosts = _socialMediaHandler.GetNewTwitterPosts(twitterAccount,
                        socialCredentials.TwitterDetails);
                    if (newTwitterPosts != null)
                    {
                        WriteToEventViewer(string.Format("New Twitter posts: {0}", newTwitterPosts.Count()));
                        _socialMediaHandler.InsertNewTwitterPosts(newTwitterPosts);
                    }
                }
                catch (Exception ex)
                {
                    WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
                }

                try
                {
                    var newFacebookPosts = _socialMediaHandler.GetNewFacebookPosts(facebookAccount,
                        socialCredentials.FacebookDetails);
                    if (newFacebookPosts != null)
                    {
                        WriteToEventViewer(string.Format("New Facebook posts: {0}", newFacebookPosts.Count));
                        _socialMediaHandler.InsertNewFacebookPosts(newFacebookPosts);
                    }
                }
                catch (Exception ex)
                {
                    WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
                }

                try
                {
                    var newYouTubePosts = _socialMediaHandler.GetNewYouTubeVideos(youTubeAccount,
                        socialCredentials.YouTubeDetails, ConfigurationHelper.Instance.YouTubeMaxRequest);
                    if (newYouTubePosts != null)
                    {
                        WriteToEventViewer(string.Format("New YouTube videos: {0}", newYouTubePosts.Count()));
                        _socialMediaHandler.InserNewYouTubeVideos(newYouTubePosts);
                    }
                }
                catch (Exception ex)
                {
                    WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
                }
            }
            catch (Exception ex)
            {
                WriteToEventViewer(string.Format("ErrorMessage: {0}. Stack Trace: {1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                timer1.Start();
            }

        }

        public void WriteToEventViewer(string message)
        {
            try
            {
                string cs = "SD.ACMA.DNCRProject2.SocialMediaWindowsService";
                EventLog elog = new EventLog();
                if (!EventLog.SourceExists(cs))
                {
                    EventLog.CreateEventSource(cs, cs);
                }
                elog.Source = cs;
                elog.EnableRaisingEvents = true;
                elog.WriteEntry(message);
            }
            catch
            { }
        }
    }
}
