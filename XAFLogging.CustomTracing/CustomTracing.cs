using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Serilog;

namespace XAFLogging.CustomTracing;

public class CustomTracing : Tracing {
    public override void LogError(Exception exception) {
        // Implement custom logging for exceptions here.
        switch (exception) {
            case ValidationException validationException:
                //ToDo Logs too much data...
                //Removed 
                //Log.Error(validationException, "Validation exception: {Details}", validationException.Result.Results);
                Log.Error(validationException, "Validation exception");    
                break;
            default:
                Log.Error(exception, "System error");
                break;
        }
    }

    public override void LogWarning(string text, params object[] args) {
        //base.LogWarning(text, args);
        Log.Warning("Warning {Text}, {Args}", text, args);
    }

    public override void LogText(string text, params object[] args) {
        //base.LogText(text, args);
        Log.Debug("Text {Text}, {Args}", text, args);
    }

    public override void LogSetOfStrings(params string[] args) {
        //base.LogSetOfStrings(args);
    }
}
