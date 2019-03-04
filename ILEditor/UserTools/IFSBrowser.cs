﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using FluentFTP;
using ILEditor.Classes;
using ILEditor.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ILEditor.UserTools
{
	public partial class IFSBrowser : DockContent
	{
		private TreeNode RightClickedNode;

		public IFSBrowser()
		{
			InitializeComponent();
			Text = "IFS Browser";
		}

		public void Reload()
		{
			bool     exists;
			TreeNode node;
			var      DirList = new List<string>();

			files.Nodes.Clear();
			if (IBMi.IsConnected)
				new Thread((ThreadStart) delegate
				{
					DirList.Add(IBMi.CurrentSystem.GetValue("homeDir"));
					foreach (var dir in IBMi.CurrentSystem.GetValue("IFS_LINKS").Split('|'))
						if (dir.Trim() != "")
							DirList.Add(dir);

					foreach (var dir in DirList)
						if (dir.Trim() != "")
						{
							if (dir.Trim() == "")
								continue;

							exists = IBMi.DirExists(dir);
							Invoke((MethodInvoker) delegate
							{
								if (exists)
								{
									node = new TreeNode(dir, new[] {new TreeNode("Loading..", 2, 2)})
									{
										Tag = dir, ImageIndex = 0, SelectedImageIndex = 0
									};

									files.Nodes.Add(node);
								}
								else
								{
									files.Nodes.Add(new TreeNode("Directory '" + dir + "' was not located.", 3, 3));
								}
							});
						}
				}).Start();
			else
				files.Nodes.Add(new TreeNode("IFS Browsing only works when connected to the remote system.", 3, 3));
		}

		private void IFSBrowser_Load(object sender, EventArgs e)
		{
			Reload();
		}

		private void files_AfterExpand(object sender, TreeViewEventArgs e)
		{
			var      listing = new List<TreeNode>();
			TreeNode node;

			new Thread((ThreadStart) delegate
			{
				var items = IBMi.GetListing(e.Node.Tag.ToString());

				foreach (var item in items)
				{
					if (item.Name.Contains("/"))
					{
						var lastIndex = item.Name.LastIndexOf("/");
						var tempName  = item.Name.Substring(lastIndex + 1, item.Name.Length - lastIndex - 1);
						item.FullName = item.FullName.Substring(0, item.FullName.Length - item.Name.Length - 1) +
						                "/" +
						                tempName;

						item.Name = tempName;
					}

					FileCache.AddStreamFile(item.FullName);

					node     = new TreeNode(item.Name);
					node.Tag = item.FullName;
					switch (item.Type)
					{
						case FtpFileSystemObjectType.Directory:
							node.ImageIndex         = 0;
							node.SelectedImageIndex = 0;
							node.Nodes.Add(new TreeNode("Loading..", 2, 2));
							listing.Add(node);

							break;
						case FtpFileSystemObjectType.File:
							node.ImageIndex         = 1;
							node.SelectedImageIndex = 1;
							listing.Add(node);

							break;
					}
				}

				if (listing.Count == 0)
					listing.Add(new TreeNode("Directory is empty.", 2, 2));

				Invoke((MethodInvoker) delegate
				{
					e.Node.Nodes.Clear();
					e.Node.Nodes.AddRange(listing.ToArray());
				});
			}).Start();
		}

		private void files_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (files.SelectedNode?.Tag == null)
				return;

			if (files.SelectedNode.Nodes.Count == 0)
				Editor.OpenSource(new RemoteSource("", files.SelectedNode.Tag.ToString()));
		}

		private void files_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Label == null)
				return;

			if (e.Label == e.Node.Text)
				return;

			if (e.Label == "")
			{
				e.CancelEdit = true;

				return;
			}

			string pathResult;

			if (e.Node.Nodes.Count > 0)
				pathResult = IBMi.RenameDir(e.Node.Tag.ToString(), e.Label);
			else
				pathResult = IBMi.RenameFile(e.Node.Tag.ToString(), e.Label);

			var success = e.Node.Tag.ToString() != pathResult;

			if (success)
				e.Node.Tag = pathResult;

			e.CancelEdit = !success;
		}

		private void files_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				files.SelectedNode = e.Node;
				RightClickedNode   = e.Node;

				createToolStripMenuItem.Enabled           = e.Node.Nodes.Count > 0;
				deleteToolStripMenuItem.Enabled           = e.Node.Parent != null;
				renameToolStripMenuItem.Enabled           = e.Node.Parent != null;
				makeShortcutToolStripMenuItem.Enabled     = e.Node.Parent != null && e.Node.Nodes.Count > 0;
				setHomeDirectoryToolStripMenuItem.Enabled = e.Node.Nodes.Count > 0;
				rightClickMenu.Show(Cursor.Position);
			}
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RightClickedNode == null)
				return;

			var result = MessageBox.Show("Are you sure you want to delete '" + RightClickedNode.Tag + "'?",
				"Warning",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Asterisk);

			if (result == DialogResult.Yes)
			{
				if (RightClickedNode.Nodes.Count == 0)
					IBMi.DeleteFile(RightClickedNode.Tag.ToString());
				else
					IBMi.DeleteDir(RightClickedNode.Tag.ToString());

				RightClickedNode.Remove();
			}
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RightClickedNode?.BeginEdit();
		}

		private void fileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RightClickedNode != null)
			{
				var window = new CreateStreamFile(RightClickedNode.Tag + "/");
				window.ShowDialog();

				if (window.Result != null)
					Editor.OpenExistingSource(window.Result);
			}
		}

		private void directoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RightClickedNode != null)
			{
				var window = new CreateDirectory(RightClickedNode.Tag + "/");
				window.ShowDialog();

				RightClickedNode.Collapse();
				RightClickedNode.Expand();
			}
		}

		private void makeShortcutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RightClickedNode != null)
			{
				var dirs = IBMi.CurrentSystem.GetValue("IFS_LINKS").Split('|').ToList();
				dirs.Add(RightClickedNode.Tag.ToString());
				IBMi.CurrentSystem.SetValue("IFS_LINKS", string.Join("|", dirs));
				Reload();
			}
		}

		private void setHomeDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RightClickedNode != null)
			{
				IBMi.CurrentSystem.SetValue("homeDir", RightClickedNode.Tag.ToString());
				Editor.TheEditor.SetStatus("Job home directory set to: " + RightClickedNode.Tag);
			}
		}

		private void manageDirs_Click(object sender, EventArgs e)
		{
			new IFSManager().ShowDialog();
			Reload();
		}
	}
}