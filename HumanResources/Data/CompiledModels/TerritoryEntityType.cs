﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using N8T.Core.Domain;

#pragma warning disable 219, 612, 618
#nullable enable

namespace HumanResources
{
    internal partial class TerritoryEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "HumanResources.Domain.Territory",
                typeof(Territory),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(Guid),
                propertyInfo: typeof(EntityBase).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(EntityBase).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "id");
            id.AddAnnotation("Relational:ColumnType", "uuid");
            id.AddAnnotation("Relational:DefaultValueSql", "uuid_generate_v4()");

            var created = runtimeEntityType.AddProperty(
                "Created",
                typeof(DateTime),
                propertyInfo: typeof(EntityBase).GetProperty("Created", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(EntityBase).GetField("<Created>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            created.AddAnnotation("Relational:ColumnName", "created");

            var description = runtimeEntityType.AddProperty(
                "Description",
                typeof(string),
                propertyInfo: typeof(Territory).GetProperty("Description", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Territory).GetField("<Description>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            description.AddAnnotation("Relational:ColumnName", "description");

            var regionId = runtimeEntityType.AddProperty(
                "RegionId",
                typeof(Guid),
                propertyInfo: typeof(Territory).GetProperty("RegionId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Territory).GetField("<RegionId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            regionId.AddAnnotation("Relational:ColumnName", "region_id");

            var updated = runtimeEntityType.AddProperty(
                "Updated",
                typeof(DateTime?),
                propertyInfo: typeof(EntityBase).GetProperty("Updated", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(EntityBase).GetField("<Updated>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            updated.AddAnnotation("Relational:ColumnName", "updated");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);
            key.AddAnnotation("Relational:Name", "pk_territories");

            var index = runtimeEntityType.AddIndex(
                new[] { id },
                unique: true);
            index.AddAnnotation("Relational:Name", "ix_territories_id");

            var index0 = runtimeEntityType.AddIndex(
                new[] { regionId });
            index0.AddAnnotation("Relational:Name", "ix_territories_region_id");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("RegionId")! },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id")! })!,
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var region = declaringEntityType.AddNavigation("Region",
                runtimeForeignKey,
                onDependent: true,
                typeof(Region),
                propertyInfo: typeof(Territory).GetProperty("Region", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Territory).GetField("<Region>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var territories = principalEntityType.AddNavigation("Territories",
                runtimeForeignKey,
                onDependent: false,
                typeof(List<Territory>),
                propertyInfo: typeof(Region).GetProperty("Territories", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Territories>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "fk_territories_regions_region_id");
            return runtimeForeignKey;
        }

        public static RuntimeSkipNavigation CreateSkipNavigation1(RuntimeEntityType declaringEntityType, RuntimeEntityType targetEntityType, RuntimeEntityType joinEntityType)
        {
            var skipNavigation = declaringEntityType.AddSkipNavigation(
                "Employees",
                targetEntityType,
                joinEntityType.FindForeignKey(
                    new[] { joinEntityType.FindProperty("TerritoriesId")! },
                    declaringEntityType.FindKey(new[] { declaringEntityType.FindProperty("Id")! })!,
                    declaringEntityType)!,
                true,
                false,
                typeof(List<Employee>),
                propertyInfo: typeof(Territory).GetProperty("Employees", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Territory).GetField("<Employees>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var inverse = targetEntityType.FindSkipNavigation("Territories");
            if (inverse != null)
            {
                skipNavigation.Inverse = inverse;
                inverse.Inverse = skipNavigation;
            }

            return skipNavigation;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", "human_resources");
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "territories");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
