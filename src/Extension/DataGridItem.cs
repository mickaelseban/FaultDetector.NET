using System.Collections.Generic;
using System.ComponentModel;

namespace FaultDetectorDotNet.Extension
{
    public class DataGridItem : INotifyPropertyChanged
    {
        private Dictionary<string, int> _hits = new Dictionary<string, int>();

        private string _class;
        public string Class
        {
            get => _class;
            set
            {
                _class = value;
                OnPropertyChanged(nameof(Class));
            }
        }

        private string _file;
        public string File
        {
            get => _file;
            set
            {
                _file = value;
                OnPropertyChanged(nameof(File));
            }
        }

        private int _line;
        public int Line
        {
            get => _line;
            set
            {
                _line = value;
                OnPropertyChanged(nameof(Line));
            }
        }

        private string _lineId;
        public string LineId
        {
            get => _lineId;
            set
            {
                _lineId = value;
                OnPropertyChanged(nameof(LineId));
            }
        }

        public Dictionary<string, int> Hits
        {
            get => _hits;
            set
            {
                _hits = value;
                OnPropertyChanged(nameof(Hits));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}