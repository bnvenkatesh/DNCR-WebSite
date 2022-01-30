using System;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using SD.ACMA.BusinessLogic;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.BusinessLogic.Helpers;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.DNCRProject.Website.Helpers;
using SD.ACMA.InterfaceTier;
using SD.Core.Data.Repository.PetaPoco.Business;
using Umbraco.Core;
using Umbraco.Core.Persistence.Repositories;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Web;
using Unity.Mvc4;

namespace SD.ACMA.DNCRProject.Website
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<UmbracoContext>(new ContainerControlledLifetimeManager(), new InjectionFactory(c => UmbracoContext.Current));
            container.RegisterType<SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer.IUnitOfWorkProvider, SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer.PetaPocoUnitOfWorkProvider>();
            container.RegisterType<SD.Core.Data.Repository.PetaPoco.Business.Interface.IRepository, SD.Core.Data.Repository.PetaPoco.Business.PetaPocoRepository>();
            container.RegisterType<IPostService, PostService>();
            container.RegisterType<ISocialMediaHelper, SocialMediaHelper>();
            container.RegisterType<IConsumerDataInterchange, DNCRConsumerWebServiceWrapper>();
            container.RegisterType<IIndustryDataInterchange, DNCRIndustryWebServiceWrapper>();
            container.RegisterType<IErrorMessageHelper, ErrorMessageHelper>();
            container.RegisterType<IWebServiceResultFormatter, WebServiceResultFormatter>();
            container.RegisterType<ISiteLoggingService, SiteLoggingService>();
            container.RegisterType<IUserContextHelper, UserContextHelper>();
            container.RegisterType<ICreditCardPaymentDataRepository, CreditCardPaymentDataRepository>();
            container.RegisterType<ICreditCardPaymentService, CreditCardPaymentService>();
            container.RegisterType<IPaymentGatewayService, PaymentGatewayService>();
            container.RegisterType<IFileHelper, FileHelper>();

            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            
        }
    }
}