﻿// Licensed to the Mix I/O Foundation under one or more agreements.
// The Mix I/O Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mix.Domain.Core.ViewModels;
using Mix.Cms.Lib.Models.Cms;
using System.Linq.Expressions;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Mix.Common.Helper;
using Mix.Cms.Lib.Services;
using static Mix.Cms.Lib.MixEnums;

namespace Mix.Cms.Api.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/{culture}/module-data")]
    public class ApiModuleDataController :
        BaseGenericApiController<MixCmsContext, MixModuleData>
    {
        public ApiModuleDataController(IMemoryCache memoryCache, Microsoft.AspNetCore.SignalR.IHubContext<Hub.PortalHub> hubContext) : base(memoryCache, hubContext)
        {
        }

        // GET api/module-data/id
        [AllowAnonymous]
        [HttpGet, HttpOptions]
        [Route("details/{viewType}/{moduleId}/{id}")]
        [Route("details/{viewType}/{moduleId}")]
        public async Task<RepositoryResponse<UpdateViewModel>> DetailsAsync(string viewType, int moduleId, string id = null)
        {

            if (string.IsNullOrEmpty(id))
            {

                var getModule = await Lib.ViewModels.MixModules.ReadListItemViewModel.Repository.GetSingleModelAsync(
        m => m.Id == moduleId && m.Specificulture == _lang).ConfigureAwait(false);
                if (getModule.IsSucceed)
                {
                    var model = new MixModuleData(
                        )
                    {
                        ModuleId = moduleId,
                        Specificulture = _lang,
                        Fields = getModule.Data.Fields,
                        Status = MixService.GetConfig<int>("DefaultContentStatus")
                    };
                    RepositoryResponse<UpdateViewModel> result = await base.GetSingleAsync<UpdateViewModel>($"{viewType}_default", null, model);

                    return result;
                }
                else
                {
                    return new RepositoryResponse<UpdateViewModel>() { IsSucceed = false };
                }
            }
            else
            {
                Expression<Func<MixModuleData, bool>> predicate = model => model.Id == id && model.Specificulture == _lang;
                var portalResult = await base.GetSingleAsync<UpdateViewModel>($"{viewType}_{id}", predicate);
                return portalResult;
            }
        }

        // GET api/module-data/id
        [HttpGet, HttpOptions]
        [Route("edit/{id}")]
        public Task<RepositoryResponse<ReadViewModel>> Edit(string id)
        {
            return base.GetSingleAsync<ReadViewModel>($"read_{id}", model => model.Id == id && model.Specificulture == _lang);
        }

        // GET api/module-data/create/id
        [HttpGet, HttpOptions]
        [Route("create/{moduleId}")]
        public async Task<RepositoryResponse<UpdateViewModel>> CreateAsync(int moduleId)
        {
            var getModule = await Lib.ViewModels.MixModules.ReadListItemViewModel.Repository.GetSingleModelAsync(
                m => m.Id == moduleId && m.Specificulture == _lang).ConfigureAwait(false);
            if (getModule.IsSucceed)
            {
                var ModuleData = new UpdateViewModel(
                    new MixModuleData()
                    {
                        ModuleId = moduleId,
                        Specificulture = _lang,
                        Fields = getModule.Data.Fields
                    });
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = true,
                    Data = ModuleData
                };
            }
            else
            {
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = getModule.Exception,
                    Errors = getModule.Errors
                };
            }
        }

        // GET api/module-data/create/id
        [AllowAnonymous]
        [HttpGet, HttpOptions]
        [Route("init-by-name/{moduleName}")]
        public async Task<RepositoryResponse<UpdateViewModel>> InitViewAsync(string moduleName)
        {
            var getModule = await Lib.ViewModels.MixModules.ReadListItemViewModel.Repository.GetSingleModelAsync(
                m => m.Name == moduleName && m.Specificulture == _lang).ConfigureAwait(false);
            if (getModule.IsSucceed)
            {
                var ModuleData = new UpdateViewModel(
                    new MixModuleData()
                    {
                        ModuleId = getModule.Data.Id,
                        Specificulture = _lang,
                        Fields = getModule.Data.Fields
                    });
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = true,
                    Data = ModuleData
                };
            }
            else
            {
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = getModule.Exception,
                    Errors = getModule.Errors
                };
            }
        }

        // GET api/module-data/create/id
        [AllowAnonymous]
        [HttpGet, HttpOptions]        
        [Route("init/{moduleId}")]
        public async Task<RepositoryResponse<UpdateViewModel>> InitByIdAsync(int moduleId)
        {
            var getModule = await Lib.ViewModels.MixModules.ReadListItemViewModel.Repository.GetSingleModelAsync(
                m => m.Id == moduleId && m.Specificulture == _lang).ConfigureAwait(false);
            if (getModule.IsSucceed)
            {
                var ModuleData = new UpdateViewModel(
                    new MixModuleData()
                    {
                        ModuleId = getModule.Data.Id,
                        Specificulture = _lang,
                        Fields = getModule.Data.Fields
                    });
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = true,
                    Data = ModuleData
                };
            }
            else
            {
                return new RepositoryResponse<UpdateViewModel>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = getModule.Exception,
                    Errors = getModule.Errors
                };
            }
        }

        // GET api/module-data/id
        [HttpGet, HttpOptions]
        [Route("delete/{id}")]
        public async Task<RepositoryResponse<MixModuleData>> DeleteAsync(string id)
        {
            return await base.DeleteAsync<ReadViewModel>(model => model.Id == id && model.Specificulture == _lang);
        }

        #region Post

        // POST api/moduleData
        [AllowAnonymous]
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<RepositoryResponse<UpdateViewModel>> Post([FromBody]UpdateViewModel data)
        {
            return await base.SaveAsync<UpdateViewModel>(data, true);
        }

        // GET api/moduleData
        [AllowAnonymous]
        [HttpPost, HttpOptions]
        [Route("export")]
        public async Task<ActionResult<RepositoryResponse<PaginationModel<ReadViewModel>>>> ExportData(
            [FromBody] RequestPaging request)
        {
            var query = HttpUtility.ParseQueryString(request.Query ?? "");
            int.TryParse(query.Get("module_id"), out int moduleId);
            int.TryParse(query.Get("article_id"), out int articleId);
            int.TryParse(query.Get("product_id"), out int productId);
            int.TryParse(query.Get("category_id"), out int categoryId);
            string key = $"{request.Key}_{request.PageSize}_{request.PageIndex}";
            Expression<Func<MixModuleData, bool>> predicate = model =>
                model.Specificulture == _lang
                && model.ModuleId == moduleId
                && (articleId == 0 || model.ArticleId == articleId)
                && (productId == 0 || model.ProductId == productId)
                && (categoryId == 0 || model.CategoryId == categoryId)
                && (!request.FromDate.HasValue
                    || (model.CreatedDateTime >= request.FromDate.Value.ToUniversalTime())
                )
                && (!request.ToDate.HasValue
                    || (model.CreatedDateTime <= request.ToDate.Value.ToUniversalTime())
                )
                    ;
            var portalResult = await base.GetListAsync<ReadViewModel>(key, request, predicate);
            foreach (var item in portalResult.Data.Items)
            {
                portalResult.Data.JsonItems.Add(item.JItem);
            }
            string exportPath = $"Exports/Module/{moduleId}";
            var result = CommonHelper.ExportJObjectToExcel(portalResult.Data.JsonItems, string.Empty, exportPath, moduleId.ToString(), null);
            return Ok(JObject.FromObject(result));
        }

        // GET api/moduleData
        [AllowAnonymous]
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<ActionResult<RepositoryResponse<PaginationModel<ReadViewModel>>>> GetList(
            [FromBody] RequestPaging request, int? level = 0)
        {
            var query = HttpUtility.ParseQueryString(request.Query ?? "");
            int.TryParse(query.Get("module_id"), out int moduleId);
            int.TryParse(query.Get("article_id"), out int articleId);
            int.TryParse(query.Get("product_id"), out int productId);
            int.TryParse(query.Get("category_id"), out int categoryId);
            string key = $"{request.Key}_{request.PageSize}_{request.PageIndex}";
            Expression<Func<MixModuleData, bool>> predicate = model =>
                model.Specificulture == _lang
                && model.ModuleId == moduleId
                && (articleId==0 || model.ArticleId == articleId)
                && (productId == 0 || model.ProductId == productId)
                && (categoryId == 0 || model.CategoryId == categoryId)
                && (!request.FromDate.HasValue
                    || (model.CreatedDateTime >= request.FromDate.Value.ToUniversalTime())
                )
                && (!request.ToDate.HasValue
                    || (model.CreatedDateTime <= request.ToDate.Value.ToUniversalTime())
                )
                    ;
            var portalResult = await base.GetListAsync<ReadViewModel>(key, request, predicate);

            return Ok(JObject.FromObject(portalResult));
        }

        // POST api/PortalPage
        [HttpPost, HttpOptions]
        [Route("update-infos")]
        public async Task<RepositoryResponse<List<ReadViewModel>>> UpdateInfos([FromBody]List<ReadViewModel> models)
        {
            if (models != null)
            {
                RemoveCache();
                return await ReadViewModel.UpdateInfosAsync(models);
            }
            else
            {
                return new RepositoryResponse<List<ReadViewModel>>();
            }
        }
        
        #endregion Post
    }
}