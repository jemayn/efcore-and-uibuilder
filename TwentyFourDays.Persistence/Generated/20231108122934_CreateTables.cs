using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwentyFourDays.Persistence.Generated
{
    /// <inheritdoc />
    public partial class CreateTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "movie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    MainActor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movieGenre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movieGenre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "movieToMovieGenre",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "int", nullable: false),
                    MoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movieToMovieGenre", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_movieToMovieGenre_movieGenre_GenresId",
                        column: x => x.GenresId,
                        principalTable: "movieGenre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_movieToMovieGenre_movie_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_movieToMovieGenre_MoviesId",
                table: "movieToMovieGenre",
                column: "MoviesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movieToMovieGenre");

            migrationBuilder.DropTable(
                name: "movieGenre");

            migrationBuilder.DropTable(
                name: "movie");
        }
    }
}
