namespace OpenMedStack.Domain
{
    public delegate bool ConflictDelegate<in TUncommitted, in TCommitted>(
        TUncommitted uncommitted,
        TCommitted committed)
        where TUncommitted : class where TCommitted : class;
}