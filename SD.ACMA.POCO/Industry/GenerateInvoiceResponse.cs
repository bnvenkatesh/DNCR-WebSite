﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SD.ACMA.POCO.Base;

namespace SD.ACMA.POCO.Industry
{
    public class GenerateInvoiceResponse : BaseWebServiceResponse
    {
        public byte[] InvoiceStream { get; set; }
    }
}
