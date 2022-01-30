﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class SubscriptionSummaryDetailsResponse : BaseWebServiceResponse
    {
        public string ResultMessage { get; set; }
        public SubscriptionSummaryModel SubscriptionSummary { get; set; }
    }
}
