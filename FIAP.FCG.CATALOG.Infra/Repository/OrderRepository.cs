using AutoMapper;
using FIAP.FCG.CATALOG.Core.Inputs;
using FIAP.FCG.CATALOG.Core.Models;
using FIAP.FCG.CATALOG.Core.Validation;
using FIAP.FCG.CATALOG.Infra.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FIAP.FCG.CATALOG.Infra.Repository
{
	public class OrderRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<Order>(context), IOrderRepository
	{
		private readonly IMapper _mapper = mapper;

        public async Task<int> Create(OrderRegisterDto orderRegister)
        {
            var entity = _mapper.Map<Order>(orderRegister);

            await Register(entity);
            return entity.Id;
        }

    }
}
