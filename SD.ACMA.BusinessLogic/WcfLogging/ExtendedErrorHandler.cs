using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.InterfaceTier;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.BusinessLogic.WcfLogging
{
    public class ExtendedErrorHandler : IErrorHandler
    {
        private ISiteLoggingService _siteLoggingService = new SiteLoggingService(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider());

        public bool HandleError(Exception error)
        {
            try
            {
                _siteLoggingService.Insert(error.Message, null, null);
            }
            catch
            {

            }

            return false;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            
        }
    }
}
