using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using RestApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class ManageController : Controller
    {
        private readonly IManageAcc _manaegAcc;
        public ManageController(IManageAcc manaegAcc)
        {
            _manaegAcc = manaegAcc;
        }
        
        [HttpGet]
        public async Task<IEnumerable<User>> GetAll()
        {
            return await _manaegAcc.GetAccs();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> FindAcc(string id)
        {
            return await _manaegAcc.GetAcc(id);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _manaegAcc.Delete(id);
            return NoContent();
        }
    }
}
