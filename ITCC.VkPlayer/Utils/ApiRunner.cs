using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ITCC.Logging;
using ITCC.VkPlayer.Enums;
using VkNet;
using VkNet.Enums.Filters;

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
            Logger.LogEntry("DB BACKGRND", level, message);
        }

        private static void LogException(LogLevel level, Exception exception)
        {
            Logger.LogException("DB BACKGRND", level, exception);
        }

        private CancellationTokenSource _authorizationCancellationTokenSource = new CancellationTokenSource();

        private readonly VkApi _api = new VkApi();
        #endregion
    }
}
