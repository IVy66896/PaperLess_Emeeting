using PaperLess_Emeeting;
using System;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

public class MousePosition : DependencyObject
{
	internal struct NativePoint
	{
		public int X;

		public int Y;
	}

	private Dispatcher dispatcher;

	private Timer timer = new Timer(100.0);

	public static readonly DependencyProperty CurrentPositionProperty = DependencyProperty.Register("CurrentPosition", typeof(Point), typeof(MousePosition));

	public Point CurrentPosition
	{
		get
		{
			return (Point)GetValue(CurrentPositionProperty);
		}
		set
		{
			dispatcher.Invoke((Action)delegate
			{
				SetValue(CurrentPositionProperty, value);
			}, new object[0]);
		}
	}

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetCursorPos(ref NativePoint pt);

	public static Point GetCurrentMousePosition()
	{
		NativePoint pt = default(NativePoint);
		GetCursorPos(ref pt);
		return new Point(pt.X, pt.Y);
	}

	public MousePosition()
	{
		dispatcher = Application.Current.MainWindow.Dispatcher;
		timer.Elapsed += timer_Elapsed;
		timer.Start();
	}

	private void timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		Point point = CurrentPosition = GetCurrentMousePosition();
	}
}
