using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace org.oikw.Launcher
{
    public partial class MainWindow : Window
    {
        static readonly string ConnectionString = @"Provider=Search.CollatorDSO;Extended Properties='Application=Windows'";
        OleDbConnection _connection;
        MainWindowModel _model;
        
        public MainWindow ()
        {
            InitializeComponent ();
            _model = new MainWindowModel (this);
            this.DataContext = _model;

            this.WindowStyle = System.Windows.WindowStyle.None;
            this.AllowsTransparency = true;
            MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e) { this.DragMove (); };
            KeyDown += delegate (object sender, KeyEventArgs e) {
                if (e.Key == Key.Escape)
                    this.Close ();
            };
            Loaded += delegate (object sender, RoutedEventArgs e) {
                Keyboard.Focus (SearchTextBox);
            };
            Closed += delegate (object sender, EventArgs e) {
                _connection.Close ();
            };
            SearchTextBox.KeyDown += delegate (object sender, KeyEventArgs e) {
                if (Keyboard.Modifiers == ModifierKeys.Control) {
                    if (e.Key == Key.H && (SearchTextBox.SelectionStart > 0 || SearchTextBox.SelectedText.Length > 0)) {
                        using (SearchTextBox.DeclareChangeBlock ()) {
                            if (SearchTextBox.SelectedText.Length == 0) {
                                SearchTextBox.SelectionStart = SearchTextBox.SelectionStart - 1;
                                SearchTextBox.SelectionLength = 1;
                            }
                            SearchTextBox.SelectedText = "";
                        }
                    } else if (e.Key == Key.D && SearchTextBox.SelectionStart < SearchTextBox.Text.Length) {
                        using (SearchTextBox.DeclareChangeBlock ()) {
                            if (SearchTextBox.SelectedText.Length == 0) {
                                SearchTextBox.SelectionLength = 1;
                            }
                            SearchTextBox.SelectedText = "";
                        }
                    }
                }
            };
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;

            _connection = new OleDbConnection (ConnectionString);
            _connection.Open ();
        }

        void SearchTextBox_TextChanged (object sender, TextChangedEventArgs e)
        {
            string raw_query_string = SearchTextBox.Text.Trim ().Replace ("\"", "").Replace("'", "");
            if (raw_query_string.Length == 0) {
                _model.ResultEntries = new ResultEntry[0];
                return;
            }

            List<ResultEntry> results = ExecuteQuery (_connection, raw_query_string);
            _model.ResultEntries = results.ToArray ();
        }

        static List<ResultEntry> ExecuteQuery (OleDbConnection connection, string search_query_string)
        {
            StringBuilder query = new StringBuilder ();
            query.Append ("SELECT System.ItemNameDisplay, System.ItemPathDisplay, System.ItemUrl, System.Kind FROM SYSTEMINDEX");
            query.Append (" WHERE Contains(System.ItemNameDisplay, '\"");
            query.Append (search_query_string);
            query.Append ("*\"') or Contains('\"");
            query.Append (search_query_string);
            query.Append ("*\"')");
            IDbCommand cmd = connection.CreateCommand ();
            cmd.CommandText = query.ToString ();
            IDataReader reader = cmd.ExecuteReader ();
            List<ResultEntry> results = new List<ResultEntry> ();
            object[] values = new object[reader.FieldCount];
            while (reader.Read ()) {
                ResultEntry entry = new ResultEntry { Name = reader.GetString (0), Path = reader.GetString (1), Url = reader.GetString (2) };
                entry.SetKinds (reader.GetValue (3) as Array);
                results.Add (entry);
            }
            return results;
        }

        [Flags]
        public enum ResultEntryKind
        {
            /* Main */
            None    = 0,
            Program = 1,
            Files   = 2,
            History = 4,

            /* Sub */
            Document = 256,
            Directory = 512
        }

        public class ResultEntry
        {
            public string Name { set; get; }
            public string Path { set; get; }
            public string Url { set; get; }
            public ResultEntryKind Kinds { private set; get; }
            public bool IsDirectory { get { return (Kinds & ResultEntryKind.Directory) != 0; } }

            public void SetKinds (Array obj)
            {
                Kinds = ResultEntryKind.None;
                if (obj != null) {
                    for (int i = 0; i < obj.GetLength (0); ++i) {
                        String kind = obj.GetValue (i) as String;
                        if (kind == null) continue;
                        kind = kind.ToLowerInvariant ();
                        if ("program".Equals (kind))
                            Kinds |= ResultEntryKind.Program;
                        else if ("document".Equals (kind))
                            Kinds |= ResultEntryKind.Files | ResultEntryKind.Document;
                        else if ("folder".Equals (kind))
                            Kinds |= ResultEntryKind.Files | ResultEntryKind.Directory;
                    }
                    if (Kinds != ResultEntryKind.None)
                        return;
                }
                if (Url.StartsWith ("iehistory://"))
                    Kinds = ResultEntryKind.History;
                else
                    Kinds = ResultEntryKind.Files;
            }

            public override string ToString ()
            {
                return this.Name;
            }
        }
    }

    public class MainWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        MainWindow _view;

        public MainWindowModel (MainWindow view)
        {
            _view = view;
        }

        protected virtual void OnPropertyChanged (string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
            }
        }

        MainWindow.ResultEntry[] _entries;
        public MainWindow.ResultEntry[] ResultEntries { get { return _entries; } set { _entries = value; OnPropertyChanged ("ResultEntries"); } }
    }
}
