using MediatR;
using Temple.Application.Core;

namespace Temple.Application.Landing;

public class Get
{
    public class Query : IRequest<Result<LandingDto>>
    {
    }

    public class Handler : IRequestHandler<Query, Result<LandingDto>>
    {
        public async Task<Result<LandingDto>> Handle(
            Query request,
            CancellationToken cancellationToken)
        {
            var result = new LandingDto
            {
                Bamse = "Bamse",
                Kylling = "Kylling"
            };

            return Result<LandingDto>.Success(result);
        }
    }
}