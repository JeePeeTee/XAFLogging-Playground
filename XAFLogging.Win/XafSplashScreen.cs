#region MIT License

// ==========================================================
// 
// XAFLogging project - Copyright (c) 2022 JeePeeTee
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// ===========================================================

#endregion

#region usings

using System.Reflection;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Svg;
using DevExpress.XtraSplashScreen;

#endregion

namespace XAFLogging.Win {
    public partial class XafSplashScreen : SplashScreen {
        public XafSplashScreen() {
            InitializeComponent();
            LoadBlankLogo();
            labelCopyright.Text = "Copyright © " + DateTime.Now.Year.ToString() + " Company Name" + Environment.NewLine + "All rights reserved.";
            UpdateLabelsPosition();
        }

        private void LoadBlankLogo() {
            var assembly = Assembly.GetExecutingAssembly();
            var blankLogoResourceName = assembly.GetName().Name + ".Images.Logo.svg";
            var svgStream = assembly.GetManifestResourceStream(blankLogoResourceName);
            if (svgStream != null) {
                svgStream.Position = 0;
                peLogo.SvgImage = SvgImage.FromStream(svgStream);
            }
        }

        protected override void DrawContent(GraphicsCache graphicsCache, Skin skin) {
            Rectangle bounds = ClientRectangle;
            bounds.Width--;
            bounds.Height--;
            graphicsCache.Graphics.DrawRectangle(graphicsCache.GetPen(Color.FromArgb(255, 87, 87, 87), 1), bounds);
        }

        protected void UpdateLabelsPosition() {
            labelApplicationName.CalcBestSize();
            var newLeft = (Width - labelApplicationName.Width) / 2;
            labelApplicationName.Location = new Point(newLeft, labelApplicationName.Top);
            labelSubtitle.CalcBestSize();
            newLeft = (Width - labelSubtitle.Width) / 2;
            labelSubtitle.Location = new Point(newLeft, labelSubtitle.Top);
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg) {
            base.ProcessCommand(cmd, arg);
            if ((UpdateSplashCommand)cmd == UpdateSplashCommand.Description) {
                labelStatus.Text = (string)arg;
            }
        }

        #endregion
    }
}