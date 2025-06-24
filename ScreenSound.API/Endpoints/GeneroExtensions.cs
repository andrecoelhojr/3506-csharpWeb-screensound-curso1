using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Modelos.Modelos;

namespace ScreenSound.API.Endpoints;

public static class GeneroExtensions
{
    public static void AddEndPointsGeneros(this WebApplication app)
    {
        app.MapGet("/Generos", ([FromServices] DAL<Genero> dal) =>
        {
            var generoList = dal.Listar();
            if (generoList is null)
            {
                return Results.NotFound("Gêneros não encontrados.");
            }
            var generoListResponse = EntityListToResponseList(generoList);
            return Results.Ok(generoListResponse);
        });

        app.MapGet("/Generos/{nome}", ([FromServices] DAL<Genero> dal, string nome) =>
        {
            var genero = dal.RecuperarPor(a => a.Nome.ToLower().Equals(nome.ToLower()));
            if (genero is null)
            {
                return Results.NotFound("Gênero não encontrado.");
            }

            return Results.Ok(EntityToResponse(genero));
        });

        app.MapPost("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest) =>
        {
            dal.Adicionar(RequestToEntity(generoRequest));
            return Results.Ok();
        });

        app.MapDelete("/Generos/{id}", ([FromServices] DAL<Genero> dal, int id) =>
        {
            var genero = dal.RecuperarPor(g => g.Id == id);
            if (genero is null)
            {
                Results.NotFound("Gênero para exclusão não encontrado.");
            }
            dal.Deletar(genero);
            return Results.NoContent();
        });

        app.MapPut("/Generos", ([FromServices] DAL<Genero> dal, [FromBody] GeneroRequest generoRequest ) =>
        {
            var updateGenero = dal.RecuperarPor(g => g.Id == generoRequest.Id);
            if (updateGenero is null)
            {
                Results.NotFound();
            }

            updateGenero.Nome = generoRequest.Nome;
            updateGenero.Descricao = generoRequest.Descricao;
            dal.Atualizar(updateGenero);
            return Results.Ok();

        });
    }
    private static ICollection<GeneroResponse> EntityListToResponseList(IEnumerable<Genero> generoList)
    {
        return generoList.Select(g => EntityToResponse(g)).ToList();
    }

    private static GeneroResponse EntityToResponse(Genero genero)
    {
        return new GeneroResponse(genero.Id, genero.Nome!, genero.Descricao);
    }
    private static Genero RequestToEntity(GeneroRequest generoRequest)
    {
        return new Genero() { Nome = generoRequest.Nome, Descricao = generoRequest.Descricao };
    }
}
