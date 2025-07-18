﻿using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Domain.Interfaces.Services;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de criação de usuário com dados nulos.");
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                var usuario = await _usuarioService.CriarUsuarioAsync(dto);
                _logger.LogInformation("Usuário criado com sucesso. ID: {UsuarioID}", usuario.Id);

                return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, new
                {
                    message = MensagemApi.CriacaoSucesso.GetMensagem(),
                    usuario
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao criar usuário (email duplicado).");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar usuário.");
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuário com ID {Id} não encontrado.", id);
                    return NotFound(new { message = MensagemApi.RegistroNaoEncontrado.GetMensagem() });
                }

                _logger.LogInformation("Usuário encontrado. ID: {Id}", id);
                return Ok(new { message = MensagemApi.ListaSucesso.GetMensagem(), usuario });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Obtém um usuário pelo email.
        /// </summary>
        [HttpGet("email")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObterPorEmail([FromQuery] string email)
        {
            try
            {
                var usuario = await _usuarioService.ObterUsuarioPorEmailAsync(email);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuário com email '{Email}' não encontrado.", email);
                    return NotFound(new { message = MensagemApi.RegistroNaoEncontrado.GetMensagem() });
                }

                _logger.LogInformation("Usuário encontrado por email: {Email}", email);
                return Ok(new { message = MensagemApi.ListaSucesso.GetMensagem(), usuario });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Tentativa de atualização com DTO nulo. ID: {Id}", id);
                return BadRequest(new { message = MensagemApi.DadosInvalidos.GetMensagem() });
            }

            try
            {
                dto.Id = id;
                await _usuarioService.AtualizarUsuarioAsync(dto);
                _logger.LogInformation("Usuário atualizado com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.AtualizacaoSucesso.GetMensagem() });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuário não encontrado ao tentar atualizar. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }

        /// <summary>
        /// Deleta um usuário pelo ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                await _usuarioService.DeletarUsuarioAsync(id);
                _logger.LogInformation("Usuário removido com sucesso. ID: {Id}", id);
                return Ok(new { message = MensagemApi.RemocaoSucesso.GetMensagem() });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Usuário não encontrado ao tentar deletar. ID: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário. ID: {Id}", id);
                return StatusCode(500, new { message = MensagemApi.ErroInterno.GetMensagem() });
            }
        }
    }
}
