using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Skinet.Core.Entites;
using Skinet.Core.Interfaces;
using Stripe;

namespace Skinet.Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        public CouponService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }
        public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
        {
            var promotionService = new PromotionCodeService();
            var options = new PromotionCodeListOptions
            {
                Code = code
            };
            var promotionCodes = await promotionService.ListAsync(options);
            var promotionCode = promotionCodes.FirstOrDefault();
            if (promotionCode != null && promotionCode.Coupon != null)
            {
                return new AppCoupon
                {
                    Name = promotionCode.Coupon.Name,
                    AmountOff = promotionCode.Coupon.AmountOff,
                    PercentOff = promotionCode.Coupon.PercentOff,
                    CouponId = promotionCode.Coupon.Id,
                    PromotionCode = promotionCode.Code
                };
            }
            return null;
        }
    }
}