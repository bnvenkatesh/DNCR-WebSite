using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SD.ACMA.DNCRProject.Website.Helpers;

namespace SD.ACMA.DNCRProject.Website.Models
{
    public class QuickWashViewModel
    {
        [MinimumElements(1, ".quickWash input.phoneInput", ErrorMessage = "Please enter the phone number you wish to check e.g. 02 1234 5678 or 0412 345 678")]
        public List<WashNumber> Numbers { get; set; }

        [MinimumElements(1, ".quickWash input.phoneInput", ErrorMessage = "Please enter the phone number you wish to check e.g. 02 1234 5678 or 0412 345 678")]
        public List<WashNumber> MinimumNumbers { get; set; }

    }

    public class WashNumber
    {
        [RegularExpression(@"^(([ ()-]*((\+?[ ()-]*61)|(0|1))([ ()-]*[0-9][ ()-]*){9})|([ ()+-]*(0)([ ()-]*[0-9][ ()-]*){2,7})|([ ()+-]*(1)[ ()-]*(0|1|2|4|5|6|7|9)([ ()-]*[0-9][ ()-]*){1,6})|([ ()+-]*(1)[ ()-]*(8)([ ()-]*[0-9][ ()-]*){5}(([ ()-]*[0-9][ ()-]*){3})?)|([ ()+-]*(1)[ ()-]*(3)([ ()-]*[0-9][ ()-]*){4}(([ ()-]*[0-9][ ()-]*){2})?(([ ()-]*[0-9][ ()-]*){2})?))$", ErrorMessage = "The number you have provided is not valid. <br/>Numbers must be 11 digits beginning with 61 or 10 digits or 3 to 8 digits beginning with 0 or 1. <br/>Numbers starting with 18 must be 7 or 10 digits. Numbers starting with 13 must be 6, 8 or 10 digits. <br/>Optional + character, ( ) brackets, - dashes and spaces are allowed. e.g. 612 1234 5678 or 0412 345 678")]
        public string Number { get; set; }

        public bool? Registered { get; set; }
    }
}