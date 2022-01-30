using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.InterfaceTier;
using SD.ACMA.BusinessLogic.Helpers;
using SD.ACMA.DatabaseIntermediary;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.Business;

namespace SD.ACMA.Service.EmailInParser
{
    public class Factory
    {
        static public IConsumerDataInterchange CreateConsumerDataInterchangeInstance()
        {
            IUnityContainer _container = new UnityContainer();

            _container
                .RegisterType(typeof(IRepository), typeof(PetaPocoRepository))
                .RegisterType(typeof(IUnitOfWorkProvider), typeof(PetaPocoUnitOfWorkProvider))
                .RegisterType(typeof(ISiteLoggingService), typeof(SiteLoggingService))
                .RegisterType(typeof(IFileHelper), typeof(FileHelper))
                .RegisterType(typeof(IErrorMessageHelper), typeof(ErrorMessageHelper))                
                .RegisterType(typeof(IConsumerDataInterchange), typeof(DNCRConsumerWebServiceWrapper));

            IConsumerDataInterchange obj = _container.Resolve<IConsumerDataInterchange>();
            return obj;
        }

           
    }
}
