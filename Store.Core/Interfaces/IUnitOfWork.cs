namespace Store.Core.Interfaces;

/// <summary>Runs a set of persistence operations in one transaction when the database supports it.</summary>
public interface IUnitOfWork
{
    /// <summary>Executes <paramref name="action"/> and commits, or rolls back when it throws.</summary>
    /// <param name="action">Work that uses the current database context.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);
}
