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
        ICompanyUpdateTransaction BeginCompanyUpdateTransaction();
        void InsertOrReplaceCompany(Company c);
        void DeleteCompany(int vatNumber);
        Company FindWithVat(int vatNumber);
        Company Search(string search);
    }
}