using System;
using System.Collections.Generic;
using OpenCVR.Model;

namespace OpenCVR.Persistence
{
    interface ICvrPersistence
    {
        void UpgradeSchemaIfRequired();
        DateTime GetLastProcessedEmailReceivedTime();
        void SetLastProcessedEmailReceivedTime(DateTime updateTime);
        IPersistenceTransaction StartTransaction();
        void InsertOrReplaceCompany(Company c);
        void DeleteCompany(int vatNumber);
    }
}