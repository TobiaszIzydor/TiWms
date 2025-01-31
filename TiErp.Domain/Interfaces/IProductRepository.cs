﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiErp.Domain.Entities;

namespace TiErp.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();
        Task<IEnumerable<Product>> GetAllSimply();
        Task<Product> GetById(int id);
        Task Create(Product product);
        Task Commit();
        Task DeleteById(int id);
    }
}
