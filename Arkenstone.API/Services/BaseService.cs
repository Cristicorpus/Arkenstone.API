using Arkenstone.Logic.Entities;
using Arkenstone.Entities;
using Arkenstone.Logic.Repository;

namespace Arkenstone.API.Services
{
    public class BaseService
    {
        protected ArkenstoneContext _context;
        
        protected AllianceRepository allianceRepository;
        protected CorporationRepository corporationRepository;
        protected CharacterRepository characterRepository;

        protected EfficiencyRepository efficiencyRepository;
        
        protected ItemRepository itemRepository;
        protected InventoryRepository inventoryRepository;

        protected LocationRepository locationRepository;
        protected SubLocationRepository subLocationRepository;
            
        protected ProdAchatRepository prodAchatRepository;



        public BaseService(ArkenstoneContext context)
        {
            _context = context;
            allianceRepository = new AllianceRepository(_context);
            corporationRepository = new CorporationRepository(_context);
            characterRepository = new CharacterRepository(_context);

            efficiencyRepository = new EfficiencyRepository(_context);

            itemRepository = new ItemRepository(_context);
            inventoryRepository = new InventoryRepository(_context);

            locationRepository = new LocationRepository(_context);
            subLocationRepository = new SubLocationRepository(_context);

            prodAchatRepository = new ProdAchatRepository(_context);
        }
    }
}
