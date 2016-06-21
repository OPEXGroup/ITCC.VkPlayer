using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITCC.VkPlayer.Enums;

namespace ITCC.VkPlayer.Utils
{
    public class OperationResult<TResult>
    {
        public SimpleOperationStatus Status { get; set; }
        public TResult Result { get; set; }

        public static OperationResult<TResult> Ok(TResult data = default(TResult))
        {
            return new OperationResult<TResult>
            {
                Status = SimpleOperationStatus.Ok,
                Result = data
            };
        }

        public static OperationResult<TResult> Error(TResult data = default(TResult))
        {
            return new OperationResult<TResult>
            {
                Status = SimpleOperationStatus.Error,
                Result = data
            };
        }

        public static OperationResult<TResult> NothingToDo(TResult data = default(TResult))
        {
            return new OperationResult<TResult>
            {
                Status = SimpleOperationStatus.NothingToDo,
                Result = data
            };
        }
    }
}
