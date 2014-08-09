using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Data
{
    public class SuppliersRepository : GenericRepository<Supplier>
    {
        public SuppliersRepository(DbContext context) : base(context) { }
    }
}
