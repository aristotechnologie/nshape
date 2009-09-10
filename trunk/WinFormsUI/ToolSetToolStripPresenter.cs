﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

using Dataweb.nShape;
using Dataweb.nShape.Advanced;
using Dataweb.nShape.Controllers;


namespace Dataweb.nShape.WinFormsUI {
	
	public partial class ToolSetToolStripPresenter : ToolStrip {
		
		public ToolSetToolStripPresenter() {
			InitializeComponent();
		}


		public ToolSetToolStripPresenter(IContainer container) {
			container.Add(this);
			InitializeComponent();
		}


		public ToolSetController ToolSetController {
			get { return toolSetController; }
			set {
				if (toolSetController != null) UnregisterToolBoxEventHandlers();
				toolSetController = value;
				if (toolSetController != null) RegisterToolBoxEventHandlers();
			}
		}


		private ToolStripItem FindItem(Tool tool) {
			for (int i = Items.Count - 1; i >= 0; --i) {
				if (Items[i].Tag == tool)
					return Items[i];
			}
			return null;
		}


		private ToolStripItem CreateItem(Tool tool) {
			ToolStripButton button = new ToolStripButton(null, tool.SmallIcon);
			button.Tag = tool;
			button.CheckOnClick = true;
			//button.Click += toolStripItem_Click;
			//button.DoubleClick += toolBoxStrip_DoubleClick;
			button.ToolTipText = tool.ToolTipText;
			button.DoubleClickEnabled = true;
			return button;
		}

		#region (Un)Registering for events

		private void RegisterToolStripEvents() {
			this.ItemClicked += new ToolStripItemClickedEventHandler(ToolSetToolStripPresenter_ItemClicked);
		}

		private void RegisterToolBoxEventHandlers() {
			toolSetController.Cleared += toolSetController_Cleared;
			toolSetController.ToolAdded += toolSetController_ToolAdded;
			toolSetController.ToolChanged += toolSetController_ToolChanged;
			toolSetController.ToolRemoved += toolSetController_ToolRemoved;
			toolSetController.ToolSelected += toolSetController_ToolSelected;
		}


		private void UnregisterToolBoxEventHandlers() {
			toolSetController.ToolSelected -= toolSetController_ToolSelected;
			toolSetController.ToolRemoved -= toolSetController_ToolRemoved;
			toolSetController.ToolChanged -= toolSetController_ToolChanged;
			toolSetController.ToolAdded -= toolSetController_ToolAdded;
			toolSetController.Cleared -= toolSetController_Cleared;
		}

		#endregion


		#region ToolStrip event handler implementations

		private void ToolSetToolStripPresenter_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			if (e.ClickedItem.Tag is Tool) toolSetController.SelectTool((Tool)e.ClickedItem.Tag);
		}


		private void toolStripItem_Click(object sender, EventArgs e) {
		}

		#endregion


		#region ToolSetController event handler implementations

		private void toolSetController_ToolSelected(object sender, ToolEventArgs e) {
			ToolStripItem item = FindItem(e.Tool);
			if (item != null) item.Select();
		}


		private void toolSetController_Cleared(object sender, EventArgs args) {
			Items.Clear();
		}


		private void toolSetController_ToolAdded(object sender, ToolEventArgs e) {
			// SaveChanges the list view: Move this to ToolSetListViewPresenter
			if (FindItem(e.Tool) != null)
				throw new nShapeException(string.Format("Tool {0} already exists.", e.Tool.Title));
			ToolStripItem item = CreateItem(e.Tool);
			// ToDo: Put the tool into the right group, seperrate groups by seperators
			//   if (!string.IsNullOrEmpty(e.Tool.Category)) {
			//      foreach (ListViewGroup group in listView.Groups) {
			//         if (group.Name.Equals(e.Tool.Category, StringComparison.InvariantCultureIgnoreCase)) {
			//            item.Group = group;
			//            break;
			//         }
			//      }
			//      if (item.Group == null) {
			//         ListViewGroup group = new ListViewGroup(e.Tool.Category, e.Tool.Category);
			//         listView.Groups.Add(group);
			//         item.Group = group;
			//      }
			//   }
			//   // Adjust the heading column in the list view
			//   if (listView.Columns[headerName] != null) {
			//      Graphics gfx = Graphics.FromHwnd(listView.Handle);
			//      if (gfx != null) {
			//         SizeF txtSize = gfx.MeasureString(e.Tool.Title, listView.Font);
			//         if (listView.Columns[headerName].Width < txtSize.Width + listView.SmallImageList.ImageSize.Width)
			//            listView.Columns[headerName].Width = (int)Math.Ceiling(txtSize.Width + listView.SmallImageList.ImageSize.Width);
			//         gfx.Dispose();
			//         gfx = null;
			//      }
			//   }
			// Add the item and select if default
			Items.Add(item);
		}


		private void toolSetController_ToolChanged(object sender, ToolEventArgs e) {
			//if (listView != null && e.Tool is TemplateTool) {
			//   listView.BeginUpdate();
			//   ListViewItem item = FindItem(e.Tool);
			//   if (item != null) {
			//      TemplateTool tool = (TemplateTool)e.Tool;
			//      largeImageList.Images[item.ImageIndex] = tool.LargeIcon;
			//      smallImageList.Images[item.ImageIndex] = tool.SmallIcon;
			//   }
			//   listView.EndUpdate();
			//}
		}


		private void toolSetController_ToolRemoved(object sender, ToolEventArgs e) {
			//listView.SuspendLayout();
			//ListViewItem item = FindItem(e.Tool);
			//listView.Items.Remove(item);
			//listView.ResumeLayout();
		}

		#endregion


		private ToolSetController toolSetController;
	}
}