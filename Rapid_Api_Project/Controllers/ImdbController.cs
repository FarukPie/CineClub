using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rapid_Api_Project.Models;


namespace Rapid_Api_Project.Controllers
{
    public class ImdbController : Controller
    {
        private readonly IConfiguration _configuration;
        private static List<ImdbViewModel.Movie> _cachedMovies = null;

        public ImdbController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<List<ImdbViewModel.Movie>> GetAllMoviesAsync()
        {
            if (_cachedMovies != null && _cachedMovies.Count > 0)
                return _cachedMovies;

            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://imdb236.p.rapidapi.com/api/imdb/top250-movies"),
                Headers =
                {
                    { "x-rapidapi-key", _configuration["RapidApiSettings:Key"] },
                    { "x-rapidapi-host", _configuration["RapidApiSettings:Host"] },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                if (!response.IsSuccessStatusCode)
                    return new List<ImdbViewModel.Movie>();

                var jsonBody = await response.Content.ReadAsStringAsync();
                _cachedMovies = JsonConvert.DeserializeObject<List<ImdbViewModel.Movie>>(jsonBody);
                return _cachedMovies ?? new List<ImdbViewModel.Movie>();
            }
        }

        public async Task<IActionResult> Index(string genre = null, int page = 1)
        {
            var movies = await GetAllMoviesAsync();

            if (movies == null || movies.Count == 0)
                return Content("API Error: Film listesi alınamadı");

            // Tür filtreleme
            if (!string.IsNullOrEmpty(genre) && genre.ToLower() != "all")
            {
                movies = movies.Where(x => x.genres != null && x.genres.Contains(genre)).ToList();
            }

            // Sayfalama
            int pageSize = 12;
            int totalMovies = movies.Count;
            int totalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            
            // Sayfa sınırlarını kontrol et
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var paginatedMovies = movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentGenre = genre ?? "all";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalMovies = totalMovies;
            
            return View(paginatedMovies);
        }

        [HttpGet]
        public async Task<IActionResult> GetRandomMovie()
        {
            var movies = await GetAllMoviesAsync();

            if (movies == null || movies.Count == 0)
                return Json(new { success = false, message = "Film bulunamadı" });

            var random = new Random();
            var randomMovie = movies[random.Next(movies.Count)];

            return Json(new
            {
                success = true,
                movie = new
                {
                    id = randomMovie.id,
                    title = randomMovie.originalTitle,
                    year = randomMovie.startYear,
                    rating = randomMovie.averageRating,
                    image = randomMovie.primaryImage,
                    description = randomMovie.description,
                    genres = randomMovie.genres,
                    runtime = randomMovie.runtimeMinutes,
                    url = randomMovie.url
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new { success = false, message = "Arama terimi çok kısa" });

            var movies = await GetAllMoviesAsync();

            if (movies == null || movies.Count == 0)
                return Json(new { success = false, message = "Film listesi yüklenemedi" });

            var searchResults = movies
                .Where(m => m.originalTitle != null && 
                           m.originalTitle.ToLower().Contains(query.ToLower()))
                .Take(8)
                .Select(m => new
                {
                    id = m.id,
                    title = m.originalTitle,
                    year = m.startYear,
                    rating = m.averageRating,
                    image = m.primaryImage,
                    description = m.description,
                    genres = m.genres,
                    runtime = m.runtimeMinutes,
                    url = m.url
                })
                .ToList();

            return Json(new { success = true, movies = searchResults });
        }

        [HttpPost]
        public async Task<IActionResult> ChatRecommendation([FromBody] ChatRequest request)
        {
            var movies = await GetAllMoviesAsync();

            if (movies == null || movies.Count == 0)
                return Json(new { success = true, message = "Film listesi yüklenemedi 😔", movies = new List<object>() });

            var message = request.Message?.ToLower() ?? "";
            var random = new Random();
            ImdbViewModel.Movie selectedMovie = null;
            string genreEmoji = "🎬";
            string genreName = "";

            // Anahtar kelime bazlı öneri sistemi
            if (message.Contains("korku") || message.Contains("horror") || message.Contains("korkutucu") || message.Contains("gerilim") || message.Contains("thriller"))
            {
                var genreMovies = movies.Where(m => m.genres != null && 
                    (m.genres.Contains("Horror") || m.genres.Contains("Thriller"))).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🎃";
                genreName = "korku/gerilim";
            }
            else if (message.Contains("komedi") || message.Contains("comedy") || message.Contains("güldür") || message.Contains("eğlenceli") || message.Contains("komik"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Comedy")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "😂";
                genreName = "komedi";
            }
            else if (message.Contains("romantik") || message.Contains("romance") || message.Contains("aşk") || message.Contains("romantizm"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Romance")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "💕";
                genreName = "romantik";
            }
            else if (message.Contains("aksiyon") || message.Contains("action") || message.Contains("heyecan") || message.Contains("dövüş"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Action")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "💥";
                genreName = "aksiyon";
            }
            else if (message.Contains("bilim kurgu") || message.Contains("sci-fi") || message.Contains("uzay") || message.Contains("bilimkurgu"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Sci-Fi")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🚀";
                genreName = "bilim kurgu";
            }
            else if (message.Contains("drama") || message.Contains("duygusal") || message.Contains("dramatik"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Drama")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🎭";
                genreName = "drama";
            }
            else if (message.Contains("suç") || message.Contains("crime") || message.Contains("mafya") || message.Contains("gangster"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Crime")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🔫";
                genreName = "suç";
            }
            else if (message.Contains("animasyon") || message.Contains("animation") || message.Contains("çizgi") || message.Contains("anime"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Animation")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🎨";
                genreName = "animasyon";
            }
            else if (message.Contains("macera") || message.Contains("adventure"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("Adventure")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "🗺️";
                genreName = "macera";
            }
            else if (message.Contains("savaş") || message.Contains("war"))
            {
                var genreMovies = movies.Where(m => m.genres != null && m.genres.Contains("War")).ToList();
                if (genreMovies.Count > 0)
                    selectedMovie = genreMovies[random.Next(genreMovies.Count)];
                genreEmoji = "⚔️";
                genreName = "savaş";
            }
            else if (message.Contains("en iyi") || message.Contains("top") || message.Contains("popüler") || message.Contains("en iyiler"))
            {
                selectedMovie = movies.First();
                genreEmoji = "🏆";
                genreName = "en iyi";
            }
            else if (message.Contains("rastgele") || message.Contains("sürpriz") || message.Contains("random") || message.Contains("bir film") || message.Contains("film öner") || message.Contains("öneri"))
            {
                selectedMovie = movies[random.Next(movies.Count)];
                genreEmoji = "🎲";
                genreName = "rastgele";
            }
            else if (message.Contains("merhaba") || message.Contains("selam") || message.Contains("hey"))
            {
                return Json(new
                {
                    success = true,
                    message = "Merhaba! 👋 Ben film öneri botuyum.\n\nBana bir tür söyle, sana film önereyim!\n\nÖrneğin:\n• \"Korku filmi öner\"\n• \"Komedi istiyorum\"\n• \"Aksiyon ver\"\n• \"Rastgele bir film\"",
                    movies = new List<object>()
                });
            }
            else
            {
                // Bilinmeyen mesaj - varsayılan olarak rastgele film öner
                selectedMovie = movies[random.Next(movies.Count)];
                genreEmoji = "🎬";
                genreName = "rastgele";
            }

            // Eğer hiç film bulunamadıysa rastgele seç
            if (selectedMovie == null)
            {
                selectedMovie = movies[random.Next(movies.Count)];
            }

            // Film bilgisini mesajda göster
            var responseMessage = $"{genreEmoji} Sana önerim:\n\n" +
                                  $"🎬 {selectedMovie.originalTitle}\n" +
                                  $"📅 {selectedMovie.startYear} | ⭐ {selectedMovie.averageRating:0.0}";

            return Json(new
            {
                success = true,
                message = responseMessage,
                movies = new List<object>()
            });
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}