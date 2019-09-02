﻿// Licensed to the Mix I/O Foundation under one or more agreements.
// The Mix I/O Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mix.Domain.Core.ViewModels;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Services;
using System.Linq.Expressions;
using System.Web;
using Mix.Cms.Lib.ViewModels.MixPagePosts;
using Microsoft.AspNetCore.SignalR;
using Mix.Cms.Hub;
using Microsoft.Extensions.Caching.Memory;

namespace Mix.Cms.Api.Controllers.v1
{
    [Produces("application/json")]
    [Route("api/v1/{culture}/page-post")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiPagePostController))]
    public class ApiPagePostController :
        BaseGenericApiController<MixCmsContext, MixPagePost>
    {
        public ApiPagePostController(MixCmsContext context, IMemoryCache memoryCache, Microsoft.AspNetCore.SignalR.IHubContext<Hub.PortalHub> hubContext) : base(context, memoryCache, hubContext)
        {

        }
        #region Get

        // GET api/page/id
        [HttpGet, HttpOptions]
        [Route("delete/{pageId}/{postId}")]
        public async Task<RepositoryResponse<MixPagePost>> DeleteAsync(int pageId, int postId)
        {
            return await base.DeleteAsync<ReadViewModel>(
                model => model.PostId == postId && model.PageId == pageId && model.Specificulture == _lang, true);
        }

        // GET api/pages/id
        [HttpGet, HttpOptions]
        [Route("detail/{pageId}/{postId}/{viewType}")]
        public async Task<ActionResult<JObject>> Details(string viewType, int? pageId, int? postId)
        {
            string msg = string.Empty;
            switch (viewType)
            {

                default:
                    if (pageId.HasValue && postId.HasValue)
                    {
                        Expression<Func<MixPagePost, bool>> predicate = model => model.PageId == pageId && model.PostId == postId && model.Specificulture == _lang;
                        var portalResult = await base.GetSingleAsync<ReadViewModel>($"{viewType}_{pageId}_{postId}", predicate);
                        if (portalResult.IsSucceed)
                        {
                            portalResult.Data.Post.DetailsUrl = MixCmsHelper.GetRouterUrl(
                                new { culture = _lang, action = "post", portalResult.Data.Post.SeoName }, Request, Url);
                        }

                        return Ok(JObject.FromObject(portalResult));
                    }
                    else
                    {
                        var model = new MixPagePost()
                        {
                            Specificulture = _lang,
                            Status = MixService.GetConfig<int>("DefaultStatus"),
                            Priority = ReadViewModel.Repository.Max(a => a.Priority).Data + 1
                        };

                        RepositoryResponse<ReadViewModel> result = await base.GetSingleAsync<ReadViewModel>($"{viewType}_default", null, model);
                        return Ok(JObject.FromObject(result));
                    }
            }
        }


        #endregion Get

        #region Post

        // POST api/page
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin, Admin")]
        [HttpPost, HttpOptions]
        [Route("save")]
        public async Task<RepositoryResponse<ReadViewModel>> Save([FromBody]ReadViewModel model)
        {
            if (model != null)
            {
                var result = await base.SaveAsync<ReadViewModel>(model, true);
                return result;
            }
            return new RepositoryResponse<ReadViewModel>() { Status = 501 };
        }

        // POST api/page
        [HttpPost, HttpOptions]
        [Route("save/{id}/{postId}")]
        public async Task<RepositoryResponse<MixPagePost>> SaveFields(int pageId, int postId, [FromBody]List<EntityField> fields)
        {
            if (fields != null)
            {
                var result = new RepositoryResponse<MixPagePost>() { IsSucceed = true };
                foreach (var property in fields)
                {
                    if (result.IsSucceed)
                    {
                        result = await ReadViewModel.Repository.UpdateFieldsAsync(c => c.PageId == pageId && c.PostId == postId && c.Specificulture == _lang, fields).ConfigureAwait(false);
                    }
                    else
                    {
                        break;
                    }

                }
                return result;
            }
            return new RepositoryResponse<MixPagePost>();
        }

        // GET api/page
        [HttpPost, HttpOptions]
        [Route("list")]
        public async Task<ActionResult<JObject>> GetList(
            [FromBody] RequestPaging request)
        {
            var query = HttpUtility.ParseQueryString(request.Query ?? "");
            bool isPage = int.TryParse(query.Get("page_id"), out int pageId);            
            bool isPost = int.TryParse(query.Get("post_id"), out int postId);
            ParseRequestPagingDate(request);
            Expression<Func<MixPagePost, bool>> predicate = model =>
                        model.Specificulture == _lang
                        && (!isPage || model.PageId == pageId)                        
                        && (!isPost || model.PostId == postId)
                        && (!request.Status.HasValue || model.Status == request.Status.Value)
                        && (string.IsNullOrWhiteSpace(request.Keyword)
                            || (model.Description.Contains(request.Keyword)
                            ))
                        ;
            string key = $"{request.Key}_{request.Query}_{request.PageSize}_{request.PageIndex}";
            switch (request.Key)
            {
                default:
                    var listItemResult = await base.GetListAsync<ReadViewModel>(key, request, predicate);
                    listItemResult.Data.Items.ForEach(n => n.IsActived = true);
                    listItemResult.Data.Items.ForEach(n => n.Post.DetailsUrl = MixCmsHelper.GetRouterUrl(
                                new { culture = _lang, action = "post", id = n.Post.Id, seoName = n.Post.SeoName }, Request, Url));
                    return JObject.FromObject(listItemResult);
            }
        }

        [HttpPost, HttpOptions]
        [Route("update-infos")]
        public async Task<RepositoryResponse<List<ReadViewModel>>> UpdateInfos([FromBody]List<ReadViewModel> models)
        {
            if (models != null)
            {                
                return await base.SaveListAsync(models, false);
            }
            else
            {
                return new RepositoryResponse<List<ReadViewModel>>();
            }
        }
        // POST api/update-infos
        [HttpPost, HttpOptions]
        [Route("save-list")]
        public async Task<RepositoryResponse<List<ReadViewModel>>> SaveList([FromBody]List<ReadViewModel> models)
        {
            if (models != null)
            {
                return await base.SaveListAsync(models, false);
            }
            else
            {
                return new RepositoryResponse<List<ReadViewModel>>();
            }
        }
        #endregion Post
    }
}