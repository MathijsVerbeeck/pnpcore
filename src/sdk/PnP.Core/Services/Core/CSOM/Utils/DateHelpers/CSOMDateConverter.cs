﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PnP.Core.Services.Core.CSOM.Utils.DateHelpers
{
    class CSOMDateConverter : IDateConversionStrategy
    {
        public List<IDateConversionStrategy> AvailableConverters { get; set; } = new List<IDateConversionStrategy>()
        {
            new DateConstuctorStrategy(),
            new FromMilisecondsConversionStrategy()
        };
        public DateTime? ConverDate(string dateValue)
        {
            DateTime? result = null;
            foreach(IDateConversionStrategy strategy in AvailableConverters)
            {
                result = strategy.ConverDate(dateValue);
                if (result.HasValue)
                    return result;
            }

            return result;
        }
    }
}
