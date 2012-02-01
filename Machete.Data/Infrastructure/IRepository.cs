﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machete.Data.Infrastructure
{
    // where T -- [ constraint ] on constructed type
    // : class -- [ reference type constaint ] 
    //                  specifies that type argument must be a reference type.
    //                  class types, interface types, delegate types, array types, 
    public interface IRepository<T> where T : class
    {
        T Add(T entity);
        void Delete(T entity);
        void Delete(Func<T, Boolean> predicate);
        T GetById(long Id);
        T Get(Func<T, Boolean> where);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetMany(Func<T, bool> where);
        IQueryable<T> GetAllQ();
        IQueryable<T> GetManyQ(Func<T, bool> where);
        IQueryable<T> GetManyQ();
        T GetQ(Func<T, bool> where);
    }
}
