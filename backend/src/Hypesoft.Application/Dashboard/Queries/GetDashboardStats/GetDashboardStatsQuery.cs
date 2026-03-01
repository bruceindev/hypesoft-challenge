using MediatR;
using Hypesoft.Application.Common.Models;

namespace Hypesoft.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}
