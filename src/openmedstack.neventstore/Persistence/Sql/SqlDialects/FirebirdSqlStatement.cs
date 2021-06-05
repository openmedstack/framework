// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="FirebirdSqlStatement.cs" company="Reimers.dk">
// //
// // </copyright>
// // <summary>
// //   Defines the $TYPE$ type.
// // </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace OpenMedStack.NEventStore.Persistence.Sql.SqlDialects
{
    using System;
    using System.Data;

    internal class FirebirdSqlStatement : CommonDbStatement
    {
        /// <inheritdoc />
        public FirebirdSqlStatement(ISqlDialect dialect, IDbConnection connection)
            : base(dialect, connection)
        {
        }

        /// <inheritdoc />
        protected override void BuildParameter(IDbCommand command, string name, object value, DbType? dbType)
        {
            switch (dbType)
            {
                case DbType.DateTimeOffset:
                    base.BuildParameter(command, name, ((DateTimeOffset)value).DateTime, DbType.DateTime);
                    break;
                default:
                    base.BuildParameter(command, name, value, dbType);
                    break;
            }
        }
    }
}
