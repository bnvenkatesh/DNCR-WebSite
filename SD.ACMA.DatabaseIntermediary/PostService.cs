using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO;
using SD.ACMA.POCO.PetaPoco;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;

namespace SD.ACMA.DatabaseIntermediary
{
    public interface IPostService
    {
        Post GetLatestPost(int mediaId, string socialAccountId);
        int InsertPost(Post newPost);
        int InsertAttachment(Attachment attachment);
        List<Post> Get10LatestPosts(int mediaId);
        List<Post> GetLatestPostsFromEach(int numberOfPosts, int twitterMediaId, int facebookMediaId, int youTubeMediaId);
        List<Post> SearchPosts(int twitterNumberOfPosts, int twitterMediaId, string twitterSearchTerm, int facebookNumberOfPosts, int facebookMediaId, string facebookSearchTerm,
            int youTubeNumberOfPosts, int youTubeMediaId, string youTubeSearchTerm);
    }

    public class PostService : IPostService
    {
        private IRepository _repository;
        private IUnitOfWorkProvider _unitOfWorkProvider;

        public PostService(IRepository repository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _repository = repository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public Post GetLatestPost(int mediaId, string socialAccountId)
        {
            if (socialAccountId == null)
            {
                return _repository.Fetch<Post>("SELECT TOP 1 * FROM Post NOLOCK WHERE MediaId =@0 " +
                                    " AND userId IS NULL " +
                                    "ORDER BY CreatedOn DESC", mediaId).FirstOrDefault();
            }
            else
            {
                return _repository.Fetch<Post>("SELECT TOP 1 * FROM Post NOLOCK WHERE MediaId =@0 " +
                                  " AND userId = @1 " +
                                  "ORDER BY CreatedOn DESC", mediaId, socialAccountId).FirstOrDefault();
            }
        }

        public int InsertPost(Post newPost)
        {
            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                newPost.InsertedOn = DateTime.Now;
                var result = _repository.Insert(uow, newPost);
                uow.Commit();

                return result;
            }
        }

        public int InsertAttachment(Attachment attachment)
        {
            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                var result = _repository.Insert(uow, attachment);
                uow.Commit();
                return result;
            }
        }

        public List<Post> Get10LatestPosts(int mediaId)
        {
            return _repository.FetchOneToMany<Post, Attachment>(
                x => x.Id, "SELECT TOP 10 * FROM Post LEFT JOIN Attachment ON Post.Id = Attachment.PostId WHERE MediaId = @0 ORDER BY CreatedOn DESC", mediaId);
        }

        public List<Post> GetLatestPostsFromEach(int numberOfPosts, int twitterMediaId, int facebookMediaId, int youTubeMediaId)
        {
            return _repository.FetchOneToMany<Post, Attachment>(
                x => x.Id, string.Format("SELECT TOP {0} * FROM Post LEFT JOIN Attachment ON Post.Id = Attachment.PostId WHERE MediaId = @0 UNION SELECT TOP {0} * FROM Post LEFT JOIN Attachment ON Post.Id = Attachment.PostId WHERE MediaId = @1 UNION SELECT TOP {0} * FROM Post LEFT JOIN Attachment ON Post.Id = Attachment.PostId WHERE MediaId = @2 ORDER BY CreatedOn DESC", numberOfPosts),
                twitterMediaId, facebookMediaId, youTubeMediaId);
        }

        public List<Post> SearchPosts(int twitterNumberOfPosts, int twitterMediaId, string twitterSearchTerm, int facebookNumberOfPosts, int facebookMediaId, string facebookSearchTerm, 
            int youTubeNumberOfPosts, int youTubeMediaId, string youTubeSearchTerm)
        {
            List<object> parameterList = new List<object>();
            StringBuilder sb = new StringBuilder();

            var dtoObject = new SocialMediaDTO
            {
                ParameterList = parameterList,
                SQLQuery = sb,
                NumberOfPosts = twitterNumberOfPosts,
                MediaId = twitterMediaId,
            };

            CreateSelectTerm(ref dtoObject);

            if(!string.IsNullOrEmpty(twitterSearchTerm))
            {
                dtoObject.SearchTerm = twitterSearchTerm;
                CreateSearchTerm(ref dtoObject);
            }

            object[] parameterArray = new object[dtoObject.Counter];
            for (int i = 0; i < dtoObject.Counter; i++)
            {
                parameterArray[i] = dtoObject.ParameterList.ElementAt(i);
            }
            sb.Append(" ORDER BY CreatedOn DESC");

            var twitterResult = _repository.FetchOneToMany<Post, Attachment>(x => x.Id, sb.ToString(), parameterArray);

            sb.Clear();
            dtoObject = new SocialMediaDTO
            {
                SQLQuery = sb,
                ParameterList = new List<object>(),
                MediaId = facebookMediaId,
                NumberOfPosts = facebookNumberOfPosts
            };

            CreateSelectTerm(ref dtoObject);

            if (!string.IsNullOrEmpty(facebookSearchTerm))
            {
                dtoObject.SearchTerm = facebookSearchTerm;
                CreateSearchTerm(ref dtoObject);
            }

            parameterArray = new object[dtoObject.Counter];
            for (int i = 0; i < dtoObject.Counter; i++)
            {
                parameterArray[i] = dtoObject.ParameterList.ElementAt(i);
            }
            sb.Append(" ORDER BY CreatedOn DESC");

            var facebookResult = _repository.FetchOneToMany<Post, Attachment>(x => x.Id, sb.ToString(), parameterArray);
            sb.Clear();
            dtoObject = new SocialMediaDTO
            {
                SQLQuery = sb,
                ParameterList = new List<object>(),
                MediaId = youTubeMediaId,
                NumberOfPosts = youTubeNumberOfPosts
            };

            CreateSelectTerm(ref dtoObject);
            if (!string.IsNullOrEmpty(youTubeSearchTerm))
            {
                dtoObject.SearchTerm = youTubeSearchTerm;
                CreateSearchTerm(ref dtoObject);
            }

            sb = dtoObject.SQLQuery;

            parameterArray = new object[dtoObject.Counter];
            for (int i = 0; i < dtoObject.Counter; i++)
            {
                parameterArray[i] = dtoObject.ParameterList.ElementAt(i);
            }
            sb.Append(" ORDER BY CreatedOn DESC");

            var youTubeResult = _repository.FetchOneToMany<Post, Attachment>(x => x.Id, sb.ToString(), parameterArray);

            var overallResult = new List<Post>();

            foreach (var item in twitterResult)
            {
                overallResult.Add(item);
            }

            foreach (var item in facebookResult)
            {
                overallResult.Add(item);
            }

            foreach (var item in youTubeResult)
            {
                overallResult.Add(item);
            }

            return overallResult;
        }

        private List<string> SplitSearchTerm(string originalSearchTerm, string separator)
        {
            return originalSearchTerm.Split(new [] { separator }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void CreateSearchTerm(ref SocialMediaDTO dto)
        {
            var result = SplitSearchTerm(dto.SearchTerm, ",");
            dto.SQLQuery.Append(" AND (");

            foreach (var item in result)
            {
                dto.SQLQuery.Append(string.Format("[Text] LIKE '%' + @{0} + '%'", dto.Counter));
                dto.ParameterList.Add(item.Trim());
                dto.Counter++;
                dto.SQLQuery.Append(string.Format(" OR [Title] LIKE '%' + @{0} + '%' OR", dto.Counter));
                dto.ParameterList.Add(item.Trim());
                dto.Counter++;
            }

            dto.SQLQuery.Remove(dto.SQLQuery.Length - 2, 2);

            dto.SQLQuery.Append(")");
        }

        private void CreateSelectTerm(ref SocialMediaDTO dto)
        {
            dto.SQLQuery.Append(string.Format("SELECT TOP {0} * FROM Post LEFT JOIN Attachment ON Post.Id = Attachment.PostId WHERE MediaId = @{1}", dto.NumberOfPosts, dto.Counter));
            dto.ParameterList.Add(dto.MediaId);
            dto.Counter++;
        }

        private void CreateSubQuery(ref SocialMediaDTO dto)
        {
            dto.SQLQuery.Append(string.Format(" AND Post.Id IN (SELECT TOP {0} Id FROM Post WHERE MediaId = @{1} ORDER BY CreatedOn DESC)", dto.NumberOfPosts, dto.Counter));
            dto.ParameterList.Add(dto.MediaId);
            dto.Counter++;
        }

    }
}
