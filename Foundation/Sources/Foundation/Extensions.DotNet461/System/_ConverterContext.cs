using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{

    class _ConverterContext : ITypeDescriptorContext
    {
        public IContainer Container => null;
        public object Instance { get; private set; }
        public PropertyDescriptor PropertyDescriptor => null;

        public _ConverterContext(object instance)
        {
            this.Instance = instance;
        }


        public void OnComponentChanged() { }
        public bool OnComponentChanging() => true;
        public object GetService(Type serviceType) => null;
    }
}
