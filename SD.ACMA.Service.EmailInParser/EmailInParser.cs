using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using SD.ACMA.InterfaceTier;

namespace SD.ACMA.Service.EmailInParser
{
    public partial class EmailInParser : ServiceBase
    {
        private EmailInParserService _service;

        public EmailInParser()
        {
            InitializeComponent();

            this._service = new EmailInParserService(this);
        }

        protected override void OnStart(string[] args)
        {
           
            this._service.StartAsync();
        }

        protected override void OnStop()
        {
            this._service.OnStop();
        }

        public void OnDebug()
        {
            this._service.StartAsync();
        }
    }
}
