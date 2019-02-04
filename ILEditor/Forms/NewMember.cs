﻿using System;
using System.IO;
using System.Windows.Forms;
using ILEditor.Classes;

namespace ILEditor.Forms
{
	public partial class NewMember : Form
	{
		public string _lib;
		public string _mbr;
		public string _spf;
		public string _text;
		public string _type;
		public bool   created;

		public NewMember(string Lib = "", string Spf = "")
		{
			InitializeComponent();

			lib.Text = Lib;
			spf.Text = Spf;
		}

		private void create_Click(object sender, EventArgs e)
		{
			var isValid = true;

			if (!IBMiUtils.IsValueObjectName(lib.Text))
				isValid = false;

			else if (!IBMiUtils.IsValueObjectName(spf.Text))
				isValid = false;

			else if (!IBMiUtils.IsValueObjectName(mbr.Text))
				isValid = false;

			if (isValid)
			{
				_lib  = lib.Text.Trim();
				_spf  = spf.Text.Trim();
				_mbr  = mbr.Text.Trim();
				_type = type.Text.Trim() == "" ? "*NONE" : type.Text.Trim();
				if (IBMi.IsConnected())
				{
					_text = text.Text.Trim() == "" ? "*BLANK" : "'" + text.Text.Trim() + "'";

					var command = "ADDPFM FILE(" +
					                 _lib +
					                 "/" +
					                 _spf +
					                 ") MBR(" +
					                 _mbr +
					                 ") TEXT(" +
					                 _text +
					                 ") SRCTYPE(" +
					                 _type +
					                 ")";

					if (IBMi.RemoteCommand(command)) //No error
						Close();
					else
						MessageBox.Show("Member not created.",
							"Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation);
				}
				else
				{
					if (_type == "*NONE")
						_type = "";

					var local = IBMiUtils.GetLocalFile(_lib, _spf, _mbr, _type);

					if (!File.Exists(local))
					{
						File.Create(local).Close();
						created = true;
						Close();
					}
					else
					{
						MessageBox.Show("Local member not created as already exists.",
							"Warning",
							MessageBoxButtons.OK,
							MessageBoxIcon.Exclamation);
					}
				}
			}
			else
			{
				MessageBox.Show("Provided member information not valid.",
					"Invalid member.",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}
		}
	}
}