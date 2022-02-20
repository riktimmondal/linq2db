﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1572, 1591
#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;

namespace Tests.T4.Wpf
{
	[CustomValidation(typeof(ViewModel.CustomValidator), "ValidateConditionalProp")]
	[CustomValidation(typeof(ViewModel.CustomValidator), "ValidateNotifiedProp3")]
	public partial class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
	{
		#region NotifiedProp1 : double

		private double _notifiedProp1;
		public  double  NotifiedProp1
		{
			get { return _notifiedProp1; }
			set
			{
				if (_notifiedProp1 != value)
				{
					BeforeNotifiedProp1Changed(value);
					_notifiedProp1 = value;
					AfterNotifiedProp1Changed();

					OnNotifiedProp1Changed();
					OnNotifiedBrush1Changed();
				}
			}
		}

		#region INotifyPropertyChanged support

		partial void BeforeNotifiedProp1Changed(double newValue);
		partial void AfterNotifiedProp1Changed ();

		public const string NameOfNotifiedProp1 = "NotifiedProp1";

		private static readonly PropertyChangedEventArgs _notifiedProp1ChangedEventArgs = new PropertyChangedEventArgs(NameOfNotifiedProp1);

		private void OnNotifiedProp1Changed()
		{
			OnPropertyChanged(_notifiedProp1ChangedEventArgs);
		}

		#endregion

		#endregion

		#region NotifiedProp2 : int

		private int _notifiedProp2 = 500;
		public  int  NotifiedProp2
		{
			get { return _notifiedProp2; }
			set
			{
				if (_notifiedProp2 != value)
				{
					BeforeNotifiedProp2Changed(value);
					_notifiedProp2 = value;
					AfterNotifiedProp2Changed();

					OnNotifiedProp1Changed();
					OnNotifiedProp2Changed();
				}
			}
		}

		#region INotifyPropertyChanged support

		partial void BeforeNotifiedProp2Changed(int newValue);
		partial void AfterNotifiedProp2Changed ();

		public const string NameOfNotifiedProp2 = "NotifiedProp2";

		private static readonly PropertyChangedEventArgs _notifiedProp2ChangedEventArgs = new PropertyChangedEventArgs(NameOfNotifiedProp2);

		private void OnNotifiedProp2Changed()
		{
			OnPropertyChanged(_notifiedProp2ChangedEventArgs);
		}

		#endregion

		#endregion

		#region NotifiedBrush1 : Brush

		public Brush NotifiedBrush1
		{
			get { return GetBrush(); }
		}

		#region INotifyPropertyChanged support

		public const string NameOfNotifiedBrush1 = "NotifiedBrush1";

		private static readonly PropertyChangedEventArgs _notifiedBrush1ChangedEventArgs = new PropertyChangedEventArgs(NameOfNotifiedBrush1);

		private void OnNotifiedBrush1Changed()
		{
			OnPropertyChanged(_notifiedBrush1ChangedEventArgs);
		}

		#endregion

		#endregion

		#region ConditionalProp : string

#if DEBUG

		private string _conditionalProp = string.Empty;
		public  string  ConditionalProp
		{
			get { return _conditionalProp; }
			set
			{
				if (_conditionalProp != value)
				{
					BeforeConditionalPropChanged(value);
					_conditionalProp = value;
					AfterConditionalPropChanged();

					OnConditionalPropChanged();
				}
			}
		}

		#region INotifyPropertyChanged support

		partial void BeforeConditionalPropChanged(string newValue);
		partial void AfterConditionalPropChanged ();

		public const string NameOfConditionalProp = "ConditionalProp";

		private static readonly PropertyChangedEventArgs _conditionalPropChangedEventArgs = new PropertyChangedEventArgs(NameOfConditionalProp);

		private void OnConditionalPropChanged()
		{
			OnPropertyChanged(_conditionalPropChangedEventArgs);
		}

		#endregion
#endif


		#endregion

		#region NotifiedProp3 : string

		private string _notifiedProp3 = string.Empty;
		public  string  NotifiedProp3
		{
			get { return _notifiedProp3; }
			set
			{
				if (_notifiedProp3 != value)
				{
					BeforeNotifiedProp3Changed(value);
					_notifiedProp3 = value;
					AfterNotifiedProp3Changed();

					OnNotifiedProp3Changed();
				}
			}
		}

		#region INotifyPropertyChanged support

		partial void BeforeNotifiedProp3Changed(string newValue);
		partial void AfterNotifiedProp3Changed ();

		public const string NameOfNotifiedProp3 = "NotifiedProp3";

		private static readonly PropertyChangedEventArgs _notifiedProp3ChangedEventArgs = new PropertyChangedEventArgs(NameOfNotifiedProp3);

		private void OnNotifiedProp3Changed()
		{
			OnPropertyChanged(_notifiedProp3ChangedEventArgs);
		}

		#endregion

		#endregion

		#region INotifyPropertyChanged support

		[field : NonSerialized]
		public virtual event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			var propertyChanged = PropertyChanged;

			if (propertyChanged != null)
			{
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs arg)
		{
			var propertyChanged = PropertyChanged;

			if (propertyChanged != null)
			{
				propertyChanged(this, arg);
			}
		}

		#endregion

		#region Validation

		[field : NonSerialized]
		public int _isValidCounter;

		public static partial class CustomValidator
		{
			public static bool IsValid(ViewModel obj)
			{
				try
				{
					obj._isValidCounter++;

					#if DEBUG
					var flag0 = ValidationResult.Success == ValidateConditionalProp(obj, obj.ConditionalProp);
					#else
					var flag0 = true;
					#endif
					var flag1 = ValidationResult.Success == ValidateNotifiedProp3(obj, obj.NotifiedProp3);

					return flag0 && flag1;
				}
				finally
				{
					obj._isValidCounter--;
				}
			}

#if DEBUG

			public static ValidationResult? ValidateConditionalProp(ViewModel obj, string value)
			{
				var list = new List<ValidationResult>();

				Validator.TryValidateProperty(
					value,
					new ValidationContext(obj, null, null) { MemberName = NameOfConditionalProp }, list);

				obj.ValidateConditionalProp(value, list);

				if (list.Count > 0)
				{
					foreach (var result in list)
						foreach (var name in result.MemberNames)
							obj.AddError(name, result.ErrorMessage);

					return list[0];
				}

				obj.RemoveError(NameOfConditionalProp);

				return ValidationResult.Success;
			}

#endif

			public static ValidationResult? ValidateNotifiedProp3(ViewModel obj, string value)
			{
				var list = new List<ValidationResult>();

				Validator.TryValidateProperty(
					value,
					new ValidationContext(obj, null, null) { MemberName = NameOfNotifiedProp3 }, list);

				obj.ValidateNotifiedProp3(value, list);

				if (list.Count > 0)
				{
					foreach (var result in list)
						foreach (var name in result.MemberNames)
							obj.AddError(name, result.ErrorMessage);

					return list[0];
				}

				obj.RemoveError(NameOfNotifiedProp3);

				return ValidationResult.Success;
			}
		}

#if DEBUG
		partial void ValidateConditionalProp(string value, List<ValidationResult> validationResults);
#endif
		partial void ValidateNotifiedProp3  (string value, List<ValidationResult> validationResults);

		#endregion

		#region INotifyDataErrorInfo support

		[field : NonSerialized]
		public virtual event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

		[field : NonSerialized]
		private readonly Dictionary<string,List<string>> _validationErrors = new Dictionary<string,List<string>>();

		public void AddError(string propertyName, string error)
		{
			List<string> errors;

			if (!_validationErrors.TryGetValue(propertyName, out errors))
			{
				_validationErrors[propertyName] = new List<string> { error };
			}
			else if (!errors.Contains(error))
			{
				errors.Add(error);
			}
			else
				return;

			OnErrorsChanged(propertyName);
		}

		public void RemoveError(string propertyName)
		{
			List<string> errors;

			if (_validationErrors.TryGetValue(propertyName, out errors) && errors.Count > 0)
			{
				_validationErrors.Clear();
				OnErrorsChanged(propertyName);
			}
		}

		protected void OnErrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null)
			{
				if (System.Windows.Application.Current.Dispatcher.CheckAccess())
					ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
				else
					System.Windows.Application.Current.Dispatcher.BeginInvoke(
						(Action)(() => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName))));
			}
		}

		public IEnumerable? GetErrors(string? propertyName)
		{
			List<string> errors;
			return propertyName != null && _validationErrors.TryGetValue(propertyName, out errors) ? errors : null;
		}

		public bool HasErrors
		{
			get { return _validationErrors.Values.Any(e => e.Count > 0); }
		}

		#endregion
	}
}
