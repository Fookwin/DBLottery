using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace LuckyBallSpirit.Controls
{
    public enum MessageBoxButton
    {
        OK = 0,
        OKCancel = 1,
        YesNo = 2,
        YesNoCancel = 3,
    }

    public enum MessageBoxResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
    }

    class MessageBox
    {
        static string okStr = "确定", cancelStr = "取消", yesStr = "是", noStr = "否", captionStr = "提示";

        public async static Task<IUICommand> Show(string message)
        {
            MessageBox msg = new MessageBox();
            return await msg.ShowMessage(message);
        }


        public async static Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageBox box = new MessageBox();

            var result = await box.ShowMessage(messageBoxText, caption, button);

            return getResult(result);
        }

        public async Task<IUICommand> ShowMessage(string message)
        {
            return await ShowMessage(message, captionStr, MessageBoxButton.OK);
        }


        public async Task<IUICommand> ShowMessage(string messageBoxText, string caption, MessageBoxButton button)
        {
            MessageDialog msg = new MessageDialog(messageBoxText, caption);

            switch (button)
            {
                case MessageBoxButton.OK:
                    {
                        msg.Commands.Add(new UICommand(okStr, CommandInvokedHandler));
                        msg.DefaultCommandIndex = 0;
                        msg.CancelCommandIndex = 0;
                        break;
                    }
                case MessageBoxButton.OKCancel:
                    {
                        msg.Commands.Add(new UICommand(okStr, CommandInvokedHandler));
                        msg.Commands.Add(new UICommand(cancelStr, CommandInvokedHandler));
                        msg.DefaultCommandIndex = 0;
                        msg.CancelCommandIndex = 1;
                        break;
                    }
                case MessageBoxButton.YesNo:
                    {
                        msg.Commands.Add(new UICommand(yesStr, CommandInvokedHandler));
                        msg.Commands.Add(new UICommand(noStr, CommandInvokedHandler));
                        msg.DefaultCommandIndex = 0;
                        msg.CancelCommandIndex = 1;
                        break;
                    }
                case MessageBoxButton.YesNoCancel:
                    {
                        msg.Commands.Add(new UICommand(yesStr, CommandInvokedHandler));
                        msg.Commands.Add(new UICommand(noStr, CommandInvokedHandler));
                        msg.Commands.Add(new UICommand(cancelStr, CommandInvokedHandler));
                        msg.DefaultCommandIndex = 1;
                        msg.CancelCommandIndex = 2;
                        break;
                    }
            }            

            IUICommand result = await msg.ShowAsync();
            return result;
        }

        public delegate void CompleteHandler(MessageBoxResult result);

        public CompleteHandler Complete = null;


        private void CommandInvokedHandler(IUICommand command)
        {
            if (Complete != null)
            {
                Complete(getResult(command));
            }
        }

        private static MessageBoxResult getResult(IUICommand command)
        {
            MessageBoxResult msgresult = MessageBoxResult.Cancel;
            if (command.Label == okStr)
            {
                msgresult = MessageBoxResult.OK;
            }
            else if (command.Label == cancelStr)
            {
                msgresult = MessageBoxResult.Cancel;
            }
            else if (command.Label == yesStr)
            {
                msgresult = MessageBoxResult.Yes;
            }
            else if (command.Label == noStr)
            {
                msgresult = MessageBoxResult.No;
            }
            else
            {
                msgresult = MessageBoxResult.None;
            }
            return msgresult;
        }
    }
}
