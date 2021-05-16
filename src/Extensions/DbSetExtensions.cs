using Microsoft.EntityFrameworkCore;

namespace CHAI.Extensions
{
    /// <summary>
    /// Entensions for <see cref="DbSet{TEntity}"/>.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Method for removing all data in a <see cref="DbSet{TEntity}"/>.
        /// </summary>
        /// <typeparam name="T">The TEntity of the DbSet.</typeparam>
        /// <param name="dbSet">The DbSet to remove data from.</param>
        public static void Clear<T>(this DbSet<T> dbSet)
            where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }
}
