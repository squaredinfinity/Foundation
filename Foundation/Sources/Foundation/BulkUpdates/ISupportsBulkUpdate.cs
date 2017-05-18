﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public interface ISupportsBulkUpdate
    {
        IBulkUpdate BeginBulkUpdate();

        bool IsBulkUpdateInProgress();
    }
}
