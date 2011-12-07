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

using TaskDialogInterop;

namespace WPFTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool _downloadTimerReset;
		private int _downloadedPercent;
		private Random _downloadRandomizer;

		public MainWindow()
		{
			InitializeComponent();

			_downloadRandomizer = new Random();
		}

		private void UpdateResult(TaskDialogResult res)
		{
			if (res == null)
			{
				lbResult.Text = "Task Dialog Result";
				return;
			}

			StringBuilder strBldr = new StringBuilder();

			strBldr.AppendLine("Task Dialog Result");
			strBldr.AppendLine("Simple Result: " + res.Result.ToString());

			if (res.RadioButtonResult.HasValue)
			{
				strBldr.AppendLine("RadioButtonResult: " + res.RadioButtonResult.ToString());
			}
			else
			{
				strBldr.AppendLine("RadioButtonResult: <null>");
			}

			if (res.CommandButtonResult.HasValue)
			{
				strBldr.AppendLine("CommandButtonResult: " + res.CommandButtonResult.ToString());
			}
			else
			{
				strBldr.AppendLine("CommandButtonResult: <null>");
			}

			if (res.CustomButtonResult.HasValue)
			{
				strBldr.AppendLine("CustomButtonResult: " + res.CustomButtonResult.ToString());
			}
			else
			{
				strBldr.AppendLine("CustomButtonResult: <null>");
			}

			if (res.VerificationChecked.HasValue)
			{
				strBldr.Append("VerificationChecked: " + res.VerificationChecked.ToString());
			}
			else
			{
				strBldr.Append("VerificationChecked: <null>");
			}

			lbResult.Text = strBldr.ToString();
		}
		private void UpdateResult(TaskDialogSimpleResult res)
		{
			lbResult.Text = "Task Dialog Result" + Environment.NewLine
				+ "Simple Result: " + res.ToString();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogOptions config = new TaskDialogOptions();

			config.Owner = this;
			config.Title = "Task Dialog Title";
			config.MainInstruction = "The main instruction text for the TaskDialog goes here";
			config.Content = "The content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.ExpandedInfo = "Any expanded content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.VerificationText = "Don't show me this message again";
			config.CustomButtons = new string[] { "&Save", "Do&n't save", "&Cancel" };
			config.MainIcon = VistaTaskDialogIcon.Shield;
			config.FooterText = "Optional footer text with an icon or <a href=\"testUri\">hyperlink</a> can be included.";
			config.FooterIcon = VistaTaskDialogIcon.Warning;
			config.AllowDialogCancellation = true;
			config.Callback = taskDialog_Callback1;

			TaskDialogResult res = TaskDialog.Show(config);

			UpdateResult(res);
		}

		private void button2_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogSimpleResult res =
				TaskDialog.ShowMessage(this,
					"Outlook",
					"ActiveSync can't log on to Outlook",
					"Make sure that Outlook is installed and functioning correctly.",
					"You may need to restart your computer. You could have a conflict due to two folders on this computer are name C:\\Program Files\\Microsoft and C:\\Program Files\\Microsoft Office. If so, rename the C:\\Program Files\\Microsoft folder so that it does not contain the word \"Microsoft.\" If this folder contains subfolders, you may need to reinstall programs in the renamed folder.\n\nFor more information on renaming folders and installing programs, see Help for your operating system.",
					null,
					null,
					TaskDialogCommonButtons.Close,
					VistaTaskDialogIcon.Error,
					VistaTaskDialogIcon.None);

			UpdateResult(res);
		}

		private void button3_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogSimpleResult res =
				TaskDialog.ShowMessage(
					this,
					"WARNING: Formatting will erase ALL data on this disk. To format the disk, click OK. To quit, click Cancel.",
					"Format Local Disk (F:)",
					TaskDialogCommonButtons.OKCancel,
					VistaTaskDialogIcon.Warning);

			UpdateResult(res);
		}

		private void button4_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogOptions config = new TaskDialogOptions();

			config.Owner = this;
			config.Title = "RadioBox Title";
			config.MainInstruction = "The main instruction text for the TaskDialog goes here.";
			config.Content = "The content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.ExpandedInfo = "Any expanded content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.RadioButtons = new string[] { "Radio Option 1", "Radio Option 2", "Radio Option 3", "Radio Option 4", "Radio Option 5" };
			config.MainIcon = VistaTaskDialogIcon.Information;

			TaskDialogResult res = TaskDialog.Show(config);

			UpdateResult(res);
		}

		private void button5_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogOptions config = new TaskDialogOptions();

			config.Owner = this;
			config.Title = "CommandLink Title";
			config.MainInstruction = "The main instruction text for the TaskDialog goes here.";
			config.Content = "The content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.ExpandedInfo = "Any expanded content text for the task dialog is shown here and the text will automatically wrap as needed.";
			config.CommandButtons = new string[] { "Command &Link 1", "Command Link 2\nLine 2\nLine 3", "Command Link 3" };
			config.MainIcon = VistaTaskDialogIcon.Information;

			TaskDialogResult res = TaskDialog.Show(config);

			UpdateResult(res);
		}

		private void button6_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogOptions config = new TaskDialogOptions();

			config.Owner = this;
			config.Title = "Windows Genuine Verification";
			config.MainInstruction = "This copy of Windows is not genuine.";
			config.Content = "You may be a victim of software counterfeiting.";
			config.CommonButtons = TaskDialogCommonButtons.Close;
			config.CustomMainIcon = System.Drawing.Icon.FromHandle(Properties.Resources.genuine_32.GetHicon());

			TaskDialogResult res = TaskDialog.Show(config);

			UpdateResult(res);

			config.CustomMainIcon.Dispose();
		}

		private void button7_Click(object sender, RoutedEventArgs e)
		{
			TaskDialogOptions config = new TaskDialogOptions();

			config.Owner = this;
			config.Title = "Downloading File...";
			config.MainInstruction = "Your file 'en_visual_studio_2010_ultimate_x86_dvd_509116.iso' is currently downloading";
			config.Content = "Time elapsed: 00:00 | Download rate: 0 KB/s";
			config.CustomButtons = new string[] { "&Reset Timer", "&Cancel" };
			config.AllowDialogCancellation = true;
			config.ShowProgressBar = true;
			config.EnableCallbackTimer = true;
			config.Callback = taskDialog_Callback2;

			TaskDialogResult res = TaskDialog.Show(config);

			UpdateResult(res);
		}

		private bool taskDialog_Callback1(IActiveTaskDialog dialog, VistaTaskDialogNotificationArgs args, object callbackData)
		{
			bool result = false;

			switch (args.Notification)
			{
				case VistaTaskDialogNotification.HyperlinkClicked:
					//result = true; // prevents HREF from being processed automatically by ShellExecute
					MessageBox.Show("Hyperlink clicked: " + args.Hyperlink);
					break;
			}

			return result;
		}
		private bool taskDialog_Callback2(IActiveTaskDialog dialog, VistaTaskDialogNotificationArgs args, object callbackData)
		{
			bool result = false;

			switch (args.Notification)
			{
				case VistaTaskDialogNotification.Created:
					_downloadedPercent = 0;
					dialog.SetProgressBarRange(0, 100);
					break;
				case VistaTaskDialogNotification.ButtonClicked:
					if (args.ButtonId == 500)
					{
						_downloadTimerReset = true;
						result = true; // prevent dialog from closing
					}
					break;
				case VistaTaskDialogNotification.Timer:
					if (_downloadedPercent < 100 && _downloadRandomizer.Next(0, 10) == 0)
					{
						_downloadedPercent++;

						dialog.SetProgressBarPosition(_downloadedPercent);
						dialog.SetWindowTitle(
							String.Format(
								"{0:P0} Complete Downloading File...",
								(Convert.ToDouble(_downloadedPercent) / 100d)));
					}

					// 131072 = 1 MB in bytes
					dialog.SetContent(
						String.Format(
							"Time elapsed: {0} | Download rate: {1}/s",
							TimeSpan.FromMilliseconds(args.TimerTickCount).ToString(@"h\:mm\:ss"),
							GetByteScaleSizeBinary(_downloadRandomizer.Next(0, 131072))));

					if (_downloadTimerReset)
					{
						// TRUE: reset tick count (args.TimerTickCount)
						result = true;
						_downloadTimerReset = false;
					}
					break;
			}

			return result;
		}

		private void btAsterisk_Click(object sender, RoutedEventArgs e)
		{
			System.Media.SystemSounds.Asterisk.Play();
		}

		private void btBeep_Click(object sender, RoutedEventArgs e)
		{
			System.Media.SystemSounds.Beep.Play();
		}

		private void btExclamation_Click(object sender, RoutedEventArgs e)
		{
			System.Media.SystemSounds.Exclamation.Play();
		}

		private void btHand_Click(object sender, RoutedEventArgs e)
		{
			System.Media.SystemSounds.Hand.Play();
		}

		private void btQuestion_Click(object sender, RoutedEventArgs e)
		{
			System.Media.SystemSounds.Question.Play();
		}

		private void checkBox1_CheckedChanged(object sender, RoutedEventArgs e)
		{
			TaskDialog.ForceEmulationMode = checkBox1.IsChecked ?? false;
		}

		private static string GetByteScaleSizeBinary(long size)
		{
			double divisor = 1d;

			string negativePrefix = (size < 0) ? "-" : "";

			long workingSize = Math.Abs(size);

			if (workingSize >= 1099511627776)
			{
				divisor = 1099511627776d;
				return negativePrefix + ((int)(Convert.ToDouble(workingSize) / divisor)).ToString() + " TB";
			}
			else if (workingSize >= 1073741824)
			{
				divisor = 1073741824d;
				return negativePrefix + ((int)(Convert.ToDouble(workingSize) / divisor)).ToString() + " GB";
			}
			else if (workingSize >= 1048576 && size < 1073741824)
			{
				divisor = 1048576d;
				return negativePrefix + ((int)(Convert.ToDouble(workingSize) / divisor)).ToString() + " MB";
			}
			else if (workingSize > 1024 && size < 1048576)
			{
				divisor = 1024d;
				return negativePrefix + ((int)(Convert.ToDouble(workingSize) / divisor)).ToString() + " kB";
			}
			else
			{
				return negativePrefix + workingSize.ToString() + " bytes";
			}
		}
	}
}
