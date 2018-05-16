using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace MyPizza.UnitTests
{
    public static class TestHelper
    {
        public static Mock<DbSet<T>> MockDbSet<T>(IList<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mock = new Mock<DbSet<T>>(MockBehavior.Loose);

            mock.As<IDbAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<T>(queryable.GetEnumerator()));
            mock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestDbAsyncQueryProvider<T>(queryable.Provider));
            mock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryable.Expression);
            mock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryable.ElementType);
            mock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(queryable.GetEnumerator());
            mock.Setup(x => x.Include(It.IsAny<string>())).Returns(mock.Object);
            mock.Setup(x => x.Local).Returns(new ObservableCollection<T>(data));
            mock.Setup(x => x.Add(It.IsAny<T>())).Returns((T a) => { data.Add(a); return a; });
            mock.Setup(x => x.AddRange(It.IsAny<IEnumerable<T>>())).Returns((IEnumerable<T> a) => { foreach (var item in a.ToArray()) data.Add(item); return a; });
            mock.Setup(x => x.Remove(It.IsAny<T>())).Returns((T a) => { data.Remove(a); return a; });
            mock.Setup(x => x.RemoveRange(It.IsAny<IEnumerable<T>>())).Returns((IEnumerable<T> a) => { foreach (var item in a.ToArray()) data.Remove(item); return a; });

            return mock;
        }

    }

    internal class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        public void Dispose()
        {
            inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(inner.MoveNext());
        }

        public T Current
        {
            get { return inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }

    internal class TestDbAsyncQueryProvider<TEntity> : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            this.inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }
}