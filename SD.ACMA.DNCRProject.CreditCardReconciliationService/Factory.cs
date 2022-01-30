using Microsoft.Practices.Unity;
using SD.ACMA.BusinessLogic.Avanade;
using SD.ACMA.BusinessLogic.PaymentGateway;
using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.InterfaceTier;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.DNCRProject.CreditCardReconciliationService
{
    public class Factory
    {
        public IUnityContainer TaskContainer1;
        public IUnityContainer TaskContainer2;

        public Factory()
        {
            TaskContainer1 = new UnityContainer();
            TaskContainer2 = new UnityContainer();
        }

        private static Factory _instance;
        public static Factory Instance
        {
            get
            {
                lock (typeof(Factory))
                {
                    if (_instance == null)
                        _instance = new Factory();

                    return _instance;
                }
            }
        }

        public void RegisterContainers()
        {
            TaskContainer1
                .RegisterType<IRepository, PetaPocoRepository>()
                .RegisterType<IUnitOfWorkProvider, PetaPocoUnitOfWorkProvider>()
                .RegisterType<ICreditCardPaymentDataRepository, CreditCardPaymentDataRepository>()
                .RegisterType<ICreditCardPaymentService, CreditCardPaymentService>()
                .RegisterType<IPaymentGatewayService, PaymentGatewayService>();

            TaskContainer2
                .RegisterType<IRepository, PetaPocoRepository>()
                .RegisterType<IUnitOfWorkProvider, PetaPocoUnitOfWorkProvider>()
                .RegisterType<ICreditCardPaymentDataRepository, CreditCardPaymentDataRepository>()
                .RegisterType<ICreditCardPaymentService, CreditCardPaymentService>()
                .RegisterType<IPaymentGatewayService, PaymentGatewayService>()
                .RegisterType<ISiteLoggingService, SiteLoggingService>()
                .RegisterType<IIndustryDataInterchange, DNCRIndustryWebServiceWrapper>();

            //TaskContainer1.RegisterType<ICreditCardPaymentService, CreditCardPaymentService>(new InjectionFactory(c =>
            //    {
            //        var creditCardPaymentDataRepository = new CreditCardPaymentDataRepository(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider());

            //        return new CreditCardPaymentService(creditCardPaymentDataRepository);
            //    }))
            //    .RegisterType<IPaymentGatewayService, PaymentGatewayService>();

            //TaskContainer2.RegisterType<ICreditCardPaymentService, CreditCardPaymentService>(new InjectionFactory(c =>
            //    {
            //        var creditCardPaymentDataRepository = new CreditCardPaymentDataRepository(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider());

            //        return new CreditCardPaymentService(creditCardPaymentDataRepository);
            //    }))
            //    .RegisterType<IPaymentGatewayService, PaymentGatewayService>();
        }
    }
}
