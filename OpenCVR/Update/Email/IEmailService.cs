using System;

namespace OpenCVR.Update.Email
{
    internal interface IEmailService
    {
        CvrEmail GetEarliestEmailNotProccessed(DateTime lastItemReceivedTime);
    }
}