using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Controls.Infrastructure
{
    public class UserActionParameters : Dictionary<string, object>, IUserActionParameters
    {
        public T GetParameterValue<T>(string key, Func<T> defaultValue)
        {
            var _o = (object)null;

            if (this.TryGetValue(key, out _o))
            {
                return (T)_o;
            }
            else
            {
                return defaultValue();
            }
        }

        public static IUserActionParameters FromDictionary(IDictionary<string, object> parameters)
        {
            var uai = new UserActionParameters();

            foreach (var kvp in parameters)
            {
                uai.Add(kvp.Key, kvp.Value);
            }

            return uai;
        }
    }
}
