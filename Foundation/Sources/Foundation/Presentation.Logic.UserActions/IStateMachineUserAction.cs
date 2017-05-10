using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation.Logic
{
    public interface IStateMachineUserAction : IUserAction
    {
        void AddState(string stateName, IUserAction action);
        void AddState(UserActionState state);
        void SetState(string stateName);
    }
}
