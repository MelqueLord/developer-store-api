using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Pagination;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found, null otherwise.</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves sales from the repository using pagination, ordering and filters.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="size">Page size.</param>
    /// <param name="order">Ordering expression.</param>
    /// <param name="saleNumber">Sale number filter.</param>
    /// <param name="customerName">Customer name filter.</param>
    /// <param name="branchName">Branch name filter.</param>
    /// <param name="isCancelled">Cancellation status filter.</param>
    /// <param name="minSaleDate">Minimum sale date filter.</param>
    /// <param name="maxSaleDate">Maximum sale date filter.</param>
    /// <param name="minTotalAmount">Minimum total amount filter.</param>
    /// <param name="maxTotalAmount">Maximum total amount filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of sales.</returns>
    Task<PagedResult<Sale>> GetPagedAsync(
        int page,
        int size,
        string? order = null,
        string? saleNumber = null,
        string? customerName = null,
        string? branchName = null,
        bool? isCancelled = null,
        DateTime? minSaleDate = null,
        DateTime? maxSaleDate = null,
        decimal? minTotalAmount = null,
        decimal? maxTotalAmount = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
