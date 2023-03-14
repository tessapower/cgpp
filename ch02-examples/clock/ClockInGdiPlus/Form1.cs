using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ClockInGdiPlus
{

    
    public partial class Form1 : Form
    {

        Graphics dc = null;

        Brush brNavy = new SolidBrush(Color.FromName("Navy"));
        Brush brRed = new SolidBrush(Color.FromName("Red"));
        Brush brWhite = new SolidBrush(Color.FromName("White"));

        Pen penRed = new Pen(new SolidBrush(Color.FromName("Red")), 1.8f);
        


        //The hardwired geometry of the clockface
        Point ptCenter = new Point(150, 150);

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();


        public Form1()
        {
            InitializeComponent();

            // Initialization of the “drawing context”
            dc = this.CreateGraphics();



            // To simulate old GDI's most basic usage model, we are
            // using pixel coordinates even though GDI+ does support
            // device-independent coord systems.
            dc.PageUnit = GraphicsUnit.Pixel;
            dc.PageScale = 1;
            this.Show();

            timer.Interval = 1000; // 1000 milliseconds = 1 second
            timer.Tick += new EventHandler(TimerEventProcessor);
            timer.Start();
        }

        private void PaintEntireClockImage() 
        {
            //Specifying the “hardwired” geometry of the clockface:
            //This section creates data structures representing geometry
            //   but does not do any actual drawing yet.
            int lengthFaceRadius = 100;
            Point ptCenter = new Point(150, 150);
            Rectangle circleBounds = new Rectangle(
                ptCenter.X - lengthFaceRadius /*X of upper-left*/ ,
                ptCenter.Y - lengthFaceRadius /*Y of upper-left*/ ,
                2 * lengthFaceRadius, 2 * lengthFaceRadius); /*width,height*/

            // Initializing the GDI object whose methods provide
            // the actual painting functionality:
            // A Graphic object can draw outlined/filled primitives,  
            // given the geometry specification and
            // a set of appearance attributes (pen/brush).
            Graphics grafobj = this.CreateGraphics();

            // Painting the circular face via 4 steps:
            // 1. Setting up a light-gray brush
            Brush brGray = new SolidBrush(Color.FromName("LightGray"));
            // 2. Using the brush to paint a filled circle
            grafobj.FillEllipse(brGray, circleBounds);
            // 3. Setting up a 4-pixel-width black pen
            //Pen penBlack = new Pen(Color.FromName("Black"), 4);
            // 4. Using the pen to draw the circle’s outline
            //grafobj.DrawEllipse(penBlack, circleBounds);

            // Painting the white dot of radius 5, at 12 o’clock, 
            // using a custom function
            //PaintDot(grafobj, 5,
            //         ptCenter.X, ptCenter.Y - lengthFaceRadius + 10);

            // Painting the hands using custom functions that receive 
            // a rotation parameter (in degrees, clockwise)
            PaintHourHand(grafobj, ptCenter.X, ptCenter.Y, angleCurHour);
            PaintMinuteHand(grafobj, ptCenter.X, ptCenter.Y, angleCurMinute);
            PaintSecondHand(grafobj, ptCenter.X, ptCenter.Y, angleCurSecond);

        }



        private void PaintDot(Graphics dc, int radius, int offX, int offY)
        {
            Brush brWhite = new SolidBrush(Color.FromName("White"));
            dc.FillEllipse(brWhite, offX-radius, offY-radius,2*radius,2*radius);

        }


        private void PaintHourHand(Graphics dc, int offX, int offY, double rotAngle)
        {
            Matrix transformer = new Matrix();
            transformer.Scale((float)1.7, (float)0.7, MatrixOrder.Append);
            transformer.Rotate((float)rotAngle, MatrixOrder.Append);
            transformer.Translate(offX, offY, MatrixOrder.Append);

            Point[] path =
                new Point[] {
                new Point(-3,-10),
                new Point(-2,80),
                new Point(0,90),
                new Point(2,80),
                new Point(3,-10)};

            transformer.TransformPoints(path);

            dc.FillPolygon(brNavy, path);

        }


        private void PaintMinuteHand(Graphics dc, int offX, int offY, double rotAngle)
        {
            // The geometry is specified in a "local" coord system, with
            // the base of the hand located at (0,0) and the hand pointing "up")
            // (meaning into negative-Y-coordinate space).
            Point[] path =                 
                new Point[] {
                new Point(-3,-10),
                new Point(-2,80),
                new Point(0,90),
                new Point(2,80),
                new Point(3,-10)};

            // We thus need to transform the geometry by first rotating
            // and then moving to the clockface’s actual centerpoint.
            Matrix transformer = new Matrix();
            transformer.Rotate((float)rotAngle, MatrixOrder.Append);
            transformer.Translate(offX, offY, MatrixOrder.Append);
            transformer.TransformPoints(path);

            dc.FillPolygon(brNavy, path);
        }



        private void PaintSecondHand_polygon(Graphics dc, int offX, int offY, double rotAngle)
        {
            // (See the commentary for DrawMinuteHand.)

            Matrix transformer = new Matrix();
            transformer.Translate(offX, offY);
            transformer.Rotate((float)rotAngle);

            // 'M -1,0 h 3 v -90 h -1 

            Point[] path =
                new Point[] {
                new Point(-1, 5),
                new Point(-1,-95),
                new Point(1,-95),
                new Point(1, 5),
                new Point(-1, 5)};

            transformer.TransformPoints(path);

            dc.FillPolygon(brRed, path);
        }




        private void PaintSecondHand(Graphics dc, int offX, int offY, double rotAngle)
        {
            // (See the commentary for DrawMinuteHand.)

            Matrix transformer = new Matrix();
            transformer.Translate(offX, offY);
            transformer.Rotate((float)rotAngle);

            // 'M -1,0 h 3 v -90 h -1 

            Point[] path =
                new Point[] {
                new Point(-1, 5),
                new Point(-1,-90),
                new Point(1,-90),
                new Point(1, 5),
                new Point(-1, 5)};

            transformer.TransformPoints(path);

            dc.FillPolygon(brRed, path);

            dc.DrawLine(penRed, new Point(0, 5), new Point(0, -90));

        }


        // This is called whenever the graphics system senses that
        // the part of the screen controlled by this application
        // has been "damaged" and needs refreshing.
        private void OnPaintEvent(object sender, PaintEventArgs e)
        {
            DetermineHandOrientations();
            PaintEntireClockImage();
        }


        private void TimerEventProcessor(Object myObject,
                                            EventArgs myEventArgs)
        {
            this.Invalidate();
        }



        double angleCurHour;
        double angleCurMinute;
        double angleCurSecond;

        private void DetermineHandOrientations()
        {
            DateTime curtime = DateTime.Now;
            double curhour = curtime.Hour;
            double curminute = curtime.Minute;
            double cursecond = curtime.Second;

            curhour = curhour + (curminute / 60.0) + (cursecond / 60.0 / 60.0);
            curminute = curminute + (cursecond / 60.0);

            angleCurHour = (curhour / 12.0) * 360.0  + 180.0;
            angleCurMinute = (curminute / 60.0) * 360.0 + 180.0;
            angleCurSecond = (cursecond/60.0) * 360.0 + 180.0;
            
        }
    }
}