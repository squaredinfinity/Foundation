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
                PropertyChanged(this, new PropertyChangedEventArgs($"Item[{index}]"));
        }
        /// <summary> 
        /// Raises the property changed event for indexer (specific index) 
        /// </summary> 
        public void RaiseIndexerChanged(int index)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs($"Item[{index}]"));
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