using AmbevWeb.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AmbevWeb.Controllers
{

    [Route("api/[controller]")]
    public class BeerController : Controller
    {
        private static readonly int _PageSize = 2;
        private readonly AmbevContext _Context;

        public BeerController(AmbevContext pContext)
        {
            _Context = pContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        //Mapeia as requisições GET para http://{host}:{porta}/api/ConsultarCervejas/id?=x&name=y
        [HttpGet("ConsultarCervejas/{page?}/{name?}")]
        public async Task<IActionResult> ConsultarCervejas(int? page = 0, string name = null)
        {
            bool noFilter = string.IsNullOrEmpty(name);
            name = noFilter ? name : name.ToLower();
            int jump = (page ?? 0) * _PageSize;
            var beer = await _Context.Cervejas
                .Where(o => noFilter || o.Nome.ToLower().Contains(name))
                .OrderBy(o => o.Nome)
                .Skip(jump)
                .Take(_PageSize)
                .AsNoTracking().ToListAsync();
            return Ok(beer);
        }

        //Mapeia as requisições GET para http://{host}:{porta}/api/ConsultarCervejaPeloIdentificador/id
        [HttpGet("ConsultarCervejaPeloIdentificador/{id?}")]
        public async Task<IActionResult> ConsultarCervejaPeloIdentificador(int? id = null)
        {
            if (!id.HasValue)
                return NotFound("Nenhum identificador de cerveja foi informado.");
            var beer = await _Context.Cervejas.FindAsync(id);
            if (beer == null)
                return NotFound("Nenhuma cerveja foi encontrada pelo identificador " + id + ".");
            return Ok(beer);
        }

        //Mapeia as requisições GET para http://{host}:{porta}/api/ConsultarVendaPeloIdentificador/id
        [HttpGet("ConsultarVendaPeloIdentificador/{id}")]
        public async Task<IActionResult> ConsultarVendaPeloIdentificador(int id)
        {
            var venda = await _Context.Vendas
                .Include(p => p.Cliente)
                .Include(p => p.ItensVenda)
                .ThenInclude(i => i.Cerveja)
                .SingleOrDefaultAsync(p => p.IdVenda == id);
            if (venda == null)
                return NotFound("Nenhuma venda foi encontrada pelo identificador " + id + ".");
            return Ok(venda);
        }

        // //Mapeia as requisições POST para http://localhost:{porta}/api/person/
        // //O [FromBody] consome o Objeto JSON enviado no corpo da requisição
        // [HttpPost]
        // public IActionResult Post([FromBody] Person person)
        // {
        //     if (person == null) return BadRequest();
        //     return new ObjectResult(_personService.Create(person));
        // }

        // //Mapeia as requisições PUT para http://localhost:{porta}/api/person/
        // //O [FromBody] consome o Objeto JSON enviado no corpo da requisição
        // [HttpPut]
        // public IActionResult Put([FromBody] Person person)
        // {
        //     if (person == null) return BadRequest();
        //     return new ObjectResult(_personService.Update(person));
        // }

        // //Mapeia as requisições DELETE para http://localhost:{porta}/api/person/{id}
        // //recebendo um ID como no Path da requisição
        // [HttpDelete("{id}")]
        // public IActionResult Delete(string id)
        // {
        //     _personService.Delete(id);
        //     return NoContent();
        // }
    }
}