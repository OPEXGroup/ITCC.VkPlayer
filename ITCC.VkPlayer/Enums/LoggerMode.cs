using System;

namespace ITCC.VkPlayer.Enums
{
    [Flags]
    internal enum LoggerMode
    {
        None = 0,
        File = 1,
        Window = 2,

        /// <summary>
        ///     Show MessageBox'es for messages of Warning and higher level
        /// </summary>
        Message = 4
    }
}