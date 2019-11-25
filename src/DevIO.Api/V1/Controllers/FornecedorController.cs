using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevIO.Api.Controllers;
using DevIO.Api.Extentions;
using DevIO.Api.ViewModel;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/fornecedores")]
    public class FornecedorController : MainController
    {
        private static string ClaimName => "Fornecedor";

        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IFornecedorService _fornecedorService;
        private readonly IMapper _mapper;

        public FornecedorController(IFornecedorRepository fornecedorRepository,
                                    IEnderecoRepository enderecoRepository,
                                    IFornecedorService fornecedorService,
                                    IMapper mapper,
                                    INotificador notificador,
                                    IUser user) : base(notificador, user)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
            _fornecedorService = fornecedorService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
        {
            var fornecedores = await _fornecedorRepository.ObterTodos();
            var vmodel = _mapper.Map<IEnumerable<FornecedorViewModel>>(fornecedores);

            return vmodel;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
        {
            var vmodel = await ObterFornecedorProdutosEndereco(id);

            if (vmodel == null) return NotFound();

            return vmodel;
        }

        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel vmodel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(vmodel));

            return CustomResponse(vmodel);
        }

        [ClaimsAuthorize("Fornecedor", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel vmodel)
        {
            if (id != vmodel.Id)
            {
                NotificarErro("O Id informado é diferente do Id passado na query");
                return CustomResponse(vmodel);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(vmodel));

            return CustomResponse(vmodel);
        }

        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
        {
            var model = await ObterFornecedorEndereco(id);
            if (model == null) return BadRequest();

            await _fornecedorService.Remover(id);

            return CustomResponse();
        }


        [HttpGet("endereco/{id:guid}")]
        public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
        {
            var vmodel = _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
            return vmodel;
        }

        [ClaimsAuthorize("Fornecedor","Atualizar")]
        [HttpPut("endereco/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel vmodel)
        {
            if (id != vmodel.Id)
            {
                NotificarErro("O Id informado é diferente do Id passado na query");
                return CustomResponse(vmodel);
            }

            if (!ModelState.IsValid) return CustomResponse(vmodel);

            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(vmodel));

            return CustomResponse(vmodel);
        }

        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}