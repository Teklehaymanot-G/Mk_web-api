using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
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
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Account
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountModel>>> Getaccount()
        {
            return await _context.account.ToListAsync();
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountModel>> GetAccountModel(int id)
        {
            var accountModel = await _context.account.FindAsync(id);

            if (accountModel == null)
            {
                return NotFound();
            }

            return accountModel;
        }

        // POST: api/Account/sign-in
        [HttpPost("sign-in")]
        public ActionResult<AccountModel> SignInAccountModel(AccountModel accountModel)
        {
            try
            {
                int abalatId = -1;
                int maekelId = -1;
                int kifilId = -1;
                int status = -1;
                string name = "";
                string userType = "";
                string maekelName = "";
                string kifilName = "";

                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT abalat.abalat_id, name, user_type, maekel_id, kifil_id, account.status " +
                              "FROM account JOIN abalat " +
                              "ON account.abalat_id = abalat.abalat_id " +
                              "WHERE username = '" + accountModel.username + "' AND password = '" + accountModel.password + "' AND account.trash = 0";
                    command.CommandType = CommandType.Text;
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            abalatId = ImportFunctions.SafeGetInt(reader, "abalat_id");
                            maekelId = ImportFunctions.SafeGetInt(reader, "maekel_id");
                            kifilId = ImportFunctions.SafeGetInt(reader, "kifil_id");
                            name = ImportFunctions.SafeGetString(reader, "name");
                            userType = ImportFunctions.SafeGetString(reader, "user_type");
                            status = ImportFunctions.SafeGetInt(reader, "status");

                            using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command2.CommandText = "SELECT name FROM maekel WHERE maekel_id='" + maekelId + "'";
                                command2.CommandType = CommandType.Text;

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {
                                        maekelName = ImportFunctions.SafeGetString(reader2, "name");
                                    }
                                    reader2.Close();
                                }
                            }

                            using (var command2 = _context.Database.GetDbConnection().CreateCommand())
                            {
                                command2.CommandText = "SELECT name FROM kifil WHERE kifil_id='" + kifilId + "'";
                                command2.CommandType = CommandType.Text;

                                using (var reader2 = command2.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {
                                        kifilName = ImportFunctions.SafeGetString(reader2, "name");
                                    }
                                    reader2.Close();
                                }
                            }
                        }
                        reader.Close();
                    }
                }

                var response = new
                {
                    name = name,
                    abalat_id = abalatId,
                    user_type = userType,
                    maekel_id = maekelId,
                    kifil_id = kifilId,
                    status = status,
                    maekel_name = maekelName,
                    kifil_name = kifilName
                };

                return new JsonResult(response);

            }
            catch (SqlException ex)
            {
                return new JsonResult(ex.Message);
            }
            catch (Exception ex)
            {
                return new JsonResult(ex.Message);
            }
        }

        // PUT: api/Account/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountModel(int id, AccountModel accountModel)
        {
            if (id != accountModel.account_id)
            {
                return BadRequest();
            }

            _context.Entry(accountModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountModelExists(id))
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

        // POST: api/Account
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<AccountModel>> PostAccountModel(AccountModel accountModel)
        {
            _context.account.Add(accountModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountModel", new { id = accountModel.account_id }, accountModel);
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AccountModel>> DeleteAccountModel(int id)
        {
            var accountModel = await _context.account.FindAsync(id);
            if (accountModel == null)
            {
                return NotFound();
            }

            _context.account.Remove(accountModel);
            await _context.SaveChangesAsync();

            return accountModel;
        }

        private bool AccountModelExists(int id)
        {
            return _context.account.Any(e => e.account_id == id);
        }

    }
}
