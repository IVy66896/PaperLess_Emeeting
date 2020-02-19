using System.ComponentModel;

public class ThumbnailImageAndPage : INotifyPropertyChanged
{
	public bool _isDownloaded;

	public string pageIndex
	{
		get;
		set;
	}

	public string rightImagePath
	{
		get;
		set;
	}

	public string leftImagePath
	{
		get;
		set;
	}

	public bool isDownloaded
	{
		get
		{
			return _isDownloaded;
		}
		set
		{
			_isDownloaded = value;
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs("isDownloaded"));
			}
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public ThumbnailImageAndPage(string _pageIndex, string _rightImagePath, string _leftImagePath, bool downloadStatus)
	{
		pageIndex = _pageIndex;
		rightImagePath = _rightImagePath;
		leftImagePath = _leftImagePath;
		_isDownloaded = downloadStatus;
	}
}
