namespace App.Business.Concrete.Services
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _routeRepository;
        private readonly IAirportRepository _airportRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<MessageResources> _localizer;
        private readonly ILogger<RouteService> _logger;

        public RouteService(
            IRouteRepository routeRepository,
            IAirportRepository airportRepository,
            IUnitOfWork unitOfWork,
            IStringLocalizer<MessageResources> localizer,
            ILogger<RouteService> logger)
        {
            _routeRepository = routeRepository;
            _airportRepository = airportRepository;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IDataResult<RouteDto>> GetByIdAsync(Guid id)
        {
            var route = await _routeRepository.IncludeGetByIdAsync(id, tracking: false);
            if (route == null)
                return new ErrorDataResult<RouteDto>(_localizer[Messages.Route_Was_Not_Found]);

            return new SuccessDataResult<RouteDto>(route.Adapt<RouteDto>(), _localizer[Messages.Route_Was_Found]);
        }

        public async Task<IDataResult<IEnumerable<RouteDto>>> GetAllAsync()
        {
            var routes = await _routeRepository.GetAllAsync(tracking: false);
            return new SuccessDataResult<IEnumerable<RouteDto>>(routes.Select(x => x.Adapt<RouteDto>()));
        }

        public async Task<IDataResult<RouteDto>> AddAsync(RouteAddDto dto)
        {
            var exists = await _routeRepository.AnyAsync(r =>
                r.DepartureAirportId == dto.DepartureAirportId &&
                r.ArrivalAirportId == dto.ArrivalAirportId);

            if (exists)
                return new ErrorDataResult<RouteDto>(_localizer[Messages.Route_Already_Exists]);

            var route = new Route
            {
                DepartureAirportId = dto.DepartureAirportId,
                ArrivalAirportId = dto.ArrivalAirportId,
                DistanceKm = dto.DistanceKm,
                EstimatedDuration = dto.EstimatedDuration
            };

            await _routeRepository.AddAsync(route);
            await _unitOfWork.SaveChangesAsync();

            var saved = await _routeRepository.IncludeGetByIdAsync(route.Id, tracking: false);
            _logger.LogInformation("{Message} RouteId: {Id}", _localizer[Messages.Route_HasBeen_Added].Value, route.Id);
            return new SuccessDataResult<RouteDto>(saved.Adapt<RouteDto>(), _localizer[Messages.Route_HasBeen_Added]);
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var route = await _routeRepository.GetByIdAsync(id);
            if (route == null)
                return new ErrorResult(_localizer[Messages.Route_Was_Not_Found]);

            await _routeRepository.DeleteAsync(route);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("{Message} RouteId: {Id}", _localizer[Messages.Route_Was_Deleted].Value, id);
            return new SuccessResult(_localizer[Messages.Route_Was_Deleted]);
        }
    }
}




