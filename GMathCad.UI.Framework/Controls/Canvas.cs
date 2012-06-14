// 
// Canvas.cs
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
using Cairo;
using System.Linq;

namespace GMathCad.UI.Framework
{
	public class Canvas : Panel<CanvasChildContainer>
	{
		public Canvas ()
		{
		}
	
		protected override void OnRender (Cairo.Context cr)
		{
			foreach (var element in Children) {
				cr.Save ();				
				
				var matrix = new Cairo.Matrix (1, 0, 0, 1, element.X, element.Y);				
				cr.Transform (matrix);
				
				element.Render (cr);
				
				cr.Restore ();
			}
		}
		
		public double GetLeft (UIElement element)
		{
			var el = Children.Cast<CanvasChildContainer>().FirstOrDefault (c => c.Content == element);
			
			if (el == null)
				return 0;
			
			return el.X;
		}
		
		public void SetLeft (double left, UIElement element)
		{
			var el = Children.Cast<CanvasChildContainer> ().FirstOrDefault (c => c.Content == element);
			
			if (el == null)
				return;
			
			el.X = left;
		}
		
		public double GetTop (UIElement element)
		{
			var el = Children.Cast<CanvasChildContainer> ().FirstOrDefault (c => c.Content == element);
			
			if (el == null)
				return 0;
			
			return el.Y;
		}
		
		public void SetTop (double top, UIElement element)
		{
			var el = Children.Cast<CanvasChildContainer> ().FirstOrDefault (c => c.Content == element);
			
			if (el == null)
				return;
			
			el.Y = top;
		}
				
		protected override Size MeasureOverride (Size availableSize, Cairo.Context cr)
		{
			Children.ToList ().ForEach (r => r.Measure (availableSize, cr));
			
			return availableSize;
		}
		
		protected override void ArrangeOverride (Size finalSize)
		{
			foreach (var container in Children) {				
				container.Arrange (!container.DesiredSize.IsEmpty ? container.DesiredSize : finalSize);
			}
		}
		
		public override Visual HitTest (double x, double y)
		{
			var hitTest = Children.Cast<CanvasChildContainer> ().FirstOrDefault (r => r.HitTest (x, y) != null);
			if (hitTest != null) {
				return hitTest.Content;
			}
			return this;
		}	
		
		
	}
}