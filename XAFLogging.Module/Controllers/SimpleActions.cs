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

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Serilog;

#endregion

namespace XAFLogging.Module.Controllers;

public class SimpleActions : ViewController {
    private readonly IContainer components;

    public SimpleActions() {
        components = new Container();

        InitializeMySimpleAction();
    }

    // New actions here... check templates actsmpl, act...

    protected override void OnActivated() {
        base.OnActivated();
        // Perform various tasks depending on the target View.
    }

    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        // Access and customize the target View control.
    }

    protected override void OnDeactivated() {
        // Unsubscribe from previously subscribed events and release other references and resources.
        base.OnDeactivated();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            foreach (var action in Actions) {
                action.Dispose();
            }

            components?.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Simple action: MySimpleAction

    private SimpleAction _mySimpleAction;

    private void InitializeMySimpleAction() {
        _mySimpleAction = new SimpleAction(this, nameof(_mySimpleAction), PredefinedCategory.View) {
            Caption = "MySimpleAction",
            ConfirmationMessage = null,
            ImageName = "ModelEditor_Application",
            SelectionDependencyType = SelectionDependencyType.Independent,
            ToolTip = null,
            //TargetObjectType = typeof(string),
            TargetViewType = ViewType.Any,
            TargetViewNesting = Nesting.Any,
            TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne,
        };

        _mySimpleAction.Execute += MySimpleActionExecute;

        Actions.Add(_mySimpleAction);

        void MySimpleActionExecute(object sender, SimpleActionExecuteEventArgs e) {
            Log.Information("Module action triggerd...");
        }
    }

    #endregion Simple action: MySimpleAction
}