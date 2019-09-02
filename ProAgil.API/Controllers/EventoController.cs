using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.API.Dtos;
using ProAgil.Domain;
using ProAgil.Repository;

namespace ProAgil.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;

        private readonly IMapper _mapper;
        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsync(true);

                var results = _mapper.Map<EventoDTO[]>(eventos);
                
                return Ok(results);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de Dados falhou. {ex.Message}");
            }
        }

        [HttpGet("{EventoId}")]
        public async Task<ActionResult> Get(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, true);
                
                var results = _mapper.Map<EventoDTO>(evento);

                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados falhou");
            } 
        }

        [HttpGet("getByTema/{tema}")]
        public async Task<ActionResult> GetByTema(string tema)
        {
            try
            {
                var results = await _repo.GetAllEventoAsyncByTema(tema, true);
                
                return Ok(results);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados falhou");
            } 
        }

        
        [HttpPost]
        public async Task<ActionResult> Post(EventoDTO model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);

                _repo.Add(evento);

                if(await _repo.SaveChangesAsync()){
                    return Created($"/api/evento/{model.Id}", _mapper.Map<EventoDTO>(evento));
                }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de Dados falhou. {ex.Message}");
            } 

            return BadRequest();
        }

        [HttpPut("{EventoId}")]
        public async Task<ActionResult> Put(int EventoId, EventoDTO model)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);

                if(evento == null){return NotFound();}
                
                _mapper.Map(model, evento);

                _repo.Update(evento);

                if(await _repo.SaveChangesAsync()){
                    return Created($"/api/evento/{evento.Id}", _mapper.Map<EventoDTO>(evento));
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados falhou");
            } 

            return BadRequest();
        }

        [HttpDelete("{EventoId}")]
        public async Task<ActionResult> Delete(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);

                if(evento == null){return NotFound();}

                _repo.Delete(evento);

                if(await _repo.SaveChangesAsync()){
                    return Ok();
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Banco de Dados falhou");
            } 

            return BadRequest();
        }
    }
}