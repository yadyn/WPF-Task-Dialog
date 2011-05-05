# WPF Task Dialog Wrapper #

It is a drop-in assembly that lets you call up Task Dialogs in your WPF app.
Uses the native [TaskDialogIndirect][1] API calls when available (Vista/7) and
falls back to an emulated WPF dialog when not (XP and earlier).

## Background ##

I've made very good use of Hedley Muscroft's [WinForms emulator and wrapper][2]
for TaskDialogs in the past. In that same spirit, I've taken part of his hard
work and done something very similar, but this time for WPF and .NET 4.0.

There are tons of other WPF/WinForms TaskDialog implementations out there,
either those that wrap the Win32 APIs, or just providing a custom WPF dialog
to use. A quick web search will find you plenty. What they don't give you is a
unified way to use the native that still works on older Windows. I go into more
detail on the rationale and history of all this in [a blog post here][3].

Hedley's work is great and I've used it plenty of times, but I wanted something
very similar that worked for WPF apps. I also thought I might take the time to
try to improve on it, mostly thanks to the power of WPF and .NET 4.0.

## Code ##

Add the reference to your WPF project. You'll find all of the classes under the
TaskDialogInterop namespace. Use the TaskDialog class's static methods to
show dialogs. Some work more akin to the more basic MessageBox, accepting
a string and other parameters to customize the display. The real power,
though, is found in the overload that takes a TaskDialogOptions object.

The basic flow in your apps will typically be something like this:

    TaskDialogOptions config = new TaskDialogOptions();

    config.Owner = this;
    config.Title = "Task Dialog Title";
    config.MainInstruction = "The main instruction text for the TaskDialog goes here";
    config.Content = "The content text for the task dialog is shown " + 
                 "here and the text will automatically wrap as needed.";
    config.ExpandedInfo = "Any expanded content text for the " + 
                      "task dialog is shown here and the text " + 
                      "will automatically wrap as needed.";
    config.VerificationText = "Don't show me this message again";
    config.CustomButtons = new string[] { "&Save", "Do&n't save", "&Cancel" };
    config.MainIcon = VistaTaskDialogIcon.Shield;
    config.FooterText = "Optional footer text with an icon can be included.";
    config.FooterIcon = VistaTaskDialogIcon.Warning;

    TaskDialogResult res = TaskDialog.Show(config);

The options object lets you specify all of the particulars. Anything you don't
define will be ignored or unused. This makes it very easy to display as much or
as little as you need to.

As you can see, buttons are handled via simple string arrays. Each string in
the array will be converted to a button using the given label. Use the
ampersand character (&) before a letter to denote the accelerator.

Depending on the method you use, you'll either get back a
TaskDialogSimpleResult or a TaskDialogResult. The simple one works just like
the DialogResult you're probably used to. In fact, it can be cast successfully
back and forth between the two. This is nice if you have existing services/code
that expect a DialogResult. The full result object contains everything you'd
want to know about how the user acted on your dialog. It has properties you
can read to find out which button, command link, radio, etc. was
chosen/clicked.

You can read more as well as see screenshots [here][4].

## Details ##

- Project created in Visual Studio 2010
- .NET 4.0 Client Profile
- Uses the native TaskDialogIndirect API when available (Vista/7)
- Licensed under [The Code Project Open License (CPOL) 1.02][5]
    - Can be freely redistributed
	- Can be modified
	- Can be used in commercial software
	- No warranty/guarantee

[1]: http://msdn.microsoft.com/en-us/library/bb760544%28v=vs.85%29.aspx
[2]: http://www.codeproject.com/KB/vista/Vista_TaskDialog_Wrapper.aspx
[3]: http://yadyn.blogspot.com/2010/12/yet-another-taskdialog-wrapper-this.html
[4]: http://www.codeproject.com/KB/WPF/WPFTaskDialogEmulator.aspx
[5]: http://www.codeproject.com/info/cpol10.aspx