using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ITCC.VkPlayer.Utils;
using JetBrains.Annotations;
using VkNet.Model.Attachments;

namespace ITCC.VkPlayer.ViewModels
{
    public class AudioViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region public

        public AudioViewModel(Audio audio)
        {
            Subject = audio;
        }

        public static AudioViewModel FromAudio(Audio audio)
        {
            return new AudioViewModel(audio);
        }

        public bool LocalFileExists()
        {
            return File.Exists(Filename);
        }

        public void UpdateDownloadedStatus()
        {
            Downloaded = LocalFileExists();
        }

        public void ToggleIsActive()
        {
            IsActive = !IsActive;
        }

        public readonly Audio Subject;

        public AudioViewModel Self => this;

        #endregion

        #region properties

        public string FullName => $"{Artist} - {Title}";

        public string Title
        {
            get { return Subject.Title; }
            set
            {
                Subject.Title = value;
                OnPropertyChanged();
            }
        }

        public string Artist
        {
            get { return Subject.Artist; }
            set
            {
                Subject.Artist = value;
                OnPropertyChanged();
            }
        }

        public Uri Url
        {
            get { return Subject.Url; }
            set
            {
                Subject.Url = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Duration
        {
            get { return TimeSpan.FromSeconds(Subject.Duration); }
            set
            {
                Subject.Duration = (int)value.TotalSeconds;
                OnPropertyChanged();
            }
        }

        public bool Downloaded
        {
            get { return _downloaded; }
            set
            {
                _downloaded = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public string Filename => $"{Configuration.MusicCacheFolder}\\{FullName}.mp3";
        #endregion

        #region private

        private bool _downloaded;
        private bool _isActive;

        #endregion
    }
}
