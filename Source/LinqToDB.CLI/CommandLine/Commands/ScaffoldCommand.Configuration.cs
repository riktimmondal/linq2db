﻿using System;
using System.Collections.Generic;
using LinqToDB.DataModel;
using LinqToDB.Metadata;
using LinqToDB.Naming;
using LinqToDB.Scaffold;
using LinqToDB.Schema;

namespace LinqToDB.CommandLine
{
	partial class ScaffoldCommand : CliCommand
	{
		/// <summary>
		/// Configure scaffolder options object.
		/// </summary>
		/// <param name="options">CLI options.</param>
		/// <returns>Scaffold settings or <c>null</c> on error.</returns>
		private static ScaffoldOptions? ProcessScaffoldOptions(Dictionary<CliOption, object?> options)
		{
			ScaffoldOptions settings;

			// before reading other options load base settings object
			if (options.Remove(General.OptionsTemplate, out var value) && value is "t4")
				settings = ScaffoldOptions.T4();
			else
				settings = ScaffoldOptions.Default();

			// process scaffold options
			ProcessSchemaOptions   (options, settings.Schema        );
			ProcessDataModelOptions(options, settings.DataModel     );
			ProcessCodeGenOptions  (options, settings.CodeGeneration);

			return settings;
		}

		/// <summary>
		/// Process code-generation options.
		/// </summary>
		/// <param name="options">All specified options.</param>
		/// <param name="settings">Code-generation settings to configure.</param>
		private static void ProcessCodeGenOptions(Dictionary<CliOption, object?> options, CodeGenerationOptions settings)
		{
			// conflicting identifiers
			if (options.Remove(CodeGen.ConflictingIdentifiers, out var value))
			{
				foreach (var name in (string[])value!)
					settings.ConflictingNames.Add(name);
			}

			// simple flags
			if (options.Remove(CodeGen.NRTEnable             , out value)) settings.EnableNullableReferenceTypes  =  (bool)value!;
			if (options.Remove(CodeGen.NoXmlDocWarns         , out value)) settings.SuppressMissingXmlDocWarnings =  (bool)value!;
			if (options.Remove(CodeGen.MarkAutogenerated     , out value)) settings.MarkAsAutoGenerated           =  (bool)value!;
			if (options.Remove(CodeGen.SingleFile            , out value)) settings.ClassPerFile                  = !(bool)value!;
			if (options.Remove(CodeGen.AddGeneratedFileSuffix, out value)) settings.AddGeneratedFileSuffix        =  (bool)value!;

			// strings
			if (options.Remove(CodeGen.Indent      , out value)) settings.Indent              = (string)value!;
			if (options.Remove(CodeGen.NewLine     , out value)) settings.NewLine             = (string)value!;
			if (options.Remove(CodeGen.CustomHeader, out value)) settings.AutoGeneratedHeader = (string)value!;
			if (options.Remove(CodeGen.Namespace   , out value)) settings.Namespace           = (string)value!;
		}

		/// <summary>
		/// Process data model options.
		/// </summary>
		/// <param name="options">All specified options.</param>
		/// <param name="settings">Data model settings to configure.</param>
		private static void ProcessDataModelOptions(Dictionary<CliOption, object?> options, DataModelOptions settings)
		{
			object? value;

			// simple flags
			if (options.Remove(DataModel.GenerateDbName                , out value)) settings.IncludeDatabaseName                = (bool)value!;
			if (options.Remove(DataModel.GenerateDefaultSchemaName     , out value)) settings.GenerateDefaultSchema              = (bool)value!;
			if (options.Remove(DataModel.DataTypeOnTables              , out value)) settings.GenerateDataType                   = (bool)value!;
			if (options.Remove(DataModel.DbTypeOnTables                , out value)) settings.GenerateDbType                     = (bool)value!;
			if (options.Remove(DataModel.LengthOnTables                , out value)) settings.GenerateLength                     = (bool)value!;
			if (options.Remove(DataModel.PrecisionOnTables             , out value)) settings.GeneratePrecision                  = (bool)value!;
			if (options.Remove(DataModel.ScaleOnTables                 , out value)) settings.GenerateScale                      = (bool)value!;
			if (options.Remove(DataModel.EmitDbInfo                    , out value)) settings.IncludeDatabaseInfo                = (bool)value!;
			if (options.Remove(DataModel.EmitInitDataContextMethod     , out value)) settings.GenerateInitDataContextMethod      = (bool)value!;
			if (options.Remove(DataModel.EmitDefaultConstructor        , out value)) settings.HasDefaultConstructor              = (bool)value!;
			if (options.Remove(DataModel.EmitConfigurationConstructor  , out value)) settings.HasConfigurationConstructor        = (bool)value!;
			if (options.Remove(DataModel.EmitOptionsConstructor        , out value)) settings.HasUntypedOptionsConstructor       = (bool)value!;
			if (options.Remove(DataModel.EmitTypedOptionsConstructor   , out value)) settings.HasTypedOptionsConstructor         = (bool)value!;
			if (options.Remove(DataModel.EmitAssociations              , out value)) settings.GenerateAssociations               = (bool)value!;
			if (options.Remove(DataModel.EmitAssociationExtensions     , out value)) settings.GenerateAssociationExtensions      = (bool)value!;
			if (options.Remove(DataModel.ReuseEntitiesInFunctions      , out value)) settings.MapProcedureResultToEntity         = (bool)value!;
			if (options.Remove(DataModel.TableFunctionReturnsITable    , out value)) settings.TableFunctionReturnsTable          = (bool)value!;
			if (options.Remove(DataModel.EmitSchemaErrors              , out value)) settings.GenerateProceduresSchemaError      = (bool)value!;
			if (options.Remove(DataModel.SkipProceduresWithSchemaErrors, out value)) settings.SkipProceduresWithSchemaErrors     = (bool)value!;
			if (options.Remove(DataModel.ReturnListFromProcedures      , out value)) settings.GenerateProcedureResultAsList      = (bool)value!;
			if (options.Remove(DataModel.DbTypeInProcedures            , out value)) settings.GenerateProcedureParameterDbType   = (bool)value!;
			if (options.Remove(DataModel.SchemasAsTypes                , out value)) settings.GenerateSchemaAsType               = (bool)value!;
			if (options.Remove(DataModel.GenerateIEquatable            , out value)) settings.GenerateIEquatable                 = (bool)value!;
			if (options.Remove(DataModel.FindParametersInOrdinalOrder  , out value)) settings.OrderFindParametersByColumnOrdinal = (bool)value!;
			if (options.Remove(DataModel.EntityClassIsPartial          , out value)) settings.EntityClassIsPartial               = (bool)value!;

			// strings
			if (options.Remove(DataModel.BaseEntity          , out value)) settings.BaseEntityClass  = (string)value!;
			if (options.Remove(DataModel.BaseEntity          , out value)) settings.BaseEntityClass  = (string)value!;
			if (options.Remove(DataModel.DataContextName     , out value)) settings.ContextClassName = (string)value!;
			if (options.Remove(DataModel.DataContextBaseClass, out value)) settings.BaseContextClass = (string)value!;

			// stored procedure signatures
			if (options.Remove(DataModel.StoredProcedureTypes, out value))
			{
				foreach (var strVal in (string[])value!)
				{
					switch (strVal)
					{
						case "sync" : settings.GenerateProcedureSync  = true; break;
						case "async": settings.GenerateProcedureAsync = true; break;
						default     : throw new InvalidOperationException($"Unsuppored value for option {DataModel.StoredProcedureTypes.Name}: {strVal}");
					}
				}
			}

			// Metadata source
			if (options.Remove(DataModel.Metadata, out value))
			{
				var str = (string)value!;
				settings.Metadata = str switch
				{
					"none"       => MetadataSource.None,
					"attributes" => MetadataSource.Attributes,
					"fluent"     => MetadataSource.FluentMapping,
					_            => throw new InvalidOperationException($"Unsuppored value for option {DataModel.Metadata.Name}: {str}")
				};
			}

			// Find method variants
			if (options.Remove(DataModel.GenerateFind, out value))
			{
				var findTypes = FindTypes.None;
				var hasNone   = false;
				foreach (var strVal in (string[])value!)
				{
					switch (strVal)
					{
						case "none"                : hasNone = true                                   ; break;
						case "sync-pk-table"       : findTypes |= FindTypes.FindByPkOnTable           ; break;
						case "async-pk-table"      : findTypes |= FindTypes.FindAsyncByPkOnTable      ; break;
						case "query-pk-table"      : findTypes |= FindTypes.FindQueryByPkOnTable      ; break;
						case "sync-entity-table"   : findTypes |= FindTypes.FindByRecordOnTable       ; break;
						case "async-entity-table"  : findTypes |= FindTypes.FindAsyncByRecordOnTable  ; break;
						case "query-entity-table"  : findTypes |= FindTypes.FindQueryByRecordOnTable  ; break;
						case "sync-pk-context"     : findTypes |= FindTypes.FindByPkOnContext         ; break;
						case "async-pk-context"    : findTypes |= FindTypes.FindAsyncByPkOnContext    ; break;
						case "query-pk-context"    : findTypes |= FindTypes.FindQueryByPkOnContext    ; break;
						case "sync-entity-context" : findTypes |= FindTypes.FindByRecordOnContext     ; break;
						case "async-entity-context": findTypes |= FindTypes.FindAsyncByRecordOnContext; break;
						case "query-entity-context": findTypes |= FindTypes.FindQueryByRecordOnContext; break;
						default                    : throw new InvalidOperationException($"Unsuppored value for option {DataModel.GenerateFind.Name}: {strVal}");
					}
				}

				if (hasNone && findTypes != FindTypes.None)
					throw new InvalidOperationException($"Option {DataModel.GenerateFind.Name} combines `none` value with other values ({string.Join(',', (string[])value!)})");

				settings.GenerateFindExtensions = findTypes;
			}

			// association many-side type
			if (options.Remove(DataModel.AssociationCollectionType, out value))
			{
				var str = (string)value!;
				if (str == "[]")
					settings.AssociationCollectionAsArray = true;
				else
				{
					settings.AssociationCollectionAsArray = false;
					settings.AssociationCollectionType = str;
				}
			}

			// non-default schema name mappings
			if (options.Remove(DataModel.SchemaTypeClassNames, out value))
			{
				var mappings = (Dictionary<string, string>)value!;
				foreach (var kvp in mappings)
					settings.SchemaMap.Add(kvp);
			}

			// naming options
			if (options.Remove(DataModel.DataContextClassNaming              , out value)) settings.DataContextClassNameOptions                    = ((NormalizationOptions)value!).MergeInto(settings.DataContextClassNameOptions);
			if (options.Remove(DataModel.EntityClassNaming                   , out value)) settings.EntityClassNameOptions                         = ((NormalizationOptions)value!).MergeInto(settings.EntityClassNameOptions);
			if (options.Remove(DataModel.EntityColumnPropertyNaming          , out value)) settings.EntityColumnPropertyNameOptions                = ((NormalizationOptions)value!).MergeInto(settings.EntityColumnPropertyNameOptions);
			if (options.Remove(DataModel.EntityContextPropertyNaming         , out value)) settings.EntityContextPropertyNameOptions               = ((NormalizationOptions)value!).MergeInto(settings.EntityContextPropertyNameOptions);
			if (options.Remove(DataModel.AssociationNaming                   , out value)) settings.SourceAssociationPropertyNameOptions           = ((NormalizationOptions)value!).MergeInto(settings.SourceAssociationPropertyNameOptions);
			if (options.Remove(DataModel.AssocationBackReferenceSingleNaming , out value)) settings.TargetSingularAssociationPropertyNameOptions   = ((NormalizationOptions)value!).MergeInto(settings.TargetSingularAssociationPropertyNameOptions);
			if (options.Remove(DataModel.AssocationBackReferenceManyNaming   , out value)) settings.TargetMultipleAssociationPropertyNameOptions   = ((NormalizationOptions)value!).MergeInto(settings.TargetMultipleAssociationPropertyNameOptions);
			if (options.Remove(DataModel.ProcOrFuncMethodNaming              , out value)) settings.ProcedureNameOptions                           = ((NormalizationOptions)value!).MergeInto(settings.ProcedureNameOptions);
			if (options.Remove(DataModel.ProcOrFuncParameterNaming           , out value)) settings.ProcedureParameterNameOptions                  = ((NormalizationOptions)value!).MergeInto(settings.ProcedureParameterNameOptions);
			if (options.Remove(DataModel.ProcOrFuncResultClassNaming         , out value)) settings.ProcedureResultClassNameOptions                = ((NormalizationOptions)value!).MergeInto(settings.ProcedureResultClassNameOptions);
			if (options.Remove(DataModel.AsyncProcResultClassNaming          , out value)) settings.AsyncProcedureResultClassNameOptions           = ((NormalizationOptions)value!).MergeInto(settings.AsyncProcedureResultClassNameOptions);
			if (options.Remove(DataModel.AsyncProcResultClassPropertyNaming  , out value)) settings.AsyncProcedureResultClassPropertiesNameOptions = ((NormalizationOptions)value!).MergeInto(settings.AsyncProcedureResultClassPropertiesNameOptions);
			if (options.Remove(DataModel.ProcOrFuncResultColumnPropertyNaming, out value)) settings.ProcedureResultColumnPropertyNameOptions       = ((NormalizationOptions)value!).MergeInto(settings.ProcedureResultColumnPropertyNameOptions);
			if (options.Remove(DataModel.FunctionTupleClassNaming            , out value)) settings.FunctionTupleResultClassNameOptions            = ((NormalizationOptions)value!).MergeInto(settings.FunctionTupleResultClassNameOptions);
			if (options.Remove(DataModel.FunctionTupleFieldPropertyNaming    , out value)) settings.FunctionTupleResultPropertyNameOptions         = ((NormalizationOptions)value!).MergeInto(settings.FunctionTupleResultPropertyNameOptions);
			if (options.Remove(DataModel.SchemaWrapperClassNaming            , out value)) settings.SchemaClassNameOptions                         = ((NormalizationOptions)value!).MergeInto(settings.SchemaClassNameOptions);
			if (options.Remove(DataModel.SchemaContextPropertyNaming         , out value)) settings.SchemaPropertyNameOptions                      = ((NormalizationOptions)value!).MergeInto(settings.SchemaPropertyNameOptions);
			if (options.Remove(DataModel.FindParameterNaming                 , out value)) settings.FindParameterNameOptions                       = ((NormalizationOptions)value!).MergeInto(settings.FindParameterNameOptions);
		}

		/// <summary>
		/// Process schema-related options.
		/// </summary>
		/// <param name="options">All specified options.</param>
		/// <param name="settings">Schema settings to configure.</param>
		private static void ProcessSchemaOptions(Dictionary<CliOption, object?> options, Scaffold.SchemaOptions settings)
		{
			// objects to load
			if (options.Remove(SchemaOptions.LoadedObjects, out var value))
			{
				var objects = SchemaObjects.None;
				foreach (var strVal in (string[])value!)
				{
					switch (strVal)
					{
						case "table"             : objects |= SchemaObjects.Table;             break;
						case "view"              : objects |= SchemaObjects.View;              break;
						case "foreign-key"       : objects |= SchemaObjects.ForeignKey;        break;
						case "stored-procedure"  : objects |= SchemaObjects.StoredProcedure;   break;
						case "scalar-function"   : objects |= SchemaObjects.ScalarFunction;    break;
						case "table-function"    : objects |= SchemaObjects.TableFunction;     break;
						case "aggregate-function": objects |= SchemaObjects.AggregateFunction; break;
					}
				}
				settings.LoadedObjects = objects;
			}

			// include/exclude schemas
			if (options.Remove(SchemaOptions.IncludedSchemas, out value))
			{
				settings.IncludeSchemas = true;

				foreach (var strVal in (string[])value!)
					settings.Schemas.Add(strVal);
			}
			else if (options.Remove(SchemaOptions.ExcludedSchemas, out value))
			{
				settings.IncludeSchemas = false;

				foreach (var strVal in (string[])value!)
					settings.Schemas.Add(strVal);
			}

			if (options.Remove(SchemaOptions.DefaultSchemas, out value))
			{
				settings.DefaultSchemas = new HashSet<string>();

				foreach (var strVal in (string[])value!)
					settings.DefaultSchemas.Add(strVal);
			}


			// include/exclude catalogs
			if (options.Remove(SchemaOptions.IncludedCatalogs, out value))
			{
				settings.IncludeCatalogs = true;

				foreach (var strVal in (string[])value!)
					settings.Catalogs.Add(strVal);
			}
			else if (options.Remove(SchemaOptions.ExcludedCatalogs, out value))
			{
				settings.IncludeCatalogs = false;

				foreach (var strVal in (string[])value!)
					settings.Catalogs.Add(strVal);
			}

			// simple flags
			if (options.Remove(SchemaOptions.PreferProviderTypes       , out value)) settings.PreferProviderSpecificTypes = (bool)value!;
			if (options.Remove(SchemaOptions.IgnoreDuplicateFKs        , out value)) settings.IgnoreDuplicateForeignKeys  = (bool)value!;
			if (options.Remove(SchemaOptions.IgnoreSystemHistoryTables , out value)) settings.IgnoreSystemHistoryTables   = (bool)value!;
			if (options.Remove(SchemaOptions.UseSafeSchemaLoadOnly     , out value)) settings.UseSafeSchemaLoad           = (bool)value!;
			if (options.Remove(SchemaOptions.LoadDatabaseName          , out value)) settings.LoadDatabaseName            = (bool)value!;
			if (options.Remove(SchemaOptions.LoadProcedureSchema       , out value)) settings.LoadProceduresSchema        = (bool)value!;
			if (options.Remove(SchemaOptions.EnableSqlServerReturnValue, out value)) settings.EnableSqlServerReturnValue  = (bool)value!;

			// table/view filter delegate
			var         includeTables = false;
			var         includeViews  = false;
			NameFilter? tableFilter   = null;
			NameFilter? viewFilter    = null;
			if (options.Remove(SchemaOptions.IncludedTables, out value))
			{
				includeTables = true;
				tableFilter   = (NameFilter)value!;
			}
			else if (options.Remove(SchemaOptions.ExcludedTables, out value))
			{
				includeTables = false;
				tableFilter   = (NameFilter)value!;
			}
			if (options.Remove(SchemaOptions.IncludedViews, out value))
			{
				includeViews = true;
				viewFilter   = (NameFilter)value!;
			}
			else if (options.Remove(SchemaOptions.ExcludedViews, out value))
			{
				includeViews = false;
				viewFilter   = (NameFilter)value!;
			}

			if (tableFilter != null || viewFilter != null)
			{
				settings.LoadTableOrView = (name, isView) =>
				{
					if (!isView && tableFilter != null) return tableFilter.ApplyTo(name.Schema, name.Name) == includeTables;
					if (isView  && viewFilter  != null) return viewFilter .ApplyTo(name.Schema, name.Name) == includeViews;
					return true;
				};
			}

			// stored procedure filter
			if (options.Remove(SchemaOptions.IncludedStoredProcedures, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadStoredProcedure = name => filter.ApplyTo(name.Schema, name.Name);
			}
			else if (options.Remove(SchemaOptions.ExcludedStoredProcedures, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadStoredProcedure = name => !filter.ApplyTo(name.Schema, name.Name);
			}

			// procedure schema load filter
			if (options.Remove(SchemaOptions.ProceduresWithSchema, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadProcedureSchema = name => filter.ApplyTo(name.Schema, name.Name);
			}
			else if (options.Remove(SchemaOptions.ProceduresWithoutSchema, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadProcedureSchema = name => !filter.ApplyTo(name.Schema, name.Name);
			}

			// table function filter
			if (options.Remove(SchemaOptions.IncludedTableFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadTableFunction = name => filter.ApplyTo(name.Schema, name.Name);
			}
			else if (options.Remove(SchemaOptions.ExcludedTableFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadTableFunction = name => !filter.ApplyTo(name.Schema, name.Name);
			}

			// scalar function filter
			if (options.Remove(SchemaOptions.IncludedScalarFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadScalarFunction = name => filter.ApplyTo(name.Schema, name.Name);
			}
			else if (options.Remove(SchemaOptions.ExcludedScalarFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadScalarFunction = name => !filter.ApplyTo(name.Schema, name.Name);
			}

			// aggregate function filter
			if (options.Remove(SchemaOptions.IncludedAggregateFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadAggregateFunction = name => filter.ApplyTo(name.Schema, name.Name);
			}
			else if (options.Remove(SchemaOptions.ExcludedAggregateFunctions, out value))
			{
				var filter = (NameFilter)value!;
				settings.LoadAggregateFunction = name => !filter.ApplyTo(name.Schema, name.Name);
			}
		}
	}
}
