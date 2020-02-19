using PaperLess_Emeeting;
using PaperLess_Emeeting.App_Code.MessageBox;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

public class AgendaRow : UserControl, IComponentConnector
{
	private Dictionary<string, string> cbData = new Dictionary<string, string>
	{
		{
			"未開始",
			"N"
		},
		{
			"進行中",
			"U"
		},
		{
			"已結束",
			"D"
		}
	};

	internal Grid imgHasFile;

	internal TextBlock txtAgendaName;

	internal TextBlock txtCaption;

	internal ComboBox cbProgress;

	internal Button btnProgress;

	private bool _contentLoaded;

	public MeetingDataAgenda meetingDataAgenda
	{
		get;
		set;
	}

	public string MeetingID
	{
		get;
		set;
	}

	public string UserID
	{
		get;
		set;
	}

	public bool IsHasFile
	{
		get;
		set;
	}

	public bool IsHasChildren
	{
		get;
		set;
	}

	public bool IsParent
	{
		get;
		set;
	}

	public event MeetingDataCT_ShowAgendaFile_Function MeetingDataCT_ShowAgendaFile_Event;

	public event MeetingDataCT_GetAgendaInwWorkCount_Function MeetingDataCT_GetAgendaInwWorkCount_Event;

	public AgendaRow(string MeetingID, string UserID, bool IsHasFile, bool IsHasChildren, bool IsParent, MeetingDataAgenda meetingDataAgenda, MeetingDataCT_ShowAgendaFile_Function callback1, MeetingDataCT_GetAgendaInwWorkCount_Function callback2)
	{
		InitializeComponent();
		this.MeetingID = MeetingID;
		this.UserID = UserID;
		this.IsHasFile = IsHasFile;
		this.IsHasChildren = IsHasChildren;
		this.IsParent = IsParent;
		this.meetingDataAgenda = meetingDataAgenda;
		MeetingDataCT_ShowAgendaFile_Event += callback1;
		MeetingDataCT_GetAgendaInwWorkCount_Event += callback2;
		base.Loaded += AgendaRow_Loaded;
	}

	private void AgendaRow_Loaded(object sender, RoutedEventArgs e)
	{
		InitUI_Part1();
		Task.Factory.StartNew(delegate
		{
			base.Dispatcher.BeginInvoke((Action)delegate
			{
				InitUI_Part2();
				InitEvent();
			});
		});
	}

	private void InitEvent()
	{
		btnProgress.MouseEnter += delegate
		{
			MouseTool.ShowHand();
		};
		btnProgress.MouseLeave += delegate
		{
			MouseTool.ShowArrow();
		};
		btnProgress.Click += btnProgress_Click;
		cbProgress.MouseLeave += delegate
		{
			cbProgress_SelectionChanged(cbProgress, new EventArgs());
		};
		txtAgendaName.MouseEnter += delegate
		{
			MouseTool.ShowHand();
		};
		txtAgendaName.MouseLeave += delegate
		{
			MouseTool.ShowArrow();
		};
		txtAgendaName.MouseLeftButtonDown += txtName_MouseLeftButtonDown;
	}

	private void SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
	{
	}

	private void txtName_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		Brush brush = ColorTool.HexColorToBrush("#0093b0");
		if (txtAgendaName.Foreground.ToString().Equals(brush.ToString()))
		{
			this.MeetingDataCT_ShowAgendaFile_Event(meetingDataAgenda.ID, meetingDataAgenda.ParentID, IsDbClick: true);
		}
		else
		{
			this.MeetingDataCT_ShowAgendaFile_Event(meetingDataAgenda.ID, meetingDataAgenda.ParentID, IsDbClick: false);
		}
		txtAgendaName.Foreground = brush;
		txtAgendaName.Inlines.LastInline.Foreground = brush;
		txtCaption.Foreground = brush;
	}

	private void cbProgress_SelectionChanged(object sender, EventArgs e)
	{
		cbProgress.Visibility = Visibility.Collapsed;
		meetingDataAgenda.Progress = cbProgress.SelectedValue.ToString();
		btnProgress.Content = (from x in cbData
			where x.Value.Equals(cbProgress.SelectedValue)
			select x.Key).First();
		ChangeColor(btnProgress.Content.ToString());
		btnProgress.Visibility = Visibility.Visible;
		GetProgressUpload.AsyncPOST(MeetingID, UserID, meetingDataAgenda.ID, cbProgress.SelectedValue.ToString(), delegate
		{
		});
	}

	private void btnProgress_Click(object sender, RoutedEventArgs e)
	{
		if (this.MeetingDataCT_GetAgendaInwWorkCount_Event(meetingDataAgenda.ID) > 0)
		{
			AutoClosingMessageBox.Show("請先完成進行中的議程");
			return;
		}
		btnProgress.Visibility = Visibility.Collapsed;
		cbProgress.Visibility = Visibility.Visible;
		cbProgress.IsDropDownOpen = true;
	}

	private void InitUI_Part1()
	{
		txtAgendaName.Inlines.Add(new Run(meetingDataAgenda.Agenda));
		if (meetingDataAgenda.Caption != null && !meetingDataAgenda.Caption.Equals(""))
		{
			txtCaption.Text = meetingDataAgenda.Caption;
			txtCaption.Foreground = new SolidColorBrush(Color.FromRgb(161, 161, 157));
			txtCaption.Visibility = Visibility.Visible;
		}
		string text = "";
		if (meetingDataAgenda.ProposalUnit != null && !meetingDataAgenda.ProposalUnit.Trim().Equals(""))
		{
			text = $" ({meetingDataAgenda.ProposalUnit})";
		}
		txtAgendaName.Inlines.Add(new Run(text)
		{
			Foreground = new SolidColorBrush(Color.FromRgb(161, 161, 157))
		});
		_ = meetingDataAgenda.ParentID;
		if (!IsParent)
		{
			txtAgendaName.Margin = new Thickness(txtAgendaName.Margin.Left + 23.0, txtAgendaName.Margin.Top, txtAgendaName.Margin.Right, txtAgendaName.Margin.Bottom);
		}
		if (IsHasFile)
		{
			imgHasFile.Visibility = Visibility.Visible;
			btnProgress.Visibility = Visibility.Visible;
		}
		if (IsParent ^ IsHasChildren)
		{
			btnProgress.Visibility = Visibility.Visible;
		}
		if (meetingDataAgenda.Progress == null || meetingDataAgenda.Progress.Equals(""))
		{
			btnProgress.Visibility = Visibility.Collapsed;
		}
	}

	private void InitUI_Part2()
	{
		cbProgress.ItemsSource = cbData;
		cbProgress.DisplayMemberPath = "Key";
		cbProgress.SelectedValuePath = "Value";
		cbProgress.SelectedValue = meetingDataAgenda.Progress;
		btnProgress.Content = cbProgress.Text;
		ChangeColor(btnProgress.Content.ToString());
	}

	private void ChangeColor(string cbDataKey)
	{
		switch (cbDataKey)
		{
		case "未開始":
			btnProgress.Foreground = ColorTool.HexColorToBrush("#3746db");
			break;
		case "已結束":
			btnProgress.Foreground = ColorTool.HexColorToBrush("#000000");
			break;
		case "進行中":
			btnProgress.Foreground = ColorTool.HexColorToBrush("#ff1a1a");
			break;
		}
	}

	private void InitSelectDB()
	{
	}

	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[DebuggerNonUserCode]
	public void InitializeComponent()
	{
		if (!_contentLoaded)
		{
			_contentLoaded = true;
			Uri resourceLocator = new Uri("/PaperLess_Emeeting_NTPC;component/agendarow.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			imgHasFile = (Grid)target;
			break;
		case 2:
			txtAgendaName = (TextBlock)target;
			break;
		case 3:
			txtCaption = (TextBlock)target;
			break;
		case 4:
			cbProgress = (ComboBox)target;
			break;
		case 5:
			btnProgress = (Button)target;
			break;
		default:
			_contentLoaded = true;
			break;
		}
	}
}
