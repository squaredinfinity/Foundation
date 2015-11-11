using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        [Description("do not attempt to access read only properties that throw exceptions")]
        public void Bug004_CloneFailsOnTypeWithReadOnlyPropertiesWhichThrowException()
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

        #region BUG 005

        [TestMethod]
        [Description("mapping fails, when type contains list property without a setter")]
        public void Bug005_mapping_fails_when_type_contains_list_property_without_a_setter()
        {
            var c1 = new Bug005_Class_One { Name = "1" };

            var c1_1 = new Bug005_Class_One { Name = "1_1" };
            var c1_2 = new Bug005_Class_One { Name = "1_2" };
            var c1_3 = new Bug005_Class_One { Name = "1_3" };

            c1.Items.Add(c1_1);
            c1.Items.Add(c1_2);
            c1.Items.Add(c1_3);

            var c2 = new Bug005_Class_One();

            var tm = new TypeMapper();

            tm.Map(c1, c2);

            Assert.AreEqual(c1.Name, c2.Name);
            Assert.AreEqual(c1.Items.Count, c2.Items.Count);

            for(int i = 0; i < c1.Items.Count; i++)
            {
                Assert.AreEqual(c1.Items[i].Name, c2.Items[i].Name);
            }
        }

        public class Bug005_Class_One
        {
            public string Name { get; set; }

            List<Bug005_Class_One> _items = new List<Bug005_Class_One>();
            public List<Bug005_Class_One> Items
            {
                get { return _items; }
            }
        }

        #endregion

        #region BUG 006

        [TestMethod]
        [Description("mapping of a collection property fails when ReuseTargetCollectionsWhenPossible is set to false")]
        public void Bug006_ReuseTargetCollectionsWhenPossible_false__Fails_to_create_new_collection()
        {
            var c1 = new Bug006_Class_One { Name = "1" };

            var trp1 = new Bug006_Class_One
            {
                Name = "trp1"
            };
            var trp2 = new Bug006_Class_One
            {
                Name = "trp2"
            };

            c1.Items.Add(trp1);
            c1.Items.Add(trp2);

            var c2 = new Bug006_Class_One();

            var mo = new MappingOptions()
            {
                ReuseTargetCollectionsWhenPossible = false,
            };

            var tm = new TypeMapper();

            tm.Map(c1, c2, mo);

            Assert.AreEqual(c1.Name, c2.Name);

            // when the bug wasn't fixed, c2.Items.Count would be 0
            Assert.AreEqual(c1.Items.Count, c2.Items.Count);

            for (int i = 0; i < c1.Items.Count; i++)
            {
                Assert.AreEqual(c1.Items[i].Name, c2.Items[i].Name);
            }
        }

        public class Bug006_Class_One
        {
            public string Name { get; set; }

            List<Bug006_Class_One> _items = new List<Bug006_Class_One>();
            public List<Bug006_Class_One> Items
            {
                get { return _items; }
                set { _items = value; }
            }
        }

        #endregion
    }
}
