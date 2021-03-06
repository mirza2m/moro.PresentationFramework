// 
// MouseDevice.cs
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

namespace moro.Framework
{
	public class MouseDevice
	{
		public RoutedEvent<MouseButtonEventArgs> PreviewButtonPressEvent { get; private set; }
		public RoutedEvent<MouseButtonEventArgs> ButtonPressEvent { get; private set; }

		public RoutedEvent<MouseButtonEventArgs> PreviewButtonReleaseEvent { get; private set; }
		public RoutedEvent<MouseButtonEventArgs> ButtonReleaseEvent { get; private set; }
		
		public RoutedEvent<MouseButtonEventArgs> PreviewMotionNotifyEvent { get; private set; }
		public RoutedEvent<MouseButtonEventArgs> MotionNotifyEvent { get; private set; }
		
		public RoutedEvent<EventArgs> MouseEnterEvent { get; private set; }
		public RoutedEvent<EventArgs> MouseLeaveEvent { get; private set; }
		
		private Visual targetElement;
		
		private List<IMouseInputProvider> providers = new List<IMouseInputProvider> ();

		public MouseDevice ()
		{
			PreviewButtonPressEvent = new TunnelingEvent<MouseButtonEventArgs> ();
			ButtonPressEvent = new BubblingEvent<MouseButtonEventArgs> (); 

			PreviewButtonReleaseEvent = new TunnelingEvent<MouseButtonEventArgs> ();
			ButtonReleaseEvent = new BubblingEvent<MouseButtonEventArgs> (); 

			PreviewMotionNotifyEvent = new TunnelingEvent<MouseButtonEventArgs> ();
			MotionNotifyEvent = new BubblingEvent<MouseButtonEventArgs> ();

			MouseEnterEvent = new DirectEvent<EventArgs> ();
			MouseLeaveEvent = new DirectEvent<EventArgs> ();
		}
		
		public Visual TargetElement { 
			get { return targetElement; }
			private set {
				if (targetElement == value)
					return;

				var oldTree = VisualTreeHelper.GetVisualBranch (targetElement);
				var newTree = VisualTreeHelper.GetVisualBranch (value);

				foreach (var element in oldTree.Except(newTree))
					RaiseMouseLeaveEvent (element);
				
				targetElement = value;

				foreach (var element in newTree.Except(oldTree))
					RaiseMouseEnterEvent (element);				
			}			
		}		

		public Visual Captured { get; set; }
		private Visual CapturedTargetElement { get { return Captured ?? TargetElement; } }
				
		public void RegistedMouseInputProvider (IMouseInputProvider provider)
		{
			provider.ButtonPressEvent += HandleProviderButtonPressEvent;
			provider.ButtonReleaseEvent += HandleButtonReleaseEvent;
			provider.MotionNotifyEvent += HandleMotionNotifyEvent;

			providers.Add (provider);
		}

		public void UnregisterMouseInputProvider (IMouseInputProvider provider)
		{
			provider.ButtonPressEvent -= HandleProviderButtonPressEvent;
			provider.ButtonReleaseEvent -= HandleButtonReleaseEvent;
			provider.MotionNotifyEvent -= HandleMotionNotifyEvent;
			providers.Remove (provider);			
		}

		public Point GetPosition (Visual visual)
		{
			var rootElement = VisualTreeHelper.GetVisualBranch (visual).Last ();

			var provider = providers.FirstOrDefault (p => p.RootElement == rootElement);
			
			if (provider == null)
				return new Point ();

			var root = Application.Current.GetRoot (provider.RootElement);
			var rootPosition = root.GetPosition ();
			var point = root.GerMousePosition ();

			return visual.PointFromScreen (new Point (rootPosition.X + point.X, rootPosition.Y + point.Y));
		}

		private void HandleProviderButtonPressEvent (object o, MouseButtonEventArgs args)
		{
			RaisePreviewButtonPressEvent (args);
			RaiseButtonPressEvent (args);
		}

		private void HandleButtonReleaseEvent (object sender, MouseButtonEventArgs e)
		{
			RaisePreviewButtonReleaseEvent (e);
			RaiseButtonReleaseEvent (e);
		}		
		
		private void HandleMotionNotifyEvent (object o, MouseButtonEventArgs args)
		{
			var provider = providers.FirstOrDefault (p => p == o);
			
			if (provider == null)
				return;

			var root = Application.Current.GetRoot (provider.RootElement);

			if (root == null)
				return;

			var point = root.GerMousePosition ();
			
			TargetElement = VisualTreeHelper.HitTest (point, provider.RootElement);

			var eventArgs = new MouseButtonEventArgs ();

			RaisePreviewMotionNotifyEvent (eventArgs);
			RaiseMotionNotifyEvent (eventArgs);
		}
		
		private void RaisePreviewButtonPressEvent (MouseButtonEventArgs args)
		{
			if (CapturedTargetElement == null)
				return;

			PreviewButtonPressEvent.RaiseEvent (CapturedTargetElement, args);
		}	
		
		private void RaiseButtonPressEvent (MouseButtonEventArgs args)
		{
			if (TargetElement == null)
				return;

			ButtonPressEvent.RaiseEvent (TargetElement, args);
		}

		private void RaisePreviewButtonReleaseEvent (MouseButtonEventArgs args)
		{
			if (CapturedTargetElement == null)
				return;
			
			PreviewButtonReleaseEvent.RaiseEvent (CapturedTargetElement, args);
		}	
		
		private void RaiseButtonReleaseEvent (MouseButtonEventArgs args)
		{
			if (CapturedTargetElement == null)
				return;
			
			ButtonReleaseEvent.RaiseEvent (CapturedTargetElement, args);
		}
		
		private void RaisePreviewMotionNotifyEvent (MouseButtonEventArgs args)
		{
			if (CapturedTargetElement == null)
				return;

			PreviewMotionNotifyEvent.RaiseEvent (CapturedTargetElement, args);
		}	
		
		private void RaiseMotionNotifyEvent (MouseButtonEventArgs args)
		{
			if (CapturedTargetElement == null)
				return;

			MotionNotifyEvent.RaiseEvent (CapturedTargetElement, args);
		}
		
		private void RaiseMouseEnterEvent (Visual visual)
		{
			if (visual == null)
				return;

			MouseEnterEvent.RaiseEvent (visual, EventArgs.Empty);
		}
		
		private void RaiseMouseLeaveEvent (Visual visual)
		{
			if (visual == null)
				return;

			MouseLeaveEvent.RaiseEvent (visual, EventArgs.Empty);
		}
	}
}

