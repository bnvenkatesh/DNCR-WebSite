using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.InterfaceTier;
using SD.ACMA.POCO.PetaPoco;
using SD.Core.Data.Repository.PetaPoco.Business.Interface;
using SD.Core.Data.Repository.PetaPoco.UnityOfWorkContainer;
using System.Xml;

namespace SD.ACMA.DatabaseIntermediary
{
    public class SiteLoggingService : ISiteLoggingService
    {
        private IRepository _repository;
        private IUnitOfWorkProvider _unitOfWorkProvider;

        public SiteLoggingService(IRepository repository, IUnitOfWorkProvider unitOfWorkProvider)
        {
            _repository = repository;
            _unitOfWorkProvider = unitOfWorkProvider;
        }

        public int Insert(string message, int? userID, Int64? correspondingID)
        {
            var newSiteLogginObject = new SiteLogging { 
                LoggedOn = DateTime.Now,
                Message = message
            };

            if (userID != null)
                newSiteLogginObject.UserID = userID.Value;

            if (correspondingID != null)
                newSiteLogginObject.CorrespondingID = correspondingID.Value;

            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                var result = _repository.Insert(uow, newSiteLogginObject);
                uow.Commit();

                return result;
            }
        }

        public int InsertXML(string messageXML, int? userID, Int64? correspondingID)
        {
            var newSiteLogginObject = new SiteLogging
            {
                LoggedOn = DateTime.Now,
                MessageXML = messageXML.Replace("&", "&amp;")
            };

            if (userID != null)
                newSiteLogginObject.UserID = userID.Value;

            if (correspondingID != null)
                newSiteLogginObject.CorrespondingID = correspondingID.Value;

            using (var uow = _unitOfWorkProvider.GetUnitOfWork())
            {
                var result = _repository.Insert(uow, newSiteLogginObject);
                uow.Commit();

                return result;
            }
        }       
    }
}
