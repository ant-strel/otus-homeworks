using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Otus.Teaching.Pcf.ReceivingFromPartner.Core.Domain;
using Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Models;
using RMQModels;

namespace Otus.Teaching.Pcf.ReceivingFromPartner.WebHost.Mappers
{
    public class PromoCodeMapper
    {
        public static PromoCode MapFromModel(ReceivingPromoCodeRequest request, Preference preference, Partner partner) {

            var promocode = new PromoCode();

            promocode.PartnerId = partner.Id;
            promocode.Partner = partner;
            promocode.Code = request.PromoCode;
            promocode.ServiceInfo = request.ServiceInfo;
           
            promocode.BeginDate = DateTime.Now;
            promocode.EndDate = DateTime.Now.AddDays(30);

            promocode.Preference = preference;
            promocode.PreferenceId = preference.Id;

            promocode.PartnerManagerId = request.PartnerManagerId;

            return promocode;
        }

        public static GivePromoCodeToCustomerDto MapGivePromocodeToCustomerFromModel(ReceivingPromoCodeRequest request, Preference preference, Partner partner)
        {

            var promocode = new GivePromoCodeToCustomerDto();

            promocode.PartnerId = partner.Id;
            promocode.Code = request.PromoCode;
            promocode.ServiceInfo = request.ServiceInfo;

            promocode.BeginDate = DateTime.Now.ToString();
            promocode.EndDate = DateTime.Now.AddDays(30).ToString();

            promocode.PreferenceId = preference.Id;

            promocode.PartnerManagerId = request.PartnerManagerId;

            return promocode;
        }
    }
}
