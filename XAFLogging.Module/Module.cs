using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using Serilog.Context;

namespace XAFLogging.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class XAFLoggingModule : ModuleBase {
    public XAFLoggingModule() {
	    GlobalLogContext.PushProperty("Version", Assembly.GetExecutingAssembly().GetName().Version);
		// 
		// XAFLoggingModule
		// 
		AdditionalExportedTypes.Add(typeof(ModelDifference));
		AdditionalExportedTypes.Add(typeof(ModelDifferenceAspect));
        AdditionalExportedTypes.Add(typeof(BaseObject));
        AdditionalExportedTypes.Add(typeof(AuditDataItemPersistent));
        AdditionalExportedTypes.Add(typeof(AuditedObjectWeakReference));
        AdditionalExportedTypes.Add(typeof(FileData));
        AdditionalExportedTypes.Add(typeof(FileAttachmentBase));
        AdditionalExportedTypes.Add(typeof(Analysis));
        AdditionalExportedTypes.Add(typeof(Event));
        AdditionalExportedTypes.Add(typeof(Resource));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.AuditTrail.AuditTrailModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Chart.ChartModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.CloneObject.CloneObjectModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Dashboards.DashboardsModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Kpi.KpiModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Notifications.NotificationsModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Office.OfficeModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotChart.PivotChartModuleBase));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotGrid.PivotGridModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        // Manage various aspects of the application UI and behavior at the module level.
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
