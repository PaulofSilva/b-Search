using System;
using System.Windows.Forms;

namespace bSearch.Windows.Controls
{
	/// <summary>
	/// Represents a Windows picture box control for displaying an image.
	/// </summary>

	public class PictureButton : System.Windows.Forms.PictureBox
	{
      /// <summary>
      /// Raised when control is clicked.
      /// </summary>
 
      public new EventHandler Click;

      /// <summary>
      /// Initializes a new instance of the PictureButton class.
      /// </summary>
      /// <history>

		public PictureButton() : base()
		{
			this.Cursor = Cursors.Hand;
		}

      /// <summary>
      /// Used to draw the image disabled if the control is disabled.
      /// </summary>
      /// <param name="e">system parameter</param>

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint (e);

         if (!this.Enabled && this.Image != null)
         {
            ControlPaint.DrawImageDisabled(e.Graphics, this.Image, 0, 0, this.BackColor);
         }
      }

      /// <summary>
      /// Validates that only the left mouse button was clicked.
      /// </summary>
      /// <param name="e">system parameter</param>

      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);

         if (e.Button == MouseButtons.Left)
         {
            if (e.X > this.Width || e.X < 0 || e.Y > this.Height || e.Y < 0)
               OnMouseLeave(new EventArgs());
            else if (Click != null)
               Click(this, new EventArgs());
         }
      }
	}
}