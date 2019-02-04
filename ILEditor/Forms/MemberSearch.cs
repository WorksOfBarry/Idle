﻿using ILEditor.Classes;
using ILEditor.UserTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ILEditor.Forms
{
    public partial class MemberSearch : Form
    {
        public MemberSearch()
        {
            InitializeComponent();
        }

        private void openClone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new CloneWindow().ShowDialog();
        }

        private void search_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchVal.Text))
            {
                searchVal.Focus();
                MessageBox.Show("Search value cannot be blank.");
                return;
            }

            if (!IBMiUtils.IsValueObjectName(lib.Text))
            {
                lib.Focus();
                MessageBox.Show("Library name is not valid.");
                return;
            }

            if (!IBMiUtils.IsValueObjectName(spf.Text))
            {
                spf.Focus();
                MessageBox.Show("SPF name is not valid.");
                return;
            }

            Editor.TheEditor.AddTool(new MemberSearchListing(lib.Text, spf.Text, searchVal.Text, caseSense.Checked), WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            this.Close();
        }
    }
}
