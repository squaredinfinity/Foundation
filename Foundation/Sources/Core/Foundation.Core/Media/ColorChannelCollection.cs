using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Media
{
    public class ColorChannelCollection : ReadOnlyCollection<ColorChannel>
    {
        internal ColorChannelCollection(IList<ColorChannel> channels)
            : base(channels) { }

        internal ColorChannelCollection(params ColorChannel[] channels)
            : base(channels) { }

        public ColorChannel this[string channelName]
        {
            get
            {
                var result =
                    (from cc in this
                     where string.Equals(cc.ChannelDefinition.Name, channelName, StringComparison.CurrentCultureIgnoreCase) //NOTE: CORE UNSUPORTED, StringComparison.InvariantCultureIgnoreCase)
                     select cc).FirstOrDefault();

                return result;
            }
        }
    }
}
