using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Collections;

namespace Chat.Dialogs
{
	internal static class LocalExtensions
	{
		public static void ForWindowFromTemplate(this object templateFrameworkElement, Action<Window> action)
		{
			Window window = ((FrameworkElement)templateFrameworkElement).TemplatedParent as Window;
			if (window != null) action(window);
		}

		public static IntPtr GetWindowHandle(this Window window)
		{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle;
		}
	}

	public partial class OptionsDesign
	{
		void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			sender.ForWindowFromTemplate(w => SystemCommands.CloseWindow(w));
		}

		void MinButtonClick(object sender, RoutedEventArgs e)
		{
			sender.ForWindowFromTemplate(w => SystemCommands.MinimizeWindow(w));
		}

		void MaxButtonClick(object sender, RoutedEventArgs e)
		{
			sender.ForWindowFromTemplate(w =>
			{
				if (w.WindowState == WindowState.Maximized) SystemCommands.RestoreWindow(w);
				else SystemCommands.MaximizeWindow(w);
			});
		}
	}
}
