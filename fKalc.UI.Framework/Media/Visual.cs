// 
// Visual.cs
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
using System.Linq;

namespace fKalc.UI.Framework
{
	public class Visual : DependencyObject
	{
		public Visual VisaulParent { get; private set; }
		public Transform VisualTransform { get; protected set; }
		
		public Visual ()
		{
			VisualTransform = new TranslateTransform (0, 0);
		}
		
		public virtual Visual HitTest (double x, double y)
		{			
			return null;
		}
		
		protected void AddVisualChild (Visual visual)
		{
			visual.VisaulParent = this;
		}
		
		protected void RemoveVisualChild (Visual visual)
		{
			visual.VisaulParent = null;
		}

		public Point PointToScreen (Point point)
		{
			var result = point;
			foreach (var visual in GetVisualBranch(this)) {
				result = visual.VisualTransform.TransformPoint (result);
			}

			return result;
		}

		public Point PointFromScreen (Point point)
		{
			var result = point;
			foreach (var visual in GetVisualBranch(this).Reverse()) {
				result = visual.VisualTransform.Inverse.TransformPoint (result);
			}

			return result;
		}

		private IEnumerable<Visual> GetVisualBranch (Visual visual)
		{
			yield return visual;
			
			if (visual.VisaulParent != null) {
				foreach (var parent in GetVisualBranch(visual.VisaulParent)) {
					yield return parent;
				}
			}
		}
	}
}

