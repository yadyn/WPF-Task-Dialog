using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskDialogInterop
{
	/// <summary>
	/// Displays a task dialog.
	/// </summary>
	public partial class TaskDialog : Window
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDialog"/> class.
		/// </summary>
		public TaskDialog()
		{
			InitializeComponent();

			this.Loaded += new RoutedEventHandler(TaskDialog_Loaded);
			this.SourceInitialized += new EventHandler(TaskDialog_SourceInitialized);
			this.KeyDown += new KeyEventHandler(TaskDialog_KeyDown);
		}

		/// <summary>
		/// Handles the Loaded event of the TaskDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void TaskDialog_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.DataContext != null && this.DataContext is TaskDialogViewModel)
			{
				this.WindowStartupLocation = (this.DataContext as TaskDialogViewModel).StartPosition;

				if ((this.DataContext as TaskDialogViewModel).NormalButtons.Count == 0)
				{
					this.MaxWidth = 462;
				}
				
				// Footer only shows the secondary white top border when the buttons section is visible
				FooterInner.BorderThickness = new Thickness(
					FooterInner.BorderThickness.Left,
					((ButtonsArea.Visibility == System.Windows.Visibility.Visible) ? 1 : 0),
					FooterInner.BorderThickness.Right,
					FooterInner.BorderThickness.Bottom);

				// Hide the special button areas if they are empty
				if ((this.DataContext as TaskDialogViewModel).CommandLinks.Count == 0)
					CommandLinks.Visibility = System.Windows.Visibility.Collapsed;
				if ((this.DataContext as TaskDialogViewModel).RadioButtons.Count == 0)
					RadioButtons.Visibility = System.Windows.Visibility.Collapsed;

				// Play the appropriate sound
				switch ((this.DataContext as TaskDialogViewModel).MainIconType)
				{
					default:
					case VistaTaskDialogIcon.None:
					case VistaTaskDialogIcon.Shield:
						// No sound
						break;
					case VistaTaskDialogIcon.Warning:
						System.Media.SystemSounds.Exclamation.Play();
						break;
					case VistaTaskDialogIcon.Error:
						System.Media.SystemSounds.Hand.Play();
						break;
					case VistaTaskDialogIcon.Information:
						System.Media.SystemSounds.Asterisk.Play();
						break;
				}
			}
		}
		/// <summary>
		/// Handles the SourceInitialized event of the TaskDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TaskDialog_SourceInitialized(object sender, EventArgs e)
		{
			if (this.DataContext != null && this.DataContext is TaskDialogViewModel)
			{
				if ((this.DataContext as TaskDialogViewModel).AllowDialogCancellation)
				{
					SafeNativeMethods.SetWindowIconVisibility(this, false);
				}
				else
				{
					// This also hides the icon, too
					SafeNativeMethods.SetWindowCloseButtonVisibility(this, false);
				}
			}
		}
		/// <summary>
		/// Handles the KeyDown event of the TaskDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void TaskDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if (this.DataContext != null && this.DataContext is TaskDialogViewModel)
			{
				// Block Alt-F4 if it isn't allowed
				if (!(this.DataContext as TaskDialogViewModel).AllowDialogCancellation
					&& e.Key == Key.System && e.SystemKey == Key.F4)
					e.Handled = true;

				// Handel Esc manually if the override has been set
				if ((this.DataContext as TaskDialogViewModel).AllowDialogCancellation
					&& e.Key == Key.Escape)
				{
					e.Handled = true;
					this.DialogResult = false;
					Close();
				}
			}
		}
		/// <summary>
		/// Handles the Click event of NormalButton controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void NormalButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
		/// <summary>
		/// Handles the Click event of CommandLink controls.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void CommandLink_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
