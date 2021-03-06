﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    [TestClass]
    public class TypeMapper__BugFixes
    {
        #region BUG 001

        [TestMethod]
        [Description("mapping fails when target property is of interface type (or abstract class)")]
        public void Bug001__MapToInterfaceProperty()
        {
            var c1 = new Bug001_Class();
            c1.Id = 1;
            c1.Property = new Bug001_Class();
            c1.Property.Id = 2;

            var tm = new TypeMapper();

            var clone = tm.DeepClone(c1);

            Assert.IsNotNull(clone);
            Assert.AreEqual(clone.Id, c1.Id);
            Assert.AreEqual(clone.Property.Id, c1.Property.Id);
            Assert.IsNull(clone.Property.Property);
        }

        public class Bug001_Class : Bug001_Interface
        {
            public int Id { get; set; }
            public Bug001_Interface Property { get; set; }
        }

        public interface Bug001_Interface
        {
            int Id { get; set; }
            Bug001_Interface Property { get; set; }
        }

        #endregion

        #region BUG 002

        [TestMethod]
        [Description("mapping fails when target property (of concrete type) in null and source is of interface type")]
        public void Bug002__MapToInterfaceProperty_WhereTargetInterfaceIsDifferentThanSourceType()
        {
            var c1 = new Bug002_Class_One();
            c1.Id = 1;
            c1.Property = new Bug002_Class_One();
            c1.Property.Id = 2;

            var tm = new TypeMapper();

            var clone = tm.Map<Bug002_Class_Two>(c1);

            Assert.IsNotNull(clone);
            Assert.AreEqual(clone.Id, c1.Id);
            Assert.AreEqual(clone.Property.Id, c1.Property.Id);
            Assert.IsNull(clone.Property.Property);
        }

        public class Bug002_Class_One : Bug002_Interface_One
        {
            public int Id { get; set; }
            public Bug002_Interface_One Property { get; set; }
        }

        public interface Bug002_Interface_One
        {
            int Id { get; set; }
            Bug002_Interface_One Property { get; set; }
        }

        public class Bug002_Class_Two
        {
            public int Id { get; set; }
            public Bug002_Class_Two Property { get; set; }
        }

        #endregion

        #region BUG 003

        [TestMethod]
        [Description("mapping fails when source has read-only property with Null value")]
        public void Bug003_MappingFailsWhenSourceHasReadOnlyPropertyWithNullValue()
        {
            var c1 = new Bug003_Class_One();
            
            var tm = new TypeMapper();

            var clone = tm.Map<Bug003_Class_One>(c1);

            Assert.IsNotNull(clone);
            Assert.AreEqual(clone.Id, c1.Id);
        }

        public class Bug003_Class_One
        {
            int? _id;
            public int? Id
            {
                get { return _id; }
            }

            public Bug003_Class_One()
            {
                _id = null;
            }
        }

        #endregion

        #region BUG 004

        [TestMethod]
        [Description("do not attempt to access read only properties")]
        public void Bug004_CloneFailsOnTypeWithReadOnlyPropertiesWhichThrowException__ShouldNotAttemptToAccessReadOnlyPropertiesAnyway()
        {
            var c1 = new Bug004_Class_One();

            var tm = new TypeMapper();

            var clone = tm.Map<Bug004_Class_One>(c1);

            Assert.IsNotNull(clone);
            Assert.AreEqual(clone.Id, c1.Id);
        }

        public class Bug004_Class_One
        {
            int? _id;
            public int? Id
            {
                get { return _id; }
            }

            public int ThrowMe
            {
                get { throw new Exception(); }
            }

            public Bug004_Class_One()
            {
                _id = null;
            }
        }

        #endregion
    }
}
