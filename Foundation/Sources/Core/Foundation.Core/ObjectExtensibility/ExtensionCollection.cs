using SquaredInfinity.Foundation.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.ObjectExtensibility
{
    public class ExtensionCollection<TOwner> :
           CollectionEx<IExtension<TOwner>>,
           IExtensionCollection<TOwner>
           where TOwner : IExtensibleObject<TOwner>
    {
        readonly TOwner Owner;

        public ExtensionCollection(TOwner owner)
        {
            this.Owner = owner;
        }

        public TExtension GetByType<TExtension>()
        {
            using (CollectionLock.AcquireReadLock())
            {
                var extension = GetExtensionInternal_NoLock(typeof(TExtension).FullName);

                if (extension != null)
                    return (TExtension)extension;

                return default(TExtension);
            }
        }

        public TExtension GetOrAdd<TExtension>(Func<TExtension> createValue)
            where TExtension : IExtension<TOwner>
        {
            using (CollectionLock.AcquireReadLock())
            {
                var extension = GetExtensionInternal_NoLock(typeof(TExtension).FullName);

                if (extension != null)
                    return (TExtension)extension;
            }

            // failed to find extension, create a new value and insert
            using (CollectionLock.AcquireUpgradeableReadLock())
            {
                var extension = GetExtensionInternal_NoLock(typeof(TExtension).FullName);

                if (extension != null)
                    return (TExtension)extension;

                extension = createValue();

                Add(extension);

                return (TExtension)extension;
            }
        }

        public object this[Type extensionType]
        {
            get
            {
                if (extensionType == null)
                    throw new ArgumentNullException("extensionType");

                using (CollectionLock.AcquireReadLock())
                {
                    return GetExtensionInternal_NoLock(extensionType.FullName);
                }
            }
        }

        public object this[string extensionTypeFullOrPartialName]
        {
            get
            {
                using (CollectionLock.AcquireReadLock())
                {
                    return GetExtensionInternal_NoLock(extensionTypeFullOrPartialName);
                }
            }
        }

        protected virtual IExtension<TOwner> GetExtensionInternal_NoLock(string extensionTypeFullOrPartialName)
        {
            //# find by full type name
            var extension =
                (from ex in this
                 where ex.GetType().FullName == extensionTypeFullOrPartialName
                 select ex).FirstOrDefault();

            if (extension != null)
                return extension;

            //# find by partial type name
            extension =
                (from ex in this
                 where ex.GetType().Name == extensionTypeFullOrPartialName
                 select ex).FirstOrDefault();

            return extension;
        }
    }
}
