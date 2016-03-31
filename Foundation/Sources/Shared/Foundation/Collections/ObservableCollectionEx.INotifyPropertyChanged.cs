using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Collections
{
    public partial class ObservableCollectionEx<TItem> : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaiseThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> 
        /// Raises the property changed event. 
        /// </summary> 
        /// <param name="expression">The expression (i.e. () => this.MyProperty).</param> 
        public void RaisePropertyChanged(Expression<Func<object>> expression)
        {
            PropertyChanged.Notify(expression);
        }
        /// <summary> 
        /// Raises the property changed event. 
        /// </summary> 
        /// <param name="propertyName">Name of a property.</param> 
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary> 
        /// Raises the property changed event for indexer (all indexes) 
        /// </summary> 
        public void RaiseIndexerChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
        }
        /// <summary> 
        /// Raises the property changed event for indexer (specific index) 
        /// </summary> 
        public void RaiseIndexerChanged(string index)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Item[{0}]".FormatWith(index)));
        }
        /// <summary> 
        /// Raises the property changed event for indexer (specific index) 
        /// </summary> 
        public void RaiseIndexerChanged(int index)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Item[{0}]".FormatWith(index)));
        }

        /// <summary>
        /// Raises the property changed event for all properties by providing string.Empty as a property name
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.propertychanged.aspx
        /// </remarks>
        public void RaiseAllPropertiesChanged()
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        protected virtual bool TrySetThisPropertyValue<T>(
            ref T backingField,
            T value,
            bool raisePropertyChanged = true,
            [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(backingField, value))
                return false;

            backingField = value;

            if (raisePropertyChanged)
                RaisePropertyChanged(propertyName);

            IncrementVersion();

            return true;
        }
    }
}

//wVnYsl2Ygkmb0VmcmF2YlBSSEFGdhRUatVmbzl2bu1gCgACIgsXDKACIgACIgACIzRncp52Zg4UYtVGI7ByZlR3OgMXZ0tDI91gCNoAIgACIgACIgQUatVmbzl2buRUY0FGIEFGdhByegcWZ0tDI91gCgACIgACIgAidvlGZgUFckFGdlRUY0FmUh52ZlhCZvVnYsVGItlmbsACZvVnYsVGItFGepsTDKACIgACIgACIlZXZuRHIFZXZuRHSh5GZsVmc8Ukdl5GdBJ3Zz5DIBZGdlJHRhRXYSFmbnV2QoFmbnVGZ70gCNoAIgACIgACIgQ2b1JGblBiTvJXbhxWa6V2Qv9mcklmbhRXZoQ2b1JGblBCZl52by1WYslmelR2Qv9mcklmbhRXZpsTDKACIgACIgACIk9WdixWZgQUZu9mctFGbppXZD92byRWauFGdlhCZvVnYsVGIu9mctFGbppXZkN0bvJHZp5WY0VWK70gCNoAIgACIgACIgQ2b1JGblBCRl52by1WYslmelRUazRXYuNWZoQ2b1JGblBibvJXbhxWa6VGZEl2c0FmbjVWK70gCgACIgACIgACZvVnYsVGIO9mctFGbppXZEl2c0FmbjVGKk9WdixWZgQWZu9mctFGbppXZkRUazRXYuNWZpsTDKACIgASf
//wVnYsl2YgMGbhN3cgQUY0FGRp1WZuNXav5GI6ASSEFGdhRUatVmbzl2bu1gCgACIgsXDKACIgACIgACIElWbl52cp9mbEFGdhByXkFGdhBSPg4WZ3BCRp1WZuNXav5GRhRXYokyONoAIgACIgACIgAXdixWajBCRp1WZuNXav5GRhRXYgQUY0FWDKACIgACIgACI71gCgACIgACIgACIgACInVGdgsHIyVGd1Jnbg8FZhRXY7ASfNoAIgACIgACIgACIgACcylmdhRXZgMXZ0Byeg8FZhRXYg0DI2FGb1V2Og0XDKACIgACIgACI91gCNoAIgACIgACIgAXdixWajBSZ2Vmb0BSR2Vmb0hUYuRGblJHPFZXZuRXQyd2c+ASQmRXZyRUY0FmUh52ZlNEah52ZlR2ONoAIgACIgACIgY3bpRGIP5WQmRXZyRUY0FmUh52ZlNEah52ZlRGKp0gCgACIgACIgAyeNoAIgACIgACIgACIgASamBCKBZGdlJHRhRXYSFmbnV2QoFmbnVGZgESPg4WdsxWKNoAIgACIgACIgACIgACIgACIBZGdlJHRhRXYSFmbnV2QoFmbnVGZoQHapNHLgUkdl5GdBJ3Zz5SRtBHd5lyONoAIgACIgACIg0XDK0gCgACIgACIgACc1JGbpNGI29WakBSVwRWY0VGRhRXYSFmbnVGKk9WdixWZg0WauxCIk9WdixWZg0WY4lSDKACIgACIgACI71gCgACIgACIgACIgACIi92bsBCahN3XjhWYudWZkBSPgYWYsNXZ70gCNoAIgACIgACIgACIgASamBCKhQUY0FmLNlmbuk0cDx2bzVGVvhSbp5WKp0gCgACIgACIgACIgACI71gCgACIgACIgACIgACIgACIgQUY0FmLNlmbg0DItlmb70gCgACIgACIgACIgACIgACIggWYz91YoFmbnVGZg0DI0JXdltTDKACIgACIgACIgACIg0XDK0gCgACIgACIgACIgACIpZGIoECRhRXYu0UY45SSzNEbvNXZU9GKtFGepkSDKACIgACIgACIgACIgsXDKACIgACIgACIgACIgACIgACRhRXYu0UY4BSPg0WY4tTDKACIgACIgACIgACIgACIgACahN3XjhWYudWZkBSPgQnc1V2ONoAIgACIgACIgACIgASfNoQDKACIgACIgACIgACIgYXYyBich52ZlBSPg0WY4BSLg0WautTDK0gCgACIgACIgACIgACIpZGIoECRhRXYuMFch5mLJN3Qs92clR1boIXYudWZpkSDKACIgACIgACIgACIgsXDKACIgACIgACIgACIgACIgACRhRXYuMFch5GI9ASbhhHItASbp52ONoAIgACIgACIgACIgACIgACIoF2cfNGah52ZlRGI9ACdyVXZ70gCgACIgACIgACIgACI91gCNoAIgACIgACIgACIgASamBCKoF2cfNGah52ZlRWKNoAIgACIgACIgACIgACIgACIP5WQmRXZyRUY0FmUh52ZlNEah52ZlRGKpsTDKACIgACIgACI91gCNoQDKACIgACIgACIwVnYsl2YgYXayRXdhxGIk9WdixWZg40by1WYslmelN0bvJHZp5WY0VGKk9WdixWZgYXYsVXZp0gCgACIgACIgAyeNoAIgACIgACIgACIgAiclRXdy5GI2FGb1V2ONoAIgACIgACIg0XDK0gCgACIgACIgACc1JGbpNGI2lmc0VXYsBCZvVnYsVGIEVmbvJXbhxWa6V2Qv9mcklmbhRXZoQ2b1JGblBibvJXbhxWa6VGZWFGb1VWKNoAIgACIgACIgsXDKACIgACIgACIgACIgIXZ0VncuBibvJXbhxWa6VGZWFGb1V2ONoAIgACIgACIg0XDK0gCgACIgACIgACc1JGbpNGI2lmc0VXYsBCZvVnYsVGIEVmbvJXbhxWa6VGRpNHdh52YlhCZvVnYsVGIu9mctFGbppXZkRUazRXYuNWZp0gCgACIgACIgAyeNoAIgACIgACIgACIgAiclRXdy5GIu9mctFGbppXZkRUazRXYuNWZ70gCgACIgACIgASfNoQDKACIgACIgACIwVnYsl2YgYXayRXdhxGIk9WdixWZg40by1WYslmelRUazRXYuNWZoQ2b1JGblBCZl52by1WYslmelRGRpNHdh52YllSDKACIgACIgACI71gCgACIgACIgACIgACIyVGd1JnbgQWZu9mctFGbppXZkRUazRXYuNWZ70gCgACIgACIgASfNoQDKACIgACIgACIwVnYsl2YgMHdylmbnBiTh1WZgsHInVGd7AyclR3Og0XDKACIgASf
//=AXdixWajBSauRXZyZWYjVGIJZVazlmYsVGRp1WZuNXav5GI6ASSEFGdhRUatVmbzl2bu1gCgACIgsXDKACIgACIgACIElWbl52cp9mbEFGdhBiVpNXaixWZgsHInVGd7ASfNoAIgACIgACIgI2bvxGIBJXZD92byRWauFGdlNXSuZXZyRXZkByegcWZ0tDI91gCgACIgACIgAidvlGZgUFckFGdlZVazlmYsVmUh52ZlhCZvVnYsVGI2l2cpJGbl9Vbp5GLgQ2b1JGblBidpNXaixWZf1WY4lyONoQDKACIgACIgACI29WakBiWv9WboQ2b1JGblBiev9WbfZWYjR3bylyONoAIgACIgACIgY3bpRGIa92btFEdQ9WauRHKk9WdixWZgo3bv12XmF2Y09mcsACZvVnYsVGIw9WauR3Xj92byRWauFGdllyONoQDKACIgACIgACIlZXZuRHIFZXZuRHSh5GZsVmc8Ukdl5GdBJ3Zz5DIBZGdlJnVpNXaixWZSFmbnV2QoFmbnVGZ70gCgACIgACIgAidvlGZgYUa0FEbsRUY0FGKpsTDKACIgACIgACI29WakBiRpRHVvJVYudWZTBXYuhCZvVnYsVGIyFmbnV2UwFmbpsTDKACIgASf
//gACIgAXdixWajByYsF2czBCRp1WZuNXav5GRhRXYNoAIgACI71gCgACIgACIgACc1JGbpNGIk9WdixWZg0UauByegcWZ0tDIp5GdlJnbhxGIzVGd7ASfNoAIgACIgACIgAXdixWajBCZvVnYsVGINFGegsHInVGd7ASauRXZy5WYsByclR3Og0XDKACIgACIgACIwVnYsl2YgQ2b1JGblByUwFmbgsHInVGd7ASauRXZy5WYsByclR3Og0XDKACIgASf