using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRepository
    {
        private readonly ProAgilContext _context;
        public ProAgilRepository(ProAgilContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public void Add<T>(T Entity) where T : class
        {
            _context.Add(Entity);
        }
        public void Update<T>(T Entity) where T : class
        {
            _context.Update(Entity);        
        }
        
        public void Delete<T>(T Entity) where T : class
        {
            _context.Remove(Entity);   
        }

        public async Task<bool> SaveChangesAsync()
        {
           return (await _context.SaveChangesAsync()) > 0;
        }

        //EVENTO
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                    .Include(c => c.Lotes)
                    .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().OrderBy(c => c.Id);

            return await query.ToArrayAsync();
        }
        public async Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
                    .Include(c => c.Lotes)
                    .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking().Where(c => c.Id == EventoId);
            
            return await query.FirstOrDefaultAsync();          
        }

        public async Task<Evento[]> GetAllEventoAsyncByTema(string tema, bool includePalestrantes)
        {
            IQueryable<Evento> query = _context.Eventos
                    .Include(c => c.Lotes)
                    .Include(c => c.RedesSociais);

            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.AsNoTracking()
                    .OrderByDescending(c => c.DataEvento)
                    .Where(c => c.Tema.ToLower().Contains(tema.ToLower()));
            
            return await query.ToArrayAsync();        
        }

        //PALESTRANTE
        public async Task<Palestrante> GetPalestranteAsync(int PalestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                    .Include(c => c.RedesSociais);

            if(includeEventos)
            {
                query = query
                    .Include(ev => ev.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.AsNoTracking().OrderBy(c => c.Nome).Where(c => c.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();        
        }

        public async Task<Palestrante[]> GetAllPalestranteAsyncByName(string name, bool includeEventos = false) 
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                    .Include(c => c.RedesSociais);

            if(includeEventos)
            {
                query = query
                    .Include(ev => ev.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.AsNoTracking().Where(c => c.Nome.ToLower().Contains(name.ToLower()));

            return await query.ToArrayAsync();       
        }
    }
}