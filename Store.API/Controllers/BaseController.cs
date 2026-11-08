using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Interfaces;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(IUnitOfWork work, IMapper mapper) : ControllerBase
    {
        protected readonly IUnitOfWork _work = work;
        protected readonly IMapper _mapper = mapper;
    }
}
