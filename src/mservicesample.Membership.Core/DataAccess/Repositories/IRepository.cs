﻿using mservicesample.Membership.Core.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mservicesample.Membership.Core.DataAccess.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);
        Task<List<T>> ListAll();
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
