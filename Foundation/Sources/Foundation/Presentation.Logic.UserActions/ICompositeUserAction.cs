using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public interface ICompositeUserAction
    {
        IReadOnlyList<IUserAction> Children { get; }
    }

}
