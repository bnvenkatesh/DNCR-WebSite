using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace SD.ACMA.DNCRProject.Website.Handlers
{
    public class EventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentLastChanceFinderResolver.Current.SetFinder(new FourOFourFinder());

            ContentService.Created += ContentCreated;
            ContentService.Saving += ContentSaving;
            ContentService.SentToPublish += ContentSentToPublish;
            ContentService.Published += ContentPublished;
            ContentService.UnPublishing += ContentUnpublishing;
            ContentService.Copying += ContentCopying;

            //UserService.SavingUser += UserSaving;

            Umbraco.Web.Trees.ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
        }

        /*private void UserSaving(IUserService sender, SaveEventArgs<IUser> e)
        {
            var oldUser = umbraco.BusinessLogic.User.getAll().FirstOrDefault(x => x.Id == e.SavedEntities.First().Id);
            var newUser = e.SavedEntities.FirstOrDefault();
        }*/

        void ContentTreeController_MenuRendering(Umbraco.Web.Trees.TreeControllerBase sender, Umbraco.Web.Trees.MenuRenderingEventArgs e)
        {
            //Remove Publish and Send to Publish menu because it bypasses the publishing workflow
            if (e.Menu.Items.Any(x => x.Alias == "publish"))
            {
                e.Menu.Items.Remove(e.Menu.Items.FirstOrDefault(x => x.Alias == "publish"));
            }
            if (e.Menu.Items.Any(x => x.Alias == "sendtopublish"))
            {
                e.Menu.Items.Remove(e.Menu.Items.FirstOrDefault(x => x.Alias == "sendtopublish"));
            }
        }

        private void ContentCopying(IContentService sender, CopyEventArgs<IContent> args)
        {
            //Update content status to Edited
            args.Copy.SetValue("status", "Edited");
        }

        private void ContentCreated(IContentService sender, NewEventArgs<IContent> args)
        {
            //Update content status to Edited
            args.Entity.SetValue("status", "Edited");

            //Set showInNav ticked by default
            if (args.Entity.Properties.Any(x => x.Alias == "showInNav"))
            {
                args.Entity.SetValue("showInNav", 1);
            }
        }

        private void ContentSaving(IContentService sender, SaveEventArgs<IContent> args)
        {
            var status = args.SavedEntities.First().GetValue("status");
            var department = args.SavedEntities.First().GetValue("currentDepartment");
            var currentUser = UmbracoContext.Current.Security.CurrentUser;

            if (currentUser != null)
            {
                //If status is sent for publishing, no content change will be saved
                if (status != null && status.ToString() == "Sent for publishing")
                {
                    var contentService = new ContentService();
                    var oldContent = contentService.GetById(args.SavedEntities.First().Id);
                    args.SavedEntities.First().Properties = oldContent.Properties;
                }
                
                //If user is an author/publisher and status is null or not sent for publishing, update content status and properties
                if (currentUser.UserType.Alias.Contains("Author") && (status == null || status.ToString() != "Sent for publishing"))
                {
                    var departmentName = UmbracoContext.Current.Security.CurrentUser.UserType.Name.Split(' ')[0];
                    args.SavedEntities.First().SetValue("status", "Edited");
                    args.SavedEntities.First().SetValue("currentDepartment", departmentName);
                    args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                    args.SavedEntities.First().SetValue("rejectContent", 0);
                }
                //If user is a secondary approver
                else if (currentUser.UserType.Alias.Contains("SecondaryApprover"))
                {
                    var rejectContent = args.SavedEntities.First().GetValue("rejectContent") != null && args.SavedEntities.First().GetValue("rejectContent").ToString() == "1";
                    //If content is rejected, update content status and send email notification to the contentOwner
                    if (rejectContent)
                    {
                        var contentOwner = args.SavedEntities.First().GetValue("contentOwner");
                        if (contentOwner != null)
                        {
                            var owner = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.Name == contentOwner.ToString());
                            if (owner.Any())
                            {
                                args.SavedEntities.First().SetValue("status", "Rejected");
                                args.SavedEntities.First().SetValue("rejectContent", 0);
                                ApplicationContext.Current.Services.ContentService.Save(args.SavedEntities.First(), currentUser.Id, false);
                                var currentOwner = owner.FirstOrDefault();
                                SendEmail(currentOwner.Email, currentOwner.Name, currentUser.Email, currentUser.Name, EmailType.Reject, args.SavedEntities.First());
                            }
                        }
                        args.Cancel = true;
                    }
                    else
                    {
                        //If content is approved and auto-publish is not set
                        if (args.SavedEntities.First().ReleaseDate == null)
                        {
                            var overrideDepartment = args.SavedEntities.First().GetValue("overrideDepartment");
                            //If department is null and override department is set, update content status and properties and send email notification to the team's author/publishers
                            //The reason why the email is sent here instead of in SentToPublish is because Umbraco UI calls save and publish instead of save and send to publish, so it will not call SentToPublish method
                            if (department == null && overrideDepartment != null)
                            {
                                department = umbraco.library.GetPreValueAsString(Int32.Parse(overrideDepartment.ToString()));
                                args.SavedEntities.First().SetValue("overrideDepartment", null);
                                args.SavedEntities.First().SetValue("currentDepartment", department);
                                args.SavedEntities.First().SetValue("status", "Sent for publishing");
                                args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                                ApplicationContext.Current.Services.ContentService.Save(args.SavedEntities.First(), currentUser.Id, false);
                                var publishers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == String.Format("{0}Authors", department));
                                foreach (var publisher in publishers)
                                {
                                    SendEmail(publisher.Email, publisher.Name, currentUser.Email, currentUser.Name, EmailType.Publish, args.SavedEntities.First());
                                }
                                args.Cancel = true;
                            }
                            else
                            {
                                //If the secondary approver starts the content change, update content status and contentOwner
                                var contentOwner = args.SavedEntities.First().GetValue("contentOwner");
                                if (contentOwner == null)
                                {
                                    args.SavedEntities.First().SetValue("status", "Edited");
                                    args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                                }
                            }
                        }
                        else
                        {
                            args.SavedEntities.First().SetValue("overrideDepartment", null);
                            var contentOwner = args.SavedEntities.First().GetValue("contentOwner");
                            if (contentOwner == null)
                            {
                                args.SavedEntities.First().SetValue("status", "Edited");
                                args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                            }
                        }
                    }
                }
                //If user is an approver
                else if (currentUser.UserType.Alias.Contains("Approver"))
                {
                    var containsSensitiveInformation = args.SavedEntities.First().GetValue("containsSensitiveInformation") != null && args.SavedEntities.First().GetValue("containsSensitiveInformation").ToString() == "1";
                    var rejectContent = args.SavedEntities.First().GetValue("rejectContent") != null && args.SavedEntities.First().GetValue("rejectContent").ToString() == "1";
                    //If content is rejected, update content status and send email notification to the contentOwner
                    if (rejectContent)
                    {
                        var contentOwner = args.SavedEntities.First().GetValue("contentOwner");
                        if (contentOwner != null)
                        {
                            var owner = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.Name == contentOwner.ToString());
                            if (owner.Any())
                            {
                                args.SavedEntities.First().SetValue("status", "Rejected");
                                ApplicationContext.Current.Services.ContentService.Save(args.SavedEntities.First(), currentUser.Id, false);
                                var currentOwner = owner.FirstOrDefault();
                                SendEmail(currentOwner.Email, currentOwner.Name, currentUser.Email, currentUser.Name, EmailType.Reject, args.SavedEntities.First());
                            }
                        }
                        args.Cancel = true;
                    }
                    //If content contains sensitive information, update content status and properties and send email notification to secondary approvers
                    else if (containsSensitiveInformation)
                    {
                        if (args.SavedEntities.First().GetValue("contentOwner") == null)
                        {
                            args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                            var departmentName = UmbracoContext.Current.Security.CurrentUser.UserType.Name.Split(' ')[0];
                            args.SavedEntities.First().SetValue("currentDepartment", departmentName);
                        }
                        if (department == null)
                        {
                            var departmentName = UmbracoContext.Current.Security.CurrentUser.UserType.Name.Split(' ')[0];
                            args.SavedEntities.First().SetValue("currentDepartment", departmentName);
                        }

                        ApplicationContext.Current.Services.ContentService.Save(args.SavedEntities.First(), currentUser.Id, false);

                        var secondaryApprovers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == "SecondaryApprovers"); 
                        foreach (var sapprover in secondaryApprovers)
                        {
                            SendEmail(sapprover.Email, sapprover.Name, currentUser.Email, currentUser.Name, EmailType.SecondApproval, args.SavedEntities.First());
                        }
                        args.SavedEntities.First().SetValue("status", "Sent for second approval");

                        ApplicationContext.Current.Services.ContentService.Save(args.SavedEntities.First(), currentUser.Id, false);

                        args.Cancel = true;
                    }
                    //If the approver starts the content change, update content status and properties
                    else if (status == null || status.ToString() == "Published")
                    {
                        var departmentName = UmbracoContext.Current.Security.CurrentUser.UserType.Name.Split(' ')[0];
                        args.SavedEntities.First().SetValue("status", "Edited");
                        args.SavedEntities.First().SetValue("currentDepartment", departmentName);
                        args.SavedEntities.First().SetValue("contentOwner", currentUser.Name);
                    }
                }
            }
        }

        private void ContentSentToPublish(IContentService sender, SendToPublishEventArgs<IContent> args)
        {
            var currentUser = UmbracoContext.Current.Security.CurrentUser;
            //If user is an author/publisher
            if (currentUser.UserType.Alias.Contains("Authors"))
            {
                //If content status is sent for publishing, publish content
                if (args.Entity.GetValue("status").ToString() == "Sent for publishing")
                {
                    ApplicationContext.Current.Services.ContentService.SaveAndPublishWithStatus(args.Entity, currentUser != null ? currentUser.Id : 0, false);
                }
                //if content status is everything else, update content status and send email notification to approvers
                else
                {
                    args.Entity.SetValue("status", "Sent for approval");
                    ApplicationContext.Current.Services.ContentService.Save(args.Entity, currentUser.Id, false);
                    var approvers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == currentUser.UserType.Alias.Replace("Authors", "Approvers"));
                    foreach (var approver in approvers)
                    {
                        SendEmail(approver.Email, approver.Name, currentUser.Email, currentUser.Name, EmailType.Approval, args.Entity);
                    }
                }
            }
            //If user is a secondary approver, update content status and properties and send email notification to publishers
            else if (currentUser.UserType.Alias.Contains("SecondaryApprovers"))
            {
                var department = args.Entity.GetValue("currentDepartment");
                var overrideDepartment = args.Entity.GetValue("overrideDepartment");
                if (overrideDepartment != null)
                {
                    department = umbraco.library.GetPreValueAsString(Int32.Parse(overrideDepartment.ToString()));
                }
                args.Entity.SetValue("overrideDepartment", null);
                args.Entity.SetValue("currentDepartment", department);
                args.Entity.SetValue("containsSensitiveInformation", 0);
                args.Entity.SetValue("status", "Sent for publishing");
                ApplicationContext.Current.Services.ContentService.Save(args.Entity, currentUser.Id, false);
                var publishers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == String.Format("{0}Authors", department));
                foreach (var publisher in publishers)
                {
                    SendEmail(publisher.Email, publisher.Name, currentUser.Email, currentUser.Name, EmailType.Publish, args.Entity);
                }
            }
            //If user is an approver, update content status and send email notification to publishers
            else if (currentUser.UserType.Alias.Contains("Approvers"))
            {
                args.Entity.SetValue("status", "Sent for publishing");
                ApplicationContext.Current.Services.ContentService.Save(args.Entity, currentUser.Id, false);
                var department = args.Entity.GetValue("currentDepartment");
                IEnumerable<umbraco.BusinessLogic.User> publishers;
                publishers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == String.Format("{0}Authors", department));
                if (department != null)
                {
                    publishers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == String.Format("{0}Authors", department));
                }
                else
                {
                    publishers = umbraco.BusinessLogic.User.getAll().Where(x => !x.Disabled && x.UserType.Alias == currentUser.UserType.Alias.Replace("Approvers", "Authors"));
                }
                foreach (var publisher in publishers)
                {
                    SendEmail(publisher.Email, publisher.Name, currentUser.Email, currentUser.Name, EmailType.Publish, args.Entity);
                }
            }
        }

        private void ContentPublished(IPublishingStrategy sender, PublishEventArgs<IContent> args)
        {
            //Update content status and properties and then save and publish
            var currentUser = UmbracoContext.Current.Security.CurrentUser;
            foreach (var entity in args.PublishedEntities)
            {
                var status = entity.GetValue("status");
                if (status == null || status.ToString() != "Published")
                {
                    entity.SetValue("status", "Published");
                    entity.SetValue("contentOwner", null);
                    entity.SetValue("containsSensitiveInformation", 0);
                    entity.SetValue("currentDepartment", null);
                    entity.SetValue("rejectContent", 0);
                    ApplicationContext.Current.Services.ContentService.SaveAndPublishWithStatus(entity, currentUser != null ? currentUser.Id : 0, false);
                }
            }
        }

        private void ContentUnpublishing(IPublishingStrategy sender, PublishEventArgs<IContent> args)
        {
            //update content status and then save then unpublish
            var currentUser = UmbracoContext.Current.Security.CurrentUser;
            foreach (var entity in args.PublishedEntities)
            {
                var status = entity.GetValue("status");
                if (status == null || status.ToString() != "Unpublished")
                {
                    entity.SetValue("status", "Unpublished");
                    ApplicationContext.Current.Services.ContentService.Save(entity, currentUser != null ? currentUser.Id : 0, false);
                    ApplicationContext.Current.Services.ContentService.UnPublish(entity, currentUser != null ? currentUser.Id : 0);
                }
            }
        }

        private void SendEmail(string toEmail, string toName, string fromEmail, string fromName, EmailType emailType, IContent content)
        {
            var host = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
            var contentUrl = host + "/umbraco/#/content/content/edit/" + content.Id;
            var mail = new MailMessage(fromEmail, toEmail);
            var client = new SmtpClient();
            mail.IsBodyHtml = true;
            if (emailType == EmailType.Approval)
            {
                mail.Subject = String.Format("Request for approval for content '{0}'", content.Name);
                mail.Body = String.Format(
                        "Hi {0},<br/><br/>{1} has requested approval for the content '{2}'.<br/><br/>Please click <a href='{3}'>here</a> to see the content and do further actions.<br/>",
                        toName, fromName, content.Name, contentUrl);
            }
            if (emailType == EmailType.SecondApproval)
            {
                mail.Subject = String.Format("Request for second approval for content '{0}'", content.Name);
                mail.Body = String.Format(
                        "Hi {0},<br/><br/>{1} has flagged the content '{2}' for containing sensitive information and requesting further approval.<br/><br/>Please click <a href='{3}'>here</a> to see the content and do further actions.<br/>",
                        toName, fromName, content.Name, contentUrl);
            }
            if (emailType == EmailType.Publish)
            {
                mail.Subject = String.Format("Request for publish for content '{0}'", content.Name);
                mail.Body = String.Format(
                        "Hi {0},<br/><br/>{1} has requested the content '{2}' to be published.<br/><br/>Please click <a href='{3}'>here</a> to see the content and do further actions.<br/>", 
                        toName, fromName, content.Name, contentUrl);
            }
            if (emailType == EmailType.Reject)
            {
                mail.Subject = String.Format("Approval for content '{0}' has been declined", content.Name);
                mail.Body = String.Format(
                        "Hi {0},<br/><br/>{1} has rejected the approval for the content '{2}'.<br/><br/>Please click <a href='{3}'>here</a> to see the content and do further actions.<br/>",
                        toName, fromName, content.Name, contentUrl);
            }
            client.Send(mail);
        }
    }

    public enum EmailType
    {
        Approval,
        SecondApproval,
        Publish,
        Reject
    }
}