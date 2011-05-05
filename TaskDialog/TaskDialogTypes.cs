using System;

namespace TaskDialogInterop
{
	/// <summary>
	/// Specifies the standard buttons that are displayed on a task dialog.
	/// </summary>
	public enum TaskDialogCommonButtons
	{
		/// <summary>
		/// The message box displays no buttons.
		/// </summary>
		None = 0,
		/// <summary>
		/// The message box displays a Close button.
		/// </summary>
		Close = 1,
		/// <summary>
		/// The message box displays Yes and No buttons.
		/// </summary>
		YesNo = 2,
		/// <summary>
		/// The message box displays Yes, No, and Cancel buttons.
		/// </summary>
		YesNoCancel = 3,
		/// <summary>
		/// The message box displays OK and Cancel buttons.
		/// </summary>
		OKCancel = 4,
		/// <summary>
		/// The message box displays Retry and Cancel buttons.
		/// </summary>
		RetryCancel = 5
	}
	/// <summary>
	/// Defines configuration options for showing a task dialog.
	/// </summary>
	public struct TaskDialogOptions
	{
		/// <summary>
		/// The default <see cref="T:TaskDialogOptions"/> to be used
		/// by all new <see cref="T:TaskDialog"/>s.
		/// </summary>
		/// <remarks>
		/// Use this to make application-wide defaults, such as for
		/// the caption.
		/// </remarks>
		public static TaskDialogOptions Default;

		/// <summary>
		/// The owner window of the task dialog box.
		/// </summary>
		public System.Windows.Window Owner;
		/// <summary>
		/// Caption of the window.
		/// </summary>
		public string Title;
		/// <summary>
		/// A large 32x32 icon that signifies the purpose of the dialog.
		/// </summary>
		public VistaTaskDialogIcon MainIcon;
		/// <summary>
		/// Principal text.
		/// </summary>
		public string MainInstruction;
		/// <summary>
		/// Supplemental text that expands on the principal text.
		/// </summary>
		public string Content;
		/// <summary>
		/// Extra text that will be hidden by default.
		/// </summary>
		public string ExpandedInfo;
		/// <summary>
		/// Standard buttons.
		/// </summary>
		public TaskDialogCommonButtons CommonButtons;
		/// <summary>
		/// Application-defined options for the user.
		/// </summary>
		public string[] RadioButtons;
		/// <summary>
		/// Buttons that are not from the set of standard buttons. Use an
		/// ampersand to denote an access key.
		/// </summary>
		public string[] CustomButtons;
		/// <summary>
		/// Command link buttons.
		/// </summary>
		public string[] CommandButtons;
		/// <summary>
		/// Zero-based index of the button to have focus by default.
		/// </summary>
		public int? DefaultButtonIndex;
		/// <summary>
		/// Text accompanied by a checkbox, typically for user feedback such as
		/// Do-not-show-this-dialog-again options.
		/// </summary>
		public string VerificationText;
		/// <summary>
		/// A small 16x16 icon that signifies the purpose of the footer text.
		/// </summary>
		public VistaTaskDialogIcon FooterIcon;
		/// <summary>
		/// Additional footer text.
		/// </summary>
		public string FooterText;
	}
	/// <summary>
	/// Provides data for all task dialog buttons.
	/// </summary>
	public class TaskDialogButtonData
	{
		/// <summary>
		/// Gets the button's ID value to return when clicked.
		/// </summary>
		public int ID { get; private set; }
		/// <summary>
		/// Gets the button's text label.
		/// </summary>
		public string Text { get; private set; }
		/// <summary>
		/// Gets a value indicating whether or not the button should be the default.
		/// </summary>
		public bool IsDefault { get; private set; }
		/// <summary>
		/// Gets a value indicating whether or not the button should be a cancel.
		/// </summary>
		public bool IsCancel { get; private set; }
		/// <summary>
		/// Gets the button's command.
		/// </summary>
		public System.Windows.Input.ICommand Command { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDialogButtonData"/> class.
		/// </summary>
		public TaskDialogButtonData()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDialogButtonData"/> struct.
		/// </summary>
		/// <param name="id">The id value for the button.</param>
		/// <param name="text">The text label.</param>
		/// <param name="command">The command to associate.</param>
		/// <param name="isDefault">Whether the button should be the default.</param>
		/// <param name="isCancel">Whether the button should be a cancel.</param>
		public TaskDialogButtonData(int id, string text, System.Windows.Input.ICommand command = null, bool isDefault = false, bool isCancel = false)
		{
			ID = id;
			Text = text;
			Command = command;
			IsDefault = isDefault;
			IsCancel = isCancel;
		}
	}
}
