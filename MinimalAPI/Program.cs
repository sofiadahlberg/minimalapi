using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//SQLite
builder.Services.AddDbContext<MusicDb>(options =>
options.UseSqlite("Data Source=MusicDb.db"));

var app = builder.Build();

// Routes and methods

// Get all posts
app.MapGet("music", async (MusicDb db) =>
await db.Musics.ToListAsync());

// Post
app.MapPost("/music", async (Music music, MusicDb db) =>
{
    db.Musics.Add(music);
    await db.SaveChangesAsync();

    return Results.Created("Music created:", music);
});

//Update
app.MapPut("music/{id}", async (int id, Music updatedMusic, MusicDb db) =>
{
    var MusicNow = await db.Musics.FindAsync(id);

    if (MusicNow == null)
    {
        return Results.NotFound($"Music with ID {id} not found.");
    }

    //Update properties
    MusicNow.Artist = updatedMusic.Artist;
    MusicNow.SongTitle = updatedMusic.SongTitle;
    MusicNow.Length = updatedMusic.Length;
    MusicNow.Category = updatedMusic.Category;

    await db.SaveChangesAsync();

    return Results.Ok($"Music with ID {id} is updated");
});

// Delete one post
app.MapDelete("music/{id}", async (int id, MusicDb db) =>
{
    var DeleteMusic = await db.Musics.FindAsync(id);
    if (DeleteMusic == null)
    {
        return Results.NotFound($"Song with ID {id} not found.");
    }
    db.Musics.Remove(DeleteMusic);
    await db.SaveChangesAsync();

    return Results.Ok($"Song with id {id} deleted");
});

app.MapGet("/", () => "Welcome to Music API");

app.UseHttpsRedirection();


app.Run();

//Music Class
class Music
{
    //Properties
    public int Id { get; set; }
    public string? Artist { get; set; }
    public string? SongTitle { get; set; }
    public int Length { get; set; }
    public string? Category { get; set; }

}

//DB context

class MusicDb : DbContext
{
    public MusicDb(DbContextOptions<MusicDb> options)
    : base(options)
    {
    }
    public DbSet<Music> Musics => Set<Music>();

}
