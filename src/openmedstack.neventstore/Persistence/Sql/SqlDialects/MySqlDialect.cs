namespace OpenMedStack.NEventStore.Persistence.Sql.SqlDialects
{
    using System;

    public class MySqlDialect : CommonSqlDialect
    {
        private const int UniqueKeyViolation = 1062;

        public override string InitializeStorage => MySqlStatements.InitializeStorage;

        public override string PersistCommit => MySqlStatements.PersistCommit;

        public override string AppendSnapshotToCommit => base.AppendSnapshotToCommit.Replace("/*FROM DUAL*/", "FROM DUAL");

        public override object CoalesceParameterValue(object value)
        {
            if (value is Guid guid)
            {
                return guid.ToByteArray();
            }

            if (value is DateTime time)
            {
                return time.Ticks;
            }

            return value;
        }

        public override bool IsDuplicate(Exception exception)
        {
            var property = exception.GetType().GetProperty("Number")!;
            return UniqueKeyViolation == (int) property.GetValue(exception, null)!;
        }
    }
}