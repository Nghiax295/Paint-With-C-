using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint_App_102230308
{
    public partial class PaintApp: Form
    {
        private bool isDrawing = false;
        private Point previousPoint;
        private Pen drawingPen = new Pen(Color.Black, 1);
        private Pen penEraser = new Pen(Color.White, 1);
        private Bitmap bitmap;
        private Graphics graphics;
        private Color selectedColor = Color.Black;
        private int penSize = 0;
        private string selectedTool = "Pen";
        private Point startPoint;
        private Point endPoint;
        private Stack<Bitmap> Undo = new Stack<Bitmap>();
        private Stack<Bitmap> Redo = new Stack<Bitmap>();
        private Shape shape = new Shape();
        public PaintApp()
        {
            InitializeComponent();
            ChangeColor_Click();
            UndoRedo_Click();
            this.Resize += PaintAppResize;

            bitmap = new Bitmap(pictureBoxShowImage.Width, pictureBoxShowImage.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            pictureBoxShowImage.Image = bitmap;
            trackBarSizePen.Value = penSize;

            trackBarZoom.Value = 5;
            pictureBoxShowImage.Width = 800;
            pictureBoxShowImage.Height = 500;
        }

        private Point realLocation(Point mousePoint)
        {
            float zoomLevel = (float)pictureBoxShowImage.Width / bitmap.Width;
            int x = (int)(mousePoint.X / zoomLevel);
            int y = (int)(mousePoint.Y / zoomLevel);
            return new Point(x, y);
        }


        private void saveState()
        {
            Undo.Push(new Bitmap(bitmap));
            Redo.Clear();
        }

        private void UndoRedo_Click()
        {
            buttonUndo.Click += buttonUndo_Click;
            buttonRedo.Click += buttonRedo_Click;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            pictureBoxShowImage.Invalidate();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.Title = "Chọn hình ảnh để mở";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                bitmap = new Bitmap(filePath);
                graphics = Graphics.FromImage(bitmap);
                pictureBoxShowImage.Image = bitmap;
                pictureBoxShowImage.Visible = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonNewfile_Click(object sender, EventArgs e)
        {
            graphics.Clear(Color.White);
            trackBarZoom.Value = 5;
            pictureBoxShowImage.Width = 800;
            pictureBoxShowImage.Height = 500;
            pictureBoxShowImage.Invalidate();
            CenterPictureBox();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.Title = "Chọn hình ảnh để mở";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                bitmap = new Bitmap(filePath);
                graphics = Graphics.FromImage(bitmap);
                pictureBoxShowImage.Image = bitmap;
                pictureBoxShowImage.Visible = true;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog.FileName);
            }
        }

        private void ChangeColor_Click()
        {
            pictureBox1.Click += PictureBoxColor_Click;
            pictureBox2.Click += PictureBoxColor_Click;
            pictureBox3.Click += PictureBoxColor_Click;
            pictureBox4.Click += PictureBoxColor_Click;
            pictureBox5.Click += PictureBoxColor_Click;
            pictureBox6.Click += PictureBoxColor_Click;
            pictureBox7.Click += PictureBoxColor_Click;
            pictureBox8.Click += PictureBoxColor_Click;
            pictureBox9.Click += PictureBoxColor_Click;
            pictureBox10.Click += PictureBoxColor_Click;
        }

        private void PictureBoxColor_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;

            if (clickedPictureBox != null)
            {
                selectedColor = clickedPictureBox.BackColor;
                drawingPen.Color = selectedColor;
                pictureBoxColorSelection.BackColor = selectedColor;
            }
        }

        private void pictureBoxShowImage_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            previousPoint = realLocation(e.Location);
            startPoint = realLocation(e.Location);
            saveState();
        }

        private void pictureBoxShowImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                if (selectedTool == "Pen")
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLine(drawingPen, previousPoint, realLocation(e.Location));
                    }
                    previousPoint = realLocation(e.Location);
                }
                else if(selectedTool == "Eraser")
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        penEraser.Width = penSize;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLine(penEraser, previousPoint, realLocation(e.Location));
                    }
                    previousPoint = realLocation(e.Location);
                }
                pictureBoxShowImage.Invalidate();
            }
        }

        private void pictureBoxShowImage_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
            endPoint = realLocation(e.Location);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (selectedTool == "Line")
                {
                    g.DrawLine(drawingPen, startPoint, endPoint);
                }
                else if (selectedTool == "Rectangle")
                {
                    g.DrawRectangle(drawingPen, shape.GetRectangle(startPoint, endPoint));
                }
                else if (selectedTool == "Circle")
                {
                    g.DrawEllipse(drawingPen, shape.GetRectangle(startPoint, endPoint));
                }
                else if (selectedTool == "Triangle")
                {
                    Point[] points = shape.GetTrianglePoints(startPoint, endPoint);
                    g.DrawPolygon(drawingPen, points);
                }
                else if (selectedTool == "Square")
                {
                    Rectangle rect = shape.GetSquare(startPoint, endPoint);
                    g.DrawRectangle(drawingPen, rect);
                }
                else if (selectedTool == "Diamond")
                {
                    Point[] points = shape.GetDiamondPoints(startPoint, endPoint);
                    g.DrawPolygon(drawingPen, points);
                }
            pictureBoxShowImage.Invalidate();
            }
        }
        
        private void buttonPen_Click(object sender, EventArgs e)
        {
            selectedTool = "Pen";
        }

        private void buttonEraser_Click(object sender, EventArgs e)
        {
            selectedTool = "Eraser";
        }

        private void buttonLine_Click(object sender, EventArgs e)
        {
            selectedTool = "Line";
        }

        private void buttonDiamond_Click(object sender, EventArgs e)
        {
            selectedTool = "Diamond";
        }

        private void buttonCircle_Click(object sender, EventArgs e)
        {
            selectedTool = "Circle";
        }

        private void buttonSquare_Click(object sender, EventArgs e)
        {
            selectedTool = "Square";
        }

        private void buttonTriangle_Click(object sender, EventArgs e)
        {
            selectedTool = "Triangle";
        }

        private void buttonRectangle_Click(object sender, EventArgs e)
        {
            selectedTool = "Rectangle";
        }

        private void buttonFillShape_Click(object sender, EventArgs e)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (SolidBrush brush = new SolidBrush(selectedColor))
                {
                    if (selectedTool == "Rectangle")
                    {
                        g.FillRectangle(brush, shape.GetRectangle(startPoint, endPoint));
                    }
                    else if (selectedTool == "Circle")
                    {
                        g.FillEllipse(brush, shape.GetRectangle(startPoint, endPoint));
                    }
                    else if (selectedTool == "Triangle")
                    {
                        Point[] points = shape.GetTrianglePoints(startPoint, endPoint);
                        g.FillPolygon(brush, points);
                    }
                    else if (selectedTool == "Square")
                    {
                        Rectangle rect = shape.GetSquare(startPoint, endPoint);
                        g.FillRectangle(brush, rect);
                    }
                }
            }
            pictureBoxShowImage.Invalidate();
        }

        private void pictureBoxColorSelection_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                selectedColor = colorDialog.Color;
                drawingPen.Color = selectedColor;
                pictureBoxColorSelection.BackColor = selectedColor;
            }
        }

        private void trackBarSizePen_Scroll(object sender, EventArgs e)
        {
            penSize = (trackBarSizePen.Value + 1) * 5;
            drawingPen.Width = penSize;
        }

        private void trackBarZoom_Scroll(object sender, EventArgs e)
        {
            double zoomLevel = (double)trackBarZoom.Value/5 + 0.2;
            pictureBoxShowImage.Width = (int)(bitmap.Width * zoomLevel);
            pictureBoxShowImage.Height = (int)(bitmap.Height * zoomLevel);
            pictureBoxShowImage.SizeMode = PictureBoxSizeMode.StretchImage;

            CenterPictureBox();
        }

        private void CenterPictureBox()
        {
            int containerWidth = this.ClientSize.Width;
            int containerHeight = this.ClientSize.Height;

            int newX = (containerWidth - pictureBoxShowImage.Width) / 2;
            int newY = 150;

            pictureBoxShowImage.Location = new Point(newX, newY);
        }

        private void PaintAppResize(object sender, EventArgs e)
        {
            CenterPictureBox();
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            if (Undo.Count > 0)
            {
                Redo.Push(new Bitmap(bitmap));

                bitmap = new Bitmap(Undo.Pop());
                pictureBoxShowImage.Image = bitmap;
                graphics = Graphics.FromImage(bitmap);
                pictureBoxShowImage.Invalidate();
            }
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            if (Redo.Count > 0)
            {
                Undo.Push(new Bitmap(bitmap));

                bitmap = new Bitmap(Redo.Pop());
                pictureBoxShowImage.Image = bitmap;
                graphics = Graphics.FromImage(bitmap);
                pictureBoxShowImage.Invalidate();
            }
        }
    }
}
