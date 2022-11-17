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
using Serilog;

#endregion

namespace XAFLogging.Win.Controllers;

public class MySimpleActions : ViewController {
    private readonly IContainer components = null;

    public MySimpleActions() {
        components = new Container();

        InitializeMyAction();
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

    #region Simple action: MyAction

    private SimpleAction _myAction = null;

    private void InitializeMyAction() {
        _myAction = new SimpleAction(this, nameof(_myAction), PredefinedCategory.View) {
            Caption = "MyAction",
            ConfirmationMessage = null,
            ImageName = "ModelEditor_Application",
            SelectionDependencyType = SelectionDependencyType.Independent,
            ToolTip = null,
            //TargetObjectType = typeof(string),
            TargetViewType = ViewType.Any,
            TargetViewNesting = Nesting.Any,
            TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne,
        };

        _myAction.Execute += MyAction_Execute;

        Actions.Add(_myAction);
    }

    private void MyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
        Log.Information("This action was fired...");

        try {
            throw new UserFriendlyException("Exception raised in code and logged...");
        }
        catch (Exception exception) {
            Log.Error(exception, "Exception");
        }
    }

    #endregion Simple action: MyAction
}