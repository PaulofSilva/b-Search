using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bSearch.Windows.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///   bSearch File Searching Utility.
    ///   Copyright (C) 2006 BigLevel Lda.
    /// 
    ///   The author may be contacted at:
    ///   suporte@bigLevel.pt or comercial@biglevel.pt
    /// </remarks>

    public partial class FilterValueType : UserControl
    {
        private ViewTypes currentViewType = ViewTypes.String;

        /// <summary>
        /// Available view types for this control.
        /// </summary>
        public enum ViewTypes
        {
            /// <summary>Display the DateTime picker</summary>
            DateTime,
            /// <summary>Display a TextBox</summary>
            String,
            /// <summary>Display the numeric up/down control</summary>
            Numeric,
            /// <summary>Display a numeric up/down control with a drop down for size type selection</summary>
            Size
        }

        /// <summary>
        /// Initialize the control.
        /// </summary>

        public FilterValueType()
        {
            InitializeComponent();

            numValue.Maximum = numSize.Maximum = decimal.MaxValue;

            // setup default view type
            SetViewType(currentViewType);
        }

        /// <summary>
        /// Handle resetting the value and controls.
        /// </summary>
        /// <param name="e">system parameter</param>

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ResetValue();
            ResetControls();
        }

        /// <summary>
        /// Reset the control's position and size to fall within the overall size of the control.
        /// </summary>

        public void ResetControls()
        {
            pnlDateTime.Left = txtValue.Left = numValue.Left = pnlSize.Left = 0;
            pnlDateTime.Top = txtValue.Top = numValue.Top = pnlSize.Top = 0;
            pnlDateTime.Width = txtValue.Width = numValue.Width = pnlSize.Width = this.Width;
            pnlDateTime.Height = txtValue.Height = numValue.Height = pnlSize.Height = this.Height;
        }

        /// <summary>
        /// Sets the view type.
        /// </summary>
        /// <param name="viewType">The view type to be displayed</param>

        public void SetViewType(ViewTypes viewType)
        {
            currentViewType = viewType;

            switch (currentViewType)
            {
                case ViewTypes.DateTime:
                    pnlDateTime.Visible = true;
                    txtValue.Visible = false;
                    numValue.Visible = false;
                    pnlSize.Visible = false;
                    break;

                case ViewTypes.String:
                    pnlDateTime.Visible = false;
                    txtValue.Visible = true;
                    numValue.Visible = false;
                    pnlSize.Visible = false;
                    break;

                case ViewTypes.Numeric:
                    pnlDateTime.Visible = false;
                    txtValue.Visible = false;
                    numValue.Visible = true;
                    pnlSize.Visible = false;
                    break;

                case ViewTypes.Size:
                    pnlDateTime.Visible = false;
                    txtValue.Visible = false;
                    numValue.Visible = false;
                    pnlSize.Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Resets all control values to the default.
        /// </summary>

        public void ResetValue()
        {
            var nowDateTime = DateTime.Now;
            dtpValue.Value = nowDateTime;
            dtpTimeValue.Value = nowDateTime;
            txtValue.Text = string.Empty;
            numValue.Value = 0;
            numSize.Value = 0;
            cboSize.SelectedIndex = 0;
        }

        /// <summary>
        /// Sets the size drop down to the give text value.
        /// </summary>
        /// <param name="itemText">byte,kb,mb,gb</param>

        public void SetSizeDropDown(string itemText)
        {
            cboSize.SelectedItem = itemText;
        }

        /// <summary>
        /// Retrieves the currently selected size type.
        /// </summary>
        /// <returns>string of byte,kb,mb,gb</returns>

        public string GetSizeDropDown()
        {
            return cboSize.SelectedItem.ToString();
        }

        /// <summary>
        /// Gets/Sets the value for the currently displayed view type.
        /// </summary>

        public string Value
        {
            get
            {
                switch (currentViewType)
                {
                    case ViewTypes.DateTime:
                        return string.Format("{0} {1}", dtpValue.Value.ToShortDateString(), dtpTimeValue.Value.ToLongTimeString());

                    case ViewTypes.Numeric:
                        return numValue.Value.ToString();

                    case ViewTypes.Size:
                        return Core.Convertors.ConvertFileSizeFromDisplay(numSize.Value.ToString(), cboSize.SelectedItem.ToString(), 0).ToString();

                    case ViewTypes.String:
                    default:
                        return txtValue.Text;
                }
            }

            set
            {
                switch (currentViewType)
                {
                    case ViewTypes.DateTime:
                        var valueDateTime = DateTime.Parse(value);
                        dtpValue.Value = valueDateTime;
                        dtpTimeValue.Value = valueDateTime;
                        break;

                    case ViewTypes.Numeric:
                        numValue.Value = decimal.Parse(value);
                        break;

                    case ViewTypes.Size:
                        numSize.Value = decimal.Parse(value);
                        break;

                    case ViewTypes.String:
                    default:
                        txtValue.Text = value;
                        break;
                }
            }
        }
    }
}
