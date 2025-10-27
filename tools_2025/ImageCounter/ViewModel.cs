using GalaSoft.MvvmLight;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace ImageCounter
{
    public class ViewModel : ViewModelBase
    {
        private static ViewModel _instance;
        public static ViewModel Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new ViewModel();
                }
                return _instance;
            }
        }

        private string _scanFolderPath;
        public string ScanFolderPath { get => _scanFolderPath; set { _scanFolderPath = value; RaisePropertyChanged("ScanFolderPath"); } }
        private string _csvSavePath;
        public string CsvSavePath { get => _csvSavePath; set { _csvSavePath = value; RaisePropertyChanged("CsvSavePath"); } }
        private int _imageCount;
        public int ImageCount { get => _imageCount; set { _imageCount = value; RaisePropertyChanged("ImageCount"); RaisePropertyChanged("BlackPercentage"); } }

        private int _blackImageCount=0;
        public int BlackImageCount { get => _blackImageCount; set { _blackImageCount = value; RaisePropertyChanged("BlackImageCount"); RaisePropertyChanged("BlackPercentage"); } }
        private string _statusMessage;
        public string StatusMessage { get => _statusMessage; set { _statusMessage = value; RaisePropertyChanged("StatusMessage"); } }
        private double _blackThreshold = 30;
        public double BlackThreshold { get => _blackThreshold; set { _blackThreshold = value; RaisePropertyChanged("BlackThreshold"); } }
        private bool _delBlackImg;
        public bool DelImg { get => _delBlackImg; set { _delBlackImg = value; RaisePropertyChanged("DelImg"); } }
        private bool _recordInfo;
        public bool RecordInfo { get => _recordInfo; set { _recordInfo = value; RaisePropertyChanged("RecordInfo"); } }
        private bool _multiprocessing = false;
        public bool Multiprocessing { get => _multiprocessing; set { _multiprocessing = value; RaisePropertyChanged("Multiprocessing"); } }

        // 计算属性：黑色图片百分比（0-100）
        public double BlackPercentage => ImageCount > 0 ? (double)BlackImageCount / ImageCount * 100 : 0;
    }

    public class ScanProgress
    {
        public int CurrentCount { get; set; }
        public int BlackImageCount { get; set; }
        public string CurrentFile { get; set; }
    }
}