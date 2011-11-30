using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TaskDialogInterop
{
	/// <summary>
	/// Provides commands and properties to the emulated TaskDialog view.
	/// </summary>
	public class TaskDialogViewModel : INotifyPropertyChanged
	{
		private static bool? _isInDesignMode;
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Security",
			"CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
			Justification = "The security risk here is neglectible.")]
		internal static bool IsInDesignMode
		{
			get
			{
				if (!_isInDesignMode.HasValue)
				{
					var prop = DesignerProperties.IsInDesignModeProperty;
					_isInDesignMode
						= (bool)DependencyPropertyDescriptor
						.FromProperty(prop, typeof(System.Windows.FrameworkElement))
						.Metadata.DefaultValue;

					// Just to be sure
					if (!_isInDesignMode.Value
						&& System.Diagnostics.Process.GetCurrentProcess().ProcessName.StartsWith
						("devenv", StringComparison.Ordinal))
					{
						_isInDesignMode = true;
					}
				}

				return _isInDesignMode.Value;
			}
		}

		private TaskDialogOptions options;
		private List<TaskDialogButtonData> _normalButtons;
		private List<TaskDialogButtonData> _commandLinks;
		private List<TaskDialogButtonData> _radioButtons;
		private int _dialogResult = -1;
		private bool _expandedInfoVisible;
		private bool _verificationChecked;
		private bool _preventClose;

		private ICommand _commandNormalButton;
		private ICommand _commandCommandLink;
		private ICommand _commandRadioButton;
		private ICommand _commandHyperlink;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDialogViewModel"/> class.
		/// </summary>
		public TaskDialogViewModel()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDialogViewModel"/> class.
		/// </summary>
		/// <param name="options">Options to use.</param>
		public TaskDialogViewModel(TaskDialogOptions options)
			: this()
		{
			this.options = options;

			FixAllButtonLabelAccessKeys();

			// If radio buttons are defined, set the dialog result to the default selected radio
			if (RadioButtons.Count > 0)
			{
				_dialogResult = RadioButtons[DefaultButtonIndex].ID;
			}
		}

		/// <summary>
		/// Gets the window start position.
		/// </summary>
		public System.Windows.WindowStartupLocation StartPosition
		{
			get
			{
				return (options.Owner == null) ? System.Windows.WindowStartupLocation.CenterScreen : System.Windows.WindowStartupLocation.CenterOwner;
			}
		}
		/// <summary>
		/// Gets the window caption.
		/// </summary>
		public string Title
		{
			get
			{
				return String.IsNullOrEmpty(options.Title) ? System.AppDomain.CurrentDomain.FriendlyName : options.Title;
			}
		}
		/// <summary>
		/// Gets the principal text for the dialog.
		/// </summary>
		public string MainInstruction
		{
			get
			{
				return options.MainInstruction;
			}
		}
		/// <summary>
		/// Gets the supplemental text for the dialog.
		/// </summary>
		public string Content
		{
			get
			{
				return options.Content;
			}
		}
		/// <summary>
		/// Gets the expanded info text for the dialog.
		/// </summary>
		public string ExpandedInfo
		{
			get
			{
				return options.ExpandedInfo;
			}
		}
		/// <summary>
		/// Gets the verification text.
		/// </summary>
		public string VerificationText
		{
			get
			{
				return options.VerificationText;
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether expanded info is visible.
		/// </summary>
		public bool ExpandedInfoVisible
		{
			get
			{
				return _expandedInfoVisible;
			}
			set
			{
				if (_expandedInfoVisible == value)
					return;

				_expandedInfoVisible = value;

				RaisePropertyChangedEvent("ExpandedInfoVisible");

				var args = new VistaTaskDialogNotificationArgs();

				args.Notification = VistaTaskDialogNotification.ExpandoButtonClicked;
				args.Expanded = _expandedInfoVisible;

				OnCallback(args);
			}
		}
		/// <summary>
		/// Gets or sets whether the verification checkbox was checked.
		/// </summary>
		public bool VerificationChecked
		{
			get
			{
				return _verificationChecked;
			}
			set
			{
				if (_verificationChecked == value)
					return;

				_verificationChecked = value;

				RaisePropertyChangedEvent("VerificationChecked");

				var args = new VistaTaskDialogNotificationArgs();

				args.Notification = VistaTaskDialogNotification.VerificationClicked;
				args.VerificationFlagChecked = _verificationChecked;

				OnCallback(args);
			}
		}
		/// <summary>
		/// Gets the footer text.
		/// </summary>
		public string FooterText
		{
			get
			{
				return options.FooterText;
			}
		}
		/// <summary>
		/// Gets the type of the main icon.
		/// </summary>
		public VistaTaskDialogIcon MainIconType
		{
			get
			{
				return options.MainIcon;
			}
		}
		/// <summary>
		/// Gets the main icon.
		/// </summary>
		public System.Windows.Media.ImageSource MainIcon
		{
			get
			{
				return ConvertIconToImageSource(options.MainIcon, options.CustomMainIcon, true);
			}
		}
		/// <summary>
		/// Gets the footer icon.
		/// </summary>
		public System.Windows.Media.ImageSource FooterIcon
		{
			get
			{
				return ConvertIconToImageSource(options.FooterIcon, options.CustomFooterIcon, false);
			}
		}
		/// <summary>
		/// Gets the default button index.
		/// </summary>
		public int DefaultButtonIndex
		{
			get
			{
				return options.DefaultButtonIndex ?? 0;
			}
		}
		/// <summary>
		/// Gets a value indicating whether or not Alt-F4, Esc, and the red X
		/// close button should work.
		/// </summary>
		public bool AllowDialogCancellation
		{
			get
			{
				// Alt-F4 should only work if there is a close button or some other
				//normal button marked as IsCancel or its been overridden
				return options.AllowDialogCancellation
					|| NormalButtons.Any(button => button.IsCancel)
					|| ((options.CommandButtons == null || options.CommandButtons.Length == 0)
						&& (options.RadioButtons == null || options.RadioButtons.Length == 0)
						&& (options.CustomButtons == null || options.CustomButtons.Length == 0));
			}
		}
		/// <summary>
		/// Gets the button labels.
		/// </summary>
		public List<TaskDialogButtonData> NormalButtons
		{
			get
			{
				if (_normalButtons == null)
				{
					// Even if no buttons are specified, show a Close button at minimum
					if (CommandLinks.Count == 0
						&& RadioButtons.Count == 0
						&& (options.CustomButtons == null || options.CustomButtons.Length == 0)
						&& options.CommonButtons == TaskDialogCommonButtons.None)
					{
						_normalButtons = new List<TaskDialogButtonData>();
						_normalButtons.Add(new TaskDialogButtonData(
							(int)VistaTaskDialogCommonButtons.Close,
							VistaTaskDialogCommonButtons.Close.ToString(),
							NormalButtonCommand,
							true, true));
					}
					else if (RadioButtons.Count > 0)
					{
						_normalButtons = new List<TaskDialogButtonData>();
						_normalButtons.Add(new TaskDialogButtonData(
							(int)VistaTaskDialogCommonButtons.OK,
							VistaTaskDialogCommonButtons.OK.ToString(),
							NormalButtonCommand,
							true, false));
					}
					else if (options.CustomButtons != null)
					{
						int i = 0;
						_normalButtons =
							(from button in options.CustomButtons
							 select new TaskDialogButtonData(
								TaskDialog.CustomButtonIDOffset + i,
								button,
								NormalButtonCommand,
								DefaultButtonIndex == i++,
								button.Contains(VistaTaskDialogCommonButtons.Cancel.ToString()) || button.Contains(VistaTaskDialogCommonButtons.Close.ToString())))
							.ToList();
					}
					else if (options.CommonButtons != TaskDialogCommonButtons.None)
					{
						int i = 0;
						VistaTaskDialogCommonButtons comBtns = TaskDialog.ConvertCommonButtons(options.CommonButtons);
						_normalButtons =
							(from button in Enum.GetValues(typeof(VistaTaskDialogCommonButtons)).Cast<int>()
							 where button != (int)VistaTaskDialogCommonButtons.None
								&& comBtns.HasFlag((VistaTaskDialogCommonButtons)button)
							 select TaskDialog.ConvertCommonButton(
								(VistaTaskDialogCommonButtons)button,
								NormalButtonCommand,
								DefaultButtonIndex == i++,
								(VistaTaskDialogCommonButtons)button == VistaTaskDialogCommonButtons.Cancel || (VistaTaskDialogCommonButtons)button == VistaTaskDialogCommonButtons.Close))
							.ToList();
					}
					else
					{
						_normalButtons = new List<TaskDialogButtonData>();
					}
				}

				return _normalButtons;
			}
		}
		/// <summary>
		/// Gets the command link labels.
		/// </summary>
		public List<TaskDialogButtonData> CommandLinks
		{
			get
			{
				if (_commandLinks == null)
				{
					if (options.CommandButtons == null || options.CommandButtons.Length == 0)
					{
						_commandLinks = new List<TaskDialogButtonData>();
					}
					else
					{
						int i = 0;
						_commandLinks = (from button in options.CommandButtons
										   select new TaskDialogButtonData(
											   TaskDialog.CommandButtonIDOffset + i,
											   button,
											   CommandLinkCommand,
											   DefaultButtonIndex == i++,
											   false))
										  .ToList();
					}
				}

				return _commandLinks;
			}
		}
		/// <summary>
		/// Gets the radio button labels.
		/// </summary>
		public List<TaskDialogButtonData> RadioButtons
		{
			get
			{
				if (_radioButtons == null)
				{
					// If command buttons are defined, ignore any radio buttons (unless design mode)
					if ((!IsInDesignMode && CommandLinks.Count > 0)
						|| options.RadioButtons == null || options.RadioButtons.Length == 0)
					{
						_radioButtons = new List<TaskDialogButtonData>();
					}
					else
					{
						int i = 0;
						_radioButtons = (from button in options.RadioButtons
										 select new TaskDialogButtonData(
											 TaskDialog.RadioButtonIDOffset + i,
											 button,
											 RadioButtonCommand,
											 DefaultButtonIndex == i++,
											 false))
										.ToList();
					}
				}

				return _radioButtons;
			}
		}
		/// <summary>
		/// Gets the value of the button, command, or radio that was ultimately chosen.
		/// </summary>
		public int DialogResult
		{
			get
			{
				return _dialogResult;
			}
		}

		/// <summary>
		/// Gets the command associated with custom and common buttons.
		/// </summary>
		public ICommand NormalButtonCommand
		{
			get
			{
				if (_commandNormalButton == null)
				{
					_commandNormalButton = new RelayCommand<int>((i) =>
						{
							if (RadioButtons.Count == 0)
							{
								_dialogResult = i;
							}

							var args = new VistaTaskDialogNotificationArgs();

							args.Notification = VistaTaskDialogNotification.ButtonClicked;
							args.ButtonId = i;

							OnCallback(args);
						});
				}

				return _commandNormalButton;
			}
		}
		/// <summary>
		/// Gets the command associated with command links.
		/// </summary>
		public ICommand CommandLinkCommand
		{
			get
			{
				if (_commandCommandLink == null)
				{
					_commandCommandLink = new RelayCommand<int>((i) =>
						{
							_dialogResult = i;
						});
				}

				return _commandCommandLink;
			}
		}
		/// <summary>
		/// Gets the command associated with radio buttons.
		/// </summary>
		public ICommand RadioButtonCommand
		{
			get
			{
				if (_commandRadioButton == null)
				{
					_commandRadioButton = new RelayCommand<int>((i) =>
						{
							_dialogResult = i;

							var args = new VistaTaskDialogNotificationArgs();

							args.Notification = VistaTaskDialogNotification.RadioButtonClicked;
							args.ButtonId = i;

							OnCallback(args);
						});
				}

				return _commandRadioButton;
			}
		}
		/// <summary>
		/// Gets the command associated with hyperlinks.
		/// </summary>
		public ICommand HyperlinkCommand
		{
			get
			{
				if (_commandHyperlink == null)
				{
					_commandHyperlink = new RelayCommand<string>((uri) =>
						{
							var args = new VistaTaskDialogNotificationArgs();

							args.Notification = VistaTaskDialogNotification.HyperlinkClicked;
							args.Hyperlink = uri;

							OnCallback(args);
						});
				}

				return _commandHyperlink;
			}
		}

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Returns a value indicating whether or not the dialog should cancel a closing event.
		/// </summary>
		/// <returns><c>true</c> if dialog closing should be canceled; otherwise, <c>false</c></returns>
		public bool ShouldCancelClosing()
		{
			return _preventClose;
		}
		/// <summary>
		/// Notifies any callback handlers that the dialog has been constructed but not yet shown.
		/// </summary>
		public void NotifyConstructed()
		{
			var args = new VistaTaskDialogNotificationArgs();

			args.Notification = VistaTaskDialogNotification.DialogConstructed;

			OnCallback(args);
		}
		/// <summary>
		/// Notifies any callback handlers that the dialog has been created but not yet shown.
		/// </summary>
		public void NotifyCreated()
		{
			var args = new VistaTaskDialogNotificationArgs();

			args.Notification = VistaTaskDialogNotification.Created;

			OnCallback(args);
		}
		/// <summary>
		/// Notifies any callback handlers that the dialog is destroyed.
		/// </summary>
		public void NotifyClosed()
		{
			var args = new VistaTaskDialogNotificationArgs();

			args.Notification = VistaTaskDialogNotification.Destroyed;

			OnCallback(args);
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event for the given property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected void RaisePropertyChangedEvent(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
		protected void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}
		/// <summary>
		/// Raises a callback.
		/// </summary>
		/// <param name="e">The <see cref="VistaTaskDialogNotificationArgs"/> instance containing the event data.</param>
		protected void OnCallback(VistaTaskDialogNotificationArgs e)
		{
			if (options.Callback != null)
			{
				HandleCallbackReturn(e, options.Callback(options, e, options.CallbackData));
			}
		}

		private void HandleCallbackReturn(VistaTaskDialogNotificationArgs e, bool returnValue)
		{
			switch (e.Notification)
			{
				default: // all others
					// Return value ignored according to MSDN
					break;
				case VistaTaskDialogNotification.ButtonClicked:
					// TRUE : prevent dialog from closing
					_preventClose = returnValue;
					break;
				case VistaTaskDialogNotification.Timer:
					// TRUE : reset tickcount
					// Timer functionality not exposed right now, though, so do nothing
					break;
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		private System.Windows.Media.ImageSource ConvertIconToImageSource(VistaTaskDialogIcon icon, Icon customIcon, bool isLarge)
		{
			System.Windows.Media.ImageSource iconSource = null;
			System.Drawing.Icon sysIcon = null;
			System.Drawing.Bitmap altBmp = null;

			try
			{
				switch (icon)
				{
					default:
					case VistaTaskDialogIcon.None:
						break;
					case VistaTaskDialogIcon.Information:
						sysIcon = SystemIcons.Information;
						break;
					case VistaTaskDialogIcon.Warning:
						sysIcon = SystemIcons.Warning;
						break;
					case VistaTaskDialogIcon.Error:
						sysIcon = SystemIcons.Error;
						break;
					case VistaTaskDialogIcon.Shield:
						if (isLarge)
						{
							altBmp = Properties.Resources.shield_32;
						}
						else
						{
							altBmp = Properties.Resources.shield_16;
						}
						break;
				}

				// Custom Icons always take priority
				if (customIcon != null)
				{
					iconSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
						customIcon.Handle,
						System.Windows.Int32Rect.Empty,
						System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
				}
				else if (sysIcon != null)
				{
					iconSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
						sysIcon.Handle,
						System.Windows.Int32Rect.Empty,
						System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
				}
				else if (altBmp != null)
				{
					iconSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
						altBmp.GetHbitmap(),
						IntPtr.Zero,
						System.Windows.Int32Rect.Empty,
						System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
				}
			}
			finally
			{
				// Not responsible for disposing of custom icons

				if (sysIcon != null)
					sysIcon.Dispose();
				if (altBmp != null)
					altBmp.Dispose();
			}

			return iconSource;
		}
		private void FixAllButtonLabelAccessKeys()
		{
			options.CommandButtons = FixLabelAccessKeys(options.CommandButtons);
			options.RadioButtons = FixLabelAccessKeys(options.RadioButtons);
			options.CustomButtons = FixLabelAccessKeys(options.CustomButtons);
		}
		private string[] FixLabelAccessKeys(string[] labels)
		{
			if (labels == null || labels.Length == 0)
				return labels;

			string[] fixedLabels = new string[labels.Length];

			for (int i = 0; i < labels.Length; i++)
			{
				// WPF uses underscores for denoting access keys, whereas TaskDialog
				//expects ampersands
				// First, we escape any existing underscores by doubling them, so that WPF
				//will render them normally
				// Last, we replace any ampersands with underscores
				fixedLabels[i] = labels[i].Replace("_", "__").Replace("&", "_");
			}

			return fixedLabels;
		}
	}
}
