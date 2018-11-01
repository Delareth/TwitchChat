using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Awesomium.Core;

namespace Chat
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();

		TransparentMouse TransparentMouse;
		static MainWindow _instance;
		public static string appId = "35unfn1ou9ff5nclvq65rd73gd1ikm";
		public static string channelName = "scr13m";

		public MainWindow()
		{
			InitializeComponent();
			ShowInTaskbar = Properties.Settings.Default.Window_TaskBar;

			webControl.Source = new Uri("file:///src/index.html");

			if (Properties.Settings.Default.FirstInit == "Visible")
			{
				Params.AllowChangeSize = true;
			}

			webControl.DocumentReady += WebControl_DocumentReady;

			AllocConsole();

			MainController.ConnectToTwitch();
			MainController.ConnectToDonate();
			MainController.ConnectToFollowers();
		}

		private void WebControl_DocumentReady(object sender, UrlEventArgs e)
		{
			string sCommand = "loadInfo('" + appId + "', '" + channelName + "')";
			webControl.ExecuteJavascript(sCommand);
		}

		#region TestButtons

		private void TestMessageButton(object sender, EventArgs e)
		{
			for(int i = 0; i < 20; i++)
			{
				string sCommand = "addMessage('PandaChat', 'Я есть панда!')";
				webControl.ExecuteJavascript(sCommand);
			}
		}

		private void TestFollowerButton(object sender, EventArgs e)
		{
			string sCommand = "addFollower('pandaaaa')";
			webControl.ExecuteJavascript(sCommand);
		}

		private void TestDonateButton(object sender, EventArgs e)
		{
			string sCommand = "addDonate('pandaa', 'Ты лучший!', '50', 'RUB')";
			webControl.ExecuteJavascript(sCommand);
		}

		#endregion

		#region MainControl

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			_instance = this;

			bool check = MainController.CheckConnect();

			if (check)
			{
				Console.WriteLine("Connected to IRC");
				MainController.SendTwitchCommands();
			}
			else
			{
				Console.WriteLine("Error in connecting to IRC");
			}
		}

		public static void RunInUiThread(Action a)
		{
			if (_instance.Dispatcher.CheckAccess())
			{
				a();
			}
			else
			{
				_instance.Dispatcher.BeginInvoke(a);
			}
		}

		public static void ExecuteJS(string command)
		{
			_instance.webControl.ExecuteJavascript(command);
		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (TransparentMouse.AllowTransparency)
			{
				// если зажата кнопка для перетаскивания
				if (Keyboard.IsKeyDown(Key.LeftCtrl))
					DragMove();
			}
			else
			{
				if (Mouse.LeftButton == MouseButtonState.Pressed)
					DragMove();
			}
		}

		private void Window_SourceInitialized(object sender, EventArgs e)
		{
			TransparentMouse = new TransparentMouse(this)
			{
				AllowTransparency = Properties.Settings.Default.Window_OverlayMode
			};
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Properties.Settings.Default.Save();
		}

		private void Window_Resized(object sender, EventArgs e)
		{
			if(webControl.IsLoaded)
			{
				string sCommand = "reloadSize()";
				webControl.ExecuteJavascript(sCommand);
			}
		}

		#endregion

		#region TreyControl

		private void FirstMousePress(object sender, EventArgs e)
		{
			if(Properties.Settings.Default.FirstInit == "Visible")
			{
				Properties.Settings.Default.FirstInit = "Hidden";
			}
		}

		private void CloseButton(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void SettingsButton(object sender, EventArgs e)
		{
			Dialogs.Options od = new Dialogs.Options();

			od.ShowDialog();
		}

		private void MouseButton(object sender, EventArgs e)
		{
			TransparentMouse.AllowTransparency = !TransparentMouse.AllowTransparency;
			Properties.Settings.Default.Window_OverlayMode = TransparentMouse.AllowTransparency;

			if(TransparentMouse.AllowTransparency)
			{
				Properties.Settings.Default.sMouseTransparenty = "Включить кликабельность";
			}
			else
			{
				Properties.Settings.Default.sMouseTransparenty = "Отключить кликабельность";
			}
		}

		private void ChangeSizeButton(object sender, EventArgs e)
		{
			Params.AllowChangeSize = !Params.AllowChangeSize;

			if(Params.AllowChangeSize)
			{
				Properties.Settings.Default.sResizeMode = "CanResizeWithGrip";
				Properties.Settings.Default.sChangeSize = "Отключить изменение размера";
			}
			else
			{
				Properties.Settings.Default.sResizeMode = "NoResize";
				Properties.Settings.Default.sChangeSize = "Включить изменение размера";
			}
		}

		private void ShowInToolbarButton(object sender, EventArgs e)
		{
			ShowInTaskbar = !ShowInTaskbar;
			Properties.Settings.Default.Window_TaskBar = ShowInTaskbar;

			if (ShowInTaskbar)
			{
				Properties.Settings.Default.sShowInTaskbar = "Скрыть из панели задач";
			}
			else
			{
				Properties.Settings.Default.sShowInTaskbar = "Отображать в панели задач";
			}
		}

		#endregion
	}
}
