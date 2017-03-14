using SquaredInfinity.Collections;
using SquaredInfinity.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Presentation.DataTemplateSelectors
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

            // by type
            if (Mappings.TryGetValue(itemType, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            // by fully qualified assembly name
            if (Mappings.TryGetValue(itemType.AssemblyQualifiedName, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            // by full name
            if (Mappings.TryGetValue(itemType.FullName, out mapping))
            {
                dataTemplate = mapping.DataTemplate;
                return true;
            }

            // by short name
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