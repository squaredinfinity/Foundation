using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TestEntities
{
    public class TypeWithEvent
    {
        public class RandomNumberEventArgs : EventArgs
        {
            public int Number { get; private set; }

            public RandomNumberEventArgs(int number)
            {
                this.Number = number;
            }
        }

        public event EventHandler<RandomNumberEventArgs> AfterRandomNumberGenerated;

        public void GenerateNumber()
        {
            var rand = new Random();

            var n = rand.Next(7, 13);

            if (AfterRandomNumberGenerated != null)
                AfterRandomNumberGenerated(this, new RandomNumberEventArgs(n));
        }
    }
}
