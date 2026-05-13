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
    public class ChisceneController : BaseController
    {
        private IChisceneService _IChiscenService;

        public ChisceneController()
        {
            _IChiscenService = ServiceLocator.GetService<IChisceneService>(HttpContextSession());
        }
    }
}
