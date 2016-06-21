using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;

namespace ITCC.VkPlayer.Utils
{
    public class ApplicationContext
    {
        public ApiRunner ApiRunner { get; } = new ApiRunner();
    }
}
