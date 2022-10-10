#region Copyright (c) 2000-2022 Sultan CRM BV
// ==========================================================
// 
// XAFSira project - Copyright (c) 2000-2022 Sultan CRM BV
// ALL RIGHTS RESERVED
// 
// The entire contents of this file is protected by Dutch and
// International Copyright Laws. Unauthorized reproduction,
// reverse-engineering, and distribution of all or any portion of
// the code contained in this file is strictly prohibited and may
// result in severe civil and criminal penalties and will be
// prosecuted to the maximum extent possible under the law.
// 
// RESTRICTIONS
// 
// THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES
// ARE CONFIDENTIAL AND PROPRIETARY TRADE
// SECRETS OF SULTAN CRM BV. THE REGISTERED DEVELOPER IS
// NOT LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING
// CODE AS PART OF AN EXECUTABLE PROGRAM.
// 
// THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED
// FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE
// COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE
// AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT
// AND PERMISSION FROM SULTAN CRM BV.
// 
// CONSULT THE LICENSE AGREEMENT FOR INFORMATION ON
// ADDITIONAL RESTRICTIONS
// 
// ===========================================================
#endregion

using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using Serilog;

namespace XAFLogging.Win.Controllers;

public class MySimpleActions : ViewController {
    private readonly IContainer components = null;

    public MySimpleActions() {
        components = new Container();
        
        InitializeMyAction();
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
            Log.Error(exception,"Exception");
        }
    }

    #endregion Simple action: MyAction

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
}