using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Threading;

namespace SquaredInfinity.Foundation
{
    /// <summary> 
    /// Implements INotifyPropertyChanged
    /// </summary> 
    [Serializable]
    public class NotifyPropertyChangedObject : INotifyPropertyChanged, IFreezable, INotifyVersionChangedObject
    {
        /// <summary> 
        /// Occurs when [property changed]. 
        /// </summary> 
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event EventHandler VersionChanged;

        /// <summary>
        /// Increments version and raises Version Changed event.
        /// </summary>
        protected void IncrementVersion()
        {
            var newVersion = Interlocked.Increment(ref _version);

            OnVersionChanged();
        }
        
        protected void OnVersionChanged()
        {
            if (VersionChanged != null)
                VersionChanged(this, EventArgs.Empty);

            RaisePropertyChanged("Version");
        }

        int _version;
        public int Version
        {
            get { return _version; }
        }

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

        protected virtual bool TrySetThisPropertyValue<T>(
            ref T backingField,
            T value,
            bool raisePropertyChanged = true,
            [CallerMemberName] string propertyName = null)
        {
            if (IsFrozen)
                return false;

            if (object.Equals(backingField, value))
                return false;

            backingField = value;

            if (raisePropertyChanged)
                RaisePropertyChanged(propertyName);

            IncrementVersion();

            return true;
        }

        bool _isFrozen = false;
        bool IFreezable.IsFrozen
        {
            get { return _isFrozen; }
        }

        bool IsFrozen
        {
            get { return _isFrozen; }
            set { _isFrozen = value; }
        }
        
        void IFreezable.Freeze()
        {
            IsFrozen = true;
        }
    }
}
