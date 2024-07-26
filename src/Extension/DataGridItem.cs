using System.Collections.Generic;
using System.ComponentModel;

namespace FaultDetectorDotNet.Extension
{
    public class DataGridItem : INotifyPropertyChanged
    {
        private string _class;

        private string _file;
        private Dictionary<string, int> _hits = new Dictionary<string, int>();

        private int _line;

        private string _lineId;

        public string Class
        {
            get => _class;
            set
            {
                _class = value;
                OnPropertyChanged(nameof(Class));
            }
        }

        public string File
        {
            get => _file;
            set
            {
                _file = value;
                OnPropertyChanged(nameof(File));
            }
        }

        public int Line
        {
            get => _line;
            set
            {
                _line = value;
                OnPropertyChanged(nameof(Line));
            }
        }

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