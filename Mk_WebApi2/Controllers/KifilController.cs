using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mk_WebApi2.Model;
using Mk_WebApi2.Models;

namespace Mk_WebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KifilController : ControllerBase
    {
        private readonly AppDbContext _context;

        public KifilController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Kifil
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KifilModel>>> Getkifil()
        {
            return await _context.kifil.ToListAsync();
        }

        // GET: api/Kifil/5
        [HttpGet("{id}")]
        public async Task<ActionResult<KifilModel>> GetKifilModel(int id)
        {
            var kifilModel = await _context.kifil.FindAsync(id);

            if (kifilModel == null)
            {
                return NotFound();
            }

            return kifilModel;
        }

        // GET: api/Kifil/5
        [HttpGet("maekel-id/{maekelId}")]
        public async Task<ActionResult<KifilModel>> GetKifilModelFromMaekelId(int id)
        {
            try
            {
                DbDataReader result = null;

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT mk.description, k.kifil_id, k.name, mk.status FROM maekel_kifil mk JOIN " +
                                "kifil k ON mk.kifil_id = k.kifil_id " +
                                "WHERE mk.maekel_id =" + id + " " +
                                "AND mk.trash = 0";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {

                        ArrayList objs = new ArrayList();
                        while (reader.Read())
                        {
                            objs.Add(new
                            {
                                description = ImportFunctions.SafeGetString(reader, "description"),
                                kifil_id = ImportFunctions.SafeGetInt(reader, "kifil_id"),
                                name = ImportFunctions.SafeGetString(reader, "name"),
                                status = ImportFunctions.SafeGetInt(reader, "status")
                            });

                        }
                        var responce = new
                        {
                            data = objs
                        };
                        return new JsonResult(responce);
                    }
                }

            }
            catch(SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // PUT: api/Kifil/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKifilModel(int id, KifilModel kifilModel)
        {
            if (id != kifilModel.kifil_id)
            {
                return BadRequest();
            }

            _context.Entry(kifilModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KifilModelExists(id))
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

        // POST: api/Kifil
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<KifilModel>> PostKifilModel(KifilModel kifilModel)
        {
            _context.kifil.Add(kifilModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKifilModel", new { id = kifilModel.kifil_id }, kifilModel);
        }

        // DELETE: api/Kifil/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<KifilModel>> DeleteKifilModel(int id)
        {
            var kifilModel = await _context.kifil.FindAsync(id);
            if (kifilModel == null)
            {
                return NotFound();
            }

            _context.kifil.Remove(kifilModel);
            await _context.SaveChangesAsync();

            return kifilModel;
        }

        private bool KifilModelExists(int id)
        {
            return _context.kifil.Any(e => e.kifil_id == id);
        }
    }
}
