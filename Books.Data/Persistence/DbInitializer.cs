using Books.Domain.Entities;
using Books.Interfaces;
using Books.Domain.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Books.Data.Persistence
{
#pragma warning disable
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationDbContext> _logger;
        private IConfiguration _configuration { get; }

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration
        )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public async void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                    _context.Database.Migrate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(Roles.RoleType.SuperAdmin.ToString()).GetAwaiter().GetResult())
            {

                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.SuperAdmin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Admin.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Employee.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Individual.ToString())).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Roles.RoleType.Company.ToString())).GetAwaiter().GetResult();


                //Create superadmin user.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["UserSettings:SuperAdmin:UserName"],
                    Email = _configuration["UserSettings:SuperAdmin:UserName"],
                    FirstName = "Khaled",
                    LastName = "Hamzi",
                    PhoneNumber = "+46752204569",
                    StreetAddress = "Falkgatan 44B",
                    State = "Sverige",
                    PostalCode = "65421",
                    City = "Motala"
                }, _configuration["UserSettings:SuperAdmin:Password"]).GetAwaiter().GetResult();

                ApplicationUser superAdmin = _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == _configuration["UserSettings:SuperAdmin:UserName"]).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(superAdmin, Roles.RoleType.SuperAdmin.ToString()).GetAwaiter().GetResult();

                //Create admin user.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["UserSettings:Admin:UserName"],
                    Email = _configuration["UserSettings:Admin:UserName"],
                    FirstName = "Khaled",
                    LastName = "Hamzi",
                    PhoneNumber = "+46752204569",
                    StreetAddress = "Falkgatan 44C",
                    State = "Sverige",
                    PostalCode = "65421",
                    City = "Motala"
                }, _configuration["UserSettings:Admin:Password"]).GetAwaiter().GetResult();

                ApplicationUser admin = _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == _configuration["UserSettings:Admin:UserName"]).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(admin, Roles.RoleType.Admin.ToString()).GetAwaiter().GetResult();

            }

            // Create covers
            if (!_context.Covers.Any())
            {
                _context.Covers.AddRange(new List<Cover>()
                    {
                        new Cover(){Name = "Paper Cover"},
                        new Cover(){Name = "Hard Cover"},
                        new Cover(){Name = "Plastic Cover"}
                    });
                _context.SaveChanges();
            }

            // Create covers
            if (!_context.Covers.Any())
            {
                _context.Covers.AddRange(new List<Cover>()
                    {
                        new Cover(){Name = "Paper Cover"},
                        new Cover(){Name = "Hard Cover"},
                        new Cover(){Name = "Plastic Cover"}
                    });
                _context.SaveChanges();
            }

            // Create categories
            if (!_context.Categories.Any())
            {
                _context.Categories.AddRange(new List<Category>()
                    {
                        new Category(){Name = "Paper Cover", DisplayOrder = 2, CreatedDateTime = DateTime.Parse("2022-01-15")},
                        new Category(){Name = "Romance", DisplayOrder = 2, CreatedDateTime = DateTime.Parse("2022-01-15")},
                        new Category(){Name = "Detective", DisplayOrder = 2, CreatedDateTime = DateTime.Parse("2022-01-16")},
                        new Category(){Name = "Tragedy", DisplayOrder = 1, CreatedDateTime = DateTime.Parse("2022-01-17")},
                        new Category(){Name = "Testing & Engineering", DisplayOrder = 6, CreatedDateTime = DateTime.Parse("2022-01-18")},
                        new Category(){Name = "Psychology & Counseling", DisplayOrder = 1, CreatedDateTime = DateTime.Parse("2022-01-19")}
                    });
                _context.SaveChanges();
            }

            // Create authors
            if (!_context.Authors.Any())
            {
                _context.Authors.AddRange(new List<Author>()
                    {
                        new Author(){FullName = "Annmarie Palm"},
                        new Author(){FullName = "Jordan B. Peterson"},
                        new Author(){FullName = "David J Anderson"},
                        new Author(){FullName = "Laurelli Rolf"},
                        new Author(){FullName = "Bo Gustafsson"},
                        new Author(){FullName = "William Shakespeare"},
                        new Author(){FullName = "Robert C. Martin"},
                        new Author(){FullName = "Stephen Denning"},
                        new Author(){FullName = "Dale Carnegie"},
                        new Author(){FullName = "Brian Tracy"},
                        new Author(){FullName = "Rashina Hoda"},
                        new Author(){FullName = "Gustafsson, Bo"},
                        new Author(){FullName = "Geoff Watts"},
                    });
                _context.SaveChanges();
            }

            // Create products
            if (!_context.Products.Any())
            {
                _context.Products.AddRange(new List<Product>()
                    {
                        new Product()
                        {
                            Title = "12 Rules for Life",
                            ISBN = "9780345816023",
                            Description = "<p>12 Rules for Life offers a deeply rewarding antidote to the chaos in our lives: eternal truths applied to our modern problems.</p>",
                            AuthorId = 2,
                            Price = 120,
                            ImageUrl = "\\images\\products\\e4dc4b00-c609-4e00-ae38-153e3ef835c3.jpg",
                            CategoryId = 6,
                            CoverId = 1,
                            InStock = 10
                        },
                        new Product()
                        {
                            Title = "Negotiation",
                            ISBN = "9780814433195",
                            Description = "<p><em>Tracy</em> teaches readers how to utilize the six key negotiating styles.</p>",
                            AuthorId = 10,
                            Price = 100,
                            ImageUrl = "\\images\\products\\4dca90aa-7e9c-4756-bf6b-c020a1728d9b.jpg",
                            CategoryId = 1,
                            CoverId = 3,
                            InStock = 0
                        },
                        new Product()
                        {
                            Title = "King Lear",
                            ISBN = "9780141012292",
                            Description = "<p>King Lear is a tragedy written by <em>William Shakespeare</em>. It depicts the gradual descent into madness of the title character, after he disposes of his kingdom by giving bequests to two of his three daughters egged on by their continual flattery, bringing tragic consequences for all.</p>",
                            AuthorId = 6,
                            Price = 125,
                            ImageUrl = "\\images\\products\\6e16025d-951a-492d-86c3-5936c2d910fc.jpg",
                            CategoryId = 4,
                            CoverId = 2,
                            InStock = 13
                        },
                        new Product()
                        {
                            Title = "Kanban",
                            ISBN = "9780984521401",
                            Description = "<p>Optimize the effectiveness of your business, to produce fit-for-purpose products and services that delight your customers, making them loyal to your brand and increasing your share, revenues and margins.</p>",
                            AuthorId = 3,
                            Price = 95,
                            ImageUrl = "\\images\\products\\6626ca37-6e94-4a28-ae57-742ba0ec23e6.jpg",
                            CategoryId = 5,
                            CoverId = 2,
                            InStock = 10

                        },
                        new Product()
                        {
                            Title = "Othello",
                             ISBN = "1853260185",
                            Description = "<p>An intense drama of love, deception, jealousy and destruction.</p>",
                            AuthorId = 6,
                            Price = 160,
                            ImageUrl = "\\images\\products\\1d258c55-efe4-4915-a293-9d918ce3508a.jpg",
                            CategoryId = 4,
                            CoverId = 2,
                            InStock = 13
                        },
                        new Product()
                        {
                            Title = "How to Win Friends and Influence People",
                             ISBN = "9781439199190",
                            Description = "<p>Dale Carnegie had an understanding of human nature that will never be outdated. Financial success, Carnegie believed, is due 15 percent to professional knowledge and 85 percent to the ability to express ideas, to assume leadership, and to arouse enthusiasm among people.</p>",
                            AuthorId = 8,
                            Price = 110,
                            ImageUrl = "\\images\\products\\39d1dac4-e09d-4d29-9af2-45696c5e23c3.jpg",
                            CategoryId = 1,
                            CoverId = 2,
                            InStock = 11
                        },
                        new Product()
                        {
                            Title = "Hamlet",
                             ISBN = "9789170379673",
                            Description = "<p>The Tragedy of Hamlet, Prince of Denmark, often shortened to Hamlet, is a tragedy written by William Shakespeare sometime between 1599 and 1601. It is Shakespeare's longest play, with 29,551 words.</p>",
                            AuthorId = 6,
                            Price = 175,
                            ImageUrl = "\\images\\products\\a70543f0-9cb5-4223-9c03-a16464780549.jpg",
                            CategoryId = 4,
                            CoverId = 2,
                            InStock = 8
                        },
                        new Product()
                        {
                            Title = "Affärsmannaskap",
                            ISBN = "9789147107483",
                            Description = "<p>Aff&auml;rsmannaskap f&ouml;r ingenj&ouml;rer, jurister och alla andra specialister: I Aff&auml;rsmannaskap har Rolf Laurelli summerat sin l&aring;nga erfarenhet av konsten att g&ouml;ra aff&auml;rer. Med boken hoppas han kunna locka fram dina aff&auml;rsinstinkter.</p>",
                            AuthorId = 4,
                            Price = 179,
                            ImageUrl = "\\images\\products\\b6dfd088-1c3d-4066-9216-bdc812bf6ad8.jpg",
                            CategoryId = 1,
                            CoverId = 2,
                            InStock = 11
                        },
                        new Product()
                        {
                            Title = "THE AGE OF AGILE",
                            ISBN = "9780814439098",
                            Description = "The Age of Agile helps readers master the three laws of Agile Management (team, customer, network).",
                            AuthorId = 8,
                            Price = 190,
                            ImageUrl = "\\images\\products\\19087cb4-a4c5-4915-841b-ba30c8207ac7.jpg",
                            CategoryId = 5,
                            CoverId = 1,
                            InStock = 11
                        },
                        new Product()
                        {
                            Title = "Scrum Mastery",
                             ISBN = "9780957587403",
                            Description = "<p>The basics of being a <em>ScrumMaster </em>are fairly straightforward: Facilitate the Scrum process and remove impediments.</p>",
                            AuthorId = 13,
                            Price = 220,
                            ImageUrl = "\\images\\products\\426c97ee-2d24-401c-9c12-1c8dca5b1317.jpg",
                            CategoryId = 5,
                            CoverId = 1,
                            InStock = 19
                        },
                        new Product()
                        {
                            Title = "Förhandla",
                            ISBN = "9789186293321",
                            Description = "I Affärsmannaskap har Rolf Laurelli summerat sin långa erfarenhet av konsten att göra affärer. Med boken hoppas han kunna locka fram dina affärsinstinkter.",
                            AuthorId = 12,
                            Price = 120,
                            ImageUrl = "\\images\\products\\b088401b-06ec-4248-bdfe-a1aa4276debe.jpg",
                            CategoryId = 1,
                            CoverId = 2,
                            InStock = 10
                        },
                        new Product()

                        {
                            Title = "Agile Processes in Software Engineering",
                            ISBN = "9783030301255",
                            Description = "<p>This book constitutes the research workshops, doctoral symposium and panel summaries presented at the 20th International Conference on Agile Software Development.</p>",
                            AuthorId = 11,
                            Price = 130,
                            ImageUrl = "\\images\\products\\8083c302-e674-4936-b2fb-6e1b9687ed16.jpg",
                            CategoryId = 5,
                            CoverId = 1,
                            InStock = 22
                        },
                    });
                _context.SaveChanges();
            }
            return;
        }
    }
}
