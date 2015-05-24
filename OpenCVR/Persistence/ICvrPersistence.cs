using System.Collections.Generic;
using OpenCVR.Update.Parse;
using OpenCVR.Update.Parse.Model;

namespace OpenCVR.Persistence
{
    interface ICvrPersistence
    {
        void UpgradeSchemaIfRequired();
        void InsertCompanies(IEnumerable<Company> companies);
    }
}