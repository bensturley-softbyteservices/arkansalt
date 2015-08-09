using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkansalt.DevConsole
{
    public class ConsoleFunctionOutputEventArgs : EventArgs
    {
        public ConsoleFunctionOutputEventArgs(string output, bool isDataRow)
        {
            this.Output = output;
            this.IsDataRow = isDataRow;
        }
        
        public string Output { get; private set; }

        public bool IsDataRow { get; private set; }

        public int DataRowNum { get; set; }
        
    }
}
