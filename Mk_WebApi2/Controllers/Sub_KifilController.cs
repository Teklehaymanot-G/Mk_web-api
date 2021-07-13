using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mk_WebApi2.Model;
using Mk_WebApi2.Models;

namespace Mk_WebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Sub_KifilController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Sub_KifilController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Sub_Kifil
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sub_KifilModel>>> Getsub_kifil()
        {
            return await _context.sub_kifil.ToListAsync();
        }

        // GET: api/Sub_Kifil/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sub_KifilModel>> GetSub_KifilModel(int id)
        {
            var sub_KifilModel = await _context.sub_kifil.FindAsync(id);

            if (sub_KifilModel == null)
            {
                return NotFound();
            }

            return sub_KifilModel;
        }

        // PUT: api/Sub_Kifil/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSub_KifilModel(int id, Sub_KifilModel sub_KifilModel)
        {
            if (id != sub_KifilModel.sub_kifil_id)
            {
                return BadRequest();
            }

            _context.Entry(sub_KifilModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Sub_KifilModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Sub_Kifil
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Sub_KifilModel>> PostSub_KifilModel(Sub_KifilModel sub_KifilModel)
        {
            _context.sub_kifil.Add(sub_KifilModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSub_KifilModel", new { id = sub_KifilModel.sub_kifil_id }, sub_KifilModel);
        }

        // DELETE: api/Sub_Kifil/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Sub_KifilModel>> DeleteSub_KifilModel(int id)
        {
            var sub_KifilModel = await _context.sub_kifil.FindAsync(id);
            if (sub_KifilModel == null)
            {
                return NotFound();
            }

            _context.sub_kifil.Remove(sub_KifilModel);
            await _context.SaveChangesAsync();

            return sub_KifilModel;
        }

        private bool Sub_KifilModelExists(int id)
        {
            return _context.sub_kifil.Any(e => e.sub_kifil_id == id);
        }
    }
}
