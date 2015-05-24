﻿using System;
using System.Collections.Generic;
using OpenCVR.Model;

namespace OpenCVR.Persistence
{
    interface ICvrPersistence
    {
        void UpgradeSchemaIfRequired();
        void InsertCompanies(IEnumerable<Company> companies);
        DateTime GetLastProcessedEmailReceivedTime();
        void SetLastProcessedEmailReceivedTime(DateTime updateTime);
    }
}