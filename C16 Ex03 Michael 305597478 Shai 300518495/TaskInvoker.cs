using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    class TaskInvoker
    {
        public void Invoke(IReceiver i_Receiver)
        {
            i_Receiver.Execute();
        }
        public static TaskInvoker GetTaskInvoker()
        {
            return new TaskInvoker();
        }
    }
}
