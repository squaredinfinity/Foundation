﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    /// <summary> 
    /// Implements INotifyPropertyChanged
    /// </summary> 
    [Serializable]
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        /// <summary> 
        /// Occurs when [property changed]. 
        /// </summary> 
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseThisPropertyChanged([CallerMemberName] string propertyName = "")
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
    }
}