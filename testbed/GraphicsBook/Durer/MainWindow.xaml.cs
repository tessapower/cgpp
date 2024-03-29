﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

// Exercise 3.2: The dots at the corners of the rendered cube in Figure 3.8
// appear behind the edges, which doesn't look natural; alter the program to
// draw the dots after the segments so that it looks better. Alter it again to
// not draw the dots at all, and only draw the segments.

namespace GraphicsBook {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        GraphPaper gp = null;
        // Flag for allowing sliders, etc., to influence display.
        bool ready = false;

        public MainWindow() {
            InitializeComponent();
            InitializeCommands();
            // Now add some graphical items in the main Canvas,
            // whose name is "GraphPaper"
            gp = this.FindName("Paper") as GraphPaper;

            // Build a table of vertices:
            const int nPoints = 8;
            const int nEdges = 12;
            double[,] vtable = new double[nPoints, 3]{
                {-0.5, -0.5, 2.5},
                {-0.5,  0.5, 2.5},
                { 0.5,  0.5, 2.5},
                { 0.5, -0.5, 2.5},
                {-0.5, -0.5, 3.5},
                {-0.5,  0.5, 3.5},
                { 0.5,  0.5, 3.5},
                { 0.5, -0.5, 3.5}};

            // Build a table of edges
            int [,] etable = new int[nEdges, 2]{
                {0, 1}, {1, 2}, {2, 3}, {3, 0},  // one face
                {0, 4}, {1, 5}, {2, 6}, {3, 7},  // joining edges
                {4, 5}, {5, 6}, {6, 7}, {7, 4}}; // opposite face

            const double xmin = -0.5;
            const double xmax =  0.5;
            const double ymin = -0.5;
            const double ymax =  0.5;

            Point [] pictureVertices = new Point[nPoints];
            const double scale = 100.0;
            for (int i = 0; i < nPoints; i++) {
                var x = vtable[i, 0];
                var y = vtable[i, 1];
                var z = vtable[i, 2];

                var xprime = x / z;
                var yprime = y / z;

                pictureVertices[i].X = scale * (1-(xprime - xmin) / (xmax - xmin)); // x / z
                pictureVertices[i].Y = scale * (yprime - ymin) / (ymax - ymin);     // y / z
            }

            for (var i = 0; i < nEdges; i++) {
                var n1 = etable[i, 0];
                var n2 = etable[i, 1];
                gp.Children.Add(new Segment(pictureVertices[n1], pictureVertices[n2]));
            }

            // Draw point for each vertex in pictureVertices
            // Comment this out to draw only the segments
            foreach (var p in pictureVertices) {
                gp.Children.Add(new Dot(p.X, p.Y));
            }

            ready = true; // Now we're ready to have sliders and buttons influence the display.
        }

#region Interaction handling -- sliders and buttons
        /* Vestigial handling-code from Testbed2DApp -- unused in this project. */

        /* Event handler for a click on button one */
        public void b1Click(object sender, RoutedEventArgs e) {
            Debug.Print("Button one clicked!\n");
            e.Handled = true; // don't propagate the click any further
        }

        void slider1change(object sender,
            RoutedPropertyChangedEventArgs<double> e) {
            Debug.Print("Slider changed, ready = " + ready 
                + ", and val = " + e.NewValue + ".\n");
            e.Handled = true;
            if (ready) {}
        }

        public void b2Click(object sender, RoutedEventArgs e) {
            Debug.Print("Button two clicked!\n");
            e.Handled = true; // don't propagate the click any further
        }
#endregion

#region Menu, command, and keypress handling
        protected static RoutedCommand ExitCommand;

        protected void InitializeCommands() {
            InputGestureCollection inp = new InputGestureCollection();
            inp.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ExitCommand = new RoutedCommand("Exit", typeof(MainWindow), inp);
            CommandBindings.Add(
                new CommandBinding(ExitCommand, CloseApp)
            );
            CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Close, CloseApp)
            );
            CommandBindings.Add(
                new CommandBinding(ApplicationCommands.New, NewCommandHandler)
            );
        }

        void NewCommandHandler(Object sender, ExecutedRoutedEventArgs e) {
            MessageBox.Show("You selected the New command",
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
        }

        // Announce keypresses, EXCEPT for CTRL, ALT, SHIFT, CAPS-LOCK, and
        // "SYSTEM" (which is how Windows seems to refer to the "ALT" keys on
        // my keyboard) modifier keys. Note that keypresses that represent
        // commands (like ctrl-N for "new") get trapped and never get
        // to this handler.
        void KeyDownHandler(object sender, KeyEventArgs e) {
            if ((e.Key != Key.LeftCtrl) &&
                (e.Key != Key.RightCtrl) &&
                (e.Key != Key.LeftAlt) &&
                (e.Key != Key.RightAlt) &&
                (e.Key != Key.System) &&
                (e.Key != Key.Capital) &&
                (e.Key != Key.LeftShift) &&
                (e.Key != Key.RightShift)) {
                MessageBox.Show(String.Format("[{0}]  {1} received @ {2}",
                                        e.Key,
                                        e.RoutedEvent.Name,
                                        DateTime.Now.ToLongTimeString()),
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
            }
        }

        void CloseApp(Object sender, ExecutedRoutedEventArgs args) {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Really Exit?",
                                Title,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question)
               ) Close();
        }
#endregion //Menu, command and keypress handling
    }
}