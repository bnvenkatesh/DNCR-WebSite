using SD.ACMA.DatabaseIntermediary;
using SD.ACMA.InterfaceTier;
using SD.Core.Data.Repository.PetaPoco.Business;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SD.ACMA.BusinessLogic.WcfLogging
{
    public class ExtendedClientMessageInspector : IClientMessageInspector
    {
        private ISiteLoggingService _siteLoggingService = new SiteLoggingService(new PetaPocoRepository(), new PetaPocoUnitOfWorkProvider());
        private long logResponse;

        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {            
            string replyString = reply.ToString();

            if (!string.IsNullOrEmpty(replyString) && replyString.TrimStart().IndexOf('<') == 0)
            {
                _siteLoggingService.InsertXML(replyString, null, logResponse);
            }
            else
            {
                _siteLoggingService.Insert(replyString, null, logResponse);
            }
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            string requestString = request.ToString();

            if (!string.IsNullOrEmpty(requestString) && requestString.TrimStart().IndexOf('<') == 0)
            {
                logResponse = _siteLoggingService.InsertXML(requestString, null, null);
            } else
            {
                logResponse = _siteLoggingService.Insert(requestString, null, null);
            }
            
            return null;
        }

        #endregion
    }
}
