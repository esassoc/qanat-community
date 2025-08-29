using System.Text.Json;
using Qanat.Models.DataTransferObjects;

namespace Qanat.PDFGenerator.Services
{
    public class VegaSpecUtilities
    {
        public static string GetMonthlyUsageChartVegaSpec(List<MonthlyUsageChartDataDto> chartDtos, int year)
        {
            var vegaSpec = $@"{{
                $schema: ""https://vega.github.io/schema/vega-lite/v5.json"",
                description: ""Monthly Groundwater Usage Bar Chart"",
                data:
                {{
                    name: ""cumulativeWaterUsages"",
                    values: { JsonSerializer.Serialize(chartDtos) },
                }},
                width: 700,
                height: 250,
                mark: {{ type: ""bar"", width: 22 }},
                encoding:
                {{
                    x:
                    {{
                        field: ""EffectiveDate"",
                        type: ""nominal"",
                        timeUnit: ""utcyearmonth"",
                        title: ""{year} Groundwater Usage"",
                        axis: {{ 
                            labelFontSize: 12, 
                            titleFontSize: 14, 
                            labelAngle: -30, 
                            titlePadding: 10 
                        }},
                    }},
                    y: 
                    {{ 
                        field: ""Value"", 
                        type: ""quantitative"", 
                        title: ""Volume (ac-ft)"", 
                        axis: {{ labelFontSize: 12, titleFontSize: 12 }},
                    }},
                    color: {{ value: ""#71C9E9"" }}
                }}

            }}";

            return vegaSpec;
        }
    }
}