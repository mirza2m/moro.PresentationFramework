// 
// Window.cs
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

namespace fkalc.UI.Framework
{
	public class Window : ContentControl
	{
		public event EventHandler Closed;
	
		private readonly DependencyProperty<string> title;
		
		public string Title { 
			get { return title.Value;} 
			set { title.Value = value; }
		}
		
		private ISurface Surface { get; set;}
		
		public Window ()
		{
			if (!Application.IsInitialized)
				throw new ApplicationException ("Application must be initialized");
			
			title = BuildProperty<string> ("Title");
						
			Surface = new GtkSurface (this);
			
			StyleHelper.ApplyStyle (this, typeof(Window));
		}
		
		public void Show ()
		{
			var width = WidthRequest ?? 100;
			var height = HeightRequest ?? 50;
			
			Measure (new Size (width, height));
			Arrange (new Rect (1, 1, width, height));
			
			Surface.Show ();
		}
		
		protected override void ArrangeOverride (Size finalSize)
		{
			base.ArrangeOverride (finalSize);
			
			Surface.Resize (finalSize);
		}
		
		public void Close ()
		{
			Surface.Close ();
			RaiseClosed ();
		}

		private void RaiseClosed ()
		{
			if (Closed != null)
				Closed (this, EventArgs.Empty);
					
		}
	}
}

