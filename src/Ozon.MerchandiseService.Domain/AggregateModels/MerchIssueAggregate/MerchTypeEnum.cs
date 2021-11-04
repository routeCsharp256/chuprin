﻿using Ozon.MerchandiseService.Domain.SeedWork;
using CommonEnums = CSharpCourse.Core.Lib.Enums;

namespace Ozon.MerchandiseService.Domain.AggregateModels.MerchIssueAggregate
{
    public class MerchTypeEnum: Enumeration
    {
        public static MerchTypeEnum WelcomePack = new MerchTypeEnum(1, nameof(CommonEnums.MerchType.WelcomePack));
        public static MerchTypeEnum VeteranPack = new MerchTypeEnum(2, nameof(CommonEnums.MerchType.VeteranPack));
        public static MerchTypeEnum ConferenceSpeakerPack = new MerchTypeEnum(3, nameof(CommonEnums.MerchType.ConferenceSpeakerPack));
        public static MerchTypeEnum ConferenceListenerPack = new MerchTypeEnum(4, nameof(CommonEnums.MerchType.ConferenceListenerPack));
        public static MerchTypeEnum ProbationPeriodEndingPack = new MerchTypeEnum(5, nameof(CommonEnums.MerchType.ProbationPeriodEndingPack));

        public MerchTypeEnum(int id, string name) : base(id, name)
        {
        }
    }
}