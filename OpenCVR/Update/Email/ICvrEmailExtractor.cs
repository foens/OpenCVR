using System;

namespace OpenCVR.Update.Email
{
    interface ICvrEmailExtractor
    {
        Uri ParseOutZipfileUrl(string emailText);
        string ParseOutLoginId(string emailText);
    }
}
