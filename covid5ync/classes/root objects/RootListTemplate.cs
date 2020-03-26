using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace isosoft.root
{
	

	public class RootListTemplate<T> : List<T>, INotifyPropertyChanged, INotifyCollectionChanged
	{
		public delegate void CollectionChangedHandler(object sender, List<T> list);

		public event PropertyChangedEventHandler			PropertyChanged;
		public event NotifyCollectionChangedEventHandler	CollectionChanged;
		public event CollectionChangedHandler				CollectionChangedInternal;

		//lock object for synchronization;
		protected static object			_syncLock		= new object();

		public RootListTemplate() : base()
		{
		}
		
		protected void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
		{
			if (property == null || PropertyChanged == null)
				return;

			var expression = property.Body as MemberExpression;

			if (expression == null)
				return;

			string name = expression.Member.Name;	// property.Name;
			NotifyPropertyChanged(name);
		}

		protected void NotifyPropertyChanged([CallerMemberName] string property_name = "")
		{
			if( PropertyChanged == null || string.IsNullOrEmpty( property_name))
				return;

			PropertyChanged.Invoke( this, new PropertyChangedEventArgs( property_name));
		}

		protected void NotifyCollectionChanged( object sender, NotifyCollectionChangedAction action, T changedItem)
		{
			//NotifyCollectionReset( sender);

			if( CollectionChangedInternal != null)
				CollectionChangedInternal.Invoke( sender, this);

			if(CollectionChanged == null)
				return;

			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs( action, changedItem));
		}


		protected void NotifyCollectionChanged( object sender, NotifyCollectionChangedAction action, IEnumerable<T> changedItems)
		{
			//NotifyCollectionReset( sender);

			if( CollectionChangedInternal != null)
				CollectionChangedInternal.Invoke( sender, this);

			if(CollectionChanged == null)
				return;

			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs( action, changedItems));
		}


		protected void NotifyCollectionReset( object sender)
		{
			if(CollectionChanged != null)
			{
				try
				{
					CollectionChanged.Invoke(sender, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
				catch (Exception)
				{
					return;
				}
			}
		}


		public virtual new bool Remove(T item)
		{
			if( base.Remove(item))
			{
				lock(_syncLock)
				{
					NotifyCollectionChanged(this, NotifyCollectionChangedAction.Remove, item);
				}
				return true;
			}
			return false;
		}


		public virtual new void Add(T item)
		{
			if(item == null)
				return;

			lock(_syncLock)
			{
				base.Add(item);
				NotifyCollectionChanged(this, NotifyCollectionChangedAction.Add, item);
			}
		}

		public virtual void Clear(bool notifyColectionChange)
		{
			lock (_syncLock)
			{
				base.Clear();
				if(notifyColectionChange)
					NotifyCollectionReset(this);
			}
		}

		public virtual new void AddRange(IEnumerable<T> list)
		{
			if (list == null)
				return;

			lock (_syncLock)
			{
				base.AddRange(list);
				NotifyCollectionChanged(this, NotifyCollectionChangedAction.Add, list);
			}
		}

		public virtual bool CopyOther(IEnumerable<T> other)
		{
			if(other == null || other == this)
				return false;

			lock(_syncLock)
			{
				this.Clear();

				foreach (T item in other)
					this.Add(item);
			}

			return true;
		}

	}
}
