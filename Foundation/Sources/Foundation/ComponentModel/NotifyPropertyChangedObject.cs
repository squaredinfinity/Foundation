using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SquaredInfinity.ComponentModel
{
    /// <summary> 
    /// Implements INotifyPropertyChanged
    /// </summary> 
    public class NotifyPropertyChangedObject : INotifyPropertyChanged, IFreezable, INotifyVersionChangedObject
    {
        /// <summary> 
        /// Occurs when [property changed]. 
        /// </summary> 
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<VersionChangedEventArgs> VersionChanged;

        /// <summary>
        /// Increments version and raises Version Changed event.
        /// </summary>
        protected void IncrementVersion()
        { 
            var newVersion = Interlocked.Increment(ref _version);

            OnVersionChanged(newVersion);
        }
        
        protected virtual void OnVersionChanged(long newVersion)
        {
            VersionChanged?.Invoke(this, new VersionChangedEventArgs(newVersion));

            RaisePropertyChanged(nameof(Version));
        }

        long _version;
        public long Version
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

        volatile bool _isFrozen = false;
        bool IFreezable.IsFrozen
        {
            get { return _isFrozen; }
        }

        bool IsFrozen
        {
            get { return _isFrozen; }
            set { TrySetThisPropertyValue(ref _isFrozen, value); }
        }
        
        void IFreezable.Freeze()
        {
            _isFrozen = true;
            RaisePropertyChanged(nameof(IsFrozen));
        }
    }
}
