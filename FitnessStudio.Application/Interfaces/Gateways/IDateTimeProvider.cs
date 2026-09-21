using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessStudio.Application.Interfaces.Gateways
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateUTC();       
    }
}
