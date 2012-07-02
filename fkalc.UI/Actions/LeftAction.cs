//
// LeftAction.cs
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
using System.Linq;
using fkalc.Tokens;

namespace fkalc.UI
{
	public class LeftAction
	{
		private MathRegion Region { get; set; }

		public LeftAction (MathRegion region)
		{
			Region = region;
		}
		
		public void Execute ()
		{
			if (Region.Selection.SelectedToken is TextToken) {
				if (Region.Selection.Position > 0) {
					Region.Selection.Position--;
					return;
				}
			}
			
			var left = Region.Selection.SelectedToken.Left ();

			if (left is PlusToken || left is MinusToken || left is MultiplicationToken) {
				left = left.Left ();
			}

			if (left == null)
				return;

			Region.Selection.SelectedToken = left;

			if (left is TextToken) {
				var textToken = left as TextToken;
				Region.Selection.Position = string.IsNullOrEmpty (textToken.Text) ? 0 : textToken.Text.Length;
			}			
		}
	}
}
