using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ITCC.Logging;
using ITCC.VkPlayer.Enums;
using ITCC.VkPlayer.Interfaces;
using ITCC.VkPlayer.UI.Common;
using ITCC.VkPlayer.ViewModels;

namespace ITCC.VkPlayer.UI.Windows
{
    /// <summary>
    /// Interaction logic for AudiosWindow.xaml
    /// </summary>
    public partial class AudiosWindow : Window, ILongTaskRunner
    {
        private const string DurationFormat = @"mm\:ss";

        private ObservableCollection<AudioViewModel> _audioViewModels;
        private AudioViewModel _activeAudio;
        private int _activeIndex;
        private bool _isPlaying;
        private bool _autoplay;
        private bool _shuffle;
        private bool _repeat;
        private bool _justLoaded = false;
        private Stack<int> _playedSongs = new Stack<int>();
        private readonly Timer _progrssBarTimer = new Timer(1000);
        private readonly MediaPlayer _player = new MediaPlayer();

        public AudiosWindow()
        {
            InitializeComponent();

            _player.MediaEnded += PlayerOnMediaEnded;
            _progrssBarTimer.Elapsed += ProgrssBarTimerOnElapsed;
            _progrssBarTimer.Start();
        }

        private void ProgrssBarTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!_isPlaying)
                return;

            App.RunOnUiThread(() =>
            {
                PlayProgressBar.Value = 100*_player.Position.TotalSeconds/_activeAudio.Duration.TotalSeconds;
                DurationLabel.Content = $"{_player.Position.ToString(DurationFormat)}/{_activeAudio.Duration.ToString(DurationFormat)}";
            });
        }

        public void BeginOperation(string message)
        {
            InProgressControl.Start(message);
            MainGrid.IsEnabled = false;
        }

        public void EndOperation()
        {
            InProgressControl.Stop();
            MainGrid.IsEnabled = true;
        }

        public void CancelOperations()
        {
            EndOperation();
        }

        private void StopActiveAudio()
        {
            _player.Pause();
            _activeAudio.IsActive = false;
            _activeAudio = null;
            _isPlaying = false;
            _activeIndex = -1;
        }

        private void SetActiveElements(Button button, AudioViewModel audio)
        {
            PlayProgressBar.Value = 0;
            _activeAudio = audio;
            audio.IsActive = true;
            _isPlaying = true;
            _activeIndex = _audioViewModels.IndexOf(audio);
        }

        private void Pause()
        {
            _player.Pause();
            _isPlaying = false;
            TogglePlayButton.Content = "Играть";
        }

        private void Resume()
        {
            if (_justLoaded)
            {
                GoToNextSong();
            }
            _player.Play();
            _isPlaying = true;
            TogglePlayButton.Content = "Пауза";
        }

        private async Task StartPlaying(AudioViewModel audio)
        {
            if (audio.Downloaded)
            {
                _player.Open(new Uri(audio.Filename, UriKind.RelativeOrAbsolute));
                Resume();
            }
            else
            {
                _player.Open(audio.Url);
                Resume();
                BeginOperation("Скачивание файла");
                await App.Context.ApiRunner.DownloadAudioAsync(audio);
                EndOperation();
            }
        }

        private int SelectNextSongIndex()
        {
            if (_shuffle)
            {
                var random = new Random();
                return random.Next(_audioViewModels.Count);
            }
            if (!_playedSongs.Any())
                return 0;

            if (_activeIndex < _audioViewModels.Count - 1)
                return _activeIndex + 1;
            return _repeat ? 0 : -1;
        }

        private int GetPreviousSongIndex()
        {
            return _playedSongs.Any() ? _playedSongs.Pop() : -1;
        }

        private async Task GoToPrevSong()
        {
            var index = GetPreviousSongIndex();
            if (index == -1)
                return;

            await StartPlaying(_audioViewModels[index]);
            SetActiveElements(null, _audioViewModels[index]);
        }

        private async Task GoToNextSong()
        {
            if (! _justLoaded)
                _playedSongs.Push(_activeIndex);
            _justLoaded = false;
            var index = SelectNextSongIndex();
            if (index == -1)
            {
                StopActiveAudio();
                return;
            }

            await StartPlaying(_audioViewModels[index]);
            SetActiveElements(null, _audioViewModels[index]);
            
        }

        private async void PlayerOnMediaEnded(object sender, EventArgs eventArgs)
        {
            if (!_autoplay)
                return;

            await GoToNextSong();
        }

        private async void AudiosWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            BeginOperation("Загрузка данных об аудиозаписях");
            var result = await App.Context.ApiRunner.GetAudios();
            if (result.Status == SimpleOperationStatus.Error)
            {
                Helpers.ShowWarning("Ошибка получения списка записей");
                EndOperation();
                return;
            }
            _justLoaded = true;

            _audioViewModels = new ObservableCollection<AudioViewModel>(result.Result.Select(AudioViewModel.FromAudio));
            foreach (var viewModel in _audioViewModels)
            {
                viewModel.UpdateDownloadedStatus();
            }
            AudiosListView.ItemsSource = _audioViewModels;

            EndOperation();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
            var button = sender as Button;

            var audio = button?.Tag as AudioViewModel;
            if (audio == null)
            {
                return;
            }
            if (audio == _activeAudio)
            {
                if (_isPlaying)
                    Pause();
                else
                {
                    Resume();
                }
                return;
            }
            if (_activeAudio != null)
            {
                StopActiveAudio();
            }

            _playedSongs.Push(_audioViewModels.IndexOf(audio));
            await StartPlaying(audio);
            
            SetActiveElements(button, audio);
        }

        private void AutoplayCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            _autoplay = true;
        }

        private void AutoplayCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _autoplay = false;
        }

        private void RepeatCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            _repeat = true;
        }

        private void RepeatCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _repeat = false;
        }

        private void ShuffleCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            _shuffle = true;
        }

        private void ShuffleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _shuffle = false;
        }

        private async void PrevSongButton_OnClick(object sender, RoutedEventArgs e)
        {
            await GoToPrevSong();
        }

        private async void NextSongButton_OnClick(object sender, RoutedEventArgs e)
        {
            await GoToNextSong();
        }

        private void TogglePlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        private void PlayProgressBar_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_activeAudio == null)
                return;
            var position = e.GetPosition(PlayProgressBar);
            var percent = position.X/PlayProgressBar.ActualWidth;
            _player.Position = TimeSpan.FromSeconds(_activeAudio.Duration.TotalSeconds*percent);

            PlayProgressBar.Value = 100 * _player.Position.TotalSeconds / _activeAudio.Duration.TotalSeconds;
            DurationLabel.Content = $"{_player.Position.ToString(DurationFormat)}/{_activeAudio.Duration.ToString(DurationFormat)}";
        }
    }
}
