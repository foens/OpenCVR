using System.Collections.Generic;
using OpenCVR.Update.Parse;

namespace OpenCVR.Persistence
{
    interface ICvrPersistence
    {
        void UpgradeSchemaIfRequired();
        void InsertCompanies(IEnumerable<Company> companies);
    }
}