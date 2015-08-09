using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkansalt.DevConsole
{
    public class ConsoleFunctionOutput
    {
        public ConsoleFunctionOutput(
            EventHandler<ConsoleFunctionOutputEventArgs> outputReadyEventHandler
            , EventHandler<ConsoleFunctionOutputGetInputEventArgs> getInputEventHandler 
            )
        {
            if (outputReadyEventHandler == null)
                throw new ArgumentNullException("outputReadyEventHandler", "OutputReadyEvent handler cannot be null.");

            this.OutputReady += outputReadyEventHandler;
            this.DataRowCount = 0;

            this.GetInput += getInputEventHandler;
        }


        #region unused constructors

        //public ConsoleFunctionOutput(string output)
        //{
        //    this.Output = output;
        //    this.DataStartRowNumber = 0;
        //    this.DataEndRowNumber = 0;
        //}

        //public ConsoleFunctionOutput(string output, int dataStartRowNumber, int dataEndRowNumber)
        //{
        //    this.Output = output;
        //    this.DataStartRowNumber = dataStartRowNumber;
        //    this.DataEndRowNumber = dataEndRowNumber;
        //}

        #endregion

        
        #region output

        public event EventHandler<ConsoleFunctionOutputEventArgs> OutputReady;

        public string Output { get; private set; }

        public int DataStartRowNumber { get; private set; }
        public int DataEndRowNumber { get; private set; }


        private void FireOutputReadyEvent(object sender, ConsoleFunctionOutputEventArgs args)
        {
            if (this.OutputReady != null)
                this.OutputReady(sender, args);
        }

        public void NotifyOutputReady(object sender, string output)
        {
            ConsoleFunctionOutputEventArgs args = new ConsoleFunctionOutputEventArgs(output, false);

            this.Output += args.Output;
            this.FireOutputReadyEvent(sender, args);
        }

        public void NotifyOutputReady(object sender, string output, bool isDataRow, bool newLineBefore = false)
        {
            if (newLineBefore)
                this.NotifyOutputReady(sender, Environment.NewLine);

            ConsoleFunctionOutputEventArgs args = new ConsoleFunctionOutputEventArgs(output, isDataRow);

            if (isDataRow)
            {
                this.DataRowCount++;
                args.DataRowNum = this.DataRowCount;
            }

            this.Output += args.Output;
            this.FireOutputReadyEvent(sender, args);
        }

        public void NotifyOutputReady(object sender, ConsoleFunctionOutputEventArgs args)
        {
            if (args.IsDataRow)
            {
                this.DataRowCount++;
                args.DataRowNum = this.DataRowCount;
            }

            this.Output += args.Output;
            this.FireOutputReadyEvent(sender, args);
        }

        public int DataRowCount { get; private set; } 
        
        #endregion

        #region input

        public event EventHandler<ConsoleFunctionOutputGetInputEventArgs> GetInput;

        private void FireGetInputEvent(object sender, ConsoleFunctionOutputGetInputEventArgs args)
        {
            if (this.GetInput != null)
                this.GetInput(sender, args);
        }

        public string NotifyGetInput(object sender, string prompt, bool newLineBefore = false)
        {
            if (newLineBefore)
                this.NotifyOutputReady(sender, Environment.NewLine);

            ConsoleFunctionOutputGetInputEventArgs args = new ConsoleFunctionOutputGetInputEventArgs(null, prompt);
            this.FireGetInputEvent(sender, args);
            return args.UserInput;
        }
        public string NotifyGetInput(object sender, string prePromptMsg, string prompt, bool newLineBefore = false)
        {
            if (newLineBefore)
                this.NotifyOutputReady(sender, Environment.NewLine);

            ConsoleFunctionOutputGetInputEventArgs args = new ConsoleFunctionOutputGetInputEventArgs(prePromptMsg, prompt);
            this.FireGetInputEvent(sender, args);
            string input = args.UserInput;
            return input;
        }

        public void NotifyGetInput(object sender, ConsoleFunctionOutputGetInputEventArgs args)
        {
            this.FireGetInputEvent(sender, args);
        }

        #endregion


    }
}
