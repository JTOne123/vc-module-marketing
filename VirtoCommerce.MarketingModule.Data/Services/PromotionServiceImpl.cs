﻿using System;
using System.Collections.Generic;
using System.Linq;
using CacheManager.Core;
using Omu.ValueInjecter;
using VirtoCommerce.Domain.Marketing.Model;
using VirtoCommerce.Domain.Marketing.Services;
using VirtoCommerce.MarketingModule.Data.Converters;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Serialization;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class PromotionServiceImpl : ServiceBase, IPromotionService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IMarketingExtensionManager _customPromotionManager;
        private readonly IExpressionSerializer _expressionSerializer;
        private readonly ICacheManager<object> _cacheManager;

        public PromotionServiceImpl(Func<IMarketingRepository> repositoryFactory, IMarketingExtensionManager customPromotionManager, IExpressionSerializer expressionSerializer, ICacheManager<object> cacheManager)
        {
            _repositoryFactory = repositoryFactory;
            _customPromotionManager = customPromotionManager;
            _expressionSerializer = expressionSerializer;
            _cacheManager = cacheManager;
        }

        #region IMarketingService Members

        public Promotion[] GetActivePromotions()
        {
            var retVal = new List<Promotion>(_customPromotionManager.Promotions);
            using (var repository = _repositoryFactory())
            {
                var dbStoredPromotions = repository.GetActivePromotions().Select(x => x.ToCoreModel(_expressionSerializer)).ToList();
                var promoComparer = AnonymousComparer.Create((Promotion x) => x.Id);
                dbStoredPromotions.Patch(retVal, promoComparer, (source, target) => target.InjectFrom(source));
            }
            return retVal.OrderBy(x => x.Priority).ToArray();
        }

        public Promotion GetPromotionById(string id)
        {
            Promotion retVal = null;
            using (var repository = _repositoryFactory())
            {
                var entity = repository.GetPromotionById(id);

                if (entity != null)
                {
                    retVal = entity.ToCoreModel(_expressionSerializer);
                }
            }

            return retVal ?? _customPromotionManager.Promotions.FirstOrDefault(x => x.Id == id);
        }

        public Promotion CreatePromotion(Promotion promotion)
        {
            var entity = promotion.ToDataModel();
            using (var repository = _repositoryFactory())
            {
                repository.Add(entity);
                CommitChanges(repository);
            }
            var retVal = GetPromotionById(entity.Id);
            _cacheManager.ClearRegion("MarketingModuleRegion");
            return retVal;
        }

        public void UpdatePromotions(Promotion[] promotions)
        {
            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                foreach (var promotion in promotions)
                {
                    var sourceEntity = promotion.ToDataModel();
                    var targetEntity = repository.GetPromotionById(promotion.Id);
                    if (targetEntity == null)
                    {
                        repository.Add(sourceEntity);
                    }
                    else
                    {
                        changeTracker.Attach(targetEntity);
                        sourceEntity.Patch(targetEntity);
                    }
                }
                CommitChanges(repository);
                _cacheManager.ClearRegion("MarketingModuleRegion");

            }
        }

        public void DeletePromotions(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                foreach (var id in ids)
                {
                    var entity = repository.GetPromotionById(id);
                    repository.Remove(entity);
                }
                CommitChanges(repository);
                _cacheManager.ClearRegion("MarketingModuleRegion");
            }
        }

        public Coupon GetCouponById(string id)
        {
            throw new NotImplementedException();
        }

        public Coupon[] GetPersonalCoupons(string customerId)
        {
            throw new NotImplementedException();
        }

        public Promotion CreateCoupon(Coupon coupon)
        {
            throw new NotImplementedException();
        }

        public void UpdateCoupons(Coupon[] coupons)
        {
            throw new NotImplementedException();
        }

        public void DeleteCoupons(string[] ids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
