using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SD.ACMA.InterfaceTier;

namespace SD.ACMA.DNCRProject.Website.Helpers
{
    public class WebServiceResultFormatter : IWebServiceResultFormatter
    {
        public List<string> GenerateResultMessages(params string[] messages)
        {
            var messagesList = new List<string>();

            foreach (var item in messages)
            {
                messagesList.Add(item);
            }

            return messagesList;
        }
    }
}