using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD.ACMA.POCO.Consumer
{
    public class Registration
    {
        //public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Numbers { get; set; }
        public List<string> FaxNumbers { get; set; }
        public string ContactNumber { get; set; }
        public string OrganisationName { get; set; }
    }
}
