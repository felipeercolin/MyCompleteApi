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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/produtos")]
    public class ProdutosController : MainController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;
        private readonly IMapper _mapper;

        public ProdutosController(INotificador notificador,
                                  IProdutoRepository produtoRepository,
                                  IProdutoService produtoService,
                                  IMapper mapper,
                                  IUser user) : base(notificador, user)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores());
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var vmodel = await ObterProduto(id);

            if (vmodel == null) return NotFound();

            return vmodel;
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel vmodel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = $"{Guid.NewGuid()}_{vmodel.Imagem}";
            if (!await UploadArquivo(vmodel.ImagemUpload, imagemNome))
            {
                return CustomResponse(vmodel);
            }

            vmodel.Imagem = imagemNome; 
            await _produtoService.Adicionar(_mapper.Map<Produto>(vmodel));

            return CustomResponse(vmodel);
        }

        [ClaimsAuthorize("Produto", "Adicionar")]
        [HttpPost("adicionar-alterantivo")]
        public async Task<ActionResult<ProdutoViewModel>> AdicionarAlterativo(ProdutoImagemViewModel vmodel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefix = $"{Guid.NewGuid()}_";
            if (!await UploadArquivoAlternativo(vmodel.ImagemUpload, imgPrefix))
            {
                return CustomResponse(vmodel);
            }

            vmodel.Imagem = imgPrefix + vmodel.ImagemUpload.FileName;
            await _produtoService.Adicionar(_mapper.Map<Produto>(vmodel));

            return CustomResponse(vmodel);
        }

        //[DisableRequestSizeLimit]
        [RequestSizeLimit(40000000)]
        [HttpPost("image")]
        public ActionResult AdicionarImagem(IFormFile file)
        {
            return Ok(file);
        }

        [ClaimsAuthorize("Produto", "Atualizar")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Atualizar(Guid id, ProdutoViewModel vmodel)
        {
            if (id != vmodel.Id)
            {
                NotificarErro("O Id informado é diferente do Id passado na query");
                return CustomResponse(vmodel);
            }

            var vmodelAtualizacao = await ObterProduto(id);
            if (vmodelAtualizacao == null) return NotFound();
            vmodel.Imagem = vmodelAtualizacao.Imagem;
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (vmodel.ImagemUpload != null)
            {
                var imagemNome = $"{Guid.NewGuid()}_{vmodel.Imagem}";
                if (!await UploadArquivo(vmodel.ImagemUpload, imagemNome))
                {
                    return CustomResponse(vmodel);
                }

                vmodel.Imagem = imagemNome;
            }

            vmodelAtualizacao.Nome = vmodel.Nome;
            vmodelAtualizacao.Descricao = vmodel.Descricao;
            vmodelAtualizacao.Valor = vmodel.Valor;
            vmodelAtualizacao.Ativo = vmodel.Ativo;

            await _produtoService.Atualizar(_mapper.Map<Produto>(vmodelAtualizacao));

            return CustomResponse(vmodel);
        }

        [ClaimsAuthorize("Produto", "Excluir")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var vmodel = await ObterProduto(id);

            if (vmodel == null) return NotFound();

            await _produtoService.Remover(id);

            return vmodel;
        }

        private async Task<bool> UploadArquivo(string arquivo, string imgNome)
        {
            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma Imagem para o produto!");
                return false;
            }

            var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/imagens", imgNome);
            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(arquivo);
            await System.IO.File.WriteAllBytesAsync(filePath, imageDataByteArray);

            return true;
        }

        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length <= 0)
            {
                NotificarErro("Forneça uma Imagem para o produto!");
                return false;
            }

            var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);
            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await stream.CopyToAsync(stream);
            }

            return true;
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));
        }
    }
}
