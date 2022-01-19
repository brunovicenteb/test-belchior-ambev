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

        //Mapeia as requisições GET para http://{host}:{porta}/api/ConsultarCervejas/id
        [HttpGet("ConsultarCervejas/{page}")]
        public async Task<IActionResult> ConsultarCervejas(int page)
        {
            int jump = page * _PageSize;
            var beer = await _Context.Cervejas
                .OrderBy(o => o.Nome)
                .Skip(jump)
                .Take(_PageSize)
                .AsNoTracking().ToListAsync();
            return Ok(beer);
        }

        //Mapeia as requisições GET para http://{host}:{porta}/api/ConsultarCervejaPeloIdentificador/id
        [HttpGet("ConsultarCervejaPeloIdentificador/{id}")]
        public async Task<IActionResult> ConsultarCervejaPeloIdentificador(int id)
        {
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