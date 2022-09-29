using AutoMapper;
using EnglishSchool.Common.Dtos;
using EnglishSchool.DAL.Interfaces;
using EnglishSchool.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Collections.Generic;

namespace EnglishSchool.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "AppUser")]

    public class MaterialsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MaterialsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<MaterialDto>> GetMaterials(CancellationToken token)
        {
            var materials = _mapper.Map<List<MaterialDto>>(await _unitOfWork.Materials.GetAll(token));
            return materials;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterial(int id, CancellationToken token)
        {
            var material = _mapper.Map<MaterialDto>(await _unitOfWork.Materials.GetById(id, token));

            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        [HttpGet("levels/{materialId}")]
        public async Task<IActionResult> GetLevelsOfMaterial(int materialId, CancellationToken token)
        {
            var material = await _unitOfWork.Materials.GetById(materialId, token);
            
            if (material == null)
            {
                return NotFound();
            }

            var levels = _mapper.Map<List<LevelDto>>
                (await _unitOfWork.Materials.GetSubentities(materialId, token));

            return Ok(levels);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterial([FromBody] MaterialDto materialDto,
            CancellationToken token)
        {
            if (materialDto == null)
            {
                return BadRequest();
            }

            var material = _mapper.Map<Material>(materialDto);

            if(!await _unitOfWork.Materials.Create(material, token))
            {
                ModelState.AddModelError("", "Can't create material");

                return StatusCode(500, ModelState);
            }
            
            return Ok("material created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial([FromBody] MaterialDto materialDto,
            int id, CancellationToken token)
        {
            if (materialDto == null)
            {
                return NotFound();
            }

            var material = _mapper.Map<Material>(materialDto);
            material.Id = id;

            await _unitOfWork.Materials.Update(material, token);
           

            return NoContent();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteMaterial(int id, CancellationToken token)
        {
            await _unitOfWork.Materials.Delete(id);

            return Ok();
        }
    }
}
