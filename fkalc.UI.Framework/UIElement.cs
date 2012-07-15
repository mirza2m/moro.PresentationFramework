// 
// UIElement.cs
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
using Gtk;

namespace fkalc.UI.Framework
{
	public class UIElement : Visual
	{		
		public event ButtonPressEventHandler ButtonPressEvent;
		public event MotionNotifyEventHandler PreviewMotionNotifyEvent;
		public event MotionNotifyEventHandler MotionNotifyEvent;
		public event EventHandler MouseEnterEvent;
		public event EventHandler MouseLeaveEvent;
		public event KeyPressEventHandler PreviewKeyPressEvent;
		public event KeyPressEventHandler KeyPressEvent;

		private readonly DependencyProperty<Visibility> visibility;
		private readonly DependencyProperty<bool> focusable;
		
		public Size DesiredSize { get; set; }

		public bool IsFocused { get; set; }

		public Visibility Visibility { 
			get { return visibility.Value;} 
			set { visibility.Value = value; }
		}

		public bool IsVisible { get { return Visibility == Visibility.Visible; } }
		
		public bool SnapsToDevicePixels { get; set; }

		public bool Focusable { 
			get { return focusable.Value;} 
			set { focusable.Value = value; }
		}

		private bool lookingFocus;
		
		public UIElement ()
		{
			visibility = BuildProperty<Visibility> ("Visibility");
			focusable = BuildProperty<bool> ("Focusable");

			Mouse.PreviewButtonPressEvent += HandlePreviewButtonPressEvent;
			Mouse.ButtonPressEvent += HandleButtonPressEvent;
			
			Mouse.PreviewMotionNotifyEvent += OnPreviewMotionNotifyEvent;
			Mouse.MotionNotifyEvent += OnMotionNotifyEvent;
			
			Mouse.MouseEnterEvent += OnMouseEnterEvent;
			Mouse.MouseLeaveEvent += OnMouseLeaveEvent;
			
			Keyboard.PreviewKeyPressEvent += OnPreviewKeyPressEvent;
			Keyboard.KeyPressEvent += OnKeyPressEvent;

			Keyboard.GotKeyboardFocusEvent += OnGotKeyboardFocusEvent;
			Keyboard.LostKeyboardFocusEvent += OnLostKeyboardFocusEvent;
		}

				
		public void Measure (Size availableSize)
		{	
			DesiredSize = MeasureCore (availableSize);
		}
		
		public void Arrange (Rect finalRect)
		{
			ArrangeCore (finalRect);
		}

		public void Render (DrawingContext dc)
		{
			OnRender (dc);

			for (int i = 0; i < VisualChildrenCount; i++) {
				var child = GetVisualChild (i);
				if (child is UIElement) {
					var uielement = child as UIElement;

					if (uielement.IsVisible) {
						dc.PushTransform (uielement.VisualTransform);

						uielement.Render (dc);

						dc.Pop ();
					}
				}
			}
		}
				
		protected virtual Size MeasureCore (Size availableSize)
		{		
			return new Size (0, 0);
		}
		
		protected virtual void ArrangeCore (Rect finalRect)
		{			
			VisualTransform = new TranslateTransform (finalRect.X, finalRect.Y);
		}

		protected virtual void OnRender (DrawingContext dc)
		{			
		}
				
		private void HandlePreviewButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			lookingFocus = true;
			//if (IsFocused) return;
			
			if (!Focusable || Mouse.Device.TargetElement != this)
				return;

			Keyboard.Focus (this);
		}

		private void HandleButtonPressEvent (object sender, ButtonPressEventArgs e)
		{
			if (lookingFocus && Focusable) {
				Keyboard.Focus (this);
			}

			lookingFocus = false;

			OnButtonPressEvent (sender, e);	
		}
				
		protected virtual void OnButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			RaiseButtonPressEvent (args);
		}
		
		protected virtual void OnPreviewKeyPressEvent (object o, KeyPressEventArgs args)
		{
			RaisePreviewKeyPressEvent (args);
		}
		
		protected virtual void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			RaiseKeyPressEvent (args);
		}
				
		protected virtual void OnPreviewMotionNotifyEvent (object o, MotionNotifyEventArgs args)
		{
			RaisePreviewMotionNotifyEvent (args);
		}
		
		protected virtual void OnMotionNotifyEvent (object o, MotionNotifyEventArgs args)
		{
			RaiseMotionNotifyEvent (args);
		}
				
		protected virtual void OnMouseEnterEvent (object sender, EventArgs args)
		{
			RaiseMouseEnterEvent (args);
		}
				
		protected virtual void OnMouseLeaveEvent (object sender, EventArgs args)
		{
			RaiseMouseLeaveEvent (args);
		}

		private void OnGotKeyboardFocusEvent (object sender, EventArgs e)
		{
			lookingFocus = false;

			if (Keyboard.FocusedElement == this)
				IsFocused = true;
		}

		private void OnLostKeyboardFocusEvent (object sender, EventArgs e)
		{
			if (Keyboard.FocusedElement == this)
				IsFocused = false;
		}
		
		private void RaiseButtonPressEvent (ButtonPressEventArgs args)
		{
			if (ButtonPressEvent != null) {
				ButtonPressEvent (this, args);
			}
		}
		
		private void RaisePreviewMotionNotifyEvent (MotionNotifyEventArgs args)
		{
			if (PreviewMotionNotifyEvent != null) {
				PreviewMotionNotifyEvent (this, args);
			}
		}
		
		private void RaiseMotionNotifyEvent (MotionNotifyEventArgs args)
		{
			if (MotionNotifyEvent != null) {
				MotionNotifyEvent (this, args);
			}
		}
		
		private void RaiseMouseEnterEvent (EventArgs args)
		{
			if (MouseEnterEvent != null) {
				MouseEnterEvent (this, args);
			}
		}
		
		private void RaiseMouseLeaveEvent (EventArgs args)
		{
			if (MouseLeaveEvent != null) {
				MouseLeaveEvent (this, args);
			}
		}
		
		private void RaisePreviewKeyPressEvent (KeyPressEventArgs args)
		{
			if (PreviewKeyPressEvent != null) {
				PreviewKeyPressEvent (this, args);
			}
		}
				
		private void RaiseKeyPressEvent (KeyPressEventArgs args)
		{
			if (KeyPressEvent != null) {
				KeyPressEvent (this, args);
			}
		}
	}
}

