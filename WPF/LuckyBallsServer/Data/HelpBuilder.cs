using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData;
using LuckyBallsData.Util;
using LuckyBallsData.Reference;

namespace LuckyBallsServer.Data
{
    class HelpBuilder : HelpManager
    {
        private const string s_version = "1.0";

        private bool _dirty = false;

        private int _topicNextId = 1;
        private int _noteNextId = 1;
        
        public void SetDirty()
        {
            _dirty = true;
        }

        public Topic AddTopic(String title, string description, String notes)
        {
            Topic topic = new Topic() { ID = _topicNextId ++, Title = title, Description = description, Notes = notes };
            _topics.Add(topic.ID, topic);

            return topic;
        }

        public Note AddNote(String content)
        {
            Note note = new Note() { ID = _noteNextId++, Content = content };
            _notes.Add(note.ID, note);

            return note;
        }

        public void RemoveTopic(Topic topic)
        {
            _topics.Remove(topic.ID);
        }

        public void RemoveNote(Note note)
        {
            _notes.Remove(note.ID);
        }

        public void InitNextIDs()
        {
            foreach (Topic topic in GetTopics())
            {
                if (_topicNextId <= topic.ID)
                    _topicNextId = topic.ID + 1;
            }

            foreach (Note note in GetNotes())
            {
                if (_noteNextId <= note.ID)
                    _noteNextId = note.ID + 1;
            }
        }

        public void SaveToFile(string file)
        {
            if (_dirty)
            {
                DBXmlDocument xml = new DBXmlDocument();
                DBXmlNode root = xml.AddRoot("Help");
                root.SetAttribute("Version", s_version);

                Write(root);

                xml.Save(file);

                _dirty = false;
            }
        }
    }
}
