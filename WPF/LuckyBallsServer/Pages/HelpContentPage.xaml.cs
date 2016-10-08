using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LuckyBallsData;
using LuckyBallsServer.Data;
using LuckyBallsData.Reference;

namespace LuckyBallsServer.Pages
{
    /// <summary>
    /// Interaction logic for HelpContentPage.xaml
    /// </summary>
    public partial class HelpContentPage : Page
    {
        private HelpBuilder _helpBuilder = null;
        private Topic _editingTopic = null;

        public HelpContentPage()
        {
            InitializeComponent();

            this.Loaded += delegate
            {
                _helpBuilder = LBDataManager.GetDataMgr().GetHelp();

                TopicList.ItemsSource = _helpBuilder.GetTopics();
                NoteList.ItemsSource = _helpBuilder.GetNotes();
            };
        }

        private void TopicList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = TopicList.SelectedItem as Topic;
            if (topic != null)
            {
                _editingTopic = topic;

                EditTopic(_editingTopic.Title, _editingTopic.Description, _editingTopic.GetNoteIDs());
            }
        }

        private void NoteList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Note note = NoteList.SelectedItem as Note;
            if (note != null)
            {
                NoteText.Text = note.Content;
            }
        }

        private void EditTopic(string title, string description, int[] nodeIds)
        {
            TitleEditor.Text = title;
            DescriptionEditor.Text = description;

            List<Note> notes = new List<Note>();
            if (nodeIds != null)
            {
                foreach (int nodeId in nodeIds)
                {
                    Note note = _helpBuilder.GetNote(nodeId);
                    if (note != null)
                        notes.Add(note);
                }
            }
            TopicNoteList.ItemsSource = notes;
        }

        private void AddTopicButton_Click(object sender, RoutedEventArgs e)
        {
            // sync topic titile.
            string title = TitleEditor.Text;
            if (title == "")
            {
                MessageBox.Show("Invalid title");
                return;
            }

            string description = DescriptionEditor.Text;
            if (description == "")
            {
                MessageBox.Show("Invalid description");
                return;
            }

            string notes = "";
            if (TopicNoteList.Items.Count > 0)
            {
                foreach (object item in TopicNoteList.Items)
                {
                    Note note = item as Note;

                    if (notes != "")
                        notes += " ";

                    notes += note.ID.ToString();
                }
            }

            // edit this topic.
            _editingTopic = _helpBuilder.AddTopic(title, description, notes);
            TopicList.SelectedItem = null;

            // refresh the topic list.
            TopicList.ItemsSource = null;
            TopicList.ItemsSource = _helpBuilder.GetTopics();

            TopicList.SelectedItem = _editingTopic;

            _helpBuilder.SetDirty();
        }

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Note selectedNote = NoteList.SelectedItem as Note;
            if (selectedNote != null)
            {
                selectedNote.Content = NoteText.Text;
            }

            // refresh the note list.
            NoteList.ItemsSource = null;
            NoteList.ItemsSource = _helpBuilder.GetNotes();

            // refresh topic note list as well.
            var list = TopicNoteList.ItemsSource;
            TopicNoteList.ItemsSource = null;
            TopicNoteList.ItemsSource = list;

            _helpBuilder.SetDirty();
        }

        private void CommitTopicButton_Click(object sender, RoutedEventArgs e)
        {
            // sync topic titile.
            string title = TitleEditor.Text;
            if (title == "")
            {
                MessageBox.Show("Invalid title");
                return;
            }

            string description = DescriptionEditor.Text;
            if (description == "")
            {
                MessageBox.Show("Invalid description");
                return;
            }

            if (TopicNoteList.Items.Count <= 0)
            {
                MessageBox.Show("No note selected.");
                return;
            }

            string notes = "";
            foreach (object item in TopicNoteList.Items)
            {
                Note note = item as Note;

                if (notes != "")
                    notes += " ";

                notes += note.ID.ToString();
            }

            if (_editingTopic == null)
            {
                // create a topic.
                _editingTopic = _helpBuilder.AddTopic(title, description, notes);
            }
            else
            {
                // change titile
                _editingTopic.Title = title;
                _editingTopic.Description = description;

                _editingTopic.Notes = notes;
            }

            // refresh the topic list.
            TopicList.ItemsSource = null;
            TopicList.ItemsSource = _helpBuilder.GetTopics();

            _helpBuilder.SetDirty();
        }

        private void AddTopicNoteButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedNoteToTopic();
        }

        private void RemoveTopicNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Note selectedNote = TopicNoteList.SelectedItem as Note;
            if (selectedNote != null)
            {
                List<Note> notes = TopicNoteList.ItemsSource as List<Note>;
                notes.Remove(selectedNote);

                TopicNoteList.ItemsSource = null;
                TopicNoteList.ItemsSource = notes;

                _helpBuilder.SetDirty();
            }
        }

        private void SaveHelpButton_Click(object sender, RoutedEventArgs e)
        {
            // save the help content.
            LBDataManager.GetDataMgr().SaveHelp();

            MessageBox.Show("Done!");
        }

        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            // create a note.
            Note newNote = _helpBuilder.AddNote(NoteText.Text);

            // refresh the note list.
            NoteList.ItemsSource = null;
            NoteList.ItemsSource = _helpBuilder.GetNotes();

            // refresh topic note list as well.
            var list = TopicNoteList.ItemsSource;
            TopicNoteList.ItemsSource = null;
            TopicNoteList.ItemsSource = list;

            _helpBuilder.SetDirty();
        }

        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Button delBtn = sender as Button;
            Note note = delBtn.DataContext as Note;
            if (note != null)
            {
                _helpBuilder.RemoveNote(note);
                _helpBuilder.SetDirty();

                // refresh the note list.
                NoteList.ItemsSource = null;
                NoteList.ItemsSource = _helpBuilder.GetNotes();
            }
        }

        private void RemoveTopicButton_Click(object sender, RoutedEventArgs e)
        {
            Button delBtn = sender as Button;
            Topic topic = delBtn.DataContext as Topic;
            if (topic != null)
            {
                _helpBuilder.RemoveTopic(topic);
                _helpBuilder.SetDirty();

                // refresh the topic list.
                TopicList.ItemsSource = null;
                TopicList.ItemsSource = _helpBuilder.GetTopics();
            }
        }

        private void NoteList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddSelectedNoteToTopic();
        }

        private void AddSelectedNoteToTopic()
        {
            Note selectedNote = NoteList.SelectedItem as Note;
            if (selectedNote != null)
            {
                // existing?
                foreach (object item in TopicNoteList.Items)
                {
                    Note note = item as Note;
                    if (note.ID == selectedNote.ID)
                        return;
                }

                List<Note> notes = TopicNoteList.ItemsSource as List<Note>;
                notes.Add(selectedNote);

                TopicNoteList.ItemsSource = null;
                TopicNoteList.ItemsSource = notes;

                _helpBuilder.SetDirty();
            }
        }
    }
}
