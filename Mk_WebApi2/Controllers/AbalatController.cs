using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Mk_WebApi2.Model;
using Mk_WebApi2.Models;
using Newtonsoft.Json;

namespace Mk_WebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbalatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AbalatController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Abalat
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AbalatModel>>> Getabalat()
        {
            // return all abalat info with maekel info
            try
            {
                var abalat = new JsonResult("");
                var maekel = new JsonResult("");

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT tblAbalat.*, maekel.name as 'maekel_name' FROM "+
                          "abalat tblAbalat JOIN maekel on tblAbalat.maekel_id = maekel.maekel_id "+
                          "WHERE tblAbalat.trash = 0 "+
                          "ORDER by maekel.name, tblAbalat.name DESC";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        abalat = new JsonResult(rows.ToList());
                    }
                }
                using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command2.CommandText = "SELECT maekel_id, name FROM maekel WHERE status = 1 AND trash = 0 ORDER BY name";
                    command2.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();
                    
                    using (var reader2 = command2.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader2);
                        maekel = new JsonResult(rows.ToList());
                    }
                }

                var responce = new
                {
                    data = abalat.Value,
                    maekelInfo = maekel.Value
                };
                var result = responce;

                return new JsonResult(responce);

            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // GET: api/Abalat/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<AbalatModel>>> GetAbalatDashboard()
        {
            // return abalat stat for dashboard
            try
            {
                int total = -1;
                int this_percentage = -1;
                int last_percentage = -1;

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT count(abalat_id) as count_abalat FROM abalat WHERE status=1";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = ImportFunctions.SafeGetInt(reader, "count_abalat");
                        }
                    }
                }

                DateTime curdate = DateTime.Now;
                this_percentage = curdate.Hour;

                var thisMonth = DateTime.Now;
                var lastMonth = thisMonth.AddMonths(-1);

                using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command2.CommandText = "SELECT count([abalat_id]) as count_abalat "+
                      "FROM[mezmur_and_kinetibeb].[dbo].[abalat] WHERE status = 1 AND "+
                       "created_on BETWEEN "+
                      "CONVERT(datetime2, '"+thisMonth+"') "+
                      "AND CONVERT(datetime2, '"+curdate+"')";
                    command2.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            this_percentage = ImportFunctions.SafeGetInt(reader2, "count_abalat");
                        }
                    }
                }

                using (var command3 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command3.CommandText = "SELECT count([abalat_id]) as count_abalat " +
                      "FROM[mezmur_and_kinetibeb].[dbo].[abalat] WHERE status = 1 AND " +
                       "created_on BETWEEN " +
                      "CONVERT(datetime2, '" + lastMonth + "') " +
                      "AND CONVERT(datetime2, '" + thisMonth + "')";
                    command3.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader3 = command3.ExecuteReader())
                    {
                        if (reader3.Read())
                        {
                            last_percentage = ImportFunctions.SafeGetInt(reader3, "count_abalat");
                        }
                    }
                }

                var response = new
                {
                    total = total,
                    this_percentage = this_percentage,
                    last_percentage = last_percentage
                };

                return new JsonResult(response);

            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // GET: api/Abalat/get-by-maekel/5
        [HttpGet("get-by-maekel/{maekelId}")]
        public async Task<ActionResult<AbalatModel>> GetAbalatByMaekel(int maekelId)
        {
            try
            {
                var abalat = new JsonResult("");
                var maekel = new JsonResult("");
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT tblAbalat.*, maekel.name as 'maekel_name' FROM " +
                          "abalat tblAbalat JOIN maekel on tblAbalat.maekel_id = maekel.maekel_id " +
                          "WHERE tblAbalat.trash = 0 AND tblAbalat.maekel_id = " + maekelId +" "+
                          "ORDER by maekel.name, tblAbalat.name DESC";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        abalat = new JsonResult(rows.ToList());
                    }
                }
                using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command2.CommandText = "SELECT maekel_id, name FROM maekel WHERE status = 1 AND trash = 0 ORDER BY name";
                    command2.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader2);
                        maekel = new JsonResult(rows.ToList());
                    }
                }

                var responce = new
                {
                    data = abalat.Value,
                    maekelInfo = maekel.Value
                };
                var result = responce;

                return new JsonResult(responce);
            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // search abalat info for ማስተባበሪያ ኃላፊ 
        [HttpPost("search")]
        public async Task<ActionResult<AbalatModel>> SearchAbalatModel([FromBody] JsonElement info)
        {
            try
            {
                var searchQuery = info.GetProperty("searchValue");

                var abalat = new JsonResult("");
                string query = "";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    query = "SELECT * FROM abalat WHERE upper(name) LIKE '%" + searchQuery + "%' AND trash=0 UNION " +
                            "SELECT * FROM abalat WHERE upper(phone) LIKE '%" + searchQuery + "%' AND trash=0 UNION " +
                            "SELECT * FROM abalat WHERE upper(sex) LIKE '%" + searchQuery + "%' AND trash=0 UNION " +
                            "SELECT * FROM abalat WHERE upper(age) LIKE '%" + searchQuery + "%' AND trash=0 UNION " +
                            "SELECT * FROM abalat WHERE upper(email) LIKE '%" + searchQuery + "%' AND trash=0";
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        abalat = new JsonResult(rows.ToList());
                    }
                }

                return new JsonResult(abalat.Value);
            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }








        // GET: api/Abalat/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AbalatModel>> GetAbalatModel(int id)
        {
            var abalatModel = await _context.abalat.FindAsync(id);

            if (abalatModel == null)
            {
                return NotFound();
            }

            return abalatModel;
        }

        // PUT: api/Abalat/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAbalatModel(int id, AbalatModel abalatModel)
        {
            if (id != abalatModel.abalat_id)
            {
                return BadRequest();
            }

            _context.Entry(abalatModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbalatModelExists(id))
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

        // POST: api/Abalat
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AbalatModel>> PostAbalatModel(AbalatModel abalatModel)
        {
            _context.abalat.Add(abalatModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAbalatModel", new { id = abalatModel.abalat_id }, abalatModel);
        }


        

        // DELETE: api/Abalat/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AbalatModel>> DeleteAbalatModel(int id)
        {
            var abalatModel = await _context.abalat.FindAsync(id);
            if (abalatModel == null)
            {
                return NotFound();
            }

            _context.abalat.Remove(abalatModel);
            await _context.SaveChangesAsync();

            return abalatModel;
        }

        private bool AbalatModelExists(int id)
        {
            return _context.abalat.Any(e => e.abalat_id == id);
        }
    }
}
