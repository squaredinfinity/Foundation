using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Presentation.Controls
{
    public interface IUserActionParameters
    {
        T GetParameterValue<T>(string key, Func<T> defaultValue);
    }
}
