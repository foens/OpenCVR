using System;
using System.Net;

namespace OpenCVR.Update.Http
{
    internal interface IHttpService
    {
        /// <returns>The output file path</returns>
        string Download(Uri url, ICredentials authenticationCredentials);
    }
}