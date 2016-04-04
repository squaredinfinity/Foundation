using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.DataTemplateSelectors
{
    public class TypeToDataTemplateMappingCollection : ObservableCollectionEx<TypeToDataTemplateMapping>
    {
        IDictionary<object, TypeToDataTemplateMapping> Mappings = new Dictionary<object, TypeToDataTemplateMapping>();

        public TypeToDataTemplateMappingCollection()
        { }

        public TypeToDataTemplateMappingCollection(IEnumerable<TypeToDataTemplateMapping> items)
            : base(monitorElementsForChanges: false, items: items)
        {
            RefreshMappings();
        }

        protected override void OnAfterVersionChanged(long newVersion)
        {
            base.OnAfterVersionChanged(newVersion);

            RefreshMappings();
        }

        void RefreshMappings()
        {
            Mappings = ConstructMappings();
        }

        IDictionary<object, TypeToDataTemplateMapping> ConstructMappings()
        {
            return this.ToDictionary<TypeToDataTemplateMapping, object>(x => ConstructMappingKey(x));
        }

        object ConstructMappingKey(TypeToDataTemplateMapping item)
        {
            if (item.TargetType != null)
                return item.TargetType;

            return item.TargetTypeName;
        }

        public TypeToDataTemplateMappingCollection CombineWith(TypeToDataTemplateMappingCollection other)
        {
            if (other == null || other.Count == 0)
                return new TypeToDataTemplateMappingCollection(this.Items);

            var result = new TypeToDataTemplateMappingCollection();

            var combined_mappings = ConstructMappings();
            combined_mappings.AddOrUpdateFrom(other.Mappings);

            return new TypeToDataTemplateMappingCollection(combined_mappings.Values);
        }

        bool TryFindTemplate(Type itemType, out DataTemplate dataTemplate)
        {
            var mapping = (TypeToDataTemplateMapping)null;

            // try exact type
            if (Mappings.TryGetValue(itemType, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            if (Mappings.TryGetValue(itemType.AssemblyQualifiedName, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            if (Mappings.TryGetValue(itemType.FullName, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            if (Mappings.TryGetValue(itemType.Name, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            dataTemplate = null;
            return false;
        }

        public bool TryFindTemplate(
            DependencyObject itemContainer,
            object item,
            Type itemType,
            out DataTemplate dataTemplate)
        {
            // try exact type
            if (TryFindTemplate(itemType, out dataTemplate))
                return true;

            // try base types
            var base_type = itemType.BaseType;

            while (base_type != null)
            {
                if (TryFindTemplate(base_type, out dataTemplate))
                    return true;

                base_type = base_type.BaseType;
            }

            // exact type not found, try interfaces
            foreach (var interfaceType in itemType.GetInterfaces())
            {
                if (TryFindTemplate(interfaceType, out dataTemplate))
                    return true;
            }

            return false;
        }
    }
}
















































//9J4b5ZHctY3eBK3fz5Gcy1iVOVodAmUYOVodA+lc7Fnc/FlbB62SacRLt0SLt0SLt0SLt0SLEWnc/JXLh5Uh2B4XytXcy9XUuFobtcULOVodA+lc7Fnc/FlbB6mGX0SLt0CiacRLt0SLt0SLtEmTFaHgfJ3exJ3fR5Wgu1yXytXcy9XN2tXgt42guZXeu9WeyRmdxFYd50id7FYLuNob2lnbvlncVJnd0VXg2gkGX0SLt0ii
//9J4b5ZHctY3eBK3fz5Gcy1iVhZHc4pxFt0SLtgoGX0SLt0SLt0SLxxngvlnctkFfw5Wg2x3etgYL0JXgI1iiacRLt0SLt0SLt8Gf8lXLWBoWudHf/1CitQncBiULKqxFt0SLt0SLt0CgB+nd7RXLZ52bylXLI2CdyFIStooGX0SLt0ii
//=0ngvlndw1CgB+ngwFYLhZHc41yRtYVY2BHeacRLt0SLIqxFt0SLt0SLt0SfC+We2BXLxxngvlnctkFfw5Wg2x3etgYL0JXgI1CgyFIStooGXoxFt0SLt0SLt0SfC+We2BXLvxHf51iVAqlb3x3ftgYL0JXgI1CgyFIStooGXoxFt0SLt0SLt0SfC+We2BXLAG4f2tHdtklbvJXetgYL0JXgI1CgyFIStooGXoxFt0SLt0SLt0SfC+We2BXLAG4f2tHdtElcvJId0J3fRZHg9lnbGqxFt0SLt0SLt0CiacRLt0SLt0SLt0SLt0CdyFYLI2yfyFog/tXLx8CiZxHcuFod8tni50ieudHf/pEiWBoWudHf/pYOtknbvJXeKhYWu9mc5p4LI1iiacRLt0SLt0SLtooGX0SLt0ii
//=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
