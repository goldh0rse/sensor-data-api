using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace RestApi.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByPropertyName<T>(
            this IQueryable<T> source,
            string propertyName,
            bool ascending = true)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            // Create an expression for the property
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression property = Expression.PropertyOrField(parameter, propertyName);
            LambdaExpression sort = Expression.Lambda(property, parameter);

            // Generate the appropriate query method (OrderBy or OrderByDescending)
            MethodCallExpression call = Expression.Call(
                typeof(Queryable),
                ascending ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(sort)
            );

            // Run the query and return the results
            return source.Provider.CreateQuery<T>(call);
        }

        public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new InMemoryAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new InMemoryAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new InMemoryAsyncEnumerable<TResult>(expression);
            }

            public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                var result = _inner.Execute<TResult>(expression);
                return new ValueTask<TResult>(result);
            }

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class InMemoryAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public InMemoryAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
            { }

            public InMemoryAsyncEnumerable(Expression expression) : base(expression)
            { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new InMemoryAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }

        public class InMemoryAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;

            public InMemoryAsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            }

            public T Current => _enumerator.Current;

            public ValueTask DisposeAsync()
            {
                _enumerator.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_enumerator.MoveNext());
            }
        }
    }

}