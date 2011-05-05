using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TaskDialogInterop
{
	public partial class TaskDialog
	{
		internal const int CommandButtonIDOffset = 2000;
		internal const int RadioButtonIDOffset = 1000;
		internal const int CustomButtonIDOffset = 500;

		/// <summary>
		/// Forces the WPF-based TaskDialog window instead of using native calls.
		/// </summary>
		public static bool ForceEmulationMode { get; set; }

		/// <summary>
		/// Occurs when a task dialog is about to show.
		/// </summary>
		/// <remarks>
		/// Use this event for both notification and modification of all task
		/// dialog showings. Changes made to the configuration options will be
		/// persisted.
		/// </remarks>
		public static event TaskDialogShowingEventHandler Showing;
		/// <summary>
		/// Occurs when a task dialog has been closed.
		/// </summary>
		public static new event TaskDialogClosedEventHandler Closed;

		/// <summary>
		/// Displays a task dialog with the given configuration options.
		/// </summary>
		/// <param name="options">
		/// A <see cref="T:TaskDialogInterop.TaskDialogOptions"/> that specifies the
		/// configuration options for the dialog.
		/// </param>
		/// <returns>
		/// A <see cref="T:TaskDialogInterop.TaskDialogResult"/> value that specifies
		/// which button is clicked by the user.
		/// </returns>
		public static TaskDialogResult Show(TaskDialogOptions options)
		{
			TaskDialogResult result = null;

			// Make a copy since we'll let Showing event possibly modify them
			TaskDialogOptions configOptions = options;

			OnShowing(new TaskDialogShowingEventArgs(ref configOptions));

			if (VistaTaskDialog.IsAvailableOnThisOS && !ForceEmulationMode)
			{
				try
				{
					result = ShowTaskDialog(configOptions);
				}
				catch (EntryPointNotFoundException)
				{
					// This can happen on some machines, usually when running Vista/7 x64
					// When it does, we'll work around the issue by forcing emulated mode
					// http://www.codeproject.com/Messages/3257715/How-to-get-it-to-work-on-Windows-7-64-bit.aspx
					ForceEmulationMode = true;
					result = ShowEmulatedTaskDialog(configOptions);
				}
			}
			else
			{
				result = ShowEmulatedTaskDialog(configOptions);
			}

			OnClosed(new TaskDialogClosedEventArgs(result));

			return result;
		}

		/// <summary>
		/// Displays a task dialog that has a message and that returns a result.
		/// </summary>
		/// <param name="owner">
		/// The <see cref="T:System.Windows.Window"/> that owns this dialog.
		/// </param>
		/// <param name="messageText">
		/// A <see cref="T:System.String"/> that specifies the text to display.
		/// </param>
		/// <returns>
		/// A <see cref="T:TaskDialogInterop.TaskDialogSimpleResult"/> value that
		/// specifies which button is clicked by the user.
		/// </returns>
		public static TaskDialogSimpleResult ShowMessage(Window owner, string messageText)
		{
			TaskDialogOptions options = TaskDialogOptions.Default;

			options.Owner = owner;
			options.Content = messageText;
			options.CommonButtons = TaskDialogCommonButtons.Close;

			return Show(options).Result;
		}
		/// <summary>
		/// Displays a task dialog that has a message and that returns a result.
		/// </summary>
		/// <param name="owner">
		/// The <see cref="T:System.Windows.Window"/> that owns this dialog.
		/// </param>
		/// <param name="messageText">
		/// A <see cref="T:System.String"/> that specifies the text to display.
		/// </param>
		/// <param name="caption">
		/// A <see cref="T:System.String"/> that specifies the title bar
		/// caption to display.
		/// </param>
		/// <returns>
		/// A <see cref="T:TaskDialogInterop.TaskDialogSimpleResult"/> value that
		/// specifies which button is clicked by the user.
		/// </returns>
		public static TaskDialogSimpleResult ShowMessage(Window owner, string messageText, string caption)
		{
			return ShowMessage(owner, messageText, caption, TaskDialogCommonButtons.Close);
		}
		/// <summary>
		/// Displays a task dialog that has a message and that returns a result.
		/// </summary>
		/// <param name="owner">
		/// The <see cref="T:System.Windows.Window"/> that owns this dialog.
		/// </param>
		/// <param name="messageText">
		/// A <see cref="T:System.String"/> that specifies the text to display.
		/// </param>
		/// <param name="caption">
		/// A <see cref="T:System.String"/> that specifies the title bar
		/// caption to display.
		/// </param>
		/// <param name="buttons">
		/// A <see cref="T:TaskDialogInterop.TaskDialogCommonButtons"/> value that
		/// specifies which button or buttons to display.
		/// </param>
		/// <returns>
		/// A <see cref="T:TaskDialogInterop.TaskDialogSimpleResult"/> value that
		/// specifies which button is clicked by the user.
		/// </returns>
		public static TaskDialogSimpleResult ShowMessage(Window owner, string messageText, string caption, TaskDialogCommonButtons buttons)
		{
			return ShowMessage(owner, messageText, caption, buttons, VistaTaskDialogIcon.None);
		}
		/// <summary>
		/// Displays a task dialog that has a message and that returns a result.
		/// </summary>
		/// <param name="owner">
		/// The <see cref="T:System.Windows.Window"/> that owns this dialog.
		/// </param>
		/// <param name="messageText">
		/// A <see cref="T:System.String"/> that specifies the text to display.
		/// </param>
		/// <param name="caption">
		/// A <see cref="T:System.String"/> that specifies the title bar
		/// caption to display.
		/// </param>
		/// <param name="buttons">
		/// A <see cref="T:TaskDialogInterop.TaskDialogCommonButtons"/> value that
		/// specifies which button or buttons to display.
		/// </param>
		/// <param name="icon">
		/// A <see cref="T:TaskDialogInterop.VistaTaskDialogIcon"/> that specifies the
		/// icon to display.
		/// </param>
		/// <returns>
		/// A <see cref="T:TaskDialogInterop.TaskDialogSimpleResult"/> value that
		/// specifies which button is clicked by the user.
		/// </returns>
		public static TaskDialogSimpleResult ShowMessage(Window owner, string messageText, string caption, TaskDialogCommonButtons buttons, VistaTaskDialogIcon icon)
		{
			TaskDialogOptions options = TaskDialogOptions.Default;

			options.Owner = owner;
			options.Title = caption;
			options.Content = messageText;
			options.CommonButtons = buttons;
			options.MainIcon = icon;

			return Show(options).Result;
		}
		/// <summary>
		/// Displays a task dialog that has a message and that returns a result.
		/// </summary>
		/// <param name="owner">
		/// The <see cref="T:System.Windows.Window"/> that owns this dialog.
		/// </param>
		/// <param name="title">
		/// A <see cref="T:System.String"/> that specifies the title bar
		/// caption to display.
		/// </param>
		/// <param name="mainInstruction">
		/// A <see cref="T:System.String"/> that specifies the main text to display.
		/// </param>
		/// <param name="content">
		/// A <see cref="T:System.String"/> that specifies the body text to display.
		/// </param>
		/// <param name="expandedInfo">
		/// A <see cref="T:System.String"/> that specifies the expanded text to display when toggled.
		/// </param>
		/// <param name="verificationText">
		/// A <see cref="T:System.String"/> that specifies the text to display next to a checkbox.
		/// </param>
		/// <param name="footerText">
		/// A <see cref="T:System.String"/> that specifies the footer text to display.
		/// </param>
		/// <param name="buttons">
		/// A <see cref="T:TaskDialogInterop.TaskDialogCommonButtons"/> value that
		/// specifies which button or buttons to display.
		/// </param>
		/// <param name="mainIcon">
		/// A <see cref="T:TaskDialogInterop.VistaTaskDialogIcon"/> that specifies the
		/// main icon to display.
		/// </param>
		/// <param name="footerIcon">
		/// A <see cref="T:TaskDialogInterop.VistaTaskDialogIcon"/> that specifies the
		/// footer icon to display.
		/// </param>
		/// <returns></returns>
		public static TaskDialogSimpleResult ShowMessage(Window owner, string title, string mainInstruction, string content, string expandedInfo, string verificationText, string footerText, TaskDialogCommonButtons buttons, VistaTaskDialogIcon mainIcon, VistaTaskDialogIcon footerIcon)
		{
			TaskDialogOptions options = TaskDialogOptions.Default;

			if (owner != null)
				options.Owner = owner;
			if (!String.IsNullOrEmpty(title))
				options.Title = title;
			if (!String.IsNullOrEmpty(mainInstruction))
				options.MainInstruction = mainInstruction;
			if (!String.IsNullOrEmpty(content))
				options.Content = content;
			if (!String.IsNullOrEmpty(expandedInfo))
				options.ExpandedInfo = expandedInfo;
			if (!String.IsNullOrEmpty(verificationText))
				options.VerificationText = verificationText;
			if (!String.IsNullOrEmpty(footerText))
				options.FooterText = footerText;
			options.CommonButtons = buttons;
			options.MainIcon = mainIcon;
			options.FooterIcon = footerIcon;

			return Show(options).Result;
		}

		internal static VistaTaskDialogCommonButtons ConvertCommonButtons(TaskDialogCommonButtons commonButtons)
		{
			VistaTaskDialogCommonButtons vtdCommonButtons = VistaTaskDialogCommonButtons.None;

			switch (commonButtons)
			{
				default:
				case TaskDialogCommonButtons.None:
					vtdCommonButtons = VistaTaskDialogCommonButtons.None;
					break;
				case TaskDialogCommonButtons.Close:
					vtdCommonButtons = VistaTaskDialogCommonButtons.Close;
					break;
				case TaskDialogCommonButtons.OKCancel:
					vtdCommonButtons = VistaTaskDialogCommonButtons.OK | VistaTaskDialogCommonButtons.Cancel;
					break;
				case TaskDialogCommonButtons.RetryCancel:
					vtdCommonButtons = VistaTaskDialogCommonButtons.Retry | VistaTaskDialogCommonButtons.Cancel;
					break;
				case TaskDialogCommonButtons.YesNo:
					vtdCommonButtons = VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No;
					break;
				case TaskDialogCommonButtons.YesNoCancel:
					vtdCommonButtons = VistaTaskDialogCommonButtons.Yes | VistaTaskDialogCommonButtons.No | VistaTaskDialogCommonButtons.Cancel;
					break;
			}

			return vtdCommonButtons;
		}
		internal static TaskDialogButtonData ConvertCommonButton(VistaTaskDialogCommonButtons commonButton, System.Windows.Input.ICommand command = null, bool isDefault = false, bool isCancel = false)
		{
			int id = 0;

			switch (commonButton)
			{
				default:
				case VistaTaskDialogCommonButtons.None:
					id = (int)TaskDialogSimpleResult.None;
					break;
				case VistaTaskDialogCommonButtons.OK:
					id = (int)TaskDialogSimpleResult.Ok;
					break;
				case VistaTaskDialogCommonButtons.Yes:
					id = (int)TaskDialogSimpleResult.Yes;
					break;
				case VistaTaskDialogCommonButtons.No:
					id = (int)TaskDialogSimpleResult.No;
					break;
				case VistaTaskDialogCommonButtons.Cancel:
					id = (int)TaskDialogSimpleResult.Cancel;
					break;
				case VistaTaskDialogCommonButtons.Retry:
					id = (int)TaskDialogSimpleResult.Retry;
					break;
				case VistaTaskDialogCommonButtons.Close:
					id = (int)TaskDialogSimpleResult.Close;
					break;
			}

			return new TaskDialogButtonData(id, "_" + commonButton.ToString(), command, isDefault, isCancel);
		}

		/// <summary>
		/// Raises the <see cref="E:Showing"/> event.
		/// </summary>
		/// <param name="e">The <see cref="TaskDialogInterop.TaskDialogShowingEventArgs"/> instance containing the event data.</param>
		private static void OnShowing(TaskDialogShowingEventArgs e)
		{
			if (Showing != null)
			{
				Showing(null, e);
			}
		}
		/// <summary>
		/// Raises the <see cref="E:Closed"/> event.
		/// </summary>
		/// <param name="e">The <see cref="TaskDialogInterop.TaskDialogClosedEventArgs"/> instance containing the event data.</param>
		private static void OnClosed(TaskDialogClosedEventArgs e)
		{
			if (Closed != null)
			{
				Closed(null, e);
			}
		}
		private static TaskDialogResult ShowTaskDialog(TaskDialogOptions options)
		{
			VistaTaskDialog vtd = new VistaTaskDialog();

			vtd.WindowTitle = options.Title;
			vtd.MainInstruction = options.MainInstruction;
			vtd.Content = options.Content;
			vtd.ExpandedInformation = options.ExpandedInfo;
			vtd.Footer = options.FooterText;

			if (options.CommandButtons != null && options.CommandButtons.Length > 0)
			{
				List<VistaTaskDialogButton> lst = new List<VistaTaskDialogButton>();
				for (int i = 0; i < options.CommandButtons.Length; i++)
				{
					try
					{
						VistaTaskDialogButton button = new VistaTaskDialogButton();
						button.ButtonId = CommandButtonIDOffset + i;
						button.ButtonText = options.CommandButtons[i];
						lst.Add(button);
					}
					catch (FormatException)
					{
					}
				}
				vtd.Buttons = lst.ToArray();
				if (options.DefaultButtonIndex.HasValue && options.DefaultButtonIndex >= 0)
					vtd.DefaultButton = options.DefaultButtonIndex.Value;
			}
			else if (options.RadioButtons != null && options.RadioButtons.Length > 0)
			{
				List<VistaTaskDialogButton> lst = new List<VistaTaskDialogButton>();
				for (int i = 0; i < options.RadioButtons.Length; i++)
				{
					try
					{
						VistaTaskDialogButton button = new VistaTaskDialogButton();
						button.ButtonId = RadioButtonIDOffset + i;
						button.ButtonText = options.RadioButtons[i];
						lst.Add(button);
					}
					catch (FormatException)
					{
					}
				}
				vtd.RadioButtons = lst.ToArray();
				vtd.NoDefaultRadioButton = (!options.DefaultButtonIndex.HasValue || options.DefaultButtonIndex.Value == -1);
				if (options.DefaultButtonIndex.HasValue && options.DefaultButtonIndex >= 0)
					vtd.DefaultRadioButton = options.DefaultButtonIndex.Value;
			}

			if (options.CustomButtons != null && options.CustomButtons.Length > 0)
			{
				List<VistaTaskDialogButton> lst = new List<VistaTaskDialogButton>();
				for (int i = 0; i < options.CustomButtons.Length; i++)
				{
					try
					{
						VistaTaskDialogButton button = new VistaTaskDialogButton();
						button.ButtonId = CustomButtonIDOffset + i;
						button.ButtonText = options.CustomButtons[i];
						lst.Add(button);
					}
					catch (FormatException)
					{
					}
				}

				vtd.Buttons = lst.ToArray();
				if (options.DefaultButtonIndex.HasValue && options.DefaultButtonIndex >= 0)
					vtd.DefaultButton = options.DefaultButtonIndex.Value;
				vtd.CommonButtons = VistaTaskDialogCommonButtons.None;
			}
			else
			{
				vtd.CommonButtons = ConvertCommonButtons(options.CommonButtons);
			}

			vtd.MainIcon = options.MainIcon;
			vtd.FooterIcon = options.FooterIcon;
			vtd.EnableHyperlinks = false;
			vtd.ShowProgressBar = false;
			vtd.AllowDialogCancellation = (options.CommonButtons == TaskDialogCommonButtons.Close ||
										   options.CommonButtons == TaskDialogCommonButtons.OKCancel ||
										   options.CommonButtons == TaskDialogCommonButtons.YesNoCancel);
			vtd.CallbackTimer = false;
			vtd.ExpandedByDefault = false;
			vtd.ExpandFooterArea = false;
			vtd.PositionRelativeToWindow = true;
			vtd.RightToLeftLayout = false;
			vtd.NoDefaultRadioButton = false;
			vtd.CanBeMinimized = false;
			vtd.ShowMarqueeProgressBar = false;
			vtd.UseCommandLinks = (options.CommandButtons != null && options.CommandButtons.Length > 0);
			vtd.UseCommandLinksNoIcon = false;
			vtd.VerificationText = options.VerificationText;
			vtd.VerificationFlagChecked = false;
			vtd.ExpandedControlText = "Hide details";
			vtd.CollapsedControlText = "Show details";
			vtd.Callback = null;

			TaskDialogResult result = null;
			int diagResult = 0;
			TaskDialogSimpleResult simpResult = TaskDialogSimpleResult.None;
			bool verificationChecked = false;
			int radioButtonResult = -1;
			int? commandButtonResult = null;
			int? customButtonResult = null;

			diagResult = vtd.Show((vtd.CanBeMinimized ? null : options.Owner), out verificationChecked, out radioButtonResult);

			if (diagResult >= CommandButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Command;
				commandButtonResult = diagResult - CommandButtonIDOffset;
			}
			else if (radioButtonResult >= RadioButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Radio;
				radioButtonResult -= RadioButtonIDOffset;
			}
			else if (diagResult >= CustomButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Custom;
				customButtonResult = diagResult - CustomButtonIDOffset;
			}
			else
			{
				simpResult = (TaskDialogSimpleResult)diagResult;
			}

			result = new TaskDialogResult(
				simpResult,
				(String.IsNullOrEmpty(options.VerificationText) ? null : (bool?)verificationChecked),
				((options.RadioButtons == null || options.RadioButtons.Length == 0) ? null : (int?)radioButtonResult),
				((options.CommandButtons == null || options.CommandButtons.Length == 0) ? null : commandButtonResult),
				((options.CustomButtons == null || options.CustomButtons.Length == 0) ? null : customButtonResult));

			return result;
		}
		private static TaskDialogResult ShowEmulatedTaskDialog(TaskDialogOptions options)
		{
			TaskDialog td = new TaskDialog();
			TaskDialogViewModel tdvm = new TaskDialogViewModel(options);

			td.DataContext = tdvm;

			if (options.Owner != null)
			{
				td.Owner = options.Owner;
			}

			td.ShowDialog();

			TaskDialogResult result = null;
			int diagResult = -1;
			TaskDialogSimpleResult simpResult = TaskDialogSimpleResult.None;
			bool verificationChecked = false;
			int radioButtonResult = -1;
			int? commandButtonResult = null;
			int? customButtonResult = null;

			diagResult = tdvm.DialogResult;
			verificationChecked = tdvm.VerificationChecked;

			if (diagResult >= CommandButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Command;
				commandButtonResult = diagResult - CommandButtonIDOffset;
			}
			else if (diagResult >= RadioButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Radio;
				radioButtonResult = diagResult - RadioButtonIDOffset;
			}
			else if (diagResult >= CustomButtonIDOffset)
			{
				simpResult = TaskDialogSimpleResult.Custom;
				customButtonResult = diagResult - CustomButtonIDOffset;
			}
			// This occurs usually when the red X button is clicked
			else if (diagResult == -1)
			{
				simpResult = TaskDialogSimpleResult.Cancel;
			}
			else
			{
				simpResult = (TaskDialogSimpleResult)diagResult;
			}

			result = new TaskDialogResult(
				simpResult,
				(String.IsNullOrEmpty(options.VerificationText) ? null : (bool?)verificationChecked),
				((options.RadioButtons == null || options.RadioButtons.Length == 0) ? null : (int?)radioButtonResult),
				((options.CommandButtons == null || options.CommandButtons.Length == 0) ? null : commandButtonResult),
				((options.CustomButtons == null || options.CustomButtons.Length == 0) ? null : customButtonResult));

			return result;
		}
	}
}
