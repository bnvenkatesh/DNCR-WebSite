using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.BusinessLogic.WcfLogging
{
    public class CustomBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ExtendedServiceBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(ExtendedServiceBehavior);
            }
        }
    }

    public class ExtendedServiceBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members
        public void AddBindingParameters(
          ServiceEndpoint endpoint, BindingParameterCollection bindingParameters
        ) { return; }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new ExtendedClientMessageInspector());

            //foreach (ClientOperation op in clientRuntime.Operations)
            //    op.ParameterInspectors.Add(new ExtendedClientMessageInspector());

            IErrorHandler errorHandler = new ExtendedErrorHandler();             
            clientRuntime.CallbackDispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(errorHandler);
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new ExtendedClientMessageInspector());
            //foreach (DispatchOperation op in endpointDispatcher.DispatchRuntime.Operations)
            //    op.ParameterInspectors.Add(new ExtendedClientMessageInspector());            
        }

        public void Validate(ServiceEndpoint endpoint) { return; }

        #endregion
    }
}
