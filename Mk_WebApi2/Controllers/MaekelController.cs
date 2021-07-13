using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
    public class MaekelController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MaekelController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Maekel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaekelModel>>> Getmaekel()
        {
            try
            {
                List<string> defaultValue = new List<string>();
                var maekel = new JsonResult(defaultValue);
                var mm = new JsonResult(defaultValue);
                var abalatInfo = new JsonResult(defaultValue);
                var usernameList = new JsonResult(defaultValue);

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT * FROM maekel WHERE trash=0 ORDER BY name";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        maekel = new JsonResult(rows.ToList());
                    }
                }
                using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command2.CommandText = "SELECT account_id, maekel_id, name "+
                                "FROM account JOIN abalat ON account.abalat_id = abalat.abalat_id "+
                                "WHERE user_type = 'ማዕከል ኃላፊ' AND account.trash = 0";
                    command2.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader2);
                        mm = new JsonResult(rows.ToList());
                    }
                }
                using (var command3 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command3.CommandText = "SELECT abalat_id, maekel_id, name FROM abalat WHERE status=1 AND trash=0 "+
                        "AND abalat_id NOT IN(SELECT abalat_id FROM account WHERE trash = 0)";
                    command3.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader3 = command3.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader3);
                        mm = new JsonResult(rows.ToList());
                    }
                }
                using (var command4 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command4.CommandText = "SELECT username FROM account";
                    command4.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader4 = command4.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader4);
                        usernameList = new JsonResult(rows.ToList());
                    }
                }


                var responce = new
                {
                    data = maekel.Value,
                    mm = mm.Value,
                    abalatInfo = abalatInfo.Value,
                    usernameList = usernameList.Value
                };

                return new JsonResult(responce);
            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // GET: api/Maekel/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<MaekelModel>>> GetMaekelDashboard()
        {
            try
            {
                int total = -1;
                int this_percentage = -1;
                int last_percentage = -1;

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT count(maekel_id) as count_maekel FROM maekel WHERE status=1";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            total = ImportFunctions.SafeGetInt(reader, "count_maekel");
                        }
                    }
                }

                DateTime curdate = DateTime.Now;
                this_percentage = curdate.Hour;

                var thisMonth = DateTime.Now;
                var lastMonth = thisMonth.AddMonths(-1);

                using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command2.CommandText = "SELECT count([maekel_id]) as count_maekel " +
                      "FROM[mezmur_and_kinetibeb].[dbo].[maekel] WHERE status = 1 AND " +
                       "created_on BETWEEN " +
                      "CONVERT(datetime2, '" + thisMonth + "') " +
                      "AND CONVERT(datetime2, '" + curdate + "')";
                    command2.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader2 = command2.ExecuteReader())
                    {
                        if (reader2.Read())
                        {
                            this_percentage = ImportFunctions.SafeGetInt(reader2, "count_maekel");
                        }
                    }
                }

                using (var command3 = _context.Database.GetDbConnection().CreateCommand())
                {
                    command3.CommandText = "SELECT count([maekel_id]) as count_maekel " +
                      "FROM[mezmur_and_kinetibeb].[dbo].[maekel] WHERE status = 1 AND " +
                       "created_on BETWEEN " +
                      "CONVERT(datetime2, '" + lastMonth + "') " +
                      "AND CONVERT(datetime2, '" + thisMonth + "')";
                    command3.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader3 = command3.ExecuteReader())
                    {
                        if (reader3.Read())
                        {
                            last_percentage = ImportFunctions.SafeGetInt(reader3, "count_maekel");
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

        // GET: api/Maekel/get-id-name
        [HttpGet("get-id-name/")]
        public async Task<ActionResult<MaekelModel>> GetMaekelIdName(int id)
        {
            try
            {
                var maekel = new JsonResult("");
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT maekel_id,name FROM maekel WHERE status=1 AND trash=0 ORDER BY name";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        maekel = new JsonResult(rows.ToList());
                    }
                }

                var responce = new
                {
                    data = maekel.Value,
                };

                return new JsonResult(responce);
            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // POST: api/Maekel
        [HttpPost]
        public async Task<ActionResult<MaekelModel>> PostMaekelModel([FromBody] MaekelModel maekelModel)
        {
            try
            {
                maekelModel.created_on = DateTime.Now;
                maekelModel.modified_on = DateTime.Now;
                maekelModel.modified_by = maekelModel.created_by;

                _context.maekel.Add(maekelModel);
                await _context.SaveChangesAsync();

                var responce = new
                {
                    success = true,
                    serverResponseFound = true,
                    makeFormEmpty = true
                };
                return new JsonResult(responce);
            }
            catch (Exception)
            {
                var res = new
                {
                    success = false,
                    serverResponseFound = true,
                    makeFormEmpty = false
                };
                return new JsonResult(res);
            }
        }

        // search maekel info
        [HttpPost("search")]
        public async Task<ActionResult<AbalatModel>> SearchMaekelModel([FromBody] JsonElement info)
        {
            try
            {
                var searchQuery = info.GetProperty("searchValue");
                
                List<string> defaultValue = new List<string>();
                var maekel = new JsonResult(defaultValue);
                string query = "";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    query = "SELECT maekel.maekel_id, maekel.name as maekel_name, abalat.abalat_id, "+
                          "abalat.name, abalat.phone, account.account_id, account.status, "+
                          "account.created_by, account.created_on, account.modified_by, "+
                          "account.modified_on, account.description "+
                       "FROM maekel " +
                           "LEFT JOIN abalat ON abalat.maekel_id = maekel.maekel_id " +
                           "LEFT JOIN account ON account.abalat_id = abalat.abalat_id " +
                       "WHERE account.user_type = 'Maekel Manager' AND account.trash = 0 " +
                            "AND(upper(maekel.name) LIKE '%"+searchQuery+"%' OR " +
                                  "upper(abalat.name) LIKE '%"+searchQuery+"%' OR " +
                                  "upper(account.description) LIKE '%"+searchQuery+"%')";
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var rows = ImportFunctions.ConvertToDictionary(reader);
                        maekel = new JsonResult(rows.ToList());
                    }
                }

                var response = new
                {
                    data = maekel.Value
                };
                return new JsonResult(query);
            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // POST: api/Maekel/import
        [HttpPost("import")]
        public async Task<ActionResult<MaekelModel>> PostMaekelImportModel([FromBody] Object obj)
        {
            try
            {
                var data = JsonDocument.Parse(obj.ToString());

                foreach (var item in data.RootElement.GetProperty("formState").GetProperty("values").EnumerateArray())
                {
                    MaekelModel maekelModel = new MaekelModel();
                    maekelModel.created_by = checkValueIsInt(data.RootElement.GetProperty("formState").GetProperty("createdBy").ToString())
                        ? int.Parse(data.RootElement.GetProperty("formState").GetProperty("createdBy").ToString())
                        : -1;

                    maekelModel.created_on = DateTime.Now;
                    maekelModel.modified_on = DateTime.Now;
                    maekelModel.modified_by = maekelModel.created_by;

                    maekelModel.name = item.GetProperty("name").ToString();
                    maekelModel.location = item.GetProperty("location").ToString();
                    maekelModel.description = item.GetProperty("description").ToString();
                    maekelModel.status = checkValueIsInt(item.GetProperty("status").ToString()) 
                        ? int.Parse(item.GetProperty("status").ToString())
                        : 0;

                    _context.maekel.Add(maekelModel);
                    //aspLog(maekelModel.ToString());
                    await _context.SaveChangesAsync();
                }


                var responce = new
                {
                    success = true,
                    serverResponseFound = true,
                    makeFormEmpty = true,
                };

                return new JsonResult(responce);
            }
            catch (Exception e)
            {
                var res = new
                {
                    success = false,
                    serverResponseFound = true,
                    makeFormEmpty = false
                };
                return new JsonResult(e.InnerException.Message);
            }
        }

        private bool checkValueIsInt(String str)
        {
            int def;
            return int.TryParse(str, out def);
        }

        private void aspLog(String val)
        {
            Response.WriteAsync(val);
            //Response.WriteAsync("<script language=javascript>console.log('" & val & "'); </script>");
        }


        // GET: api/Maekel/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MaekelModel>> GetMaekelModel(int id)
        {
            var maekelModel = await _context.maekel.FindAsync(id);

            if (maekelModel == null)
            {
                return NotFound();
            }

            return maekelModel;
        }

        // PUT: api/Maekel/changeStatus
        // change status
        [HttpPut("changeStatus")]
        public async Task<ActionResult<MaekelModel>> PutMaekelModel_ChangeStatus([FromBody] Object obj)
        {

            try
            {
                MaekelModel maekelModel = new MaekelModel();

                maekelModel.modified_on = DateTime.Now;
                var data = JsonDocument.Parse(obj.ToString());
                maekelModel.maekel_id = checkValueIsInt(data.RootElement.GetProperty("id").ToString())
                        ? int.Parse(data.RootElement.GetProperty("id").ToString())
                        : -1;
                int id = maekelModel.maekel_id;

                maekelModel.status = checkValueIsInt(data.RootElement.GetProperty("status").ToString())
                        ? int.Parse(data.RootElement.GetProperty("status").ToString())
                        : -1;
                maekelModel.modified_by = checkValueIsInt(data.RootElement.GetProperty("modifiedBy").ToString())
                        ? int.Parse(data.RootElement.GetProperty("modifiedBy").ToString())
                        : -1;

                var entry = _context.Entry(maekelModel);
                entry.Property(m => m.status).IsModified = true;
                await _context.SaveChangesAsync();

                var responce = new
                {
                    success = true,
                    serverResponseFound = true,
                    makeFormEmpty = true,
                };

                return new JsonResult(responce);
            }
            catch(Exception e)
            {
                return new JsonResult(e.Message);
            }
        }

        
        // DELETE: api/Maekel/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MaekelModel>> DeleteMaekelModel(int id)
        {
            var maekelModel = await _context.maekel.FindAsync(id);
            if (maekelModel == null)
            {
                return NotFound();
            }

            _context.maekel.Remove(maekelModel);
            await _context.SaveChangesAsync();

            var responce = new
            {
                success = true,
                serverResponseFound = true,
                makeFormEmpty = true,
            };
            return new JsonResult(responce);
        }

        private bool MaekelModelExists(int id)
        {
            return _context.maekel.Any(e => e.maekel_id == id);
        }
    }
}
