using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using bSearch.Core;
using libbSearch;

namespace bSearch.Windows.Forms
{
   /// <summary>
   /// Used to manipulate the exclusion list.
   /// </summary>
   /// <remarks>
   ///   bSearch File Searching Utility. 
   ///   Copyright (C) 2006 BigLevel Lda.
   /// 
   ///   The author may be contacted at:
   ///   suporte@bigLevel.pt or comercial@biglevel.pt
   /// </remarks>

   public partial class frmExclusions : BaseForm
   {
      private List<FilterItem> filterItems = new List<FilterItem>();
      private bool inhibitAutoCheck;

      /// <summary>
      /// Gets the Exclusion list from this dialog.
      /// </summary>
      public List<FilterItem> FilterItems { get { return filterItems; } }

      /// <summary>
      /// Create the instance of this form.
      /// </summary>

      public frmExclusions(List<FilterItem> items)
      {
         InitializeComponent();

         filterItems = items;

         API.ListViewExtensions.SetTheme(lstExclusions);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void frmExclusions_Load(object sender, EventArgs e)
      {
         Language.ProcessForm(this);

         StartPosition = FormStartPosition.Manual;

         // set default window size and position
         Rectangle defaultBounds = new Rectangle(frmMain.ActiveForm.Left + ((frmMain.ActiveForm.Width - this.Width) / 2), frmMain.ActiveForm.Top + ((frmMain.ActiveForm.Height - this.Height) / 2), this.Width, this.Height);

         // get window size and position from user settings
         int width = GeneralSettings.ExclusionsDisplaySavePosition && GeneralSettings.ExclusionsDisplayWidth != -1 ? GeneralSettings.ExclusionsDisplayWidth : defaultBounds.Width;
         int height = GeneralSettings.ExclusionsDisplaySavePosition && GeneralSettings.ExclusionsDisplayHeight != -1 ? GeneralSettings.ExclusionsDisplayHeight : defaultBounds.Height;
         int left = GeneralSettings.ExclusionsDisplaySavePosition && GeneralSettings.ExclusionsDisplayLeft != -1 ? GeneralSettings.ExclusionsDisplayLeft : defaultBounds.X;
         int top = GeneralSettings.ExclusionsDisplaySavePosition && GeneralSettings.ExclusionsDisplayTop != -1 ? GeneralSettings.ExclusionsDisplayTop : defaultBounds.Y;

         // set window size and position from user settings
         Bounds = new Rectangle(left, top, width, height);

         // if form can't find a screen to fit on reset to default on primary screen
         if (!Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(Bounds)))
         {
            Bounds = defaultBounds;
         }

         // set column text
         lstExclusions.Columns[0].Text = Language.GetGenericText("Exclusions.Enabled", "Enabled");
         lstExclusions.Columns[1].Text = Language.GetGenericText("Exclusions.Category", "Category");
         lstExclusions.Columns[2].Text = Language.GetGenericText("Exclusions.Type", "Type");
         lstExclusions.Columns[3].Text = Language.GetGenericText("Exclusions.Value", "Value");
         lstExclusions.Columns[4].Text = Language.GetGenericText("Exclusions.Option", "Option");

         // set column widths from user settings

         if ((GeneralSettings.ExclusionsDisplayColumnEnabledWidth != -1) && GeneralSettings.ExclusionsDisplaySavePosition)
            lstExclusions.Columns[0].Width = GeneralSettings.ExclusionsDisplayColumnEnabledWidth;
         else
            lstExclusions.Columns[0].Width = Constants.EXCLUSIONS_DISPLAY_COLUMN_WIDTH_ENABLED * GeneralSettings.WindowsDPIPerCentSetting / 100;

         if ((GeneralSettings.ExclusionsDisplayColumnCategoryWidth != -1) && GeneralSettings.ExclusionsDisplaySavePosition)
            lstExclusions.Columns[1].Width = GeneralSettings.ExclusionsDisplayColumnCategoryWidth;
         else
            lstExclusions.Columns[1].Width = Constants.EXCLUSIONS_DISPLAY_COLUMN_WIDTH_CATEGORY * GeneralSettings.WindowsDPIPerCentSetting / 100;

         if ((GeneralSettings.ExclusionsDisplayColumnTypeWidth != -1) && GeneralSettings.ExclusionsDisplaySavePosition)
            lstExclusions.Columns[2].Width = GeneralSettings.ExclusionsDisplayColumnTypeWidth;
         else
            lstExclusions.Columns[2].Width = Constants.EXCLUSIONS_DISPLAY_COLUMN_WIDTH_TYPE * GeneralSettings.WindowsDPIPerCentSetting / 100;

         if ((GeneralSettings.ExclusionsDisplayColumnValueWidth != -1) && GeneralSettings.ExclusionsDisplaySavePosition)
            lstExclusions.Columns[3].Width = GeneralSettings.ExclusionsDisplayColumnValueWidth;
         else
            lstExclusions.Columns[3].Width = Constants.EXCLUSIONS_DISPLAY_COLUMN_WIDTH_VALUE * GeneralSettings.WindowsDPIPerCentSetting / 100;

         if ((GeneralSettings.ExclusionsDisplayColumnOptionWidth != -1) && GeneralSettings.ExclusionsDisplaySavePosition)
            lstExclusions.Columns[4].Width = GeneralSettings.ExclusionsDisplayColumnOptionWidth;
         else
            lstExclusions.Columns[4].Width = Constants.EXCLUSIONS_DISPLAY_COLUMN_WIDTH_OPTION * GeneralSettings.WindowsDPIPerCentSetting / 100;
         
         LoadExclusions();

         SetButtonState();
      }

      /// <summary>
      /// Cancel the dialog.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnCancel_Click(object sender, EventArgs e)
      {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }

      /// <summary>
      /// Save the exclusion items.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnOK_Click(object sender, EventArgs e)
      {
         filterItems = GetCurrentFilterItems();

         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      /// <summary>
      /// Delete the selected exclusion items.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnDelete_Click(object sender, EventArgs e)
      {
         // remove
         if (lstExclusions.SelectedItems.Count > 0)
         {
            foreach (ListViewItem item in lstExclusions.SelectedItems)
            {
               lstExclusions.Items.Remove(item);
            }
            SetButtonState();
         }

         this.DialogResult = DialogResult.None;
      }

      /// <summary>
      /// Edit an exclusion item.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnEdit_Click(object sender, EventArgs e)
      {
         if (lstExclusions.SelectedItems.Count > 0)
         {
            // get currently selected exclusion
            var item = lstExclusions.SelectedItems[0].Tag as FilterItem;
            item.Enabled = lstExclusions.SelectedItems[0].Checked;

            var dlg = new frmAddEditExclusions(GetCurrentFilterItems(), item);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
               item = dlg.CurrentItem;
               var listItem = GetListViewItem(item);
               lstExclusions.SelectedItems[0].Checked = item.Enabled;
               lstExclusions.SelectedItems[0].Tag = item;

               lstExclusions.SelectedItems[0].SubItems[1].Text = listItem.SubItems[1].Text;
               lstExclusions.SelectedItems[0].SubItems[2].Text = listItem.SubItems[2].Text;
               lstExclusions.SelectedItems[0].SubItems[3].Text = listItem.SubItems[3].Text;
               lstExclusions.SelectedItems[0].SubItems[4].Text = listItem.SubItems[4].Text;

               SetButtonState();
            }
         }

         this.DialogResult = DialogResult.None;
      }

      /// <summary>
      /// Add a new exclusion item.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnAdd_Click(object sender, EventArgs e)
      {
         var dlg = new frmAddEditExclusions(GetCurrentFilterItems(), null);
         if (dlg.ShowDialog(this) == DialogResult.OK)
         {
            // create new entry
            lstExclusions.Items.Add(GetListViewItem(dlg.CurrentItem));

            SetButtonState();
         }

         this.DialogResult = DialogResult.None;
      }

      /// <summary>
      /// Update the button states.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         SetButtonState();
      }

      /// <summary>
      /// Edit the selected exclusion entry.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_DoubleClick(object sender, EventArgs e)
      {
         Point clientPoint = lstExclusions.PointToClient(Control.MousePosition);
         ListViewItem item = lstExclusions.GetItemAt(clientPoint.X, clientPoint.Y);

         if (item != null)
         {
            item.Selected = true;
            btnEdit_Click(null, null);
         }
      }

      /// <summary>
      /// Handles the key down event (supports ctrl-a, del).
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         if (e.KeyCode == Keys.A && e.Control) //ctrl+a  Select All
         {
            foreach (ListViewItem item in lstExclusions.Items)
            {
               item.Selected = true;
            }
         }

         if (e.KeyCode == Keys.Delete) //delete
         {
            btnDelete_Click(sender, EventArgs.Empty);
         }
      }

      /// <summary>
      /// Sets the TextEditor's button states depending on if one is selected.
      /// </summary>

      private void SetButtonState()
      {
         if (lstExclusions.SelectedItems.Count > 0)
         {
            btnDelete.Enabled = true;
            btnEdit.Enabled = true;
         }
         else
         {
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
         }
      }

      /// <summary>
      /// Loads the exclusion list.
      /// </summary>

      private void LoadExclusions()
      {
         if (filterItems != null && filterItems.Count > 0)
         {
            lstExclusions.BeginUpdate();
            foreach (FilterItem item in filterItems)
            {
               lstExclusions.Items.Add(GetListViewItem(item));
            }
            lstExclusions.EndUpdate();
         }
      }

      /// <summary>
      /// Get the list view item from the given FilterItem object.
      /// </summary>
      /// <param name="item">FilterItem object</param>
      /// <returns>ListViewItem object</returns>

      private ListViewItem GetListViewItem(FilterItem item)
      {
         ListViewItem listItem = new ListViewItem();
         listItem.Tag = item;
         listItem.Checked = item.Enabled;
         listItem.SubItems.Add(Language.GetGenericText("Exclusions." + item.FilterType.Category.ToString(), item.FilterType.Category.ToString()));
         listItem.SubItems.Add(Language.GetGenericText("Exclusions." + item.FilterType.SubCategory.ToString(), item.FilterType.SubCategory.ToString()));
         
         string valueText = item.Value;         
         string optionText = Language.GetGenericText("Exclusions." + item.ValueOption.ToString());
         string additionalInfo = string.Empty;
         if (item.ValueIgnoreCase)
         {
            additionalInfo = Language.GetGenericText("Exclusions.IgnoreCase");
         }
         else if (item.FilterType.Category == FilterType.Categories.File && item.FilterType.SubCategory == FilterType.SubCategories.Size && !string.IsNullOrEmpty(item.ValueSizeOption))
         {
            valueText = string.Format("{0} {1}", bSearch.Core.Convertors.ConvertFileSizeForDisplay(item.Value, item.ValueSizeOption), item.ValueSizeOption);
         }

         listItem.SubItems.Add(valueText);
         listItem.SubItems.Add(string.Format("{0}{1}", optionText, additionalInfo));

         return listItem;
      }

      /// <summary>
      /// Restore default exclusion list.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void btnRestoreDefaults_Click(object sender, EventArgs e)
      {
         lstExclusions.Items.Clear();

         filterItems = FilterItem.ConvertStringToFilterItems(Constants.DefaultFilterItems);
         LoadExclusions();
      }
      
      /// <summary>
      /// Handle not changing checked state of item when double clicking to edit it.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         inhibitAutoCheck = true;
      }

      /// <summary>
      /// Handle not changing checked state of item when double clicking to edit it.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
      {
         if (inhibitAutoCheck)
         {
            e.NewValue = e.CurrentValue;
         }
      }

      /// <summary>
      /// Handle not changing checked state of item when double clicking to edit it.
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void lstExclusions_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
      {
         inhibitAutoCheck = false;
      }

      /// <summary>
      /// Handles ListView Column Click event to allow Enabled column to toggle all checkboxes.
      /// </summary>
      /// <param name="sender">lstExclusions listview</param>
      /// <param name="e">column click arguments</param>

      private void lstExclusions_ColumnClick(object sender, ColumnClickEventArgs e)
      {
         // enabled column
         if (e.Column == 0)
         {
            bool allChecked = (lstExclusions.CheckedItems.Count == lstExclusions.Items.Count);
            foreach (ListViewItem item in lstExclusions.Items)
            {
               item.Checked = !allChecked;
            }
         }
      }

      /// <summary>
      /// Get a list of FilterItems that match the currently displayed FilterItems in this screen.
      /// </summary>
      /// <returns>List of FilterItems</returns>

      private List<FilterItem> GetCurrentFilterItems()
      {
         var list = new List<FilterItem>(lstExclusions.Items.Count);
         foreach (ListViewItem listItem in lstExclusions.Items)
         {
            var item = listItem.Tag as FilterItem;
            item.Enabled = listItem.Checked;
            list.Add(item);
         }

         return list;
      }

      /// <summary>
      /// Save column widths and window position (if enabled).
      /// </summary>
      /// <param name="sender">system parameter</param>
      /// <param name="e">system parameter</param>

      private void frmExclusionsDisplay_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (GeneralSettings.ExclusionsDisplaySavePosition)
         {
            GeneralSettings.ExclusionsDisplayTop = this.Top;
            GeneralSettings.ExclusionsDisplayLeft = this.Left;
            GeneralSettings.ExclusionsDisplayWidth = this.Width;
            GeneralSettings.ExclusionsDisplayHeight = this.Height;

            GeneralSettings.ExclusionsDisplayColumnEnabledWidth = lstExclusions.Columns[0].Width;
            GeneralSettings.ExclusionsDisplayColumnCategoryWidth = lstExclusions.Columns[1].Width;
            GeneralSettings.ExclusionsDisplayColumnTypeWidth = lstExclusions.Columns[2].Width;
            GeneralSettings.ExclusionsDisplayColumnValueWidth = lstExclusions.Columns[3].Width;
            GeneralSettings.ExclusionsDisplayColumnOptionWidth = lstExclusions.Columns[4].Width;
         }
      }
   }
}
