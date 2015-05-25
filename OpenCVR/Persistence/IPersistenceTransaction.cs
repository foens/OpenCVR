using System;

namespace OpenCVR.Persistence
{
    public interface IPersistenceTransaction : IDisposable
    {
        void Commit();
    }
}