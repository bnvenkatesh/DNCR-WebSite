using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SD.ACMA.BusinessLogic.SocialMedia;
using SD.ACMA.DatabaseIntermediary;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;

namespace SD.ADMC.DNCRProject.SocialMediaWindowsService
{
    public class Factory
    {
        static public ISocialMediaHandler CreateSocialMediaHandlerInstance()
        {
            IUnityContainer _container = new UnityContainer();

            _container
                .RegisterType(typeof(IRepository), typeof(PetaPocoRepository))
                .RegisterType(typeof(IUnitOfWorkProvider), typeof(PetaPocoUnitOfWorkProvider))
                .RegisterType(typeof(IPostService), typeof(PostService))
                .RegisterType(typeof(ISocialMediaHandler), typeof(SocialMediaHandler));

            ISocialMediaHandler obj = _container.Resolve<ISocialMediaHandler>();
            return obj;
        }
    }
}
