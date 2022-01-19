using System;
using System.Linq;
using AmbevWeb.Models;
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

        //http://{host}:{porta}/api/ConsultarCervejas?pagina=xx&nome=yy
        [HttpGet("ConsultarCervejas/{pagina?}/{nome?}")]
        public async Task<IActionResult> ConsultarCervejas(int? pagina = 0, string nome = null)
        {
            // Paginando [OK]
            // Filtrando por nome [OK]
            // Ordenar por nome de forma crescente [OK]
            bool noFilter = string.IsNullOrEmpty(nome);
            nome = noFilter ? nome : nome.ToLower();
            int jump = (pagina ?? 0) * _PageSize;
            var beer = await _Context.Cervejas
                .Where(o => noFilter || o.Nome.ToLower().Contains(nome))
                .OrderBy(o => o.Nome)
                .Skip(jump)
                .Take(_PageSize)
                .AsNoTracking().ToListAsync();
            return Ok(beer);
        }

        //http://{host}:{porta}/api/ConsultarCervejaPeloIdentificador?id=xx
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

        //http://{host}:{porta}/api/ConsultarVendaPeloIdentificador?id=xx
        [HttpGet("ConsultarVendaPeloIdentificador/{id}")]
        public async Task<IActionResult> ConsultarVendaPeloIdentificador(int? id = null)
        {
            if (!id.HasValue)
                return NotFound("Nenhum identificador de venda foi informado.");
            var venda = await _Context.Vendas
                .Include(p => p.Cliente)
                .Include(p => p.ItensVenda)
                .ThenInclude(i => i.Cerveja)
                .SingleOrDefaultAsync(p => p.IdVenda == id);
            if (venda == null)
                return NotFound("Nenhuma venda foi encontrada pelo identificador " + id + ".");
            return Ok(venda);
        }

        //http://{host}:{porta}/api/ConsultarVendas?pagina=xx&inicio=yy&final=zz
        [HttpGet("ConsultarVendas/{pagina?}/{inicio?}/{final?}")]
        public async Task<IActionResult> ConsultarVendas(int? pagina = 0, DateTime? inicio = null, DateTime? final = null)
        {
            // Páginando [OK]
            // Ordenando de forma decrescente pela data do início da venda [OK]
            // Range [OK]
            int jump = (pagina ?? 0) * _PageSize;
            string debug = !inicio.HasValue ? "Início não chegou" : inicio.Value.ToString("dd/MM/yyyy mm:HH:ss");
            debug += "   <===>   " + (!final.HasValue ? "Final não chegou" : final.Value.ToString("dd/MM/yyyy mm:HH:ss"));
            Console.WriteLine(debug);
            var vendas = await _Context.Vendas
                .Where(o => (!inicio.HasValue || o.InicioVenda >= inicio.Value) &&
                            (!final.HasValue || o.InicioVenda <= final.Value))
                .OrderByDescending(o => o.InicioVenda)
                .Skip(jump)
                .Take(_PageSize)
                .Include(o => o.Cliente)
                .Include(o => o.ItensVenda)
                .AsNoTracking().ToListAsync();
            return Ok(vendas);
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