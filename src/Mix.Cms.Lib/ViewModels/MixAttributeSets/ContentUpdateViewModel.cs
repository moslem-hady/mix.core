﻿using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSets
{
    public class ContentUpdateViewModel
      : ViewModelBase<MixCmsContext, MixAttributeSet, ContentUpdateViewModel>
    {
        #region Properties
        #region Models

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("type")]
        public int? Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("createdDateTime")]
        public DateTime CreatedDateTime { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }

        #endregion Models
        #region Views
        [JsonProperty("attributes")]
        public List<MixAttributeFields.UpdateViewModel> Attributes { get; set; }

        [JsonProperty("articleData")]
        public PaginationModel<MixArticleAttributeDatas.UpdateViewModel> ArticleData { get;set;}
        #endregion
        #endregion Properties
        #region Contructors

        public ContentUpdateViewModel() : base()
        {
        }

        public ContentUpdateViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
        {
        }

        #endregion Contructors
        #region Overrides
        public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            Attributes = MixAttributeFields.UpdateViewModel
                .Repository.GetModelListBy(a => a.AttributeSetId == Id, _context, _transaction).Data.OrderBy(a=>a.Priority).ToList();
        }
        public override MixAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            if (Id == 0)
            {
                Id = Repository.Max(s => s.Id, _context, _transaction).Data + 1;
                CreatedDateTime = DateTime.UtcNow;
            }
            return base.ParseModel(_context, _transaction);
        }

       
        #endregion

        #region Expand
        public void LoadArticleData(int articleId, string specificulture, int? pageSize = null, int? pageIndex = 0
            , MixCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getData = MixArticleAttributeDatas.UpdateViewModel.Repository
            .GetModelListBy(
                m => m.ArticleId == articleId && m.Specificulture == specificulture
                , MixService.GetConfig<string>(MixConstants.ConfigurationKeyword.OrderBy), 0
                , pageSize, pageIndex
                , _context: _context, _transaction: _transaction);
            if (!getData.IsSucceed || getData.Data == null || getData.Data.Items.Count==0)
            {
                ArticleData = new PaginationModel<MixArticleAttributeDatas.UpdateViewModel>() { TotalItems = 1};
                ArticleData.Items.Add(new MixArticleAttributeDatas.UpdateViewModel(Id, Attributes));
            }
            else
            {
                ArticleData = getData.Data;
            }
        }
        #endregion
    }
}