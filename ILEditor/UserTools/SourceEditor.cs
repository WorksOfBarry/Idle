﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.IO;
using ILEditor.Classes;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ILEditor.Classes.LanguageTools;
using System.Threading;
using ICSharpCode.AvalonEdit;
using System.Xml;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using FindReplace;
using ICSharpCode.AvalonEdit.CodeCompletion;
using System.Windows.Input;

namespace ILEditor.UserTools
{
    public enum ILELanguage
    {
        None,
        CL,
        CPP,
        RPG,
        SQL,
        COBOL
    }

    public partial class SourceEditor : UserControl
    {
        private TextEditor textEditor = null;
        private ILELanguage Language;
        private int RcdLen;

        public SourceEditor(String LocalFile, ILELanguage Language = ILELanguage.None, int RecordLength = 0)
        {
            InitializeComponent();

            //https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting

            this.Language = Language;
            this.RcdLen = RecordLength;

            textEditor = new TextEditor();
            textEditor.ShowLineNumbers = true;
            textEditor.Text = File.ReadAllText(LocalFile);

            textEditor.FontFamily = new System.Windows.Media.FontFamily(IBMi.CurrentSystem.GetValue("FONT"));
            textEditor.FontSize = float.Parse(IBMi.CurrentSystem.GetValue("ZOOM"));

            textEditor.TextChanged += TextEditor_TextChanged;

            textEditor.Options.ConvertTabsToSpaces = true;
            textEditor.Options.EnableTextDragDrop = false;
            textEditor.Options.IndentationSize = int.Parse(IBMi.CurrentSystem.GetValue("INDENT_SIZE"));
            textEditor.Options.ShowSpaces = (IBMi.CurrentSystem.GetValue("SHOW_SPACES") == "true");
            textEditor.Options.HighlightCurrentLine = (IBMi.CurrentSystem.GetValue("HIGHLIGHT_CURRENT_LINE") == "true");

            textEditor.Options.AllowScrollBelowDocument = true;
            
            if (this.RcdLen > 0)
            {
                textEditor.Options.ShowColumnRuler = true;
                textEditor.Options.ColumnRulerPosition = this.RcdLen;
            }

            //SearchPanel.Install(textEditor);
            SearchReplacePanel.Install(textEditor);

            string lang = "";
            switch (Language)
            {
                case ILELanguage.RPG:
                    lang = "RPG.xml";
                    break;
                case ILELanguage.SQL:
                    lang = "SQL.xml";
                    break;
                case ILELanguage.CPP:
                    lang = "CPP.xml";
                    break;
                case ILELanguage.CL:
                    lang = "CL.xml";
                    break;
                case ILELanguage.COBOL:
                    lang = "COBOL.xml";
                    break;
            }

            if (File.Exists(Program.SYNTAXDIR + lang))
            {
                Stream xshd_stream = File.OpenRead(Program.SYNTAXDIR + lang);
                XmlTextReader xshd_reader = new XmlTextReader(xshd_stream);
                // Apply the new syntax highlighting definition.
                textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(xshd_reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                xshd_reader.Close();
                xshd_stream.Close();
            }

            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;
            host.Child = textEditor;
            this.Controls.Add(host);
            textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
        }
        void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			DocumentLine line = textEditor.Document.GetLineByOffset(textEditor.CaretOffset);
			String line_text_to_cursor = textEditor.Document.GetText(line.Offset, (textEditor.CaretOffset-line.Offset));
			string lastWord = line_text_to_cursor.Split(' ').Last();
			// Open code completion after the user has pressed dot:
			completionWindow = new CompletionWindow(textEditor.TextArea);
			IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;

			string[] words = textEditor.Text.Split(' ');
			HashSet<string> set = new HashSet<string>(words);
			string[] result = new string[set.Count];
			set.CopyTo(result);

			data.Clear();
			Array.Sort(result);

			Boolean has_items = false;
			foreach (var word in result)
			{
				String trimmed_word = word.Trim();
				if ((lastWord.Length > 0) && (trimmed_word.StartsWith(lastWord) && (!trimmed_word.Equals(lastWord))))
				{
					data.Add(new AutoCompleteData(trimmed_word, trimmed_word));
					has_items = true;
				}
			}
			if (has_items)
			{
				completionWindow.Show();
				
				completionWindow.Closed += delegate
				{
					completionWindow = null;
				};
			}
		}
        
        public string GetText()
        {
            return textEditor.Text;
        }

        public void GotoLine(int line, int col)
        {
            line++; col++;
            int pos = textEditor.Document.GetOffset(line, col);
            textEditor.ScrollToLine(line);
            textEditor.CaretOffset = pos;
            textEditor.Focus();
        }

        public void Zoom(float change)
        {
            if (textEditor.FontSize + change > 5 && textEditor.FontSize + change < 100)
            {
                textEditor.FontSize += change;
                IBMi.CurrentSystem.SetValue("ZOOM", textEditor.FontSize.ToString());
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            if (!this.Parent.Text.EndsWith("*"))
            {
                this.Parent.Text += "*";
            }

            DocumentLine line = textEditor.Document.GetLineByOffset(textEditor.CaretOffset);
            int col = textEditor.CaretOffset - line.Offset;
            Editor.TheEditor.SetStatus(line.LineNumber.ToString() + ", " + col.ToString());
        }
        
        #region RPG

        public void ConvertSelectedRPG()
        {
            if (textEditor.SelectedText == "")
            {
                MessageBox.Show("Please highlight the code you want to convert and then try the conversion again.", "Fixed-To-Free", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string[] lines = textEditor.SelectedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                string freeForm = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    freeForm = RPGFree.getFree(lines[i]);
                    if (freeForm != "")
                    {
                        lines[i] = freeForm;
                    }
                }

                textEditor.SelectedText = String.Join(Environment.NewLine, lines);
            }

        }
        #endregion

        #region CL

        public void FormatCL()
        {
            string[] Lines = textEditor.Text.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.None);
            textEditor.Clear();
            int length = (RcdLen > 0 ? RcdLen : 80);
            textEditor.AppendText(String.Join(Environment.NewLine, CLFile.CorrectLines(Lines, length)));
        }
        #endregion
    }
}
