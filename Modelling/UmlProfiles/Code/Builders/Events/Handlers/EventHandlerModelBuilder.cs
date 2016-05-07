﻿using System;
using System.Linq;
using Microsoft.VisualStudio.ArchitectureTools.Extensibility.Uml;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Uml.Classes;

namespace Cqrs.Modelling.UmlProfiles.Builders.Events.Handlers
{
	public abstract class EventHandlerModelBuilder : ModelBuilder
	{
		protected EventHandlerModelBuilder(string operationModeName, bool copyAttributes = true)
		{
			CopyAttributes = copyAttributes;
			OperationModeName = operationModeName;
		}

		protected virtual string GetPropertyInstanceName()
		{
			return string.Format("Build{0}Command", OperationModeName);
		}

		protected string OperationModeName { get; private set; }

		protected bool CopyAttributes { get; private set; }

		protected override bool ShouldCreateModel(Store store, PropertyInstance propertyInstance)
		{
			return propertyInstance.Name == GetPropertyInstanceName() && propertyInstance.Value.ToLowerInvariant() == "true";
		}

		protected override bool ShouldDeleteModel(Store store, PropertyInstance propertyInstance)
		{
			return propertyInstance.Name == GetPropertyInstanceName() && propertyInstance.Value.ToLowerInvariant() == "false";
		}

		protected override void CreateModel(Store store, PropertyInstance propertyInstance)
		{
			IClass propertyInstanceModelClass = GetPropertyInstanceModelClass(store, propertyInstance);
			// See https://msdn.microsoft.com/en-us/library/cc512845.aspx#elements for copy/paste
			using (var transaction = store.TransactionManager.BeginTransaction())
			{
				try
				{
					var modulePackage = (Package)propertyInstanceModelClass.Package;
					var eventsPackage = modulePackage.NestedPackages.SingleOrDefault(package => package.Name == "Events") as Package;
					if (eventsPackage == null)
					{
						eventsPackage = (Package)modulePackage.CreatePackage();
						eventsPackage.Name = "Events";
					}

					var eventHandlersPackage = eventsPackage.NestedPackages.SingleOrDefault(package => package.Name == "Handlers") as Package;
					if (eventHandlersPackage == null)
					{
						eventHandlersPackage = (Package)eventsPackage.CreatePackage();
						eventHandlersPackage.Name = "Handlers";
					}

					var entityPackage = modulePackage.NestedPackages.SingleOrDefault(package => package.Name == "Entities") as Package;
					if (entityPackage == null)
					{
						entityPackage = (Package)modulePackage.CreatePackage();
						entityPackage.Name = "Entities";
					}


					string className = string.Format("{1}{0}dEventHandler", OperationModeName, propertyInstanceModelClass.Name);
					var clonedClass = eventHandlersPackage.OwnedElements
						// This bit filters out the associations
						.Where(element => element.AppliedStereotypes.Any())
						.Cast<Class>()
						.SingleOrDefault(element => CSharpHelper.ClassifierName(element) == className);
					if (clonedClass == null)
					{
						clonedClass = (Class)eventHandlersPackage.CreateClass();
						clonedClass.Name = className;
					}
					AddStereotypeInstanceIfMissingRefreshOtherwise(clonedClass, propertyInstanceModelClass, "AutoGenerated");
					IStereotypeInstance eventHandler = AddStereotypeInstanceIfMissingRefreshOtherwise(clonedClass, propertyInstanceModelClass, "EventHandler");


					// Copy Properties
//					if (CopyAttributes)
//						foreach (IProperty ownedAttribute in propertyInstanceModelClass.OwnedAttributes)
//							AddAttributeIfMissingRefreshOtherwise(clonedClass, ownedAttribute);

					var eventClass = eventsPackage.OwnedElements
						// This bit filters out the associations
						.Where(element => element.AppliedStereotypes.Any())
						.Cast<Class>()
						.SingleOrDefault(element => CSharpHelper.ClassifierName(element) == string.Format("{1}{0}d", OperationModeName, propertyInstanceModelClass.Name));

					// Create Association
					string operationName = string.Format("{0}::{1}Entity", entityPackage.Name, propertyInstanceModelClass.Name);
					AddAssociationIfMissingRefreshOtherwise(eventClass, clonedClass, operationName);

					// Create stub operation on propertyInstanceModelClass
					var operation = AddOperationIfMissingRefreshOtherwise(clonedClass, "Handle");
					AddStereotypeInstanceIfMissingRefreshOtherwise(operation, operation, "AutoGenerated");
					/*
					IStereotypeInstance stereoType = AddStereotypeInstanceIfMissingRefreshOtherwise(operation, operation, "AggregateRootMethod");
					stereoType.PropertyInstances.Single(property => property.Name == "AggregateRootMethodType").Value = "Simple";
					stereoType.PropertyInstances.Single(property => property.Name == "EventName").Value = string.Format("{1}{0}d", OperationModeName, propertyInstanceModelClass.Name);
					*/

					eventHandler.PropertyInstances.Single(property => property.Name == "EntityName").Value = operationName;
					eventHandler.PropertyInstances.Single(property => property.Name == "EventName").Value = string.Format("{1}{0}d", OperationModeName, propertyInstanceModelClass.Name);

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
				}
			}
		}

		protected override void DeleteModel(Store store, PropertyInstance propertyInstance)
		{
			// ElementFactory elementFactory = GetMatchingElementFactory(store, propertyInstance);
			IClass propertyInstanceModelClass = GetPropertyInstanceModelClass(store, propertyInstance);
			// See https://msdn.microsoft.com/en-us/library/cc512845.aspx#elements for copy/paste
			using (var transaction = store.TransactionManager.BeginTransaction())
			{
				try
				{
					var modulePackage = (Package)propertyInstanceModelClass.Package;
					var eventsPackage = modulePackage.NestedPackages.SingleOrDefault(package => package.Name == "Events") as Package;
					if (eventsPackage == null)
						return;

					var eventHandlersPackage = eventsPackage.NestedPackages.SingleOrDefault(package => package.Name == "Handlers") as Package;
					if (eventHandlersPackage == null)
						return;

					string className = string.Format("{1}{0}dEventHandler", OperationModeName, propertyInstanceModelClass.Name);
					var clonedClass = eventHandlersPackage.OwnedElements
						// This bit filters out the associations
						.Where(element => element.AppliedStereotypes.Any())
						.Cast<Class>()
						.SingleOrDefault(element => CSharpHelper.ClassifierName(element) == className);
					if (clonedClass == null)
						return;
					clonedClass.Delete();

					transaction.Commit();
				}
				catch (Exception)
				{
					transaction.Rollback();
				}
			}
		}
	}
}