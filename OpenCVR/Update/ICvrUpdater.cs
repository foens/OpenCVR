namespace OpenCVR.Update
{
    interface ICvrUpdater
    {
        /// <summary>
        /// Returns true if an update was available and applied
        /// </summary>
        /// <returns></returns>
        bool TryUpdate();
    }
}