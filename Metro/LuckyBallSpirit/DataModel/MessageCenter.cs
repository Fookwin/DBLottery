using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallSpirit.Controls;

namespace LuckyBallSpirit.DataModel
{
    public enum MessageType
    {
        Information = 0,
        Error = 1,
        Warning = 2,
        Success = 3
    }

    enum MessagePriority
    {
        Queuing = 0,
        Immediate = 1
    }

    class MessageItem
    {
        public string Message
        {
            get;
            set;
        }

        public MessagePriority Priority
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }
    }

    class MessageCenter
    {
        private static MessageCenter _instance = null;

        private FlyoutMessageBox _messageBox = new FlyoutMessageBox();
        private Queue<MessageItem> _pendingMessages = new Queue<MessageItem>();

        public static void AddMessage(string message, MessageType type, MessagePriority prty, int duration)
        {
            MessageItem item = new MessageItem() { Message = message, Priority = prty, Duration = duration };

            if (prty == MessagePriority.Immediate)
            {
                // Show it immediately.
                Instance()._messageBox.SetMessage(item.Message, duration, type);
                Instance()._messageBox.Show();
            }
            else
            {
                // Push back to the message queue.
                Instance()._pendingMessages.Enqueue(item);
            }
        }

        private MessageCenter()
        {
        }

        private static MessageCenter Instance()
        {
            if (_instance == null)
                _instance = new MessageCenter();

            return _instance;
        }
    }
}
