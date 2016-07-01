using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptPad.Api
{
    public enum FieldType
    {
        Password = 1,
        Username = 2,
        Email = 3,
        AccountNumber = 4,
        CreditCardNumber = 5,
        Numeric = 6,
        Text = 7
    }
}
