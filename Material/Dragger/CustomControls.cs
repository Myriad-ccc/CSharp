using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShapeShift
{
    public class CustomButton : Control
    {
        private bool IsPressed, disablePress;
        private bool IsHovering, disableHover;
        private readonly bool CustomSize = true;

        public CustomButton(float? cFontSize = null, bool? disableOnHover = null, bool? disableOnPress = null)
        {
            string cFont = "Gilroy-Medium";
            if (!cFontSize.HasValue)
            {
                cFontSize = 24f;
                CustomSize = false;
            }

            try { this.Font = new Font(cFont, (float)cFontSize); }
            catch (Exception)
            {
                cFont = "Arial";
                this.Font = new Font(cFont, (float)cFontSize);
            }
            this.BackColor = Color.FromArgb(255, 25, 25, 25);
            this.ForeColor = Color.FromArgb(255, 225, 225, 225);
            this.DoubleBuffered = true;

            this.MouseEnter += (s, ev) => { IsHovering = true; this.Invalidate(); };
            this.MouseLeave += (s, ev) => { IsHovering = false; IsPressed = false; this.Invalidate(); };
            this.MouseDown += (s, ev) => { IsPressed = true; this.Invalidate(); };
            this.MouseUp += (s, ev) => { IsPressed = false; this.Invalidate(); };

            disableHover = disableOnHover ?? false;
            disablePress = disableOnPress ?? false;
        }
        protected override void OnPaint(PaintEventArgs eventArgs)
        {
            base.OnPaint(eventArgs);

            Graphics g = eventArgs.Graphics;

            Color fill = this.BackColor;

            if (!disableHover && IsHovering)
            {
               fill = Color.FromArgb(255, 43, 43, 43);
            }
            if (!disablePress && IsPressed)
            {
                fill = Color.FromArgb(255, 73, 73, 73);
            }

            using (Brush fillBrush = new SolidBrush(fill))
            {
                g.FillRectangle(fillBrush, this.ClientRectangle);
            }
            using (Pen borderPen = new Pen(Color.FromArgb(255, 185, 185, 185)))
            {
                g.DrawRectangle(borderPen, new Rectangle(new Point(0, 0), new Size(this.Width - 1, this.Height - 1)));
            }
            if (CustomSize) return;
            TextRenderer.DrawText(g, this.Text, this.Font, this.ClientRectangle, this.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    public class CustomComboBox : ComboBox
    {
        public CustomComboBox(float? cFontSize = null)
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;

            string cFont = "Gilroy-Medium";
            if (!cFontSize.HasValue) cFontSize = 24f;
            try { this.Font = new Font(cFont, (float)cFontSize); }
            catch (Exception)
            {
                cFont = "Arial";
                this.Font = new Font(cFont, (float)cFontSize);
            }
            this.BackColor = Color.FromArgb(255, 25, 25, 25);
            this.ForeColor = Color.FromArgb(255, 225, 225, 225);

            this.DoubleBuffered = true;
            this.FlatStyle = FlatStyle.Flat;
            this.SetStyle(ControlStyles.Selectable, false);

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += CustomComboBox_DrawItem;
        }

        private void CustomComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;

            if (e.Index < 0 || e.Index >= this.Items.Count)
            {
                e.DrawBackground();
                return;
            }

            object item = this.Items[e.Index];
            string itemText = item?.ToString() ?? string.Empty;

            Color itemBackColor = this.BackColor;
            Color itemForeColor = this.ForeColor;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                itemBackColor = SystemColors.Highlight;
                itemForeColor = SystemColors.HighlightText;
            }
            else
            {
                itemBackColor = this.BackColor;
                itemForeColor = this.ForeColor;
            }

            using (Brush backgroundBrush = new SolidBrush(itemBackColor))
            {
                g.FillRectangle(backgroundBrush, e.Bounds);
            }
            using (Brush textBrush = new SolidBrush(itemForeColor))
            {
                g.DrawString(itemText, e.Font, textBrush, e.Bounds, StringFormat.GenericDefault);
            }
        }
    }

    public class CustomTextBox : TextBox
    {
        public CustomTextBox()
        {
            string cFont = "Gilroy-Medium";
            float cFontSize = 24;
            try { this.Font = new Font(cFont, (float)cFontSize); }
            catch (Exception)
            {
                cFont = "Arial";
                this.Font = new Font(cFont, (float)cFontSize);
            }
            this.BackColor = Color.FromArgb(255, 25, 25, 25);
            this.ForeColor = Color.FromArgb(255, 225, 225, 225);
            this.BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            Color fill = this.BackColor;

            using (Brush fillBrush = new SolidBrush(fill))
            {
                g.FillRectangle(fillBrush, this.ClientRectangle);
            }
            using (Pen borderPen = new Pen(Color.FromArgb(255, 185, 185, 185)))
            {
                g.DrawRectangle(borderPen, new Rectangle(
                    new Point(this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1), this.Size));
            }
        }
    }

    public class CustomLabel : Label
    {
        public CustomLabel(string tryFont = null, string defaultFont = null, float? fontSize = null)
        {
            string DefaultFont = defaultFont ?? "Arial";
            string DefaultTryFont = "Gilroy-Medium";
            string Font = tryFont ?? DefaultTryFont;
            float FontSize = fontSize ?? 24f;
            try 
            { 
                this.Font = new Font(Font, (float)FontSize); 
            }
            catch (Exception)
            {
                this.Font = new Font(DefaultFont, (float)FontSize);
            }

            this.DoubleBuffered = true;
            this.BorderStyle = BorderStyle.None;
            this.FlatStyle = FlatStyle.Flat;
            this.ForeColor = Color.FromArgb(255, 225, 225, 225);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (Brush textBrush = new SolidBrush(this.ForeColor))
            {
                g.DrawString(this.Text, this.Font, textBrush, this.ClientRectangle);
            }
        }
    }

    public class CustomPanel : Panel
    {
        public CustomPanel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(255, 35, 35, 35);
        }
    }
}
