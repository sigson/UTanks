using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace UTanksServer.Database {
  /// <remarks>
  ///   Do not use <see cref="EntityFrameworkQueryableExtensions.Include{TEntity,TProperty}"/> and
  ///   <see cref="EntityFrameworkQueryableExtensions.Include{TEntity}(IQueryable{TEntity}, string)"/> because of massive overhead,
  ///	  which could result in database server out of memory error
  /// </remarks>
  public static class DbSetExtensions {
    // public static IQueryable<TEntity> IncludePlayer<TEntity>(this IQueryable<TEntity> queryable) where TEntity : PlayerData {
    // 	return queryable
    // 		.IncludeOptimized((player) => player...);
    // }
  }
}
