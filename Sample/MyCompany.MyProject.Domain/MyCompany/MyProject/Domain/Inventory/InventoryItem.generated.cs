﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#region Copyright
// -----------------------------------------------------------------------
// <copyright company="cdmdotnet Limited">
//     Copyright cdmdotnet Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
#endregion
using Cqrs.Domain;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cqrs.Configuration;
using Cqrs.Logging;
using MyCompany.MyProject.Domain.Inventory.Events;

namespace MyCompany.MyProject.Domain.Inventory
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.523.412")]
	public  partial class InventoryItem : AggregateRoot<Cqrs.Authentication.ISingleSignOnToken>
	{
		public Guid Rsn
		{
			get { return Id; }
			private set { Id = value; }
		}

		public bool IsLogicallyDeleted {get; set;}

		#region Implementation of IMessageWithAuthenticationToken<Cqrs.Authentication.ISingleSignOnToken>

		public Cqrs.Authentication.ISingleSignOnToken AuthenticationToken { get; set; }

		#endregion

		protected IDependencyResolver DependencyResolver { get; private set; }

		protected ILog Log { get; private set; }

		public bool Activated { get; private set; }

// ReSharper disable UnusedMember.Local
		/// <summary>
		/// A constructor for the <see cref="Cqrs.Domain.Factories.IAggregateFactory"/>
		/// </summary>
		private InventoryItem()
		{
		}

		/// <summary>
		/// A constructor for the <see cref="Cqrs.Domain.Factories.IAggregateFactory"/>
		/// </summary>
		private InventoryItem(IDependencyResolver dependencyResolver, ILog log)
		{
			DependencyResolver = dependencyResolver;
			Log = log;
		}
// ReSharper restore UnusedMember.Local

		public InventoryItem(IDependencyResolver dependencyResolver, ILog log, Guid rsn)
		{
			DependencyResolver = dependencyResolver;
			Log = log;
			Rsn = rsn;
		}

		public virtual void ChangeName(string newName)
		{
			Log.LogDebug("Entered", "InventoryItem/ChangeName");
			OnChangeName(newName);
			Log.LogDebug("Exited", "InventoryItem/ChangeName");
		}
		partial void OnChangeName(string newName);

		public virtual void Remove(long count)
		{
			Log.LogDebug("Entered", "InventoryItem/Remove");
			OnRemove(count);
			Log.LogDebug("Exited", "InventoryItem/Remove");
		}
		partial void OnRemove(long count);

		public virtual void CheckIn(long count)
		{
			Log.LogDebug("Entered", "InventoryItem/CheckIn");
			OnCheckIn(count);
			Log.LogDebug("Exited", "InventoryItem/CheckIn");
		}
		partial void OnCheckIn(long count);

		public virtual void Deactivate()
		{
			Log.LogDebug("Entered", "InventoryItem/Deactivate");
			OnDeactivate();
			Log.LogDebug("Exited", "InventoryItem/Deactivate");
		}
		partial void OnDeactivate();

		public virtual void CreateInventoryItem(string name)
		{
			Log.LogDebug("Entered", "InventoryItem/CreateInventoryItem");
			Log.LogDebug("Applying event", "InventoryItem/CreateInventoryItem/InventoryItemCreated");
			ApplyChange(new InventoryItemCreated(Rsn, name));
			Log.LogDebug("Exited", "InventoryItem/CreateInventoryItem");
		}
		private void Apply(InventoryItemCreated @event)
		{
			OnApply(@event);
		}
		partial void OnApply(InventoryItemCreated @event);

	}
}
