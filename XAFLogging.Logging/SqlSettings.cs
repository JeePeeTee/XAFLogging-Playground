#region Copyright (c) 2000-2022 Sultan CRM BV
// ==========================================================
// 
// XAFLogging project - Copyright (c) 2000-2022 Sultan CRM BV
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

using System.Collections.ObjectModel;
using System.Data;
using Serilog.Sinks.MSSqlServer;

namespace XAFLogging.Logging; 

public class SqlSettings {
    public static ColumnOptions ColumnOptions {
        // Add extra columns to Event logging table
        get {
            var columnOptions = new ColumnOptions {
                AdditionalColumns = new Collection<SqlColumn> {
                    new SqlColumn
                        { ColumnName = "User", PropertyName = "User", DataType = SqlDbType.NVarChar, DataLength = 64 },
                    new SqlColumn
                        { ColumnName = "MachineName", PropertyName = "MachineName", DataType = SqlDbType.NVarChar, DataLength = 64 },
                    new SqlColumn
                        { ColumnName = "ProcessId", PropertyName = "ProcessId", DataType = SqlDbType.Int }
                }
            };

            // Remove XML logging
            columnOptions.Store.Remove(StandardColumn.Properties);
            // Add Json logging
            columnOptions.Store.Add(StandardColumn.LogEvent);
            return columnOptions;
        }
    }

}