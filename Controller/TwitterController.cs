using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Microsoft.Extensions.Logging;

namespace TwitterDashboardBackend.Controllers
{
    [ApiController]
    [Route("api/twitter")]
    public class TwitterController : ControllerBase
    {
        private readonly ITwitterClient _twitterClient;
        private readonly ILogger<TwitterController> _logger;

        public TwitterController(ITwitterClient twitterClient, ILogger<TwitterController> logger)
        {
            _twitterClient = twitterClient ?? throw new ArgumentNullException(nameof(twitterClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("profile/{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            try
            {
                var user = await _twitterClient.Users.GetUserAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado");
                    return NotFound("Usuário não encontrado.");
                }

                return Ok(new
                {
                    Name = user.Name,
                    ScreenName = user.ScreenName,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Description = user.Description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil do usuário");
                return StatusCode(500, "Erro ao obter perfil do usuário");
            }
        }

        [HttpGet("timeline/{username}")]
        public async Task<IActionResult> GetTimeline(string username)
        {
            try
            {
                var user = await _twitterClient.Users.GetUserAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado");
                    return NotFound("Usuário não encontrado.");
                }

                var timelineParameters = new GetUserTimelineParameters(user)
                {
                    MaximumNumberOfTweetsToRetrieve = 20,
                    IncludeEntities = true
                };

                var tweets = await _twitterClient.Timelines.GetUserTimelineAsync(timelineParameters);
                if (tweets == null || !tweets.Any())
                {
                    _logger.LogInformation("Nenhum tweet encontrado para o usuário");
                    return NotFound("Nenhum tweet encontrado.");
                }

                var tweetList = tweets.Select(t => new
                {
                    Id = t.Id,
                    FullText = t.FullText,
                    CreatedAt = t.CreatedAt,
                    LikeCount = t.FavoriteCount,
                    RetweetCount = t.RetweetCount
                });

                return Ok(tweetList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter timeline do usuário");
                return StatusCode(500, "Erro ao obter timeline");
            }
        }

        [HttpGet("stats/{username}")]
        public async Task<IActionResult> GetStats(string username)
        {
            try
            {
                var user = await _twitterClient.Users.GetUserAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado");
                    return NotFound("Usuário não encontrado.");
                }

                return Ok(new
                {
                    followers = user.FollowersCount,
                    following = user.FriendsCount, // Verifique se é FriendsCount ou FollowingCount conforme a versão do Tweetinvi
                    tweets = user.StatusesCount,
                    likes = user.FavoritesCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do usuário");
                return StatusCode(500, "Erro ao obter estatísticas");
            }
        }
    }
}