using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found, null otherwise.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves sales from the database using pagination, ordering and filters.
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
    public async Task<PagedResult<Sale>> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsQueryable();

        query = ApplyFilters(
            query,
            saleNumber,
            customerName,
            branchName,
            isCancelled,
            minSaleDate,
            maxSaleDate,
            minTotalAmount,
            maxTotalAmount);

        var totalCount = await query.CountAsync(cancellationToken);
        var sales = await ApplyOrdering(query, order)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PagedResult<Sale>(sales, totalCount, page, size);
    }

    /// <summary>
    /// Updates an existing sale in the database.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Deletes a sale from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Sale> ApplyFilters(
        IQueryable<Sale> query,
        string? saleNumber,
        string? customerName,
        string? branchName,
        bool? isCancelled,
        DateTime? minSaleDate,
        DateTime? maxSaleDate,
        decimal? minTotalAmount,
        decimal? maxTotalAmount)
    {
        query = ApplyStringFilter(query, saleNumber, sale => sale.SaleNumber);
        query = ApplyStringFilter(query, customerName, sale => sale.CustomerName);
        query = ApplyStringFilter(query, branchName, sale => sale.BranchName);

        if (isCancelled.HasValue)
            query = query.Where(sale => sale.IsCancelled == isCancelled.Value);

        if (minSaleDate.HasValue)
            query = query.Where(sale => sale.SaleDate >= minSaleDate.Value);

        if (maxSaleDate.HasValue)
            query = query.Where(sale => sale.SaleDate <= maxSaleDate.Value);

        if (minTotalAmount.HasValue)
            query = query.Where(sale => sale.TotalAmount >= minTotalAmount.Value);

        if (maxTotalAmount.HasValue)
            query = query.Where(sale => sale.TotalAmount <= maxTotalAmount.Value);

        return query;
    }

    private static IQueryable<Sale> ApplyStringFilter(
        IQueryable<Sale> query,
        string? value,
        System.Linq.Expressions.Expression<Func<Sale, string>> property)
    {
        if (string.IsNullOrWhiteSpace(value))
            return query;

        var pattern = value.Replace('*', '%');
        if (value.Contains('*'))
            return query.Where(sale => EF.Functions.ILike(EF.Property<string>(sale, GetPropertyName(property)), pattern));

        return query.Where(sale => EF.Property<string>(sale, GetPropertyName(property)) == value);
    }

    private static IOrderedQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? order)
    {
        var clauses = string.IsNullOrWhiteSpace(order)
            ? ["saleDate desc"]
            : order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        IOrderedQueryable<Sale>? orderedQuery = null;

        foreach (var clause in clauses)
        {
            var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var field = parts[0];
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            orderedQuery = ApplyOrderingClause(orderedQuery ?? query, field, descending, orderedQuery != null);
        }

        return orderedQuery ?? query.OrderByDescending(sale => sale.SaleDate);
    }

    private static IOrderedQueryable<Sale> ApplyOrderingClause(
        IQueryable<Sale> query,
        string field,
        bool descending,
        bool thenBy)
    {
        return field.ToLowerInvariant() switch
        {
            "id" => OrderBy(query, sale => sale.Id, descending, thenBy),
            "salenumber" => OrderBy(query, sale => sale.SaleNumber, descending, thenBy),
            "saledate" => OrderBy(query, sale => sale.SaleDate, descending, thenBy),
            "customername" => OrderBy(query, sale => sale.CustomerName, descending, thenBy),
            "branchname" => OrderBy(query, sale => sale.BranchName, descending, thenBy),
            "totalamount" => OrderBy(query, sale => sale.TotalAmount, descending, thenBy),
            "iscancelled" => OrderBy(query, sale => sale.IsCancelled, descending, thenBy),
            _ => OrderBy(query, sale => sale.SaleDate, true, thenBy)
        };
    }

    private static IOrderedQueryable<Sale> OrderBy<TKey>(
        IQueryable<Sale> query,
        System.Linq.Expressions.Expression<Func<Sale, TKey>> keySelector,
        bool descending,
        bool thenBy)
    {
        if (thenBy && query is IOrderedQueryable<Sale> orderedQuery)
            return descending
                ? orderedQuery.ThenByDescending(keySelector)
                : orderedQuery.ThenBy(keySelector);

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }

    private static string GetPropertyName(System.Linq.Expressions.Expression<Func<Sale, string>> property)
    {
        if (property.Body is System.Linq.Expressions.MemberExpression memberExpression)
            return memberExpression.Member.Name;

        throw new ArgumentException("Expression must be a property access.", nameof(property));
    }
}
