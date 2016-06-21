using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITCC.Logging;
using ITCC.VkPlayer.Enums;
using ITCC.VkPlayer.ViewModels;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace ITCC.VkPlayer.Utils
{
    public class ApiRunner
    {
        #region public

        public async Task<SimpleOperationStatus> AuthorizeAsync(string username, string password, CancellationToken cancellationToken = default(CancellationToken))
        {
            _authorizationCancellationTokenSource.Cancel();
            _authorizationCancellationTokenSource = new CancellationTokenSource();

            try
            {
                var authParams = new ApiAuthParams
                {
                    ApplicationId = Configuration.AppId,
                    Login = username,
                    Password = password,
                    Settings = Settings.Audio
                };
                await Task.Run(() => _api.Authorize(authParams), cancellationToken);
                LogMessage(LogLevel.Info, $"Authorization successfull, token: {_api.Token}");
                _api.OnTokenExpires += TokenExpiresCallback;
                return SimpleOperationStatus.Ok;
            }
            catch (Exception ex)
            {
                LogException(LogLevel.Debug, ex);
                return SimpleOperationStatus.Error;
            }
        }

        public void CancelLogin()
        {
            _authorizationCancellationTokenSource.Cancel();
        }

        public async Task<OperationResult<ReadOnlyCollection<Audio>>> GetAudios()
        {
            try
            {
                User user;
                var audios = await Task.Factory.StartNew(() => _api.Audio.Get(out user, new AudioGetParams()));
                LogMessage(LogLevel.Debug, $"Got {audios.Count} audios");
                return OperationResult<ReadOnlyCollection<Audio>>.Ok(audios);
            }
            catch (Exception ex)
            {
                LogException(LogLevel.Debug, ex);
                return OperationResult<ReadOnlyCollection<Audio>>.Error();
            }
        }

        public async Task<SimpleOperationStatus> DownloadAudioAsync(AudioViewModel audio)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(audio.Url, audio.Filename);
                    App.RunOnUiThread(audio.UpdateDownloadedStatus);
                    return SimpleOperationStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                LogException(LogLevel.Debug, ex);
                return SimpleOperationStatus.Error;
            }
        }
        #endregion

        #region private

        private void TokenExpiresCallback(VkApi api)
        {
            try
            {
                api.RefreshToken();
            }
            catch (Exception ex)
            {
                LogException(LogLevel.Error, ex);
            }
        }

        private static void LogMessage(LogLevel level, string message)
        {
            Logger.LogEntry("API RUNNER", level, message);
        }

        private static void LogException(LogLevel level, Exception exception)
        {
            Logger.LogException("API RUNNER", level, exception);
        }

        private CancellationTokenSource _authorizationCancellationTokenSource = new CancellationTokenSource();

        private readonly VkApi _api = new VkApi();
        #endregion
    }
}
