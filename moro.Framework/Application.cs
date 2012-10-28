// 
// Application.cs
//  
// Author:
//       Oleg Sur <oleg.sur@gmail.com>
// 
// Copyright (c) 2012 Oleg Sur
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using moro.Framework.Data;
using System.Linq;

namespace moro.Framework
{
	public class Application
	{
		public static Application Current { get; private set; }
		
		public Dictionary<object, object> Resources { get; private set; }
		public Window MainWindow { get; private	set; }		
		public IEnumerable<Window> Windows { get { return windows; } }
				
		internal static readonly bool IsInitialized = false;
		
		private IApplication aplication;
		private readonly List<Window> windows = new List<Window> ();
		private readonly List<IElementHost> roots = new List<IElementHost> ();


	
		static Application ()
		{	
			Current = new Application ();
			IsInitialized = true;
		}
		
		private Application ()
		{
			aplication = new GtkApplication ();			
			aplication.Init ();
			
			Resources = new Dictionary<object, object> ();
			
			var style = new Style ();
			style.Setters.Add (new Setter ("FontFamily", "Arial"));
			style.Setters.Add (new Setter ("FontSize", 20d));	
			style.Setters.Add (new Setter ("Foreground", Colors.Black));
			Resources [typeof(TextBlock)] = style;
			
			style = new Style ();
			style.Setters.Add (new Setter ("BorderThickness", 1.5d));
			style.Setters.Add (new Setter ("BorderColor", Colors.Black));
			style.Setters.Add (new Setter ("Template", new ControlTemplate (ButtonTemplate)));			
			Resources [typeof(Button)] = style;
			
			style = new Style ();			
			style.Setters.Add (new Setter ("Template", new ControlTemplate (ItemsControlTemplate)));			
			Resources [typeof(ItemsControl)] = style;
			
			style = new Style ();			
			style.Setters.Add (new Setter ("Template", new ControlTemplate (UserControlTemplate)));			
			Resources [typeof(UserControl)] = style;
			
			style = new Style ();			
			style.Setters.Add (new Setter ("Template", new ControlTemplate (element => WindowTemplate (element as Window))));			
			Resources [typeof(Window)] = style;
		}
		
		private static UIElement ButtonTemplate (UIElement element)
		{
			var border = new Border ();
			border.CornerRadius = new CornerRadius (3);
			
			BindingOperations.SetBinding (element.GetProperty ("Padding"), border.GetProperty ("Padding"));
			BindingOperations.SetBinding (element.GetProperty ("Background"), border.GetProperty ("Background"));
			BindingOperations.SetBinding (element.GetProperty ("BorderThickness"), border.GetProperty ("BorderThickness"));
			BindingOperations.SetBinding (element.GetProperty ("BorderColor"), border.GetProperty ("BorderColor"));
			
			BindingOperations.SetBinding (element.GetProperty ("Content"), border.GetProperty ("Child"));
			
			return border;		
		}
		
		private static UIElement ItemsControlTemplate (UIElement element)
		{
			var border = new Border ();
			
			BindingOperations.SetBinding (element.GetProperty ("Padding"), border.GetProperty ("Padding"));
			BindingOperations.SetBinding (element.GetProperty ("Background"), border.GetProperty ("Background"));
			BindingOperations.SetBinding (element.GetProperty ("BorderThickness"), border.GetProperty ("BorderThickness"));
			BindingOperations.SetBinding (element.GetProperty ("BorderColor"), border.GetProperty ("BorderColor"));
			
			BindingOperations.SetBinding (element.GetProperty ("ItemsPanel"), border.GetProperty ("Child"));
			
			return border;		
		}
		
		private static UIElement UserControlTemplate (UIElement element)
		{
			var border = new Border ();
			
			BindingOperations.SetBinding (element.GetProperty ("Padding"), border.GetProperty ("Padding"));
			BindingOperations.SetBinding (element.GetProperty ("Background"), border.GetProperty ("Background"));
			BindingOperations.SetBinding (element.GetProperty ("BorderThickness"), border.GetProperty ("BorderThickness"));
			BindingOperations.SetBinding (element.GetProperty ("BorderColor"), border.GetProperty ("BorderColor"));
			
			BindingOperations.SetBinding (element.GetProperty ("Content"), border.GetProperty ("Child"));
			
			return border;		
		}

				
		private static UIElement WindowTemplate (Window element)
		{
			var grid = new Grid () {HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch};
			grid.RowDefinitions.Add (new RowDefinition () {Height = GridLength.Auto});
			grid.RowDefinitions.Add (new RowDefinition ());			
			grid.ColumnDefinitions.Add (new ColumnDefinition ());
			
			var titleGrid = new Grid () {HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch};
			titleGrid.RowDefinitions.Add (new RowDefinition ());
			titleGrid.ColumnDefinitions.Add (new ColumnDefinition ());
			titleGrid.ColumnDefinitions.Add (new ColumnDefinition () { Width = GridLength.Auto });
			
			var title = new TextBlock ();		
			title.Foreground = Colors.White;
			title.HorizontalAlignment = HorizontalAlignment.Center;
			BindingOperations.SetBinding (element.GetProperty ("Title"), title.GetProperty ("Text"));
						
			var closeButton = CloseButton ();			
			closeButton.Click += (sender, e) => element.Close ();
			
			titleGrid.Children.Add (title);			
			titleGrid.Children.Add (closeButton);
			
			titleGrid.SetColumn (0, title);
			titleGrid.SetColumn (1, closeButton);
			
			var titleBorder = new Border ();
			titleBorder.Background = GetTitleBrush ();
			titleBorder.Child = titleGrid;
			titleBorder.Padding = new Thickness (3);
									
			grid.Children.Add (titleBorder);
			grid.SetRow (0, titleBorder);
			
			var border = new Border ();
			
			BindingOperations.SetBinding (element.GetProperty ("Padding"), border.GetProperty ("Padding"));
			BindingOperations.SetBinding (element.GetProperty ("Background"), border.GetProperty ("Background"));
			BindingOperations.SetBinding (element.GetProperty ("BorderThickness"), border.GetProperty ("BorderThickness"));
			BindingOperations.SetBinding (element.GetProperty ("BorderColor"), border.GetProperty ("BorderColor"));
			
			BindingOperations.SetBinding (element.GetProperty ("Content"), border.GetProperty ("Child"));
			
			grid.Children.Add (border);
			grid.SetRow (1, border);
			
			return grid;		
		}	
		
		private static LinearGradientBrush GetTitleBrush ()
		{
			var result = new LinearGradientBrush ();
			
			result.StartPoint = new Point (0, 0);
			result.EndPoint = new Point (0, 1);
			result.GradientStops.Add (new GradientStop (new Color (0x97, 0xb8, 0xe2), 0));
			result.GradientStops.Add (new GradientStop (new Color (0x4e, 0x76, 0xa8), 1));
			
			return result;
		}
		
		private static Button CloseButton ()
		{
			var figure1 = new PathFigure ();
			figure1.StartPoint = new Point (0, 0);
			figure1.Segments.Add (new LineSegment () { Point = new Point(7,7)});
			
			var figure2 = new PathFigure ();
			figure2.StartPoint = new Point (7, 0);
			figure2.Segments.Add (new LineSegment () { Point = new Point(0,7)});
			
			var pathGeomentry = new PathGeometry ();
			pathGeomentry.Figures.Add (figure1);
			pathGeomentry.Figures.Add (figure2);
			
			var path = new Path ();
			path.WidthRequest = 7;
			path.HeightRequest = 7;
			path.Stroke = Colors.White;
			path.StrokeThickness = 2;
			path.Data = pathGeomentry;
						
			var button = new Button () { Content = path };
			button.Padding = new Thickness (4);
			button.BorderColor = new Color (0x1f, 0x42, 0x6f); 
			button.BorderThickness = 1;
			button.Background = GetTitleBrush ();
			return button;
		}
		
		public void Run (Window window)
		{
			MainWindow = window;
			MainWindow.Closed += (sender, e) => aplication.Shutdown ();
			
			window.Show ();
			
			aplication.Run ();			
		}
		
		internal void RegisterWindow (Window window)
		{
			windows.Add (window);
			aplication.RegisterWindow (window);
		}

		internal void UnregisterWindow (Window window)
		{
			windows.Remove (window);
			aplication.UnregisterWindow (window);
		}

		public void RegisterRoot (IElementHost elementHost)
		{
			roots.Add (elementHost);
		}

		public IElementHost GetRoot (Visual visual)
		{
			return roots.FirstOrDefault (r => r.Child == visual);
		}
	}
}
