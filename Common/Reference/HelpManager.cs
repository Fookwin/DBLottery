using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData;
using LuckyBallsData.Util;

namespace LuckyBallsData.Reference
{
    public class Topic
    {
        public int ID
        {
            get;
            set;
        }

        public String Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public String Notes
        {
            get;
            set;
        }

        public int[] GetNoteIDs()
        {
            if (Notes == null || Notes == "")
                return null;

            String[] ids = Notes.Split(' ');

            int[] output = new int[ids.Count()];

            int index = 0;
            foreach (String id in ids)
            {
                output[index++] = Convert.ToInt32(id);
            }
            return output;
        }

        public void SaveToXml(ref DBXmlNode node)
        {
            node.SetAttribute("ID", ID.ToString());
            node.SetAttribute("Description", Description);
            node.SetAttribute("Title", Title);
            node.SetAttribute("Notes", Notes);
        }

        public void ReadFromXml(DBXmlNode node)
        {
            ID = Convert.ToInt32(node.GetAttribute("ID"));
            Description = node.GetAttribute("Description");
            Title = node.GetAttribute("Title");
            Notes = node.GetAttribute("Notes");
        }
    }

    public class Note
    {
        public int ID
        {
            get;
            set;
        }

        public String Content
        {
            get;
            set;
        }

        public void SaveToXml(ref DBXmlNode node)
        {
            node.SetAttribute("ID", ID.ToString());
            node.SetAttribute("Text", Content);
        }

        public void ReadFromXml(DBXmlNode node)
        {
            ID = Convert.ToInt32(node.GetAttribute("ID"));
            Content = node.GetAttribute("Text");
        }
    }

    public class HelpManager
    {
        protected Dictionary<int, Topic> _topics = new Dictionary<int, Topic>();
        protected Dictionary<int, Note> _notes = new Dictionary<int, Note>();

        public ICollection<Topic> GetTopics()
        {
            return _topics.Values;
        }

        public ICollection<Note> GetNotes()
        {
            return _notes.Values;
        }

        public Topic GetTopic(int id)
        {
            if (_topics.ContainsKey(id))
                return _topics[id];

            return null;
        }

        public Note GetNote(int id)
        {
            if (_notes.ContainsKey(id))
                return _notes[id];

            return null;
        }

        public List<Note> GetTopicNotes(Topic tp)
        {
            List<Note> notes = new List<Note>();
  
            foreach (int nodeId in tp.GetNoteIDs())
            {
                Note note = GetNote(nodeId);
                if (note != null)
                    notes.Add(note);
            }

            return notes;
        }

        public void Read(DBXmlNode node)
        {
            DBXmlNode topicsNode = node.FirstChildNode("Topics");
            foreach (DBXmlNode tpNode in topicsNode.ChildNodes())
            {
                Topic topic = new Topic();
                topic.ReadFromXml(tpNode);
                _topics.Add(topic.ID, topic);
            }

            DBXmlNode notesNode = node.FirstChildNode("Notes");
            foreach (DBXmlNode ndNode in notesNode.ChildNodes())
            {
                Note note = new Note();
                note.ReadFromXml(ndNode);
                _notes.Add(note.ID, note);
            }
        }

        public void Write(DBXmlNode node)
        {
            DBXmlNode topicsNode = node.AddChild("Topics");
            foreach (Topic tp in _topics.Values)
            {
                DBXmlNode tpNode = topicsNode.AddChild("Topic");
                tp.SaveToXml(ref tpNode);
            }

            DBXmlNode notesNode = node.AddChild("Notes");
            foreach (Note nd in _notes.Values)
            {
                DBXmlNode ndNode = notesNode.AddChild("Note");
                nd.SaveToXml(ref ndNode);
            }
        }
    }
}
