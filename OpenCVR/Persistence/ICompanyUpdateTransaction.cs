using System;
using OpenCVR.Model;

namespace OpenCVR.Persistence
{
    public interface ICompanyUpdateTransaction : IDisposable
    {
        void Commit();
        void InsertOrReplaceCompany(Company company);
        void DeleteCompany(int vat);
    }
}