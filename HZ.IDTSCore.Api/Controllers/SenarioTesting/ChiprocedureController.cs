using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class ChiprocedureController : BaseController
    {
        private IChiprocedureService _IChiprocedureService;

        public ChiprocedureController()
        {
            _IChiprocedureService = ServiceLocator.GetService<IChiprocedureService>(HttpContextSession());
        }
    }
}
