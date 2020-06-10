using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected BookLibraryContext BookLibraryContext { get; set; }
        protected IEmailService EmailService;

        public RepositoryBase(BookLibraryContext repositoryContext, IEmailService emailService)
        {
            this.BookLibraryContext = repositoryContext;
            this.EmailService = emailService;
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await BookLibraryContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAync(Expression<Func<T, bool>> expression)
        {
            return await BookLibraryContext.Set<T>().Where(expression).ToListAsync();
        }
        public void Create(T entity)
        {
            this.BookLibraryContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            this.BookLibraryContext.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await BookLibraryContext.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            this.BookLibraryContext.Set<T>().Update(entity);
        }
    }
}
