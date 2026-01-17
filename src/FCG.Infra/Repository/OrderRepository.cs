using AutoMapper;
using FCG.Core.Core.Inputs;
using FCG.Core.Core.Models;
using FCG.Infra.Context;

namespace FCG.Infra.Repository
{
    public class OrderRepository(ApplicationDbContext context, IMapper mapper) : EFRepository<Order>(context), IOrderRepository
    {
        private readonly IMapper _mapper = mapper;

        public async Task<Guid> Create(OrderRegisterDto orderRegister)
        {
            var entity = _mapper.Map<Order>(orderRegister);

            await Register(entity);
            return entity.Id;
        }

        public async Task<OrderResponseDto?> GetById(Guid id)
        {
            var order = await Get(id);
            return order is null ? null : _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<bool> Update(Guid id, OrderUpdateDto orderUpdateDto)
        {
            var order = await Get(id) ?? throw new ArgumentNullException(nameof(id), $"Erro ao atualizar: Jogo inexistente!");
            _mapper.Map(orderUpdateDto, order);
            return await Edit(order);
        }

    }
}
