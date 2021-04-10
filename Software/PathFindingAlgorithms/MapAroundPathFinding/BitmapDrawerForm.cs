using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapAroundPathFinding
{
    public partial class BitmapDrawerForm : Form
    {
        private int
            _xLeftOffset,
            _yTopOffset,
            _xRightOffset,
            _yBottomOffset;

        public BitmapDrawerForm()
        {
            InitializeComponent();

            _xLeftOffset = PictureBox.Location.X;
            _yBottomOffset = PictureBox.Location.Y;
            _xRightOffset = Size.Width - (PictureBox.Size.Width + _xLeftOffset);
            _yTopOffset = Size.Height - (PictureBox.Size.Height + _yBottomOffset);
        }

        public void DrawBitmap(Bitmap bitmap)
        {
            PictureBox.Size = bitmap.Size;
            PictureBox.Image = bitmap;

            int formWidth = _xLeftOffset + PictureBox.Width + _xRightOffset;
            int formHeight = _yBottomOffset + PictureBox.Height + _yTopOffset;
            Size = new Size(formWidth, formHeight);
            PictureBox.Location = new Point(_xLeftOffset, _yBottomOffset);
        }
    }
}
