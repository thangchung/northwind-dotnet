﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using HumanResources.Domain;
using Microsoft.EntityFrameworkCore.Metadata;
using N8T.Core.Domain;

#pragma warning disable 219, 612, 618
#nullable enable

namespace HumanResources
{
    internal partial class EmployeeEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType? baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "HumanResources.Domain.Employee",
                typeof(Employee),
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

            var birthDate = runtimeEntityType.AddProperty(
                "BirthDate",
                typeof(DateTime?),
                propertyInfo: typeof(Employee).GetProperty("BirthDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<BirthDate>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            birthDate.AddAnnotation("Relational:ColumnName", "birth_date");

            var created = runtimeEntityType.AddProperty(
                "Created",
                typeof(DateTime),
                propertyInfo: typeof(EntityBase).GetProperty("Created", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(EntityBase).GetField("<Created>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd);
            created.AddAnnotation("Relational:ColumnName", "created");
            created.AddAnnotation("Relational:DefaultValueSql", "now()");

            var extension = runtimeEntityType.AddProperty(
                "Extension",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("Extension", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<Extension>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            extension.AddAnnotation("Relational:ColumnName", "extension");

            var firstName = runtimeEntityType.AddProperty(
                "FirstName",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("FirstName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<FirstName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            firstName.AddAnnotation("Relational:ColumnName", "first_name");

            var hireDate = runtimeEntityType.AddProperty(
                "HireDate",
                typeof(DateTime?),
                propertyInfo: typeof(Employee).GetProperty("HireDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<HireDate>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            hireDate.AddAnnotation("Relational:ColumnName", "hire_date");

            var lastName = runtimeEntityType.AddProperty(
                "LastName",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("LastName", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<LastName>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            lastName.AddAnnotation("Relational:ColumnName", "last_name");

            var notes = runtimeEntityType.AddProperty(
                "Notes",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("Notes", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<Notes>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            notes.AddAnnotation("Relational:ColumnName", "notes");

            var photo = runtimeEntityType.AddProperty(
                "Photo",
                typeof(byte[]),
                propertyInfo: typeof(Employee).GetProperty("Photo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<Photo>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            photo.AddAnnotation("Relational:ColumnName", "photo");

            var photoPath = runtimeEntityType.AddProperty(
                "PhotoPath",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("PhotoPath", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<PhotoPath>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            photoPath.AddAnnotation("Relational:ColumnName", "photo_path");

            var reportsToId = runtimeEntityType.AddProperty(
                "ReportsToId",
                typeof(Guid?),
                propertyInfo: typeof(Employee).GetProperty("ReportsToId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<ReportsToId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            reportsToId.AddAnnotation("Relational:ColumnName", "reports_to_id");

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            title.AddAnnotation("Relational:ColumnName", "title");

            var titleOfCourtesy = runtimeEntityType.AddProperty(
                "TitleOfCourtesy",
                typeof(string),
                propertyInfo: typeof(Employee).GetProperty("TitleOfCourtesy", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<TitleOfCourtesy>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            titleOfCourtesy.AddAnnotation("Relational:ColumnName", "title_of_courtesy");

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
            key.AddAnnotation("Relational:Name", "pk_employees");

            var index = runtimeEntityType.AddIndex(
                new[] { id },
                unique: true);
            index.AddAnnotation("Relational:Name", "ix_employees_id");

            var index0 = runtimeEntityType.AddIndex(
                new[] { reportsToId });
            index0.AddAnnotation("Relational:Name", "ix_employees_reports_to_id");

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("ReportsToId")! },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id")! })!,
                principalEntityType);

            var reportsTo = declaringEntityType.AddNavigation("ReportsTo",
                runtimeForeignKey,
                onDependent: true,
                typeof(Employee),
                propertyInfo: typeof(Employee).GetProperty("ReportsTo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<ReportsTo>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            runtimeForeignKey.AddAnnotation("Relational:Name", "fk_employees_employees_reports_to_temp_id1");
            return runtimeForeignKey;
        }

        public static RuntimeSkipNavigation CreateSkipNavigation1(RuntimeEntityType declaringEntityType, RuntimeEntityType targetEntityType, RuntimeEntityType joinEntityType)
        {
            var skipNavigation = declaringEntityType.AddSkipNavigation(
                "Territories",
                targetEntityType,
                joinEntityType.FindForeignKey(
                    new[] { joinEntityType.FindProperty("EmployeesId")! },
                    declaringEntityType.FindKey(new[] { declaringEntityType.FindProperty("Id")! })!,
                    declaringEntityType)!,
                true,
                false,
                typeof(List<Territory>),
                propertyInfo: typeof(Employee).GetProperty("Territories", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Employee).GetField("<Territories>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var inverse = targetEntityType.FindSkipNavigation("Employees");
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
            runtimeEntityType.AddAnnotation("Relational:TableName", "employees");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
