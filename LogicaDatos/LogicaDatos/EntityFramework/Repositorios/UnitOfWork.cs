using Infrastructure.Persistence.EntityFramework;
using LogicaAplicacion.ServiceInterfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.LogicaDatos.EntityFramework.Repositorios
{
    public class UnitOfWork: IUnitOfWork  
    {
        private readonly GestorSalasContext _dbContext;

        public UnitOfWork(GestorSalasContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
