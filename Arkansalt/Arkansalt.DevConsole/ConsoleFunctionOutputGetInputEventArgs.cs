using System;

namespace Arkansalt.DevConsole
{
    public class ConsoleFunctionOutputGetInputEventArgs : EventArgs
    {
        public ConsoleFunctionOutputGetInputEventArgs(string prePromptText, string promptText)
        {
            this.PrePromptText = prePromptText;
            this.PromptText = promptText;
        }


        public string PrePromptText { get; private set; }

        public string PromptText { get; private set; }


        public string UserInput
        {
            get
            {
                if (this._userInput == null)
                    this._userInput = string.Empty;
                return this._userInput;
            }
            set { this._userInput = value; }
        }

        private string _userInput = string.Empty;

    }
}