using Newtonsoft.Json;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Commons;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Exceptions;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Extensions;

public static class CollectionExtension
{
    public static IQueryable<T> ToPagedList<T>(this IQueryable<T> sources,
        PaginationParams @params = null) where T : Auditable
    {

        var metaData = new PaginationMetaData(sources.Count(), @params);

        var json = JsonConvert.SerializeObject(metaData);

        if (HttpContextHelper.ResponseHeaders != null)
        {
            HttpContextHelper.ResponseHeaders.Remove("Pagination");
            HttpContextHelper.ResponseHeaders.Add("Pagination", json);
            HttpContextHelper.ResponseHeaders.Add("Access-Control-Expose-Headers", "Pagination");
        }

        return @params.PageIndex > 0 && @params.PageSize > 0 ?
            sources.OrderByDescending(p => p.CreatedAt)
                .Skip((@params.PageIndex - 1) * @params.PageSize).Take(@params.PageSize) :
            throw new CustomException(405, "Please, enter valid numbers");
    }
}